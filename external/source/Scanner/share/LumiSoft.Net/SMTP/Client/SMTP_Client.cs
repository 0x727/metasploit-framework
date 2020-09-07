using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.DNS;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.IO;
using LumiSoft.Net.Mail;
using LumiSoft.Net.Mime;
using LumiSoft.Net.MIME;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.SMTP.Client
{
	// Token: 0x0200014A RID: 330
	public class SMTP_Client : TCP_Client
	{
		// Token: 0x06000D20 RID: 3360 RVA: 0x00037D8E File Offset: 0x00036D8E
		public override void Dispose()
		{
			base.Dispose();
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x000519B4 File Offset: 0x000509B4
		public override void Disconnect()
		{
			this.Disconnect(true);
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x000519C0 File Offset: 0x000509C0
		public void Disconnect(bool sendQuit)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (flag)
			{
				throw new InvalidOperationException("SMTP client is not connected.");
			}
			try
			{
				if (sendQuit)
				{
					base.WriteLine("QUIT");
					base.ReadLine();
				}
			}
			catch
			{
			}
			this.m_LocalHostName = null;
			this.m_RemoteHostName = null;
			this.m_GreetingText = "";
			this.m_IsEsmtpSupported = false;
			this.m_pEsmtpFeatures = null;
			this.m_MailFrom = null;
			this.m_pRecipients = null;
			this.m_pAuthdUserIdentity = null;
			try
			{
				base.Disconnect();
			}
			catch
			{
			}
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x00051A90 File Offset: 0x00050A90
		public void EhloHelo(string hostName)
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
			bool flag2 = hostName == null;
			if (flag2)
			{
				throw new ArgumentNullException("hostName");
			}
			bool flag3 = hostName == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'hostName' value must be specified.", "hostName");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.EhloHeloAsyncOP ehloHeloAsyncOP = new SMTP_Client.EhloHeloAsyncOP(hostName))
			{
				ehloHeloAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.EhloHeloAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag4 = !this.EhloHeloAsync(ehloHeloAsyncOP);
				if (flag4)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag5 = ehloHeloAsyncOP.Error != null;
				if (flag5)
				{
					throw ehloHeloAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x00051BA8 File Offset: 0x00050BA8
		public bool EhloHeloAsync(SMTP_Client.EhloHeloAsyncOP op)
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

		// Token: 0x06000D25 RID: 3365 RVA: 0x00051C2A File Offset: 0x00050C2A
		public void StartTLS()
		{
			this.StartTLS(null);
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x00051C38 File Offset: 0x00050C38
		public void StartTLS(RemoteCertificateValidationCallback certCallback)
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
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.StartTlsAsyncOP startTlsAsyncOP = new SMTP_Client.StartTlsAsyncOP(certCallback))
			{
				startTlsAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.StartTlsAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.StartTlsAsync(startTlsAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = startTlsAsyncOP.Error != null;
				if (flag3)
				{
					throw startTlsAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x00051D30 File Offset: 0x00050D30
		public bool StartTlsAsync(SMTP_Client.StartTlsAsyncOP op)
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

		// Token: 0x06000D28 RID: 3368 RVA: 0x00051DCC File Offset: 0x00050DCC
		public AUTH_SASL_Client AuthGetStrongestMethod(string userName, string password)
		{
			return this.AuthGetStrongestMethod(null, userName, password);
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00051DE8 File Offset: 0x00050DE8
		public AUTH_SASL_Client AuthGetStrongestMethod(string domain, string userName, string password)
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
			bool flag2 = userName == null;
			if (flag2)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag3 = password == null;
			if (flag3)
			{
				throw new ArgumentNullException("userName");
			}
			List<string> list = new List<string>(this.SaslAuthMethods);
			bool flag4 = list.Count == 0;
			if (flag4)
			{
				throw new NotSupportedException("SMTP server does not support authentication.");
			}
			bool flag5 = list.Contains("NTLM") && (!string.IsNullOrEmpty(domain) || userName.IndexOf('\\') > -1);
			AUTH_SASL_Client result;
			if (flag5)
			{
				bool flag6 = !string.IsNullOrEmpty(domain);
				if (flag6)
				{
					result = new AUTH_SASL_Client_Ntlm(domain, userName, password);
				}
				else
				{
					string[] array = userName.Split(new char[]
					{
						'\\'
					});
					result = new AUTH_SASL_Client_Ntlm(array[0], array[1], password);
				}
			}
			else
			{
				bool flag7 = list.Contains("DIGEST-MD5");
				if (flag7)
				{
					result = new AUTH_SASL_Client_DigestMd5("SMTP", this.RemoteEndPoint.Address.ToString(), userName, password);
				}
				else
				{
					bool flag8 = list.Contains("CRAM-MD5");
					if (flag8)
					{
						result = new AUTH_SASL_Client_CramMd5(userName, password);
					}
					else
					{
						bool flag9 = list.Contains("LOGIN");
						if (flag9)
						{
							result = new AUTH_SASL_Client_Login(userName, password);
						}
						else
						{
							bool flag10 = list.Contains("PLAIN");
							if (!flag10)
							{
								throw new NotSupportedException("We don't support any of the SMTP server authentication methods.");
							}
							result = new AUTH_SASL_Client_Plain(userName, password);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00051F88 File Offset: 0x00050F88
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.AuthAsyncOP authAsyncOP = new SMTP_Client.AuthAsyncOP(sasl))
			{
				authAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.AuthAsyncOP> e1)
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

		// Token: 0x06000D2B RID: 3371 RVA: 0x00052098 File Offset: 0x00051098
		public bool AuthAsync(SMTP_Client.AuthAsyncOP op)
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

		// Token: 0x06000D2C RID: 3372 RVA: 0x00052132 File Offset: 0x00051132
		public void MailFrom(string from, long messageSize)
		{
			this.MailFrom(from, messageSize, SMTP_DSN_Ret.NotSpecified, null);
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x00052140 File Offset: 0x00051140
		public void MailFrom(string from, long messageSize, SMTP_DSN_Ret ret, string envid)
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.MailFromAsyncOP mailFromAsyncOP = new SMTP_Client.MailFromAsyncOP(from, messageSize, ret, envid))
			{
				mailFromAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.MailFromAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.MailFromAsync(mailFromAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = mailFromAsyncOP.Error != null;
				if (flag3)
				{
					throw mailFromAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x00052220 File Offset: 0x00051220
		public bool MailFromAsync(SMTP_Client.MailFromAsyncOP op)
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

		// Token: 0x06000D2F RID: 3375 RVA: 0x000522A2 File Offset: 0x000512A2
		public void RcptTo(string to)
		{
			this.RcptTo(to, SMTP_DSN_Notify.NotSpecified, null);
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x000522B0 File Offset: 0x000512B0
		public void RcptTo(string to, SMTP_DSN_Notify notify, string orcpt)
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.RcptToAsyncOP rcptToAsyncOP = new SMTP_Client.RcptToAsyncOP(to, notify, orcpt))
			{
				rcptToAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.RcptToAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.RcptToAsync(rcptToAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = rcptToAsyncOP.Error != null;
				if (flag3)
				{
					throw rcptToAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00052390 File Offset: 0x00051390
		public bool RcptToAsync(SMTP_Client.RcptToAsyncOP op)
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

		// Token: 0x06000D32 RID: 3378 RVA: 0x00052412 File Offset: 0x00051412
		public void SendMessage(Stream stream)
		{
			this.SendMessage(stream, false);
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x00052420 File Offset: 0x00051420
		public void SendMessage(Stream stream, bool useBdatIfPossibe)
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.SendMessageAsyncOP sendMessageAsyncOP = new SMTP_Client.SendMessageAsyncOP(stream, useBdatIfPossibe))
			{
				sendMessageAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.SendMessageAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.SendMessageAsync(sendMessageAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = sendMessageAsyncOP.Error != null;
				if (flag3)
				{
					throw sendMessageAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x000524FC File Offset: 0x000514FC
		public bool SendMessageAsync(SMTP_Client.SendMessageAsyncOP op)
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

		// Token: 0x06000D35 RID: 3381 RVA: 0x00052580 File Offset: 0x00051580
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.RsetAsyncOP rsetAsyncOP = new SMTP_Client.RsetAsyncOP())
			{
				rsetAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.RsetAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.RsetAsync(rsetAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = rsetAsyncOP.Error != null;
				if (flag3)
				{
					throw rsetAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0005265C File Offset: 0x0005165C
		public bool RsetAsync(SMTP_Client.RsetAsyncOP op)
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

		// Token: 0x06000D37 RID: 3383 RVA: 0x000526E0 File Offset: 0x000516E0
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
			ManualResetEvent wait = new ManualResetEvent(false);
			using (SMTP_Client.NoopAsyncOP noopAsyncOP = new SMTP_Client.NoopAsyncOP())
			{
				noopAsyncOP.CompletedAsync += delegate(object s1, EventArgs<SMTP_Client.NoopAsyncOP> e1)
				{
					wait.Set();
				};
				bool flag2 = !this.NoopAsync(noopAsyncOP);
				if (flag2)
				{
					wait.Set();
				}
				wait.WaitOne();
				wait.Close();
				bool flag3 = noopAsyncOP.Error != null;
				if (flag3)
				{
					throw noopAsyncOP.Error;
				}
			}
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x000527BC File Offset: 0x000517BC
		public bool NoopAsync(SMTP_Client.NoopAsyncOP op)
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

		// Token: 0x06000D39 RID: 3385 RVA: 0x00052840 File Offset: 0x00051840
		protected override void OnConnected(TCP_Client.CompleteConnectCallback callback)
		{
			SMTP_Client.ReadResponseAsyncOP readGreetingOP = new SMTP_Client.ReadResponseAsyncOP();
			readGreetingOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
			{
				this.ReadServerGreetingCompleted(readGreetingOP, callback);
			};
			bool flag = !this.ReadResponseAsync(readGreetingOP);
			if (flag)
			{
				this.ReadServerGreetingCompleted(readGreetingOP, callback);
			}
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x000528B0 File Offset: 0x000518B0
		private void ReadServerGreetingCompleted(SMTP_Client.ReadResponseAsyncOP op, TCP_Client.CompleteConnectCallback connectCallback)
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
					bool flag2 = op.ReplyLines[0].ReplyCode == 220;
					if (flag2)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (SMTP_t_ReplyLine smtp_t_ReplyLine in op.ReplyLines)
						{
							stringBuilder.AppendLine(smtp_t_ReplyLine.Text);
						}
						this.m_GreetingText = stringBuilder.ToString();
						this.m_pEsmtpFeatures = new List<string>();
						this.m_pRecipients = new List<string>();
					}
					else
					{
						error = new SMTP_ClientException(op.ReplyLines);
					}
				}
			}
			catch (Exception ex)
			{
				error = ex;
			}
			connectCallback(error);
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x00052988 File Offset: 0x00051988
		private bool ReadResponseAsync(SMTP_Client.ReadResponseAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
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

		// Token: 0x06000D3C RID: 3388 RVA: 0x000529F0 File Offset: 0x000519F0
		private bool SupportsCapability(string capability)
		{
			bool flag = capability == null;
			if (flag)
			{
				throw new ArgumentNullException("capability");
			}
			bool flag2 = this.m_pEsmtpFeatures == null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (string a in this.m_pEsmtpFeatures)
				{
					bool flag3 = string.Equals(a, capability, StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x00052A84 File Offset: 0x00051A84
		[Obsolete("Use QuickSend(Mail_Message) instead")]
		public static void QuickSend(LumiSoft.Net.Mime.Mime message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			string from = "";
			bool flag2 = message.MainEntity.From != null && message.MainEntity.From.Count > 0;
			if (flag2)
			{
				from = ((MailboxAddress)message.MainEntity.From[0]).EmailAddress;
			}
			List<string> list = new List<string>();
			bool flag3 = message.MainEntity.To != null;
			if (flag3)
			{
				MailboxAddress[] mailboxes = message.MainEntity.To.Mailboxes;
				foreach (MailboxAddress mailboxAddress in mailboxes)
				{
					list.Add(mailboxAddress.EmailAddress);
				}
			}
			bool flag4 = message.MainEntity.Cc != null;
			if (flag4)
			{
				MailboxAddress[] mailboxes2 = message.MainEntity.Cc.Mailboxes;
				foreach (MailboxAddress mailboxAddress2 in mailboxes2)
				{
					list.Add(mailboxAddress2.EmailAddress);
				}
			}
			bool flag5 = message.MainEntity.Bcc != null;
			if (flag5)
			{
				MailboxAddress[] mailboxes3 = message.MainEntity.Bcc.Mailboxes;
				foreach (MailboxAddress mailboxAddress3 in mailboxes3)
				{
					list.Add(mailboxAddress3.EmailAddress);
				}
				message.MainEntity.Bcc.Clear();
			}
			foreach (string to in list)
			{
				SMTP_Client.QuickSend(null, from, to, new MemoryStream(message.ToByteData()));
			}
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00052C64 File Offset: 0x00051C64
		public static void QuickSend(Mail_Message message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			string from = "";
			bool flag2 = message.From != null && message.From.Count > 0;
			if (flag2)
			{
				from = message.From[0].Address;
			}
			List<string> list = new List<string>();
			bool flag3 = message.To != null;
			if (flag3)
			{
				Mail_t_Mailbox[] mailboxes = message.To.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox in mailboxes)
				{
					list.Add(mail_t_Mailbox.Address);
				}
			}
			bool flag4 = message.Cc != null;
			if (flag4)
			{
				Mail_t_Mailbox[] mailboxes2 = message.Cc.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox2 in mailboxes2)
				{
					list.Add(mail_t_Mailbox2.Address);
				}
			}
			bool flag5 = message.Bcc != null;
			if (flag5)
			{
				Mail_t_Mailbox[] mailboxes3 = message.Bcc.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox3 in mailboxes3)
				{
					list.Add(mail_t_Mailbox3.Address);
				}
				message.Bcc.Clear();
			}
			foreach (string to in list)
			{
				MemoryStream memoryStream = new MemoryStream();
				message.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
				memoryStream.Position = 0L;
				SMTP_Client.QuickSend(null, from, to, memoryStream);
			}
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x00052E2C File Offset: 0x00051E2C
		public static void QuickSend(string from, string to, Stream message)
		{
			SMTP_Client.QuickSend(null, from, to, message);
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x00052E3C File Offset: 0x00051E3C
		public static void QuickSend(string localHost, string from, string to, Stream message)
		{
			bool flag = from == null;
			if (flag)
			{
				throw new ArgumentNullException("from");
			}
			bool flag2 = from != "" && !SMTP_Utils.IsValidAddress(from);
			if (flag2)
			{
				throw new ArgumentException("Argument 'from' has invalid value.");
			}
			bool flag3 = to == null;
			if (flag3)
			{
				throw new ArgumentNullException("to");
			}
			bool flag4 = to == "";
			if (flag4)
			{
				throw new ArgumentException("Argument 'to' value must be specified.");
			}
			bool flag5 = !SMTP_Utils.IsValidAddress(to);
			if (flag5)
			{
				throw new ArgumentException("Argument 'to' has invalid value.");
			}
			bool flag6 = message == null;
			if (flag6)
			{
				throw new ArgumentNullException("message");
			}
			SMTP_Client.QuickSendSmartHost(localHost, Dns_Client.Static.GetEmailHosts(to)[0].HostName, 25, false, from, new string[]
			{
				to
			}, message);
		}

		// Token: 0x06000D41 RID: 3393 RVA: 0x00052F10 File Offset: 0x00051F10
		public static void QuickSendSmartHost(string host, int port, bool ssl, Mail_Message message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			SMTP_Client.QuickSendSmartHost(null, host, port, ssl, null, null, message);
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x00052F3F File Offset: 0x00051F3F
		public static void QuickSendSmartHost(string localHost, string host, int port, TcpClientSecurity security, string userName, string password, Mail_Message message)
		{
			SMTP_Client.QuickSendSmartHost(localHost, host, port, security == TcpClientSecurity.SSL, userName, password, message);
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x00052F58 File Offset: 0x00051F58
		public static void QuickSendSmartHost(string localHost, string host, int port, bool ssl, string userName, string password, Mail_Message message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			string from = "";
			bool flag2 = message.From != null && message.From.Count > 0;
			if (flag2)
			{
				from = message.From[0].Address;
			}
			List<string> list = new List<string>();
			bool flag3 = message.To != null;
			if (flag3)
			{
				Mail_t_Mailbox[] mailboxes = message.To.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox in mailboxes)
				{
					list.Add(mail_t_Mailbox.Address);
				}
			}
			bool flag4 = message.Cc != null;
			if (flag4)
			{
				Mail_t_Mailbox[] mailboxes2 = message.Cc.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox2 in mailboxes2)
				{
					list.Add(mail_t_Mailbox2.Address);
				}
			}
			bool flag5 = message.Bcc != null;
			if (flag5)
			{
				Mail_t_Mailbox[] mailboxes3 = message.Bcc.Mailboxes;
				foreach (Mail_t_Mailbox mail_t_Mailbox3 in mailboxes3)
				{
					list.Add(mail_t_Mailbox3.Address);
				}
				message.Bcc.Clear();
			}
			MemoryStream memoryStream = new MemoryStream();
			message.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
			memoryStream.Position = 0L;
			SMTP_Client.QuickSendSmartHost(localHost, host, port, ssl, userName, password, from, list.ToArray(), memoryStream);
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x000530F8 File Offset: 0x000520F8
		public static void QuickSendSmartHost(string host, int port, string from, string[] to, Stream message)
		{
			SMTP_Client.QuickSendSmartHost(null, host, port, false, null, null, from, to, message);
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00053118 File Offset: 0x00052118
		public static void QuickSendSmartHost(string host, int port, bool ssl, string from, string[] to, Stream message)
		{
			SMTP_Client.QuickSendSmartHost(null, host, port, ssl, null, null, from, to, message);
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00053138 File Offset: 0x00052138
		public static void QuickSendSmartHost(string localHost, string host, int port, bool ssl, string from, string[] to, Stream message)
		{
			SMTP_Client.QuickSendSmartHost(localHost, host, port, ssl, null, null, from, to, message);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00053158 File Offset: 0x00052158
		public static void QuickSendSmartHost(string localHost, string host, int port, bool ssl, string userName, string password, string from, string[] to, Stream message)
		{
			SMTP_Client.QuickSendSmartHost(localHost, host, port, ssl ? TcpClientSecurity.SSL : TcpClientSecurity.None, userName, password, from, to, message);
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x00053180 File Offset: 0x00052180
		public static void QuickSendSmartHost(string localHost, string host, int port, TcpClientSecurity security, string userName, string password, string from, string[] to, Stream message)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			bool flag2 = host == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'host' value may not be empty.");
			}
			bool flag3 = port < 1;
			if (flag3)
			{
				throw new ArgumentException("Argument 'port' value must be >= 1.");
			}
			bool flag4 = from == null;
			if (flag4)
			{
				throw new ArgumentNullException("from");
			}
			bool flag5 = from != "" && !SMTP_Utils.IsValidAddress(from);
			if (flag5)
			{
				throw new ArgumentException("Argument 'from' has invalid value.");
			}
			bool flag6 = to == null;
			if (flag6)
			{
				throw new ArgumentNullException("to");
			}
			bool flag7 = to.Length == 0;
			if (flag7)
			{
				throw new ArgumentException("Argument 'to' must contain at least 1 recipient.");
			}
			foreach (string text in to)
			{
				bool flag8 = !SMTP_Utils.IsValidAddress(text);
				if (flag8)
				{
					throw new ArgumentException("Argument 'to' has invalid value '" + text + "'.");
				}
			}
			bool flag9 = message == null;
			if (flag9)
			{
				throw new ArgumentNullException("message");
			}
			using (SMTP_Client smtp_Client = new SMTP_Client())
			{
				smtp_Client.Connect(host, port, security == TcpClientSecurity.SSL);
				bool flag10 = security == TcpClientSecurity.TLS || (security == TcpClientSecurity.UseTlsIfSupported && smtp_Client.SupportsCapability(SMTP_ServiceExtensions.STARTTLS));
				if (flag10)
				{
					smtp_Client.StartTLS();
				}
				smtp_Client.EhloHelo((localHost != null) ? localHost : Dns.GetHostName());
				bool flag11 = !string.IsNullOrEmpty(userName);
				if (flag11)
				{
					smtp_Client.Auth(smtp_Client.AuthGetStrongestMethod(userName, password));
				}
				smtp_Client.MailFrom(from, -1L);
				foreach (string to2 in to)
				{
					smtp_Client.RcptTo(to2);
				}
				smtp_Client.SendMessage(message);
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06000D49 RID: 3401 RVA: 0x0005337C File Offset: 0x0005237C
		// (set) Token: 0x06000D4A RID: 3402 RVA: 0x000533B0 File Offset: 0x000523B0
		public string LocalHostName
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LocalHostName;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool isConnected = this.IsConnected;
				if (isConnected)
				{
					throw new InvalidOperationException("Property LocalHostName is available only when SMTP client is not connected.");
				}
				this.m_LocalHostName = value;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06000D4B RID: 3403 RVA: 0x000533F8 File Offset: 0x000523F8
		public string RemoteHostName
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
				return this.m_RemoteHostName;
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x00053448 File Offset: 0x00052448
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

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06000D4D RID: 3405 RVA: 0x00053498 File Offset: 0x00052498
		public bool IsEsmtpSupported
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
				return this.m_IsEsmtpSupported;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06000D4E RID: 3406 RVA: 0x000534E8 File Offset: 0x000524E8
		public string[] EsmtpFeatures
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
				return this.m_pEsmtpFeatures.ToArray();
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06000D4F RID: 3407 RVA: 0x0005353C File Offset: 0x0005253C
		public string[] SaslAuthMethods
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
				foreach (string text in this.EsmtpFeatures)
				{
					string a = text.Split(new char[]
					{
						' '
					})[0];
					bool flag2 = string.Equals(a, SMTP_ServiceExtensions.AUTH, StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						return text.Substring(4).Trim().Split(new char[]
						{
							' '
						});
					}
				}
				return new string[0];
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x000535F4 File Offset: 0x000525F4
		public long MaxAllowedMessageSize
		{
			get
			{
				try
				{
					foreach (string text in this.EsmtpFeatures)
					{
						bool flag = text.ToUpper().StartsWith(SMTP_ServiceExtensions.SIZE);
						if (flag)
						{
							return Convert.ToInt64(text.Split(new char[]
							{
								' '
							})[1]);
						}
					}
				}
				catch
				{
				}
				return 0L;
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x00053670 File Offset: 0x00052670
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

		// Token: 0x06000D52 RID: 3410 RVA: 0x000536C0 File Offset: 0x000526C0
		[Obsolete("Use method 'Auth' instead.")]
		public void Authenticate(string userName, string password)
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
			bool flag2 = string.IsNullOrEmpty(userName);
			if (flag2)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag3 = password == null;
			if (flag3)
			{
				password = "";
			}
			string a = "LOGIN";
			List<string> list = new List<string>(this.SaslAuthMethods);
			bool flag4 = list.Contains("DIGEST-MD5");
			if (flag4)
			{
				a = "DIGEST-MD5";
			}
			else
			{
				bool flag5 = list.Contains("CRAM-MD5");
				if (flag5)
				{
					a = "CRAM-MD5";
				}
			}
			bool flag6 = a == "LOGIN";
			if (flag6)
			{
				base.WriteLine("AUTH LOGIN");
				string text = base.ReadLine();
				bool flag7 = !text.StartsWith("334");
				if (flag7)
				{
					throw new SMTP_ClientException(text);
				}
				base.WriteLine(Convert.ToBase64String(Encoding.ASCII.GetBytes(userName)));
				text = base.ReadLine();
				bool flag8 = !text.StartsWith("334");
				if (flag8)
				{
					throw new SMTP_ClientException(text);
				}
				base.WriteLine(Convert.ToBase64String(Encoding.ASCII.GetBytes(password)));
				text = base.ReadLine();
				bool flag9 = !text.StartsWith("235");
				if (flag9)
				{
					throw new SMTP_ClientException(text);
				}
				this.m_pAuthdUserIdentity = new GenericIdentity(userName, "LOGIN");
			}
			else
			{
				bool flag10 = a == "CRAM-MD5";
				if (flag10)
				{
					base.WriteLine("AUTH CRAM-MD5");
					string text2 = base.ReadLine();
					bool flag11 = !text2.StartsWith("334");
					if (flag11)
					{
						throw new SMTP_ClientException(text2);
					}
					HMACMD5 hmacmd = new HMACMD5(Encoding.ASCII.GetBytes(password));
					string str = Net_Utils.ToHex(hmacmd.ComputeHash(Convert.FromBase64String(text2.Split(new char[]
					{
						' '
					})[1]))).ToLower();
					base.WriteLine(Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + " " + str)));
					text2 = base.ReadLine();
					bool flag12 = !text2.StartsWith("235");
					if (flag12)
					{
						throw new SMTP_ClientException(text2);
					}
					this.m_pAuthdUserIdentity = new GenericIdentity(userName, "CRAM-MD5");
				}
				else
				{
					bool flag13 = a == "DIGEST-MD5";
					if (flag13)
					{
						base.WriteLine("AUTH DIGEST-MD5");
						string text3 = base.ReadLine();
						bool flag14 = !text3.StartsWith("334");
						if (flag14)
						{
							throw new SMTP_ClientException(text3);
						}
						AUTH_SASL_DigestMD5_Challenge auth_SASL_DigestMD5_Challenge = AUTH_SASL_DigestMD5_Challenge.Parse(Encoding.Default.GetString(Convert.FromBase64String(text3.Split(new char[]
						{
							' '
						})[1])));
						AUTH_SASL_DigestMD5_Response auth_SASL_DigestMD5_Response = new AUTH_SASL_DigestMD5_Response(auth_SASL_DigestMD5_Challenge, auth_SASL_DigestMD5_Challenge.Realm[0], userName, password, Guid.NewGuid().ToString().Replace("-", ""), 1, auth_SASL_DigestMD5_Challenge.QopOptions[0], "smtp/" + this.RemoteEndPoint.Address.ToString());
						base.WriteLine(Convert.ToBase64String(Encoding.Default.GetBytes(auth_SASL_DigestMD5_Response.ToResponse())));
						text3 = base.ReadLine();
						bool flag15 = !text3.StartsWith("334");
						if (flag15)
						{
							throw new SMTP_ClientException(text3);
						}
						bool flag16 = !string.Equals(Encoding.Default.GetString(Convert.FromBase64String(text3.Split(new char[]
						{
							' '
						})[1])), auth_SASL_DigestMD5_Response.ToRspauthResponse(userName, password), StringComparison.InvariantCultureIgnoreCase);
						if (flag16)
						{
							throw new Exception("SMTP server 'rspauth' value mismatch.");
						}
						base.WriteLine("");
						text3 = base.ReadLine();
						bool flag17 = !text3.StartsWith("235");
						if (flag17)
						{
							throw new SMTP_ClientException(text3);
						}
						this.m_pAuthdUserIdentity = new GenericIdentity(userName, "DIGEST-MD5");
					}
				}
			}
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x00053AE4 File Offset: 0x00052AE4
		[Obsolete("Use method 'AuthAsync' instead.")]
		public IAsyncResult BeginAuthenticate(string userName, string password, AsyncCallback callback, object state)
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
			SMTP_Client.AuthenticateDelegate authenticateDelegate = new SMTP_Client.AuthenticateDelegate(this.Authenticate);
			AsyncResultState asyncResultState = new AsyncResultState(this, authenticateDelegate, callback, state);
			asyncResultState.SetAsyncResult(authenticateDelegate.BeginInvoke(userName, password, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x00053B7C File Offset: 0x00052B7C
		[Obsolete("Use method 'AuthAsync' instead.")]
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
				throw new ArgumentException("Argument 'asyncResult' was not returned by a call to the BeginAuthenticate method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginAuthenticate was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.AuthenticateDelegate;
			if (flag3)
			{
				((SMTP_Client.AuthenticateDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginAuthenticate method.");
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x00053C40 File Offset: 0x00052C40
		[Obsolete("Use method 'NoopAsync' instead.")]
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
			SMTP_Client.NoopDelegate noopDelegate = new SMTP_Client.NoopDelegate(this.Noop);
			AsyncResultState asyncResultState = new AsyncResultState(this, noopDelegate, callback, state);
			asyncResultState.SetAsyncResult(noopDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x00053CBC File Offset: 0x00052CBC
		[Obsolete("Use method 'NoopAsync' instead.")]
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
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.NoopDelegate;
			if (flag3)
			{
				((SMTP_Client.NoopDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginNoop method.");
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x00053D80 File Offset: 0x00052D80
		[Obsolete("Use method StartTlsAsync instead.")]
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
			bool isSecureConnection = this.IsSecureConnection;
			if (isSecureConnection)
			{
				throw new InvalidOperationException("Connection is already secure.");
			}
			SMTP_Client.StartTLSDelegate startTLSDelegate = new SMTP_Client.StartTLSDelegate(this.StartTLS);
			AsyncResultState asyncResultState = new AsyncResultState(this, startTLSDelegate, callback, state);
			asyncResultState.SetAsyncResult(startTLSDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x00053E14 File Offset: 0x00052E14
		[Obsolete("Use method StartTlsAsync instead.")]
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
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.StartTLSDelegate;
			if (flag3)
			{
				((SMTP_Client.StartTLSDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x00053ED8 File Offset: 0x00052ED8
		[Obsolete("Use method RcptToAsync instead.")]
		public IAsyncResult BeginRcptTo(string to, AsyncCallback callback, object state)
		{
			return this.BeginRcptTo(to, SMTP_DSN_Notify.NotSpecified, null, callback, state);
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00053EF8 File Offset: 0x00052EF8
		[Obsolete("Use method RcptToAsync instead.")]
		public IAsyncResult BeginRcptTo(string to, SMTP_DSN_Notify notify, string orcpt, AsyncCallback callback, object state)
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
			SMTP_Client.RcptToDelegate rcptToDelegate = new SMTP_Client.RcptToDelegate(this.RcptTo);
			AsyncResultState asyncResultState = new AsyncResultState(this, rcptToDelegate, callback, state);
			asyncResultState.SetAsyncResult(rcptToDelegate.BeginInvoke(to, notify, orcpt, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00053F78 File Offset: 0x00052F78
		[Obsolete("Use method RcptToAsync instead.")]
		public void EndRcptTo(IAsyncResult asyncResult)
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
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.RcptToDelegate;
			if (flag3)
			{
				((SMTP_Client.RcptToDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0005403C File Offset: 0x0005303C
		[Obsolete("Use method MailFromAsync instead.")]
		public IAsyncResult BeginMailFrom(string from, long messageSize, AsyncCallback callback, object state)
		{
			return this.BeginMailFrom(from, messageSize, SMTP_DSN_Ret.NotSpecified, null, callback, state);
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0005405C File Offset: 0x0005305C
		[Obsolete("Use method MailFromAsync instead.")]
		public IAsyncResult BeginMailFrom(string from, long messageSize, SMTP_DSN_Ret ret, string envid, AsyncCallback callback, object state)
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
			SMTP_Client.MailFromDelegate mailFromDelegate = new SMTP_Client.MailFromDelegate(this.MailFrom);
			AsyncResultState asyncResultState = new AsyncResultState(this, mailFromDelegate, callback, state);
			asyncResultState.SetAsyncResult(mailFromDelegate.BeginInvoke(from, (long)((int)messageSize), ret, envid, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x000540E0 File Offset: 0x000530E0
		[Obsolete("Use method MailFromAsync instead.")]
		public void EndMailFrom(IAsyncResult asyncResult)
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
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.MailFromDelegate;
			if (flag3)
			{
				((SMTP_Client.MailFromDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x000541A4 File Offset: 0x000531A4
		[Obsolete("Use 'RsetAsync' method instead.")]
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
			SMTP_Client.ResetDelegate resetDelegate = new SMTP_Client.ResetDelegate(this.Reset);
			AsyncResultState asyncResultState = new AsyncResultState(this, resetDelegate, callback, state);
			asyncResultState.SetAsyncResult(resetDelegate.BeginInvoke(new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00054220 File Offset: 0x00053220
		[Obsolete("Use 'RsetAsync' method instead.")]
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
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.ResetDelegate;
			if (flag3)
			{
				((SMTP_Client.ResetDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginReset method.");
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x000542E4 File Offset: 0x000532E4
		[Obsolete("Use Rset method instead.")]
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
			base.WriteLine("RSET");
			string text = base.ReadLine();
			bool flag2 = !text.StartsWith("250");
			if (flag2)
			{
				throw new SMTP_ClientException(text);
			}
			this.m_MailFrom = null;
			this.m_pRecipients.Clear();
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x00054368 File Offset: 0x00053368
		[Obsolete("Use method 'SendMessageAsync' instead.")]
		public IAsyncResult BeginSendMessage(Stream message, AsyncCallback callback, object state)
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
			bool flag2 = message == null;
			if (flag2)
			{
				throw new ArgumentNullException("message");
			}
			SMTP_Client.SendMessageDelegate sendMessageDelegate = new SMTP_Client.SendMessageDelegate(this.SendMessage);
			AsyncResultState asyncResultState = new AsyncResultState(this, sendMessageDelegate, callback, state);
			asyncResultState.SetAsyncResult(sendMessageDelegate.BeginInvoke(message, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x000543FC File Offset: 0x000533FC
		[Obsolete("Use method 'SendMessageAsync' instead.")]
		public void EndSendMessage(IAsyncResult asyncResult)
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
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginSendMessage method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginSendMessage was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.SendMessageDelegate;
			if (flag3)
			{
				((SMTP_Client.SendMessageDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
				return;
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginSendMessage method.");
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x000544C0 File Offset: 0x000534C0
		[Obsolete("Use method Dns_Client.GetEmailHostsAsync instead.")]
		public static IAsyncResult BeginGetDomainHosts(string domain, AsyncCallback callback, object state)
		{
			bool flag = domain == null;
			if (flag)
			{
				throw new ArgumentNullException("domain");
			}
			bool flag2 = string.IsNullOrEmpty(domain);
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'domain' value, you need to specify domain value.");
			}
			SMTP_Client.GetDomainHostsDelegate getDomainHostsDelegate = new SMTP_Client.GetDomainHostsDelegate(SMTP_Client.GetDomainHosts);
			AsyncResultState asyncResultState = new AsyncResultState(null, getDomainHostsDelegate, callback, state);
			asyncResultState.SetAsyncResult(getDomainHostsDelegate.BeginInvoke(domain, new AsyncCallback(asyncResultState.CompletedCallback), null));
			return asyncResultState;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x00054534 File Offset: 0x00053534
		[Obsolete("Use method Dns_Client.GetEmailHostsAsync instead.")]
		public static string[] EndGetDomainHosts(IAsyncResult asyncResult)
		{
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResultState asyncResultState = asyncResult as AsyncResultState;
			bool flag2 = asyncResultState == null;
			if (flag2)
			{
				throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginGetDomainHosts method.");
			}
			bool isEndCalled = asyncResultState.IsEndCalled;
			if (isEndCalled)
			{
				throw new InvalidOperationException("BeginGetDomainHosts was previously called for the asynchronous connection.");
			}
			asyncResultState.IsEndCalled = true;
			bool flag3 = asyncResultState.AsyncDelegate is SMTP_Client.GetDomainHostsDelegate;
			if (flag3)
			{
				return ((SMTP_Client.GetDomainHostsDelegate)asyncResultState.AsyncDelegate).EndInvoke(asyncResultState.AsyncResult);
			}
			throw new ArgumentException("Argument asyncResult was not returned by a call to the BeginGetDomainHosts method.");
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000545CC File Offset: 0x000535CC
		[Obsolete("Use method Dns_Client.GetEmailHosts instead.")]
		public static string[] GetDomainHosts(string domain)
		{
			bool flag = domain == null;
			if (flag)
			{
				throw new ArgumentNullException("domain");
			}
			bool flag2 = string.IsNullOrEmpty(domain);
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'domain' value, you need to specify domain value.");
			}
			bool flag3 = domain.IndexOf("@") > -1;
			if (flag3)
			{
				domain = domain.Substring(domain.IndexOf('@') + 1);
			}
			List<string> list = new List<string>();
			using (Dns_Client dns_Client = new Dns_Client())
			{
				DnsServerResponse dnsServerResponse = dns_Client.Query(domain, DNS_QType.MX);
				bool flag4 = dnsServerResponse.ResponseCode == DNS_RCode.NO_ERROR;
				if (!flag4)
				{
					throw new DNS_ClientException(dnsServerResponse.ResponseCode);
				}
				foreach (DNS_rr_MX dns_rr_MX in dnsServerResponse.GetMXRecords())
				{
					bool flag5 = !string.IsNullOrEmpty(dns_rr_MX.Host);
					if (flag5)
					{
						list.Add(dns_rr_MX.Host);
					}
				}
			}
			bool flag6 = list.Count == 0;
			if (flag6)
			{
				list.Add(domain);
			}
			return list.ToArray();
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06000D67 RID: 3431 RVA: 0x000546F8 File Offset: 0x000536F8
		// (set) Token: 0x06000D68 RID: 3432 RVA: 0x00054710 File Offset: 0x00053710
		[Obsolete("Use method SendMessage argument 'useBdatIfPossibe' instead.")]
		public bool BdatEnabled
		{
			get
			{
				return this.m_BdatEnabled;
			}
			set
			{
				this.m_BdatEnabled = value;
			}
		}

		// Token: 0x04000593 RID: 1427
		private string m_LocalHostName = null;

		// Token: 0x04000594 RID: 1428
		private string m_RemoteHostName = null;

		// Token: 0x04000595 RID: 1429
		private string m_GreetingText = "";

		// Token: 0x04000596 RID: 1430
		private bool m_IsEsmtpSupported = false;

		// Token: 0x04000597 RID: 1431
		private List<string> m_pEsmtpFeatures = null;

		// Token: 0x04000598 RID: 1432
		private string m_MailFrom = null;

		// Token: 0x04000599 RID: 1433
		private List<string> m_pRecipients = null;

		// Token: 0x0400059A RID: 1434
		private GenericIdentity m_pAuthdUserIdentity = null;

		// Token: 0x0400059B RID: 1435
		private bool m_BdatEnabled = true;

		// Token: 0x020002E8 RID: 744
		public class EhloHeloAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001927 RID: 6439 RVA: 0x0009AFD0 File Offset: 0x00099FD0
			public EhloHeloAsyncOP(string hostName)
			{
				bool flag = hostName == null;
				if (flag)
				{
					throw new ArgumentNullException("hostName");
				}
				bool flag2 = hostName == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Argument 'hostName' value must be specified.", "hostName");
				}
				this.m_HostName = hostName;
			}

			// Token: 0x06001928 RID: 6440 RVA: 0x0009B05C File Offset: 0x0009A05C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_HostName = null;
					this.m_pSmtpClient = null;
					this.m_pReplyLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001929 RID: 6441 RVA: 0x0009B0A8 File Offset: 0x0009A0A8
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("EHLO " + this.m_HostName + "\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "EHLO " + this.m_HostName);
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.EhloCommandSendingCompleted), null);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
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

			// Token: 0x0600192A RID: 6442 RVA: 0x0009B1E0 File Offset: 0x0009A1E0
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

			// Token: 0x0600192B RID: 6443 RVA: 0x0009B258 File Offset: 0x0009A258
			private void EhloCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.EhloReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.EhloReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600192C RID: 6444 RVA: 0x0009B330 File Offset: 0x0009A330
			private void EhloReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pReplyLines = op.ReplyLines;
						bool flag3 = this.m_pReplyLines[0].ReplyCode == 250;
						if (flag3)
						{
							this.m_pSmtpClient.m_RemoteHostName = this.m_pReplyLines[0].Text.Split(new char[]
							{
								' '
							}, 2)[0];
							this.m_pSmtpClient.m_IsEsmtpSupported = true;
							List<string> list = new List<string>();
							foreach (SMTP_t_ReplyLine smtp_t_ReplyLine in this.m_pReplyLines)
							{
								list.Add(smtp_t_ReplyLine.Text);
							}
							this.m_pSmtpClient.m_pEsmtpFeatures = list;
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pSmtpClient.LogAddText("EHLO failed, will try HELO.");
							byte[] bytes = Encoding.UTF8.GetBytes("HELO " + this.m_HostName + "\r\n");
							this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "HELO " + this.m_HostName);
							this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.HeloCommandSendingCompleted), null);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x0600192D RID: 6445 RVA: 0x0009B544 File Offset: 0x0009A544
			private void HeloCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.HeloReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.HeloReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600192E RID: 6446 RVA: 0x0009B61C File Offset: 0x0009A61C
			private void HeloReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pReplyLines = op.ReplyLines;
						bool flag3 = this.m_pReplyLines[0].ReplyCode == 250;
						if (flag3)
						{
							this.m_pSmtpClient.m_RemoteHostName = this.m_pReplyLines[0].Text.Split(new char[]
							{
								' '
							}, 2)[0];
							this.m_pSmtpClient.m_IsEsmtpSupported = true;
							List<string> list = new List<string>();
							foreach (SMTP_t_ReplyLine smtp_t_ReplyLine in this.m_pReplyLines)
							{
								list.Add(smtp_t_ReplyLine.Text);
							}
							this.m_pSmtpClient.m_pEsmtpFeatures = list;
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000829 RID: 2089
			// (get) Token: 0x0600192F RID: 6447 RVA: 0x0009B7E0 File Offset: 0x0009A7E0
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700082A RID: 2090
			// (get) Token: 0x06001930 RID: 6448 RVA: 0x0009B7F8 File Offset: 0x0009A7F8
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

			// Token: 0x1700082B RID: 2091
			// (get) Token: 0x06001931 RID: 6449 RVA: 0x0009B84C File Offset: 0x0009A84C
			public SMTP_t_ReplyLine[] ReplyLines
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
						throw new InvalidOperationException("Property 'ReplyLines' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pReplyLines;
				}
			}

			// Token: 0x140000B8 RID: 184
			// (add) Token: 0x06001932 RID: 6450 RVA: 0x0009B8B4 File Offset: 0x0009A8B4
			// (remove) Token: 0x06001933 RID: 6451 RVA: 0x0009B8EC File Offset: 0x0009A8EC
			
			public event EventHandler<EventArgs<SMTP_Client.EhloHeloAsyncOP>> CompletedAsync = null;

			// Token: 0x06001934 RID: 6452 RVA: 0x0009B924 File Offset: 0x0009A924
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.EhloHeloAsyncOP>(this));
				}
			}

			// Token: 0x04000B02 RID: 2818
			private object m_pLock = new object();

			// Token: 0x04000B03 RID: 2819
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B04 RID: 2820
			private Exception m_pException = null;

			// Token: 0x04000B05 RID: 2821
			private string m_HostName = null;

			// Token: 0x04000B06 RID: 2822
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B07 RID: 2823
			private SMTP_t_ReplyLine[] m_pReplyLines = null;

			// Token: 0x04000B08 RID: 2824
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002E9 RID: 745
		public class StartTlsAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001935 RID: 6453 RVA: 0x0009B954 File Offset: 0x0009A954
			public StartTlsAsyncOP(RemoteCertificateValidationCallback certCallback)
			{
				this.m_pCertCallback = certCallback;
			}

			// Token: 0x06001936 RID: 6454 RVA: 0x0009B9A8 File Offset: 0x0009A9A8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pCertCallback = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001937 RID: 6455 RVA: 0x0009B9EC File Offset: 0x0009A9EC
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("STARTTLS\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "STARTTLS");
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.StartTlsCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x06001938 RID: 6456 RVA: 0x0009BB00 File Offset: 0x0009AB00
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

			// Token: 0x06001939 RID: 6457 RVA: 0x0009BB78 File Offset: 0x0009AB78
			private void StartTlsCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.StartTlsReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.StartTlsReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600193A RID: 6458 RVA: 0x0009BC44 File Offset: 0x0009AC44
			private void StartTlsReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 220;
						if (flag3)
						{
							this.m_pSmtpClient.LogAddText("Starting TLS handshake.");
							TCP_Client.SwitchToSecureAsyncOP switchSecureOP = new TCP_Client.SwitchToSecureAsyncOP(this.m_pCertCallback);
							switchSecureOP.CompletedAsync += delegate(object s, EventArgs<TCP_Client.SwitchToSecureAsyncOP> e)
							{
								this.SwitchToSecureCompleted(switchSecureOP);
							};
							bool flag4 = !this.m_pSmtpClient.SwitchToSecureAsync(switchSecureOP);
							if (flag4)
							{
								this.SwitchToSecureCompleted(switchSecureOP);
							}
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				op.Dispose();
				bool flag5 = this.m_pException != null;
				if (flag5)
				{
					bool flag6 = this.m_pSmtpClient != null;
					if (flag6)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600193B RID: 6459 RVA: 0x0009BDA4 File Offset: 0x0009ADA4
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						this.m_pSmtpClient.LogAddText("TLS handshake completed sucessfully.");
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag3 = this.m_pSmtpClient != null;
					if (flag3)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x1700082C RID: 2092
			// (get) Token: 0x0600193C RID: 6460 RVA: 0x0009BE84 File Offset: 0x0009AE84
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700082D RID: 2093
			// (get) Token: 0x0600193D RID: 6461 RVA: 0x0009BE9C File Offset: 0x0009AE9C
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

			// Token: 0x140000B9 RID: 185
			// (add) Token: 0x0600193E RID: 6462 RVA: 0x0009BEF0 File Offset: 0x0009AEF0
			// (remove) Token: 0x0600193F RID: 6463 RVA: 0x0009BF28 File Offset: 0x0009AF28
			
			public event EventHandler<EventArgs<SMTP_Client.StartTlsAsyncOP>> CompletedAsync = null;

			// Token: 0x06001940 RID: 6464 RVA: 0x0009BF60 File Offset: 0x0009AF60
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.StartTlsAsyncOP>(this));
				}
			}

			// Token: 0x04000B0A RID: 2826
			private object m_pLock = new object();

			// Token: 0x04000B0B RID: 2827
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B0C RID: 2828
			private Exception m_pException = null;

			// Token: 0x04000B0D RID: 2829
			private RemoteCertificateValidationCallback m_pCertCallback = null;

			// Token: 0x04000B0E RID: 2830
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B0F RID: 2831
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002EA RID: 746
		public class AuthAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001941 RID: 6465 RVA: 0x0009BF90 File Offset: 0x0009AF90
			public AuthAsyncOP(AUTH_SASL_Client sasl)
			{
				bool flag = sasl == null;
				if (flag)
				{
					throw new ArgumentNullException("sasl");
				}
				this.m_pSASL = sasl;
			}

			// Token: 0x06001942 RID: 6466 RVA: 0x0009BFF8 File Offset: 0x0009AFF8
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001943 RID: 6467 RVA: 0x0009C034 File Offset: 0x0009B034
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
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
						this.m_pSmtpClient.LogAddWrite((long)bytes.Length, Encoding.UTF8.GetString(bytes).TrimEnd(new char[0]));
						this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
					}
					else
					{
						byte[] bytes2 = Encoding.UTF8.GetBytes("AUTH " + this.m_pSASL.Name + "\r\n");
						this.m_pSmtpClient.LogAddWrite((long)bytes2.Length, "AUTH " + this.m_pSASL.Name);
						this.m_pSmtpClient.TcpStream.BeginWrite(bytes2, 0, bytes2.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x06001944 RID: 6468 RVA: 0x0009C23C File Offset: 0x0009B23C
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

			// Token: 0x06001945 RID: 6469 RVA: 0x0009C2B4 File Offset: 0x0009B2B4
			private void AuthCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.AuthReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.AuthReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001946 RID: 6470 RVA: 0x0009C380 File Offset: 0x0009B380
			private void AuthReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
			{
				try
				{
					bool flag = op.ReplyLines[0].ReplyCode == 334;
					if (flag)
					{
						byte[] serverResponse = Convert.FromBase64String(op.ReplyLines[0].Text);
						byte[] inArray = this.m_pSASL.Continue(serverResponse);
						byte[] bytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(inArray) + "\r\n");
						this.m_pSmtpClient.LogAddWrite((long)bytes.Length, Convert.ToBase64String(inArray));
						this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.AuthCommandSendingCompleted), null);
					}
					else
					{
						bool flag2 = op.ReplyLines[0].ReplyCode == 235;
						if (flag2)
						{
							this.m_pSmtpClient.m_pAuthdUserIdentity = new GenericIdentity(this.m_pSASL.UserName, this.m_pSASL.Name);
							this.SetState(AsyncOP_State.Completed);
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag3 = this.m_pSmtpClient != null;
					if (flag3)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x1700082E RID: 2094
			// (get) Token: 0x06001947 RID: 6471 RVA: 0x0009C4E0 File Offset: 0x0009B4E0
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700082F RID: 2095
			// (get) Token: 0x06001948 RID: 6472 RVA: 0x0009C4F8 File Offset: 0x0009B4F8
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

			// Token: 0x140000BA RID: 186
			// (add) Token: 0x06001949 RID: 6473 RVA: 0x0009C54C File Offset: 0x0009B54C
			// (remove) Token: 0x0600194A RID: 6474 RVA: 0x0009C584 File Offset: 0x0009B584
			
			public event EventHandler<EventArgs<SMTP_Client.AuthAsyncOP>> CompletedAsync = null;

			// Token: 0x0600194B RID: 6475 RVA: 0x0009C5BC File Offset: 0x0009B5BC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.AuthAsyncOP>(this));
				}
			}

			// Token: 0x04000B11 RID: 2833
			private object m_pLock = new object();

			// Token: 0x04000B12 RID: 2834
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B13 RID: 2835
			private Exception m_pException = null;

			// Token: 0x04000B14 RID: 2836
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B15 RID: 2837
			private AUTH_SASL_Client m_pSASL = null;

			// Token: 0x04000B16 RID: 2838
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002EB RID: 747
		public class MailFromAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600194C RID: 6476 RVA: 0x0009C5EC File Offset: 0x0009B5EC
			public MailFromAsyncOP(string from, long messageSize) : this(from, messageSize, SMTP_DSN_Ret.NotSpecified, null)
			{
			}

			// Token: 0x0600194D RID: 6477 RVA: 0x0009C5FC File Offset: 0x0009B5FC
			public MailFromAsyncOP(string from, long messageSize, SMTP_DSN_Ret ret, string envid)
			{
				this.m_MailFrom = from;
				this.m_MessageSize = messageSize;
				this.m_DsnRet = ret;
				this.m_EnvID = envid;
			}

			// Token: 0x0600194E RID: 6478 RVA: 0x0009C67C File Offset: 0x0009B67C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_MailFrom = null;
					this.m_EnvID = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x0600194F RID: 6479 RVA: 0x0009C6C8 File Offset: 0x0009B6C8
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool flag2 = false;
					foreach (string text in this.m_pSmtpClient.EsmtpFeatures)
					{
						bool flag3 = text.ToLower().StartsWith("size ");
						if (flag3)
						{
							flag2 = true;
							break;
						}
					}
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("MAIL FROM:<" + this.m_MailFrom + ">");
					bool flag4 = flag2 && this.m_MessageSize > 0L;
					if (flag4)
					{
						stringBuilder.Append(" SIZE=" + this.m_MessageSize.ToString());
					}
					bool flag5 = this.m_DsnRet == SMTP_DSN_Ret.FullMessage;
					if (flag5)
					{
						stringBuilder.Append(" RET=FULL");
					}
					else
					{
						bool flag6 = this.m_DsnRet == SMTP_DSN_Ret.Headers;
						if (flag6)
						{
							stringBuilder.Append(" RET=HDRS");
						}
					}
					bool flag7 = !string.IsNullOrEmpty(this.m_EnvID);
					if (flag7)
					{
						stringBuilder.Append(" ENVID=" + this.m_EnvID);
					}
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString() + "\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, stringBuilder.ToString());
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.MailCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag8 = this.m_pSmtpClient != null;
					if (flag8)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x06001950 RID: 6480 RVA: 0x0009C90C File Offset: 0x0009B90C
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

			// Token: 0x06001951 RID: 6481 RVA: 0x0009C984 File Offset: 0x0009B984
			private void MailCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.MailReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.MailReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001952 RID: 6482 RVA: 0x0009CA50 File Offset: 0x0009BA50
			private void MailReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 250;
						if (flag3)
						{
							this.m_pSmtpClient.m_MailFrom = this.m_MailFrom;
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000830 RID: 2096
			// (get) Token: 0x06001953 RID: 6483 RVA: 0x0009CB88 File Offset: 0x0009BB88
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000831 RID: 2097
			// (get) Token: 0x06001954 RID: 6484 RVA: 0x0009CBA0 File Offset: 0x0009BBA0
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

			// Token: 0x140000BB RID: 187
			// (add) Token: 0x06001955 RID: 6485 RVA: 0x0009CBF4 File Offset: 0x0009BBF4
			// (remove) Token: 0x06001956 RID: 6486 RVA: 0x0009CC2C File Offset: 0x0009BC2C
			
			public event EventHandler<EventArgs<SMTP_Client.MailFromAsyncOP>> CompletedAsync = null;

			// Token: 0x06001957 RID: 6487 RVA: 0x0009CC64 File Offset: 0x0009BC64
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.MailFromAsyncOP>(this));
				}
			}

			// Token: 0x04000B18 RID: 2840
			private object m_pLock = new object();

			// Token: 0x04000B19 RID: 2841
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B1A RID: 2842
			private Exception m_pException = null;

			// Token: 0x04000B1B RID: 2843
			private string m_MailFrom = null;

			// Token: 0x04000B1C RID: 2844
			private long m_MessageSize = -1L;

			// Token: 0x04000B1D RID: 2845
			private SMTP_DSN_Ret m_DsnRet = SMTP_DSN_Ret.NotSpecified;

			// Token: 0x04000B1E RID: 2846
			private string m_EnvID = null;

			// Token: 0x04000B1F RID: 2847
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B20 RID: 2848
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002EC RID: 748
		public class RcptToAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001958 RID: 6488 RVA: 0x0009CC94 File Offset: 0x0009BC94
			public RcptToAsyncOP(string to) : this(to, SMTP_DSN_Notify.NotSpecified, null)
			{
			}

			// Token: 0x06001959 RID: 6489 RVA: 0x0009CCA4 File Offset: 0x0009BCA4
			public RcptToAsyncOP(string to, SMTP_DSN_Notify notify, string orcpt)
			{
				bool flag = to == null;
				if (flag)
				{
					throw new ArgumentNullException("to");
				}
				bool flag2 = to == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Argument 'to' value must be specified.", "to");
				}
				this.m_To = to;
				this.m_DsnNotify = notify;
				this.m_ORcpt = orcpt;
			}

			// Token: 0x0600195A RID: 6490 RVA: 0x0009CD48 File Offset: 0x0009BD48
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_To = null;
					this.m_ORcpt = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x0600195B RID: 6491 RVA: 0x0009CD94 File Offset: 0x0009BD94
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("RCPT TO:<" + this.m_To + ">");
					bool flag2 = this.m_DsnNotify == SMTP_DSN_Notify.NotSpecified;
					if (!flag2)
					{
						bool flag3 = this.m_DsnNotify == SMTP_DSN_Notify.Never;
						if (flag3)
						{
							stringBuilder.Append(" NOTIFY=NEVER");
						}
						else
						{
							bool flag4 = true;
							bool flag5 = (this.m_DsnNotify & SMTP_DSN_Notify.Delay) > SMTP_DSN_Notify.NotSpecified;
							if (flag5)
							{
								stringBuilder.Append(" NOTIFY=DELAY");
								flag4 = false;
							}
							bool flag6 = (this.m_DsnNotify & SMTP_DSN_Notify.Failure) > SMTP_DSN_Notify.NotSpecified;
							if (flag6)
							{
								bool flag7 = flag4;
								if (flag7)
								{
									stringBuilder.Append(" NOTIFY=FAILURE");
								}
								else
								{
									stringBuilder.Append(",FAILURE");
								}
								flag4 = false;
							}
							bool flag8 = (this.m_DsnNotify & SMTP_DSN_Notify.Success) > SMTP_DSN_Notify.NotSpecified;
							if (flag8)
							{
								bool flag9 = flag4;
								if (flag9)
								{
									stringBuilder.Append(" NOTIFY=SUCCESS");
								}
								else
								{
									stringBuilder.Append(",SUCCESS");
								}
							}
						}
					}
					bool flag10 = !string.IsNullOrEmpty(this.m_ORcpt);
					if (flag10)
					{
						stringBuilder.Append(" ORCPT=" + this.m_ORcpt);
					}
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString() + "\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, stringBuilder.ToString());
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.RcptCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag11 = this.m_pSmtpClient != null;
					if (flag11)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x0600195C RID: 6492 RVA: 0x0009CFF8 File Offset: 0x0009BFF8
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

			// Token: 0x0600195D RID: 6493 RVA: 0x0009D070 File Offset: 0x0009C070
			private void RcptCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.RcptReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.RcptReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600195E RID: 6494 RVA: 0x0009D13C File Offset: 0x0009C13C
			private void RcptReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 250;
						if (flag3)
						{
							bool flag4 = !this.m_pSmtpClient.m_pRecipients.Contains(this.m_To);
							if (flag4)
							{
								this.m_pSmtpClient.m_pRecipients.Add(this.m_To);
							}
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag5 = this.m_pSmtpClient != null;
					if (flag5)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000832 RID: 2098
			// (get) Token: 0x0600195F RID: 6495 RVA: 0x0009D298 File Offset: 0x0009C298
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000833 RID: 2099
			// (get) Token: 0x06001960 RID: 6496 RVA: 0x0009D2B0 File Offset: 0x0009C2B0
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

			// Token: 0x140000BC RID: 188
			// (add) Token: 0x06001961 RID: 6497 RVA: 0x0009D304 File Offset: 0x0009C304
			// (remove) Token: 0x06001962 RID: 6498 RVA: 0x0009D33C File Offset: 0x0009C33C
			
			public event EventHandler<EventArgs<SMTP_Client.RcptToAsyncOP>> CompletedAsync = null;

			// Token: 0x06001963 RID: 6499 RVA: 0x0009D374 File Offset: 0x0009C374
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.RcptToAsyncOP>(this));
				}
			}

			// Token: 0x04000B22 RID: 2850
			private object m_pLock = new object();

			// Token: 0x04000B23 RID: 2851
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B24 RID: 2852
			private Exception m_pException = null;

			// Token: 0x04000B25 RID: 2853
			private string m_To = null;

			// Token: 0x04000B26 RID: 2854
			private SMTP_DSN_Notify m_DsnNotify = SMTP_DSN_Notify.NotSpecified;

			// Token: 0x04000B27 RID: 2855
			private string m_ORcpt = null;

			// Token: 0x04000B28 RID: 2856
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B29 RID: 2857
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002ED RID: 749
		public class SendMessageAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001964 RID: 6500 RVA: 0x0009D3A4 File Offset: 0x0009C3A4
			public SendMessageAsyncOP(Stream stream, bool useBdatIfPossibe)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pStream = stream;
				this.m_UseBdat = useBdatIfPossibe;
			}

			// Token: 0x06001965 RID: 6501 RVA: 0x0009D42C File Offset: 0x0009C42C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pStream = null;
					this.m_pSmtpClient = null;
					this.m_pBdatBuffer = null;
					this.m_BdatSendBuffer = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001966 RID: 6502 RVA: 0x0009D47C File Offset: 0x0009C47C
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool flag2 = false;
					foreach (string text in this.m_pSmtpClient.EsmtpFeatures)
					{
						bool flag3 = text.ToUpper() == SMTP_ServiceExtensions.CHUNKING;
						if (flag3)
						{
							flag2 = true;
							break;
						}
					}
					bool flag4 = flag2 && this.m_UseBdat;
					if (flag4)
					{
						this.m_pBdatBuffer = new byte[64000];
						this.m_BdatSendBuffer = new byte[64100];
						this.m_pStream.BeginRead(this.m_pBdatBuffer, 0, this.m_pBdatBuffer.Length, new AsyncCallback(this.BdatChunkReadingCompleted), null);
					}
					else
					{
						byte[] bytes = Encoding.UTF8.GetBytes("DATA\r\n");
						this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "DATA");
						this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.DataCommandSendingCompleted), null);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag5 = this.m_pSmtpClient != null;
					if (flag5)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
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

			// Token: 0x06001967 RID: 6503 RVA: 0x0009D640 File Offset: 0x0009C640
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

			// Token: 0x06001968 RID: 6504 RVA: 0x0009D6B8 File Offset: 0x0009C6B8
			private void BdatChunkReadingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_BdatBytesInBuffer = this.m_pStream.EndRead(ar);
					bool flag = this.m_BdatBytesInBuffer > 0;
					if (flag)
					{
						byte[] bytes = Encoding.UTF8.GetBytes("BDAT " + this.m_BdatBytesInBuffer + "\r\n");
						this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "BDAT " + this.m_BdatBytesInBuffer);
						this.m_pSmtpClient.LogAddWrite((long)this.m_BdatBytesInBuffer, "<BDAT data-chunk of " + this.m_BdatBytesInBuffer + " bytes>");
						Array.Copy(bytes, this.m_BdatSendBuffer, bytes.Length);
						Array.Copy(this.m_pBdatBuffer, 0, this.m_BdatSendBuffer, bytes.Length, this.m_BdatBytesInBuffer);
						this.m_pSmtpClient.TcpStream.BeginWrite(this.m_BdatSendBuffer, 0, bytes.Length + this.m_BdatBytesInBuffer, new AsyncCallback(this.BdatCommandSendingCompleted), null);
					}
					else
					{
						byte[] bytes2 = Encoding.UTF8.GetBytes("BDAT 0 LAST\r\n");
						this.m_pSmtpClient.LogAddWrite((long)bytes2.Length, "BDAT 0 LAST");
						this.m_pSmtpClient.TcpStream.BeginWrite(bytes2, 0, bytes2.Length, new AsyncCallback(this.BdatCommandSendingCompleted), null);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001969 RID: 6505 RVA: 0x0009D86C File Offset: 0x0009C86C
			private void BdatCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.BdatReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.BdatReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600196A RID: 6506 RVA: 0x0009D938 File Offset: 0x0009C938
			private void BdatReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 250;
						if (flag3)
						{
							bool flag4 = this.m_BdatBytesInBuffer == 0;
							if (flag4)
							{
								this.SetState(AsyncOP_State.Completed);
								return;
							}
							this.m_pStream.BeginRead(this.m_pBdatBuffer, 0, this.m_pBdatBuffer.Length, new AsyncCallback(this.BdatChunkReadingCompleted), null);
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag5 = this.m_pSmtpClient != null;
					if (flag5)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x0600196B RID: 6507 RVA: 0x0009DA90 File Offset: 0x0009CA90
			private void DataCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.DataReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.DataReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x0600196C RID: 6508 RVA: 0x0009DB5C File Offset: 0x0009CB5C
			private void DataReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 354;
						if (flag3)
						{
							SmartStream.WritePeriodTerminatedAsyncOP sendMsgOP = new SmartStream.WritePeriodTerminatedAsyncOP(this.m_pStream);
							sendMsgOP.CompletedAsync += delegate(object s, EventArgs<SmartStream.WritePeriodTerminatedAsyncOP> e)
							{
								this.DataMsgSendingCompleted(sendMsgOP);
							};
							bool flag4 = !this.m_pSmtpClient.TcpStream.WritePeriodTerminatedAsync(sendMsgOP);
							if (flag4)
							{
								this.DataMsgSendingCompleted(sendMsgOP);
							}
						}
						else
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.SetState(AsyncOP_State.Completed);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag5 = this.m_pSmtpClient != null;
					if (flag5)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x0600196D RID: 6509 RVA: 0x0009DCD4 File Offset: 0x0009CCD4
			private void DataMsgSendingCompleted(SmartStream.WritePeriodTerminatedAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						this.m_pSmtpClient.LogAddWrite((long)op.BytesWritten, "Sent message " + op.BytesWritten + " bytes.");
						SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
						readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
						{
							this.DataReadFinalResponseCompleted(readResponseOP);
						};
						bool flag3 = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
						if (flag3)
						{
							this.DataReadFinalResponseCompleted(readResponseOP);
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x0600196E RID: 6510 RVA: 0x0009DE38 File Offset: 0x0009CE38
			private void DataReadFinalResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						this.SetState(AsyncOP_State.Completed);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode < 200 || op.ReplyLines[0].ReplyCode > 299;
						if (flag3)
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
						}
						this.SetState(AsyncOP_State.Completed);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x17000834 RID: 2100
			// (get) Token: 0x0600196F RID: 6511 RVA: 0x0009DF5C File Offset: 0x0009CF5C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000835 RID: 2101
			// (get) Token: 0x06001970 RID: 6512 RVA: 0x0009DF74 File Offset: 0x0009CF74
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

			// Token: 0x140000BD RID: 189
			// (add) Token: 0x06001971 RID: 6513 RVA: 0x0009DFC8 File Offset: 0x0009CFC8
			// (remove) Token: 0x06001972 RID: 6514 RVA: 0x0009E000 File Offset: 0x0009D000
			
			public event EventHandler<EventArgs<SMTP_Client.SendMessageAsyncOP>> CompletedAsync = null;

			// Token: 0x06001973 RID: 6515 RVA: 0x0009E038 File Offset: 0x0009D038
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.SendMessageAsyncOP>(this));
				}
			}

			// Token: 0x04000B2B RID: 2859
			private object m_pLock = new object();

			// Token: 0x04000B2C RID: 2860
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B2D RID: 2861
			private Exception m_pException = null;

			// Token: 0x04000B2E RID: 2862
			private Stream m_pStream = null;

			// Token: 0x04000B2F RID: 2863
			private bool m_UseBdat = false;

			// Token: 0x04000B30 RID: 2864
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B31 RID: 2865
			private byte[] m_pBdatBuffer = null;

			// Token: 0x04000B32 RID: 2866
			private int m_BdatBytesInBuffer = 0;

			// Token: 0x04000B33 RID: 2867
			private byte[] m_BdatSendBuffer = null;

			// Token: 0x04000B34 RID: 2868
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002EE RID: 750
		public class RsetAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001975 RID: 6517 RVA: 0x0009E0A0 File Offset: 0x0009D0A0
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001976 RID: 6518 RVA: 0x0009E0DC File Offset: 0x0009D0DC
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("RSET\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "RSET");
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.RsetCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x06001977 RID: 6519 RVA: 0x0009E1F0 File Offset: 0x0009D1F0
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

			// Token: 0x06001978 RID: 6520 RVA: 0x0009E268 File Offset: 0x0009D268
			private void RsetCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.RsetReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.RsetReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001979 RID: 6521 RVA: 0x0009E334 File Offset: 0x0009D334
			private void RsetReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 250;
						if (!flag3)
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000836 RID: 2102
			// (get) Token: 0x0600197A RID: 6522 RVA: 0x0009E44C File Offset: 0x0009D44C
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000837 RID: 2103
			// (get) Token: 0x0600197B RID: 6523 RVA: 0x0009E464 File Offset: 0x0009D464
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

			// Token: 0x140000BE RID: 190
			// (add) Token: 0x0600197C RID: 6524 RVA: 0x0009E4B8 File Offset: 0x0009D4B8
			// (remove) Token: 0x0600197D RID: 6525 RVA: 0x0009E4F0 File Offset: 0x0009D4F0
			
			public event EventHandler<EventArgs<SMTP_Client.RsetAsyncOP>> CompletedAsync = null;

			// Token: 0x0600197E RID: 6526 RVA: 0x0009E528 File Offset: 0x0009D528
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.RsetAsyncOP>(this));
				}
			}

			// Token: 0x04000B36 RID: 2870
			private object m_pLock = new object();

			// Token: 0x04000B37 RID: 2871
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B38 RID: 2872
			private Exception m_pException = null;

			// Token: 0x04000B39 RID: 2873
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B3A RID: 2874
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002EF RID: 751
		public class NoopAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x06001980 RID: 6528 RVA: 0x0009E590 File Offset: 0x0009D590
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pSmtpClient = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x06001981 RID: 6529 RVA: 0x0009E5CC File Offset: 0x0009D5CC
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					byte[] bytes = Encoding.UTF8.GetBytes("NOOP\r\n");
					this.m_pSmtpClient.LogAddWrite((long)bytes.Length, "NOOP");
					this.m_pSmtpClient.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.NoopCommandSendingCompleted), null);
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
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

			// Token: 0x06001982 RID: 6530 RVA: 0x0009E6E0 File Offset: 0x0009D6E0
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

			// Token: 0x06001983 RID: 6531 RVA: 0x0009E758 File Offset: 0x0009D758
			private void NoopCommandSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSmtpClient.TcpStream.EndWrite(ar);
					SMTP_Client.ReadResponseAsyncOP readResponseOP = new SMTP_Client.ReadResponseAsyncOP();
					readResponseOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.ReadResponseAsyncOP> e)
					{
						this.NoopReadResponseCompleted(readResponseOP);
					};
					bool flag = !this.m_pSmtpClient.ReadResponseAsync(readResponseOP);
					if (flag)
					{
						this.NoopReadResponseCompleted(readResponseOP);
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag2 = this.m_pSmtpClient != null;
					if (flag2)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x06001984 RID: 6532 RVA: 0x0009E824 File Offset: 0x0009D824
			private void NoopReadResponseCompleted(SMTP_Client.ReadResponseAsyncOP op)
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
						this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					}
					else
					{
						bool flag3 = op.ReplyLines[0].ReplyCode == 250;
						if (!flag3)
						{
							this.m_pException = new SMTP_ClientException(op.ReplyLines);
							this.m_pSmtpClient.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
						}
					}
				}
				catch (Exception ex)
				{
					this.m_pException = ex;
					bool flag4 = this.m_pSmtpClient != null;
					if (flag4)
					{
						this.m_pSmtpClient.LogAddException("Exception: " + ex.Message, ex);
					}
				}
				op.Dispose();
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000838 RID: 2104
			// (get) Token: 0x06001985 RID: 6533 RVA: 0x0009E940 File Offset: 0x0009D940
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000839 RID: 2105
			// (get) Token: 0x06001986 RID: 6534 RVA: 0x0009E958 File Offset: 0x0009D958
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

			// Token: 0x140000BF RID: 191
			// (add) Token: 0x06001987 RID: 6535 RVA: 0x0009E9AC File Offset: 0x0009D9AC
			// (remove) Token: 0x06001988 RID: 6536 RVA: 0x0009E9E4 File Offset: 0x0009D9E4
			
			public event EventHandler<EventArgs<SMTP_Client.NoopAsyncOP>> CompletedAsync = null;

			// Token: 0x06001989 RID: 6537 RVA: 0x0009EA1C File Offset: 0x0009DA1C
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.NoopAsyncOP>(this));
				}
			}

			// Token: 0x04000B3C RID: 2876
			private object m_pLock = new object();

			// Token: 0x04000B3D RID: 2877
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B3E RID: 2878
			private Exception m_pException = null;

			// Token: 0x04000B3F RID: 2879
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B40 RID: 2880
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002F0 RID: 752
		private class ReadResponseAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x0600198A RID: 6538 RVA: 0x0009EA4C File Offset: 0x0009DA4C
			public ReadResponseAsyncOP()
			{
				this.m_pReplyLines = new List<SMTP_t_ReplyLine>();
			}

			// Token: 0x0600198B RID: 6539 RVA: 0x0009EA84 File Offset: 0x0009DA84
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pSmtpClient = null;
					this.m_pReplyLines = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x0600198C RID: 6540 RVA: 0x0009EAC8 File Offset: 0x0009DAC8
			internal bool Start(SMTP_Client owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSmtpClient = owner;
				bool result;
				try
				{
					SmartStream.ReadLineAsyncOP op = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					op.CompletedAsync += delegate(object s, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						try
						{
							bool flag3 = !this.ReadLineCompleted(op);
							if (flag3)
							{
								this.SetState(AsyncOP_State.Completed);
								this.OnCompletedAsync();
							}
							else
							{
								while (owner.TcpStream.ReadLine(op, true))
								{
									bool flag4 = !this.ReadLineCompleted(op);
									if (flag4)
									{
										this.SetState(AsyncOP_State.Completed);
										this.OnCompletedAsync();
										break;
									}
								}
							}
						}
						catch (Exception pException2)
						{
							this.m_pException = pException2;
							this.SetState(AsyncOP_State.Completed);
							this.OnCompletedAsync();
						}
					};
					while (owner.TcpStream.ReadLine(op, true))
					{
						bool flag2 = !this.ReadLineCompleted(op);
						if (flag2)
						{
							this.SetState(AsyncOP_State.Completed);
							return false;
						}
					}
					result = true;
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.SetState(AsyncOP_State.Completed);
					result = false;
				}
				return result;
			}

			// Token: 0x0600198D RID: 6541 RVA: 0x0009EBC4 File Offset: 0x0009DBC4
			private void SetState(AsyncOP_State state)
			{
				this.m_State = state;
			}

			// Token: 0x0600198E RID: 6542 RVA: 0x0009EBD0 File Offset: 0x0009DBD0
			private bool ReadLineCompleted(SmartStream.ReadLineAsyncOP op)
			{
				bool flag = op == null;
				if (flag)
				{
					throw new ArgumentNullException("op");
				}
				try
				{
					bool flag2 = op.Error != null;
					if (!flag2)
					{
						this.m_pSmtpClient.LogAddRead((long)op.BytesInBuffer, op.LineUtf8);
						SMTP_t_ReplyLine smtp_t_ReplyLine = SMTP_t_ReplyLine.Parse(op.LineUtf8);
						this.m_pReplyLines.Add(smtp_t_ReplyLine);
						return !smtp_t_ReplyLine.IsLastLine;
					}
					this.m_pException = op.Error;
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				return false;
			}

			// Token: 0x1700083A RID: 2106
			// (get) Token: 0x0600198F RID: 6543 RVA: 0x0009EC78 File Offset: 0x0009DC78
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x1700083B RID: 2107
			// (get) Token: 0x06001990 RID: 6544 RVA: 0x0009EC90 File Offset: 0x0009DC90
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

			// Token: 0x1700083C RID: 2108
			// (get) Token: 0x06001991 RID: 6545 RVA: 0x0009ECE4 File Offset: 0x0009DCE4
			public SMTP_t_ReplyLine[] ReplyLines
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
						throw new InvalidOperationException("Property 'ReplyLines' is accessible only in 'AsyncOP_State.Completed' state.");
					}
					bool flag3 = this.m_pException != null;
					if (flag3)
					{
						throw this.m_pException;
					}
					return this.m_pReplyLines.ToArray();
				}
			}

			// Token: 0x140000C0 RID: 192
			// (add) Token: 0x06001992 RID: 6546 RVA: 0x0009ED54 File Offset: 0x0009DD54
			// (remove) Token: 0x06001993 RID: 6547 RVA: 0x0009ED8C File Offset: 0x0009DD8C
			
			public event EventHandler<EventArgs<SMTP_Client.ReadResponseAsyncOP>> CompletedAsync = null;

			// Token: 0x06001994 RID: 6548 RVA: 0x0009EDC4 File Offset: 0x0009DDC4
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Client.ReadResponseAsyncOP>(this));
				}
			}

			// Token: 0x04000B42 RID: 2882
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000B43 RID: 2883
			private Exception m_pException = null;

			// Token: 0x04000B44 RID: 2884
			private SMTP_Client m_pSmtpClient = null;

			// Token: 0x04000B45 RID: 2885
			private List<SMTP_t_ReplyLine> m_pReplyLines = null;
		}

		// Token: 0x020002F1 RID: 753
		// (Invoke) Token: 0x06001996 RID: 6550
		[Obsolete("Use method 'AuthAsync' instead.")]
		private delegate void AuthenticateDelegate(string userName, string password);

		// Token: 0x020002F2 RID: 754
		// (Invoke) Token: 0x0600199A RID: 6554
		private delegate void NoopDelegate();

		// Token: 0x020002F3 RID: 755
		// (Invoke) Token: 0x0600199E RID: 6558
		private delegate void StartTLSDelegate();

		// Token: 0x020002F4 RID: 756
		// (Invoke) Token: 0x060019A2 RID: 6562
		private delegate void RcptToDelegate(string to, SMTP_DSN_Notify notify, string orcpt);

		// Token: 0x020002F5 RID: 757
		// (Invoke) Token: 0x060019A6 RID: 6566
		private delegate void MailFromDelegate(string from, long messageSize, SMTP_DSN_Ret ret, string envid);

		// Token: 0x020002F6 RID: 758
		// (Invoke) Token: 0x060019AA RID: 6570
		private delegate void ResetDelegate();

		// Token: 0x020002F7 RID: 759
		// (Invoke) Token: 0x060019AE RID: 6574
		private delegate void SendMessageDelegate(Stream message);

		// Token: 0x020002F8 RID: 760
		// (Invoke) Token: 0x060019B2 RID: 6578
		private delegate string[] GetDomainHostsDelegate(string domain);
	}
}
