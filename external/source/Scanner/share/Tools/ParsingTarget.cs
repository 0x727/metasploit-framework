using CommandLineParser.Arguments;

namespace Tools
{
    class ParsingTarget
    {
        [ValueArgument(typeof(string), 'h', "host", Description = "Set target host ip/ips/cidr. \r\n            Ex: 192.168.1.1,192.168.1.1-192.168.200.1,192.168.0.0/16", DefaultValue = "")]
        public string strTarget;

        [BoundedValueArgument(typeof(int), 'p', "port", MinValue = 1, MaxValue = 65535, Description = "Set target port[1,65535]", DefaultValue = -1)]
        public int nPort;

        [ValueArgument(typeof(string), 'u', "username", Description = "Set brute username or username file path (.txt)", DefaultValue = "")]
        public string strUsernameOrFilePath;

        [ValueArgument(typeof(string), 'w', "password", Description = "Set brute password or password file path (.txt)", DefaultValue = "")]
        public string strPasswordOrFilePath;

        [SwitchArgument('c', "crackone", true, Description = "Set whether crack one account for per ip/port service, default=true.")]
        public bool bCrackOne;

        [BoundedValueArgument(typeof(int), 't', "thread", MinValue = 1, MaxValue = 10000, Description = "Set max threads num[1,10000], default=50.")]
        public int nThread;

        [BoundedValueArgument(typeof(int), 'r', "retry", MinValue = 0, MaxValue = 10, Description = "Set connect retry num[0,10], default=0.")]
        public int nRetry;

        [BoundedValueArgument(typeof(int), 'o', "timeout", MinValue = 1, MaxValue = 300, Description = "Set connect timeout seconds[1,300], default=5.")]
        public int nTimeOut;

        [SwitchArgument('v', "verbose", false, Description = "Set whether show verbose log, default=false.")]
        public bool bVerbose;
    }
}