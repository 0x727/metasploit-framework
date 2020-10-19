using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace SharpRDPCheck
{
    public class Options
    {
        public static int SocketTimeout = 3000;  
        public static string ClientName = "Windows7"; // Client Name
        public string Domain = ""; // Domain
        public string DomainAndUsername = ""; // Domain and Username
        public string Host = ""; // Host
        public string hostname = "";
        public string Username = ""; // Username
        public string Password = ""; // Password
        public string hash = ""; // NTLM hash
        public int Port = 3389; // Port
        public bool enableNLA = true; // Enable NLA
        public MCS.NegotiationFlags serverNegotiateFlags;
    }
}