using System;
using System.Threading;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;

namespace scanner.port
{
	class ParsingTargetPort
	{
		[ValueArgument(typeof(string), 'h', "host", Description = "Set target host ip/ips/domain. \r\n            Ex: 192.168.1.1 192.168.1.1-192.168.200.1  192.168.1.1-192.168.1.200", DefaultValue = "")]
		public string strTarget;

		[ValueArgument(typeof(string), 'p', "port", Description = "Set target port. Ex: 22,80,100-200,3389")]
		public string strPort;

        [BoundedValueArgument(typeof(int), 't', "thread", MinValue = 1, MaxValue = 10000, Description = "Set max threads num[1,10000], default=50.")]
        public int nThread;

        [BoundedValueArgument(typeof(int), 'o', "timeout", MinValue = 1, MaxValue = 300, Description = "Set connect timeout seconds[1,300], default=5.")]
        public int nTimeOut;
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
                PortScan.MaxThread = 50;
                PortScan.TimeoutSec = 5;
                if (p.nTimeOut > 0 && p.nTimeOut <= 300) PortScan.TimeoutSec = p.nTimeOut;
                if (p.nThread > 0 && p.nThread <= 10000) PortScan.MaxThread = p.nThread;

                //ServiceProbes.Init("C:\\Users\\Administrator\\Desktop\\nmap-service-probes");
                PortScan.GetBanner = false;

                PortScan.Start();
                while (PortScan.NowThread <= 0)
                    Thread.Sleep(100);
                while (PortScan.isRuning)
                    Thread.Sleep(100);
                while (PortScan.NowThread > 0)
                    Thread.Sleep(100);
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
			Console.WriteLine(Host + " Opened");
		}
	}
}
