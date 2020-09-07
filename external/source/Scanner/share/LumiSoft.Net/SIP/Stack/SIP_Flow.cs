using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Timers;
using LumiSoft.Net.IO;
using LumiSoft.Net.MIME;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.TCP;
using LumiSoft.Net.UDP;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000089 RID: 137
	public class SIP_Flow : IDisposable
	{
		// Token: 0x060004FD RID: 1277 RVA: 0x000199D4 File Offset: 0x000189D4
		internal SIP_Flow(SIP_Stack stack, bool isServer, IPEndPoint localEP, IPEndPoint remoteEP, string transport)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			bool flag2 = localEP == null;
			if (flag2)
			{
				throw new ArgumentNullException("localEP");
			}
			bool flag3 = remoteEP == null;
			if (flag3)
			{
				throw new ArgumentNullException("remoteEP");
			}
			bool flag4 = transport == null;
			if (flag4)
			{
				throw new ArgumentNullException("transport");
			}
			this.m_pStack = stack;
			this.m_IsServer = isServer;
			this.m_pLocalEP = localEP;
			this.m_pRemoteEP = remoteEP;
			this.m_Transport = transport.ToUpper();
			this.m_CreateTime = DateTime.Now;
			this.m_LastActivity = DateTime.Now;
			this.m_ID = string.Concat(new string[]
			{
				this.m_pLocalEP.ToString(),
				"-",
				this.m_pRemoteEP.ToString(),
				"-",
				this.m_Transport
			});
			this.m_pMessage = new MemoryStream();
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00019B44 File Offset: 0x00018B44
		internal SIP_Flow(SIP_Stack stack, TCP_ServerSession session)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			bool flag2 = session == null;
			if (flag2)
			{
				throw new ArgumentNullException("session");
			}
			this.m_pStack = stack;
			this.m_pTcpSession = session;
			this.m_IsServer = true;
			this.m_pLocalEP = session.LocalEndPoint;
			this.m_pRemoteEP = session.RemoteEndPoint;
			this.m_Transport = (session.IsSecureConnection ? "TLS" : "TCP");
			this.m_CreateTime = DateTime.Now;
			this.m_LastActivity = DateTime.Now;
			this.m_ID = string.Concat(new string[]
			{
				this.m_pLocalEP.ToString(),
				"-",
				this.m_pRemoteEP.ToString(),
				"-",
				this.m_Transport
			});
			this.m_pMessage = new MemoryStream();
			session.Disposed += delegate(object s, EventArgs e)
			{
				this.Dispose();
			};
			this.BeginReadHeader();
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x00017E58 File Offset: 0x00016E58
		private void Session_Disposed(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00019CC0 File Offset: 0x00018CC0
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.OnDisposing();
					this.m_IsDisposed = true;
					bool flag2 = this.m_pTcpSession != null;
					if (flag2)
					{
						this.m_pTcpSession.Dispose();
						this.m_pTcpSession = null;
					}
					this.m_pMessage = null;
					bool flag3 = this.m_pKeepAliveTimer != null;
					if (flag3)
					{
						this.m_pKeepAliveTimer.Dispose();
						this.m_pKeepAliveTimer = null;
					}
				}
			}
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00019D68 File Offset: 0x00018D68
		public void Send(SIP_Request request)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = request == null;
				if (flag2)
				{
					throw new ArgumentNullException("request");
				}
				this.SendInternal(request.ToByteData());
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x00019DE4 File Offset: 0x00018DE4
		public void Send(SIP_Response response)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = response == null;
				if (flag2)
				{
					throw new ArgumentNullException("response");
				}
				this.SendInternal(response.ToByteData());
				this.m_LastPing = DateTime.Now;
			}
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00019E6C File Offset: 0x00018E6C
		public void SendPing()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = this.m_pStack.TransportLayer.Stack.Logger != null;
				if (flag2)
				{
					this.m_pStack.TransportLayer.Stack.Logger.AddWrite("", null, 2L, "Flow [id='" + this.ID + "'] sent \"ping\"", this.LocalEP, this.RemoteEP);
				}
				this.SendInternal(new byte[]
				{
					13,
					10,
					13,
					10
				});
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x00019F44 File Offset: 0x00018F44
		internal void Start()
		{
			AutoResetEvent startLock = new AutoResetEvent(false);
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					startLock.Set();
					bool flag2 = !this.m_IsServer && this.m_Transport != "UDP";
					if (flag2)
					{
						try
						{
							TCP_Client tcp_Client = new TCP_Client();
							tcp_Client.Connect(this.m_pLocalEP, this.m_pRemoteEP, this.m_Transport == "TLS");
							this.m_pTcpSession = tcp_Client;
							this.BeginReadHeader();
						}
						catch
						{
							this.Dispose();
						}
					}
				}
			});
			startLock.WaitOne();
			startLock.Close();
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x00019F98 File Offset: 0x00018F98
		internal void SendInternal(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			try
			{
				bool flag2 = this.m_Transport == "UDP";
				if (flag2)
				{
					this.m_pStack.TransportLayer.UdpServer.SendPacket(this.m_pLocalEP, data, 0, data.Length, this.m_pRemoteEP);
				}
				else
				{
					bool flag3 = this.m_Transport == "TCP";
					if (flag3)
					{
						this.m_pTcpSession.TcpStream.Write(data, 0, data.Length);
					}
					else
					{
						bool flag4 = this.m_Transport == "TLS";
						if (flag4)
						{
							this.m_pTcpSession.TcpStream.Write(data, 0, data.Length);
						}
					}
				}
				this.m_BytesWritten += (long)data.Length;
			}
			catch (IOException ex)
			{
				this.Dispose();
				throw ex;
			}
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001A088 File Offset: 0x00019088
		private void BeginReadHeader()
		{
			this.m_pMessage.SetLength(0L);
			this.m_pTcpSession.TcpStream.BeginReadHeader(this.m_pMessage, this.m_pStack.TransportLayer.Stack.MaximumMessageSize, SizeExceededAction.JunkAndThrowException, new AsyncCallback(this.BeginReadHeader_Completed), null);
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001A0E0 File Offset: 0x000190E0
		private void BeginReadHeader_Completed(IAsyncResult asyncResult)
		{
			try
			{
				int num = this.m_pTcpSession.TcpStream.EndReadHeader(asyncResult);
				bool flag = num == 0;
				if (flag)
				{
					bool isServer = this.IsServer;
					if (isServer)
					{
						bool lastCRLF = this.m_LastCRLF;
						if (lastCRLF)
						{
							this.m_LastCRLF = false;
							this.m_pStack.TransportLayer.OnMessageReceived(this, new byte[]
							{
								13,
								10,
								13,
								10
							});
						}
						else
						{
							this.m_LastCRLF = true;
						}
					}
					else
					{
						this.m_pStack.TransportLayer.OnMessageReceived(this, new byte[]
						{
							13,
							10
						});
					}
					this.BeginReadHeader();
				}
				else
				{
					this.m_LastCRLF = false;
					this.m_pMessage.Write(new byte[]
					{
						13,
						10
					}, 0, 2);
					this.m_pMessage.Position = 0L;
					string text = MIME_Utils.ParseHeaderField("Content-Length:", this.m_pMessage);
					this.m_pMessage.Position = this.m_pMessage.Length;
					int num2 = 0;
					bool flag2 = text != "";
					if (flag2)
					{
						num2 = Convert.ToInt32(text);
					}
					bool flag3 = num2 > 0;
					if (flag3)
					{
						this.m_pTcpSession.TcpStream.BeginReadFixedCount(this.m_pMessage, (long)num2, new AsyncCallback(this.BeginReadData_Completed), null);
					}
					else
					{
						byte[] message = this.m_pMessage.ToArray();
						this.BeginReadHeader();
						this.m_pStack.TransportLayer.OnMessageReceived(this, message);
					}
				}
			}
			catch
			{
				this.Dispose();
			}
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001A298 File Offset: 0x00019298
		private void BeginReadData_Completed(IAsyncResult asyncResult)
		{
			try
			{
				this.m_pTcpSession.TcpStream.EndReadFixedCount(asyncResult);
				byte[] message = this.m_pMessage.ToArray();
				this.BeginReadHeader();
				this.m_pStack.TransportLayer.OnMessageReceived(this, message);
			}
			catch
			{
				this.Dispose();
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001A300 File Offset: 0x00019300
		internal void OnUdpPacketReceived(UDP_e_PacketReceived e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			this.m_LastActivity = DateTime.Now;
			byte[] array = new byte[e.Count];
			Array.Copy(e.Buffer, array, e.Count);
			this.m_pStack.TransportLayer.OnMessageReceived(this, array);
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x0001A360 File Offset: 0x00019360
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x0001A378 File Offset: 0x00019378
		public bool IsServer
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsServer;
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x0001A3AC File Offset: 0x000193AC
		public DateTime CreateTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CreateTime;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x0001A3E0 File Offset: 0x000193E0
		public string ID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_ID;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600050E RID: 1294 RVA: 0x0001A414 File Offset: 0x00019414
		public IPEndPoint LocalEP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLocalEP;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x0001A448 File Offset: 0x00019448
		public IPEndPoint LocalPublicEP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_pLocalPublicEP != null;
				IPEndPoint pLocalPublicEP;
				if (flag)
				{
					pLocalPublicEP = this.m_pLocalPublicEP;
				}
				else
				{
					this.m_pLocalPublicEP = this.LocalEP;
					try
					{
						AutoResetEvent completionWaiter = new AutoResetEvent(false);
						SIP_Request sip_Request = this.m_pStack.CreateRequest("OPTIONS", new SIP_t_NameAddress("sip:ping@publicIP.com"), new SIP_t_NameAddress("sip:ping@publicIP.com"));
						sip_Request.MaxForwards = 0;
						SIP_ClientTransaction optionsTransaction = this.m_pStack.TransactionLayer.CreateClientTransaction(this, sip_Request, true);
						optionsTransaction.ResponseReceived += delegate(object s, SIP_ResponseReceivedEventArgs e)
						{
							SIP_t_ViaParm topMostValue = e.Response.Via.GetTopMostValue();
							IPEndPoint ipendPoint = new IPEndPoint((topMostValue.Received == null) ? this.LocalEP.Address : topMostValue.Received, (topMostValue.RPort > 0) ? topMostValue.RPort : this.LocalEP.Port);
							bool flag2 = !this.LocalEP.Address.Equals(ipendPoint.Address);
							if (flag2)
							{
								this.m_pLocalPublicEP = ipendPoint;
							}
							bool flag3 = completionWaiter != null;
							if (flag3)
							{
								completionWaiter.Set();
							}
						};
						optionsTransaction.StateChanged += delegate(object s, EventArgs e)
						{
							bool flag2 = optionsTransaction.State == SIP_TransactionState.Terminated;
							if (flag2)
							{
								bool flag3 = completionWaiter != null;
								if (flag3)
								{
									completionWaiter.Set();
								}
							}
						};
						optionsTransaction.Start();
						completionWaiter.WaitOne();
						completionWaiter.Close();
						completionWaiter = null;
					}
					catch
					{
					}
					pLocalPublicEP = this.m_pLocalPublicEP;
				}
				return pLocalPublicEP;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x0001A57C File Offset: 0x0001957C
		public IPEndPoint RemoteEP
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRemoteEP;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x0001A5B0 File Offset: 0x000195B0
		public string Transport
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Transport;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x0001A5E4 File Offset: 0x000195E4
		public bool IsReliable
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Transport != "UDP";
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0001A624 File Offset: 0x00019624
		public bool IsSecure
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Transport == "TLS";
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x0001A670 File Offset: 0x00019670
		// (set) Token: 0x06000515 RID: 1301 RVA: 0x0001A6A8 File Offset: 0x000196A8
		public bool SendKeepAlives
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pKeepAliveTimer != null;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				if (value)
				{
					bool flag = this.m_pKeepAliveTimer == null;
					if (flag)
					{
						this.m_pKeepAliveTimer = new TimerEx(15000.0, true);
						this.m_pKeepAliveTimer.Elapsed += delegate(object s, ElapsedEventArgs e)
						{
							try
							{
								bool flag3 = this.m_pStack.TransportLayer.Stack.Logger != null;
								if (flag3)
								{
									this.m_pStack.TransportLayer.Stack.Logger.AddWrite("", null, 2L, "Flow [id='" + this.ID + "'] sent \"ping\"", this.LocalEP, this.RemoteEP);
								}
								this.SendInternal(new byte[]
								{
									13,
									10,
									13,
									10
								});
							}
							catch
							{
							}
						};
						this.m_pKeepAliveTimer.Enabled = true;
					}
				}
				else
				{
					bool flag2 = this.m_pKeepAliveTimer != null;
					if (flag2)
					{
						this.m_pKeepAliveTimer.Dispose();
						this.m_pKeepAliveTimer = null;
					}
				}
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x0001A748 File Offset: 0x00019748
		public DateTime LastActivity
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_Transport == "TCP" || this.m_Transport == "TLS";
				DateTime lastActivity;
				if (flag)
				{
					lastActivity = this.m_pTcpSession.LastActivity;
				}
				else
				{
					lastActivity = this.m_LastActivity;
				}
				return lastActivity;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0001A7B8 File Offset: 0x000197B8
		public DateTime LastPing
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LastPing;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x0001A7EC File Offset: 0x000197EC
		public long BytesWritten
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_BytesWritten;
			}
		}

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000519 RID: 1305 RVA: 0x0001A820 File Offset: 0x00019820
		// (remove) Token: 0x0600051A RID: 1306 RVA: 0x0001A858 File Offset: 0x00019858
		
		public event EventHandler IsDisposing = null;

		// Token: 0x0600051B RID: 1307 RVA: 0x0001A890 File Offset: 0x00019890
		private void OnDisposing()
		{
			bool flag = this.IsDisposing != null;
			if (flag)
			{
				this.IsDisposing(this, new EventArgs());
			}
		}

		// Token: 0x0400019D RID: 413
		private object m_pLock = new object();

		// Token: 0x0400019E RID: 414
		private bool m_IsDisposed = false;

		// Token: 0x0400019F RID: 415
		private bool m_IsServer = false;

		// Token: 0x040001A0 RID: 416
		private SIP_Stack m_pStack = null;

		// Token: 0x040001A1 RID: 417
		private TCP_Session m_pTcpSession = null;

		// Token: 0x040001A2 RID: 418
		private DateTime m_CreateTime;

		// Token: 0x040001A3 RID: 419
		private string m_ID = "";

		// Token: 0x040001A4 RID: 420
		private IPEndPoint m_pLocalEP = null;

		// Token: 0x040001A5 RID: 421
		private IPEndPoint m_pLocalPublicEP = null;

		// Token: 0x040001A6 RID: 422
		private IPEndPoint m_pRemoteEP = null;

		// Token: 0x040001A7 RID: 423
		private string m_Transport = "";

		// Token: 0x040001A8 RID: 424
		private DateTime m_LastActivity;

		// Token: 0x040001A9 RID: 425
		private DateTime m_LastPing;

		// Token: 0x040001AA RID: 426
		private long m_BytesWritten = 0L;

		// Token: 0x040001AB RID: 427
		private MemoryStream m_pMessage = null;

		// Token: 0x040001AC RID: 428
		private bool m_LastCRLF = false;

		// Token: 0x040001AD RID: 429
		private TimerEx m_pKeepAliveTimer = null;
	}
}
