using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.POP3.Client
{
	// Token: 0x020000E7 RID: 231
	public class POP3_Client : TCP_Client
	{
		// Token: 0x06000949 RID: 2377 RVA: 0x00037D3C File Offset: 0x00036D3C
		public POP3_Client()
		{
			this.m_pExtCapabilities = new List<string>();
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00037D8E File Offset: 0x00036D8E
		public override void Dispose()
		{
			base.Dispose();
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x00037D98 File Offset: 0x00036D98
		public override void Disconnect()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("POP3 client is not connected.");
			}
			try
			{
				base.WriteLine("QUIT");
			}
			catch
			{
			}
			try
			{
				base.Disconnect();
			}
			catch
			{
			}
			this.m_GreetingText = "";
			this.m_ApopHashKey = "";
			this.m_pExtCapabilities = new List<string>();
			this.m_IsUidlSupported = false;
			bool flag2 = this.m_pMessages != null;
			if (flag2)
			{
				this.m_pMessages.Dispose();
				this.m_pMessages = null;
			}
			this.m_pAuthdUserIdentity = null;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00037E70 File Offset: 0x00036E70
		public void Capa()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			using (POP3_Client.CapaAsyncOP capaAsyncOP = new POP3_Client.CapaAsyncOP())
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					capaAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.CapaAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag2 = !this.CapaAsync(capaAsyncOP);
					if (flag2)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag3 = capaAsyncOP.Error != null;
					if (flag3)
					{
						throw capaAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00037F70 File Offset: 0x00036F70
		public bool CapaAsync(POP3_Client.CapaAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
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

		// Token: 0x0600094E RID: 2382 RVA: 0x00037FF4 File Offset: 0x00036FF4
		public void Stls(RemoteCertificateValidationCallback certCallback)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("The STLS command is only valid in non-authenticated state.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			using (POP3_Client.StlsAsyncOP stlsAsyncOP = new POP3_Client.StlsAsyncOP(certCallback))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					stlsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.StlsAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag2 = !this.StlsAsync(stlsAsyncOP);
					if (flag2)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag3 = stlsAsyncOP.Error != null;
					if (flag3)
					{
						throw stlsAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00038130 File Offset: 0x00037130
		public bool StlsAsync(POP3_Client.StlsAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("The STLS command is only valid in non-authenticated state.");
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

		// Token: 0x06000950 RID: 2384 RVA: 0x000381CC File Offset: 0x000371CC
		public void Login(string user, string password)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Session is already authenticated.");
			}
			bool flag2 = user == null;
			if (flag2)
			{
				throw new ArgumentNullException("user");
			}
			bool flag3 = user == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'user' value must be specified.", "user");
			}
			bool flag4 = password == null;
			if (flag4)
			{
				throw new ArgumentNullException("password");
			}
			using (POP3_Client.LoginAsyncOP loginAsyncOP = new POP3_Client.LoginAsyncOP(user, password))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					loginAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.LoginAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag5 = !this.LoginAsync(loginAsyncOP);
					if (flag5)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag6 = loginAsyncOP.Error != null;
					if (flag6)
					{
						throw loginAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00038340 File Offset: 0x00037340
		public bool LoginAsync(POP3_Client.LoginAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Session is already authenticated.");
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

		// Token: 0x06000952 RID: 2386 RVA: 0x000383DC File Offset: 0x000373DC
		public void Auth(AUTH_SASL_Client sasl)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Connection is already authenticated.");
			}
			bool flag2 = sasl == null;
			if (flag2)
			{
				throw new ArgumentNullException("sasl");
			}
			using (POP3_Client.AuthAsyncOP authAsyncOP = new POP3_Client.AuthAsyncOP(sasl))
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					authAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.AuthAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.AuthAsync(authAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag4 = authAsyncOP.Error != null;
					if (flag4)
					{
						throw authAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00038518 File Offset: 0x00037518
		public bool AuthAsync(POP3_Client.AuthAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Connection is already authenticated.");
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

		// Token: 0x06000954 RID: 2388 RVA: 0x000385B4 File Offset: 0x000375B4
		public void Noop()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The NOOP command is only valid in TRANSACTION state.");
			}
			using (POP3_Client.NoopAsyncOP noopAsyncOP = new POP3_Client.NoopAsyncOP())
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					noopAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.NoopAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.NoopAsync(noopAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag4 = noopAsyncOP.Error != null;
					if (flag4)
					{
						throw noopAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x000386D4 File Offset: 0x000376D4
		public bool NoopAsync(POP3_Client.NoopAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The NOOP command is only valid in TRANSACTION(authenticated) state.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00038774 File Offset: 0x00037774
		public void Rset()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The RSET command is only valid in TRANSACTION state.");
			}
			using (POP3_Client.RsetAsyncOP rsetAsyncOP = new POP3_Client.RsetAsyncOP())
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					rsetAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.RsetAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.RsetAsync(rsetAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag4 = rsetAsyncOP.Error != null;
					if (flag4)
					{
						throw rsetAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00038894 File Offset: 0x00037894
		public bool RsetAsync(POP3_Client.RsetAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The RSET command is only valid in TRANSACTION(authenticated) state.");
			}
			bool flag3 = op == null;
			if (flag3)
			{
				throw new ArgumentNullException("op");
			}
			bool flag4 = op.State > AsyncOP_State.WaitingForStart;
			if (flag4)
			{
				throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
			}
			return op.Start(this);
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00038934 File Offset: 0x00037934
		private bool FillMessagesAsync(POP3_Client.FillMessagesAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
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

		// Token: 0x06000959 RID: 2393 RVA: 0x000389B8 File Offset: 0x000379B8
		private bool ListAsync(POP3_Client.ListAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
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

		// Token: 0x0600095A RID: 2394 RVA: 0x00038A3C File Offset: 0x00037A3C
		private bool UidlAsync(POP3_Client.UidlAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
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

		// Token: 0x0600095B RID: 2395 RVA: 0x00038AC0 File Offset: 0x00037AC0
		protected override void OnConnected(TCP_Client.CompleteConnectCallback callback)
		{
			SmartStream.ReadLineAsyncOP readGreetingOP = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
			readGreetingOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
			{
				this.ReadServerGreetingCompleted(readGreetingOP, callback);
			};
			bool flag = this.TcpStream.ReadLine(readGreetingOP, true);
			if (flag)
			{
				this.ReadServerGreetingCompleted(readGreetingOP, callback);
			}
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00038B3C File Offset: 0x00037B3C
		private void ReadServerGreetingCompleted(SmartStream.ReadLineAsyncOP op, TCP_Client.CompleteConnectCallback connectCallback)
		{
			Exception error = null;
			try
			{
				bool flag = op.Error != null;
				if (flag)
				{
					error = op.Error;
				}
				else
				{
					string lineUtf = op.LineUtf8;
					base.LogAddRead((long)op.BytesInBuffer, lineUtf);
					bool flag2 = op.LineUtf8.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						this.m_GreetingText = lineUtf.Substring(3).Trim();
						bool flag3 = lineUtf.IndexOf("<") > -1 && lineUtf.IndexOf(">") > -1;
						if (flag3)
						{
							this.m_ApopHashKey = lineUtf.Substring(lineUtf.IndexOf("<"), lineUtf.LastIndexOf(">") - lineUtf.IndexOf("<") + 1);
						}
					}
					else
					{
						error = new POP3_ClientException(lineUtf);
					}
				}
			}
			catch (Exception ex)
			{
				error = ex;
			}
			connectCallback(error);
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x00038C30 File Offset: 0x00037C30
		public string GreetingText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_GreetingText;
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x0600095E RID: 2398 RVA: 0x00038C80 File Offset: 0x00037C80
		[Obsolete("USe ExtendedCapabilities instead !")]
		public string[] ExtenededCapabilities
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pExtCapabilities.ToArray();
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x00038CD4 File Offset: 0x00037CD4
		public string[] ExtendedCapabilities
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pExtCapabilities.ToArray();
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000960 RID: 2400 RVA: 0x00038D28 File Offset: 0x00037D28
		public bool IsUidlSupported
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					throw new InvalidOperationException("You must authenticate first.");
				}
				return this.m_IsUidlSupported;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x00038D90 File Offset: 0x00037D90
		public POP3_ClientMessageCollection Messages
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					throw new InvalidOperationException("You must authenticate first.");
				}
				return this.m_pMessages;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000962 RID: 2402 RVA: 0x00038DF8 File Offset: 0x00037DF8
		public override GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pAuthdUserIdentity;
			}
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00038E48 File Offset: 0x00037E48
		[Obsolete("Use Stls/StlsAsync method instead.")]
		public void StartTLS()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("The STLS command is only valid in non-authenticated state.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			base.WriteLine("STLS");
			string text = base.ReadLine();
			bool flag2 = !text.ToUpper().StartsWith("+OK");
			if (flag2)
			{
				throw new POP3_ClientException(text);
			}
			base.SwitchToSecure();
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00038EF4 File Offset: 0x00037EF4
		[Obsolete("Use Stls/StlsAsync method instead.")]
		public IAsyncResult BeginStartTLS(AsyncCallback callback, object state)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("The STLS command is only valid in non-authenticated state.");
			}
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			POP3_Client.StartTLSDelegate startTLSDelegate = new POP3_Client.StartTLSDelegate(this.StartTLS);
			AsyncResultState asyncResultState = new AsyncResultState(this, startTLSDelegate, callback, state);
			asyncResultState.SetAsyncResult(startTLSDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x00038FA0 File Offset: 0x00037FA0
		[Obsolete("Use Stls/StlsAsync method instead.")]
		public void EndStartTLS(IAsyncResult asyncResult)
		{
			bool isDisposed = base.IsDisposed;
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
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginReset was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is POP3_Client.StartTLSDelegate;
			if (flag3)
			{
				((POP3_Client.StartTLSDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00039064 File Offset: 0x00038064
		[Obsolete("Use Login/LoginAsync method instead.")]
		public void Authenticate(string userName, string password, bool tryApop)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Session is already authenticated.");
			}
			bool flag2 = tryApop && this.m_ApopHashKey.Length > 0;
			if (flag2)
			{
				string str = Net_Utils.ComputeMd5(this.m_ApopHashKey + password, true);
				int num = this.TcpStream.WriteLine("APOP " + userName + " " + str);
				base.LogAddWrite((long)num, "APOP " + userName + " " + str);
				string text = base.ReadLine();
				bool flag3 = text.StartsWith("+OK");
				if (!flag3)
				{
					throw new POP3_ClientException(text);
				}
				this.m_pAuthdUserIdentity = new GenericIdentity(userName, "apop");
			}
			else
			{
				int num2 = this.TcpStream.WriteLine("USER " + userName);
				base.LogAddWrite((long)num2, "USER " + userName);
				string text2 = base.ReadLine();
				bool flag4 = text2.StartsWith("+OK");
				if (!flag4)
				{
					throw new POP3_ClientException(text2);
				}
				num2 = this.TcpStream.WriteLine("PASS " + password);
				base.LogAddWrite((long)num2, "PASS <***REMOVED***>");
				text2 = base.ReadLine();
				bool flag5 = text2.StartsWith("+OK");
				if (!flag5)
				{
					throw new POP3_ClientException(text2);
				}
				this.m_pAuthdUserIdentity = new GenericIdentity(userName, "pop3-user/pass");
			}
			bool isAuthenticated2 = base.IsAuthenticated;
			if (isAuthenticated2)
			{
				this.FillMessages();
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00039230 File Offset: 0x00038230
		[Obsolete("Use Login/LoginAsync method instead.")]
		public IAsyncResult BeginAuthenticate(string userName, string password, bool tryApop, AsyncCallback callback, object state)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool isAuthenticated = base.IsAuthenticated;
			if (isAuthenticated)
			{
				throw new InvalidOperationException("Session is already authenticated.");
			}
			POP3_Client.AuthenticateDelegate authenticateDelegate = new POP3_Client.AuthenticateDelegate(this.Authenticate);
			AsyncResultState asyncResultState = new AsyncResultState(this, authenticateDelegate, callback, state);
			asyncResultState.SetAsyncResult(authenticateDelegate.BeginInvoke(userName, password, tryApop, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x000392C8 File Offset: 0x000382C8
		[Obsolete("Use Login/LoginAsync method instead.")]
		public void EndAuthenticate(IAsyncResult asyncResult)
		{
			bool isDisposed = base.IsDisposed;
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
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginAuthenticate method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginAuthenticate was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is POP3_Client.AuthenticateDelegate;
			if (flag3)
			{
				((POP3_Client.AuthenticateDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginAuthenticate method.");
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0003938C File Offset: 0x0003838C
		[Obsolete("Use Noop/NoopAsync method instead.")]
		public IAsyncResult BeginNoop(AsyncCallback callback, object state)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			POP3_Client.NoopDelegate noopDelegate = new POP3_Client.NoopDelegate(this.Noop);
			AsyncResultState asyncResultState = new AsyncResultState(this, noopDelegate, callback, state);
			asyncResultState.SetAsyncResult(noopDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00039408 File Offset: 0x00038408
		[Obsolete("Use Noop/NoopAsync method instead.")]
		public void EndNoop(IAsyncResult asyncResult)
		{
			bool isDisposed = base.IsDisposed;
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
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginNoop method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginNoop was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is POP3_Client.NoopDelegate;
			if (flag3)
			{
				((POP3_Client.NoopDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginNoop method.");
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x000394CC File Offset: 0x000384CC
		[Obsolete("Use Rset/RsetAsync method instead.")]
		public IAsyncResult BeginReset(AsyncCallback callback, object state)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The RSET command is only valid in authenticated state.");
			}
			POP3_Client.ResetDelegate resetDelegate = new POP3_Client.ResetDelegate(this.Reset);
			AsyncResultState asyncResultState = new AsyncResultState(this, resetDelegate, callback, state);
			asyncResultState.SetAsyncResult(resetDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00039564 File Offset: 0x00038564
		[Obsolete("Use Rset/RsetAsync method instead.")]
		public void EndReset(IAsyncResult asyncResult)
		{
			bool isDisposed = base.IsDisposed;
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
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginReset was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is POP3_Client.ResetDelegate;
			if (flag3)
			{
				((POP3_Client.ResetDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00039628 File Offset: 0x00038628
		[Obsolete("Use Rset/RsetAsync method instead.")]
		public void Reset()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("You must connect first.");
			}
			bool flag2 = !base.IsAuthenticated;
			if (flag2)
			{
				throw new InvalidOperationException("The RSET command is only valid in TRANSACTION state.");
			}
			using (POP3_Client.RsetAsyncOP rsetAsyncOP = new POP3_Client.RsetAsyncOP())
			{
				using (ManualResetEvent wait = new ManualResetEvent(false))
				{
					rsetAsyncOP.CompletedAsync += delegate(object s1, EventArgs<POP3_Client.RsetAsyncOP> e1)
					{
						wait.Set();
					};
					bool flag3 = !this.RsetAsync(rsetAsyncOP);
					if (flag3)
					{
						wait.Set();
					}
					wait.WaitOne();
					wait.Close();
					bool flag4 = rsetAsyncOP.Error != null;
					if (flag4)
					{
						throw rsetAsyncOP.Error;
					}
				}
			}
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x00039748 File Offset: 0x00038748
		[Obsolete("deprecated")]
		private void FillMessages()
		{
			this.m_pMessages = new POP3_ClientMessageCollection(this);
			base.WriteLine("LIST");
			string text = base.ReadLine();
			bool flag = text.StartsWith("+OK");
			if (flag)
			{
				for (;;)
				{
					text = base.ReadLine();
					bool flag2 = text.Trim() == ".";
					if (flag2)
					{
						break;
					}
					string[] array = text.Trim().Split(new char[]
					{
						' '
					});
					this.m_pMessages.Add(Convert.ToInt32(array[1]));
				}
				base.WriteLine("UIDL");
				text = base.ReadLine();
				bool flag3 = text.StartsWith("+OK");
				if (flag3)
				{
					this.m_IsUidlSupported = true;
					for (;;)
					{
						text = base.ReadLine();
						bool flag4 = text.Trim() == ".";
						if (flag4)
						{
							break;
						}
						string[] array2 = text.Trim().Split(new char[]
						{
							' '
						});
						this.m_pMessages[Convert.ToInt32(array2[0]) - 1].SetUID(array2[1]);
					}
				}
				else
				{
					this.m_IsUidlSupported = false;
				}
				return;
			}
			throw new POP3_ClientException(text);
		}

		// Token: 0x04000429 RID: 1065
		private string m_GreetingText = "";

		// Token: 0x0400042A RID: 1066
		private string m_ApopHashKey = "";

		// Token: 0x0400042B RID: 1067
		private List<string> m_pExtCapabilities = null;

		// Token: 0x0400042C RID: 1068
		private bool m_IsUidlSupported = false;

		// Token: 0x0400042D RID: 1069
		private POP3_ClientMessageCollection m_pMessages = null;

		// Token: 0x0400042E RID: 1070
		private GenericIdentity m_pAuthdUserIdentity = null;

		// Token: 0x020002A9 RID: 681
		public class CapaAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017AD RID: 6061 RVA: 0x0009215C File Offset: 0x0009115C
			public CapaAsyncOP()
			{
				this.m_pResponseLines = new List<string>();
			}

			// Token: 0x060017AE RID: 6062 RVA: 0x000921B4 File Offset: 0x000911B4
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.m_pResponseLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017AF RID: 6063 RVA: 0x000921F8 File Offset: 0x000911F8
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("CAPA\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "CAPA");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.CapaCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017B0 RID: 6064 RVA: 0x000922F8 File Offset: 0x000912F8
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

			// Token: 0x060017B1 RID: 6065 RVA: 0x00092370 File Offset: 0x00091370
			private void CapaCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.CapaReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.CapaReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017B2 RID: 6066 RVA: 0x0009243C File Offset: 0x0009143C
			private void CapaReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
							readLineOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
							{
								try
								{
									this.ReadMultiLineResponseLineCompleted(readLineOP);
									while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
									{
										this.ReadMultiLineResponseLineCompleted(readLineOP);
									}
								}
								catch (Exception ex2)
								{
									this.m_pException = ex2;
									this.m_pPop3Client.LogAddException("Exception: " + ex2.Message, ex2);
									this.SetState(AsyncOP_State.Completed);
								}
							};
							while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
							{
								this.ReadMultiLineResponseLineCompleted(readLineOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017B3 RID: 6067 RVA: 0x000925CC File Offset: 0x000915CC
			private void ReadMultiLineResponseLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = op.BytesInBuffer == 0;
						if (flag2)
						{
							this.m_pException = new IOException("POP3 server closed connection unexpectedly.");
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							bool flag3 = string.Equals(op.LineUtf8, ".", StringComparison.InvariantCultureIgnoreCase);
							if (flag3)
							{
								this.m_pPop3Client.m_pExtCapabilities.Clear();
								this.m_pPop3Client.m_pExtCapabilities.AddRange(this.m_pResponseLines);
								this.SetState(AsyncOP_State.Completed);
							}
							else
							{
								this.m_pResponseLines.Add(op.LineUtf8);
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x170007CD RID: 1997
			// (get) Token: 0x060017B4 RID: 6068 RVA: 0x00092710 File Offset: 0x00091710
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007CE RID: 1998
			// (get) Token: 0x060017B5 RID: 6069 RVA: 0x00092728 File Offset: 0x00091728
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

			// Token: 0x140000A6 RID: 166
			// (add) Token: 0x060017B6 RID: 6070 RVA: 0x0009277C File Offset: 0x0009177C
			// (remove) Token: 0x060017B7 RID: 6071 RVA: 0x000927B4 File Offset: 0x000917B4
			
			public event EventHandler<EventArgs<POP3_Client.CapaAsyncOP>> CompletedAsync = null;

			// Token: 0x060017B8 RID: 6072 RVA: 0x000927EC File Offset: 0x000917EC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.CapaAsyncOP>(this));
				}
			}

			// Token: 0x040009EB RID: 2539
			private object m_pLock = new object();

			// Token: 0x040009EC RID: 2540
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009ED RID: 2541
			private Exception m_pException = null;

			// Token: 0x040009EE RID: 2542
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009EF RID: 2543
			private bool m_RiseCompleted = false;

			// Token: 0x040009F0 RID: 2544
			private List<string> m_pResponseLines = null;
		}

		// Token: 0x020002AA RID: 682
		public class StlsAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017B9 RID: 6073 RVA: 0x0009281C File Offset: 0x0009181C
			public StlsAsyncOP(RemoteCertificateValidationCallback certCallback)
			{
				this.m_pCertCallback = certCallback;
			}

			// Token: 0x060017BA RID: 6074 RVA: 0x00092870 File Offset: 0x00091870
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017BB RID: 6075 RVA: 0x000928AC File Offset: 0x000918AC
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("STLS\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "STLS");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.StlsCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017BC RID: 6076 RVA: 0x000929AC File Offset: 0x000919AC
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

			// Token: 0x060017BD RID: 6077 RVA: 0x00092A24 File Offset: 0x00091A24
			private void StlsCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.StlsReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.StlsReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017BE RID: 6078 RVA: 0x00092AF0 File Offset: 0x00091AF0
			private void StlsReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							this.m_pPop3Client.LogAddText("Starting TLS handshake.");
							TCP_Client.SwitchToSecureAsyncOP switchSecureOP = new TCP_Client.SwitchToSecureAsyncOP(this.m_pCertCallback);
							switchSecureOP.CompletedAsync += delegate(object s, EventArgs<TCP_Client.SwitchToSecureAsyncOP> e)
							{
								this.SwitchToSecureCompleted(switchSecureOP);
							};
							bool flag3 = !this.m_pPop3Client.SwitchToSecureAsync(switchSecureOP);
							if (flag3)
							{
								this.SwitchToSecureCompleted(switchSecureOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017BF RID: 6079 RVA: 0x00092C7C File Offset: 0x00091C7C
			private void SwitchToSecureCompleted(TCP_Client.SwitchToSecureAsyncOP op)
			{
				bool flag = op == null;
				if (flag)
				{
					throw new ArgumentNullException("op");
				}
				try
				{
					bool flag2 = op.Error != null;
					if (flag2)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pPop3Client.LogAddText("TLS handshake completed successfully.");
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pPop3Client.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x170007CF RID: 1999
			// (get) Token: 0x060017C0 RID: 6080 RVA: 0x00092D4C File Offset: 0x00091D4C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007D0 RID: 2000
			// (get) Token: 0x060017C1 RID: 6081 RVA: 0x00092D64 File Offset: 0x00091D64
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

			// Token: 0x140000A7 RID: 167
			// (add) Token: 0x060017C2 RID: 6082 RVA: 0x00092DB8 File Offset: 0x00091DB8
			// (remove) Token: 0x060017C3 RID: 6083 RVA: 0x00092DF0 File Offset: 0x00091DF0
			
			public event EventHandler<EventArgs<POP3_Client.StlsAsyncOP>> CompletedAsync = null;

			// Token: 0x060017C4 RID: 6084 RVA: 0x00092E28 File Offset: 0x00091E28
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.StlsAsyncOP>(this));
				}
			}

			// Token: 0x040009F2 RID: 2546
			private object m_pLock = new object();

			// Token: 0x040009F3 RID: 2547
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009F4 RID: 2548
			private Exception m_pException = null;

			// Token: 0x040009F5 RID: 2549
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009F6 RID: 2550
			private bool m_RiseCompleted = false;

			// Token: 0x040009F7 RID: 2551
			private RemoteCertificateValidationCallback m_pCertCallback = null;
		}

		// Token: 0x020002AB RID: 683
		public class LoginAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017C5 RID: 6085 RVA: 0x00092E58 File Offset: 0x00091E58
			public LoginAsyncOP(string user, string password)
			{
				bool flag = user == null;
				if (flag)
				{
					throw new ArgumentNullException("user");
				}
				bool flag2 = user == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Argument 'user' value must be specified.", "user");
				}
				bool flag3 = password == null;
				if (flag3)
				{
					throw new ArgumentNullException("password");
				}
				this.m_User = user;
				this.m_Password = password;
			}

			// Token: 0x060017C6 RID: 6086 RVA: 0x00092F00 File Offset: 0x00091F00
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017C7 RID: 6087 RVA: 0x00092F3C File Offset: 0x00091F3C
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("USER " + this.m_User + "\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "USER " + this.m_User);
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.UserCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017C8 RID: 6088 RVA: 0x00093058 File Offset: 0x00092058
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

			// Token: 0x060017C9 RID: 6089 RVA: 0x000930D0 File Offset: 0x000920D0
			private void UserCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.UserReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.UserReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017CA RID: 6090 RVA: 0x0009319C File Offset: 0x0009219C
			private void UserReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							byte[] bytes = Encoding.UTF8.GetBytes("PASS " + this.m_Password + "\r\n");
							this.m_pPop3Client.LogAddWrite((long)bytes.Length, "PASS <***REMOVED***>");
							this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.PassCommandSendingCompleted), null);
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017CB RID: 6091 RVA: 0x0009330C File Offset: 0x0009230C
			private void PassCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.PassReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.PassReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017CC RID: 6092 RVA: 0x000933D8 File Offset: 0x000923D8
			private void PassReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							POP3_Client.FillMessagesAsyncOP fillOP = new POP3_Client.FillMessagesAsyncOP();
							fillOP.CompletedAsync += delegate(object sender, EventArgs<POP3_Client.FillMessagesAsyncOP> e)
							{
								this.FillMessagesCompleted(fillOP);
							};
							bool flag3 = !this.m_pPop3Client.FillMessagesAsync(fillOP);
							if (flag3)
							{
								this.FillMessagesCompleted(fillOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017CD RID: 6093 RVA: 0x0009354C File Offset: 0x0009254C
			private void FillMessagesCompleted(POP3_Client.FillMessagesAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.m_pAuthdUserIdentity = new GenericIdentity(this.m_User, "pop3-user/pass");
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007D1 RID: 2001
			// (get) Token: 0x060017CE RID: 6094 RVA: 0x00093618 File Offset: 0x00092618
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007D2 RID: 2002
			// (get) Token: 0x060017CF RID: 6095 RVA: 0x00093630 File Offset: 0x00092630
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

			// Token: 0x140000A8 RID: 168
			// (add) Token: 0x060017D0 RID: 6096 RVA: 0x00093684 File Offset: 0x00092684
			// (remove) Token: 0x060017D1 RID: 6097 RVA: 0x000936BC File Offset: 0x000926BC
			
			public event EventHandler<EventArgs<POP3_Client.LoginAsyncOP>> CompletedAsync = null;

			// Token: 0x060017D2 RID: 6098 RVA: 0x000936F4 File Offset: 0x000926F4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.LoginAsyncOP>(this));
				}
			}

			// Token: 0x040009F9 RID: 2553
			private object m_pLock = new object();

			// Token: 0x040009FA RID: 2554
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x040009FB RID: 2555
			private Exception m_pException = null;

			// Token: 0x040009FC RID: 2556
			private POP3_Client m_pPop3Client = null;

			// Token: 0x040009FD RID: 2557
			private bool m_RiseCompleted = false;

			// Token: 0x040009FE RID: 2558
			private string m_User = null;

			// Token: 0x040009FF RID: 2559
			private string m_Password = null;
		}

		// Token: 0x020002AC RID: 684
		public class AuthAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017D3 RID: 6099 RVA: 0x00093724 File Offset: 0x00092724
			public AuthAsyncOP(AUTH_SASL_Client sasl)
			{
				bool flag = sasl == null;
				if (flag)
				{
					throw new ArgumentNullException("sasl");
				}
				this.m_pSASL = sasl;
			}

			// Token: 0x060017D4 RID: 6100 RVA: 0x0009378C File Offset: 0x0009278C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017D5 RID: 6101 RVA: 0x000937C8 File Offset: 0x000927C8
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool supportsInitialResponse = this.m_pSASL.SupportsInitialResponse;
					if (supportsInitialResponse)
					{
						byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(new string[]
						{
							"AUTH ",
							this.m_pSASL.Name,
							" ",
							Convert.ToBase64String(this.m_pSASL.Continue(null)),
							"\r\n"
						}));
						this.m_pPop3Client.LogAddWrite((long)bytes.Length, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]));
						this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
					}
					else
					{
						byte[] bytes2 = Encoding.UTF8.GetBytes("AUTH " + this.m_pSASL.Name + "\r\n");
						this.m_pPop3Client.LogAddWrite((long)bytes2.Length, "AUTH " + this.m_pSASL.Name);
						this.m_pPop3Client.TcpStream.BeginWrite(bytes2, 0, bytes2.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017D6 RID: 6102 RVA: 0x000939C0 File Offset: 0x000929C0
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

			// Token: 0x060017D7 RID: 6103 RVA: 0x00093A38 File Offset: 0x00092A38
			private void AuthCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.AuthReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.AuthReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017D8 RID: 6104 RVA: 0x00093B04 File Offset: 0x00092B04
			private void AuthReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							POP3_Client.FillMessagesAsyncOP fillOP = new POP3_Client.FillMessagesAsyncOP();
							fillOP.CompletedAsync += delegate(object sender, EventArgs<POP3_Client.FillMessagesAsyncOP> e)
							{
								this.FillMessagesCompleted(fillOP);
							};
							bool flag3 = !this.m_pPop3Client.FillMessagesAsync(fillOP);
							if (flag3)
							{
								this.FillMessagesCompleted(fillOP);
							}
						}
						else
						{
							bool flag4 = op.LineUtf8.StartsWith("+");
							if (flag4)
							{
								byte[] serverResponse = Convert.FromBase64String(op.LineUtf8.Split(new char[]
								{
									' '
								}, 2)[1]);
								byte[] inArray = this.m_pSASL.Continue(serverResponse);
								byte[] bytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(inArray) + "\r\n");
								this.m_pPop3Client.LogAddWrite((long)bytes.Length, Convert.ToBase64String(inArray));
								this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
							}
							else
							{
								this.m_pException = new POP3_ClientException(op.LineUtf8);
								this.SetState(AsyncOP_State.Completed);
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017D9 RID: 6105 RVA: 0x00093D20 File Offset: 0x00092D20
			private void FillMessagesCompleted(POP3_Client.FillMessagesAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.m_pAuthdUserIdentity = new GenericIdentity(this.m_pSASL.UserName, this.m_pSASL.Name);
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007D3 RID: 2003
			// (get) Token: 0x060017DA RID: 6106 RVA: 0x00093DF8 File Offset: 0x00092DF8
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007D4 RID: 2004
			// (get) Token: 0x060017DB RID: 6107 RVA: 0x00093E10 File Offset: 0x00092E10
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

			// Token: 0x140000A9 RID: 169
			// (add) Token: 0x060017DC RID: 6108 RVA: 0x00093E64 File Offset: 0x00092E64
			// (remove) Token: 0x060017DD RID: 6109 RVA: 0x00093E9C File Offset: 0x00092E9C
			
			public event EventHandler<EventArgs<POP3_Client.AuthAsyncOP>> CompletedAsync = null;

			// Token: 0x060017DE RID: 6110 RVA: 0x00093ED4 File Offset: 0x00092ED4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.AuthAsyncOP>(this));
				}
			}

			// Token: 0x04000A01 RID: 2561
			private object m_pLock = new object();

			// Token: 0x04000A02 RID: 2562
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A03 RID: 2563
			private Exception m_pException = null;

			// Token: 0x04000A04 RID: 2564
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A05 RID: 2565
			private AUTH_SASL_Client m_pSASL = null;

			// Token: 0x04000A06 RID: 2566
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002AD RID: 685
		public class NoopAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017E0 RID: 6112 RVA: 0x00093F3C File Offset: 0x00092F3C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017E1 RID: 6113 RVA: 0x00093F78 File Offset: 0x00092F78
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("NOOP\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "NOOP");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.NoopCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017E2 RID: 6114 RVA: 0x00094078 File Offset: 0x00093078
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

			// Token: 0x060017E3 RID: 6115 RVA: 0x000940F0 File Offset: 0x000930F0
			private void NoopCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.NoopReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.NoopReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017E4 RID: 6116 RVA: 0x000941BC File Offset: 0x000931BC
			private void NoopReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007D5 RID: 2005
			// (get) Token: 0x060017E5 RID: 6117 RVA: 0x000942CC File Offset: 0x000932CC
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007D6 RID: 2006
			// (get) Token: 0x060017E6 RID: 6118 RVA: 0x000942E4 File Offset: 0x000932E4
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

			// Token: 0x140000AA RID: 170
			// (add) Token: 0x060017E7 RID: 6119 RVA: 0x00094338 File Offset: 0x00093338
			// (remove) Token: 0x060017E8 RID: 6120 RVA: 0x00094370 File Offset: 0x00093370
			
			public event EventHandler<EventArgs<POP3_Client.NoopAsyncOP>> CompletedAsync = null;

			// Token: 0x060017E9 RID: 6121 RVA: 0x000943A8 File Offset: 0x000933A8
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.NoopAsyncOP>(this));
				}
			}

			// Token: 0x04000A08 RID: 2568
			private object m_pLock = new object();

			// Token: 0x04000A09 RID: 2569
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A0A RID: 2570
			private Exception m_pException = null;

			// Token: 0x04000A0B RID: 2571
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A0C RID: 2572
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002AE RID: 686
		public class RsetAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017EB RID: 6123 RVA: 0x00094410 File Offset: 0x00093410
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017EC RID: 6124 RVA: 0x0009444C File Offset: 0x0009344C
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("RSET\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "RSET");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.RsetCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017ED RID: 6125 RVA: 0x0009454C File Offset: 0x0009354C
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

			// Token: 0x060017EE RID: 6126 RVA: 0x000945C4 File Offset: 0x000935C4
			private void RsetCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.RsetReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.RsetReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060017EF RID: 6127 RVA: 0x00094690 File Offset: 0x00093690
			private void RsetReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							foreach (object obj in this.m_pPop3Client.m_pMessages)
							{
								POP3_ClientMessage pop3_ClientMessage = (POP3_ClientMessage)obj;
								pop3_ClientMessage.SetMarkedForDeletion(false);
							}
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007D7 RID: 2007
			// (get) Token: 0x060017F0 RID: 6128 RVA: 0x00094814 File Offset: 0x00093814
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007D8 RID: 2008
			// (get) Token: 0x060017F1 RID: 6129 RVA: 0x0009482C File Offset: 0x0009382C
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

			// Token: 0x140000AB RID: 171
			// (add) Token: 0x060017F2 RID: 6130 RVA: 0x00094880 File Offset: 0x00093880
			// (remove) Token: 0x060017F3 RID: 6131 RVA: 0x000948B8 File Offset: 0x000938B8
			
			public event EventHandler<EventArgs<POP3_Client.RsetAsyncOP>> CompletedAsync = null;

			// Token: 0x060017F4 RID: 6132 RVA: 0x000948F0 File Offset: 0x000938F0
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.RsetAsyncOP>(this));
				}
			}

			// Token: 0x04000A0E RID: 2574
			private object m_pLock = new object();

			// Token: 0x04000A0F RID: 2575
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A10 RID: 2576
			private Exception m_pException = null;

			// Token: 0x04000A11 RID: 2577
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A12 RID: 2578
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002AF RID: 687
		private class FillMessagesAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060017F6 RID: 6134 RVA: 0x00094958 File Offset: 0x00093958
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060017F7 RID: 6135 RVA: 0x00094994 File Offset: 0x00093994
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					POP3_Client.ListAsyncOP listOP = new POP3_Client.ListAsyncOP();
					listOP.CompletedAsync += delegate(object sender, EventArgs<POP3_Client.ListAsyncOP> e)
					{
						this.ListCompleted(listOP);
					};
					bool flag2 = !this.m_pPop3Client.ListAsync(listOP);
					if (flag2)
					{
						this.ListCompleted(listOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x060017F8 RID: 6136 RVA: 0x00094AA8 File Offset: 0x00093AA8
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

			// Token: 0x060017F9 RID: 6137 RVA: 0x00094B20 File Offset: 0x00093B20
			private void ListCompleted(POP3_Client.ListAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.m_pMessages = new POP3_ClientMessageCollection(this.m_pPop3Client);
						foreach (string text in op.ResponseLines)
						{
							this.m_pPop3Client.m_pMessages.Add(Convert.ToInt32(text.Trim().Split(new char[]
							{
								' '
							})[1]));
						}
						POP3_Client.UidlAsyncOP uidlOP = new POP3_Client.UidlAsyncOP();
						uidlOP.CompletedAsync += delegate(object sender, EventArgs<POP3_Client.UidlAsyncOP> e)
						{
							this.UidlCompleted(uidlOP);
						};
						bool flag2 = !this.m_pPop3Client.UidlAsync(uidlOP);
						if (flag2)
						{
							this.UidlCompleted(uidlOP);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060017FA RID: 6138 RVA: 0x00094C98 File Offset: 0x00093C98
			private void UidlCompleted(POP3_Client.UidlAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.m_IsUidlSupported = true;
						foreach (string text in op.ResponseLines)
						{
							string[] array = text.Trim().Split(new char[]
							{
								' '
							});
							this.m_pPop3Client.m_pMessages[Convert.ToInt32(array[0]) - 1].SetUID(array[1]);
						}
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x170007D9 RID: 2009
			// (get) Token: 0x060017FB RID: 6139 RVA: 0x00094D80 File Offset: 0x00093D80
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007DA RID: 2010
			// (get) Token: 0x060017FC RID: 6140 RVA: 0x00094D98 File Offset: 0x00093D98
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

			// Token: 0x140000AC RID: 172
			// (add) Token: 0x060017FD RID: 6141 RVA: 0x00094DEC File Offset: 0x00093DEC
			// (remove) Token: 0x060017FE RID: 6142 RVA: 0x00094E24 File Offset: 0x00093E24
			
			public event EventHandler<EventArgs<POP3_Client.FillMessagesAsyncOP>> CompletedAsync = null;

			// Token: 0x060017FF RID: 6143 RVA: 0x00094E5C File Offset: 0x00093E5C
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.FillMessagesAsyncOP>(this));
				}
			}

			// Token: 0x04000A14 RID: 2580
			private object m_pLock = new object();

			// Token: 0x04000A15 RID: 2581
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A16 RID: 2582
			private Exception m_pException = null;

			// Token: 0x04000A17 RID: 2583
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A18 RID: 2584
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002B0 RID: 688
		private class ListAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001800 RID: 6144 RVA: 0x00094E8C File Offset: 0x00093E8C
			public ListAsyncOP()
			{
				this.m_pResponseLines = new List<string>();
			}

			// Token: 0x06001801 RID: 6145 RVA: 0x00094EE4 File Offset: 0x00093EE4
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.m_pResponseLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001802 RID: 6146 RVA: 0x00094F28 File Offset: 0x00093F28
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("LIST\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "LIST");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.ListCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x06001803 RID: 6147 RVA: 0x00095028 File Offset: 0x00094028
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

			// Token: 0x06001804 RID: 6148 RVA: 0x000950A0 File Offset: 0x000940A0
			private void ListCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.ListReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.ListReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001805 RID: 6149 RVA: 0x0009516C File Offset: 0x0009416C
			private void ListReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
							readLineOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
							{
								try
								{
									this.ReadMultiLineResponseLineCompleted(readLineOP);
									while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
									{
										this.ReadMultiLineResponseLineCompleted(readLineOP);
									}
								}
								catch (Exception ex2)
								{
									this.m_pException = ex2;
									this.m_pPop3Client.LogAddException("Exception: " + ex2.Message, ex2);
									this.SetState(AsyncOP_State.Completed);
								}
							};
							while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
							{
								this.ReadMultiLineResponseLineCompleted(readLineOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x06001806 RID: 6150 RVA: 0x000952FC File Offset: 0x000942FC
			private void ReadMultiLineResponseLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = op.BytesInBuffer == 0;
						if (flag2)
						{
							this.m_pException = new IOException("POP3 server closed connection unexpectedly.");
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							bool flag3 = string.Equals(op.LineUtf8, ".", StringComparison.InvariantCultureIgnoreCase);
							if (flag3)
							{
								this.m_pPop3Client.m_pExtCapabilities.Clear();
								this.m_pPop3Client.m_pExtCapabilities.AddRange(this.m_pResponseLines);
								this.SetState(AsyncOP_State.Completed);
							}
							else
							{
								this.m_pResponseLines.Add(op.LineUtf8);
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x170007DB RID: 2011
			// (get) Token: 0x06001807 RID: 6151 RVA: 0x00095440 File Offset: 0x00094440
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007DC RID: 2012
			// (get) Token: 0x06001808 RID: 6152 RVA: 0x00095458 File Offset: 0x00094458
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

			// Token: 0x170007DD RID: 2013
			// (get) Token: 0x06001809 RID: 6153 RVA: 0x000954AC File Offset: 0x000944AC
			public string[] ResponseLines
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
					return this.m_pResponseLines.ToArray();
				}
			}

			// Token: 0x140000AD RID: 173
			// (add) Token: 0x0600180A RID: 6154 RVA: 0x00095504 File Offset: 0x00094504
			// (remove) Token: 0x0600180B RID: 6155 RVA: 0x0009553C File Offset: 0x0009453C
			
			public event EventHandler<EventArgs<POP3_Client.ListAsyncOP>> CompletedAsync = null;

			// Token: 0x0600180C RID: 6156 RVA: 0x00095574 File Offset: 0x00094574
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.ListAsyncOP>(this));
				}
			}

			// Token: 0x04000A1A RID: 2586
			private object m_pLock = new object();

			// Token: 0x04000A1B RID: 2587
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A1C RID: 2588
			private Exception m_pException = null;

			// Token: 0x04000A1D RID: 2589
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A1E RID: 2590
			private bool m_RiseCompleted = false;

			// Token: 0x04000A1F RID: 2591
			private List<string> m_pResponseLines = null;
		}

		// Token: 0x020002B1 RID: 689
		private class UidlAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600180D RID: 6157 RVA: 0x000955A4 File Offset: 0x000945A4
			public UidlAsyncOP()
			{
				this.m_pResponseLines = new List<string>();
			}

			// Token: 0x0600180E RID: 6158 RVA: 0x000955FC File Offset: 0x000945FC
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pPop3Client = null;
					this.m_pResponseLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x0600180F RID: 6159 RVA: 0x00095640 File Offset: 0x00094640
			internal bool Start(POP3_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pPop3Client = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("UIDL\r\n");
					this.m_pPop3Client.LogAddWrite((long)bytes.Length, "UIDL");
					this.m_pPop3Client.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.UidlCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
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

			// Token: 0x06001810 RID: 6160 RVA: 0x00095740 File Offset: 0x00094740
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

			// Token: 0x06001811 RID: 6161 RVA: 0x000957B8 File Offset: 0x000947B8
			private void UidlCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pPop3Client.TcpStream.EndWrite(ar);
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						this.UidlReadResponseCompleted(op);
					};
					bool flag = this.m_pPop3Client.TcpStream.ReadLine(op, true);
					if (flag)
					{
						this.UidlReadResponseCompleted(op);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001812 RID: 6162 RVA: 0x00095884 File Offset: 0x00094884
			private void UidlReadResponseCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = string.Equals(op.LineUtf8.Split(new char[]
						{
							' '
						}, 2)[0], "+OK", StringComparison.InvariantCultureIgnoreCase);
						if (flag2)
						{
							SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
							readLineOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
							{
								try
								{
									this.ReadMultiLineResponseLineCompleted(readLineOP);
									while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
									{
										this.ReadMultiLineResponseLineCompleted(readLineOP);
									}
								}
								catch (Exception ex2)
								{
									this.m_pException = ex2;
									this.m_pPop3Client.LogAddException("Exception: " + ex2.Message, ex2);
									this.SetState(AsyncOP_State.Completed);
								}
							};
							while (this.State == AsyncOP_State.Active && this.m_pPop3Client.TcpStream.ReadLine(readLineOP, true))
							{
								this.ReadMultiLineResponseLineCompleted(readLineOP);
							}
						}
						else
						{
							this.m_pException = new POP3_ClientException(op.LineUtf8);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x06001813 RID: 6163 RVA: 0x00095A14 File Offset: 0x00094A14
			private void ReadMultiLineResponseLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						this.m_pException = op.Error;
						this.m_pPop3Client.LogAddException("Exception: " + op.Error.Message, op.Error);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pPop3Client.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						bool flag2 = op.BytesInBuffer == 0;
						if (flag2)
						{
							this.m_pException = new IOException("POP3 server closed connection unexpectedly.");
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							bool flag3 = string.Equals(op.LineUtf8, ".", StringComparison.InvariantCultureIgnoreCase);
							if (flag3)
							{
								this.m_pPop3Client.m_pExtCapabilities.Clear();
								this.m_pPop3Client.m_pExtCapabilities.AddRange(this.m_pResponseLines);
								this.SetState(AsyncOP_State.Completed);
							}
							else
							{
								this.m_pResponseLines.Add(op.LineUtf8);
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					this.m_pPop3Client.LogAddException("Exception: " + ex.Message, ex);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x170007DE RID: 2014
			// (get) Token: 0x06001814 RID: 6164 RVA: 0x00095B58 File Offset: 0x00094B58
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x170007DF RID: 2015
			// (get) Token: 0x06001815 RID: 6165 RVA: 0x00095B70 File Offset: 0x00094B70
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

			// Token: 0x170007E0 RID: 2016
			// (get) Token: 0x06001816 RID: 6166 RVA: 0x00095BC4 File Offset: 0x00094BC4
			public string[] ResponseLines
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
					return this.m_pResponseLines.ToArray();
				}
			}

			// Token: 0x140000AE RID: 174
			// (add) Token: 0x06001817 RID: 6167 RVA: 0x00095C1C File Offset: 0x00094C1C
			// (remove) Token: 0x06001818 RID: 6168 RVA: 0x00095C54 File Offset: 0x00094C54
			
			public event EventHandler<EventArgs<POP3_Client.UidlAsyncOP>> CompletedAsync = null;

			// Token: 0x06001819 RID: 6169 RVA: 0x00095C8C File Offset: 0x00094C8C
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<POP3_Client.UidlAsyncOP>(this));
				}
			}

			// Token: 0x04000A21 RID: 2593
			private object m_pLock = new object();

			// Token: 0x04000A22 RID: 2594
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000A23 RID: 2595
			private Exception m_pException = null;

			// Token: 0x04000A24 RID: 2596
			private POP3_Client m_pPop3Client = null;

			// Token: 0x04000A25 RID: 2597
			private bool m_RiseCompleted = false;

			// Token: 0x04000A26 RID: 2598
			private List<string> m_pResponseLines = null;
		}

		// Token: 0x020002B2 RID: 690
		// (Invoke) Token: 0x0600181B RID: 6171
		private delegate void StartTLSDelegate();

		// Token: 0x020002B3 RID: 691
		// (Invoke) Token: 0x0600181F RID: 6175
		private delegate void AuthenticateDelegate(string userName, string password, bool tryApop);

		// Token: 0x020002B4 RID: 692
		// (Invoke) Token: 0x06001823 RID: 6179
		private delegate void NoopDelegate();

		// Token: 0x020002B5 RID: 693
		// (Invoke) Token: 0x06001827 RID: 6183
		private delegate void ResetDelegate();
	}
}
