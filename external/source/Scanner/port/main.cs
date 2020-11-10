using System;
using System.Threading;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace scanner.port
{
	class ParsingTargetPort
	{
#pragma warning disable 0649
        [ValueArgument(typeof(string), 'h', "host", Description = "Set target host ip/ips/cidr. \r\n            Ex: 192.168.1.1,192.168.1.1-192.168.200.1,192.168.0.0/16", DefaultValue = "")]
		public string strTarget;

		[ValueArgument(typeof(string), 'p', "port", Description = "Set target port. Ex: 22,80,100-200,3389")]
		public string strPort;

        [BoundedValueArgument(typeof(int), 't', "thread", MinValue = 1, MaxValue = 20000, Description = "Set max threads num[1,20000], default=50.")]
        public int nThread;

        [BoundedValueArgument(typeof(int), 'o', "timeout", MinValue = 1, MaxValue = 300, Description = "Set connect timeout seconds[1,300], default=5.")]
        public int nTimeOut;

        [BoundedValueArgument(typeof(int), 'l', "ttl", MinValue = 1, MaxValue = 256, Description = "Set ping ttl[1,256], default=64.")]
        public int nTTL;

        [SwitchArgument('a', "alive", false, Description = "Set whether use ping to detect alive, default=false.")]
        public bool bDetectAlive;

        [SwitchArgument('v', "verbose", false, Description = "Set whether show verbose log, default=false.")]
        public bool bVerbose;
#pragma warning restore 0649
    }
    class main
    {
        static void Main(string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageOnEmptyCommandline = true;

            var p = new ParsingTargetPort();
            // read the argument attributes 
            parser.ShowUsageHeader = "Port scan program";
            parser.ShowUsageFooter = "Thank you for using.";
            parser.ExtractArgumentAttributes(p);

            try
            {
                parser.ParseCommandLine(args);
                //parser.ShowParsedArguments();
                if (args.Length <= 0)
                {
                    return;
                }

                if (p.strTarget == null || p.strPort == null)
                {
                    Console.WriteLine("Please fill arguments host/port!");
                    parser.ShowUsage();
                    return;
                }

                PortScan.AddPortScanValue += AddPortScanValue;
                PortScan.Host = p.strTarget;
                PortScan.PortFile = p.strPort;
                PortScan.DetectAlive = p.bDetectAlive;
                //Console.WriteLine("-----------------Detect alive:{0}:{1}", PortScan.DetectAlive, p.bDetectAlive);
                PortScan.ShowVerbose = p.bVerbose;
                PortScan.MaxThread = 50;
                PortScan.TimeoutSec = 5;
                PortScan.TTL = 64;
                if (p.nTimeOut > 0 && p.nTimeOut <= 300) PortScan.TimeoutSec = p.nTimeOut;
                if (p.nThread > 0 && p.nThread <= 20000) PortScan.MaxThread = p.nThread;
                if (p.nTTL > 0 && p.nTTL <= 256) PortScan.TTL = p.nTTL;

                //ServiceProbes.Init("C:\\Users\\Administrator\\Desktop\\nmap-service-probes");
                PortScan.GetBanner = false;
                if (PortScan.ShowVerbose)
                    Console.WriteLine("[{0}] Start Program...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
                PortScan.Start();
                while (PortScan.NowThread <= 0)
                    Thread.Sleep(100);
                while (PortScan.isRuning)
                {
                    //Console.WriteLine("isRuning:{0}, NowThread:{1}", PortScan.isRuning, PortScan.NowThread);
                    Thread.Sleep(100);
                }
                if (PortScan.ShowVerbose)
                    Console.WriteLine("[{2}] isRuning:{0}, NowThread:{1}", PortScan.isRuning, PortScan.NowThread, DateTime.Now.ToString("MM/dd HH:mm:ss"));
                while (PortScan.NowThread > 0)
                {
                    //Console.WriteLine("isRuning:{0}, NowThread:{1}", PortScan.isRuning, PortScan.NowThread);
                    Thread.Sleep(100);
                }
                if (PortScan.ShowVerbose)
                    Console.WriteLine("[{2}] isRuning:{0}, NowThread:{1}", PortScan.isRuning, PortScan.NowThread, DateTime.Now.ToString("MM/dd HH:mm:ss"));

            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                parser.ShowUsage();
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2.Message);
                parser.ShowUsage();
            }
		}

		private static void AddPortScanValue(string Host, string Protocol, string PortData)
		{
			Console.WriteLine("[{0}] {1} Opened", DateTime.Now.ToString("MM/dd HH:mm:ss"), Host);
		}

        private static void PingThread(string strIP)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            
            System.Net.NetworkInformation.PingReply r =  p.Send(strIP);
            //r.Options.Ttl
        }
	}
}
