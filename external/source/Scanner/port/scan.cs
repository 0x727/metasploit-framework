using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Net.NetworkInformation;

namespace scanner.port
{
	public class MyTask
	{
		public Socket Sock;
		public Stopwatch RunTime;
		public SocketAsyncEventArgs SAEA;
	}

	public class Interval
	{
		public Interval()
		{
			this.st = 0L;
			this.ed = 0L;
		}

		public Interval(long s, long e)
		{
			this.st = s;
			this.ed = e;
		}

		public long st;
		public long ed;
	}

	class PortScan
    {
		public static bool ShowVerbose;
		public static bool isRuning;
		public static string Host;
		public static string PortFile;
		public static int NowThread;
		public static int MaxThread;
		public static int TimeoutSec;
		public static int TTL;
		public static bool DetectAlive;
		public static bool GetBanner;
		public static int ScanSpeed;
		public static Stopwatch ScanTime = new Stopwatch();
		//public static string BasePath = AppDomain.CurrentDomain.BaseDirectory + "PortFiles\\";
        private static List<MyTask> MyTasks = new List<MyTask>();
        private static List<int> ScanPorts = new List<int>();
		private static BlockingCollection<IPEndPoint> IPES;
		private static string TaskTempFile;
        private static long NowProgress;
        private static long MaxProgress;

        private static int upCount = 0;
		private static object lockObjForCount = new object();
		//private static Queue<string> queueAliveHosts = new Queue<string>();
		//private static object lockObjForList = new object();
		private static List<string> lstHostAlive = new List<string>();


		public static event PortScan.AddPortScanValueEventHandler AddPortScanValue;
		public delegate void AddPortScanValueEventHandler(string Host, string Protocol, string PortData);

		public static void Start()
        {
			PortScan.GetPorts();
			PortScan.GetTasks();
			PortScan.IPES = new BlockingCollection<IPEndPoint>(PortScan.MaxThread);
			int maxThread = PortScan.MaxThread;
			for (int i = 1; i <= maxThread; i++)
			{
				MyTask myTask = new MyTask();
				myTask.SAEA = new SocketAsyncEventArgs();
				myTask.SAEA.DisconnectReuseSocket = true;
				myTask.RunTime = new Stopwatch();

				myTask.SAEA.Completed += delegate (object sender, SocketAsyncEventArgs e)
				{
					PortScan.OnCompleted((Socket)sender, e);
				};
				PortScan.MyTasks.Add(myTask);
				PortScan.MyTasks.Add(myTask);
			}
			PortScan.NowThread = 0;
			PortScan.ScanSpeed = 0;
            PortScan.NowProgress = 0L;
            PortScan.isRuning = true;
			PortScan.ScanTime.Restart();
			if (PortScan.DetectAlive)
            {
				Task.Factory.StartNew(new Action(PortScan.ScanAlive));
				//Task.Factory.StartNew(new Action(PortScan.ReadTask));
			}
            else
            {
				Task.Factory.StartNew(new Action(PortScan.ReadTaskForNoDetectAlive));
			}
			Task.Factory.StartNew(new Action(PortScan.TakeTask));
			Task.Factory.StartNew(new Action(PortScan.CheckTimeout));
        }

        //public static string GetProgressValue()
        //{
        //    Thread.Sleep(1000);
        //    string result = string.Format("{0} {1}/s", ((double)PortScan.NowProgress / (double)PortScan.MaxProgress * 100.0).ToString("0.00") + "%", PortScan.ScanSpeed.ToString());
        //    PortScan.ScanSpeed = 0;
        //    return result;
        //}

