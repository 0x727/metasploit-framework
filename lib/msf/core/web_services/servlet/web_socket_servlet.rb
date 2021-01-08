require 'msf/core/rpc'
require 'faye/websocket'
require 'rex/text/color'
include Rex::Text::Color

def supports_color?
  true
end

module Msf::WebServices
  module WebsocketServlet
    Faye::WebSocket.load_adapter('thin')
    def self.api_path
      '/api/v1/websocket'
    end

    def self.api_path_for_notify
      "#{WebsocketServlet.api_path}/notify"
    end

    def self.api_path_for_console
      "#{WebsocketServlet.api_path}/console"
    end

    def self.registered(app)
      app.get WebsocketServlet.api_path_for_notify, &notify
      app.get WebsocketServlet.api_path_for_console, &console
    end

    def self.notify
      lambda {
        warden.authenticate!
        if Faye::WebSocket.websocket?(env)
          ws = Faye::WebSocket.new(env, nil, { ping: 15 })
          server_name = ws.env['SERVER_NAME']
          ws.on :open do |_event|
            framework.websocket.register(:notify, ws)
            data = framework.websocket.wrap_websocket_data(:notify, 'login', { server_name: server_name })
            framework.websocket.notify(:notify, data)
          end

          ws.on :close do |_event|
            framework.websocket.deregister(:notify, ws)
            data = framework.websocket.wrap_websocket_data(:notify, 'logout', { server_name: server_name })
            framework.websocket.notify(:notify, data)
            ws = nil
          end

          ws.on :message do |event|
            begin
              ws_data = JSON.parse(event.data)
              framework.websocket.notify(:notify, ws_data.to_json)
            rescue JSON::ParserError => e
              data = framework.websocket.wrap_websocket_data(:notify, 'error', { error: e.to_s })
              ws.send(data)
            end
          end
          ws.rack_response
        else
          [200, { 'Content-Type' => 'application/json' }, ['Error']]
        end
      }
    end

    def self.console
      lambda {
        warden.authenticate!
        if Faye::WebSocket.websocket?(env)
          ws = Faye::WebSocket.new(env, nil, { ping: 15 })
          ws.on :open do |_event|
            framework.websocket.register(:console, ws)
            opts = {:framework => framework, :DisableBanner=>true}
            @console_driver = Msf::Ui::Web::DriverFactory.instance.get_or_create(opts=opts)
            @cid = @console_driver.create_console(opts)
            # send welcome message
            prompt = @console_driver.consoles[@cid].console.update_prompt
            content_color = @console_driver.consoles[@cid].read
            start_msg = {
              'cid'    => @cid,
              'data'   => Rex::Text.encode_base64(content_color)   || '',
              'prompt' => @console_driver.consoles[@cid].prompt || '',
            }
            ws.send(start_msg.to_json)
            # create separated subscriber
            @sub_id = "ws_#{@cid}"
            @console_driver.consoles[@cid].pipe.create_subscriber_proc(
              @sub_id, &proc { |output|
                prompt = @console_driver.consoles[@cid].console.update_prompt
                if (@console_driver.consoles[@cid].console.active_session)
                  prompt = @console_driver.consoles[@cid].console.update_prompt('%undmeterpreter%clr')
                end
                content_color = substitute_colors(output)
                data = {
                  'cid'    => @cid,
                  'prompt' => prompt || '',
                  'data'   => Rex::Text.encode_base64(content_color) || ''
                }
                ws.send(data.to_json)
              }
            )
          end

          ws.on :close do |_event|
            framework.websocket.deregister(:console, ws)
            @console_driver.destroy_console(@cid)
            dlog("destroy_console #{@cid}")
            ws = nil
          end

          ws.on :message do |event|
            input = event.data
            @console_driver.write_console(@cid, input)
          end
          ws.rack_response
        else
          [200, { 'Content-Type' => 'application/json' }, ['Error']]
        end
      }
    end
  end
end