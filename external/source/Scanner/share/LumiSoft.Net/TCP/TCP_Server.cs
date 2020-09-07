using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using LumiSoft.Net.Log;

namespace LumiSoft.Net.TCP
{
	// Token: 0x02000031 RID: 49
	public class TCP_Server<T> : IDisposable where T : TCP_ServerSession, new()
	{
		// Token: 0x0600018C RID: 396 RVA: 0x0000A840 File Offset: 0x00009840
		public TCP_Server()
		{
			this.m_pConnectionAcceptors = new List<TCP_Server<T>.TCP_Acceptor>();
			this.m_pListeningPoints = new List<TCP_Server<T>.ListeningPoint>();
			this.m_pSessions = new TCP_SessionCollection<TCP_ServerSession>();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000A8F8 File Offset: 0x000098F8
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				bool isRunning = this.m_IsRunning;
				if (isRunning)
				{
					try
					{
						this.Stop();
					}
					catch
					{
					}
				}
				this.m_IsDisposed = true;
				try
				{
					this.OnDisposed();
				}
				catch
				{
				}
				this.m_pSessions = null;
				this.Started = null;
				this.Stopped = null;
				this.Disposed = null;
				this.Error = null;
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000A988 File Offset: 0x00009988
		private void m_pTimer_IdleTimeout_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				TCP_ServerSession[] array = this.Sessions.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					T t = (T)((object)array[i]);
					try
					{
						bool flag = DateTime.Now > t.TcpStream.LastActivity.AddSeconds((double)this.m_SessionIdleTimeout);
						if (flag)
						{
							t.OnTimeoutI();
							bool flag2 = !t.IsDisposed;
							if (flag2)
							{
								t.Disconnect();
								t.Dispose();
							}
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

		// Token: 0x0600018F RID: 399 RVA: 0x0000AA68 File Offset: 0x00009A68
		public void Start()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("TCP_Server");
			}
			bool isRunning = this.m_IsRunning;
			if (!isRunning)
			{
				this.m_IsRunning = true;
				this.m_StartTime = DateTime.Now;
				this.m_ConnectionsProcessed = 0L;
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					this.StartListen();
				});
				this.m_pTimer_IdleTimeout = new TimerEx(30000.0, true);
				this.m_pTimer_IdleTimeout.Elapsed += this.m_pTimer_IdleTimeout_Elapsed;
				this.m_pTimer_IdleTimeout.Enabled = true;
				this.OnStarted();
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000AB08 File Offset: 0x00009B08
		public void Stop()
		{
			bool flag = !this.m_IsRunning;
			if (!flag)
			{
				this.m_IsRunning = false;
				foreach (TCP_Server<T>.TCP_Acceptor tcp_Acceptor in this.m_pConnectionAcceptors.ToArray())
				{
					try
					{
						tcp_Acceptor.Dispose();
					}
					catch (Exception x)
					{
						this.OnError(x);
					}
				}
				this.m_pConnectionAcceptors.Clear();
				foreach (TCP_Server<T>.ListeningPoint listeningPoint in this.m_pListeningPoints.ToArray())
				{
					try
					{
						listeningPoint.Socket.Close();
					}
					catch (Exception x2)
					{
						this.OnError(x2);
					}
				}
				this.m_pListeningPoints.Clear();
				this.m_pTimer_IdleTimeout.Dispose();
				this.m_pTimer_IdleTimeout = null;
				this.OnStopped();
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000AC04 File Offset: 0x00009C04
		public void Restart()
		{
			this.Stop();
			this.Start();
		}

		// Token: 0x06000192 RID: 402 RVA: 0x000091B8 File Offset: 0x000081B8
		protected virtual void OnMaxConnectionsExceeded(T session)
		{
		}

		// Token: 0x06000193 RID: 403 RVA: 0x000091B8 File Offset: 0x000081B8
		protected virtual void OnMaxConnectionsPerIPExceeded(T session)
		{
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000AC18 File Offset: 0x00009C18
		private void StartListen()
		{
			try
			{
				foreach (TCP_Server<T>.ListeningPoint listeningPoint in this.m_pListeningPoints.ToArray())
				{
					try
					{
						listeningPoint.Socket.Close();
					}
					catch (Exception x)
					{
						this.OnError(x);
					}
				}
				this.m_pListeningPoints.Clear();
				IPBindInfo[] pBindings = this.m_pBindings;
				int j = 0;
				while (j < pBindings.Length)
				{
					IPBindInfo ipbindInfo = pBindings[j];
					try
					{
						bool flag = ipbindInfo.IP.AddressFamily == AddressFamily.InterNetwork;
						Socket socket;
						if (flag)
						{
							socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						}
						else
						{
							bool flag2 = ipbindInfo.IP.AddressFamily == AddressFamily.InterNetworkV6;
							if (!flag2)
							{
								goto IL_1B0;
							}
							socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
						}
						socket.Bind(new IPEndPoint(ipbindInfo.IP, ipbindInfo.Port));
						socket.Listen(100);
						TCP_Server<T>.ListeningPoint item = new TCP_Server<T>.ListeningPoint(socket, ipbindInfo);
						this.m_pListeningPoints.Add(item);
						for (int k = 0; k < 10; k++)
						{
							TCP_Server<T>.TCP_Acceptor acceptor = new TCP_Server<T>.TCP_Acceptor(socket);
							acceptor.Tags["bind"] = ipbindInfo;
							acceptor.ConnectionAccepted += delegate(object s1, EventArgs<Socket> e1)
							{
								this.ProcessConnection(e1.Value, (IPBindInfo)acceptor.Tags["bind"]);
							};
							acceptor.Error += delegate(object s1, ExceptionEventArgs e1)
							{
								this.OnError(e1.Exception);
							};
							this.m_pConnectionAcceptors.Add(acceptor);
							acceptor.Start();
						}
					}
					catch (Exception x2)
					{
						this.OnError(x2);
					}
					IL_1B0:
					j++;
					continue;
					goto IL_1B0;
				}
			}
			catch (Exception x3)
			{
				this.OnError(x3);
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000AE44 File Offset: 0x00009E44
		private void ProcessConnection(Socket socket, IPBindInfo bindInfo)
		{
			bool flag = socket == null;
			if (flag)
			{
				throw new ArgumentNullException("socket");
			}
			bool flag2 = bindInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("bindInfo");
			}
			this.m_ConnectionsProcessed += 1L;
			try
			{
				T t = Activator.CreateInstance<T>();
				t.Init(this, socket, bindInfo.HostName, bindInfo.SslMode == SslMode.SSL, bindInfo.Certificate);
				bool flag3 = this.m_MaxConnections != 0L && (long)this.m_pSessions.Count > this.m_MaxConnections;
				if (flag3)
				{
					this.OnMaxConnectionsExceeded(t);
					t.Dispose();
				}
				else
				{
					bool flag4 = this.m_MaxConnectionsPerIP != 0L && this.m_pSessions.GetConnectionsPerIP(t.RemoteEndPoint.Address) > this.m_MaxConnectionsPerIP;
					if (flag4)
					{
						this.OnMaxConnectionsPerIPExceeded(t);
						t.Dispose();
					}
					else
					{
						t.Disonnected += delegate(object sender, EventArgs e)
						{
							this.m_pSessions.Remove((TCP_ServerSession)sender);
						};
						this.m_pSessions.Add(t);
						this.OnSessionCreated(t);
						t.StartI();
					}
				}
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000AFA0 File Offset: 0x00009FA0
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000197 RID: 407 RVA: 0x0000AFB8 File Offset: 0x00009FB8
		public bool IsRunning
		{
			get
			{
				return this.m_IsRunning;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000198 RID: 408 RVA: 0x0000AFD0 File Offset: 0x00009FD0
		// (set) Token: 0x06000199 RID: 409 RVA: 0x0000B004 File Offset: 0x0000A004
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
					bool isRunning = this.m_IsRunning;
					if (isRunning)
					{
						this.StartListen();
					}
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000B0C4 File Offset: 0x0000A0C4
		public IPEndPoint[] LocalEndPoints
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				List<IPEndPoint> list = new List<IPEndPoint>();
				foreach (IPBindInfo ipbindInfo in this.Bindings)
				{
					bool flag = ipbindInfo.IP.Equals(IPAddress.Any);
					if (flag)
					{
						foreach (IPAddress ipaddress in Dns.GetHostAddresses(""))
						{
							bool flag2 = ipaddress.AddressFamily == AddressFamily.InterNetwork && !list.Contains(new IPEndPoint(ipaddress, ipbindInfo.Port));
							if (flag2)
							{
								list.Add(new IPEndPoint(ipaddress, ipbindInfo.Port));
							}
						}
					}
					else
					{
						bool flag3 = ipbindInfo.IP.Equals(IPAddress.IPv6Any);
						if (flag3)
						{
							foreach (IPAddress ipaddress2 in Dns.GetHostAddresses(""))
							{
								bool flag4 = ipaddress2.AddressFamily == AddressFamily.InterNetworkV6 && !list.Contains(new IPEndPoint(ipaddress2, ipbindInfo.Port));
								if (flag4)
								{
									list.Add(new IPEndPoint(ipaddress2, ipbindInfo.Port));
								}
							}
						}
						else
						{
							bool flag5 = !list.Contains(ipbindInfo.EndPoint);
							if (flag5)
							{
								list.Add(ipbindInfo.EndPoint);
							}
						}
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000B25C File Offset: 0x0000A25C
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000B28C File Offset: 0x0000A28C
		public long MaxConnections
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				return this.m_MaxConnections;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxConnections' value must be >= 0.");
				}
				this.m_MaxConnections = value;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000B2CC File Offset: 0x0000A2CC
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000B2FC File Offset: 0x0000A2FC
		public long MaxConnectionsPerIP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				return this.m_MaxConnectionsPerIP;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = this.m_MaxConnectionsPerIP < 0L;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxConnectionsPerIP' value must be >= 0.");
				}
				this.m_MaxConnectionsPerIP = value;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000B344 File Offset: 0x0000A344
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000B374 File Offset: 0x0000A374
		public int SessionIdleTimeout
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				return this.m_SessionIdleTimeout;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'SessionIdleTimeout' value must be >= 0.");
				}
				this.m_SessionIdleTimeout = value;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000B3B4 File Offset: 0x0000A3B4
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000B3E8 File Offset: 0x0000A3E8
		public Logger Logger
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLogger;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_pLogger = value;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000B41C File Offset: 0x0000A41C
		public DateTime StartTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("TCP server is not running.");
				}
				return this.m_StartTime;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000B464 File Offset: 0x0000A464
		public long ConnectionsProcessed
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("TCP server is not running.");
				}
				return this.m_ConnectionsProcessed;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000B4AC File Offset: 0x0000A4AC
		public TCP_SessionCollection<TCP_ServerSession> Sessions
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Server");
				}
				bool flag = !this.m_IsRunning;
				if (flag)
				{
					throw new InvalidOperationException("TCP server is not running.");
				}
				return this.m_pSessions;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060001A6 RID: 422 RVA: 0x0000B4F4 File Offset: 0x0000A4F4
		// (remove) Token: 0x060001A7 RID: 423 RVA: 0x0000B52C File Offset: 0x0000A52C
		
		public event EventHandler Started = null;

		// Token: 0x060001A8 RID: 424 RVA: 0x0000B564 File Offset: 0x0000A564
		protected void OnStarted()
		{
			bool flag = this.Started != null;
			if (flag)
			{
				this.Started(this, new EventArgs());
			}
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060001A9 RID: 425 RVA: 0x0000B594 File Offset: 0x0000A594
		// (remove) Token: 0x060001AA RID: 426 RVA: 0x0000B5CC File Offset: 0x0000A5CC
		
		public event EventHandler Stopped = null;

		// Token: 0x060001AB RID: 427 RVA: 0x0000B604 File Offset: 0x0000A604
		protected void OnStopped()
		{
			bool flag = this.Stopped != null;
			if (flag)
			{
				this.Stopped(this, new EventArgs());
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060001AC RID: 428 RVA: 0x0000B634 File Offset: 0x0000A634
		// (remove) Token: 0x060001AD RID: 429 RVA: 0x0000B66C File Offset: 0x0000A66C
		
		public event EventHandler Disposed = null;

		// Token: 0x060001AE RID: 430 RVA: 0x0000B6A4 File Offset: 0x0000A6A4
		protected void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060001AF RID: 431 RVA: 0x0000B6D4 File Offset: 0x0000A6D4
		// (remove) Token: 0x060001B0 RID: 432 RVA: 0x0000B70C File Offset: 0x0000A70C
		
		public event EventHandler<TCP_ServerSessionEventArgs<T>> SessionCreated = null;

		// Token: 0x060001B1 RID: 433 RVA: 0x0000B744 File Offset: 0x0000A744
		private void OnSessionCreated(T session)
		{
			bool flag = this.SessionCreated != null;
			if (flag)
			{
				this.SessionCreated(this, new TCP_ServerSessionEventArgs<T>(this, session));
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060001B2 RID: 434 RVA: 0x0000B778 File Offset: 0x0000A778
		// (remove) Token: 0x060001B3 RID: 435 RVA: 0x0000B7B0 File Offset: 0x0000A7B0
		
		public event ErrorEventHandler Error = null;

		// Token: 0x060001B4 RID: 436 RVA: 0x0000B7E8 File Offset: 0x0000A7E8
		private void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new Error_EventArgs(x, new StackTrace()));
			}
		}

		// Token: 0x040000A8 RID: 168
		private bool m_IsDisposed = false;

		// Token: 0x040000A9 RID: 169
		private bool m_IsRunning = false;

		// Token: 0x040000AA RID: 170
		private IPBindInfo[] m_pBindings = new IPBindInfo[0];

		// Token: 0x040000AB RID: 171
		private long m_MaxConnections = 0L;

		// Token: 0x040000AC RID: 172
		private long m_MaxConnectionsPerIP = 0L;

		// Token: 0x040000AD RID: 173
		private int m_SessionIdleTimeout = 100;

		// Token: 0x040000AE RID: 174
		private Logger m_pLogger = null;

		// Token: 0x040000AF RID: 175
		private DateTime m_StartTime;

		// Token: 0x040000B0 RID: 176
		private long m_ConnectionsProcessed = 0L;

		// Token: 0x040000B1 RID: 177
		private List<TCP_Server<T>.TCP_Acceptor> m_pConnectionAcceptors = null;

		// Token: 0x040000B2 RID: 178
		private List<TCP_Server<T>.ListeningPoint> m_pListeningPoints = null;

		// Token: 0x040000B3 RID: 179
		private TCP_SessionCollection<TCP_ServerSession> m_pSessions = null;

		// Token: 0x040000B4 RID: 180
		private TimerEx m_pTimer_IdleTimeout = null;

		// Token: 0x0200027E RID: 638
		private class ListeningPoint
		{
			// Token: 0x060016D8 RID: 5848 RVA: 0x0008DBFC File Offset: 0x0008CBFC
			public ListeningPoint(Socket socket, IPBindInfo bind)
			{
				bool flag = socket == null;
				if (flag)
				{
					throw new ArgumentNullException("socket");
				}
				bool flag2 = bind == null;
				if (flag2)
				{
					throw new ArgumentNullException("socket");
				}
				this.m_pSocket = socket;
				this.m_pBindInfo = bind;
			}

			// Token: 0x1700077C RID: 1916
			// (get) Token: 0x060016D9 RID: 5849 RVA: 0x0008DC58 File Offset: 0x0008CC58
			public Socket Socket
			{
				get
				{
					return this.m_pSocket;
				}
			}

			// Token: 0x1700077D RID: 1917
			// (get) Token: 0x060016DA RID: 5850 RVA: 0x0008DC70 File Offset: 0x0008CC70
			public IPBindInfo BindInfo
			{
				get
				{
					return this.m_pBindInfo;
				}
			}

			// Token: 0x0400095A RID: 2394
			private Socket m_pSocket = null;

			// Token: 0x0400095B RID: 2395
			private IPBindInfo m_pBindInfo = null;
		}

		// Token: 0x0200027F RID: 639
		private class TCP_Acceptor : IDisposable
		{
			// Token: 0x060016DB RID: 5851 RVA: 0x0008DC88 File Offset: 0x0008CC88
			public TCP_Acceptor(Socket socket)
			{
				bool flag = socket == null;
				if (flag)
				{
					throw new ArgumentNullException("socket");
				}
				this.m_pSocket = socket;
				this.m_pTags = new Dictionary<string, object>();
			}

			// Token: 0x060016DC RID: 5852 RVA: 0x0008DCF4 File Offset: 0x0008CCF4
			public void Dispose()
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					this.m_pSocket = null;
					this.m_pSocketArgs = null;
					this.m_pTags = null;
					this.ConnectionAccepted = null;
					this.Error = null;
				}
			}

			// Token: 0x060016DD RID: 5853 RVA: 0x0008DD3C File Offset: 0x0008CD3C
			public void Start()
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
					ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						try
						{
							bool flag = Net_Utils.IsSocketAsyncSupported();
							if (flag)
							{
								this.m_pSocketArgs = new SocketAsyncEventArgs();
								this.m_pSocketArgs.Completed += delegate(object s1, SocketAsyncEventArgs e1)
								{
									bool isDisposed2 = this.m_IsDisposed;
									if (!isDisposed2)
									{
										try
										{
											bool flag2 = this.m_pSocketArgs.SocketError == SocketError.Success;
											if (flag2)
											{
												this.OnConnectionAccepted(this.m_pSocketArgs.AcceptSocket);
											}
											else
											{
												this.OnError(new Exception("Socket error '" + this.m_pSocketArgs.SocketError + "'."));
											}
											this.IOCompletionAccept();
										}
										catch (Exception x2)
										{
											this.OnError(x2);
										}
									}
								};
								this.IOCompletionAccept();
							}
							else
							{
								this.m_pSocket.BeginAccept(new AsyncCallback(this.AsyncSocketAccept), null);
							}
						}
						catch (Exception x)
						{
							this.OnError(x);
						}
					});
				}
			}

			// Token: 0x060016DE RID: 5854 RVA: 0x0008DD8C File Offset: 0x0008CD8C
			private void IOCompletionAccept()
			{
				try
				{
					this.m_pSocketArgs.AcceptSocket = null;
					while (!this.m_IsDisposed && !this.m_pSocket.AcceptAsync(this.m_pSocketArgs))
					{
						bool flag = this.m_pSocketArgs.SocketError == SocketError.Success;
						if (flag)
						{
							try
							{
								this.OnConnectionAccepted(this.m_pSocketArgs.AcceptSocket);
								this.m_pSocketArgs.AcceptSocket = null;
							}
							catch (Exception x)
							{
								this.OnError(x);
							}
						}
						else
						{
							this.OnError(new Exception("Socket error '" + this.m_pSocketArgs.SocketError + "'."));
						}
					}
				}
				catch (Exception x2)
				{
					this.OnError(x2);
				}
			}

			// Token: 0x060016DF RID: 5855 RVA: 0x0008DE74 File Offset: 0x0008CE74
			private void AsyncSocketAccept(IAsyncResult ar)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					try
					{
						this.OnConnectionAccepted(this.m_pSocket.EndAccept(ar));
					}
					catch (Exception x)
					{
						this.OnError(x);
					}
					try
					{
						this.m_pSocket.BeginAccept(new AsyncCallback(this.AsyncSocketAccept), null);
					}
					catch (Exception x2)
					{
						this.OnError(x2);
					}
				}
			}

