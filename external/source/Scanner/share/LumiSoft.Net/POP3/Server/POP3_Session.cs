using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000F0 RID: 240
	public class POP3_Session : TCP_ServerSession
	{
		// Token: 0x06000997 RID: 2455 RVA: 0x00039DD8 File Offset: 0x00038DD8
		public POP3_Session()
		{
			this.m_pAuthentications = new Dictionary<string, AUTH_SASL_ServerMechanism>(StringComparer.CurrentCultureIgnoreCase);
			this.m_pMessages = new KeyValueCollection<string, POP3_ServerMessage>();
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x00039E64 File Offset: 0x00038E64
		protected override void Start()
		{
			base.Start();
			try
			{
				bool flag = string.IsNullOrEmpty(this.Server.GreetingText);
				string text;
				if (flag)
				{
					text = "+OK [" + Net_Utils.GetLocalHostName(base.LocalHostName) + "] POP3 Service Ready.";
				}
				else
				{
					text = "+OK " + this.Server.GreetingText;
				}
				POP3_e_Started pop3_e_Started = this.OnStarted(text);
				bool flag2 = !string.IsNullOrEmpty(pop3_e_Started.Response);
				if (flag2)
				{
					this.WriteLine(text.ToString());
				}
				bool flag3 = string.IsNullOrEmpty(pop3_e_Started.Response) || pop3_e_Started.Response.ToUpper().StartsWith("-ERR");
				if (flag3)
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

		// Token: 0x06000999 RID: 2457 RVA: 0x00039F50 File Offset: 0x00038F50
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
								this.WriteLine("-ERR Internal server error.");
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

		// Token: 0x0600099A RID: 2458 RVA: 0x00039FFC File Offset: 0x00038FFC
		protected override void OnTimeout()
		{
			try
			{
				this.WriteLine("-ERR Idle timeout, closing connection.");
			}
			catch
			{
			}
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x0003A030 File Offset: 0x00039030
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

		// Token: 0x0600099C RID: 2460 RVA: 0x0003A0DC File Offset: 0x000390DC
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
				string[] array = Encoding.UTF8.GetString(op.Buffer, 0, op.LineBytesInBuffer).Split(new char[]
				{
					' '
				}, 2);
				string text = array[0].ToUpperInvariant();
				string cmdText = (array.Length == 2) ? array[1] : "";
				bool flag3 = this.Server.Logger != null;
				if (flag3)
				{
					bool flag4 = text == "PASS";
					if (flag4)
					{
						this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)op.BytesInBuffer, "PASS <***REMOVED***>", this.LocalEndPoint, this.RemoteEndPoint);
					}
					else
					{
						this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)op.BytesInBuffer, op.LineUtf8, this.LocalEndPoint, this.RemoteEndPoint);
					}
				}
				bool flag5 = text == "STLS";
				if (flag5)
				{
					this.STLS(cmdText);
				}
				else
				{
					bool flag6 = text == "USER";
					if (flag6)
					{
						this.USER(cmdText);
					}
					else
					{
						bool flag7 = text == "PASS";
						if (flag7)
						{
							this.PASS(cmdText);
						}
						else
						{
							bool flag8 = text == "AUTH";
							if (flag8)
							{
								this.AUTH(cmdText);
							}
							else
							{
								bool flag9 = text == "STAT";
								if (flag9)
								{
									this.STAT(cmdText);
								}
								else
								{
									bool flag10 = text == "LIST";
									if (flag10)
									{
										this.LIST(cmdText);
									}
									else
									{
										bool flag11 = text == "UIDL";
										if (flag11)
										{
											this.UIDL(cmdText);
										}
										else
										{
											bool flag12 = text == "TOP";
											if (flag12)
											{
												this.TOP(cmdText);
											}
											else
											{
												bool flag13 = text == "RETR";
												if (flag13)
												{
													this.RETR(cmdText);
												}
												else
												{
													bool flag14 = text == "DELE";
													if (flag14)
													{
														this.DELE(cmdText);
													}
													else
													{
														bool flag15 = text == "NOOP";
														if (flag15)
														{
															this.NOOP(cmdText);
														}
														else
														{
															bool flag16 = text == "RSET";
															if (flag16)
															{
																this.RSET(cmdText);
															}
															else
															{
																bool flag17 = text == "CAPA";
																if (flag17)
																{
																	this.CAPA(cmdText);
																}
																else
																{
																	bool flag18 = text == "QUIT";
																	if (flag18)
																	{
																		this.QUIT(cmdText);
																	}
																	else
																	{
																		this.m_BadCommands++;
																		bool flag19 = this.Server.MaxBadCommands != 0 && this.m_BadCommands > this.Server.MaxBadCommands;
																		if (flag19)
																		{
																			this.WriteLine("-ERR Too many bad commands, closing transmission channel.");
																			this.Disconnect();
																			return false;
																		}
																		this.WriteLine("-ERR Error: command '" + text + "' not recognized.");
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
					}
				}
			}
			catch (Exception x)
			{
				this.OnError(x);
			}
			return result;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x0003A49C File Offset: 0x0003949C
		private void STLS(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.TcpStream.WriteLine("-ERR This ommand is only valid in AUTHORIZATION state (RFC 2595 4).");
				}
				else
				{
					bool isSecureConnection = this.IsSecureConnection;
					if (isSecureConnection)
					{
						this.WriteLine("-ERR Bad sequence of commands: Connection is already secure.");
					}
					else
					{
						bool flag = base.Certificate == null;
						if (flag)
						{
							this.WriteLine("-ERR TLS not available: Server has no SSL certificate.");
						}
						else
						{
							this.WriteLine("+OK Ready to start TLS.");
							try
							{
								base.SwitchToSecure();
								this.LogAddText("TLS negotiation completed successfully.");
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

		// Token: 0x0600099E RID: 2462 RVA: 0x0003A57C File Offset: 0x0003957C
		private void USER(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.TcpStream.WriteLine("-ERR Re-authentication error.");
				}
				else
				{
					bool flag = this.m_UserName != null;
					if (flag)
					{
						this.TcpStream.WriteLine("-ERR User name already specified.");
					}
					else
					{
						this.m_UserName = cmdText;
						this.TcpStream.WriteLine("+OK User name OK.");
					}
				}
			}
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x0003A5FC File Offset: 0x000395FC
		private void PASS(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.TcpStream.WriteLine("-ERR Re-authentication error.");
				}
				else
				{
					bool flag = this.m_UserName == null;
					if (flag)
					{
						this.TcpStream.WriteLine("-ERR Specify user name first.");
					}
					else
					{
						bool flag2 = string.IsNullOrEmpty(cmdText);
						if (flag2)
						{
							this.TcpStream.WriteLine("-ERR Error in arguments.");
						}
						else
						{
							POP3_e_Authenticate pop3_e_Authenticate = this.OnAuthenticate(this.m_UserName, cmdText);
							bool isAuthenticated2 = pop3_e_Authenticate.IsAuthenticated;
							if (isAuthenticated2)
							{
								this.m_pUser = new GenericIdentity(this.m_UserName, "POP3-USER/PASS");
								POP3_e_GetMessagesInfo pop3_e_GetMessagesInfo = this.OnGetMessagesInfo();
								int num = 1;
								foreach (POP3_ServerMessage pop3_ServerMessage in pop3_e_GetMessagesInfo.Messages)
								{
									pop3_ServerMessage.SequenceNumber = num++;
									this.m_pMessages.Add(pop3_ServerMessage.UID, pop3_ServerMessage);
								}
								this.TcpStream.WriteLine("+OK Authenticated successfully.");
							}
							else
							{
								this.TcpStream.WriteLine("-ERR Authentication failed.");
							}
						}
					}
				}
			}
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0003A760 File Offset: 0x00039760
		private void AUTH(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.TcpStream.WriteLine("-ERR Re-authentication error.");
				}
				else
				{
					bool flag = string.IsNullOrEmpty(cmdText);
					if (flag)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("+OK\r\n");
						foreach (AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism in this.m_pAuthentications.Values)
						{
							stringBuilder.Append(auth_SASL_ServerMechanism.Name + "\r\n");
						}
						stringBuilder.Append(".\r\n");
						this.WriteLine(stringBuilder.ToString());
					}
					else
					{
						bool flag2 = !this.Authentications.ContainsKey(cmdText);
						if (flag2)
						{
							this.WriteLine("-ERR Not supported authentication mechanism.");
						}
						else
						{
							byte[] array = new byte[0];
							AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism2 = this.Authentications[cmdText];
							auth_SASL_ServerMechanism2.Reset();
							SmartStream.ReadLineAsyncOP readLineAsyncOP;
							for (;;)
							{
								byte[] array2 = auth_SASL_ServerMechanism2.Continue(array);
								bool isCompleted = auth_SASL_ServerMechanism2.IsCompleted;
								if (isCompleted)
								{
									break;
								}
								bool flag3 = array2.Length == 0;
								if (flag3)
								{
									this.WriteLine("+ ");
								}
								else
								{
									this.WriteLine("+ " + Convert.ToBase64String(array2));
								}
								readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
								this.TcpStream.ReadLine(readLineAsyncOP, false);
								bool flag4 = readLineAsyncOP.Error != null;
								if (flag4)
								{
									goto Block_10;
								}
								bool flag5 = this.Server.Logger != null;
								if (flag5)
								{
									this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)readLineAsyncOP.BytesInBuffer, "base64 auth-data", this.LocalEndPoint, this.RemoteEndPoint);
								}
								bool flag6 = readLineAsyncOP.LineUtf8 == "*";
								if (flag6)
								{
									goto Block_12;
								}
								try
								{
									array = Convert.FromBase64String(readLineAsyncOP.LineUtf8);
								}
								catch
								{
									this.WriteLine("-ERR Invalid client response '" + array + "'.");
									return;
								}
							}
							bool isAuthenticated2 = auth_SASL_ServerMechanism2.IsAuthenticated;
							if (isAuthenticated2)
							{
								this.m_pUser = new GenericIdentity(auth_SASL_ServerMechanism2.UserName, "SASL-" + auth_SASL_ServerMechanism2.Name);
								POP3_e_GetMessagesInfo pop3_e_GetMessagesInfo = this.OnGetMessagesInfo();
								int num = 1;
								foreach (POP3_ServerMessage pop3_ServerMessage in pop3_e_GetMessagesInfo.Messages)
								{
									pop3_ServerMessage.SequenceNumber = num++;
									this.m_pMessages.Add(pop3_ServerMessage.UID, pop3_ServerMessage);
								}
								this.WriteLine("+OK Authentication succeeded.");
							}
							else
							{
								this.WriteLine("-ERR Authentication credentials invalid.");
							}
							return;
							Block_10:
							throw readLineAsyncOP.Error;
							Block_12:
							this.WriteLine("-ERR Authentication canceled.");
						}
					}
				}
			}
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0003AAA4 File Offset: 0x00039AA4
		private void STAT(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					int num = 0;
					int num2 = 0;
					foreach (object obj in this.m_pMessages)
					{
						POP3_ServerMessage pop3_ServerMessage = (POP3_ServerMessage)obj;
						bool flag2 = !pop3_ServerMessage.IsMarkedForDeletion;
						if (flag2)
						{
							num++;
							num2 += pop3_ServerMessage.Size;
						}
					}
					this.WriteLine(string.Concat(new object[]
					{
						"+OK ",
						num,
						" ",
						num2
					}));
				}
			}
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0003AB98 File Offset: 0x00039B98
		private void LIST(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag2 = string.IsNullOrEmpty(cmdText);
					if (flag2)
					{
						int num = 0;
						int num2 = 0;
						foreach (object obj in this.m_pMessages)
						{
							POP3_ServerMessage pop3_ServerMessage = (POP3_ServerMessage)obj;
							bool flag3 = !pop3_ServerMessage.IsMarkedForDeletion;
							if (flag3)
							{
								num++;
								num2 += pop3_ServerMessage.Size;
							}
						}
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(string.Concat(new object[]
						{
							"+OK ",
							num,
							" messages (",
							num2,
							" bytes).\r\n"
						}));
						foreach (object obj2 in this.m_pMessages)
						{
							POP3_ServerMessage pop3_ServerMessage2 = (POP3_ServerMessage)obj2;
							stringBuilder.Append(string.Concat(new object[]
							{
								pop3_ServerMessage2.SequenceNumber,
								" ",
								pop3_ServerMessage2.Size,
								"\r\n"
							}));
						}
						stringBuilder.Append(".");
						this.WriteLine(stringBuilder.ToString());
					}
					else
					{
						bool flag4 = array.Length > 1 || !Net_Utils.IsInteger(array[0]);
						if (flag4)
						{
							this.WriteLine("-ERR Error in arguments.");
						}
						else
						{
							POP3_ServerMessage pop3_ServerMessage3 = null;
							bool flag5 = this.m_pMessages.TryGetValueAt(Convert.ToInt32(array[0]) - 1, out pop3_ServerMessage3);
							if (flag5)
							{
								bool isMarkedForDeletion = pop3_ServerMessage3.IsMarkedForDeletion;
								if (isMarkedForDeletion)
								{
									this.WriteLine("-ERR Invalid operation: Message marked for deletion.");
								}
								else
								{
									this.WriteLine(string.Concat(new object[]
									{
										"+OK ",
										pop3_ServerMessage3.SequenceNumber,
										" ",
										pop3_ServerMessage3.Size
									}));
								}
							}
							else
							{
								this.WriteLine("-ERR no such message or message marked for deletion.");
							}
						}
					}
				}
			}
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0003AE30 File Offset: 0x00039E30
		private void UIDL(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag2 = string.IsNullOrEmpty(cmdText);
					if (flag2)
					{
						int num = 0;
						int num2 = 0;
						foreach (object obj in this.m_pMessages)
						{
							POP3_ServerMessage pop3_ServerMessage = (POP3_ServerMessage)obj;
							bool flag3 = !pop3_ServerMessage.IsMarkedForDeletion;
							if (flag3)
							{
								num++;
								num2 += pop3_ServerMessage.Size;
							}
						}
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(string.Concat(new object[]
						{
							"+OK ",
							num,
							" messages (",
							num2,
							" bytes).\r\n"
						}));
						foreach (object obj2 in this.m_pMessages)
						{
							POP3_ServerMessage pop3_ServerMessage2 = (POP3_ServerMessage)obj2;
							stringBuilder.Append(string.Concat(new object[]
							{
								pop3_ServerMessage2.SequenceNumber,
								" ",
								pop3_ServerMessage2.UID,
								"\r\n"
							}));
						}
						stringBuilder.Append(".");
						this.WriteLine(stringBuilder.ToString());
					}
					else
					{
						bool flag4 = array.Length > 1;
						if (flag4)
						{
							this.WriteLine("-ERR Error in arguments.");
						}
						else
						{
							POP3_ServerMessage pop3_ServerMessage3 = null;
							bool flag5 = this.m_pMessages.TryGetValueAt(Convert.ToInt32(array[0]) - 1, out pop3_ServerMessage3);
							if (flag5)
							{
								bool isMarkedForDeletion = pop3_ServerMessage3.IsMarkedForDeletion;
								if (isMarkedForDeletion)
								{
									this.WriteLine("-ERR Invalid operation: Message marked for deletion.");
								}
								else
								{
									this.WriteLine(string.Concat(new object[]
									{
										"+OK ",
										pop3_ServerMessage3.SequenceNumber,
										" ",
										pop3_ServerMessage3.UID
									}));
								}
							}
							else
							{
								this.WriteLine("-ERR no such message or message marked for deletion.");
							}
						}
					}
				}
			}
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x0003B0B0 File Offset: 0x0003A0B0
		private void TOP(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag2 = array.Length != 2 || !Net_Utils.IsInteger(array[0]) || !Net_Utils.IsInteger(array[1]);
					if (flag2)
					{
						this.WriteLine("-ERR Error in arguments.");
					}
					else
					{
						POP3_ServerMessage pop3_ServerMessage = null;
						bool flag3 = this.m_pMessages.TryGetValueAt(Convert.ToInt32(array[0]) - 1, out pop3_ServerMessage);
						if (flag3)
						{
							bool isMarkedForDeletion = pop3_ServerMessage.IsMarkedForDeletion;
							if (isMarkedForDeletion)
							{
								this.WriteLine("-ERR Invalid operation: Message marked for deletion.");
							}
							else
							{
								POP3_e_GetTopOfMessage pop3_e_GetTopOfMessage = this.OnGetTopOfMessage(pop3_ServerMessage, Convert.ToInt32(array[1]));
								bool flag4 = pop3_e_GetTopOfMessage.Data == null;
								if (flag4)
								{
									this.WriteLine("-ERR no such message.");
								}
								else
								{
									this.WriteLine("+OK Start sending top of message.");
									long num = this.TcpStream.WritePeriodTerminated(new MemoryStream(pop3_e_GetTopOfMessage.Data));
									bool flag5 = this.Server.Logger != null;
									if (flag5)
									{
										this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, num, "Wrote top of message(" + num + " bytes).", this.LocalEndPoint, this.RemoteEndPoint);
									}
								}
							}
						}
						else
						{
							this.WriteLine("-ERR no such message.");
						}
					}
				}
			}
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0003B244 File Offset: 0x0003A244
		private void RETR(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag2 = array.Length != 1 || !Net_Utils.IsInteger(array[0]);
					if (flag2)
					{
						this.WriteLine("-ERR Error in arguments.");
					}
					else
					{
						POP3_ServerMessage pop3_ServerMessage = null;
						bool flag3 = this.m_pMessages.TryGetValueAt(Convert.ToInt32(array[0]) - 1, out pop3_ServerMessage);
						if (flag3)
						{
							bool isMarkedForDeletion = pop3_ServerMessage.IsMarkedForDeletion;
							if (isMarkedForDeletion)
							{
								this.WriteLine("-ERR Invalid operation: Message marked for deletion.");
							}
							else
							{
								POP3_e_GetMessageStream pop3_e_GetMessageStream = this.OnGetMessageStream(pop3_ServerMessage);
								bool flag4 = pop3_e_GetMessageStream.MessageStream == null;
								if (flag4)
								{
									this.WriteLine("-ERR no such message.");
								}
								else
								{
									try
									{
										this.WriteLine("+OK Start sending message.");
										long num = this.TcpStream.WritePeriodTerminated(pop3_e_GetMessageStream.MessageStream);
										bool flag5 = this.Server.Logger != null;
										if (flag5)
										{
											this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, num, "Wrote message(" + num + " bytes).", this.LocalEndPoint, this.RemoteEndPoint);
										}
									}
									finally
									{
										bool closeMessageStream = pop3_e_GetMessageStream.CloseMessageStream;
										if (closeMessageStream)
										{
											pop3_e_GetMessageStream.MessageStream.Dispose();
										}
									}
								}
							}
						}
						else
						{
							this.WriteLine("-ERR no such message.");
						}
					}
				}
			}
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x0003B3F8 File Offset: 0x0003A3F8
		private void DELE(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag2 = array.Length != 1 || !Net_Utils.IsInteger(array[0]);
					if (flag2)
					{
						this.WriteLine("-ERR Error in arguments.");
					}
					else
					{
						POP3_ServerMessage pop3_ServerMessage = null;
						bool flag3 = this.m_pMessages.TryGetValueAt(Convert.ToInt32(array[0]) - 1, out pop3_ServerMessage);
						if (flag3)
						{
							bool flag4 = !pop3_ServerMessage.IsMarkedForDeletion;
							if (flag4)
							{
								pop3_ServerMessage.SetIsMarkedForDeletion(true);
								this.WriteLine("+OK Message marked for deletion.");
							}
							else
							{
								this.WriteLine("-ERR Message already marked for deletion.");
							}
						}
						else
						{
							this.WriteLine("-ERR no such message.");
						}
					}
				}
			}
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0003B4E4 File Offset: 0x0003A4E4
		private void NOOP(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					this.WriteLine("+OK");
				}
			}
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0003B534 File Offset: 0x0003A534
		private void RSET(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("-ERR Authentication required.");
				}
				else
				{
					foreach (object obj in this.m_pMessages)
					{
						POP3_ServerMessage pop3_ServerMessage = (POP3_ServerMessage)obj;
						pop3_ServerMessage.SetIsMarkedForDeletion(false);
					}
					this.WriteLine("+OK");
					this.OnReset();
				}
			}
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0003B5E0 File Offset: 0x0003A5E0
		private void CAPA(string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("-ERR Bad sequence of commands: Session rejected.");
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("+OK Capability list follows\r\n");
				stringBuilder.Append("PIPELINING\r\n");
				stringBuilder.Append("UIDL\r\n");
				stringBuilder.Append("TOP\r\n");
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism in this.Authentications.Values)
				{
					bool flag = !auth_SASL_ServerMechanism.RequireSSL || (auth_SASL_ServerMechanism.RequireSSL && this.IsSecureConnection);
					if (flag)
					{
						stringBuilder2.Append(auth_SASL_ServerMechanism.Name + " ");
					}
				}
				bool flag2 = stringBuilder2.Length > 0;
				if (flag2)
				{
					stringBuilder.Append("SASL " + stringBuilder2.ToString().Trim() + "\r\n");
				}
				bool flag3 = !this.IsSecureConnection && base.Certificate != null;
				if (flag3)
				{
					stringBuilder.Append("STLS\r\n");
				}
				stringBuilder.Append(".");
				this.WriteLine(stringBuilder.ToString());
			}
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x0003B740 File Offset: 0x0003A740
		private void QUIT(string cmdText)
		{
			try
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					foreach (object obj in this.m_pMessages)
					{
						POP3_ServerMessage pop3_ServerMessage = (POP3_ServerMessage)obj;
						bool isMarkedForDeletion = pop3_ServerMessage.IsMarkedForDeletion;
						if (isMarkedForDeletion)
						{
							this.OnDeleteMessage(pop3_ServerMessage);
						}
					}
				}
				this.WriteLine("+OK <" + Net_Utils.GetLocalHostName(base.LocalHostName) + "> Service closing transmission channel.");
			}
			catch
			{
			}
			this.Disconnect();
			this.Dispose();
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x0003B800 File Offset: 0x0003A800
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

		// Token: 0x060009AC RID: 2476 RVA: 0x0003B870 File Offset: 0x0003A870
		public void LogAddText(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			bool flag2 = this.Server.Logger != null;
			if (flag2)
			{
				this.Server.Logger.AddText(this.ID, text);
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x060009AD RID: 2477 RVA: 0x0003B8C0 File Offset: 0x0003A8C0
		public new POP3_Server Server
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (POP3_Server)base.Server;
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x060009AE RID: 2478 RVA: 0x0003B8FC File Offset: 0x0003A8FC
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

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x060009AF RID: 2479 RVA: 0x0003B930 File Offset: 0x0003A930
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

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0003B964 File Offset: 0x0003A964
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

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x060009B1 RID: 2481 RVA: 0x0003B998 File Offset: 0x0003A998
		// (remove) Token: 0x060009B2 RID: 2482 RVA: 0x0003B9D0 File Offset: 0x0003A9D0
		
		public event EventHandler<POP3_e_Started> Started = null;

		// Token: 0x060009B3 RID: 2483 RVA: 0x0003BA08 File Offset: 0x0003AA08
		private POP3_e_Started OnStarted(string reply)
		{
			POP3_e_Started pop3_e_Started = new POP3_e_Started(reply);
			bool flag = this.Started != null;
			if (flag)
			{
				this.Started(this, pop3_e_Started);
			}
			return pop3_e_Started;
		}

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060009B4 RID: 2484 RVA: 0x0003BA40 File Offset: 0x0003AA40
		// (remove) Token: 0x060009B5 RID: 2485 RVA: 0x0003BA78 File Offset: 0x0003AA78
		
		public event EventHandler<POP3_e_Authenticate> Authenticate = null;

		// Token: 0x060009B6 RID: 2486 RVA: 0x0003BAB0 File Offset: 0x0003AAB0
		private POP3_e_Authenticate OnAuthenticate(string user, string password)
		{
			POP3_e_Authenticate pop3_e_Authenticate = new POP3_e_Authenticate(user, password);
			bool flag = this.Authenticate != null;
			if (flag)
			{
				this.Authenticate(this, pop3_e_Authenticate);
			}
			return pop3_e_Authenticate;
		}

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x060009B7 RID: 2487 RVA: 0x0003BAE8 File Offset: 0x0003AAE8
		// (remove) Token: 0x060009B8 RID: 2488 RVA: 0x0003BB20 File Offset: 0x0003AB20
		
		public event EventHandler<POP3_e_GetMessagesInfo> GetMessagesInfo = null;

		// Token: 0x060009B9 RID: 2489 RVA: 0x0003BB58 File Offset: 0x0003AB58
		private POP3_e_GetMessagesInfo OnGetMessagesInfo()
		{
			POP3_e_GetMessagesInfo pop3_e_GetMessagesInfo = new POP3_e_GetMessagesInfo();
			bool flag = this.GetMessagesInfo != null;
			if (flag)
			{
				this.GetMessagesInfo(this, pop3_e_GetMessagesInfo);
			}
			return pop3_e_GetMessagesInfo;
		}

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x060009BA RID: 2490 RVA: 0x0003BB90 File Offset: 0x0003AB90
		// (remove) Token: 0x060009BB RID: 2491 RVA: 0x0003BBC8 File Offset: 0x0003ABC8
		
		public event EventHandler<POP3_e_GetTopOfMessage> GetTopOfMessage = null;

		// Token: 0x060009BC RID: 2492 RVA: 0x0003BC00 File Offset: 0x0003AC00
		private POP3_e_GetTopOfMessage OnGetTopOfMessage(POP3_ServerMessage message, int lines)
		{
			POP3_e_GetTopOfMessage pop3_e_GetTopOfMessage = new POP3_e_GetTopOfMessage(message, lines);
			bool flag = this.GetTopOfMessage != null;
			if (flag)
			{
				this.GetTopOfMessage(this, pop3_e_GetTopOfMessage);
			}
			return pop3_e_GetTopOfMessage;
		}

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x060009BD RID: 2493 RVA: 0x0003BC38 File Offset: 0x0003AC38
		// (remove) Token: 0x060009BE RID: 2494 RVA: 0x0003BC70 File Offset: 0x0003AC70
		
		public event EventHandler<POP3_e_GetMessageStream> GetMessageStream = null;

		// Token: 0x060009BF RID: 2495 RVA: 0x0003BCA8 File Offset: 0x0003ACA8
		private POP3_e_GetMessageStream OnGetMessageStream(POP3_ServerMessage message)
		{
			POP3_e_GetMessageStream pop3_e_GetMessageStream = new POP3_e_GetMessageStream(message);
			bool flag = this.GetMessageStream != null;
			if (flag)
			{
				this.GetMessageStream(this, pop3_e_GetMessageStream);
			}
			return pop3_e_GetMessageStream;
		}

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x060009C0 RID: 2496 RVA: 0x0003BCE0 File Offset: 0x0003ACE0
		// (remove) Token: 0x060009C1 RID: 2497 RVA: 0x0003BD18 File Offset: 0x0003AD18
		
		public event EventHandler<POP3_e_DeleteMessage> DeleteMessage = null;

		// Token: 0x060009C2 RID: 2498 RVA: 0x0003BD50 File Offset: 0x0003AD50
		private void OnDeleteMessage(POP3_ServerMessage message)
		{
			bool flag = this.DeleteMessage != null;
			if (flag)
			{
				this.DeleteMessage(this, new POP3_e_DeleteMessage(message));
			}
		}

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x060009C3 RID: 2499 RVA: 0x0003BD80 File Offset: 0x0003AD80
		// (remove) Token: 0x060009C4 RID: 2500 RVA: 0x0003BDB8 File Offset: 0x0003ADB8
		
		public event EventHandler Reset = null;

		// Token: 0x060009C5 RID: 2501 RVA: 0x0003BDF0 File Offset: 0x0003ADF0
		private void OnReset()
		{
			bool flag = this.Reset != null;
			if (flag)
			{
				this.Reset(this, new EventArgs());
			}
		}

		// Token: 0x04000442 RID: 1090
		private Dictionary<string, AUTH_SASL_ServerMechanism> m_pAuthentications = null;

		// Token: 0x04000443 RID: 1091
		private bool m_SessionRejected = false;

		// Token: 0x04000444 RID: 1092
		private int m_BadCommands = 0;

		// Token: 0x04000445 RID: 1093
		private string m_UserName = null;

		// Token: 0x04000446 RID: 1094
		private GenericIdentity m_pUser = null;

		// Token: 0x04000447 RID: 1095
		private KeyValueCollection<string, POP3_ServerMessage> m_pMessages = null;
	}
}
