using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using LumiSoft.Net.IO;
using LumiSoft.Net.Log;

namespace LumiSoft.Net.TCP
{
	// Token: 0x02000032 RID: 50
	public class TCP_ServerSession : TCP_Session
	{
		// Token: 0x060001B8 RID: 440 RVA: 0x0000B84C File Offset: 0x0000A84C
		public TCP_ServerSession()
		{
			this.m_pTags = new Dictionary<string, object>();
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000B8F4 File Offset: 0x0000A8F4
		public override void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				bool flag = !this.m_IsTerminated;
				if (flag)
				{
					try
					{
						this.Disconnect();
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
				this.m_pLocalEP = null;
				this.m_pRemoteEP = null;
				this.m_pCertificate = null;
				bool flag2 = this.m_pTcpStream != null;
				if (flag2)
				{
					this.m_pTcpStream.Dispose();
				}
				this.m_pTcpStream = null;
				bool flag3 = this.m_pRawTcpStream != null;
				if (flag3)
				{
					this.m_pRawTcpStream.Close();
				}
				this.m_pRawTcpStream = null;
				this.m_pTags = null;
				this.IdleTimeout = null;
				this.Disonnected = null;
				this.Disposed = null;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000B9DC File Offset: 0x0000A9DC
		internal void Init(object server, Socket socket, string hostName, bool ssl, X509Certificate certificate)
		{
			this.m_pServer = server;
			this.m_LocalHostName = hostName;
			this.m_IsSsl = ssl;
			this.m_ID = Guid.NewGuid().ToString();
			this.m_ConnectTime = DateTime.Now;
			this.m_pLocalEP = (IPEndPoint)socket.LocalEndPoint;
			this.m_pRemoteEP = (IPEndPoint)socket.RemoteEndPoint;
			this.m_pCertificate = certificate;
			socket.ReceiveBufferSize = 32000;
			socket.SendBufferSize = 32000;
			this.m_pRawTcpStream = new NetworkStream(socket, true);
			this.m_pTcpStream = new SmartStream(this.m_pRawTcpStream, true);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000BA88 File Offset: 0x0000AA88
		internal void StartI()
		{
			bool isSsl = this.m_IsSsl;
			if (isSsl)
			{
				this.LogAddText("Starting SSL negotiation now.");
				DateTime startTime = DateTime.Now;
				Action<TCP_ServerSession.SwitchToSecureAsyncOP> switchSecureCompleted = delegate(TCP_ServerSession.SwitchToSecureAsyncOP e)
				{
					try
					{
						bool flag2 = e.Error != null;
						if (flag2)
						{
							this.LogAddException(e.Error);
							bool flag3 = !this.IsDisposed;
							if (flag3)
							{
								this.Disconnect();
							}
						}
						else
						{
							this.LogAddText("SSL negotiation completed successfully in " + (DateTime.Now - startTime).TotalSeconds.ToString("f2") + " seconds.");
							this.Start();
						}
					}
					catch (Exception exception)
					{
						this.LogAddException(exception);
						bool flag4 = !this.IsDisposed;
						if (flag4)
						{
							this.Disconnect();
						}
					}
				};
				TCP_ServerSession.SwitchToSecureAsyncOP op = new TCP_ServerSession.SwitchToSecureAsyncOP();
				op.CompletedAsync += delegate(object sender, EventArgs<TCP_ServerSession.SwitchToSecureAsyncOP> e)
				{
					switchSecureCompleted(op);
				};
				bool flag = !this.SwitchToSecureAsync(op);
				if (flag)
				{
					switchSecureCompleted(op);
				}
			}
			else
			{
				this.Start();
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x000091B8 File Offset: 0x000081B8
		protected virtual void Start()
		{
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000BB30 File Offset: 0x0000AB30
		public void SwitchToSecure()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("TCP_ServerSession");
			}
			bool isSecure = this.m_IsSecure;
			if (isSecure)
			{
				throw new InvalidOperationException("Session is already SSL/TLS.");
			}
			bool flag = this.m_pCertificate == null;
			if (flag)
			{
				throw new InvalidOperationException("There is no certificate specified.");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			using (TCP_ServerSession.SwitchToSecureAsyncOP switchToSecureAsyncOP = new TCP_ServerSession.SwitchToSecureAsyncOP())
			{
				switchToSecureAsyncOP.CompletedAsync += delegate(object s1, EventArgs<TCP_ServerSession.SwitchToSecureAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.SwitchToSecureAsync(switchToSecureAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = switchToSecureAsyncOP.Error != null;
				if (flag3)
				{
					throw switchToSecureAsyncOP.Error;
				}
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000BC24 File Offset: 0x0000AC24
		public bool SwitchToSecureAsync(TCP_ServerSession.SwitchToSecureAsyncOP op)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			bool flag = this.m_pCertificate == null;
			if (flag)
			{
				throw new InvalidOperationException("There is no certificate specified.");
			}
			bool flag2 = op == null;
			if (flag2)
			{
				throw new ArgumentNullException("op");
			}
			bool flag3 = op.State > AsyncOP_State.WaitingForStart;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000BCBE File Offset: 0x0000ACBE
		public override void Disconnect()
		{
			this.Disconnect(null);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000BCCC File Offset: 0x0000ACCC
		public void Disconnect(string text)
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				bool isTerminated = this.m_IsTerminated;
				if (!isTerminated)
				{
					this.m_IsTerminated = true;
					bool flag = !string.IsNullOrEmpty(text);
					if (flag)
					{
						try
						{
							this.m_pTcpStream.Write(text);
						}
						catch (Exception x)
						{
							this.OnError(x);
						}
					}
					try
					{
						this.OnDisonnected();
					}
					catch (Exception x2)
					{
						this.OnError(x2);
					}
					this.Dispose();
				}
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000091B8 File Offset: 0x000081B8
		protected virtual void OnTimeout()
		{
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000BD68 File Offset: 0x0000AD68
		internal virtual void OnTimeoutI()
		{
			this.OnTimeout();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000BD74 File Offset: 0x0000AD74
		private void LogAddText(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			try
			{
				object value = this.Server.GetType().GetProperty("Logger").GetValue(this.Server, null);
				bool flag2 = value != null;
				if (flag2)
				{
					((Logger)value).AddText(this.ID, this.AuthenticatedUserIdentity, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000BE04 File Offset: 0x0000AE04
		private void LogAddException(Exception exception)
		{
			bool flag = exception == null;
			if (flag)
			{
				throw new ArgumentNullException("exception");
			}
			try
			{
				object value = this.Server.GetType().GetProperty("Logger").GetValue(this.Server, null);
				bool flag2 = value != null;
				if (flag2)
				{
					((Logger)value).AddException(this.ID, this.AuthenticatedUserIdentity, exception.Message, this.LocalEndPoint, this.RemoteEndPoint, exception);
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x0000BE98 File Offset: 0x0000AE98
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000BEB0 File Offset: 0x0000AEB0
		public object Server
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pServer;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x0000BEE0 File Offset: 0x0000AEE0
		public string LocalHostName
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_LocalHostName;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000BF10 File Offset: 0x0000AF10
		public X509Certificate Certificate
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pCertificate;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x0000BF40 File Offset: 0x0000AF40
		// (set) Token: 0x060001CA RID: 458 RVA: 0x0000BF58 File Offset: 0x0000AF58
		public object Tag
		{
			get
			{
				return this.m_pTag;
			}
			set
			{
				this.m_pTag = value;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000BF64 File Offset: 0x0000AF64
		public Dictionary<string, object> Tags
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pTags;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000BF94 File Offset: 0x0000AF94
		public override bool IsConnected
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001CD RID: 461 RVA: 0x0000BFA8 File Offset: 0x0000AFA8
		public override string ID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_ID;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000BFD8 File Offset: 0x0000AFD8
		public override DateTime ConnectTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_ConnectTime;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000C008 File Offset: 0x0000B008
		public override DateTime LastActivity
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pTcpStream.LastActivity;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000C03C File Offset: 0x0000B03C
		public override IPEndPoint LocalEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pLocalEP;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000C06C File Offset: 0x0000B06C
		public override IPEndPoint RemoteEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pRemoteEP;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000C09C File Offset: 0x0000B09C
		public override bool IsSecureConnection
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_IsSecure;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000C0CC File Offset: 0x0000B0CC
		public override SmartStream TcpStream
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_ServerSession");
				}
				return this.m_pTcpStream;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060001D4 RID: 468 RVA: 0x0000C0FC File Offset: 0x0000B0FC
		// (remove) Token: 0x060001D5 RID: 469 RVA: 0x0000C134 File Offset: 0x0000B134
		
		public event EventHandler IdleTimeout = null;

		// Token: 0x060001D6 RID: 470 RVA: 0x0000C16C File Offset: 0x0000B16C
		private void OnIdleTimeout()
		{
			bool flag = this.IdleTimeout != null;
			if (flag)
			{
				this.IdleTimeout(this, new EventArgs());
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060001D7 RID: 471 RVA: 0x0000C19C File Offset: 0x0000B19C
		// (remove) Token: 0x060001D8 RID: 472 RVA: 0x0000C1D4 File Offset: 0x0000B1D4
		
		public event EventHandler Disonnected = null;

		// Token: 0x060001D9 RID: 473 RVA: 0x0000C20C File Offset: 0x0000B20C
		private void OnDisonnected()
		{
			bool flag = this.Disonnected != null;
			if (flag)
			{
				this.Disonnected(this, new EventArgs());
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060001DA RID: 474 RVA: 0x0000C23C File Offset: 0x0000B23C
		// (remove) Token: 0x060001DB RID: 475 RVA: 0x0000C274 File Offset: 0x0000B274
		
		public event EventHandler Disposed = null;

		// Token: 0x060001DC RID: 476 RVA: 0x0000C2AC File Offset: 0x0000B2AC
		private void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060001DD RID: 477 RVA: 0x0000C2DC File Offset: 0x0000B2DC
		// (remove) Token: 0x060001DE RID: 478 RVA: 0x0000C314 File Offset: 0x0000B314
		
		public event ErrorEventHandler Error = null;

		// Token: 0x060001DF RID: 479 RVA: 0x0000C34C File Offset: 0x0000B34C
		protected virtual void OnError(Exception x)
		{
			bool flag = this.Error != null;
			if (flag)
			{
				this.Error(this, new Error_EventArgs(x, new StackTrace()));
			}
		}

		// Token: 0x040000BA RID: 186
		private bool m_IsDisposed = false;

		// Token: 0x040000BB RID: 187
		private bool m_IsTerminated = false;

		// Token: 0x040000BC RID: 188
		private object m_pServer = null;

		// Token: 0x040000BD RID: 189
		private string m_ID = "";

		// Token: 0x040000BE RID: 190
		private DateTime m_ConnectTime;

		// Token: 0x040000BF RID: 191
		private string m_LocalHostName = "";

		// Token: 0x040000C0 RID: 192
		private IPEndPoint m_pLocalEP = null;

		// Token: 0x040000C1 RID: 193
		private IPEndPoint m_pRemoteEP = null;

		// Token: 0x040000C2 RID: 194
		private bool m_IsSsl = false;

		// Token: 0x040000C3 RID: 195
		private bool m_IsSecure = false;

		// Token: 0x040000C4 RID: 196
		private X509Certificate m_pCertificate = null;

		// Token: 0x040000C5 RID: 197
		private NetworkStream m_pRawTcpStream = null;

		// Token: 0x040000C6 RID: 198
		private SmartStream m_pTcpStream = null;

		// Token: 0x040000C7 RID: 199
		private object m_pTag = null;

		// Token: 0x040000C8 RID: 200
		private Dictionary<string, object> m_pTags = null;

		// Token: 0x02000281 RID: 641
		public class SwitchToSecureAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060016EC RID: 5868 RVA: 0x0008E1E4 File Offset: 0x0008D1E4
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pTcpSession = null;
					this.m_pSslStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060016ED RID: 5869 RVA: 0x0008E228 File Offset: 0x0008D228
			internal bool Start(TCP_ServerSession owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pTcpSession = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pSslStream = new SslStream(this.m_pTcpSession.TcpStream.SourceStream, true);
					this.m_pSslStream.BeginAuthenticateAsServer(this.m_pTcpSession.m_pCertificate, new AsyncCallback(this.BeginAuthenticateAsServerCompleted), null);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
				}
				object pLock = this.m_pLock;
				bool result;
				lock (pLock)
				{
					this.m_RiseCompleted = true;
					result = (this.m_State == AsyncOP_State.Active);
				}
				return result;
			}

			// Token: 0x060016EE RID: 5870 RVA: 0x0008E304 File Offset: 0x0008D304
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag3)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x060016EF RID: 5871 RVA: 0x0008E37C File Offset: 0x0008D37C
			private void BeginAuthenticateAsServerCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSslStream.EndAuthenticateAsServer(ar);
					this.m_pTcpSession.m_pTcpStream.IsOwner = false;
					this.m_pTcpSession.m_pTcpStream.Dispose();
					this.m_pTcpSession.m_IsSecure = true;
					this.m_pTcpSession.m_pTcpStream = new SmartStream(this.m_pSslStream, true);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x1700077F RID: 1919
			// (get) Token: 0x060016F0 RID: 5872 RVA: 0x0008E408 File Offset: 0x0008D408
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000780 RID: 1920
			// (get) Token: 0x060016F1 RID: 5873 RVA: 0x0008E420 File Offset: 0x0008D420
			public Exception Error
			{
				get
				{
					bool flag = this.m_State == AsyncOP_State.Disposed;
					if (flag)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool flag2 = this.m_State != AsyncOP_State.Completed;
					if (flag2)
					{
						throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					return this.m_pException;
				}
			}

			// Token: 0x140000A2 RID: 162
			// (add) Token: 0x060016F2 RID: 5874 RVA: 0x0008E474 File Offset: 0x0008D474
			// (remove) Token: 0x060016F3 RID: 5875 RVA: 0x0008E4AC File Offset: 0x0008D4AC
			
			public event EventHandler<EventArgs<TCP_ServerSession.SwitchToSecureAsyncOP>> CompletedAsync = null;

			// Token: 0x060016F4 RID: 5876 RVA: 0x0008E4E4 File Offset: 0x0008D4E4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<TCP_ServerSession.SwitchToSecureAsyncOP>(this));
				}
			}

			// Token: 0x04000965 RID: 2405
			private object m_pLock = new object();

			// Token: 0x04000966 RID: 2406
			private bool m_RiseCompleted = false;

			// Token: 0x04000967 RID: 2407
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000968 RID: 2408
			private Exception m_pException = null;

			// Token: 0x04000969 RID: 2409
			private TCP_ServerSession m_pTcpSession = null;

			// Token: 0x0400096A RID: 2410
			private SslStream m_pSslStream = null;
		}
	}
}