			// Token: 0x1700077E RID: 1918
			// (get) Token: 0x060016E0 RID: 5856 RVA: 0x0008DEFC File Offset: 0x0008CEFC
			public Dictionary<string, object> Tags
			{
				get
				{
					return this.m_pTags;
				}
			}

			// Token: 0x140000A0 RID: 160
			// (add) Token: 0x060016E1 RID: 5857 RVA: 0x0008DF14 File Offset: 0x0008CF14
			// (remove) Token: 0x060016E2 RID: 5858 RVA: 0x0008DF4C File Offset: 0x0008CF4C
			
			public event EventHandler<EventArgs<Socket>> ConnectionAccepted = null;

			// Token: 0x060016E3 RID: 5859 RVA: 0x0008DF84 File Offset: 0x0008CF84
			private void OnConnectionAccepted(Socket socket)
			{
				bool flag = this.ConnectionAccepted != null;
				if (flag)
				{
					this.ConnectionAccepted(this, new EventArgs<Socket>(socket));
				}
			}

			// Token: 0x140000A1 RID: 161
			// (add) Token: 0x060016E4 RID: 5860 RVA: 0x0008DFB4 File Offset: 0x0008CFB4
			// (remove) Token: 0x060016E5 RID: 5861 RVA: 0x0008DFEC File Offset: 0x0008CFEC
			
			public event EventHandler<ExceptionEventArgs> Error = null;

			// Token: 0x060016E6 RID: 5862 RVA: 0x0008E024 File Offset: 0x0008D024
			private void OnError(Exception x)
			{
				bool flag = this.Error != null;
				if (flag)
				{
					this.Error(this, new ExceptionEventArgs(x));
				}
			}

			// Token: 0x0400095C RID: 2396
			private bool m_IsDisposed = false;

			// Token: 0x0400095D RID: 2397
			private bool m_IsRunning = false;

			// Token: 0x0400095E RID: 2398
			private Socket m_pSocket = null;

			// Token: 0x0400095F RID: 2399
			private SocketAsyncEventArgs m_pSocketArgs = null;

			// Token: 0x04000960 RID: 2400
			private Dictionary<string, object> m_pTags = null;
		}
	}
}