        private static void GetPorts()
		{
			PortScan.ScanPorts.Clear();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			try
			{
				//foreach (string text in File.ReadLines(PortScan.BasePath + PortScan.PortFile + ".txt"))
				//foreach (string text in File.ReadLines(PortScan.PortFile))
				string text = PortScan.PortFile;
				{
					if (text.Length != 0)
					{
						foreach (string text2 in text.Split(new char[]
						{
							','
						}))
						{
							if (text2.Contains("-"))
							{
								string[] array2 = text2.Split(new char[]
								{
									'-'
								});
								if (array2.Length == 2)
								{
									int num = (int)Math.Round(Convert.ToDouble(array2[0]));
									int num2 = (int)Math.Round(Convert.ToDouble(array2[1]));
									for (int j = num; j <= num2; j++)
									{
										if (!dictionary.ContainsKey(j))
										{
											dictionary.Add(j, 0);
											PortScan.ScanPorts.Add(j);
										}
									}
								}
							}
							else
							{
								int num3 = int.Parse(text2);
								if (!dictionary.ContainsKey(num3))
								{
									dictionary.Add(num3, 0);
									PortScan.ScanPorts.Add(num3);
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		private static void GetTasks()
		{
			string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (File.Exists(PortScan.Host))
			{
				PortScan.TaskTempFile = PortScan.Host;
			}
			else
			{
				PortScan.TaskTempFile = appPath + "\\MyScanFile.tmp";
				File.WriteAllText(PortScan.TaskTempFile, PortScan.Host.Replace(",", "\r\n"));
			}
			List<Interval> list = new List<Interval>();
			int[] array = new int[4];
			IPAddress ipaddress = null;
			try
			{
				foreach (string text in File.ReadLines(PortScan.TaskTempFile))
				{
					if (text.Length != 0)
					{
						if (text.Contains("-"))
						{
							string[] array2 = text.Split(new char[]
							{
								'-'
							});
							if (array2.Length == 2)
							{
								long s = (long)((ulong)PortScan.IpToLong(array2[0]));
								long e = (long)((ulong)PortScan.IpToLong(array2[1]));
								list.Add(new Interval(s, e));
							}
						}
						else if (text.Contains("/") && !text.Contains("://"))
						{
							string[] array2 = text.Split(new char[]
							{
								'/'
							});
							if (array2.Length == 2)
							{
								string text2 = array2[0];
								string text3 = array2[1];
								array2 = text2.Split(new char[]
								{
									'.'
								});
								int num = 0;
								do
								{
									array[num] = int.Parse(array2[num]);
									array2[num] = Convert.ToString(array[num], 2).PadLeft(8, '0');
									num++;
								}
								while (num <= 3);
								string text4 = string.Join("", array2);
								text4 = text4.Substring(0, Convert.ToInt32(text3)).PadRight(32, '1');
								int num2 = 0;
								int num3 = 0;
								do
								{
									array2[num2] = text4.Substring(num3, 8);
									array[num2] = Convert.ToInt32(array2[num2], 2);
									num2++;
									num3 += 8;
								}
								while (num3 <= 31);
								text3 = string.Join<int>(".", array);
								long s = (long)((ulong)PortScan.IpToLong(text2));
								long e = (long)((ulong)PortScan.IpToLong(text3));
								list.Add(new Interval(s, e));
							}
						}
						else
						{
							string text_tmp = text;
							try
							{
								if (text.Contains("://"))
								{
									text_tmp = new Uri(text).Host;
								}
							}
							catch
							{
							}
							if (IPAddress.TryParse(text_tmp, out ipaddress))
							{
								list.Add(new Interval((long)((ulong)PortScan.IpToLong(text_tmp)), (long)((ulong)PortScan.IpToLong(text_tmp))));
							}
							else
							{
								try
								{
									text_tmp = Dns.GetHostEntry(text_tmp).AddressList[0].ToString();
									list.Add(new Interval((long)((ulong)PortScan.IpToLong(text_tmp)), (long)((ulong)PortScan.IpToLong(text_tmp))));
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			catch
			{
			}

			PortScan.TaskTempFile = appPath + "\\MyScanFile.tmp";
			StreamWriter streamWriter = new StreamWriter(PortScan.TaskTempFile);
			List<Interval> list2 = PortScan.Merge(list);
			long num4 = 0L;
			try
			{
				foreach (Interval interval in list2)
				{
					if (interval.st != interval.ed)
					{
						streamWriter.WriteLine(PortScan.LongToIP(interval.st) + "-" + PortScan.LongToIP(interval.ed));
						num4 += interval.ed - interval.st + 1L;
					}
					else
					{
						streamWriter.WriteLine(PortScan.LongToIP(interval.st));
						num4 += 1L;
					}
				}
			}
			catch
			{
			}
			streamWriter.Close();
            PortScan.MaxProgress = (long)PortScan.ScanPorts.Count * num4;
        }

		private static void ScanAlive()
		{
			if (ShowVerbose)
				Console.WriteLine("[{0}] Start to ScanAlive...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
			int ttl = PortScan.TTL;
			int timeoutSecond = PortScan.TimeoutSec;

			int scanportNum = PortScan.ScanPorts.Count;
			int MaxSocketNum = PortScan.MaxThread >= 5 ? PortScan.MaxThread / 5 : 1;
			List<string> lstIP = new List<string>();
			IPAddress ipaddress = null;
			IPAddress ipaddress2 = null;
			try
			{
				foreach (string text in File.ReadLines(PortScan.TaskTempFile))
				{
					if (!PortScan.isRuning)
					{
						break;
					}
					if (text.Contains("-"))
					{
						string[] array = text.Split(new char[] { '-' });
						if (IPAddress.TryParse(array[0], out ipaddress) && IPAddress.TryParse(array[1], out ipaddress2))
						{
							long num = (long)((ulong)PortScan.IpToLong(array[0]));
							long num2 = (long)((ulong)PortScan.IpToLong(array[1]));
							for (long num3 = num; num3 <= num2; num3 += 1L)
							{
								if (!PortScan.isRuning)
								{
									break;
								}
								if (!lstIP.Contains(PortScan.LongToIP(num3)))
									lstIP.Add(PortScan.LongToIP(num3));
							}
						}
						else
						{
							if (!lstIP.Contains(text))
								lstIP.Add(text);
						}
					}
					else
					{
						if (!lstIP.Contains(text))
							lstIP.Add(text);
					}
				}
			}
			catch
			{
			}
			File.Delete(PortScan.TaskTempFile);

			string data = "AAAAAAAA";
			byte[] buffer = Encoding.ASCII.GetBytes(data);
			PingOptions options = new PingOptions(ttl, true);

			CountdownEvent countdown = new CountdownEvent(1);
			Stopwatch sw = new Stopwatch();
			sw.Start();
			if (ShowVerbose)
				Console.WriteLine("[{0}] Start to detect...", DateTime.Now.ToString("MM/dd HH:mm:ss"));

			int nCount = 0;
			DateTime nLastTime = DateTime.Now;
			foreach (string ip in lstIP)
			{
				if (!PortScan.isRuning)
				{
					break;
				}

				nCount++;
				if (DateTime.Now.Subtract(nLastTime) > TimeSpan.FromSeconds(1))
				{
					if (ShowVerbose)
						Console.WriteLine("[{2}] Took {0} seconds.  have {1} hosts active. Now detect IP: {3}.",
						sw.ElapsedMilliseconds / 1000, upCount, DateTime.Now.ToString("MM/dd HH:mm:ss"), ip);
					nLastTime = DateTime.Now;
				}
				if (nCount % 10 == 0 && countdown.CurrentCount > MaxSocketNum)
				{
					Thread.Sleep(10);
					continue;
				}
				for (int i = 0; i < 2; i++)
				{
					Ping p = new Ping();
					p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
					countdown.AddCount();
					p.SendAsync(ip, timeoutSecond * 1000, buffer, options, countdown);
				}
			}

			if (ShowVerbose)
				Console.WriteLine("[{2}] Took {0} seconds.  have {1} hosts active.", sw.ElapsedMilliseconds / 1000, upCount, DateTime.Now.ToString("MM/dd HH:mm:ss"));
			if (ShowVerbose)
				Console.WriteLine("[{0}] Begin countdown wait...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
			countdown.Signal();
			countdown.Wait();
			if (ShowVerbose)
				Console.WriteLine("[{0}] End countdown wait...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
			PortScan.IPES.CompleteAdding();
			sw.Stop();
			TimeSpan span = new TimeSpan(sw.ElapsedTicks);
			Console.WriteLine("[{2}] Took {0} seconds.  have {1} hosts active.", sw.ElapsedMilliseconds / 1000, upCount, DateTime.Now.ToString("MM/dd HH:mm:ss"));
		}

		private static void p_PingCompleted(object sender, PingCompletedEventArgs e)
		{
			if (e.Reply != null)
			{
				string ip = e.Reply.Address.ToString();
				if (e.Reply.Status == IPStatus.Success)
				{
					if (!lstHostAlive.Contains(ip))
					{
						if (ShowVerbose)
							Console.WriteLine("[{3}] {0} is up: ({1} ms {2} ttl)", ip, e.Reply.RoundtripTime, e.Reply.Options.Ttl, DateTime.Now.ToString("MM/dd HH:mm:ss"));
						lock (lockObjForCount)
						{
							upCount++;
						}
						PortScan.MaxProgress = (long)PortScan.ScanPorts.Count * upCount;
						PortScan.AddHost(ip);
					}
				}
                else
                {
					//Console.WriteLine("Ping {0} failed: {1}", ip, e.Reply.Status.ToString());
                }
			}
			else
			{
                //Console.WriteLine("Pinging failed. (Null Reply object?)");
            }
			((CountdownEvent)e.UserState).Signal();
		}

        private static void ReadTaskForNoDetectAlive()
        {
            IPAddress ipaddress = null;
            IPAddress ipaddress2 = null;
            try
            {
                foreach (string text in File.ReadLines(PortScan.TaskTempFile))
                {
                    if (!PortScan.isRuning)
                    {
                        break;
                    }
                    if (text.Contains("-"))
                    {
                        string[] array = text.Split(new char[] { '-' });
                        if (IPAddress.TryParse(array[0], out ipaddress) && IPAddress.TryParse(array[1], out ipaddress2))
                        {
                            long num = (long)((ulong)PortScan.IpToLong(array[0]));
                            long num2 = (long)((ulong)PortScan.IpToLong(array[1]));
                            for (long num3 = num; num3 <= num2; num3 += 1L)
                            {
                                if (!PortScan.isRuning)
                                {
                                    break;
                                }
                                PortScan.AddHost(PortScan.LongToIP(num3));
                            }
                        }
                        else
                        {
                            PortScan.AddHost(text);
                        }
                    }
                    else
                    {
                        PortScan.AddHost(text);
                    }
                }
            }
            catch
            {
            }
            PortScan.IPES.CompleteAdding();
            File.Delete(PortScan.TaskTempFile);
        }

        private static void AddHost(string IP)
		{
			try
			{
				if (lstHostAlive.Contains(IP))
					return;
				else
					lstHostAlive.Add(IP);
				foreach (int num in PortScan.ScanPorts)
				{
					if (!PortScan.isRuning)
					{
						break;
					}
					//PortScan.Host = IP + ":" + Convert.ToString(num);
					PortScan.IPES.Add(new IPEndPoint(IPAddress.Parse(IP), num));
				}
			}
			catch
			{
			}
		}

		private static void CheckTimeout()
		{
			//for (; ; )
			//{
			//	if (!PortScan.isRuning)
			//	{
			//		goto IL_69;
			//	}
			//IL_07:
			//	try
			//	{
			//		foreach (MyTask myTask in PortScan.MyTasks)
			//		{
			//			if (myTask.RunTime.IsRunning && myTask.RunTime.Elapsed.Seconds >= PortScan.TimeoutSec)
			//			{
			//				myTask.Sock.Close(0);
			//			}
			//		}
			//		goto IL_72;
			//	}
			//	finally
			//	{
			//		//List<MyTask>.Enumerator enumerator;
			//		//((IDisposable)enumerator).Dispose();
			//	}
			//	goto IL_69;
			//IL_72:
			//	Thread.Sleep(500);
			//	continue;
			//IL_69:
			//	if (PortScan.NowThread != 0)
			//	{
			//		goto IL_07;
			//	}
			//	break;
			//}
			if (ShowVerbose)
				Console.WriteLine("[{0}] Start CheckTimeout...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
			DateTime nStartTime = DateTime.Now;
			DateTime nLastTime = DateTime.Now;
			while (true)
            {
				if (PortScan.isRuning)// && PortScan.NowThread != 0)
                {
					if (PortScan.NowThread == 0)
                    {
						Thread.Sleep(500);
						continue;
                    }
					try
					{
						int nCount = 0;
						foreach (MyTask myTask in PortScan.MyTasks)
						{
							if (myTask.RunTime.IsRunning && myTask.RunTime.Elapsed.Seconds >= PortScan.TimeoutSec)
							{
								myTask.Sock.Close(0);
								nCount++;
							}
						}
						if (DateTime.Now.Subtract(nLastTime) > TimeSpan.FromSeconds(1))
						{
							if (ShowVerbose)
								Console.WriteLine("[{0}] CheckTimeout, Took {1} seconds, close Tasks {2}.", DateTime.Now.ToString("MM/dd HH:mm:ss"),
								DateTime.Now.Subtract(nStartTime).TotalSeconds, nCount);
							nLastTime = DateTime.Now;
						}
					}
					catch
					{
					}
					Thread.Sleep(500);
					continue;
				}
				else
                {
					break;
                }

			}
		}

		private static void TakeTask()
		{
			if (ShowVerbose)
				Console.WriteLine("[{0}] Start TakeTask...", DateTime.Now.ToString("MM/dd HH:mm:ss"));
			DateTime nStartTime = DateTime.Now;
			DateTime nLastTime = DateTime.Now;
			while (!PortScan.IPES.IsCompleted)
			{
				bool bAdded = false;
				try
				{
                    foreach (MyTask myTask in PortScan.MyTasks)
					{
						if (!PortScan.isRuning)
						{
							goto break_point;
						}


                        if ((myTask.Sock == null || !myTask.Sock.Connected) && !myTask.RunTime.IsRunning)
						{
							BlockingCollection<IPEndPoint> ipes = PortScan.IPES;
							SocketAsyncEventArgs saea;
							IPEndPoint remoteEndPoint = (IPEndPoint)(saea = myTask.SAEA).RemoteEndPoint;
							bool flag = ipes.TryTake(out remoteEndPoint);
							saea.RemoteEndPoint = remoteEndPoint;
							if (flag)
							{
								if (DateTime.Now.Subtract(nLastTime) > TimeSpan.FromSeconds(1))
								{
									if (ShowVerbose)
										Console.WriteLine("[{0}] Took {3} seconds, progress {4}, {5}/{6}.  Now MyTask: {1}:{2}.", DateTime.Now.ToString("MM/dd HH:mm:ss"),
										remoteEndPoint.Address, remoteEndPoint.Port, DateTime.Now.Subtract(nStartTime).TotalSeconds,
										((double)PortScan.NowProgress / (double)PortScan.MaxProgress * 100.0).ToString("0.00") + "%",
										PortScan.NowProgress, PortScan.MaxProgress);
									nLastTime = DateTime.Now;
								}
								myTask.RunTime.Restart();
								myTask.SAEA.UserToken = myTask.RunTime;
								myTask.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
								{
									SendTimeout = PortScan.TimeoutSec * 1000,
									ReceiveTimeout = PortScan.TimeoutSec * 1000
								};
								myTask.Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
								Interlocked.Increment(ref PortScan.NowThread);
								bAdded = true;
								myTask.Sock.ConnectAsync(myTask.SAEA);
								bAdded = false;
							}
						}
					}
				}
				catch
				{
					if (bAdded)
						Interlocked.Decrement(ref PortScan.NowThread);
				}
				Thread.Sleep(100);
			}
		break_point:
			Thread.Sleep(100);
			while (PortScan.IPES.Count > 0)
			{
				PortScan.IPES.Take();
			}
			while (PortScan.NowThread > 0)
			{
				Thread.Sleep(100);
			}
			try
			{
				foreach (MyTask myTask2 in PortScan.MyTasks)
				{
					myTask2.SAEA.Dispose();
					if (myTask2.Sock != null)
					{
						myTask2.Sock.Close(0);
					}
				}
			}
			catch
			{
			}
			PortScan.MyTasks.Clear();
			PortScan.ScanPorts.Clear();
			PortScan.isRuning = false;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		private static void OnCompleted(Socket Sender, SocketAsyncEventArgs e)
		{
			((Stopwatch)e.UserToken).Stop();
            Interlocked.Increment(ref PortScan.NowProgress);
            Interlocked.Increment(ref PortScan.ScanSpeed);
			Interlocked.Decrement(ref PortScan.NowThread);

			if (e.SocketError == SocketError.Success)
			{
				string port = e.RemoteEndPoint.ToString().Split(new char[]{':'})[1];
				string PortData = "-";
				string PortType = "-";
				if (PortScan.GetBanner)
				{
					Task task = new Task(delegate ()
					{
						try
						{
							Sender.Send(new byte[1]);
							byte[] array = new byte[4097];
							int num = Sender.Receive(array);
							if (num > 0)
							{
								PortData = ServiceProbes.BytesToHexString(array.Take(num).ToArray<byte>());
								PortType = ServiceProbes.GetServerName(PortData);
								//if (Operators.CompareString(PortType, "", false) == 0)
								//......
								if (PortType != "")
								{
									if (PortData.StartsWith("HTTP/"))
									{
										PortType = "+http";
									}
									else
									{
										PortType = ServiceProbes.GetPortName(port);
									}
								}
								else
								{
									PortType = "+" + PortType;
								}
							}
							else
							{
								PortType = ServiceProbes.GetPortName(port);
								PortData = "-";
							}
						}
						catch
						{
							PortType = ServiceProbes.GetPortName(port);
							PortData = "-";
						}
					});
					task.Start();
					task.Wait();
				}
				PortScan.AddPortScanValueEventHandler addPortScanValueEvent = PortScan.AddPortScanValue;
				if (addPortScanValueEvent != null)
				{
					addPortScanValueEvent(e.RemoteEndPoint.ToString(), PortType, PortData);
				}
			}
            else
            {
                //Console.WriteLine("{0} {1}", e.RemoteEndPoint.ToString(), e.SocketError.ToString());
            }
            Sender.Close(0);
		}

		private static uint IpToLong(string ip)
		{
			IPAddress ipaddress = null;
			IPAddress.TryParse(ip, out ipaddress);
			uint result;
			if (ipaddress == null)
			{
				result = 0U;
			}
			else
			{
				byte[] addressBytes = ipaddress.GetAddressBytes();
				Array.Reverse(addressBytes);
				result = BitConverter.ToUInt32(addressBytes, 0);
			}
			return result;
		}

		private static string LongToIP(long longIP)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(longIP >> 24);
			stringBuilder.Append(".");
			stringBuilder.Append((longIP & 16777215L) >> 16);
			stringBuilder.Append(".");
			stringBuilder.Append((longIP & 65535L) >> 8);
			stringBuilder.Append(".");
			stringBuilder.Append(longIP & 255L);
			return stringBuilder.ToString();
		}

		private static List<Interval> Merge(List<Interval> intervals)
		{
			List<Interval> list = new List<Interval>();
			List<Interval> result;
			if (intervals.Count == 0)
			{
				result = list;
			}
			else
			{
				intervals.OrderBy(i => i.st);
				list.Add(intervals[0]);
				long num = (long)(intervals.Count - 1);
				for (long num2 = 1L; num2 <= num; num2 += 1L)
				{
					if (intervals[(int)num2].st <= list[list.Count - 1].ed)
					{
						list[list.Count - 1].ed = Math.Max(intervals[(int)num2].ed, list[list.Count - 1].ed);
					}
					else
					{
						list.Add(intervals[(int)num2]);
					}
				}
				result = list;
			}
			return result;
		}

	}

	public class ServiceProbes
	{
		private static List<string> AllProbes = new List<string>();
		private static Dictionary<int, List<string>> PortSendBuffS = new Dictionary<int, List<string>>();
		private static Dictionary<string, string> PortNames = new Dictionary<string, string>();


		private static void GetPortName()
		{
			string[] array = "1,tcpmux;2,compressnet;3,compressnet;5,rje;7,echo;9,discard;11,systat;13,daytime;15,netstat;17,qotd;18,msp;19,chargen;20,ftp-data;21,ftp;22,ssh;23,telnet;24,priv-mail;25,smtp;26,rsftp;27,nsw-fe;29,msg-icp;31,msg-auth;33,dsp;35,priv-print;37,time;38,rap;39,rlp;41,graphics;42,nameserver;43,whois;44,mpm-flags;45,mpm;46,mpm-snd;47,ni-ftp;48,auditd;49,tacacs;50,re-mail-ck;51,la-maint;52,xns-time;53,domain;54,xns-ch;55,isi-gl;56,xns-auth;57,priv-term;58,xns-mail;59,priv-file;61,ni-mail;62,acas;63,via-ftp;64,covia;65,tacacs-ds;66,sqlnet;67,dhcps;68,dhcpc;69,tftp;70,gopher;71,netrjs-1;72,netrjs-2;73,netrjs-3;74,netrjs-4;75,priv-dial;76,deos;77,priv-rje;78,vettcp;79,finger;80,http;81,hosts2-ns;82,xfer;83,mit-ml-dev;84,ctf;85,mit-ml-dev;86,mfcobol;87,priv-term-l;88,kerberos-sec;89,su-mit-tg;90,dnsix;91,mit-dov;92,npp;93,dcp;94,objcall;95,supdup;96,dixie;97,swift-rvf;98,linuxconf|tacnews;99,metagram;100,newacct;101,hostname;102,iso-tsap;103,gppitnp;104,acr-nema;105,csnet-ns;106,pop3pw|3com-tsmux;107,rtelnet;108,snagas;109,pop2;110,pop3;111,rpcbind;112,mcidas;113,ident|auth;114,audionews;115,sftp;116,ansanotify;117,uucp-path;118,sqlserv;119,nntp;120,cfdptkt;121,erpc;122,smakynet;123,ntp;124,ansatrader;125,locus-map;126,unitary;127,locus-con;128,gss-xlicen;129,pwdgen;130,cisco-fna;131,cisco-tna;132,cisco-sys;133,statsrv;134,ingres-net;135,msrpc;136,profile;137,netbios-ns;138,netbios-dgm;139,netbios-ssn;140,emfis-data;141,emfis-cntl;142,bl-idm;143,imap;144,news;145,uaac;146,iso-tp0;147,iso-ip;148,cronus;149,aed-512;150,sql-net;151,hems;152,bftp;153,sgmp;154,netsc-prod;155,netsc-dev;156,sqlsrv;157,knet-cmp;158,pcmail-srv;159,nss-routing;160,sgmp-traps;161,snmp;162,snmptrap;163,cmip-man;164,cmip-agent|smip-agent;165,xns-courier;166,s-net;167,namp;168,rsvd;169,send;170,print-srv;171,multiplex;172,cl-1;173,xyplex-mux;174,mailq;175,vmnet;176,genrad-mux;177,xdmcp;178,nextstep;179,bgp;180,ris;181,unify;182,audit;183,ocbinder;184,ocserver;185,remote-kis;186,kis;187,aci;188,mumps;189,qft;190,gacp|cacp;191,prospero;192,osu-nms;193,srmp;194,irc;195,dn6-nlm-aud;196,dn6-smm-red;197,dls;198,dls-mon;199,smux;200,src;201,at-rtmp;202,at-nbp;203,at-3;204,at-echo;205,at-5;206,at-zis;207,at-7;208,at-8;209,tam;210,z39.50;211,914c-g;212,anet;213,ipx;214,vmpwscs;215,softpc;216,atls;217,dbase;218,mpp;219,uarps;220,imap3;221,fln-spx;222,rsh-spx;223,cdc;224,masqdialer;242,direct;243,sur-meas;244,dayna;245,link;246,dsp3270;247,subntbcst_tftp;248,bhfhs;256,fw1-secureremote|rap;257,fw1-mc-fwmodule|set;258,fw1-mc-gui|yak-chat;259,esro-gen|firewall1-rdp;260,openport;261,nsiiops;262,arcisdms;263,hdap;264,bgmp|fw1-or-bgmp;265,maybe-fw1|x-bone-ctl;266,sst;267,td-service;268,td-replica;269,manet;270,gist;271,pt-tls;280,http-mgmt;281,personal-link;282,cableport-ax;283,rescap;284,corerjd;286,fxp;287,k-block;308,novastorbakcup;309,entrusttime;310,bhmds;311,asip-webadmin;312,vslmp;313,magenta-logic;314,opalis-robot;315,dpsi;316,decauth;317,zannet;318,pkix-timestamp;319,ptp-event;320,ptp-general;321,pip;322,rtsps;323,rpki-rtr;324,rpki-rtr-tls;333,texar;344,pdap;345,pawserv;346,zserv;347,fatserv;348,csi-sgwp;349,mftp;350,matip-type-a;351,matip-type-b;352,dtag-ste-sb;353,ndsauth;354,bh611;355,datex-asn;356,cloanto-net-1;357,bhevent;358,shrinkwrap;359,tenebris_nts;360,scoi2odialog;361,semantix;362,srssend;363,rsvp_tunnel;364,aurora-cmgr;365,dtk;366,odmr;367,mortgageware;368,qbikgdp;369,rpc2portmap;370,codaauth2;371,clearcase;372,ulistserv;373,legent-1;374,legent-2;375,hassle;376,nip;377,tnETOS;378,dsETOS;379,is99c;380,is99s;381,hp-collector;382,hp-managed-node;383,hp-alarm-mgr;384,arns;385,ibm-app;386,asa;387,aurp;388,unidata-ldm;389,ldap;390,uis;391,synotics-relay;392,synotics-broker;393,dis;394,embl-ndt;395,netcp;396,netware-ip;397,mptn;398,kryptolan;399,iso-tsap-c2;400,work-sol;401,ups;402,genie;403,decap;404,nced;405,ncld;406,imsp;407,timbuktu;408,prm-sm;409,prm-nm;410,decladebug;411,rmt;412,synoptics-trap;413,smsp;414,infoseek;415,bnet;416,silverplatter;417,onmux;418,hyper-g;419,ariel1;420,smpte;421,ariel2;422,ariel3;423,opc-job-start;424,opc-job-track;425,icad-el;426,smartsdp;427,svrloc;428,ocs_cmu;429,ocs_amu;430,utmpsd;431,utmpcd;432,iasd;433,nnsp;434,mobileip-agent;435,mobilip-mn;436,dna-cml;437,comscm;438,dsfgw;439,dasp;440,sgcp;441,decvms-sysmgt;442,cvc_hostd;443,https;444,snpp;445,microsoft-ds;446,ddm-rdb;447,ddm-dfm;448,ddm-ssl;449,as-servermap;450,tserver;451,sfs-smp-net;452,sfs-config;453,creativeserver;454,contentserver;455,creativepartnr;456,macon;457,scohelp;458,appleqtc;459,ampr-rcmd;460,skronk;461,datasurfsrv;462,datasurfsrvsec;463,alpes;464,kpasswd5;465,smtps;466,digital-vrc;467,mylex-mapd;468,photuris;469,rcp;470,scx-proxy;471,mondex;472,ljk-login;473,hybrid-pop;474,tn-tl-w1|tn-tl-w2;475,tcpnethaspsrv;476,tn-tl-fd1;477,ss7ns;478,spsc;479,iafserver;480,loadsrv|iafdbase;481,dvs|ph;482,bgs-nsi|xlog;483,ulpnet;484,integra-sme;485,powerburst;486,sstats|avian;487,saft;488,gss-http;489,nest-protocol;490,micom-pfs;491,go-login;492,ticf-1;493,ticf-2;494,pov-ray;495,intecourier;496,pim-rp-disc;497,retrospect;498,siam;499,iso-ill;500,isakmp;501,stmf;502,mbap;503,intrinsa;504,citadel;505,mailbox-lm;506,ohimsrv;507,crs;508,xvttp;509,snare;510,fcp;511,passgo;512,exec|biff;513,login|who;514,shell|syslog;515,printer;516,videotex;517,talk;518,ntalk;519,utime;520,efs|route;521,ripng;522,ulp;523,ibm-db2;524,ncp;525,timed;526,tempo;527,stx;528,custix;529,irc;530,courier;531,conference;532,netnews;533,netwall;534,mm-admin;535,iiop;536,opalis-rdv;537,nmsp;538,gdomap;539,apertus-ldp;540,uucp;541,uucp-rlogin;542,commerce;543,klogin;544,kshell;545,ekshell|appleqtcsrvr;546,dhcpv6-client;547,dhcpv6-server;548,afp;549,idfp;550,new-rwho;551,cybercash;552,deviceshare;553,pirp;554,rtsp;555,dsf;556,remotefs;557,openvms-sysipc;558,sdnskmp;559,teedtap;560,rmonitor;561,monitor;562,chshell;563,snews;564,9pfs;565,whoami;566,streettalk;567,banyan-rpc;568,ms-shuttle;569,ms-rome;570,meter;571,umeter;572,sonar;573,banyan-vip;574,ftp-agent;575,vemmi;576,ipcd;577,vnas;578,ipdd;579,decbsrv;580,sntp-heartbeat;581,bdp;582,scc-security;583,philips-vc;584,keyserver;585,imap4-ssl;586,password-chg;587,submission;588,cal;589,eyelink;590,tns-cml;591,http-alt;592,eudora-set;593,http-rpc-epmap;594,tpip;595,cab-protocol;596,smsd;597,ptcnameservice;598,sco-websrvrmg3;599,acp;600,ipcserver;601,syslog-conn;602,xmlrpc-beep;603,mnotes|idxp;604,tunnel;605,soap-beep;606,urm;607,nqs;608,sift-uft;609,npmp-trap;610,npmp-local;611,npmp-gui;612,hmmp-ind;613,hmmp-op;614,sshell;615,sco-inetmgr;616,sco-sysmgr;617,sco-dtmgr;618,dei-icda;619,compaq-evm;620,sco-websrvrmgr;621,escp-ip;622,collaborator;623,oob-ws-http|asf-rmcp;624,cryptoadmin;625,apple-xsrvr-admin|dec_dlm;626,apple-imap-admin|serialnumberd;627,passgo-tivoli;628,qmqp;629,3com-amp3;630,rda;631,ipp;632,bmpp;633,servstat;634,ginad;635,rlzdbase|mount;636,ldapssl|ldaps;637,lanserver;638,mcns-sec;639,msdp;640,entrust-sps|pcnfs;641,repcmd;642,esro-emsdp;643,sanity;644,dwr;645,pssc;646,ldp;647,dhcp-failover;648,rrp;649,cadview-3d;650,obex|bwnfs;651,ieee-mms;652,hello-port;653,repscmd;654,aodv;655,tinc;656,spmp;657,rmc;658,tenfold;660,mac-srvr-admin;661,hap;662,pftp;663,purenoise;664,secure-aux-bus;665,sun-dr;666,doom;667,disclose;668,mecomm;669,meregister;670,vacdsm-sws;671,vacdsm-app;672,vpps-qua;673,cimplex;674,acap;675,dctp;676,vpps-via;677,vpp;678,ggf-ncp;679,mrm;680,entrust-aaas;681,entrust-aams;682,xfr;683,corba-iiop;684,corba-iiop-ssl;685,mdc-portmapper;686,hcp-wismar;687,asipregistry;688,realm-rusd;689,nmap;690,vatp;691,resvc|msexch-routing;692,hyperwave-isp;693,connendp;694,ha-cluster;695,ieee-mms-ssl;696,rushd;697,uuidgen;698,olsr;699,accessnetwork;700,epp;701,lmp;702,iris-beep;704,elcsd;705,agentx;706,silc;707,borland-dsj;709,entrustmanager;710,entrust-ash;711,cisco-tdp;712,tbrpf;713,iris-xpc;714,iris-xpcs;715,iris-lwz;716,pana;723,omfs;729,netviewdm1;730,netviewdm2;731,netviewdm3;737,sometimes-rpc2;740,netcp;741,netgw;742,netrcs;744,flexlm;747,fujitsu-dev;748,ris-cm;749,kerberos-adm;750,kerberos;751,kerberos_master;752,qrh;753,rrh;754,krb_prop|tell;758,nlogin;759,con;760,krbupdate|ns;761,kpasswd|rxe;762,quotad;763,cycleserv;764,omserv;765,webster;767,phonebook;769,vid;770,cadlock;771,rtip;772,cycleserv2;773,submit|notify;774,rpasswd|acmaint_dbd;775,entomb|acmaint_transd;776,wpages;777,multiling-http;780,wpgs;781,hp-collector;782,hp-managed-node;783,spamassassin;786,concert;787,qsc;799,controlit;800,mdbs_daemon;801,device;802,mbap-s;808,ccproxy-http;810,fcp-udp;828,itm-mcell-s;829,pkix-3-ca-ra;830,netconf-ssh;831,netconf-beep;832,netconfsoaphttp;833,netconfsoapbeep;847,dhcp-failover2;848,gdoi;853,domain-s;860,iscsi;861,owamp-control;862,twamp-control;871,supfilesrv;873,rsync;886,iclcnet-locate;887,iclcnet_svinfo;888,accessbuilder;898,sun-manageconsole;900,omginitialrefs;901,samba-swat|smpnameres;902,iss-realsecure|ideafarm-door;903,iss-console-mgr|ideafarm-panic;910,kink;911,xact-backup;912,apex-mesh;913,apex-edge;950,oftep-rpc;953,rndc;975,securenetpro-sensor;989,ftps-data;990,ftps;991,nas;992,telnets;993,imaps;994,ircs;995,pop3s;996,xtreelic|vsinet;997,maitrd;998,busboy|puparp;999,garcon|applix;1000,cadlock|ock;1002,windows-icfw;1008,ufsd;1010,surf;1012,sometimes-rpc1;1021,exp1;1022,exp2;1023,netvenuechat;1024,kdm;1025,NFS-or-IIS|blackjack;1026,LSA-or-nterm|win-rpc;1027,IIS;1028,ms-lsa;1029,ms-lsa|solid-mux;1030,iad1;1031,iad2;1032,iad3;1033,netinfo|netinfo-local;1034,zincite-a|activesync-notify;1035,multidropper|mxxrlogin;1036,nsstp;1037,ams;1038,mtqp;1039,sbl;1040,netsaint|netarx;1041,danf-ak2;1042,afrog;1043,boinc;1044,dcutility;1045,fpitp;1046,wfremotertm;1047,neod1;1048,neod2;1049,td-postman;1050,java-or-OTGfileshare|cma;1051,optima-vnet;1052,ddt;1053,remote-as;1054,brvread;1055,ansyslmd;1056,vfo;1057,startron;1058,nim;1059,nimreg;1060,polestar;1061,kiosk;1062,veracity;1063,kyoceranetdev;1064,jstel;1065,syscomlan;1066,fpo-fns;1067,instl_boots;1068,instl_bootc;1069,cognex-insight;1070,gmrupdateserv;1071,bsquare-voip;1072,cardax;1073,bridgecontrol;1074,warmspotMgmt;1075,rdrmshc;1076,sns_credit|dab-sti-c;1077,imgames;1078,avocent-proxy;1079,asprovatalk;1080,socks;1081,pvuniwien;1082,amt-esd-prot;1083,ansoft-lm-1;1084,ansoft-lm-2;1085,webobjects;1086,cplscrambler-lg;1087,cplscrambler-in;1088,cplscrambler-al;1089,ff-annunc;1090,ff-fms;1091,ff-sm;1092,obrpd;1093,proofd;1094,rootd;1095,nicelink;1096,cnrprotocol;1097,sunclustermgr;1098,rmiactivation;1099,rmiregistry;1100,mctp;1101,pt2-discover;1102,adobeserver-1;1103,xaudio|adobeserver-2;1104,xrl;1105,ftranhc;1106,isoipsigport-1;1107,isoipsigport-2;1108,ratio-adp;1109,kpop;1110,nfsd-status|nfsd-keepalive;1111,lmsocialserver;1112,msql|icp;1113,ltp-deepspace;1114,mini-sql;1115,ardus-trns;1116,ardus-cntl;1117,ardus-mtrns;1118,sacred;1119,bnetgame;1120,bnetfile;1121,rmpp;1122,availant-mgr;1123,murray;1124,hpvmmcontrol;1125,hpvmmagent;1126,hpvmmdata;1127,supfiledbg|kwdb-commn;1128,saphostctrl;1129,saphostctrls;1130,casp;1131,caspssl;1132,kvm-via-ip;1133,dfn;1134,aplx;1135,omnivision;1136,hhb-gateway;1137,trim;1138,encrypted_admin;1139,cce3x|evm;1140,autonoc;1141,mxomss;1142,edtools;1143,imyx;1144,fuscript;1145,x9-icue;1146,audit-transfer;1147,capioverlan;1148,elfiq-repl;1149,bvtsonar;1150,blaze;1151,unizensus;1152,winpoplanmess;1153,c1222-acse;1154,resacommunity;1155,nfa;1156,iascontrol-oms;1157,iascontrol;1158,lsnr|dbcontrol-oms;1159,oracle-oms;1160,olsv;1161,health-polling;1162,health-trap;1163,sddp;1164,qsm-proxy;1165,qsm-gui;1166,qsm-remote;1167,cisco-ipsla;1168,vchat;1169,tripwire;1170,atc-lm;1171,atc-appserver;1172,dnap;1173,d-cinema-rrp;1174,fnet-remote-ui;1175,dossier;1176,indigo-server;1177,dkmessenger;1178,skkserv|sgi-storman;1179,b2n;1180,mc-client;1181,3comnetman;1182,accelenet|accelenet-data;1183,llsurfup-http;1184,llsurfup-https;1185,catchpole;1186,mysql-cluster;1187,alias;1188,hp-webadmin;1189,unet;1190,commlinx-avl;1191,gpfs;1192,caids-sensor;1193,fiveacross;1194,openvpn;1195,rsf-1;1196,netmagic;1197,carrius-rshell;1198,cajo-discovery;1199,dmidi;1200,scol;1201,nucleus-sand;1202,caiccipc;1203,ssslic-mgr;1204,ssslog-mgr;1205,accord-mgc;1206,anthony-data;1207,metasage;1208,seagull-ais;1209,ipcd3;1210,eoss;1211,groove-dpp;1212,lupa;1213,mpc-lifenet;1214,fasttrack;1215,scanstat-1;1216,etebac5;1217,hpss-ndapi;1218,aeroflight-ads;1219,aeroflight-ret;1220,quicktime|qt-serveradmin;1221,sweetware-apps;1222,nerv;1223,tgp;1224,vpnz;1225,slinkysearch;1226,stgxfws;1227,dns2go;1228,florence;1229,zented;1230,periscope;1231,menandmice-lpm;1232,first-defense;1233,univ-appserver;1234,hotline|search-agent;1235,mosaicsyssvc1;1236,bvcontrol;1237,tsdos390;1238,hacl-qs;1239,nmsd;1240,instantia;1241,nessus;1242,nmasoverip;1243,serialgateway;1244,isbconference1;1245,isbconference2;1246,payrouter;1247,visionpyramid;1248,hermes;1249,mesavistaco;1250,swldy-sias;1251,servergraph;1252,bspne-pcc;1253,q55-pcc;1254,de-noc;1255,de-cache-query;1256,de-server;1257,shockwave2;1258,opennl;1259,opennl-voice;1260,ibm-ssd;1261,mpshrsv;1262,qnts-orb;1263,dka;1264,prat;1265,dssiapi;1266,dellpwrappks;1267,epc;1268,propel-msgsys;1269,watilapp;1270,ssserver|opsmgr;1271,excw;1272,cspmlockmgr;1273,emc-gateway;1274,t1distproc;1275,ivcollector;1276,ivmanager;1277,miva-mqs;1278,dellwebadmin-1;1279,dellwebadmin-2;1280,pictrography;1281,healthd;1282,emperion;1283,productinfo;1284,iee-qfx;1285,neoiface;1286,netuitive;1287,routematch;1288,navbuddy;1289,jwalkserver;1290,winjaserver;1291,seagulllms;1292,dsdn;1293,pkt-krb-ipsec;1294,cmmdriver;1295,ehtp;1296,dproxy;1297,sdproxy;1298,lpcp;1299,hp-sci;1300,h323hostcallsc;1301,ci3-software-1;1302,ci3-software-2;1303,sftsrv;1304,boomerang;1305,pe-mike;1306,re-conn-proto;1307,pacmand;1308,odsi;1309,jtag-server;1310,husky;1311,rxmon;1312,sti-envision;1313,bmc_patroldb;1314,pdps;1315,els;1316,exbit-escp;1317,vrts-ipcserver;1318,krb5gatekeeper;1319,amx-icsp;1320,amx-axbnet;1321,pip;1322,novation;1323,brcd;1324,delta-mcp;1325,dx-instrument;1326,wimsic;1327,ultrex;1328,ewall;1329,netdb-export;1330,streetperfect;1331,intersan;1332,pcia-rxp-b;1333,passwrd-policy;1334,writesrv;1335,digital-notary;1336,ischat;1337,waste|menandmice-dns;1338,wmc-log-svc;1339,kjtsiteserver;1340,naap;1341,qubes;1342,esbroker;1343,re101;1344,icap;1345,vpjp;1346,alta-ana-lm;1347,bbn-mmc;1348,bbn-mmx;1349,sbook;1350,editbench;1351,equationbuilder;1352,lotusnotes;1353,relief;1354,rightbrain;1355,intuitive-edge;1356,cuillamartin;1357,pegboard;1358,connlcli;1359,ftsrv;1360,mimer;1361,linx;1362,timeflies;1363,ndm-requester;1364,ndm-server;1365,adapt-sna;1366,netware-csp;1367,dcs;1368,screencast;1369,gv-us;1370,us-gv;1371,fc-cli;1372,fc-ser;1373,chromagrafx;1374,molly;1375,bytex;1376,ibm-pps;1377,cichlid;1378,elan;1379,dbreporter;1380,telesis-licman;1381,apple-licman;1382,udt_os;1383,gwha;1384,os-licman;1385,atex_elmd;1386,checksum;1387,cadsi-lm;1388,objective-dbc;1389,iclpv-dm;1390,iclpv-sc;1391,iclpv-sas;1392,iclpv-pm;1393,iclpv-nls;1394,iclpv-nlc;1395,iclpv-wsm;1396,dvl-activemail;1397,audio-activmail;1398,video-activmail;1399,cadkey-licman;1400,cadkey-tablet;1401,goldleaf-licman;1402,prm-sm-np;1403,prm-nm-np;1404,igi-lm;1405,ibm-res;1406,netlabs-lm;1407,dbsa-lm;1408,sophia-lm;1409,here-lm;1410,hiq;1411,af;1412,innosys;1413,innosys-acl;1414,ibm-mqseries;1415,dbstar;1416,novell-lu6.2;1417,timbuktu-srv1;1418,timbuktu-srv2;1419,timbuktu-srv3;1420,timbuktu-srv4;1421,gandalf-lm;1422,autodesk-lm;1423,essbase;1424,hybrid;1425,zion-lm;1426,sas-1;1427,mloadd;1428,informatik-lm;1429,nms;1430,tpdu;1431,rgtp;1432,blueberry-lm;1433,ms-sql-s;1434,ms-sql-m;1435,ibm-cics;1436,sas-2;1437,tabula;1438,eicon-server;1439,eicon-x25;1440,eicon-slp;1441,cadis-1;1442,cadis-2;1443,ies-lm;1444,marcam-lm;1445,proxima-lm;1446,ora-lm;1447,apri-lm;1448,oc-lm;1449,peport;1450,dwf;1451,infoman;1452,gtegsc-lm;1453,genie-lm;1454,interhdl_elmd;1455,esl-lm;1456,dca;1457,valisys-lm;1458,nrcabq-lm;1459,proshare1;1460,proshare2;1461,ibm_wrless_lan;1462,world-lm;1463,nucleus;1464,msl_lmd;1465,pipes;1466,oceansoft-lm;1467,csdmbase;1468,csdm;1469,aal-lm;1470,uaiact;1471,csdmbase;1472,csdm;1473,openmath;1474,telefinder;1475,taligent-lm;1476,clvm-cfg;1477,ms-sna-server;1478,ms-sna-base;1479,dberegister;1480,pacerforum;1481,airs;1482,miteksys-lm;1483,afs;1484,confluent;1485,lansource;1486,nms_topo_serv;1487,localinfosrvr;1488,docstor;1489,dmdocbroker;1490,insitu-conf;1491,anynetgateway;1492,stone-design-1;1493,netmap_lm;1494,citrix-ica;1495,cvc;1496,liberty-lm;1497,rfx-lm;1498,watcom-sql;1499,fhc;1500,vlsi-lm;1501,sas-3;1502,shivadiscovery;1503,imtc-mcs;1504,evb-elm;1505,funkproxy;1506,utcd;1507,symplex;1508,diagmond;1509,robcad-lm;1510,mvx-lm;1511,3l-l1;1512,wins;1513,fujitsu-dtc;1514,fujitsu-dtcns;1515,ifor-protocol;1516,vpad;1517,vpac;1518,vpvd;1519,vpvc;1520,atm-zip-office;1521,oracle|ncube-lm;1522,rna-lm;1523,cichild-lm;1524,ingreslock;1525,orasrv|oracle;1526,pdap-np;1527,tlisrv;1528,mciautoreg;1529,support|coauthor;1530,rap-service;1531,rap-listen;1532,miroconnect;1533,virtual-places;1534,micromuse-lm;1535,ampr-info;1536,ampr-inter;1537,sdsc-lm;1538,3ds-lm;1539,intellistor-lm;1540,rds;1541,rds2;1542,gridgen-elmd;1543,simba-cs;1544,aspeclmd;1545,vistium-share;1546,abbaccuray;1547,laplink;1548,axon-lm;1549,shivahose|shivasound;1550,3m-image-lm;1551,hecmtl-db;1552,pciarray;1553,sna-cs;1554,caci-lm;1555,livelan;1556,veritas_pbx;1557,arbortext-lm;1558,xingmpeg;1559,web2host;1560,asci-val;1561,facilityview;1562,pconnectmgr;1563,cadabra-lm;1564,pay-per-view;1565,winddlb;1566,corelvideo;1567,jlicelmd;1568,tsspmap;1569,ets;1570,orbixd;1571,rdb-dbs-disp;1572,chip-lm;1573,itscomm-ns;1574,mvel-lm;1575,oraclenames;1576,moldflow-lm;1577,hypercube-lm;1578,jacobus-lm;1579,ioc-sea-lm;1580,tn-tl-r1|tn-tl-r2;1581,mil-2045-47001;1582,msims;1583,simbaexpress;1584,tn-tl-fd2;1585,intv;1586,ibm-abtact;1587,pra_elmd;1588,triquest-lm;1589,vqp;1590,gemini-lm;1591,ncpm-pm;1592,commonspace;1593,mainsoft-lm;1594,sixtrak;1595,radio;1596,radio-sm|radio-bc;1597,orbplus-iiop;1598,picknfs;1599,simbaservices;1600,issd;1601,aas;1602,inspect;1603,picodbc;1604,icabrowser;1605,slp;1606,slm-api;1607,stt;1608,smart-lm;1609,isysg-lm;1610,taurus-wh;1611,ill;1612,netbill-trans;1613,netbill-keyrep;1614,netbill-cred;1615,netbill-auth;1616,netbill-prod;1617,nimrod-agent;1618,skytelnet;1619,xs-openstorage;1620,faxportwinport;1621,softdataphone;1622,ontime;1623,jaleosnd;1624,udp-sr-port;1625,svs-omagent;1626,shockwave;1627,t128-gateway;1628,lontalk-norm;1629,lontalk-urgnt;1630,oraclenet8cman;1631,visitview;1632,pammratc;1633,pammrpc;1634,loaprobe;1635,edb-server1;1636,isdc;1637,islc;1638,ismc;1639,cert-initiator;1640,cert-responder;1641,invision;1642,isis-am;1643,isis-ambc;1644,saiseh;1645,sightline|radius;1646,sa-msg-port|radacct;1647,rsap;1648,concurrent-lm;1649,kermit;1650,nkd;1651,shiva_confsrvr;1652,xnmp;1653,alphatech-lm;1654,stargatealerts;1655,dec-mbadmin;1656,dec-mbadmin-h;1657,fujitsu-mmpdc;1658,sixnetudr;1659,sg-lm;1660,skip-mc-gikreq;1661,netview-aix-1;1662,netview-aix-2;1663,netview-aix-3;1664,netview-aix-4;1665,netview-aix-5;1666,netview-aix-6;1667,netview-aix-7;1668,netview-aix-8;1669,netview-aix-9;1670,netview-aix-10;1671,netview-aix-11;1672,netview-aix-12;1673,proshare-mc-1;1674,proshare-mc-2;1675,pdp;1676,netcomm1|netcomm2;1677,groupwise;1678,prolink;1679,darcorp-lm;1680,CarbonCopy|microcom-sbp;1681,sd-elmd;1682,lanyon-lantern;1683,ncpm-hip;1684,snaresecure;1685,n2nremote;1686,cvmon;1687,nsjtp-ctrl;1688,nsjtp-data;1689,firefox;1690,ng-umds;1691,empire-empuma;1692,sstsys-lm;1693,rrirtr;1694,rrimwm;1695,rrilwm;1696,rrifmm;1697,rrisat;1698,rsvp-encap-1;1699,rsvp-encap-2;1700,mps-raft;1701,l2f|L2TP;1702,deskshare;1703,hb-engine;1704,bcs-broker;1705,slingshot;1706,jetform;1707,vdmplay;1708,gat-lmd;1709,centra;1710,impera;1711,pptconference;1712,registrar;1713,conferencetalk;1714,sesi-lm;1715,houdini-lm;1716,xmsg;1717,fj-hdnet;1718,h323gatedisc|h225gatedisc;1719,h323gatestat;1720,h323q931|h323hostcall;1721,caicci;1722,hks-lm;1723,pptp;1724,csbphonemaster;1725,iden-ralp;1726,iberiagames;1727,winddx;1728,telindus;1729,[...string is too long...]".Split(new char[]
			{
				';'
			});
			checked
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					ServiceProbes.PortNames.Add(array2[0], array2[1]);
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002788 File Offset: 0x00000988
		public static void Init(string strFilePath)
		{
			ServiceProbes.GetPortName();
			ServiceProbes.AllProbes.Clear();
			ServiceProbes.PortSendBuffS.Clear();
			List<string> list = new List<string>();
			try
			{
				foreach (string text in File.ReadLines(strFilePath))
				{
					if (text.StartsWith("match "))
					{
						string text2 = Regex.Match(text, "(?<=\\sm[\\||\\%|\\=]+).*(?=[\\||\\%|\\=]+)", RegexOptions.IgnoreCase).Value;
						if (text2.Length > 0)
						{
							string[] array = text.Split(new char[]
							{
								' '
							});
							ServiceProbes.AllProbes.Add(array[1] + "\t" + ServiceProbes.HexStringToString(text2));
						}
					}
					if (text.StartsWith("Probe ") | text.StartsWith("ports "))
					{
						list.Add(text);
					}
				}
			}
			catch
			{
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = list.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (list[i].StartsWith("Probe TCP NULL"))
				{
					dictionary.Add(list[i], "");
				}
				else if (list[i].StartsWith("Probe "))
				{
					dictionary.Add(list[i], list[i + 1]);
				}
			}
			int num2 = 1;
			do
			{
				ServiceProbes.PortSendBuffS.Add(num2, new List<string>());
				num2++;
			}
			while (num2 <= 65535);
			try
			{
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					if (keyValuePair.Key.StartsWith("Probe TCP ") && keyValuePair.Value.StartsWith("ports "))
					{
						string text2 = keyValuePair.Value.Replace("ports ", "");
						foreach (string text3 in text2.Split(new char[]
						{
							','
						}))
						{
							if (text3.Contains("-"))
							{
								string[] array = text3.Split(new char[]
								{
									'-'
								});
								int num3 = (int)Math.Round(Convert.ToDouble(array[0]));
								int num4 = (int)Math.Round(Convert.ToDouble(array[1]));
								for (int k = num3; k <= num4; k++)
								{
									ServiceProbes.PortSendBuffS[k].Add(keyValuePair.Key.Replace("Probe TCP ", ""));
								}
							}
							else
							{
								int key = (int)Math.Round(Convert.ToDouble(text3));
								ServiceProbes.PortSendBuffS[key].Add(keyValuePair.Key.Replace("Probe TCP ", ""));
							}
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				foreach (KeyValuePair<int, List<string>> keyValuePair2 in new Dictionary<int, List<string>>(ServiceProbes.PortSendBuffS))
				{
					if (keyValuePair2.Value.Count == 0)
					{
						ServiceProbes.PortSendBuffS.Remove(keyValuePair2.Key);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000021CB File Offset: 0x000003CB
		public static void Dispose()
		{
			ServiceProbes.AllProbes.Clear();
			ServiceProbes.PortSendBuffS.Clear();
			ServiceProbes.PortNames.Clear();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002AFC File Offset: 0x00000CFC
		public static string GetServerName(string PortData)
		{
			try
			{
				foreach (string text in ServiceProbes.AllProbes)
				{
					string[] array = text.Split(new char[]
					{
						'\t'
					});
					if (array.Length == 2)
					{
						string input = ServiceProbes.HexStringToString(PortData);
						string pattern = array[1];
						try
						{
							MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.IgnoreCase);
							foreach (Match match in matches)
							{
								if (match.Value.Length > 2)
								{
									return array[0];
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
			return "";
		}

		public static string GetPortName(string port)
		{
			string result;
			if (ServiceProbes.PortNames.ContainsKey(port))
			{
				result = ServiceProbes.PortNames[port];
			}
			else
			{
				result = "-";
			}
			return result;
		}

        public static string BytesToHexString(byte[] Bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            checked
            {
                while (i < Bytes.Length)
                {
                    byte b = Bytes[i];
                    byte b2 = b;
                    if ((b2 >= 32 && b2 <= 126) || b2 == 13 || b2 == 10)
                    {
                        goto IL_BE;
                    }
                    if (b2 == 9)
                    {
                        goto IL_BE;
                    }
                    if (b2 == 0)
                    {
                        stringBuilder.Append("\\0");
                    }
                    else if (b2 == 12)
                    {
                        stringBuilder.Append("\\f");
                    }
                    else if (b2 == 11)
                    {
                        stringBuilder.Append("\\v");
                    }
                    else if (b2 == 8)
                    {
                        stringBuilder.Append("\\b");
                    }
                    else if (b2 == 7)
                    {
                        stringBuilder.Append("\\a");
                    }
                    else
                    {
                        stringBuilder.Append("\\x" + b.ToString("X2"));
                    }
                IL_CB:
                    i++;
                    continue;
                IL_BE:
                    //......
                    stringBuilder.Append(Strings.ChrW((int)b));
                    goto IL_CB;
                }
                return stringBuilder.ToString();
            }
        }

        public static string HexStringToString(string text)
        {
            Queue<char> queue = new Queue<char>(text);
            StringBuilder stringBuilder = new StringBuilder();
            while (queue.Count > 0)
            {
                char value = queue.Dequeue();
                //if (Operators.CompareString(Conversions.ToString(value), "\\", false) == 0)
                if (value != '\\')
                {
                    if (queue.Count == 0)
                    {
                        stringBuilder.Append(value);
                    }
                    else
                    {
                        char c = queue.Dequeue();
                        if (c <= 'n')
                        {
                            if (c == '0')
                            {
                                stringBuilder.Append('\0');
                                continue;
                            }
                            if (c == 'n')
                            {
                                stringBuilder.Append('\n');
                                continue;
                            }
                        }
                        else
                        {
                            if (c == 'r')
                            {
                                stringBuilder.Append('\r');
                                continue;
                            }
                            if (c == 't')
                            {
                                stringBuilder.Append('\t');
                                continue;
                            }
                            if (c == 'x')
                            {
								if (queue.Count > 0)
								{
									char value2 = queue.Dequeue();
									if (queue.Count > 0)
									{
										char value3 = queue.Dequeue();
										//......
										//stringBuilder.Append(Strings.ChrW(int.Parse("&H" + Convert.ToString(value2) + Convert.ToString(value3), System.Globalization.NumberStyles.HexNumber)));
										stringBuilder.Append(Strings.ChrW(Conversions.ToInteger(Conversion.Val("&H" + Conversions.ToString(value2) + Conversions.ToString(value3)))));

										continue;
									}
									else
                                    {
										stringBuilder.Append("\\x" + Convert.ToString(value2));
										continue;
                                    }
								}
								else
                                {
									stringBuilder.Append("\\x");
									continue;
                                }
                            }
                        }
                        stringBuilder.Append(Convert.ToString(value) + Convert.ToString(c));
                    }
                }
                else
                {
                    stringBuilder.Append(value);
                }
            }
            return stringBuilder.ToString();
        }

        // Token: 0x06000020 RID: 32 RVA: 0x00002E24 File Offset: 0x00001024
        public static byte[] HexStringTobytes(string text)
		{
			List<byte> list = new List<byte>();
			foreach (char c in Regex.Unescape(text))
			{
				list.Add((byte)c);
			}
			return list.ToArray();
		}
	}
}
