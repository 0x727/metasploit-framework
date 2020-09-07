//using Chilkat;
//using FluentFTP;
using LumiSoft.Net.FTP.Client;
using System;
using System.Net;
//using Tools;

namespace SNETCracker.Model
{
    class CrackFTP : CrackService
    {
        public CrackFTP() {

        }
        
        public override Server crack(String ip, int port,String username,String password,int timeOut) {

            FTP_Client ftp = new FTP_Client();
            Server server = new Server();
            if ("空".Equals(password))
            {
                password = "";
            }
            try
            {
                //ftp.Host = ip;
                //ftp.Port = port;
                //ftp.Credentials = new NetworkCredential(username, password);
                //ftp.ConnectTimeout = timeOut*1000;
                //ftp.ReadTimeout = timeOut*1000;

                //ftp.Connect();

                //if (ftp.IsConnected)
                //{
                //    server.isSuccess = true;
                //    server.banner = ftp.SystemType;
                //}

                ftp.Timeout = timeOut;
                ftp.Connect(ip, port, false);
                if (ftp.IsConnected)
                {
                    ftp.Authenticate(username, password);

                    if (ftp.IsAuthenticated)
                    {
                        server.isSuccess = ftp.IsAuthenticated;
                        server.banner = ftp.GreetingText;
                    }

                }


            }
            catch (Exception e)
            {
                if(e.Message.IndexOf("cannot log in") ==-1){
                    throw e;
                } 
            }
            finally
            {
                if (ftp != null)
                {
                    ftp.Disconnect();
                }
            }
            return server;
        }

        
   
    }
}
