using System;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;
using System.Text;

namespace port.port
{
    class CPingScan
    {
        static int upCount = 0;
        static object lockObj = new object();

        public static void scanA(string ipBase, int ttl, int timeoutSecond)
        {
            string data = "AAAAAAAA";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingOptions options = new PingOptions(ttl, true);

            CountdownEvent countdown = new CountdownEvent(1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //string ipBase = "192.168.1.";
            for (int i = 1; i < 255; i++)
            {
                for (int j = 1; j < 255; j++)
                {
                    for (int k = 1; k < 255; k++)
                    {
                        string ip = ipBase + "." + i.ToString() + "." + j.ToString() + "." + k.ToString();

                        try
                        {
                            Ping p = new Ping();
                            p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
                            countdown.AddCount();
                            p.SendAsync(ip, timeoutSecond * 1000, buffer, options, countdown);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Exception: {0} {1}", ip, e.Message);
                        }
                    }
                    Console.WriteLine("count:{0}", countdown.CurrentCount);
                    if (countdown.CurrentCount > 20000)
                        Thread.Sleep(10);
                }
                //Thread.Sleep(3 * 1000);
                Console.WriteLine("Took {0} seconds. to {1} have {2} hosts active.", sw.ElapsedMilliseconds / 1000, ipBase + "." + i.ToString(), upCount);
            }
            countdown.Signal();
            countdown.Wait();
            sw.Stop();
            TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            Console.WriteLine("Took {0} seconds. {1} have {2} hosts active.", sw.ElapsedMilliseconds / 1000, ipBase, upCount);
        }

        public static void scan(string ipBase, int ttl, int timeoutSecond)
        {
            string data = "AAAAAAAA";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingOptions options = new PingOptions(ttl, true);

            CountdownEvent countdown = new CountdownEvent(1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 1; i < 255; i++)
            {
                string ip = ipBase + "."+ i.ToString();

                Ping p = new Ping();
                p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
                countdown.AddCount();
                p.SendAsync(ip, timeoutSecond * 1000, buffer, options, countdown);
            }
            countdown.Signal();
            countdown.Wait();
            sw.Stop();
            TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            Console.WriteLine("Took {0} seconds. {1} have {2} hosts active.", sw.ElapsedMilliseconds/1000, ipBase, upCount);

        }

        private static void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string ip = e.Reply.Address.ToString();
                Console.WriteLine("{0} is up: ({1} ms {2} ttl)", ip, e.Reply.RoundtripTime, e.Reply.Options.Ttl);
                lock (lockObj)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                //Console.WriteLine("Pinging {0} failed. (Null Reply object?)", ip);
            }
            //countdown.Signal();
            ((CountdownEvent)e.UserState).Signal();
        }
    }
}
