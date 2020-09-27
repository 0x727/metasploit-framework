using System;
using CommandLineParser.Exceptions;
using Tools;


namespace scanner.mysql
{

    class main
    {
        static void Main(string[] args)
        {
            string strServiceName = "MySQL";
            int nServicePort = 3306;

            var parser = new CommandLineParser.CommandLineParser();
            parser.ShowUsageOnEmptyCommandline = true;

            var p = new ParsingTarget();
            // read the argument attributes 
            parser.ShowUsageHeader = strServiceName + " brute program";
            parser.ShowUsageFooter = "Thank you for using.";
            parser.ExtractArgumentAttributes(p);
            ThreadTool objMain = new Tools.ThreadTool();

            try
            {
                parser.ParseCommandLine(args);
                //parser.ShowParsedArguments();
                if (args.Length <= 0)
                {
                    return;
                }

                if (p.strTarget == null || p.strUsernameOrFilePath == null || p.strPasswordOrFilePath == null)
                {
                    objMain.LogError("Please fill arguments host/username/password!");
                    parser.ShowUsage();
                    return;
                }

                objMain.crackerOneCount = p.bCrackOne;
                objMain.retryCount = p.nRetry;
                if (p.nTimeOut > 0 && p.nTimeOut <= 300) objMain.timeOut = p.nTimeOut;
                if (p.nThread > 0 && p.nThread <= 10000) objMain.maxThread = p.nThread;
                if (p.nPort <= 0 || p.nPort > 65535) p.nPort = nServicePort;

                if (!objMain.cracker(strServiceName, p.strTarget, p.nPort, p.strUsernameOrFilePath, p.strPasswordOrFilePath))
                {
                    parser.ShowUsage();
                }
            }
            catch (CommandLineException e)
            {
                objMain.LogError(e.Message);
                parser.ShowUsage();
            }
            catch (Exception e2)
            {
                objMain.LogError(e2.Message);
                parser.ShowUsage();
            }

        }


    }
}
