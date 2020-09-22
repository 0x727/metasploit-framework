# -*- coding: binary -*-

require 'msf/core'

module Msf
  module WS
    class EventNotify
      class Subscriber
        include Framework::Offspring
        def initialize(framework)
          self.framework = framework
        end

        def respond_to?(_name, *_args)
          # Why yes, I can do that.
          true
        end

        def on_session_open(session)
          res = {
            'type'         => session.type.to_s,
            'tunnel_local' => session.tunnel_local.to_s,
            'tunnel_peer'  => session.tunnel_peer.to_s,
            'via_exploit'  => session.via_exploit.to_s,
            'via_payload'  => session.via_payload.to_s,
            'desc'         => session.desc.to_s,
            'info'         => session.info.to_s,
            'workspace'    => session.workspace.to_s,
            'session_host' => session.session_host.to_s,
            'session_port' => session.session_port.to_i,
            'target_host'  => session.target_host.to_s,
            'username'     => session.username.to_s,
            'uuid'         => session.uuid.to_s,
            'exploit_uuid' => session.exploit_uuid.to_s,
            'routes'       => session.routes.join(","),
            'arch'         => session.arch.to_s
          }
          if (session.type.to_s == 'meterpreter')
            res['platform'] = session.platform.to_s
          end
          data = framework.websocket.wrap_websocket_data(:notify, __method__, res)
          framework.websocket.notify(:notify, data)
        end

        def on_session_close(session, reason = '')
          res = {
            'sid'         => session.sid,
            'type'        => session.type.to_s,
            'reason'      => reason.to_s,
            'tunnel_to_s' => session.tunnel_to_s,
            'via_exploit' => session.via_exploit.to_s,
            'via_payload' => session.via_payload.to_s
          }
          if (session.type.to_s == 'meterpreter')
            res['platform'] = session.platform.to_s
          end
          data = framework.websocket.wrap_websocket_data(:notify, __method__, res)
          framework.websocket.notify(:notify, data)
        end

        def on_session_output(session, output)
          output = Rex::Text.encode_base64(output)
          res = {
            'sid'  => session.sid,
            'output' => output
          }
          data = framework.websocket.wrap_websocket_data(:notify, __method__, res)
          framework.websocket.notify(:notify, data)
        end

        def method_missing(_method_name, *_args)
        end

      end

      def initialize(framework, _opts)
        @subscriber = Subscriber.new(framework)
        event_subscribers = framework.events.instance_variable_get(:@session_event_subscribers).collect(&:class)
        general_subscribers = framework.events.instance_variable_get(:@general_event_subscribers).collect(&:class)
        # add self to session subscriber for listen the on_session_*
        if !event_subscribers.include?(@subscriber.class)
          framework.events.add_session_subscriber(@subscriber)
        end
        # add self to general subscriber for listen the on_module_*
        if !general_subscribers.include?(@subscriber.class)
          framework.events.add_general_subscriber(@subscriber)
        end
      end
    end
  end
end