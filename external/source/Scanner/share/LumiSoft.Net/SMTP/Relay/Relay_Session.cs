using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.IO;
using LumiSoft.Net.Log;
using LumiSoft.Net.SMTP.Client;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000147 RID: 327
	public class Relay_Session : TCP_Session
	{
		// Token: 0x06000CEB RID: 3307 RVA: 0x0004F9C8 File Offset: 0x0004E9C8
		internal Relay_Session(Relay_Server server, Relay_QueueItem realyItem)
		{
			bool flag = server == null;
			if (flag)
			{
				throw new ArgumentNullException("server");
			}
			bool flag2 = realyItem == null;
			if (flag2)
			{
				throw new ArgumentNullException("realyItem");
			}
			this.m_pServer = server;
			this.m_pRelayItem = realyItem;
			this.m_SessionID = Guid.NewGuid().ToString();
			this.m_SessionCreateTime = DateTime.Now;
			this.m_pTargets = new List<Relay_Session.Relay_Target>();
			this.m_pSmtpClient = new SMTP_Client();
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0004FA98 File Offset: 0x0004EA98
		internal Relay_Session(Relay_Server server, Relay_QueueItem realyItem, Relay_SmartHost[] smartHosts)
		{
			bool flag = server == null;
			if (flag)
			{
				throw new ArgumentNullException("server");
			}
			bool flag2 = realyItem == null;
			if (flag2)
			{
				throw new ArgumentNullException("realyItem");
			}
			bool flag3 = smartHosts == null;
			if (flag3)
			{
				throw new ArgumentNullException("smartHosts");
			}
			this.m_pServer = server;
			this.m_pRelayItem = realyItem;
			this.m_pSmartHosts = smartHosts;
			this.m_RelayMode = Relay_Mode.SmartHost;
			this.m_SessionID = Guid.NewGuid().ToString();
			this.m_SessionCreateTime = DateTime.Now;
			this.m_pTargets = new List<Relay_Session.Relay_Target>();
			this.m_pSmtpClient = new SMTP_Client();
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0004FB89 File Offset: 0x0004EB89
		public override void Dispose()
		{
			this.Dispose(new ObjectDisposedException(base.GetType().Name));
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0004FBA4 File Offset: 0x0004EBA4
		public void Dispose(Exception exception)
		{
			try
			{
				lock (this)
				{
					bool isDisposed = this.m_IsDisposed;
					if (!isDisposed)
					{
						try
						{
							this.m_pServer.OnSessionCompleted(this, exception);
						}
						catch
						{
						}
						this.m_pServer.Sessions.Remove(this);
						this.m_IsDisposed = true;
						this.m_pLocalBindInfo = null;
						this.m_pRelayItem = null;
						this.m_pSmartHosts = null;
						bool flag2 = this.m_pSmtpClient != null;
						if (flag2)
						{
							this.m_pSmtpClient.Dispose();
							this.m_pSmtpClient = null;
						}
						this.m_pTargets = null;
						bool flag3 = this.m_pActiveTarget != null;
						if (flag3)
						{
							this.m_pServer.RemoveIpUsage(this.m_pActiveTarget.Target.Address);
							this.m_pActiveTarget = null;
						}
						this.m_pServer = null;
					}
				}
			}
			catch (Exception x)
			{
				bool flag4 = this.m_pServer != null;
				if (flag4)
				{
					this.m_pServer.OnError(x);
				}
			}
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0004FCDC File Offset: 0x0004ECDC
		internal void Start(object state)
		{
			try
			{
				bool flag = this.m_pServer.Logger != null;
				if (flag)
				{
					this.m_pSmtpClient.Logger = new Logger();
					this.m_pSmtpClient.Logger.WriteLog += this.SmtpClient_WriteLog;
				}
				this.LogText(string.Concat(new string[]
				{
					"Starting to relay message '",
					this.m_pRelayItem.MessageID,
					"' from '",
					this.m_pRelayItem.From,
					"' to '",
					this.m_pRelayItem.To,
					"'."
				}));
				bool flag2 = this.m_RelayMode == Relay_Mode.Dns;
				if (flag2)
				{
					Dns_Client.GetEmailHostsAsyncOP op = new Dns_Client.GetEmailHostsAsyncOP(this.m_pRelayItem.To);
					op.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetEmailHostsAsyncOP> e1)
					{
						this.EmailHostsResolveCompleted(this.m_pRelayItem.To, op);
					};
					bool flag3 = !this.m_pServer.DnsClient.GetEmailHostsAsync(op);
					if (flag3)
					{
						this.EmailHostsResolveCompleted(this.m_pRelayItem.To, op);
					}
				}
				else
				{
					bool flag4 = this.m_RelayMode == Relay_Mode.SmartHost;
					if (flag4)
					{
						string[] array = new string[this.m_pSmartHosts.Length];
						for (int i = 0; i < this.m_pSmartHosts.Length; i++)
						{
							array[i] = this.m_pSmartHosts[i].Host;
						}
						Dns_Client.GetHostsAddressesAsyncOP op = new Dns_Client.GetHostsAddressesAsyncOP(array);
						op.CompletedAsync += delegate(object s1, EventArgs<Dns_Client.GetHostsAddressesAsyncOP> e1)
						{
							this.SmartHostsResolveCompleted(op);
						};
						bool flag5 = !this.m_pServer.DnsClient.GetHostsAddressesAsync(op);
						if (flag5)
						{
							this.SmartHostsResolveCompleted(op);
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0004FEFC File Offset: 0x0004EEFC
		public override void Disconnect()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (!flag)
			{
				this.m_pSmtpClient.Disconnect();
			}
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x0004FF44 File Offset: 0x0004EF44
		public void Disconnect(string text)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = !this.IsConnected;
			if (!flag)
			{
				this.m_pSmtpClient.TcpStream.WriteLine(text);
				this.Disconnect();
			}
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0004FF98 File Offset: 0x0004EF98
		private void EmailHostsResolveCompleted(string to, Dns_Client.GetEmailHostsAsyncOP op)
		{
			bool flag = to == null;
			if (flag)
			{
				throw new ArgumentNullException("to");
			}
			bool flag2 = op == null;
			if (flag2)
			{
				throw new ArgumentNullException("op");
			}
			bool flag3 = op.Error != null;
			if (flag3)
			{
				this.LogText(string.Concat(new string[]
				{
					"Failed to resolve email domain for email address '",
					to,
					"' with error: ",
					op.Error.Message,
					"."
				}));
				this.Dispose(op.Error);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (HostEntry hostEntry in op.Hosts)
				{
					foreach (IPAddress address in hostEntry.Addresses)
					{
						this.m_pTargets.Add(new Relay_Session.Relay_Target(hostEntry.HostName, new IPEndPoint(address, 25)));
					}
					stringBuilder.Append(hostEntry.HostName + " ");
				}
				this.LogText("Resolved to following email hosts: (" + stringBuilder.ToString().TrimEnd(new char[0]) + ").");
				this.BeginConnect();
			}
			op.Dispose();
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x000500EC File Offset: 0x0004F0EC
		private void SmartHostsResolveCompleted(Dns_Client.GetHostsAddressesAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
			bool flag2 = op.Error != null;
			if (flag2)
			{
				this.LogText("Failed to resolve relay smart host(s) ip addresses with error: " + op.Error.Message + ".");
				this.Dispose(op.Error);
			}
			else
			{
				for (int i = 0; i < op.HostEntries.Length; i++)
				{
					Relay_SmartHost relay_SmartHost = this.m_pSmartHosts[i];
					foreach (IPAddress address in op.HostEntries[i].Addresses)
					{
						this.m_pTargets.Add(new Relay_Session.Relay_Target(relay_SmartHost.Host, new IPEndPoint(address, relay_SmartHost.Port), relay_SmartHost.SslMode, relay_SmartHost.UserName, relay_SmartHost.Password));
					}
				}
				this.BeginConnect();
			}
			op.Dispose();
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x000501EC File Offset: 0x0004F1EC
		private void BeginConnect()
		{
			bool flag = this.m_pTargets.Count == 0;
			if (flag)
			{
				this.LogText("No relay target(s) for '" + this.m_pRelayItem.To + "', aborting.");
				this.Dispose(new Exception("No relay target(s) for '" + this.m_pRelayItem.To + "', aborting."));
			}
			else
			{
				bool flag2 = this.m_pServer.MaxConnectionsPerIP > 0L;
				if (flag2)
				{
					bool flag3 = this.m_pServer.RelayMode == Relay_Mode.Dns || this.m_pServer.SmartHostsBalanceMode == BalanceMode.LoadBalance;
					if (flag3)
					{
						foreach (Relay_Session.Relay_Target relay_Target in this.m_pTargets)
						{
							this.m_pLocalBindInfo = this.m_pServer.GetLocalBinding(relay_Target.Target.Address);
							bool flag4 = this.m_pLocalBindInfo != null;
							if (flag4)
							{
								bool flag5 = this.m_pServer.TryAddIpUsage(relay_Target.Target.Address);
								if (flag5)
								{
									this.m_pActiveTarget = relay_Target;
									this.m_pTargets.Remove(relay_Target);
									break;
								}
								this.LogText(string.Concat(new object[]
								{
									"Skipping relay target (",
									relay_Target.HostName,
									"->",
									relay_Target.Target.Address,
									"), maximum connections to the specified IP has reached."
								}));
							}
							else
							{
								this.LogText(string.Concat(new object[]
								{
									"Skipping relay target (",
									relay_Target.HostName,
									"->",
									relay_Target.Target.Address,
									"), no suitable local IPv4/IPv6 binding."
								}));
							}
						}
					}
					else
					{
						this.m_pLocalBindInfo = this.m_pServer.GetLocalBinding(this.m_pTargets[0].Target.Address);
						bool flag6 = this.m_pLocalBindInfo != null;
						if (flag6)
						{
							bool flag7 = this.m_pServer.TryAddIpUsage(this.m_pTargets[0].Target.Address);
							if (flag7)
							{
								this.m_pActiveTarget = this.m_pTargets[0];
								this.m_pTargets.RemoveAt(0);
							}
							else
							{
								this.LogText(string.Concat(new object[]
								{
									"Skipping relay target (",
									this.m_pTargets[0].HostName,
									"->",
									this.m_pTargets[0].Target.Address,
									"), maximum connections to the specified IP has reached."
								}));
							}
						}
						else
						{
							this.LogText(string.Concat(new object[]
							{
								"Skipping relay target (",
								this.m_pTargets[0].HostName,
								"->",
								this.m_pTargets[0].Target.Address,
								"), no suitable local IPv4/IPv6 binding."
							}));
						}
					}
				}
				else
				{
					this.m_pLocalBindInfo = this.m_pServer.GetLocalBinding(this.m_pTargets[0].Target.Address);
					bool flag8 = this.m_pLocalBindInfo != null;
					if (flag8)
					{
						this.m_pActiveTarget = this.m_pTargets[0];
						this.m_pTargets.RemoveAt(0);
					}
					else
					{
						this.LogText(string.Concat(new object[]
						{
							"Skipping relay target (",
							this.m_pTargets[0].HostName,
							"->",
							this.m_pTargets[0].Target.Address,
							"), no suitable local IPv4/IPv6 binding."
						}));
					}
				}
				bool flag9 = this.m_pLocalBindInfo == null;
				if (flag9)
				{
					this.LogText("No suitable IPv4/IPv6 local IP endpoint for relay target.");
					this.Dispose(new Exception("No suitable IPv4/IPv6 local IP endpoint for relay target."));
				}
				else
				{
					bool flag10 = this.m_pActiveTarget == null;
					if (flag10)
					{
						this.LogText("All targets has exeeded maximum allowed connection per IP address, skip relay.");
						this.Dispose(new Exception("All targets has exeeded maximum allowed connection per IP address, skip relay."));
					}
					else
					{
						this.m_pSmtpClient.LocalHostName = this.m_pLocalBindInfo.HostName;
						TCP_Client.ConnectAsyncOP connectOP = new TCP_Client.ConnectAsyncOP(new IPEndPoint(this.m_pLocalBindInfo.IP, 0), this.m_pActiveTarget.Target, false, null);
						connectOP.CompletedAsync += delegate(object s, EventArgs<TCP_Client.ConnectAsyncOP> e)
						{
							this.ConnectCompleted(connectOP);
						};
						bool flag11 = !this.m_pSmtpClient.ConnectAsync(connectOP);
						if (flag11)
						{
							this.ConnectCompleted(connectOP);
						}
					}
				}
			}
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x000506D0 File Offset: 0x0004F6D0
		private void ConnectCompleted(TCP_Client.ConnectAsyncOP op)
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
					try
					{
						this.m_pServer.RemoveIpUsage(this.m_pActiveTarget.Target.Address);
						this.m_pActiveTarget = null;
						bool flag3 = !this.IsDisposed && !this.IsConnected && this.m_pTargets.Count > 0;
						if (flag3)
						{
							this.BeginConnect();
						}
						else
						{
							this.Dispose(op.Error);
						}
					}
					catch (Exception exception)
					{
						this.Dispose(exception);
					}
				}
				else
				{
					string hostName = string.IsNullOrEmpty(this.m_pLocalBindInfo.HostName) ? Dns.GetHostName() : this.m_pLocalBindInfo.HostName;
					SMTP_Client.EhloHeloAsyncOP ehloOP = new SMTP_Client.EhloHeloAsyncOP(hostName);
					ehloOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.EhloHeloAsyncOP> e)
					{
						this.EhloCommandCompleted(ehloOP);
					};
					bool flag4 = !this.m_pSmtpClient.EhloHeloAsync(ehloOP);
					if (flag4)
					{
						this.EhloCommandCompleted(ehloOP);
					}
				}
			}
			catch (Exception exception2)
			{
				this.Dispose(exception2);
			}
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0005084C File Offset: 0x0004F84C
		private void EhloCommandCompleted(SMTP_Client.EhloHeloAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					bool flag3 = !this.m_pSmtpClient.IsSecureConnection && ((this.m_pServer.UseTlsIfPossible && this.IsTlsSupported()) || this.m_pActiveTarget.SslMode == SslMode.TLS);
					if (flag3)
					{
						SMTP_Client.StartTlsAsyncOP startTlsOP = new SMTP_Client.StartTlsAsyncOP(null);
						startTlsOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.StartTlsAsyncOP> e)
						{
							this.StartTlsCommandCompleted(startTlsOP);
						};
						bool flag4 = !this.m_pSmtpClient.StartTlsAsync(startTlsOP);
						if (flag4)
						{
							this.StartTlsCommandCompleted(startTlsOP);
						}
					}
					else
					{
						bool flag5 = !string.IsNullOrEmpty(this.m_pActiveTarget.UserName);
						if (flag5)
						{
							SMTP_Client.AuthAsyncOP authOP = new SMTP_Client.AuthAsyncOP(this.m_pSmtpClient.AuthGetStrongestMethod(this.m_pActiveTarget.UserName, this.m_pActiveTarget.Password));
							authOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.AuthAsyncOP> e)
							{
								this.AuthCommandCompleted(authOP);
							};
							bool flag6 = !this.m_pSmtpClient.AuthAsync(authOP);
							if (flag6)
							{
								this.AuthCommandCompleted(authOP);
							}
						}
						else
						{
							long messageSize = -1L;
							try
							{
								messageSize = this.m_pRelayItem.MessageStream.Length - this.m_pRelayItem.MessageStream.Position;
							}
							catch
							{
							}
							SMTP_Client.MailFromAsyncOP mailOP = new SMTP_Client.MailFromAsyncOP(this.From, messageSize, this.IsDsnSupported() ? this.m_pRelayItem.DSN_Ret : SMTP_DSN_Ret.NotSpecified, this.IsDsnSupported() ? this.m_pRelayItem.EnvelopeID : null);
							mailOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.MailFromAsyncOP> e)
							{
								this.MailCommandCompleted(mailOP);
							};
							bool flag7 = !this.m_pSmtpClient.MailFromAsync(mailOP);
							if (flag7)
							{
								this.MailCommandCompleted(mailOP);
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00050AE0 File Offset: 0x0004FAE0
		private void StartTlsCommandCompleted(SMTP_Client.StartTlsAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					string hostName = string.IsNullOrEmpty(this.m_pLocalBindInfo.HostName) ? Dns.GetHostName() : this.m_pLocalBindInfo.HostName;
					SMTP_Client.EhloHeloAsyncOP ehloOP = new SMTP_Client.EhloHeloAsyncOP(hostName);
					ehloOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.EhloHeloAsyncOP> e)
					{
						this.EhloCommandCompleted(ehloOP);
					};
					bool flag3 = !this.m_pSmtpClient.EhloHeloAsync(ehloOP);
					if (flag3)
					{
						this.EhloCommandCompleted(ehloOP);
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x00050BC8 File Offset: 0x0004FBC8
		private void AuthCommandCompleted(SMTP_Client.AuthAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					long messageSize = -1L;
					try
					{
						messageSize = this.m_pRelayItem.MessageStream.Length - this.m_pRelayItem.MessageStream.Position;
					}
					catch
					{
					}
					SMTP_Client.MailFromAsyncOP mailOP = new SMTP_Client.MailFromAsyncOP(this.From, messageSize, this.IsDsnSupported() ? this.m_pRelayItem.DSN_Ret : SMTP_DSN_Ret.NotSpecified, this.IsDsnSupported() ? this.m_pRelayItem.EnvelopeID : null);
					mailOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.MailFromAsyncOP> e)
					{
						this.MailCommandCompleted(mailOP);
					};
					bool flag3 = !this.m_pSmtpClient.MailFromAsync(mailOP);
					if (flag3)
					{
						this.MailCommandCompleted(mailOP);
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x00050CF8 File Offset: 0x0004FCF8
		private void MailCommandCompleted(SMTP_Client.MailFromAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					SMTP_Client.RcptToAsyncOP rcptOP = new SMTP_Client.RcptToAsyncOP(this.To, this.IsDsnSupported() ? this.m_pRelayItem.DSN_Notify : SMTP_DSN_Notify.NotSpecified, this.IsDsnSupported() ? this.m_pRelayItem.OriginalRecipient : null);
					rcptOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.RcptToAsyncOP> e)
					{
						this.RcptCommandCompleted(rcptOP);
					};
					bool flag3 = !this.m_pSmtpClient.RcptToAsync(rcptOP);
					if (flag3)
					{
						this.RcptCommandCompleted(rcptOP);
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00050DEC File Offset: 0x0004FDEC
		private void RcptCommandCompleted(SMTP_Client.RcptToAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					SMTP_Client.SendMessageAsyncOP sendMsgOP = new SMTP_Client.SendMessageAsyncOP(this.m_pRelayItem.MessageStream, false);
					sendMsgOP.CompletedAsync += delegate(object s, EventArgs<SMTP_Client.SendMessageAsyncOP> e)
					{
						this.MessageSendingCompleted(sendMsgOP);
					};
					bool flag3 = !this.m_pSmtpClient.SendMessageAsync(sendMsgOP);
					if (flag3)
					{
						this.MessageSendingCompleted(sendMsgOP);
					}
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00050EB8 File Offset: 0x0004FEB8
		private void MessageSendingCompleted(SMTP_Client.SendMessageAsyncOP op)
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
					this.Dispose(op.Error);
				}
				else
				{
					this.Dispose(null);
				}
			}
			catch (Exception exception)
			{
				this.Dispose(exception);
			}
			op.Dispose();
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00050F2C File Offset: 0x0004FF2C
		private void SmtpClient_WriteLog(object sender, WriteLogEventArgs e)
		{
			try
			{
				bool flag = this.m_pServer.Logger == null;
				if (!flag)
				{
					bool flag2 = e.LogEntry.EntryType == LogEntryType.Read;
					if (flag2)
					{
						this.m_pServer.Logger.AddRead(this.m_SessionID, e.LogEntry.UserIdentity, e.LogEntry.Size, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
					}
					else
					{
						bool flag3 = e.LogEntry.EntryType == LogEntryType.Text;
						if (flag3)
						{
							this.m_pServer.Logger.AddText(this.m_SessionID, e.LogEntry.UserIdentity, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
						}
						else
						{
							bool flag4 = e.LogEntry.EntryType == LogEntryType.Write;
							if (flag4)
							{
								this.m_pServer.Logger.AddWrite(this.m_SessionID, e.LogEntry.UserIdentity, e.LogEntry.Size, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
							}
							else
							{
								bool flag5 = e.LogEntry.EntryType == LogEntryType.Exception;
								if (flag5)
								{
									this.m_pServer.Logger.AddException(this.m_SessionID, e.LogEntry.UserIdentity, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint, e.LogEntry.Exception);
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

		// Token: 0x06000CFD RID: 3325 RVA: 0x00051104 File Offset: 0x00050104
		private void LogText(string text)
		{
			bool flag = this.m_pServer.Logger != null;
			if (flag)
			{
				GenericIdentity userIdentity = null;
				try
				{
					userIdentity = this.AuthenticatedUserIdentity;
				}
				catch
				{
				}
				IPEndPoint localEP = null;
				IPEndPoint remoteEP = null;
				try
				{
					localEP = this.m_pSmtpClient.LocalEndPoint;
					remoteEP = this.m_pSmtpClient.RemoteEndPoint;
				}
				catch
				{
				}
				this.m_pServer.Logger.AddText(this.m_SessionID, userIdentity, text, localEP, remoteEP);
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00051194 File Offset: 0x00050194
		private bool IsDsnSupported()
		{
			foreach (string a in this.m_pSmtpClient.EsmtpFeatures)
			{
				bool flag = string.Equals(a, SMTP_ServiceExtensions.DSN, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x000511E0 File Offset: 0x000501E0
		private bool IsTlsSupported()
		{
			foreach (string a in this.m_pSmtpClient.EsmtpFeatures)
			{
				bool flag = string.Equals(a, SMTP_ServiceExtensions.STARTTLS, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000D00 RID: 3328 RVA: 0x0005122C File Offset: 0x0005022C
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000D01 RID: 3329 RVA: 0x00051244 File Offset: 0x00050244
		public string LocalHostName
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (this.m_pLocalBindInfo == null) ? "" : this.m_pLocalBindInfo.HostName;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000D02 RID: 3330 RVA: 0x0005128C File Offset: 0x0005028C
		public DateTime SessionCreateTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SessionCreateTime;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x000512C0 File Offset: 0x000502C0
		public int ExpectedTimeout
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (int)((long)this.m_pServer.SessionIdleTimeout - (DateTime.Now.Ticks - this.TcpStream.LastActivity.Ticks) / 10000L);
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000D04 RID: 3332 RVA: 0x00051324 File Offset: 0x00050324
		public string From
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.From;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x00051360 File Offset: 0x00050360
		public string To
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.To;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06000D06 RID: 3334 RVA: 0x0005139C File Offset: 0x0005039C
		public string MessageID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.MessageID;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06000D07 RID: 3335 RVA: 0x000513D8 File Offset: 0x000503D8
		public Stream MessageStream
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.MessageStream;
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00051414 File Offset: 0x00050414
		public string RemoteHostName
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = this.m_pActiveTarget != null;
				string result;
				if (flag)
				{
					result = this.m_pActiveTarget.HostName;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06000D09 RID: 3337 RVA: 0x00051460 File Offset: 0x00050460
		public Relay_Queue Queue
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.Queue;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06000D0A RID: 3338 RVA: 0x0005149C File Offset: 0x0005049C
		public object QueueTag
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRelayItem.Tag;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06000D0B RID: 3339 RVA: 0x000514D8 File Offset: 0x000504D8
		public override GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				bool isDisposed = this.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = !this.m_pSmtpClient.IsConnected;
				if (flag)
				{
					throw new InvalidOperationException("You must connect first.");
				}
				return this.m_pSmtpClient.AuthenticatedUserIdentity;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06000D0C RID: 3340 RVA: 0x00051530 File Offset: 0x00050530
		public override bool IsConnected
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.IsConnected;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06000D0D RID: 3341 RVA: 0x0005156C File Offset: 0x0005056C
		public override string ID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_SessionID;
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06000D0E RID: 3342 RVA: 0x000515A0 File Offset: 0x000505A0
		public override DateTime ConnectTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.ConnectTime;
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06000D0F RID: 3343 RVA: 0x000515DC File Offset: 0x000505DC
		public override DateTime LastActivity
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.LastActivity;
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x00051618 File Offset: 0x00050618
		public override IPEndPoint LocalEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.LocalEndPoint;
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06000D11 RID: 3345 RVA: 0x00051654 File Offset: 0x00050654
		public override IPEndPoint RemoteEndPoint
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.RemoteEndPoint;
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06000D12 RID: 3346 RVA: 0x00051690 File Offset: 0x00050690
		public override SmartStream TcpStream
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pSmtpClient.TcpStream;
			}
		}

		// Token: 0x04000581 RID: 1409
		private bool m_IsDisposed = false;

		// Token: 0x04000582 RID: 1410
		private Relay_Server m_pServer = null;

		// Token: 0x04000583 RID: 1411
		private IPBindInfo m_pLocalBindInfo = null;

		// Token: 0x04000584 RID: 1412
		private Relay_QueueItem m_pRelayItem = null;

		// Token: 0x04000585 RID: 1413
		private Relay_SmartHost[] m_pSmartHosts = null;

		// Token: 0x04000586 RID: 1414
		private Relay_Mode m_RelayMode = Relay_Mode.Dns;

		// Token: 0x04000587 RID: 1415
		private string m_SessionID = "";

		// Token: 0x04000588 RID: 1416
		private DateTime m_SessionCreateTime;

		// Token: 0x04000589 RID: 1417
		private SMTP_Client m_pSmtpClient = null;

		// Token: 0x0400058A RID: 1418
		private List<Relay_Session.Relay_Target> m_pTargets = null;

		// Token: 0x0400058B RID: 1419
		private Relay_Session.Relay_Target m_pActiveTarget = null;

		// Token: 0x020002DC RID: 732
		private class Relay_Target
		{
			// Token: 0x0600190A RID: 6410 RVA: 0x0009ADBC File Offset: 0x00099DBC
			public Relay_Target(string hostName, IPEndPoint target)
			{
				this.m_HostName = hostName;
				this.m_pTarget = target;
			}

			// Token: 0x0600190B RID: 6411 RVA: 0x0009ADFC File Offset: 0x00099DFC
			public Relay_Target(string hostName, IPEndPoint target, SslMode sslMode, string userName, string password)
			{
				this.m_HostName = hostName;
				this.m_pTarget = target;
				this.m_SslMode = sslMode;
				this.m_UserName = userName;
				this.m_Password = password;
			}

			// Token: 0x17000824 RID: 2084
			// (get) Token: 0x0600190C RID: 6412 RVA: 0x0009AE60 File Offset: 0x00099E60
			public string HostName
			{
				get
				{
					return this.m_HostName;
				}
			}

			// Token: 0x17000825 RID: 2085
			// (get) Token: 0x0600190D RID: 6413 RVA: 0x0009AE78 File Offset: 0x00099E78
			public IPEndPoint Target
			{
				get
				{
					return this.m_pTarget;
				}
			}

			// Token: 0x17000826 RID: 2086
			// (get) Token: 0x0600190E RID: 6414 RVA: 0x0009AE90 File Offset: 0x00099E90
			public SslMode SslMode
			{
				get
				{
					return this.m_SslMode;
				}
			}

			// Token: 0x17000827 RID: 2087
			// (get) Token: 0x0600190F RID: 6415 RVA: 0x0009AEA8 File Offset: 0x00099EA8
			public string UserName
			{
				get
				{
					return this.m_UserName;
				}
			}

			// Token: 0x17000828 RID: 2088
			// (get) Token: 0x06001910 RID: 6416 RVA: 0x0009AEC0 File Offset: 0x00099EC0
			public string Password
			{
				get
				{
					return this.m_Password;
				}
			}

			// Token: 0x04000AE7 RID: 2791
			private string m_HostName = "";

			// Token: 0x04000AE8 RID: 2792
			private IPEndPoint m_pTarget = null;

			// Token: 0x04000AE9 RID: 2793
			private SslMode m_SslMode = SslMode.None;

			// Token: 0x04000AEA RID: 2794
			private string m_UserName = null;

			// Token: 0x04000AEB RID: 2795
			private string m_Password = null;
		}
	}
}
