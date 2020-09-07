using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.Mail;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x02000140 RID: 320
	public class SMTP_Session : TCP_ServerSession
	{
		// Token: 0x06000C6C RID: 3180 RVA: 0x0004BC44 File Offset: 0x0004AC44
		public SMTP_Session()
		{
			this.m_pAuthentications = new Dictionary<string, AUTH_SASL_ServerMechanism>(StringComparer.CurrentCultureIgnoreCase);
			this.m_pTo = new Dictionary<string, SMTP_RcptTo>();
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x0004BCEC File Offset: 0x0004ACEC
		public override void Dispose()
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				base.Dispose();
				this.m_pAuthentications = null;
				this.m_EhloHost = null;
				this.m_pUser = null;
				this.m_pFrom = null;
				this.m_pTo = null;
				bool flag = this.m_pMessageStream != null;
				if (flag)
				{
					this.m_pMessageStream.Dispose();
					this.m_pMessageStream = null;
				}
			}
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x0004BD54 File Offset: 0x0004AD54
		protected override void Start()
		{
			base.Start();
			try
			{
				bool flag = string.IsNullOrEmpty(this.Server.GreetingText);
				SMTP_Reply smtp_Reply;
				if (flag)
				{
					smtp_Reply = new SMTP_Reply(220, "<" + Net_Utils.GetLocalHostName(base.LocalHostName) + "> Simple Mail Transfer Service Ready.");
				}
				else
				{
					smtp_Reply = new SMTP_Reply(220, this.Server.GreetingText);
				}
				smtp_Reply = this.OnStarted(smtp_Reply);
				this.WriteLine(smtp_Reply.ToString());
				bool flag2 = smtp_Reply.ReplyCode >= 300;
				if (flag2)
				{
					this.m_SessionRejected = true;
				}
				this.BeginReadCmd();
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x0004BE1C File Offset: 0x0004AE1C
		protected override void OnError(Exception x)
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				bool flag = x == null;
				if (!flag)
				{
					try
					{
						this.LogAddText("Exception: " + x.Message);
						bool flag2 = x is IOException || x is SocketException;
						if (flag2)
						{
							this.Dispose();
						}
						else
						{
							base.OnError(x);
							try
							{
								this.WriteLine("500 Internal server error.");
							}
							catch
							{
								this.Dispose();
							}
						}
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x0004BEC8 File Offset: 0x0004AEC8
		protected override void OnTimeout()
		{
			try
			{
				bool flag = this.m_pMessageStream != null;
				if (flag)
				{
					this.OnMessageStoringCanceled();
				}
				this.WriteLine("421 Idle timeout, closing connection.");
			}
			catch
			{
			}
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x0004BF14 File Offset: 0x0004AF14
		private void BeginReadCmd()
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				try
				{
					SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
					readLineOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
					{
						bool flag2 = this.ProcessCmd(readLineOP);
						if (flag2)
						{
							this.BeginReadCmd();
						}
					};
					while (this.TcpStream.ReadLine(readLineOP, true))
					{
						bool flag = !this.ProcessCmd(readLineOP);
						if (flag)
						{
							break;
						}
					}
				}
				catch (Exception x)
				{
					this.OnError(x);
				}
			}
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x0004BFC0 File Offset: 0x0004AFC0
		private bool ProcessCmd(SmartStream.ReadLineAsyncOP op)
		{
			bool result = true;
			try
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					return false;
				}
				bool flag = op.Error != null;
				if (flag)
				{
					this.OnError(op.Error);
				}
				bool flag2 = op.BytesInBuffer == 0;
				if (flag2)
				{
					this.LogAddText("The remote host '" + this.RemoteEndPoint.ToString() + "' shut down socket.");
					this.Dispose();
					return false;
				}
				bool flag3 = this.Server.Logger != null;
				if (flag3)
				{
					this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)op.BytesInBuffer, op.LineUtf8, this.LocalEndPoint, this.RemoteEndPoint);
				}
				string[] array = Encoding.UTF8.GetString(op.Buffer, 0, op.LineBytesInBuffer).Split(new char[]
				{
					' '
				}, 2);
				string text = array[0].ToUpperInvariant();
				string cmdText = (array.Length == 2) ? array[1] : "";
				bool flag4 = text == "EHLO";
				if (flag4)
				{
					this.EHLO(cmdText);
				}
				else
				{
					bool flag5 = text == "HELO";
					if (flag5)
					{
						this.HELO(cmdText);
					}
					else
					{
						bool flag6 = text == "STARTTLS";
						if (flag6)
						{
							this.STARTTLS(cmdText);
						}
						else
						{
							bool flag7 = text == "AUTH";
							if (flag7)
							{
								this.AUTH(cmdText);
							}
							else
							{
								bool flag8 = text == "MAIL";
								if (flag8)
								{
									this.MAIL(cmdText);
								}
								else
								{
									bool flag9 = text == "RCPT";
									if (flag9)
									{
										this.RCPT(cmdText);
									}
									else
									{
										bool flag10 = text == "DATA";
										if (flag10)
										{
											SMTP_Session.Cmd_DATA cmdData = new SMTP_Session.Cmd_DATA();
											cmdData.CompletedAsync += delegate(object sender, EventArgs<SMTP_Session.Cmd_DATA> e)
											{
												bool flag19 = cmdData.Error != null;
												if (flag19)
												{
													bool flag20 = cmdData.Error is IncompleteDataException;
													if (flag20)
													{
														this.LogAddText("Disposing SMTP session, remote endpoint closed socket.");
													}
													else
													{
														this.LogAddText("Disposing SMTP session, fatal error:" + cmdData.Error.Message);
														this.OnError(cmdData.Error);
													}
													this.Dispose();
												}
												else
												{
													this.BeginReadCmd();
												}
												cmdData.Dispose();
											};
											bool flag11 = !cmdData.Start(this, cmdText);
											if (flag11)
											{
												bool flag12 = cmdData.Error != null;
												if (flag12)
												{
													bool flag13 = cmdData.Error is IncompleteDataException;
													if (flag13)
													{
														this.LogAddText("Disposing SMTP session, remote endpoint closed socket.");
													}
													else
													{
														this.LogAddText("Disposing SMTP session, fatal error:" + cmdData.Error.Message);
														this.OnError(cmdData.Error);
													}
													this.Dispose();
													result = false;
												}
												cmdData.Dispose();
											}
											else
											{
												result = false;
											}
										}
										else
										{
											bool flag14 = text == "BDAT";
											if (flag14)
											{
												result = this.BDAT(cmdText);
											}
											else
											{
												bool flag15 = text == "RSET";
												if (flag15)
												{
													this.RSET(cmdText);
												}
												else
												{
													bool flag16 = text == "NOOP";
													if (flag16)
													{
														this.NOOP(cmdText);
													}
													else
													{
														bool flag17 = text == "QUIT";
														if (flag17)
														{
															this.QUIT(cmdText);
															result = false;
														}
														else
														{
															this.m_BadCommands++;
															bool flag18 = this.Server.MaxBadCommands != 0 && this.m_BadCommands > this.Server.MaxBadCommands;
															if (flag18)
															{
																this.WriteLine("421 Too many bad commands, closing transmission channel.");
																this.Disconnect();
																return false;
															}
															this.WriteLine("502 Error: command '" + text + "' not recognized.");
														}
													}
												}
											}
										}
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
			return result;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x0004C3B0 File Offset: 0x0004B3B0
		private void ReadCommandAsync(SMTP_Session.ReadCommandAsyncOP op)
		{
			bool flag = op == null;
			if (flag)
			{
				throw new ArgumentNullException("op");
			}
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x0004C3D4 File Offset: 0x0004B3D4
		private void ReadCommandCompleted(SMTP_Session.ReadCommandAsyncOP op)
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				bool flag = op == null;
				if (flag)
				{
				}
			}
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x0004C3FC File Offset: 0x0004B3FC
		private bool SendResponseAsync(SMTP_Session.SendResponseAsyncOP op)
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

		// Token: 0x06000C76 RID: 3190 RVA: 0x0004C464 File Offset: 0x0004B464
		private void EHLO(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = string.IsNullOrEmpty(cmdText) || cmdText.Split(new char[]
				{
					' '
				}).Length != 1;
				if (flag)
				{
					this.WriteLine("501 Syntax error, syntax: \"EHLO\" SP hostname CRLF");
				}
				else
				{
					List<string> list = new List<string>();
					list.Add(Net_Utils.GetLocalHostName(base.LocalHostName));
					bool flag2 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.PIPELINING);
					if (flag2)
					{
						list.Add(SMTP_ServiceExtensions.PIPELINING);
					}
					bool flag3 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.SIZE);
					if (flag3)
					{
						list.Add(SMTP_ServiceExtensions.SIZE + " " + this.Server.MaxMessageSize);
					}
					bool flag4 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.STARTTLS) && !this.IsSecureConnection && base.Certificate != null;
					if (flag4)
					{
						list.Add(SMTP_ServiceExtensions.STARTTLS);
					}
					bool flag5 = this.Server.Extentions.Contains(SMTP_ServiceExtensions._8BITMIME);
					if (flag5)
					{
						list.Add(SMTP_ServiceExtensions._8BITMIME);
					}
					bool flag6 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.BINARYMIME);
					if (flag6)
					{
						list.Add(SMTP_ServiceExtensions.BINARYMIME);
					}
					bool flag7 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.CHUNKING);
					if (flag7)
					{
						list.Add(SMTP_ServiceExtensions.CHUNKING);
					}
					bool flag8 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.DSN);
					if (flag8)
					{
						list.Add(SMTP_ServiceExtensions.DSN);
					}
					StringBuilder stringBuilder = new StringBuilder();
					foreach (AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism in this.Authentications.Values)
					{
						bool flag9 = !auth_SASL_ServerMechanism.RequireSSL || (auth_SASL_ServerMechanism.RequireSSL && this.IsSecureConnection);
						if (flag9)
						{
							stringBuilder.Append(auth_SASL_ServerMechanism.Name + " ");
						}
					}
					bool flag10 = stringBuilder.Length > 0;
					if (flag10)
					{
						list.Add(SMTP_ServiceExtensions.AUTH + " " + stringBuilder.ToString().Trim());
					}
					SMTP_Reply smtp_Reply = new SMTP_Reply(250, list.ToArray());
					smtp_Reply = this.OnEhlo(cmdText, smtp_Reply);
					bool flag11 = smtp_Reply.ReplyCode < 300;
					if (flag11)
					{
						this.m_EhloHost = cmdText;
						this.Reset();
					}
					this.WriteLine(smtp_Reply.ToString());
				}
			}
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x0004C73C File Offset: 0x0004B73C
		private void HELO(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = string.IsNullOrEmpty(cmdText) || cmdText.Split(new char[]
				{
					' '
				}).Length != 1;
				if (flag)
				{
					this.WriteLine("501 Syntax error, syntax: \"HELO\" SP hostname CRLF");
				}
				else
				{
					SMTP_Reply smtp_Reply = new SMTP_Reply(250, Net_Utils.GetLocalHostName(base.LocalHostName));
					smtp_Reply = this.OnEhlo(cmdText, smtp_Reply);
					bool flag2 = smtp_Reply.ReplyCode < 300;
					if (flag2)
					{
						this.m_EhloHost = cmdText;
						this.Reset();
					}
					this.WriteLine(smtp_Reply.ToString());
				}
			}
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x0004C7EC File Offset: 0x0004B7EC
		private void STARTTLS(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !string.IsNullOrEmpty(cmdText);
				if (flag)
				{
					this.WriteLine("501 Syntax error: No parameters allowed.");
				}
				else
				{
					bool isSecureConnection = this.IsSecureConnection;
					if (isSecureConnection)
					{
						this.WriteLine("503 Bad sequence of commands: Connection is already secure.");
					}
					else
					{
						bool flag2 = base.Certificate == null;
						if (flag2)
						{
							this.WriteLine("454 TLS not available: Server has no SSL certificate.");
						}
						else
						{
							this.WriteLine("220 Ready to start TLS.");
							try
							{
								base.SwitchToSecure();
								this.LogAddText("TLS negotiation completed successfully.");
								this.m_EhloHost = null;
								this.Reset();
							}
							catch (Exception ex)
							{
								this.LogAddText("TLS negotiation failed: " + ex.Message + ".");
								this.Disconnect();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0004C8D8 File Offset: 0x0004B8D8
		private void AUTH(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.WriteLine("503 Bad sequence of commands: you are already authenticated.");
				}
				else
				{
					bool flag = this.m_pFrom != null;
					if (flag)
					{
						this.WriteLine("503 Bad sequence of commands: The AUTH command is not permitted during a mail transaction.");
					}
					else
					{
						string[] array = cmdText.Split(new char[]
						{
							' '
						});
						bool flag2 = array.Length > 2;
						if (flag2)
						{
							this.WriteLine("501 Syntax error, syntax: AUTH SP mechanism [SP initial-response] CRLF");
						}
						else
						{
							byte[] array2 = new byte[0];
							bool flag3 = array.Length == 2;
							if (flag3)
							{
								bool flag4 = array[1] == "=";
								if (!flag4)
								{
									try
									{
										array2 = Convert.FromBase64String(array[1]);
									}
									catch
									{
										this.WriteLine("501 Syntax error: Parameter 'initial-response' value must be BASE64 or contain a single character '='.");
										return;
									}
								}
							}
							string key = array[0];
							bool flag5 = !this.Authentications.ContainsKey(key);
							if (flag5)
							{
								this.WriteLine("501 Not supported authentication mechanism.");
							}
							else
							{
								byte[] array3 = array2;
								AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism = this.Authentications[key];
								auth_SASL_ServerMechanism.Reset();
								SmartStream.ReadLineAsyncOP readLineAsyncOP;
								for (;;)
								{
									byte[] array4 = auth_SASL_ServerMechanism.Continue(array3);
									bool isCompleted = auth_SASL_ServerMechanism.IsCompleted;
									if (isCompleted)
									{
										break;
									}
									bool flag6 = array4.Length == 0;
									if (flag6)
									{
										this.WriteLine("334 ");
									}
									else
									{
										this.WriteLine("334 " + Convert.ToBase64String(array4));
									}
									readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
									this.TcpStream.ReadLine(readLineAsyncOP, false);
									bool flag7 = readLineAsyncOP.Error != null;
									if (flag7)
									{
										goto Block_12;
									}
									bool flag8 = this.Server.Logger != null;
									if (flag8)
									{
										this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)readLineAsyncOP.BytesInBuffer, "base64 auth-data", this.LocalEndPoint, this.RemoteEndPoint);
									}
									bool flag9 = readLineAsyncOP.LineUtf8 == "*";
									if (flag9)
									{
										goto Block_14;
									}
									try
									{
										array3 = Convert.FromBase64String(readLineAsyncOP.LineUtf8);
									}
									catch
									{
										this.WriteLine("501 Invalid client response '" + array3 + "'.");
										return;
									}
								}
								bool isAuthenticated2 = auth_SASL_ServerMechanism.IsAuthenticated;
								if (isAuthenticated2)
								{
									this.m_pUser = new GenericIdentity(auth_SASL_ServerMechanism.UserName, "SASL-" + auth_SASL_ServerMechanism.Name);
									this.WriteLine("235 2.7.0 Authentication succeeded.");
								}
								else
								{
									this.WriteLine("535 5.7.8 Authentication credentials invalid.");
								}
								return;
								Block_12:
								throw readLineAsyncOP.Error;
								Block_14:
								this.WriteLine("501 Authentication canceled.");
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0004CBAC File Offset: 0x0004BBAC
		private void MAIL(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = string.IsNullOrEmpty(this.m_EhloHost);
				if (flag)
				{
					this.WriteLine("503 Bad sequence of commands: send EHLO/HELO first.");
				}
				else
				{
					bool flag2 = this.m_pFrom != null;
					if (flag2)
					{
						this.WriteLine("503 Bad sequence of commands: nested MAIL command.");
					}
					else
					{
						bool flag3 = this.m_pMessageStream != null;
						if (flag3)
						{
							this.WriteLine("503 Bad sequence of commands: BDAT command is pending.");
						}
						else
						{
							bool flag4 = this.Server.MaxTransactions != 0 && this.m_Transactions >= this.Server.MaxTransactions;
							if (flag4)
							{
								this.WriteLine("503 Bad sequence of commands: Maximum allowed mail transactions exceeded.");
							}
							else
							{
								bool flag5 = cmdText.ToUpper().StartsWith("FROM:");
								if (flag5)
								{
									cmdText = cmdText.Substring(5).Trim();
									int num = -1;
									string body = null;
									SMTP_DSN_Ret ret = SMTP_DSN_Ret.NotSpecified;
									string envid = null;
									bool flag6 = !cmdText.StartsWith("<") || cmdText.IndexOf('>') == -1;
									if (flag6)
									{
										this.WriteLine("501 Syntax error, syntax: \"MAIL FROM:\" \"<\" address \">\" / \"<>\" [SP Mail-parameters] CRLF");
									}
									else
									{
										string mailbox = cmdText.Substring(1, cmdText.IndexOf('>') - 1).Trim();
										cmdText = cmdText.Substring(cmdText.IndexOf('>') + 1).Trim();
										string[] array = string.IsNullOrEmpty(cmdText) ? new string[0] : cmdText.Split(new char[]
										{
											' '
										});
										foreach (string text in array)
										{
											string[] array3 = text.Split(new char[]
											{
												'='
											}, 2);
											bool flag7 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.SIZE) && array3[0].ToUpper() == "SIZE";
											if (flag7)
											{
												bool flag8 = array3.Length == 1;
												if (flag8)
												{
													this.WriteLine("501 Syntax error: SIZE parameter value must be specified.");
													return;
												}
												bool flag9 = !int.TryParse(array3[1], out num);
												if (flag9)
												{
													this.WriteLine("501 Syntax error: SIZE parameter value must be integer.");
													return;
												}
												bool flag10 = num > this.Server.MaxMessageSize;
												if (flag10)
												{
													this.WriteLine("552 Message exceeds fixed maximum message size.");
													return;
												}
											}
											else
											{
												bool flag11 = this.Server.Extentions.Contains(SMTP_ServiceExtensions._8BITMIME) && array3[0].ToUpper() == "BODY";
												if (flag11)
												{
													bool flag12 = array3.Length == 1;
													if (flag12)
													{
														this.WriteLine("501 Syntax error: BODY parameter value must be specified.");
														return;
													}
													bool flag13 = array3[1].ToUpper() != "7BIT" && array3[1].ToUpper() != "8BITMIME" && array3[1].ToUpper() != "BINARYMIME";
													if (flag13)
													{
														this.WriteLine("501 Syntax error: BODY parameter value must be \"7BIT\",\"8BITMIME\" or \"BINARYMIME\".");
														return;
													}
													body = array3[1].ToUpper();
												}
												else
												{
													bool flag14 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.DSN) && array3[0].ToUpper() == "RET";
													if (flag14)
													{
														bool flag15 = array3.Length == 1;
														if (flag15)
														{
															this.WriteLine("501 Syntax error: RET parameter value must be specified.");
															return;
														}
														bool flag16 = array3[1].ToUpper() != "FULL";
														if (flag16)
														{
															ret = SMTP_DSN_Ret.FullMessage;
														}
														else
														{
															bool flag17 = array3[1].ToUpper() != "HDRS";
															if (!flag17)
															{
																this.WriteLine("501 Syntax error: RET parameter value must be \"FULL\" or \"HDRS\".");
																return;
															}
															ret = SMTP_DSN_Ret.Headers;
														}
													}
													else
													{
														bool flag18 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.DSN) && array3[0].ToUpper() == "ENVID";
														if (flag18)
														{
															bool flag19 = array3.Length == 1;
															if (flag19)
															{
																this.WriteLine("501 Syntax error: ENVID parameter value must be specified.");
																return;
															}
															envid = array3[1].ToUpper();
														}
														else
														{
															bool flag20 = array3[0].ToUpper() == "AUTH";
															if (!flag20)
															{
																this.WriteLine("555 Unsupported parameter: " + text);
																return;
															}
														}
													}
												}
											}
										}
										SMTP_MailFrom smtp_MailFrom = new SMTP_MailFrom(mailbox, num, body, ret, envid);
										SMTP_Reply smtp_Reply = new SMTP_Reply(250, "OK.");
										smtp_Reply = this.OnMailFrom(smtp_MailFrom, smtp_Reply);
										bool flag21 = smtp_Reply.ReplyCode < 300;
										if (flag21)
										{
											this.m_pFrom = smtp_MailFrom;
											this.m_Transactions++;
										}
										this.WriteLine(smtp_Reply.ToString());
									}
								}
								else
								{
									this.WriteLine("501 Syntax error, syntax: \"MAIL FROM:\" \"<\" address \">\" / \"<>\" [SP Mail-parameters] CRLF");
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0004D08C File Offset: 0x0004C08C
		private void RCPT(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = string.IsNullOrEmpty(this.m_EhloHost);
				if (flag)
				{
					this.WriteLine("503 Bad sequence of commands: send EHLO/HELO first.");
				}
				else
				{
					bool flag2 = this.m_pFrom == null;
					if (flag2)
					{
						this.WriteLine("503 Bad sequence of commands: send 'MAIL FROM:' first.");
					}
					else
					{
						bool flag3 = this.m_pMessageStream != null;
						if (flag3)
						{
							this.WriteLine("503 Bad sequence of commands: BDAT command is pending.");
						}
						else
						{
							bool flag4 = cmdText.ToUpper().StartsWith("TO:");
							if (flag4)
							{
								cmdText = cmdText.Substring(3).Trim();
								SMTP_DSN_Notify smtp_DSN_Notify = SMTP_DSN_Notify.NotSpecified;
								string orcpt = null;
								bool flag5 = !cmdText.StartsWith("<") || cmdText.IndexOf('>') == -1;
								if (flag5)
								{
									this.WriteLine("501 Syntax error, syntax: \"RCPT TO:\" \"<\" address \">\" [SP Rcpt-parameters] CRLF");
								}
								else
								{
									string text = cmdText.Substring(1, cmdText.IndexOf('>') - 1).Trim();
									cmdText = cmdText.Substring(cmdText.IndexOf('>') + 1).Trim();
									bool flag6 = text == string.Empty;
									if (flag6)
									{
										this.WriteLine("501 Syntax error('address' value must be specified), syntax: \"RCPT TO:\" \"<\" address \">\" [SP Rcpt-parameters] CRLF");
									}
									else
									{
										string[] array = string.IsNullOrEmpty(cmdText) ? new string[0] : cmdText.Split(new char[]
										{
											' '
										});
										foreach (string text2 in array)
										{
											string[] array3 = text2.Split(new char[]
											{
												'='
											}, 2);
											bool flag7 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.DSN) && array3[0].ToUpper() == "NOTIFY";
											if (flag7)
											{
												bool flag8 = array3.Length == 1;
												if (flag8)
												{
													this.WriteLine("501 Syntax error: NOTIFY parameter value must be specified.");
													return;
												}
												string[] array4 = array3[1].ToUpper().Split(new char[]
												{
													','
												});
												foreach (string text3 in array4)
												{
													bool flag9 = text3.Trim().ToUpper() == "NEVER";
													if (flag9)
													{
														smtp_DSN_Notify |= SMTP_DSN_Notify.Never;
													}
													else
													{
														bool flag10 = text3.Trim().ToUpper() == "SUCCESS";
														if (flag10)
														{
															smtp_DSN_Notify |= SMTP_DSN_Notify.Success;
														}
														else
														{
															bool flag11 = text3.Trim().ToUpper() == "FAILURE";
															if (flag11)
															{
																smtp_DSN_Notify |= SMTP_DSN_Notify.Failure;
															}
															else
															{
																bool flag12 = text3.Trim().ToUpper() == "DELAY";
																if (!flag12)
																{
																	this.WriteLine("501 Syntax error: Not supported NOTIFY parameter value '" + text3 + "'.");
																	return;
																}
																smtp_DSN_Notify |= SMTP_DSN_Notify.Delay;
															}
														}
													}
												}
											}
											else
											{
												bool flag13 = this.Server.Extentions.Contains(SMTP_ServiceExtensions.DSN) && array3[0].ToUpper() == "ORCPT";
												if (flag13)
												{
													bool flag14 = array3.Length == 1;
													if (flag14)
													{
														this.WriteLine("501 Syntax error: ORCPT parameter value must be specified.");
														return;
													}
													orcpt = array3[1].ToUpper();
												}
												else
												{
													this.WriteLine("555 Unsupported parameter: " + text2);
												}
											}
										}
										bool flag15 = this.m_pTo.Count >= this.Server.MaxRecipients;
										if (flag15)
										{
											this.WriteLine("452 Too many recipients");
										}
										else
										{
											SMTP_RcptTo smtp_RcptTo = new SMTP_RcptTo(text, smtp_DSN_Notify, orcpt);
											SMTP_Reply smtp_Reply = new SMTP_Reply(250, "OK.");
											smtp_Reply = this.OnRcptTo(smtp_RcptTo, smtp_Reply);
											bool flag16 = smtp_Reply.ReplyCode < 300;
											if (flag16)
											{
												bool flag17 = !this.m_pTo.ContainsKey(text.ToLower());
												if (flag17)
												{
													this.m_pTo.Add(text.ToLower(), smtp_RcptTo);
												}
											}
											this.WriteLine(smtp_Reply.ToString());
										}
									}
								}
							}
							else
							{
								this.WriteLine("501 Syntax error, syntax: \"RCPT TO:\" \"<\" address \">\" [SP Rcpt-parameters] CRLF");
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x0004D4B4 File Offset: 0x0004C4B4
		private bool DATA(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			bool result;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
				result = true;
			}
			else
			{
				bool flag = string.IsNullOrEmpty(this.m_EhloHost);
				if (flag)
				{
					this.WriteLine("503 Bad sequence of commands: send EHLO/HELO first.");
					result = true;
				}
				else
				{
					bool flag2 = this.m_pFrom == null;
					if (flag2)
					{
						this.WriteLine("503 Bad sequence of commands: send 'MAIL FROM:' first.");
						result = true;
					}
					else
					{
						bool flag3 = this.m_pTo.Count == 0;
						if (flag3)
						{
							this.WriteLine("503 Bad sequence of commands: send 'RCPT TO:' first.");
							result = true;
						}
						else
						{
							bool flag4 = this.m_pMessageStream != null;
							if (flag4)
							{
								this.WriteLine("503 Bad sequence of commands: DATA and BDAT commands cannot be used in the same transaction.");
								result = true;
							}
							else
							{
								DateTime startTime = DateTime.Now;
								this.m_pMessageStream = this.OnGetMessageStream();
								bool flag5 = this.m_pMessageStream == null;
								if (flag5)
								{
									this.m_pMessageStream = new MemoryStreamEx(32000);
								}
								byte[] array = this.CreateReceivedHeader();
								this.m_pMessageStream.Write(array, 0, array.Length);
								this.WriteLine("354 Start mail input; end with <CRLF>.<CRLF>");
								SmartStream.ReadPeriodTerminatedAsyncOP readPeriodTermOP = new SmartStream.ReadPeriodTerminatedAsyncOP(this.m_pMessageStream, (long)this.Server.MaxMessageSize, SizeExceededAction.JunkAndThrowException);
								readPeriodTermOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP> e)
								{
									this.DATA_End(startTime, readPeriodTermOP);
								};
								bool flag6 = this.TcpStream.ReadPeriodTerminated(readPeriodTermOP, true);
								if (flag6)
								{
									this.DATA_End(startTime, readPeriodTermOP);
									result = true;
								}
								else
								{
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x0004D648 File Offset: 0x0004C648
		private void DATA_End(DateTime startTime, SmartStream.ReadPeriodTerminatedAsyncOP op)
		{
			try
			{
				bool flag = op.Error != null;
				if (flag)
				{
					bool flag2 = op.Error is LineSizeExceededException;
					if (flag2)
					{
						this.WriteLine("500 Line too long.");
					}
					else
					{
						bool flag3 = op.Error is DataSizeExceededException;
						if (flag3)
						{
							this.WriteLine("552 Too much mail data.");
						}
						else
						{
							this.OnError(op.Error);
						}
					}
					this.OnMessageStoringCanceled();
				}
				else
				{
					SMTP_Reply smtp_Reply = new SMTP_Reply(250, "DATA completed in " + (DateTime.Now - startTime).TotalSeconds.ToString("f2") + " seconds.");
					smtp_Reply = this.OnMessageStoringCompleted(smtp_Reply);
					this.WriteLine(smtp_Reply.ToString());
				}
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
			this.Reset();
			this.BeginReadCmd();
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x0004D748 File Offset: 0x0004C748
		private bool BDAT(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			bool result;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
				result = true;
			}
			else
			{
				bool flag = string.IsNullOrEmpty(this.m_EhloHost);
				if (flag)
				{
					this.WriteLine("503 Bad sequence of commands: send EHLO/HELO first.");
					result = true;
				}
				else
				{
					bool flag2 = this.m_pFrom == null;
					if (flag2)
					{
						this.WriteLine("503 Bad sequence of commands: send 'MAIL FROM:' first.");
						result = true;
					}
					else
					{
						bool flag3 = this.m_pTo.Count == 0;
						if (flag3)
						{
							this.WriteLine("503 Bad sequence of commands: send 'RCPT TO:' first.");
							result = true;
						}
						else
						{
							DateTime startTime = DateTime.Now;
							int chunkSize = 0;
							bool last = false;
							string[] array = cmdText.Split(new char[]
							{
								' '
							});
							bool flag4 = cmdText == string.Empty || array.Length > 2;
							if (flag4)
							{
								this.WriteLine("501 Syntax error, syntax: \"BDAT\" SP chunk-size [SP \"LAST\"] CRLF");
								result = true;
							}
							else
							{
								bool flag5 = !int.TryParse(array[0], out chunkSize);
								if (flag5)
								{
									this.WriteLine("501 Syntax error(chunk-size must be integer), syntax: \"BDAT\" SP chunk-size [SP \"LAST\"] CRLF");
									result = true;
								}
								else
								{
									bool flag6 = array.Length == 2;
									if (flag6)
									{
										bool flag7 = array[1].ToUpperInvariant() != "LAST";
										if (flag7)
										{
											this.WriteLine("501 Syntax error, syntax: \"BDAT\" SP chunk-size [SP \"LAST\"] CRLF");
											return true;
										}
										last = true;
									}
									bool flag8 = this.m_pMessageStream == null;
									if (flag8)
									{
										this.m_pMessageStream = this.OnGetMessageStream();
										bool flag9 = this.m_pMessageStream == null;
										if (flag9)
										{
											this.m_pMessageStream = new MemoryStreamEx(32000);
										}
										byte[] array2 = this.CreateReceivedHeader();
										this.m_pMessageStream.Write(array2, 0, array2.Length);
									}
									Stream storeStream = this.m_pMessageStream;
									bool flag10 = this.m_BDatReadedCount + chunkSize > this.Server.MaxMessageSize;
									if (flag10)
									{
										storeStream = new JunkingStream();
									}
									this.TcpStream.BeginReadFixedCount(storeStream, (long)chunkSize, delegate(IAsyncResult ar)
									{
										try
										{
											this.TcpStream.EndReadFixedCount(ar);
											this.m_BDatReadedCount += chunkSize;
											bool flag11 = this.m_BDatReadedCount > this.Server.MaxMessageSize;
											bool last;
											if (flag11)
											{
												this.WriteLine("552 Too much mail data.");
												this.OnMessageStoringCanceled();
											}
											else
											{
												SMTP_Reply smtp_Reply = new SMTP_Reply(250, string.Concat(new object[]
												{
													chunkSize,
													" bytes received in ",
													(DateTime.Now - startTime).TotalSeconds.ToString("f2"),
													" seconds."
												}));
												last = last;
												if (last)
												{
													smtp_Reply = this.OnMessageStoringCompleted(smtp_Reply);
												}
												this.WriteLine(smtp_Reply.ToString());
											}
											bool last2 = last;
											if (last2)
											{
												this.Reset();
											}
										}
										catch (Exception x)
										{
											this.OnError(x);
										}
										this.BeginReadCmd();
									}, null);
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x0004D970 File Offset: 0x0004C970
		private void RSET(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = this.m_pMessageStream != null;
				if (flag)
				{
					this.OnMessageStoringCanceled();
				}
				this.Reset();
				this.WriteLine("250 OK.");
			}
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0004D9C0 File Offset: 0x0004C9C0
		private void NOOP(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("503 bad sequence of commands: Session rejected.");
			}
			else
			{
				this.WriteLine("250 OK.");
			}
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0004D9F4 File Offset: 0x0004C9F4
		private void QUIT(string cmdText)
		{
			try
			{
				this.WriteLine("221 <" + Net_Utils.GetLocalHostName(base.LocalHostName) + "> Service closing transmission channel.");
			}
			catch
			{
			}
			this.Disconnect();
			this.Dispose();
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x0004DA4C File Offset: 0x0004CA4C
		private void Reset()
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				this.m_pFrom = null;
				this.m_pTo.Clear();
				this.m_pMessageStream = null;
				this.m_BDatReadedCount = 0;
			}
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0004DA88 File Offset: 0x0004CA88
		private byte[] CreateReceivedHeader()
		{
			Mail_h_Received mail_h_Received = new Mail_h_Received(this.EhloHost, Net_Utils.GetLocalHostName(base.LocalHostName), DateTime.Now);
			mail_h_Received.From_TcpInfo = new Mail_t_TcpInfo(this.RemoteEndPoint.Address, null);
			mail_h_Received.Via = "TCP";
			bool flag = !base.IsAuthenticated && !this.IsSecureConnection;
			if (flag)
			{
				mail_h_Received.With = "ESMTP";
			}
			else
			{
				bool flag2 = base.IsAuthenticated && !this.IsSecureConnection;
				if (flag2)
				{
					mail_h_Received.With = "ESMTPA";
				}
				else
				{
					bool flag3 = !base.IsAuthenticated && this.IsSecureConnection;
					if (flag3)
					{
						mail_h_Received.With = "ESMTPS";
					}
					else
					{
						bool flag4 = base.IsAuthenticated && this.IsSecureConnection;
						if (flag4)
						{
							mail_h_Received.With = "ESMTPSA";
						}
					}
				}
			}
			return Encoding.UTF8.GetBytes(mail_h_Received.ToString());
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0004DB88 File Offset: 0x0004CB88
		private void WriteLine(string line)
		{
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			int num = this.TcpStream.WriteLine(line);
			bool flag2 = this.Server.Logger != null;
			if (flag2)
			{
				this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, (long)num, line, this.LocalEndPoint, this.RemoteEndPoint);
			}
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0004DBF8 File Offset: 0x0004CBF8
		public void LogAddRead(long size, string text)
		{
			try
			{
				bool flag = this.Server.Logger != null;
				if (flag)
				{
					this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, size, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0004DC60 File Offset: 0x0004CC60
		public void LogAddWrite(long size, string text)
		{
			try
			{
				bool flag = this.Server.Logger != null;
				if (flag)
				{
					this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, size, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0004DCC8 File Offset: 0x0004CCC8
		public void LogAddText(string text)
		{
			try
			{
				bool flag = this.Server.Logger != null;
				if (flag)
				{
					this.Server.Logger.AddText(this.IsConnected ? this.ID : "", this.IsConnected ? this.AuthenticatedUserIdentity : null, text, this.IsConnected ? this.LocalEndPoint : null, this.IsConnected ? this.RemoteEndPoint : null);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0004DD60 File Offset: 0x0004CD60
		public void LogAddException(string text, Exception x)
		{
			try
			{
				bool flag = this.Server.Logger != null;
				if (flag)
				{
					this.Server.Logger.AddException(this.IsConnected ? this.ID : "", this.IsConnected ? this.AuthenticatedUserIdentity : null, text, this.IsConnected ? this.LocalEndPoint : null, this.IsConnected ? this.RemoteEndPoint : null, x);
				}
			}
			catch
			{
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000C89 RID: 3209 RVA: 0x0004DDF8 File Offset: 0x0004CDF8
		public new SMTP_Server Server
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (SMTP_Server)base.Server;
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x0004DE34 File Offset: 0x0004CE34
		public Dictionary<string, AUTH_SASL_ServerMechanism> Authentications
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pAuthentications;
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000C8B RID: 3211 RVA: 0x0004DE68 File Offset: 0x0004CE68
		public int BadCommands
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_BadCommands;
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x0004DE9C File Offset: 0x0004CE9C
		public int Transactions
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Transactions;
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x0004DED0 File Offset: 0x0004CED0
		public string EhloHost
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_EhloHost;
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x0004DF04 File Offset: 0x0004CF04
		public override GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pUser;
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x0004DF38 File Offset: 0x0004CF38
		public SMTP_MailFrom From
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pFrom;
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x0004DF6C File Offset: 0x0004CF6C
		public SMTP_RcptTo[] To
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				Dictionary<string, SMTP_RcptTo> pTo = this.m_pTo;
				SMTP_RcptTo[] result;
				lock (pTo)
				{
					SMTP_RcptTo[] array = new SMTP_RcptTo[this.m_pTo.Count];
					this.m_pTo.Values.CopyTo(array, 0);
					result = array;
				}
				return result;
			}
		}

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06000C91 RID: 3217 RVA: 0x0004DFF0 File Offset: 0x0004CFF0
		// (remove) Token: 0x06000C92 RID: 3218 RVA: 0x0004E028 File Offset: 0x0004D028
		
		public event EventHandler<SMTP_e_Started> Started = null;

		// Token: 0x06000C93 RID: 3219 RVA: 0x0004E060 File Offset: 0x0004D060
		private SMTP_Reply OnStarted(SMTP_Reply reply)
		{
			bool flag = this.Started != null;
			SMTP_Reply result;
			if (flag)
			{
				SMTP_e_Started smtp_e_Started = new SMTP_e_Started(this, reply);
				this.Started(this, smtp_e_Started);
				result = smtp_e_Started.Reply;
			}
			else
			{
				result = reply;
			}
			return result;
		}

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06000C94 RID: 3220 RVA: 0x0004E0A0 File Offset: 0x0004D0A0
		// (remove) Token: 0x06000C95 RID: 3221 RVA: 0x0004E0D8 File Offset: 0x0004D0D8
		
		public event EventHandler<SMTP_e_Ehlo> Ehlo = null;

		// Token: 0x06000C96 RID: 3222 RVA: 0x0004E110 File Offset: 0x0004D110
		private SMTP_Reply OnEhlo(string domain, SMTP_Reply reply)
		{
			bool flag = this.Ehlo != null;
			SMTP_Reply result;
			if (flag)
			{
				SMTP_e_Ehlo smtp_e_Ehlo = new SMTP_e_Ehlo(this, domain, reply);
				this.Ehlo(this, smtp_e_Ehlo);
				result = smtp_e_Ehlo.Reply;
			}
			else
			{
				result = reply;
			}
			return result;
		}

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06000C97 RID: 3223 RVA: 0x0004E154 File Offset: 0x0004D154
		// (remove) Token: 0x06000C98 RID: 3224 RVA: 0x0004E18C File Offset: 0x0004D18C
		
		public event EventHandler<SMTP_e_MailFrom> MailFrom = null;

		// Token: 0x06000C99 RID: 3225 RVA: 0x0004E1C4 File Offset: 0x0004D1C4
		private SMTP_Reply OnMailFrom(SMTP_MailFrom from, SMTP_Reply reply)
		{
			bool flag = this.MailFrom != null;
			SMTP_Reply result;
			if (flag)
			{
				SMTP_e_MailFrom smtp_e_MailFrom = new SMTP_e_MailFrom(this, from, reply);
				this.MailFrom(this, smtp_e_MailFrom);
				result = smtp_e_MailFrom.Reply;
			}
			else
			{
				result = reply;
			}
			return result;
		}

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06000C9A RID: 3226 RVA: 0x0004E208 File Offset: 0x0004D208
		// (remove) Token: 0x06000C9B RID: 3227 RVA: 0x0004E240 File Offset: 0x0004D240
		
		public event EventHandler<SMTP_e_RcptTo> RcptTo = null;

		// Token: 0x06000C9C RID: 3228 RVA: 0x0004E278 File Offset: 0x0004D278
		private SMTP_Reply OnRcptTo(SMTP_RcptTo to, SMTP_Reply reply)
		{
			bool flag = this.RcptTo != null;
			SMTP_Reply result;
			if (flag)
			{
				SMTP_e_RcptTo smtp_e_RcptTo = new SMTP_e_RcptTo(this, to, reply);
				this.RcptTo(this, smtp_e_RcptTo);
				result = smtp_e_RcptTo.Reply;
			}
			else
			{
				result = reply;
			}
			return result;
		}

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06000C9D RID: 3229 RVA: 0x0004E2BC File Offset: 0x0004D2BC
		// (remove) Token: 0x06000C9E RID: 3230 RVA: 0x0004E2F4 File Offset: 0x0004D2F4
		
		public event EventHandler<SMTP_e_Message> GetMessageStream = null;

		// Token: 0x06000C9F RID: 3231 RVA: 0x0004E32C File Offset: 0x0004D32C
		private Stream OnGetMessageStream()
		{
			bool flag = this.GetMessageStream != null;
			Stream result;
			if (flag)
			{
				SMTP_e_Message smtp_e_Message = new SMTP_e_Message(this);
				this.GetMessageStream(this, smtp_e_Message);
				result = smtp_e_Message.Stream;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x06000CA0 RID: 3232 RVA: 0x0004E36C File Offset: 0x0004D36C
		// (remove) Token: 0x06000CA1 RID: 3233 RVA: 0x0004E3A4 File Offset: 0x0004D3A4
		
		public event EventHandler MessageStoringCanceled = null;

		// Token: 0x06000CA2 RID: 3234 RVA: 0x0004E3DC File Offset: 0x0004D3DC
		private void OnMessageStoringCanceled()
		{
			bool flag = this.MessageStoringCanceled != null;
			if (flag)
			{
				this.MessageStoringCanceled(this, new EventArgs());
			}
		}

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x06000CA3 RID: 3235 RVA: 0x0004E40C File Offset: 0x0004D40C
		// (remove) Token: 0x06000CA4 RID: 3236 RVA: 0x0004E444 File Offset: 0x0004D444
		
		public event EventHandler<SMTP_e_MessageStored> MessageStoringCompleted = null;

		// Token: 0x06000CA5 RID: 3237 RVA: 0x0004E47C File Offset: 0x0004D47C
		private SMTP_Reply OnMessageStoringCompleted(SMTP_Reply reply)
		{
			bool flag = this.MessageStoringCompleted != null;
			SMTP_Reply result;
			if (flag)
			{
				SMTP_e_MessageStored smtp_e_MessageStored = new SMTP_e_MessageStored(this, this.m_pMessageStream, reply);
				this.MessageStoringCompleted(this, smtp_e_MessageStored);
				result = smtp_e_MessageStored.Reply;
			}
			else
			{
				result = reply;
			}
			return result;
		}

		// Token: 0x04000549 RID: 1353
		private Dictionary<string, AUTH_SASL_ServerMechanism> m_pAuthentications = null;

		// Token: 0x0400054A RID: 1354
		private int m_BadCommands = 0;

		// Token: 0x0400054B RID: 1355
		private int m_Transactions = 0;

		// Token: 0x0400054C RID: 1356
		private bool m_SessionRejected = false;

		// Token: 0x0400054D RID: 1357
		private string m_EhloHost = null;

		// Token: 0x0400054E RID: 1358
		private GenericIdentity m_pUser = null;

		// Token: 0x0400054F RID: 1359
		private SMTP_MailFrom m_pFrom = null;

		// Token: 0x04000550 RID: 1360
		private Dictionary<string, SMTP_RcptTo> m_pTo = null;

		// Token: 0x04000551 RID: 1361
		private Stream m_pMessageStream = null;

		// Token: 0x04000552 RID: 1362
		private int m_BDatReadedCount = 0;

		// Token: 0x020002D5 RID: 725
		private class ReadCommandAsyncOP
		{
		}

		// Token: 0x020002D6 RID: 726
		private class SendResponseAsyncOP : IDisposable, IAsyncOP
		{
			// Token: 0x060018EA RID: 6378 RVA: 0x00099EAC File Offset: 0x00098EAC
			public SendResponseAsyncOP(SMTP_t_ReplyLine reply)
			{
				bool flag = reply == null;
				if (flag)
				{
					throw new ArgumentNullException("reply");
				}
				this.m_pReplyLines = new SMTP_t_ReplyLine[]
				{
					reply
				};
			}

			// Token: 0x060018EB RID: 6379 RVA: 0x00099F1C File Offset: 0x00098F1C
			public SendResponseAsyncOP(SMTP_t_ReplyLine[] replyLines)
			{
				bool flag = replyLines == null;
				if (flag)
				{
					throw new ArgumentNullException("replyLines");
				}
				bool flag2 = replyLines.Length < 1;
				if (flag2)
				{
					throw new ArgumentException("Argument 'replyLines' must contain at least 1 item.", "replyLines");
				}
				this.m_pReplyLines = replyLines;
			}

			// Token: 0x060018EC RID: 6380 RVA: 0x00099F9C File Offset: 0x00098F9C
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pReplyLines = null;
					this.m_pSession = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060018ED RID: 6381 RVA: 0x00099FE0 File Offset: 0x00098FE0
			public bool Start(SMTP_Session owner)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSession = owner;
				this.SetState(AsyncOP_State.Active);
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (SMTP_t_ReplyLine smtp_t_ReplyLine in this.m_pReplyLines)
					{
						stringBuilder.Append(smtp_t_ReplyLine.ToString());
					}
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					this.m_pSession.LogAddWrite((long)bytes.Length, stringBuilder.ToString());
					this.m_pSession.TcpStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(this.ResponseSendingCompleted), null);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pSession.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
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

			// Token: 0x060018EE RID: 6382 RVA: 0x0009A128 File Offset: 0x00099128
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

			// Token: 0x060018EF RID: 6383 RVA: 0x0009A1A0 File Offset: 0x000991A0
			private void ResponseSendingCompleted(IAsyncResult ar)
			{
				try
				{
					this.m_pSession.TcpStream.EndWrite(ar);
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pSession.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
				}
				this.SetState(AsyncOP_State.Completed);
			}

			// Token: 0x17000820 RID: 2080
			// (get) Token: 0x060018F0 RID: 6384 RVA: 0x0009A210 File Offset: 0x00099210
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000821 RID: 2081
			// (get) Token: 0x060018F1 RID: 6385 RVA: 0x0009A228 File Offset: 0x00099228
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

			// Token: 0x140000B6 RID: 182
			// (add) Token: 0x060018F2 RID: 6386 RVA: 0x0009A27C File Offset: 0x0009927C
			// (remove) Token: 0x060018F3 RID: 6387 RVA: 0x0009A2B4 File Offset: 0x000992B4
			
			public event EventHandler<EventArgs<SMTP_Session.SendResponseAsyncOP>> CompletedAsync = null;

			// Token: 0x060018F4 RID: 6388 RVA: 0x0009A2EC File Offset: 0x000992EC
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Session.SendResponseAsyncOP>(this));
				}
			}

			// Token: 0x04000ACE RID: 2766
			private object m_pLock = new object();

			// Token: 0x04000ACF RID: 2767
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000AD0 RID: 2768
			private Exception m_pException = null;

			// Token: 0x04000AD1 RID: 2769
			private SMTP_t_ReplyLine[] m_pReplyLines = null;

			// Token: 0x04000AD2 RID: 2770
			private SMTP_Session m_pSession = null;

			// Token: 0x04000AD3 RID: 2771
			private bool m_RiseCompleted = false;
		}

		// Token: 0x020002D7 RID: 727
		private class Cmd_DATA : IDisposable, IAsyncOP
		{
			// Token: 0x060018F6 RID: 6390 RVA: 0x0009A354 File Offset: 0x00099354
			public void Dispose()
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					this.SetState(AsyncOP_State.Disposed);
					this.m_pException = null;
					this.m_pSession = null;
					this.CompletedAsync = null;
				}
			}

			// Token: 0x060018F7 RID: 6391 RVA: 0x0009A390 File Offset: 0x00099390
			public bool Start(SMTP_Session owner, string cmdText)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				this.m_pSession = owner;
				this.m_StartTime = DateTime.Now;
				this.SetState(AsyncOP_State.Active);
				try
				{
					bool sessionRejected = this.m_pSession.m_SessionRejected;
					if (sessionRejected)
					{
						this.SendFinalResponse(new SMTP_t_ReplyLine(503, "Bad sequence of commands: Session rejected.", true));
					}
					else
					{
						bool flag2 = string.IsNullOrEmpty(this.m_pSession.m_EhloHost);
						if (flag2)
						{
							this.SendFinalResponse(new SMTP_t_ReplyLine(503, "Bad sequence of commands: Send EHLO/HELO first.", true));
						}
						else
						{
							bool flag3 = this.m_pSession.m_pFrom == null;
							if (flag3)
							{
								this.SendFinalResponse(new SMTP_t_ReplyLine(503, "Bad sequence of commands: Send 'MAIL FROM:' first.", true));
							}
							else
							{
								bool flag4 = this.m_pSession.m_pTo.Count == 0;
								if (flag4)
								{
									this.SendFinalResponse(new SMTP_t_ReplyLine(503, "Bad sequence of commands: Send 'RCPT TO:' first.", true));
								}
								else
								{
									bool flag5 = !string.IsNullOrEmpty(cmdText);
									if (flag5)
									{
										this.SendFinalResponse(new SMTP_t_ReplyLine(500, "Command line syntax error.", true));
									}
									else
									{
										this.m_pSession.m_pMessageStream = this.m_pSession.OnGetMessageStream();
										bool flag6 = this.m_pSession.m_pMessageStream == null;
										if (flag6)
										{
											this.m_pSession.m_pMessageStream = new MemoryStreamEx(32000);
										}
										SMTP_Session.SendResponseAsyncOP sendResponseOP = new SMTP_Session.SendResponseAsyncOP(new SMTP_t_ReplyLine(354, "Start mail input; end with <CRLF>.<CRLF>", true));
										sendResponseOP.CompletedAsync += delegate(object sender, EventArgs<SMTP_Session.SendResponseAsyncOP> e)
										{
											this.Send354ResponseCompleted(sendResponseOP);
										};
										bool flag7 = !this.m_pSession.SendResponseAsync(sendResponseOP);
										if (flag7)
										{
											this.Send354ResponseCompleted(sendResponseOP);
										}
									}
								}
							}
						}
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pSession.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
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

			// Token: 0x060018F8 RID: 6392 RVA: 0x0009A620 File Offset: 0x00099620
			private void SetState(AsyncOP_State state)
			{
				bool flag = this.m_State == AsyncOP_State.Disposed;
				if (!flag)
				{
					object pLock = this.m_pLock;
					lock (pLock)
					{
						this.m_State = state;
						bool flag3 = this.m_State == AsyncOP_State.Completed;
						if (flag3)
						{
							this.m_pSession.Reset();
						}
						bool flag4 = this.m_State == AsyncOP_State.Completed && this.m_RiseCompleted;
						if (flag4)
						{
							this.OnCompletedAsync();
						}
					}
				}
			}

			// Token: 0x060018F9 RID: 6393 RVA: 0x0009A6B4 File Offset: 0x000996B4
			private void SendFinalResponse(SMTP_t_ReplyLine reply)
			{
				try
				{
					bool flag = reply == null;
					if (flag)
					{
						throw new ArgumentNullException("reply");
					}
					SMTP_Session.SendResponseAsyncOP sendResponseOP = new SMTP_Session.SendResponseAsyncOP(reply);
					sendResponseOP.CompletedAsync += delegate(object sender, EventArgs<SMTP_Session.SendResponseAsyncOP> e)
					{
						this.SendFinalResponseCompleted(sendResponseOP);
					};
					bool flag2 = !this.m_pSession.SendResponseAsync(sendResponseOP);
					if (flag2)
					{
						this.SendFinalResponseCompleted(sendResponseOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pSession.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
			}

			// Token: 0x060018FA RID: 6394 RVA: 0x0009A780 File Offset: 0x00099780
			private void SendFinalResponseCompleted(SMTP_Session.SendResponseAsyncOP op)
			{
				bool flag = op.Error != null;
				if (flag)
				{
					this.m_pException = op.Error;
				}
				this.SetState(AsyncOP_State.Completed);
				op.Dispose();
			}

			// Token: 0x060018FB RID: 6395 RVA: 0x0009A7B8 File Offset: 0x000997B8
			private void Send354ResponseCompleted(SMTP_Session.SendResponseAsyncOP op)
			{
				try
				{
					byte[] array = this.m_pSession.CreateReceivedHeader();
					this.m_pSession.m_pMessageStream.Write(array, 0, array.Length);
					SmartStream.ReadPeriodTerminatedAsyncOP readPeriodTermOP = new SmartStream.ReadPeriodTerminatedAsyncOP(this.m_pSession.m_pMessageStream, (long)this.m_pSession.Server.MaxMessageSize, SizeExceededAction.JunkAndThrowException);
					readPeriodTermOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadPeriodTerminatedAsyncOP> e)
					{
						this.MessageReadingCompleted(readPeriodTermOP);
					};
					bool flag = this.m_pSession.TcpStream.ReadPeriodTerminated(readPeriodTermOP, true);
					if (flag)
					{
						this.MessageReadingCompleted(readPeriodTermOP);
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
					this.m_pSession.LogAddException("Exception: " + this.m_pException.Message, this.m_pException);
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x060018FC RID: 6396 RVA: 0x0009A8B8 File Offset: 0x000998B8
			private void MessageReadingCompleted(SmartStream.ReadPeriodTerminatedAsyncOP op)
			{
				try
				{
					bool flag = op.Error != null;
					if (flag)
					{
						bool flag2 = op.Error is LineSizeExceededException;
						if (flag2)
						{
							this.SendFinalResponse(new SMTP_t_ReplyLine(500, "Line too long.", true));
						}
						else
						{
							bool flag3 = op.Error is DataSizeExceededException;
							if (flag3)
							{
								this.SendFinalResponse(new SMTP_t_ReplyLine(552, "Too much mail data.", true));
							}
							else
							{
								this.m_pException = op.Error;
							}
						}
						this.m_pSession.OnMessageStoringCanceled();
					}
					else
					{
						this.m_pSession.LogAddRead(op.BytesStored, "Readed " + op.BytesStored + " message bytes.");
						SMTP_Reply smtp_Reply = new SMTP_Reply(250, "DATA completed in " + (DateTime.Now - this.m_StartTime).TotalSeconds.ToString("f2") + " seconds.");
						smtp_Reply = this.m_pSession.OnMessageStoringCompleted(smtp_Reply);
						this.SendFinalResponse(SMTP_t_ReplyLine.Parse(smtp_Reply.ReplyCode + " " + smtp_Reply.ReplyLines[0]));
					}
				}
				catch (Exception pException)
				{
					this.m_pException = pException;
				}
				bool flag4 = this.m_pException != null;
				if (flag4)
				{
					this.SetState(AsyncOP_State.Completed);
				}
				op.Dispose();
			}

			// Token: 0x17000822 RID: 2082
			// (get) Token: 0x060018FD RID: 6397 RVA: 0x0009AA44 File Offset: 0x00099A44
			public AsyncOP_State State
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x17000823 RID: 2083
			// (get) Token: 0x060018FE RID: 6398 RVA: 0x0009AA5C File Offset: 0x00099A5C
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

			// Token: 0x140000B7 RID: 183
			// (add) Token: 0x060018FF RID: 6399 RVA: 0x0009AAB0 File Offset: 0x00099AB0
			// (remove) Token: 0x06001900 RID: 6400 RVA: 0x0009AAE8 File Offset: 0x00099AE8
			
			public event EventHandler<EventArgs<SMTP_Session.Cmd_DATA>> CompletedAsync = null;

			// Token: 0x06001901 RID: 6401 RVA: 0x0009AB20 File Offset: 0x00099B20
			private void OnCompletedAsync()
			{
				bool flag = this.CompletedAsync != null;
				if (flag)
				{
					this.CompletedAsync(this, new EventArgs<SMTP_Session.Cmd_DATA>(this));
				}
			}

			// Token: 0x04000AD5 RID: 2773
			private object m_pLock = new object();

			// Token: 0x04000AD6 RID: 2774
			private AsyncOP_State m_State = AsyncOP_State.WaitingForStart;

			// Token: 0x04000AD7 RID: 2775
			private Exception m_pException = null;

			// Token: 0x04000AD8 RID: 2776
			private SMTP_Session m_pSession = null;

			// Token: 0x04000AD9 RID: 2777
			private DateTime m_StartTime;

			// Token: 0x04000ADA RID: 2778
			private bool m_RiseCompleted = false;
		}
	}
}
