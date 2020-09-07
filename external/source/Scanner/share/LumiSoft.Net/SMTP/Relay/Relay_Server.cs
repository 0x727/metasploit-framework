using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.Log;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000146 RID: 326
	public class Relay_Server : IDisposable
	{
		// Token: 0x06000CC3 RID: 3267 RVA: 0x0004E950 File Offset: 0x0004D950
		public Relay_Server()
		{
			this.m_pQueues = new List<Relay_Queue>();
			this.m_pSmartHosts = new CircleCollection<Relay_SmartHost>();
			this.m_pDsnClient = new Dns_Client();
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x0004EA24 File Offset: 0x0004DA24
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				try
				{
					bool isRunning = this.m_IsRunning;
					if (isRunning)
					{
						this.Stop();
					}
				}
				catch
				{
				}
				this.m_IsDisposed = true;
				this.Error = null;
				this.SessionCompleted = null;
				this.m_pQueues = null;
				this.m_pSmartHosts = null;
				this.m_pDsnClient.Dispose();
				this.m_pDsnClient = null;
			}
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x0004EAA4 File Offset: 0x0004DAA4
		private void m_pTimerTimeout_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				foreach (Relay_Session relay_Session in this.Sessions.ToArray())
				{
					try
					{
						bool flag = relay_Session.LastActivity.AddSeconds((double)this.m_SessionIdleTimeout) < DateTime.Now;
						if (flag)
						{
							relay_Session.Dispose(new Exception("Session idle timeout."));
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
		}

		// Token: 0x06000CC6 RID: 3270 RVA: 0x0004EB44 File Offset: 0x0004DB44
		public virtual void Start()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				this.m_pLocalEndPointIPv4 = new CircleCollection<IPBindInfo>();
				this.m_pLocalEndPointIPv6 = new CircleCollection<IPBindInfo>();
				this.m_pSessions = new TCP_SessionCollection<Relay_Session>();
				this.m_pConnectionsPerIP = new Dictionary<IPAddress, long>();
				Thread thread = new Thread(new ThreadStart(this.Run));
				thread.Start();
				this.m_pTimerTimeout = new TimerEx(30000.0);
				this.m_pTimerTimeout.Elapsed += this.m_pTimerTimeout_Elapsed;
				this.m_pTimerTimeout.Start();
			}
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0004EC04 File Offset: 0x0004DC04
		public virtual void Stop()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.m_IsRunning;
			if (!flag)
			{
				this.m_IsRunning = false;
				this.m_pLocalEndPointIPv4 = null;
				this.m_pLocalEndPointIPv6 = null;
				this.m_pSessions = null;
				this.m_pConnectionsPerIP = null;
				this.m_pTimerTimeout.Dispose();
				this.m_pTimerTimeout = null;
			}
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0004EC74 File Offset: 0x0004DC74
		private void Run()
		{
			while (this.m_IsRunning)
			{
				try
				{
					bool hasBindingsChanged = this.m_HasBindingsChanged;
					if (hasBindingsChanged)
					{
						this.m_pLocalEndPointIPv4.Clear();
						this.m_pLocalEndPointIPv6.Clear();
						foreach (IPBindInfo ipbindInfo in this.m_pBindings)
						{
							bool flag = ipbindInfo.IP == IPAddress.Any;
							if (flag)
							{
								foreach (IPAddress ipaddress in Dns.GetHostAddresses(""))
								{
									bool flag2 = ipaddress.AddressFamily == AddressFamily.InterNetwork;
									if (flag2)
									{
										IPBindInfo item = new IPBindInfo(ipbindInfo.HostName, ipbindInfo.Protocol, ipaddress, 25);
										bool flag3 = !this.m_pLocalEndPointIPv4.Contains(item);
										if (flag3)
										{
											this.m_pLocalEndPointIPv4.Add(item);
										}
									}
								}
							}
							else
							{
								bool flag4 = ipbindInfo.IP == IPAddress.IPv6Any;
								if (flag4)
								{
									foreach (IPAddress ipaddress2 in Dns.GetHostAddresses(""))
									{
										bool flag5 = ipaddress2.AddressFamily == AddressFamily.InterNetworkV6;
										if (flag5)
										{
											IPBindInfo item2 = new IPBindInfo(ipbindInfo.HostName, ipbindInfo.Protocol, ipaddress2, 25);
											bool flag6 = !this.m_pLocalEndPointIPv6.Contains(item2);
											if (flag6)
											{
												this.m_pLocalEndPointIPv6.Add(item2);
											}
										}
									}
								}
								else
								{
									IPBindInfo item3 = new IPBindInfo(ipbindInfo.HostName, ipbindInfo.Protocol, ipbindInfo.IP, 25);
									bool flag7 = ipbindInfo.IP.AddressFamily == AddressFamily.InterNetwork;
									if (flag7)
									{
										bool flag8 = !this.m_pLocalEndPointIPv4.Contains(item3);
										if (flag8)
										{
											this.m_pLocalEndPointIPv4.Add(item3);
										}
									}
									else
									{
										bool flag9 = !this.m_pLocalEndPointIPv6.Contains(item3);
										if (flag9)
										{
											this.m_pLocalEndPointIPv6.Add(item3);
										}
									}
								}
							}
						}
						this.m_HasBindingsChanged = false;
					}
					bool flag10 = this.m_pLocalEndPointIPv4.Count == 0 && this.m_pLocalEndPointIPv6.Count == 0;
					if (flag10)
					{
						Thread.Sleep(10);
					}
					else
					{
						bool flag11 = this.m_MaxConnections != 0L && (long)this.m_pSessions.Count >= this.m_MaxConnections;
						if (flag11)
						{
							Thread.Sleep(10);
						}
						else
						{
							Relay_QueueItem relay_QueueItem = null;
							foreach (Relay_Queue relay_Queue in this.m_pQueues)
							{
								relay_QueueItem = relay_Queue.DequeueMessage();
								bool flag12 = relay_QueueItem != null;
								if (flag12)
								{
									break;
								}
							}
							bool flag13 = relay_QueueItem == null;
							if (flag13)
							{
								Thread.Sleep(10);
							}
							else
							{
								bool flag14 = relay_QueueItem.TargetServer != null;
								if (flag14)
								{
									Relay_Session relay_Session = new Relay_Session(this, relay_QueueItem, new Relay_SmartHost[]
									{
										relay_QueueItem.TargetServer
									});
									this.m_pSessions.Add(relay_Session);
									ThreadPool.QueueUserWorkItem(new WaitCallback(relay_Session.Start));
								}
								else
								{
									bool flag15 = this.m_RelayMode == Relay_Mode.Dns;
									if (flag15)
									{
										Relay_Session relay_Session2 = new Relay_Session(this, relay_QueueItem);
										this.m_pSessions.Add(relay_Session2);
										ThreadPool.QueueUserWorkItem(new WaitCallback(relay_Session2.Start));
									}
									else
									{
										bool flag16 = this.m_RelayMode == Relay_Mode.SmartHost;
										if (flag16)
										{
											bool flag17 = this.m_SmartHostsBalanceMode == BalanceMode.FailOver;
											Relay_SmartHost[] smartHosts;
											if (flag17)
											{
												smartHosts = this.m_pSmartHosts.ToArray();
											}
											else
											{
												smartHosts = this.m_pSmartHosts.ToCurrentOrderArray();
											}
											Relay_Session relay_Session3 = new Relay_Session(this, relay_QueueItem, smartHosts);
											this.m_pSessions.Add(relay_Session3);
											ThreadPool.QueueUserWorkItem(new WaitCallback(relay_Session3.Start));
										}
									}
								}
							}
						}
					}
				}
				catch (Exception x)
				{
					this.OnError(x);
				}
			}
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0004F0B4 File Offset: 0x0004E0B4
		internal IPBindInfo GetLocalBinding(IPAddress remoteIP)
		{
			bool flag = remoteIP == null;
			if (flag)
			{
				throw new ArgumentNullException("remoteIP");
			}
			bool flag2 = remoteIP.AddressFamily == AddressFamily.InterNetworkV6;
			IPBindInfo result;
			if (flag2)
			{
				bool flag3 = this.m_pLocalEndPointIPv6.Count == 0;
				if (flag3)
				{
					result = null;
				}
				else
				{
					result = this.m_pLocalEndPointIPv6.Next();
				}
			}
			else
			{
				bool flag4 = this.m_pLocalEndPointIPv4.Count == 0;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = this.m_pLocalEndPointIPv4.Next();
				}
			}
			return result;
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0004F138 File Offset: 0x0004E138
		internal bool TryAddIpUsage(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			Dictionary<IPAddress, long> pConnectionsPerIP = this.m_pConnectionsPerIP;
			bool result;
			lock (pConnectionsPerIP)
			{
				long num = 0L;
				bool flag3 = this.m_pConnectionsPerIP.TryGetValue(ip, out num);
				if (flag3)
				{
					bool flag4 = this.m_MaxConnectionsPerIP > 0L && num >= this.m_MaxConnectionsPerIP;
					if (flag4)
					{
						return false;
					}
					this.m_pConnectionsPerIP[ip] = num + 1L;
				}
				else
				{
					this.m_pConnectionsPerIP.Add(ip, 1L);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0004F1F4 File Offset: 0x0004E1F4
		internal void RemoveIpUsage(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			Dictionary<IPAddress, long> pConnectionsPerIP = this.m_pConnectionsPerIP;
			lock (pConnectionsPerIP)
			{
				long num = 0L;
				bool flag3 = this.m_pConnectionsPerIP.TryGetValue(ip, out num);
				if (flag3)
				{
					bool flag4 = num == 1L;
					if (flag4)
					{
						this.m_pConnectionsPerIP.Remove(ip);
					}
					else
					{
						this.m_pConnectionsPerIP[ip] = num - 1L;
					}
				}
			}
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x0004F294 File Offset: 0x0004E294
		internal long GetIpUsage(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			Dictionary<IPAddress, long> pConnectionsPerIP = this.m_pConnectionsPerIP;
			long result;
			lock (pConnectionsPerIP)
			{
				long num = 0L;
				bool flag3 = this.m_pConnectionsPerIP.TryGetValue(ip, out num);
				if (flag3)
				{
					result = num;
				}
				else
				{
					result = 0L;
				}
			}
			return result;
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x0004F30C File Offset: 0x0004E30C
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000CCE RID: 3278 RVA: 0x0004F324 File Offset: 0x0004E324
		public bool IsRunning
		{
			get
			{
				return this.m_IsRunning;
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x0004F33C File Offset: 0x0004E33C
		// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x0004F370 File Offset: 0x0004E370
		public IPBindInfo[] Bindings
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pBindings;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					value = new IPBindInfo[0];
				}
				bool flag2 = false;
				bool flag3 = this.m_pBindings.Length != value.Length;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					for (int i = 0; i < this.m_pBindings.Length; i++)
					{
						bool flag4 = !this.m_pBindings[i].Equals(value[i]);
						if (flag4)
						{
							flag2 = true;
							break;
						}
					}
				}
				bool flag5 = flag2;
				if (flag5)
				{
					this.m_pBindings = value;
					this.m_HasBindingsChanged = true;
				}
			}
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0004F420 File Offset: 0x0004E420
		// (set) Token: 0x06000CD2 RID: 3282 RVA: 0x0004F454 File Offset: 0x0004E454
		public Relay_Mode RelayMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RelayMode;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_RelayMode = value;
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000CD3 RID: 3283 RVA: 0x0004F488 File Offset: 0x0004E488
		public List<Relay_Queue> Queues
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pQueues;
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000CD4 RID: 3284 RVA: 0x0004F4BC File Offset: 0x0004E4BC
		// (set) Token: 0x06000CD5 RID: 3285 RVA: 0x0004F4F0 File Offset: 0x0004E4F0
		public BalanceMode SmartHostsBalanceMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SmartHostsBalanceMode;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_SmartHostsBalanceMode = value;
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000CD6 RID: 3286 RVA: 0x0004F524 File Offset: 0x0004E524
		// (set) Token: 0x06000CD7 RID: 3287 RVA: 0x0004F560 File Offset: 0x0004E560
		public Relay_SmartHost[] SmartHosts
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmartHosts.ToArray();
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("SmartHosts");
				}
				this.m_pSmartHosts.Add(value);
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000CD8 RID: 3288 RVA: 0x0004F5AC File Offset: 0x0004E5AC
		// (set) Token: 0x06000CD9 RID: 3289 RVA: 0x0004F5E0 File Offset: 0x0004E5E0
		public long MaxConnections
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxConnections;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxConnections' value must be >= 0.");
				}
				this.m_MaxConnections = value;
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0004F628 File Offset: 0x0004E628
		// (set) Token: 0x06000CDB RID: 3291 RVA: 0x0004F65C File Offset: 0x0004E65C
		public long MaxConnectionsPerIP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxConnectionsPerIP;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_MaxConnectionsPerIP < 0L;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxConnectionsPerIP' value must be >= 0.");
				}
				this.m_MaxConnectionsPerIP = value;
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000CDC RID: 3292 RVA: 0x0004F6A8 File Offset: 0x0004E6A8
		// (set) Token: 0x06000CDD RID: 3293 RVA: 0x0004F6DC File Offset: 0x0004E6DC
		public bool UseTlsIfPossible
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_UseTlsIfPossible;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_UseTlsIfPossible = value;
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0004F710 File Offset: 0x0004E710
		// (set) Token: 0x06000CDF RID: 3295 RVA: 0x0004F744 File Offset: 0x0004E744
		public int SessionIdleTimeout
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SessionIdleTimeout;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_SessionIdleTimeout < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'SessionIdleTimeout' value must be >= 0.");
				}
				this.m_SessionIdleTimeout = value;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000CE0 RID: 3296 RVA: 0x0004F790 File Offset: 0x0004E790
		public TCP_SessionCollection<Relay_Session> Sessions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("Relay server not running.");
				}
				return this.m_pSessions;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000CE1 RID: 3297 RVA: 0x0004F7E0 File Offset: 0x0004E7E0
		// (set) Token: 0x06000CE2 RID: 3298 RVA: 0x0004F7F8 File Offset: 0x0004E7F8
		public Logger Logger
		{
			get
			{
				return this.m_pLogger;
			}
			set
			{
				this.m_pLogger = value;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x0004F804 File Offset: 0x0004E804
		// (set) Token: 0x06000CE4 RID: 3300 RVA: 0x0004F838 File Offset: 0x0004E838
		public Dns_Client DnsClient
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pDsnClient;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("DnsClient");
				}
				this.m_pDsnClient = value;
			}
		}

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x06000CE5 RID: 3301 RVA: 0x0004F880 File Offset: 0x0004E880
		// (remove) Token: 0x06000CE6 RID: 3302 RVA: 0x0004F8B8 File Offset: 0x0004E8B8
		
		public event Relay_SessionCompletedEventHandler SessionCompleted = null;

		// Token: 0x06000CE7 RID: 3303 RVA: 0x0004F8F0 File Offset: 0x0004E8F0
		protected internal virtual void OnSessionCompleted(Relay_Session session, Exception exception)
		{
			bool flag = this.SessionCompleted != null;
			if (flag)
			{
				this.SessionCompleted(new Relay_SessionCompletedEventArgs(session, exception));
			}
		}

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x06000CE8 RID: 3304 RVA: 0x0004F920 File Offset: 0x0004E920
		// (remove) Token: 0x06000CE9 RID: 3305 RVA: 0x0004F958 File Offset: 0x0004E958
		
		public event ErrorEventHandler Error = null;

		// Token: 0x06000CEA RID: 3306 RVA: 0x0004F990 File Offset: 0x0004E990
		protected internal virtual void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new Error_EventArgs(x, new StackTrace()));
			}
		}

		// Token: 0x0400056C RID: 1388
		private bool m_IsDisposed = false;

		// Token: 0x0400056D RID: 1389
		private bool m_IsRunning = false;

		// Token: 0x0400056E RID: 1390
		private IPBindInfo[] m_pBindings = new IPBindInfo[0];

		// Token: 0x0400056F RID: 1391
		private bool m_HasBindingsChanged = false;

		// Token: 0x04000570 RID: 1392
		private Relay_Mode m_RelayMode = Relay_Mode.Dns;

		// Token: 0x04000571 RID: 1393
		private List<Relay_Queue> m_pQueues = null;

		// Token: 0x04000572 RID: 1394
		private BalanceMode m_SmartHostsBalanceMode = BalanceMode.LoadBalance;

		// Token: 0x04000573 RID: 1395
		private CircleCollection<Relay_SmartHost> m_pSmartHosts = null;

		// Token: 0x04000574 RID: 1396
		private CircleCollection<IPBindInfo> m_pLocalEndPointIPv4 = null;

		// Token: 0x04000575 RID: 1397
		private CircleCollection<IPBindInfo> m_pLocalEndPointIPv6 = null;

		// Token: 0x04000576 RID: 1398
		private long m_MaxConnections = 0L;

		// Token: 0x04000577 RID: 1399
		private long m_MaxConnectionsPerIP = 0L;

		// Token: 0x04000578 RID: 1400
		private bool m_UseTlsIfPossible = false;

		// Token: 0x04000579 RID: 1401
		private Dns_Client m_pDsnClient = null;

		// Token: 0x0400057A RID: 1402
		private TCP_SessionCollection<Relay_Session> m_pSessions = null;

		// Token: 0x0400057B RID: 1403
		private Dictionary<IPAddress, long> m_pConnectionsPerIP = null;

		// Token: 0x0400057C RID: 1404
		private int m_SessionIdleTimeout = 30;

		// Token: 0x0400057D RID: 1405
		private TimerEx m_pTimerTimeout = null;

		// Token: 0x0400057E RID: 1406
		private Logger m_pLogger = null;
	}
}
