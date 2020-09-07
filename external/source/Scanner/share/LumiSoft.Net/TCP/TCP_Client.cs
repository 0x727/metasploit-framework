using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using LumiSoft.Net.IO;
using LumiSoft.Net.Log;

namespace LumiSoft.Net.TCP
{
	// Token: 0x02000030 RID: 48
	public class TCP_Client : TCP_Session
	{
		// Token: 0x06000162 RID: 354 RVA: 0x00009A08 File Offset: 0x00008A08
		public override void Dispose()
		{
			lock (this)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					try
					{
						this.Disconnect();
					}
					catch
					{
					}
					this.m_IsDisposed = true;
				}
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00009A74 File Offset: 0x00008A74
		public void Connect(string host, int port)
		{
			this.Connect(host, port, false);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009A84 File Offset: 0x00008A84
		public void Connect(string host, int port, bool ssl)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("TCP_Client");
			}
			bool isConnected = this.m_IsConnected;
			if (isConnected)
			{
				throw new InvalidOperationException("TCP client is already connected.");
			}
			bool flag = string.IsNullOrEmpty(host);
			if (flag)
			{
				throw new ArgumentException("Argument 'host' value may not be null or empty.");
			}
			bool flag2 = port < 1;
			if (flag2)
			{
				throw new ArgumentException("Argument 'port' value must be >= 1.");
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			for (int i = 0; i < hostAddresses.Length; i++)
			{
				try
				{
					this.Connect(null, new IPEndPoint(hostAddresses[i], port), ssl);
					break;
				}
				catch (Exception ex)
				{
					bool isConnected2 = this.IsConnected;
					if (isConnected2)
					{
						throw ex;
					}
					bool flag3 = i == hostAddresses.Length - 1;
					if (flag3)
					{
						throw ex;
					}
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00009B5C File Offset: 0x00008B5C
		public void Connect(IPEndPoint remoteEP, bool ssl)
		{
			this.Connect(null, remoteEP, ssl);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009B69 File Offset: 0x00008B69
		public void Connect(IPEndPoint localEP, IPEndPoint remoteEP, bool ssl)
		{
			this.Connect(localEP, remoteEP, ssl, null);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00009B78 File Offset: 0x00008B78
		public void Connect(IPEndPoint localEP, IPEndPoint remoteEP, bool ssl, RemoteCertificateValidationCallback certCallback)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isConnected = this.m_IsConnected;
			if (isConnected)
			{
				throw new InvalidOperationException("TCP client is already connected.");
			}
			bool flag = remoteEP == null;
			if (flag)
			{
				throw new ArgumentNullException("remoteEP");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			using (TCP_Client.ConnectAsyncOP connectAsyncOP = new TCP_Client.ConnectAsyncOP(localEP, remoteEP, ssl, certCallback))
			{
				connectAsyncOP.CompletedAsync += delegate(object s1, EventArgs<TCP_Client.ConnectAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.ConnectAsync(connectAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = connectAsyncOP.Error != null;
				if (flag3)
				{
					throw connectAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00009C70 File Offset: 0x00008C70
		public bool ConnectAsync(TCP_Client.ConnectAsyncOP op)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.State > AsyncOP_State.WaitingForStart;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00009CD8 File Offset: 0x00008CD8
		public override void Disconnect()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("TCP_Client");
			}
			bool flag = !this.m_IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("TCP client is not connected.");
			}
			this.m_IsConnected = false;
			this.m_pLocalEP = null;
			this.m_pRemoteEP = null;
			this.m_pTcpStream.Dispose();
			this.m_IsSecure = false;
			this.m_pTcpStream = null;
			this.LogAddText("Disconnected.");
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00009D50 File Offset: 0x00008D50
		public IAsyncResult BeginDisconnect(AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.m_IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("TCP client is not connected.");
			}
			TCP_Client.DisconnectDelegate disconnectDelegate = new TCP_Client.DisconnectDelegate(this.Disconnect);
			AsyncResultState asyncResultState = new AsyncResultState(this, disconnectDelegate, callback, state);
			asyncResultState.SetAsyncResult(disconnectDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00009DCC File Offset: 0x00008DCC
		public void EndDisconnect(IAsyncResult asyncResult)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResultState asyncResultState = asyncResult as AsyncResultState;
			bool flag2 = asyncResultState == null || asyncResultState.AsyncObject != this;
			if (flag2)
			{
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginDisconnect method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndDisconnect was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is TCP_Client.DisconnectDelegate;
			if (flag3)
			{
				((TCP_Client.DisconnectDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginDisconnect method.");
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00009E90 File Offset: 0x00008E90
		protected void SwitchToSecure()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("TCP_Client");
			}
			bool flag = !this.m_IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("TCP client is not connected.");
			}
			bool isSecure = this.m_IsSecure;
			if (isSecure)
			{
				throw new InvalidOperationException("TCP client is already secure.");
			}
			this.LogAddText("Switching to SSL.");
			SslStream sslStream = new SslStream(this.m_pTcpStream.SourceStream, true, new RemoteCertificateValidationCallback(this.RemoteCertificateValidationCallback));
			sslStream.AuthenticateAsClient("dummy");
			this.m_pTcpStream.IsOwner = false;
			this.m_pTcpStream.Dispose();
			this.m_IsSecure = true;
			this.m_pTcpStream = new SmartStream(sslStream, true);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00009F48 File Offset: 0x00008F48
		private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool flag = this.m_pCertificateCallback != null;
			bool result;
			if (flag)
			{
				result = this.m_pCertificateCallback(sender, certificate, chain, sslPolicyErrors);
			}
			else
			{
				bool flag2 = sslPolicyErrors == SslPolicyErrors.None || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) > SslPolicyErrors.None;
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00009F94 File Offset: 0x00008F94
		protected bool SwitchToSecureAsync(TCP_Client.SwitchToSecureAsyncOP op)
		{
			bool isDisposed = this.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
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

		// Token: 0x0600016F RID: 367 RVA: 0x000091B8 File Offset: 0x000081B8
		protected virtual void OnConnected()
		{
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A030 File Offset: 0x00009030
		protected virtual void OnConnected(TCP_Client.CompleteConnectCallback callback)
		{
			try
			{
				this.OnConnected();
				callback(null);
			}
			catch (Exception error)
			{
				callback(error);
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000A070 File Offset: 0x00009070
		protected string ReadLine()
		{
			SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
			this.TcpStream.ReadLine(readLineAsyncOP, false);
			bool flag = readLineAsyncOP.Error != null;
			if (flag)
			{
				throw readLineAsyncOP.Error;
			}
			string lineUtf = readLineAsyncOP.LineUtf8;
			bool flag2 = readLineAsyncOP.BytesInBuffer > 0;
			if (flag2)
			{
				this.LogAddRead((long)readLineAsyncOP.BytesInBuffer, lineUtf);
			}
			else
			{
				this.LogAddText("Remote host closed connection.");
			}
			return lineUtf;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000A0F0 File Offset: 0x000090F0
		protected void WriteLine(string line)
		{
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			int num = this.TcpStream.WriteLine(line);
			this.LogAddWrite((long)num, line);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000A12C File Offset: 0x0000912C
		protected internal void LogAddRead(long size, string text)
		{
			try
			{
				bool flag = this.m_pLogger != null;
				if (flag)
				{
					this.m_pLogger.AddRead(this.ID, this.AuthenticatedUserIdentity, size, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000A188 File Offset: 0x00009188
		protected internal void LogAddWrite(long size, string text)
		{
			try
			{
				bool flag = this.m_pLogger != null;
				if (flag)
				{
					this.m_pLogger.AddWrite(this.ID, this.AuthenticatedUserIdentity, size, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000A1E4 File Offset: 0x000091E4
		protected internal void LogAddText(string text)
		{
			try
			{
				bool flag = this.m_pLogger != null;
				if (flag)
				{
					this.m_pLogger.AddText(this.IsConnected ? this.ID : "", this.IsConnected ? this.AuthenticatedUserIdentity : null, text, this.IsConnected ? this.LocalEndPoint : null, this.IsConnected ? this.RemoteEndPoint : null);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000A270 File Offset: 0x00009270
		protected internal void LogAddException(string text, Exception x)
		{
			try
			{
				bool flag = this.m_pLogger != null;
				if (flag)
				{
					this.m_pLogger.AddException(this.IsConnected ? this.ID : "", this.IsConnected ? this.AuthenticatedUserIdentity : null, text, this.IsConnected ? this.LocalEndPoint : null, this.IsConnected ? this.RemoteEndPoint : null, x);
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000A2FC File Offset: 0x000092FC
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000A314 File Offset: 0x00009314
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000A32C File Offset: 0x0000932C
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

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600017A RID: 378 RVA: 0x0000A338 File Offset: 0x00009338
		public override bool IsConnected
		{
			get
			{
				return this.m_IsConnected;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600017B RID: 379 RVA: 0x0000A350 File Offset: 0x00009350
		public override string ID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_ID;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600017C RID: 380 RVA: 0x0000A398 File Offset: 0x00009398
		public override DateTime ConnectTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_ConnectTime;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000A3E0 File Offset: 0x000093E0
		public override DateTime LastActivity
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_pTcpStream.LastActivity;
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600017E RID: 382 RVA: 0x0000A42C File Offset: 0x0000942C
		public override IPEndPoint LocalEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_pLocalEP;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000A474 File Offset: 0x00009474
		public override IPEndPoint RemoteEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_pRemoteEP;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000180 RID: 384 RVA: 0x0000A4BC File Offset: 0x000094BC
		public override bool IsSecureConnection
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_IsSecure;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000A504 File Offset: 0x00009504
		public override SmartStream TcpStream
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("TCP_Client");
				}
				bool flag = !this.m_IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("TCP client is not connected.");
				}
				return this.m_pTcpStream;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000A54C File Offset: 0x0000954C
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000A564 File Offset: 0x00009564
		public RemoteCertificateValidationCallback ValidateCertificateCallback
		{
			get
			{
				return this.m_pCertificateCallback;
			}
			set
			{
				this.m_pCertificateCallback = value;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000A570 File Offset: 0x00009570
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000A588 File Offset: 0x00009588
		public int Timeout
		{
			get
			{
				return this.m_Timeout;
			}
			set
			{
				this.m_Timeout = value;
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000A594 File Offset: 0x00009594
		[Obsolete("Use method ConnectAsync instead.")]
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			return this.BeginConnect(host, port, false, callback, state);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000A5B4 File Offset: 0x000095B4
		[Obsolete("Use method ConnectAsync instead.")]
		public IAsyncResult BeginConnect(string host, int port, bool ssl, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isConnected = this.m_IsConnected;
			if (isConnected)
			{
				throw new InvalidOperationException("TCP client is already connected.");
			}
			bool flag = string.IsNullOrEmpty(host);
			if (flag)
			{
				throw new ArgumentException("Argument 'host' value may not be null or empty.");
			}
			bool flag2 = port < 1;
			if (flag2)
			{
				throw new ArgumentException("Argument 'port' value must be >= 1.");
			}
			TCP_Client.BeginConnectHostDelegate beginConnectHostDelegate = new TCP_Client.BeginConnectHostDelegate(this.Connect);
			AsyncResultState asyncResultState = new AsyncResultState(this, beginConnectHostDelegate, callback, state);
			asyncResultState.SetAsyncResult(beginConnectHostDelegate.BeginInvoke(host, port, ssl, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000A660 File Offset: 0x00009660
		[Obsolete("Use method ConnectAsync instead.")]
		public IAsyncResult BeginConnect(IPEndPoint remoteEP, bool ssl, AsyncCallback callback, object state)
		{
			return this.BeginConnect(null, remoteEP, ssl, callback, state);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A680 File Offset: 0x00009680
		[Obsolete("Use method ConnectAsync instead.")]
		public IAsyncResult BeginConnect(IPEndPoint localEP, IPEndPoint remoteEP, bool ssl, AsyncCallback callback, object state)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isConnected = this.m_IsConnected;
			if (isConnected)
			{
				throw new InvalidOperationException("TCP client is already connected.");
			}
			bool flag = remoteEP == null;
			if (flag)
			{
				throw new ArgumentNullException("remoteEP");
			}
			TCP_Client.BeginConnectEPDelegate beginConnectEPDelegate = new TCP_Client.BeginConnectEPDelegate(this.Connect);
			AsyncResultState asyncResultState = new AsyncResultState(this, beginConnectEPDelegate, callback, state);
			asyncResultState.SetAsyncResult(beginConnectEPDelegate.BeginInvoke(localEP, remoteEP, ssl, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A714 File Offset: 0x00009714
		[Obsolete("Use method ConnectAsync instead.")]
		public void EndConnect(IAsyncResult asyncResult)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResultState asyncResultState = asyncResult as AsyncResultState;
			bool flag2 = asyncResultState == null || asyncResultState.AsyncObject != this;
			if (flag2)
			{
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginConnect method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("EndConnect was previously called for the asynchronous operation.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is TCP_Client.BeginConnectHostDelegate;
			if (flag3)
			{
				((TCP_Client.BeginConnectHostDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
			}
			else
			{
				bool flag4 = asyncResultState.AsyncDelegate is TCP_Client.BeginConnectEPDelegate;
				if (!flag4)
				{
					throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginConnect method.");
				}
				((TCP_Client.BeginConnectEPDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000A808 File Offset: 0x00009808
		[Obsolete("Don't use this method.")]
		protected void OnError(Exception x)
		{
			try
			{
				bool flag = this.m_pLogger != null;
				if (flag)
				{
				}
			}
			catch
			{
			}
		}

		// Token: 0x0400009D RID: 157
		private bool m_IsDisposed = false;

		// Token: 0x0400009E RID: 158
		private bool m_IsConnected = false;

		// Token: 0x0400009F RID: 159
		private string m_ID = "";

		// Token: 0x040000A0 RID: 160
		private DateTime m_ConnectTime;

		// Token: 0x040000A1 RID: 161
		private IPEndPoint m_pLocalEP = null;

		// Token: 0x040000A2 RID: 162
		private IPEndPoint m_pRemoteEP = null;

		// Token: 0x040000A3 RID: 163
		private bool m_IsSecure = false;

		// Token: 0x040000A4 RID: 164
		private SmartStream m_pTcpStream = null;

		// Token: 0x040000A5 RID: 165
		private Logger m_pLogger = null;

		// Token: 0x040000A6 RID: 166
		private RemoteCertificateValidationCallback m_pCertificateCallback = null;

		// Token: 0x040000A7 RID: 167
		private int m_Timeout = 61000;

		// Token: 0x02000277 RID: 631
		public class ConnectAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060016AA RID: 5802 RVA: 0x0008CF58 File Offset: 0x0008BF58
			public ConnectAsyncOP(IPEndPoint localEP, IPEndPoint remoteEP, bool ssl, RemoteCertificateValidationCallback certCallback)
			{
				bool flag = remoteEP == null;
				if (flag)
				{
					throw new ArgumentNullException("localEP");
				}
				this.m_pLocalEP = localEP;
				this.m_pRemoteEP = remoteEP;
				this.m_SSL = ssl;
				this.m_pCertCallback = certCallback;
			}

			// Token: 0x060016AB RID: 5803 RVA: 0x0008CFF8 File Offset: 0x0008BFF8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pLocalEP = null;
					this.m_pRemoteEP = null;
					this.m_SSL = false;
					this.m_pCertCallback = null;
					this.m_pTcpClient = null;
					this.m_pSocket = null;
					this.m_pStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060016AC RID: 5804 RVA: 0x0008D060 File Offset: 0x0008C060
			internal bool Start(TCP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pTcpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool flag2 = this.m_pRemoteEP.AddressFamily == AddressFamily.InterNetwork;
					if (flag2)
					{
						this.m_pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						this.m_pSocket.ReceiveTimeout = this.m_pTcpClient.m_Timeout;
						this.m_pSocket.SendTimeout = this.m_pTcpClient.m_Timeout;
					}
					else
					{
						bool flag3 = this.m_pRemoteEP.AddressFamily == AddressFamily.InterNetworkV6;
						if (flag3)
						{
							this.m_pSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
							this.m_pSocket.ReceiveTimeout = this.m_pTcpClient.m_Timeout;
							this.m_pSocket.SendTimeout = this.m_pTcpClient.m_Timeout;
						}
					}
					bool flag4 = this.m_pLocalEP != null;
					if (flag4)
					{
						this.m_pSocket.Bind(this.m_pLocalEP);
					}
					this.m_pTcpClient.LogAddText("Connecting to " + this.m_pRemoteEP.ToString() + ".");
					this.m_pSocket.BeginConnect(this.m_pRemoteEP, new AsyncCallback(this.BeginConnectCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.CleanupSocketRelated();
					bool flag5 = this.m_pTcpClient != null;
					if (flag5)
					{
						this.m_pTcpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
					return false;
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

			// Token: 0x060016AD RID: 5805 RVA: 0x0008D25C File Offset: 0x0008C25C
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

			// Token: 0x060016AE RID: 5806 RVA: 0x0008D2D4 File Offset: 0x0008C2D4
			private void BeginConnectCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSocket.EndConnect(ar);
					this.m_pTcpClient.LogAddText(string.Concat(new string[]
					{
						"Connected, localEP='",
						this.m_pSocket.LocalEndPoint.ToString(),
						"'; remoteEP='",
						this.m_pSocket.RemoteEndPoint.ToString(),
						"'."
					}));
					bool ssl = this.m_SSL;
					if (ssl)
					{
						this.m_pTcpClient.LogAddText("Starting SSL handshake.");
						this.m_pStream = new SslStream(new NetworkStream(this.m_pSocket, true), false, new RemoteCertificateValidationCallback(this.RemoteCertificateValidationCallback));
						((SslStream)this.m_pStream).BeginAuthenticateAsClient("dummy", new AsyncCallback(this.BeginAuthenticateAsClientCompleted), null);
					}
					else
					{
						this.m_pStream = new NetworkStream(this.m_pSocket, true);
						this.InternalConnectCompleted();
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.CleanupSocketRelated();
					bool flag = this.m_pTcpClient != null;
					if (flag)
					{
						this.m_pTcpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060016AF RID: 5807 RVA: 0x0008D420 File Offset: 0x0008C420
			private void BeginAuthenticateAsClientCompleted(IAsyncResult ar)
			{
				try
				{
					((SslStream)this.m_pStream).EndAuthenticateAsClient(ar);
					this.m_pTcpClient.LogAddText("SSL handshake completed sucessfully.");
					this.InternalConnectCompleted();
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.CleanupSocketRelated();
					bool flag = this.m_pTcpClient != null;
					if (flag)
					{
						this.m_pTcpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060016B0 RID: 5808 RVA: 0x0008D4B4 File Offset: 0x0008C4B4
			private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				bool flag = this.m_pCertCallback != null;
				bool result;
				if (flag)
				{
					result = this.m_pCertCallback(sender, certificate, chain, sslPolicyErrors);
				}
				else
				{
					bool flag2 = sslPolicyErrors == SslPolicyErrors.None || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) > SslPolicyErrors.None;
					result = flag2;
				}
				return result;
			}

			// Token: 0x060016B1 RID: 5809 RVA: 0x0008D500 File Offset: 0x0008C500
			private void CleanupSocketRelated()
			{
				try
				{
					bool flag = this.m_pStream != null;
					if (flag)
					{
						this.m_pStream.Dispose();
					}
					bool flag2 = this.m_pSocket != null;
					if (flag2)
					{
						this.m_pSocket.Close();
					}
				}
				catch
				{
				}
			}

			// Token: 0x060016B2 RID: 5810 RVA: 0x0008D560 File Offset: 0x0008C560
			private void InternalConnectCompleted()
			{
				this.m_pTcpClient.m_IsConnected = true;
				this.m_pTcpClient.m_ID = Guid.NewGuid().ToString();
				this.m_pTcpClient.m_ConnectTime = DateTime.Now;
				this.m_pTcpClient.m_pLocalEP = (IPEndPoint)this.m_pSocket.LocalEndPoint;
				this.m_pTcpClient.m_pRemoteEP = (IPEndPoint)this.m_pSocket.RemoteEndPoint;
				this.m_pTcpClient.m_pTcpStream = new SmartStream(this.m_pStream, true);
				this.m_pTcpClient.m_pTcpStream.Encoding = Encoding.UTF8;
				this.m_pTcpClient.OnConnected(new TCP_Client.CompleteConnectCallback(this.CompleteConnectCallback));
			}

			// Token: 0x060016B3 RID: 5811 RVA: 0x0008D623 File Offset: 0x0008C623
			private void CompleteConnectCallback(Exception error)
			{
				this.m_pException = error;
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000776 RID: 1910
			// (get) Token: 0x060016B4 RID: 5812 RVA: 0x0008D638 File Offset: 0x0008C638
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000777 RID: 1911
			// (get) Token: 0x060016B5 RID: 5813 RVA: 0x0008D650 File Offset: 0x0008C650
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

			// Token: 0x17000778 RID: 1912
			// (get) Token: 0x060016B6 RID: 5814 RVA: 0x0008D6A4 File Offset: 0x0008C6A4
			public Socket Socket
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
						throw new InvalidOperationException("Property 'Socket' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pSocket;
				}
			}

			// Token: 0x17000779 RID: 1913
			// (get) Token: 0x060016B7 RID: 5815 RVA: 0x0008D70C File Offset: 0x0008C70C
			public Stream Stream
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
						throw new InvalidOperationException("Property 'Stream' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pStream;
				}
			}

			// Token: 0x1400009E RID: 158
			// (add) Token: 0x060016B8 RID: 5816 RVA: 0x0008D774 File Offset: 0x0008C774
			// (remove) Token: 0x060016B9 RID: 5817 RVA: 0x0008D7AC File Offset: 0x0008C7AC
			
			public event EventHandler<EventArgs<TCP_Client.ConnectAsyncOP>> CompletedAsync = null;

			// Token: 0x060016BA RID: 5818 RVA: 0x0008D7E4 File Offset: 0x0008C7E4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<TCP_Client.ConnectAsyncOP>(this));
				}
			}

			// Token: 0x04000945 RID: 2373
			private object m_pLock = new object();

			// Token: 0x04000946 RID: 2374
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000947 RID: 2375
			private Exception m_pException = null;

			// Token: 0x04000948 RID: 2376
			private IPEndPoint m_pLocalEP = null;

			// Token: 0x04000949 RID: 2377
			private IPEndPoint m_pRemoteEP = null;

			// Token: 0x0400094A RID: 2378
			private bool m_SSL = false;

			// Token: 0x0400094B RID: 2379
			private RemoteCertificateValidationCallback m_pCertCallback = null;

			// Token: 0x0400094C RID: 2380
			private TCP_Client m_pTcpClient = null;

			// Token: 0x0400094D RID: 2381
			private Socket m_pSocket = null;

			// Token: 0x0400094E RID: 2382
			private Stream m_pStream = null;

			// Token: 0x0400094F RID: 2383
			private bool m_RiseCompleted = false;
		}

		// Token: 0x02000278 RID: 632
		// (Invoke) Token: 0x060016BC RID: 5820
		private delegate void DisconnectDelegate();

		// Token: 0x02000279 RID: 633
		protected class SwitchToSecureAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060016BF RID: 5823 RVA: 0x0008D814 File Offset: 0x0008C814
			public SwitchToSecureAsyncOP(RemoteCertificateValidationCallback certCallback)
			{
				this.m_pCertCallback = certCallback;
			}

			// Token: 0x060016C0 RID: 5824 RVA: 0x0008D86C File Offset: 0x0008C86C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pCertCallback = null;
					this.m_pSslStream = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060016C1 RID: 5825 RVA: 0x0008D8B0 File Offset: 0x0008C8B0
			internal bool Start(TCP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pTcpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					this.m_pSslStream = new SslStream(this.m_pTcpClient.m_pTcpStream.SourceStream, false, new RemoteCertificateValidationCallback(this.RemoteCertificateValidationCallback));
					this.m_pSslStream.BeginAuthenticateAsClient("dummy", new AsyncCallback(this.BeginAuthenticateAsClientCompleted), null);
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

			// Token: 0x060016C2 RID: 5826 RVA: 0x0008D990 File Offset: 0x0008C990
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

			// Token: 0x060016C3 RID: 5827 RVA: 0x0008DA08 File Offset: 0x0008CA08
			private bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				bool flag = this.m_pCertCallback != null;
				bool result;
				if (flag)
				{
					result = this.m_pCertCallback(sender, certificate, chain, sslPolicyErrors);
				}
				else
				{
					bool flag2 = sslPolicyErrors == SslPolicyErrors.None || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) > SslPolicyErrors.None;
					result = flag2;
				}
				return result;
			}

			// Token: 0x060016C4 RID: 5828 RVA: 0x0008DA54 File Offset: 0x0008CA54
			private void BeginAuthenticateAsClientCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSslStream.EndAuthenticateAsClient(ar);
					this.m_pTcpClient.m_pTcpStream.IsOwner = false;
					this.m_pTcpClient.m_pTcpStream.Dispose();
					this.m_pTcpClient.m_IsSecure = true;
					this.m_pTcpClient.m_pTcpStream = new SmartStream(this.m_pSslStream, true);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x1700077A RID: 1914
			// (get) Token: 0x060016C5 RID: 5829 RVA: 0x0008DAE0 File Offset: 0x0008CAE0
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700077B RID: 1915
			// (get) Token: 0x060016C6 RID: 5830 RVA: 0x0008DAF8 File Offset: 0x0008CAF8
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

			// Token: 0x1400009F RID: 159
			// (add) Token: 0x060016C7 RID: 5831 RVA: 0x0008DB4C File Offset: 0x0008CB4C
			// (remove) Token: 0x060016C8 RID: 5832 RVA: 0x0008DB84 File Offset: 0x0008CB84
			
			public event EventHandler<EventArgs<TCP_Client.SwitchToSecureAsyncOP>> CompletedAsync = null;

			// Token: 0x060016C9 RID: 5833 RVA: 0x0008DBBC File Offset: 0x0008CBBC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<TCP_Client.SwitchToSecureAsyncOP>(this));
				}
			}

			// Token: 0x04000951 RID: 2385
			private object m_pLock = new object();

			// Token: 0x04000952 RID: 2386
			private bool m_RiseCompleted = false;

			// Token: 0x04000953 RID: 2387
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000954 RID: 2388
			private Exception m_pException = null;

			// Token: 0x04000955 RID: 2389
			private RemoteCertificateValidationCallback m_pCertCallback = null;

			// Token: 0x04000956 RID: 2390
			private TCP_Client m_pTcpClient = null;

			// Token: 0x04000957 RID: 2391
			private SslStream m_pSslStream = null;
		}

		// Token: 0x0200027A RID: 634
		// (Invoke) Token: 0x060016CB RID: 5835
		protected delegate void CompleteConnectCallback(Exception error);

		// Token: 0x0200027B RID: 635
		// (Invoke) Token: 0x060016CF RID: 5839
		private delegate void BeginConnectHostDelegate(string host, int port, bool ssl);

		// Token: 0x0200027C RID: 636
		// (Invoke) Token: 0x060016D3 RID: 5843
		private delegate void BeginConnectEPDelegate(IPEndPoint localEP, IPEndPoint remoteEP, bool ssl);
	}
}
