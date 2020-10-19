using System;
using SharpRDPCheck;

namespace SNETCracker.Model
{
    class CrackRDP : CrackService
    {
        public CrackRDP()
        {

        }
        public override Server crack(String ip, int port, String username, String password, int timeOut)
        {
            Server server = new Server();

            MCS mcs = new MCS();
            try
            {
                Options.SocketTimeout = timeOut * 1000;
                mcs.options.Host = ip;
                mcs.options.Port = port;
                mcs.options.Username = username;
                //if (args[2] == "plaintext")
                //    mcs.options.Password = args[4];
                //else if (args[2] == "ntlmhash")
                //    mcs.options.hash = args[4];
                //else
                //{
                //    Console.WriteLine("[!] Wrong parameter");
                //    System.Environment.Exit(0);
                //}
                mcs.options.Password = password;
                if (mcs.Connect(mcs.options.Host, mcs.options.Port))
                {
                    server.isConnected = true;
                    mcs.sendConnectionRequest(null, false);
                    server.isSuccess = true;
                }
                else
                {
                    throw new IPBreakException(ip + port);
                }
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("Server doesn't support PROTOCOL_HYBRID") >= 0)
                {
                    throw new IPBreakException(ip + port);
                }
                else
                    throw e;
            }
            finally
            {
                mcs.Dispose();
            }

            return server;
        }
    }
}
