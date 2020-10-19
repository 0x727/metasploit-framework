# -*- coding: UTF-8 -*-

class MetasploitModule < Msf::Post

    # include Msf::Post::File
    # include Msf::Auxiliary::AuthBrute
  
    def initialize(info = {})
        super(
            update_info(
                info,
                'Name' => '端口扫描模块',
                'Description' => %q{
                本模块调用execute_dotnet_assembly模块在内存中执行端口扫描程序
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
                # OptPath.new('DOTNET_EXE', [false, '.net程序路径']),
                # OptString.new('ARGUMENTS', [false, '命令行参数（必须为字符串）']),
                OptString.new('PROCESS', [false, '准备启动的进程名称', 'notepad.exe']),
                OptInt.new('PID', [false, '注入到的Pid', 0]),
                OptInt.new('PPID', [false, '创建新进程时用于 PPID spoofing 的进程标识符。 (0 = no PPID spoofing)', 0]),
                OptInt.new('TIMEOUT', [false, '等待返回结果的超时时间（单位：秒）', 600]),

                OptString.new('RHOST', [true, '目标主机地址，格式示例：192.168.1.1,192.168.1.1-192.168.200.1,192.168.0.0/16']),
                OptString.new('RPORT', [true, '目标端口']),
                OptInt.new('THREADS', [false, '最大并行线程数量，默认为50']),
                OptBool.new('KILL', [true, '任务结束之后关闭进程', false]),
           ], self.class
        )
  
        register_advanced_options(
            [
                OptInt.new('WAIT', [false, '进程运行后等待其执行的时间（单位：秒）', 10]),
                OptInt.new('CONNECT_TIMEOUT', [false, '连接的超时时间（单位：秒），默认为5秒']),
                OptInt.new('TTL', [false, 'PING的TTL次数']),
                OptBool.new('AMSIBYPASS', [true, 'Enable Amsi bypass', true]),
                OptBool.new('ETWBYPASS', [true, 'Enable Etw bypass', true]),
                OptString.new('USETHREADTOKEN', [false, '使用线程模拟生成进程', true]),
            ]
        )
    end
  
    def run

        port_exe_path = ::File.join(Msf::Config.data_directory, 'post', 'scanner', 'port.exe')
        port_exe_path = ::File.expand_path(port_exe_path)
        datastore['DOTNET_EXE'] = port_exe_path
        vprint_status("DOTNET_EXE: #{port_exe_path}")

        datastore['ARGUMENTS'] = ''
        # datastore['USETHREADTOKEN'] = true
        # datastore['AMSIBYPASS'] = true
        # datastore['ETWBYPASS'] = true
 
        unless File.file?(port_exe_path)
            fail_with(Failure::BadConfig, "错误：没有找到.net运行程序#{port_exe_path}！")
        end

        if datastore['THREADS'] != nil and datastore['THREADS'].positive? and datastore['THREADS'] <= 20000
            datastore['ARGUMENTS'] += ' -t ' + datastore['THREADS'].toString()
        end

        if datastore['CONNECT_TIMEOUT'] != nil and datastore['CONNECT_TIMEOUT'].positive? and datastore['CONNECT_TIMEOUT'] <= 300
            datastore['ARGUMENTS'] += ' -o ' + datastore['CONNECT_TIMEOUT'].toString()
        end

        if datastore['TTL'] != nil and datastore['TTL'].positive? and datastore['TTL'] <= 256
            datastore['ARGUMENTS'] += ' -l ' + datastore['TTL'].toString()
        end

        if datastore['RHOST'] != nil and datastore['RHOST'].length > 0
            datastore['ARGUMENTS'] += ' -h ' + datastore['RHOST']
        else
            fail_with(Failure::BadConfig, "错误：目标设置不正确！")
        end

        if datastore['RPORT'] != nil and datastore['RHOST'].length > 0
            datastore['ARGUMENTS'] += ' -p ' + datastore['RPORT']
        else
            fail_with(Failure::BadConfig, "错误：端口#{datastore['RPORT']}设置不正确！")
        end

        datastore['ARGUMENTS'] = datastore['ARGUMENTS'].lstrip
        vprint_status("ARGUMENTS: #{datastore['ARGUMENTS']}")
        
        execute_dotnet_assembly = framework.modules.create('post/windows/manage/execute_dotnet_assembly')
        
        result = execute_dotnet_assembly.run_simple(
        'LocalInput'  => user_input,
        'LocalOutput' => user_output,
        'Options'     => datastore
        )
    end
end
  