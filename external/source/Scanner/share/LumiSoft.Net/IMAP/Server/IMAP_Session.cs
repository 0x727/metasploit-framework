using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Timers;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200020E RID: 526
	public class IMAP_Session : TCP_ServerSession
	{
		// Token: 0x0600127C RID: 4732 RVA: 0x0006FFC8 File Offset: 0x0006EFC8
		public IMAP_Session()
		{
			this.m_pAuthentications = new Dictionary<string, AUTH_SASL_ServerMechanism>(StringComparer.CurrentCultureIgnoreCase);
			this.m_pCapabilities = new List<string>();
			this.m_pCapabilities.AddRange(new string[]
			{
				"IMAP4rev1",
				"NAMESPACE",
				"QUOTA",
				"ACL",
				"IDLE",
				"ENABLE",
				"UTF8=ACCEPT",
				"SASL-IR"
			});
			this.m_pResponseSender = new IMAP_Session.ResponseSender(this);
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00070148 File Offset: 0x0006F148
		public override void Dispose()
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				base.Dispose();
				this.m_pAuthentications = null;
				this.m_pCapabilities = null;
				this.m_pUser = null;
				this.m_pSelectedFolder = null;
				bool flag = this.m_pResponseSender != null;
				if (flag)
				{
					this.m_pResponseSender.Dispose();
				}
				this.Started = null;
				this.Login = null;
				this.Namespace = null;
				this.List = null;
				this.Create = null;
				this.Delete = null;
				this.Rename = null;
				this.LSub = null;
				this.Subscribe = null;
				this.Unsubscribe = null;
				this.Select = null;
				this.GetMessagesInfo = null;
				this.Append = null;
				this.GetQuotaRoot = null;
				this.GetQuota = null;
				this.GetAcl = null;
				this.SetAcl = null;
				this.DeleteAcl = null;
				this.ListRights = null;
				this.MyRights = null;
				this.Fetch = null;
				this.Search = null;
				this.Store = null;
				this.Copy = null;
				this.Expunge = null;
			}
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x00070254 File Offset: 0x0006F254
		protected override void Start()
		{
			base.Start();
			try
			{
				bool flag = string.IsNullOrEmpty(this.Server.GreetingText);
				IMAP_r_u_ServerStatus response;
				if (flag)
				{
					response = new IMAP_r_u_ServerStatus("OK", "<" + Net_Utils.GetLocalHostName(base.LocalHostName) + "> IMAP4rev1 server ready.");
				}
				else
				{
					response = new IMAP_r_u_ServerStatus("OK", this.Server.GreetingText);
				}
				IMAP_e_Started imap_e_Started = this.OnStarted(response);
				bool flag2 = imap_e_Started.Response != null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(imap_e_Started.Response);
				}
				bool flag3 = imap_e_Started.Response == null || imap_e_Started.Response.ResponseCode.Equals("NO", StringComparison.InvariantCultureIgnoreCase);
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

		// Token: 0x0600127F RID: 4735 RVA: 0x00070344 File Offset: 0x0006F344
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
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("BAD", "Internal server error: " + x.Message));
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

		// Token: 0x06001280 RID: 4736 RVA: 0x00070410 File Offset: 0x0006F410
		protected override void OnTimeout()
		{
			try
			{
				this.WriteLine("* BYE Idle timeout, closing connection.");
			}
			catch
			{
			}
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x00070444 File Offset: 0x0006F444
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

		// Token: 0x06001282 RID: 4738 RVA: 0x000704F0 File Offset: 0x0006F4F0
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
				}, 3);
				bool flag3 = array.Length < 2;
				if (flag3)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("BAD", "Error: Command '" + op.LineUtf8 + "' not recognized."));
					return true;
				}
				string text = array[0];
				string text2 = array[1].ToUpperInvariant();
				string text3 = (array.Length == 3) ? array[2] : "";
				bool flag4 = this.Server.Logger != null;
				if (flag4)
				{
					bool flag5 = text2 == "LOGIN";
					if (flag5)
					{
						this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)op.BytesInBuffer, op.LineUtf8.Substring(0, op.LineUtf8.LastIndexOf(' ')) + " <***REMOVED***>", this.LocalEndPoint, this.RemoteEndPoint);
					}
					else
					{
						this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)op.BytesInBuffer, op.LineUtf8, this.LocalEndPoint, this.RemoteEndPoint);
					}
				}
				bool flag6 = IMAP_Session._CmdReader.EndsWithLiteralString(text3) && !string.Equals(text2, "APPEND", StringComparison.InvariantCultureIgnoreCase);
				if (flag6)
				{
					IMAP_Session._CmdReader cmdReader = new IMAP_Session._CmdReader(this, text3, Encoding.UTF8);
					cmdReader.Start();
					text3 = cmdReader.CmdLine;
				}
				bool flag7 = text2 == "STARTTLS";
				if (flag7)
				{
					this.STARTTLS(text, text3);
					result = false;
				}
				else
				{
					bool flag8 = text2 == "LOGIN";
					if (flag8)
					{
						this.LOGIN(text, text3);
					}
					else
					{
						bool flag9 = text2 == "AUTHENTICATE";
						if (flag9)
						{
							this.AUTHENTICATE(text, text3);
						}
						else
						{
							bool flag10 = text2 == "NAMESPACE";
							if (flag10)
							{
								this.NAMESPACE(text, text3);
							}
							else
							{
								bool flag11 = text2 == "LIST";
								if (flag11)
								{
									this.LIST(text, text3);
								}
								else
								{
									bool flag12 = text2 == "CREATE";
									if (flag12)
									{
										this.CREATE(text, text3);
									}
									else
									{
										bool flag13 = text2 == "DELETE";
										if (flag13)
										{
											this.DELETE(text, text3);
										}
										else
										{
											bool flag14 = text2 == "RENAME";
											if (flag14)
											{
												this.RENAME(text, text3);
											}
											else
											{
												bool flag15 = text2 == "LSUB";
												if (flag15)
												{
													this.LSUB(text, text3);
												}
												else
												{
													bool flag16 = text2 == "SUBSCRIBE";
													if (flag16)
													{
														this.SUBSCRIBE(text, text3);
													}
													else
													{
														bool flag17 = text2 == "UNSUBSCRIBE";
														if (flag17)
														{
															this.UNSUBSCRIBE(text, text3);
														}
														else
														{
															bool flag18 = text2 == "STATUS";
															if (flag18)
															{
																this.STATUS(text, text3);
															}
															else
															{
																bool flag19 = text2 == "SELECT";
																if (flag19)
																{
																	this.SELECT(text, text3);
																}
																else
																{
																	bool flag20 = text2 == "EXAMINE";
																	if (flag20)
																	{
																		this.EXAMINE(text, text3);
																	}
																	else
																	{
																		bool flag21 = text2 == "APPEND";
																		if (flag21)
																		{
																			this.APPEND(text, text3);
																			return false;
																		}
																		bool flag22 = text2 == "GETQUOTAROOT";
																		if (flag22)
																		{
																			this.GETQUOTAROOT(text, text3);
																		}
																		else
																		{
																			bool flag23 = text2 == "GETQUOTA";
																			if (flag23)
																			{
																				this.GETQUOTA(text, text3);
																			}
																			else
																			{
																				bool flag24 = text2 == "GETACL";
																				if (flag24)
																				{
																					this.GETACL(text, text3);
																				}
																				else
																				{
																					bool flag25 = text2 == "SETACL";
																					if (flag25)
																					{
																						this.SETACL(text, text3);
																					}
																					else
																					{
																						bool flag26 = text2 == "DELETEACL";
																						if (flag26)
																						{
																							this.DELETEACL(text, text3);
																						}
																						else
																						{
																							bool flag27 = text2 == "LISTRIGHTS";
																							if (flag27)
																							{
																								this.LISTRIGHTS(text, text3);
																							}
																							else
																							{
																								bool flag28 = text2 == "MYRIGHTS";
																								if (flag28)
																								{
																									this.MYRIGHTS(text, text3);
																								}
																								else
																								{
																									bool flag29 = text2 == "ENABLE";
																									if (flag29)
																									{
																										this.ENABLE(text, text3);
																									}
																									else
																									{
																										bool flag30 = text2 == "CHECK";
																										if (flag30)
																										{
																											this.CHECK(text, text3);
																										}
																										else
																										{
																											bool flag31 = text2 == "CLOSE";
																											if (flag31)
																											{
																												this.CLOSE(text, text3);
																											}
																											else
																											{
																												bool flag32 = text2 == "FETCH";
																												if (flag32)
																												{
																													this.FETCH(false, text, text3);
																												}
																												else
																												{
																													bool flag33 = text2 == "SEARCH";
																													if (flag33)
																													{
																														this.SEARCH(false, text, text3);
																													}
																													else
																													{
																														bool flag34 = text2 == "STORE";
																														if (flag34)
																														{
																															this.STORE(false, text, text3);
																														}
																														else
																														{
																															bool flag35 = text2 == "COPY";
																															if (flag35)
																															{
																																this.COPY(false, text, text3);
																															}
																															else
																															{
																																bool flag36 = text2 == "UID";
																																if (flag36)
																																{
																																	this.UID(text, text3);
																																}
																																else
																																{
																																	bool flag37 = text2 == "EXPUNGE";
																																	if (flag37)
																																	{
																																		this.EXPUNGE(text, text3);
																																	}
																																	else
																																	{
																																		bool flag38 = text2 == "IDLE";
																																		if (flag38)
																																		{
																																			result = this.IDLE(text, text3);
																																		}
																																		else
																																		{
																																			bool flag39 = text2 == "CAPABILITY";
																																			if (flag39)
																																			{
																																				this.CAPABILITY(text, text3);
																																			}
																																			else
																																			{
																																				bool flag40 = text2 == "NOOP";
																																				if (flag40)
																																				{
																																					this.NOOP(text, text3);
																																				}
																																				else
																																				{
																																					bool flag41 = text2 == "LOGOUT";
																																					if (flag41)
																																					{
																																						this.LOGOUT(text, text3);
																																						result = false;
																																					}
																																					else
																																					{
																																						this.m_BadCommands++;
																																						bool flag42 = this.Server.MaxBadCommands != 0 && this.m_BadCommands > this.Server.MaxBadCommands;
																																						if (flag42)
																																						{
																																							this.WriteLine("* BYE Too many bad commands, closing transmission channel.");
																																							this.Disconnect();
																																							return false;
																																						}
																																						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(text, "BAD", "Error: Command '" + text2 + "' not recognized."));
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

		// Token: 0x06001283 RID: 4739 RVA: 0x00070C54 File Offset: 0x0006FC54
		private void STARTTLS(string cmdTag, string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Bad sequence of commands: Session rejected."));
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "This ommand is only valid in not-authenticated state."));
				}
				else
				{
					bool isSecureConnection = this.IsSecureConnection;
					if (isSecureConnection)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Bad sequence of commands: Connection is already secure."));
					}
					else
					{
						bool flag = base.Certificate == null;
						if (flag)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "TLS not available: Server has no SSL certificate."));
						}
						else
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "Begin TLS negotiation now."));
							try
							{
								DateTime startTime = DateTime.Now;
								Action<TCP_ServerSession.SwitchToSecureAsyncOP> switchSecureCompleted = delegate(TCP_ServerSession.SwitchToSecureAsyncOP e)
								{
									try
									{
										bool flag3 = e.Error != null;
										if (flag3)
										{
											this.LogAddException(e.Error);
											this.Disconnect();
										}
										else
										{
											this.LogAddText("SSL negotiation completed successfully in " + (DateTime.Now - startTime).TotalSeconds.ToString("f2") + " seconds.");
											this.BeginReadCmd();
										}
									}
									catch (Exception exception2)
									{
										this.LogAddException(exception2);
										this.Disconnect();
									}
								};
								TCP_ServerSession.SwitchToSecureAsyncOP op = new TCP_ServerSession.SwitchToSecureAsyncOP();
								op.CompletedAsync += delegate(object sender, EventArgs<TCP_ServerSession.SwitchToSecureAsyncOP> e)
								{
									switchSecureCompleted(op);
								};
								bool flag2 = !base.SwitchToSecureAsync(op);
								if (flag2)
								{
									switchSecureCompleted(op);
								}
							}
							catch (Exception exception)
							{
								this.LogAddException(exception);
								this.Disconnect();
							}
						}
					}
				}
			}
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00070DDC File Offset: 0x0006FDDC
		private void LOGIN(string cmdTag, string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Bad sequence of commands: Session rejected."));
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Re-authentication error."));
				}
				else
				{
					bool flag = this.SupportsCap("LOGINDISABLED");
					if (flag)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command 'LOGIN' is disabled, use AUTHENTICATE instead."));
					}
					else
					{
						bool flag2 = string.IsNullOrEmpty(cmdText);
						if (flag2)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
						}
						else
						{
							string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
							bool flag3 = array.Length != 2;
							if (flag3)
							{
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
							}
							else
							{
								IMAP_e_Login imap_e_Login = this.OnLogin(array[0], array[1]);
								bool isAuthenticated2 = imap_e_Login.IsAuthenticated;
								if (isAuthenticated2)
								{
									this.m_pUser = new GenericIdentity(array[0], "IMAP-LOGIN");
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "LOGIN completed."));
								}
								else
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "LOGIN failed."));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x00070F48 File Offset: 0x0006FF48
		private void AUTHENTICATE(string cmdTag, string cmdText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Bad sequence of commands: Session rejected."));
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Re-authentication error."));
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					});
					bool flag = array.Length > 2;
					if (flag)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						byte[] array2 = new byte[0];
						bool flag2 = array.Length == 2;
						if (flag2)
						{
							bool flag3 = array[1] == "=";
							if (!flag3)
							{
								try
								{
									array2 = Convert.FromBase64String(array[1]);
								}
								catch
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Syntax error: Parameter 'initial-response' value must be BASE64 or contain a single character '='."));
									return;
								}
							}
						}
						string key = array[0];
						bool flag4 = !this.Authentications.ContainsKey(key);
						if (flag4)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Not supported authentication mechanism."));
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
								bool flag5 = array4.Length == 0;
								if (flag5)
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus("+", ""));
								}
								else
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus("+", Convert.ToBase64String(array4)));
								}
								readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
								this.TcpStream.ReadLine(readLineAsyncOP, false);
								bool flag6 = readLineAsyncOP.Error != null;
								if (flag6)
								{
									goto Block_11;
								}
								bool flag7 = this.Server.Logger != null;
								if (flag7)
								{
									this.Server.Logger.AddRead(this.ID, this.AuthenticatedUserIdentity, (long)readLineAsyncOP.BytesInBuffer, "base64 auth-data", this.LocalEndPoint, this.RemoteEndPoint);
								}
								bool flag8 = readLineAsyncOP.LineUtf8 == "*";
								if (flag8)
								{
									goto Block_13;
								}
								try
								{
									array3 = Convert.FromBase64String(readLineAsyncOP.LineUtf8);
								}
								catch
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Invalid client response '" + array3 + "'."));
									return;
								}
							}
							bool isAuthenticated2 = auth_SASL_ServerMechanism.IsAuthenticated;
							if (isAuthenticated2)
							{
								this.m_pUser = new GenericIdentity(auth_SASL_ServerMechanism.UserName, "SASL-" + auth_SASL_ServerMechanism.Name);
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "Authentication succeeded."));
							}
							else
							{
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication credentials invalid."));
							}
							return;
							Block_11:
							throw readLineAsyncOP.Error;
							Block_13:
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication canceled."));
						}
					}
				}
			}
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x000712A0 File Offset: 0x000702A0
		private void NAMESPACE(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("NAMESPACE");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					IMAP_e_Namespace imap_e_Namespace = this.OnNamespace(new IMAP_r_ServerStatus(cmdTag, "OK", "NAMESPACE command completed."));
					bool flag3 = imap_e_Namespace.NamespaceResponse != null;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(imap_e_Namespace.NamespaceResponse);
					}
					this.m_pResponseSender.SendResponseAsync(imap_e_Namespace.Response);
				}
			}
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x00071358 File Offset: 0x00070358
		private void LIST(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
				}
				else
				{
					string refName = IMAP_Utils.DecodeMailbox(array[0]);
					string text = IMAP_Utils.DecodeMailbox(array[1]);
					long ticks = DateTime.Now.Ticks;
					bool flag3 = text == string.Empty;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_List(this.m_FolderSeparator));
					}
					else
					{
						IMAP_e_List imap_e_List = this.OnList(refName, text);
						foreach (IMAP_r_u_List response in imap_e_List.Folders)
						{
							this.m_pResponseSender.SendResponseAsync(response);
						}
					}
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "LIST Completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
				}
			}
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x000714D8 File Offset: 0x000704D8
		private void CREATE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
				IMAP_e_Folder imap_e_Folder = this.OnCreate(cmdTag, folder, new IMAP_r_ServerStatus(cmdTag, "OK", "CREATE command completed."));
				this.m_pResponseSender.SendResponseAsync(imap_e_Folder.Response);
			}
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x0007154C File Offset: 0x0007054C
		private void DELETE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string folder = IMAP_Utils.DecodeMailbox(cmdText);
				IMAP_e_Folder imap_e_Folder = this.OnDelete(cmdTag, folder, new IMAP_r_ServerStatus(cmdTag, "OK", "DELETE command completed."));
				this.m_pResponseSender.SendResponseAsync(imap_e_Folder.Response);
			}
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x000715B8 File Offset: 0x000705B8
		private void RENAME(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
				}
				else
				{
					IMAP_e_Rename imap_e_Rename = this.OnRename(cmdTag, IMAP_Utils.DecodeMailbox(array[0]), IMAP_Utils.DecodeMailbox(array[1]));
					bool flag3 = imap_e_Rename.Response == null;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Internal server error: IMAP Server application didn't return any resposne."));
					}
					else
					{
						this.m_pResponseSender.SendResponseAsync(imap_e_Rename.Response);
					}
				}
			}
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x00071688 File Offset: 0x00070688
		private void LSUB(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
				}
				else
				{
					string refName = IMAP_Utils.DecodeMailbox(array[0]);
					string folder = IMAP_Utils.DecodeMailbox(array[1]);
					long ticks = DateTime.Now.Ticks;
					IMAP_e_LSub imap_e_LSub = this.OnLSub(refName, folder);
					foreach (IMAP_r_u_LSub response in imap_e_LSub.Folders)
					{
						this.m_pResponseSender.SendResponseAsync(response);
					}
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "LSUB Completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
				}
			}
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x000717DC File Offset: 0x000707DC
		private void SUBSCRIBE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string text = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
				bool flag2 = string.IsNullOrEmpty(text);
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Mailbox name must be specified."));
				}
				else
				{
					IMAP_e_Folder imap_e_Folder = this.OnSubscribe(cmdTag, text, new IMAP_r_ServerStatus(cmdTag, "OK", "SUBSCRIBE command completed."));
					this.m_pResponseSender.SendResponseAsync(imap_e_Folder.Response);
				}
			}
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x00071878 File Offset: 0x00070878
		private void UNSUBSCRIBE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
				IMAP_e_Folder imap_e_Folder = this.OnUnsubscribe(cmdTag, folder, new IMAP_r_ServerStatus(cmdTag, "OK", "UNSUBSCRIBE command completed."));
				this.m_pResponseSender.SendResponseAsync(imap_e_Folder.Response);
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x000718EC File Offset: 0x000708EC
		private void STATUS(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ', false, 2);
				bool flag2 = array.Length != 2;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
				}
				else
				{
					long ticks = DateTime.Now.Ticks;
					try
					{
						string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(array[0]));
						bool flag3 = !array[1].StartsWith("(") || !array[1].EndsWith(")");
						if (flag3)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
						}
						else
						{
							IMAP_e_Select imap_e_Select = this.OnSelect(cmdTag, folder);
							bool flag4 = imap_e_Select.ErrorResponse != null;
							if (flag4)
							{
								this.m_pResponseSender.SendResponseAsync(imap_e_Select.ErrorResponse);
							}
							else
							{
								IMAP_e_MessagesInfo imap_e_MessagesInfo = this.OnGetMessagesInfo(folder);
								int messagesCount = -1;
								int recentCount = -1;
								long uidNext = -1L;
								long folderUid = -1L;
								int unseenCount = -1;
								foreach (string a in array[1].Substring(1, array[1].Length - 2).Split(new char[]
								{
									' '
								}))
								{
									bool flag5 = string.Equals(a, "MESSAGES", StringComparison.InvariantCultureIgnoreCase);
									if (flag5)
									{
										messagesCount = imap_e_MessagesInfo.Exists;
									}
									else
									{
										bool flag6 = string.Equals(a, "RECENT", StringComparison.InvariantCultureIgnoreCase);
										if (flag6)
										{
											recentCount = imap_e_MessagesInfo.Recent;
										}
										else
										{
											bool flag7 = string.Equals(a, "UIDNEXT", StringComparison.InvariantCultureIgnoreCase);
											if (flag7)
											{
												uidNext = imap_e_MessagesInfo.UidNext;
											}
											else
											{
												bool flag8 = string.Equals(a, "UIDVALIDITY", StringComparison.InvariantCultureIgnoreCase);
												if (flag8)
												{
													folderUid = (long)imap_e_Select.FolderUID;
												}
												else
												{
													bool flag9 = string.Equals(a, "UNSEEN", StringComparison.InvariantCultureIgnoreCase);
													if (!flag9)
													{
														this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
														return;
													}
													unseenCount = imap_e_MessagesInfo.Unseen;
												}
											}
										}
									}
								}
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Status(folder, messagesCount, recentCount, uidNext, folderUid, unseenCount));
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "STATUS completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
							}
						}
					}
					catch (Exception ex)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: " + ex.Message));
					}
				}
			}
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x00071BF8 File Offset: 0x00070BF8
		private void SELECT(string cmdTag, string cmdText)
		{
			long ticks = DateTime.Now.Ticks;
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder != null;
				if (flag2)
				{
					this.m_pSelectedFolder = null;
				}
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ');
				bool flag3 = array.Length >= 2;
				if (flag3)
				{
					bool flag4 = string.Equals(array[1], "(UTF8)", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", new IMAP_t_orc_Unknown("NOT-UTF-8"), "Mailbox does not support UTF-8 access."));
					}
					else
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
				}
				else
				{
					try
					{
						string folder = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(cmdText));
						IMAP_e_Select imap_e_Select = this.OnSelect(cmdTag, folder);
						bool flag5 = imap_e_Select.ErrorResponse == null;
						if (flag5)
						{
							IMAP_e_MessagesInfo imap_e_MessagesInfo = this.OnGetMessagesInfo(folder);
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Exists(imap_e_MessagesInfo.Exists));
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Recent(imap_e_MessagesInfo.Recent));
							bool flag6 = imap_e_MessagesInfo.FirstUnseen > -1;
							if (flag6)
							{
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_Unseen(imap_e_MessagesInfo.FirstUnseen), "Message " + imap_e_MessagesInfo.FirstUnseen + " is the first unseen."));
							}
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_UidNext((int)imap_e_MessagesInfo.UidNext), "Predicted next message UID."));
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_UidValidity((long)imap_e_Select.FolderUID), "Folder UID value."));
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Flags(imap_e_Select.Flags.ToArray()));
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_PermanentFlags(imap_e_Select.PermanentFlags.ToArray()), "Avaliable permanent flags."));
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", new IMAP_t_orc_Unknown(imap_e_Select.IsReadOnly ? "READ-ONLY" : "READ-WRITE"), "SELECT completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
							this.m_pSelectedFolder = new IMAP_Session._SelectedFolder(folder, imap_e_Select.IsReadOnly, imap_e_MessagesInfo.MessagesInfo);
							this.m_pSelectedFolder.Reindex();
						}
						else
						{
							this.m_pResponseSender.SendResponseAsync(imap_e_Select.ErrorResponse);
						}
					}
					catch (Exception ex)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: " + ex.Message));
					}
				}
			}
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x00071F24 File Offset: 0x00070F24
		private void EXAMINE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				long ticks = DateTime.Now.Ticks;
				bool flag2 = this.m_pSelectedFolder != null;
				if (flag2)
				{
					this.m_pSelectedFolder = null;
				}
				string[] array = TextUtils.SplitQuotedString(cmdText, ' ');
				bool flag3 = array.Length >= 2;
				if (flag3)
				{
					bool flag4 = string.Equals(array[1], "(UTF8)", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", new IMAP_t_orc_Unknown("NOT-UTF-8"), "Mailbox does not support UTF-8 access."));
					}
					else
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
				}
				else
				{
					string folder = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(cmdText));
					IMAP_e_Select imap_e_Select = this.OnSelect(cmdTag, folder);
					bool flag5 = imap_e_Select.ErrorResponse == null;
					if (flag5)
					{
						IMAP_e_MessagesInfo imap_e_MessagesInfo = this.OnGetMessagesInfo(folder);
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Exists(imap_e_MessagesInfo.Exists));
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Recent(imap_e_MessagesInfo.Recent));
						bool flag6 = imap_e_MessagesInfo.FirstUnseen > -1;
						if (flag6)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_Unseen(imap_e_MessagesInfo.FirstUnseen), "Message " + imap_e_MessagesInfo.FirstUnseen + " is the first unseen."));
						}
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_UidNext((int)imap_e_MessagesInfo.UidNext), "Predicted next message UID."));
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_UidValidity((long)imap_e_Select.FolderUID), "Folder UID value."));
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Flags(imap_e_Select.Flags.ToArray()));
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_ServerStatus("OK", new IMAP_t_orc_PermanentFlags(imap_e_Select.PermanentFlags.ToArray()), "Avaliable permanent flags."));
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", new IMAP_t_orc_ReadOnly(), "EXAMINE completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
						this.m_pSelectedFolder = new IMAP_Session._SelectedFolder(folder, imap_e_Select.IsReadOnly, imap_e_MessagesInfo.MessagesInfo);
						this.m_pSelectedFolder.Reindex();
					}
					else
					{
						this.m_pResponseSender.SendResponseAsync(imap_e_Select.ErrorResponse);
					}
				}
			}
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x000721E4 File Offset: 0x000711E4
		private void APPEND(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				long startTime = DateTime.Now.Ticks;
				StringReader stringReader = new StringReader(cmdText);
				stringReader.ReadToFirstChar();
				bool flag2 = stringReader.StartsWith("\"");
				string folder;
				if (flag2)
				{
					folder = IMAP_Utils.DecodeMailbox(stringReader.ReadWord());
				}
				else
				{
					folder = IMAP_Utils.DecodeMailbox(stringReader.QuotedReadToDelimiter(' '));
				}
				stringReader.ReadToFirstChar();
				List<string> list = new List<string>();
				bool flag3 = stringReader.StartsWith("(");
				if (flag3)
				{
					foreach (string text in stringReader.ReadParenthesized().Split(new char[]
					{
						' '
					}))
					{
						bool flag4 = text.Length > 0 && !list.Contains(text.Substring(1));
						if (flag4)
						{
							list.Add(text.Substring(1));
						}
					}
				}
				stringReader.ReadToFirstChar();
				DateTime date = DateTime.MinValue;
				bool flag5 = !stringReader.StartsWith("{");
				if (flag5)
				{
					date = IMAP_Utils.ParseDate(stringReader.ReadWord());
				}
				int size = Convert.ToInt32(stringReader.ReadParenthesized());
				bool flag6 = stringReader.Available > 0L;
				if (flag6)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
				}
				else
				{
					IMAP_e_Append e = this.OnAppend(folder, list.ToArray(), date, size, new IMAP_r_ServerStatus(cmdTag, "OK", "APPEND command completed in %exectime seconds."));
					bool isError = e.Response.IsError;
					if (isError)
					{
						this.m_pResponseSender.SendResponseAsync(e.Response);
					}
					else
					{
						bool flag7 = e.Stream == null;
						if (flag7)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Internal server error: No storage stream available."));
						}
						else
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus("+", "Ready for literal data."));
							AsyncCallback callback = delegate(IAsyncResult ar)
							{
								try
								{
									this.TcpStream.EndReadFixedCount(ar);
									this.LogAddRead((long)size, "Readed " + size + " bytes.");
									SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
									this.TcpStream.ReadLine(readLineAsyncOP, false);
									bool flag8 = readLineAsyncOP.Error != null;
									if (flag8)
									{
										this.OnError(readLineAsyncOP.Error);
									}
									else
									{
										this.LogAddRead((long)readLineAsyncOP.BytesInBuffer, readLineAsyncOP.LineUtf8);
										e.OnCompleted();
										this.m_pResponseSender.SendResponseAsync(IMAP_r_ServerStatus.Parse(e.Response.ToString().TrimEnd(new char[0]).Replace("%exectime", ((DateTime.Now.Ticks - startTime) / 10000000m).ToString("f2"))));
										this.BeginReadCmd();
									}
								}
								catch (Exception x)
								{
									this.OnError(x);
								}
							};
							this.TcpStream.BeginReadFixedCount(e.Stream, (long)size, callback, null);
						}
					}
				}
			}
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x00072464 File Offset: 0x00071464
		private void GETQUOTAROOT(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("QUOTA");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					bool flag3 = string.IsNullOrEmpty(cmdText);
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
						IMAP_e_GetQuotaRoot imap_e_GetQuotaRoot = this.OnGetGuotaRoot(folder, new IMAP_r_ServerStatus(cmdTag, "OK", "GETQUOTAROOT command completed."));
						bool flag4 = imap_e_GetQuotaRoot.QuotaRootResponses.Count > 0;
						if (flag4)
						{
							foreach (IMAP_r_u_QuotaRoot response in imap_e_GetQuotaRoot.QuotaRootResponses)
							{
								this.m_pResponseSender.SendResponseAsync(response);
							}
						}
						bool flag5 = imap_e_GetQuotaRoot.QuotaResponses.Count > 0;
						if (flag5)
						{
							foreach (IMAP_r_u_Quota response2 in imap_e_GetQuotaRoot.QuotaResponses)
							{
								this.m_pResponseSender.SendResponseAsync(response2);
							}
						}
						this.m_pResponseSender.SendResponseAsync(imap_e_GetQuotaRoot.Response);
					}
				}
			}
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x00072608 File Offset: 0x00071608
		private void GETQUOTA(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				string quotaRoot = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
				IMAP_e_GetQuota imap_e_GetQuota = this.OnGetQuota(quotaRoot, new IMAP_r_ServerStatus(cmdTag, "OK", "QUOTA command completed."));
				bool flag2 = imap_e_GetQuota.QuotaResponses.Count > 0;
				if (flag2)
				{
					foreach (IMAP_r_u_Quota response in imap_e_GetQuota.QuotaResponses)
					{
						this.m_pResponseSender.SendResponseAsync(response);
					}
				}
				this.m_pResponseSender.SendResponseAsync(imap_e_GetQuota.Response);
			}
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x000726E4 File Offset: 0x000716E4
		private void GETACL(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ACL");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
					IMAP_e_GetAcl imap_e_GetAcl = this.OnGetAcl(folder, new IMAP_r_ServerStatus(cmdTag, "OK", "GETACL command completed."));
					bool flag3 = imap_e_GetAcl.AclResponses.Count > 0;
					if (flag3)
					{
						foreach (IMAP_r_u_Acl response in imap_e_GetAcl.AclResponses)
						{
							this.m_pResponseSender.SendResponseAsync(response);
						}
					}
					this.m_pResponseSender.SendResponseAsync(imap_e_GetAcl.Response);
				}
			}
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x000727F4 File Offset: 0x000717F4
		private void SETACL(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ACL");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
					bool flag3 = array.Length != 3;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						string text = array[2];
						IMAP_Flags_SetType flagsSetType = IMAP_Flags_SetType.Replace;
						bool flag4 = text.StartsWith("+");
						if (flag4)
						{
							flagsSetType = IMAP_Flags_SetType.Add;
							text = text.Substring(1);
						}
						else
						{
							bool flag5 = text.StartsWith("-");
							if (flag5)
							{
								flagsSetType = IMAP_Flags_SetType.Remove;
								text = text.Substring(1);
							}
						}
						IMAP_e_SetAcl imap_e_SetAcl = this.OnSetAcl(IMAP_Utils.DecodeMailbox(array[0]), IMAP_Utils.DecodeMailbox(array[1]), flagsSetType, text, new IMAP_r_ServerStatus(cmdTag, "OK", "SETACL command completed."));
						this.m_pResponseSender.SendResponseAsync(imap_e_SetAcl.Response);
					}
				}
			}
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00072920 File Offset: 0x00071920
		private void DELETEACL(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ACL");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
					bool flag3 = array.Length != 2;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						IMAP_e_DeleteAcl imap_e_DeleteAcl = this.OnDeleteAcl(IMAP_Utils.DecodeMailbox(array[0]), IMAP_Utils.DecodeMailbox(array[1]), new IMAP_r_ServerStatus(cmdTag, "OK", "DELETEACL command completed."));
						this.m_pResponseSender.SendResponseAsync(imap_e_DeleteAcl.Response);
					}
				}
			}
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00072A00 File Offset: 0x00071A00
		private void LISTRIGHTS(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ACL");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					string[] array = TextUtils.SplitQuotedString(cmdText, ' ', true);
					bool flag3 = array.Length != 2;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						IMAP_e_ListRights imap_e_ListRights = this.OnListRights(IMAP_Utils.DecodeMailbox(array[0]), IMAP_Utils.DecodeMailbox(array[1]), new IMAP_r_ServerStatus(cmdTag, "OK", "LISTRIGHTS command completed."));
						bool flag4 = imap_e_ListRights.ListRightsResponse != null;
						if (flag4)
						{
							this.m_pResponseSender.SendResponseAsync(imap_e_ListRights.ListRightsResponse);
						}
						this.m_pResponseSender.SendResponseAsync(imap_e_ListRights.Response);
					}
				}
			}
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00072B08 File Offset: 0x00071B08
		private void MYRIGHTS(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ACL");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					string folder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(cmdText));
					IMAP_e_MyRights imap_e_MyRights = this.OnMyRights(folder, new IMAP_r_ServerStatus(cmdTag, "OK", "MYRIGHTS command completed."));
					bool flag3 = imap_e_MyRights.MyRightsResponse != null;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(imap_e_MyRights.MyRightsResponse);
					}
					this.m_pResponseSender.SendResponseAsync(imap_e_MyRights.Response);
				}
			}
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00072BD0 File Offset: 0x00071BD0
		private void ENABLE(string cmdTag, string cmdText)
		{
			bool flag = !this.SupportsCap("ENABLE");
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Command 'ENABLE' not supported."));
			}
			else
			{
				bool flag2 = !base.IsAuthenticated;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				}
				else
				{
					bool flag3 = string.IsNullOrEmpty(cmdText);
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "No arguments, or syntax error in an argument."));
					}
					else
					{
						foreach (string b in cmdText.Split(new char[]
						{
							' '
						}))
						{
							bool flag4 = string.Equals("UTF8=ACCEPT", b, StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								this.m_MailboxEncoding = IMAP_Mailbox_Encoding.ImapUtf8;
								this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Enable(new string[]
								{
									"UTF8=ACCEPT"
								}));
							}
						}
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "ENABLE command completed."));
					}
				}
			}
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x00072CF4 File Offset: 0x00071CF4
		private void CHECK(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					long ticks = DateTime.Now.Ticks;
					this.UpdateSelectedFolderAndSendChanges();
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "CHECK Completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
				}
			}
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x00072DCC File Offset: 0x00071DCC
		private void CLOSE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					bool flag3 = this.m_pSelectedFolder != null && !this.m_pSelectedFolder.IsReadOnly;
					if (flag3)
					{
						foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pSelectedFolder.MessagesInfo)
						{
							bool flag4 = imap_MessageInfo.ContainsFlag("Deleted");
							if (flag4)
							{
								this.OnExpunge(imap_MessageInfo, new IMAP_r_ServerStatus("dummy", "OK", "This is CLOSE command expunge, so this response is not used."));
							}
						}
					}
					this.m_pSelectedFolder = null;
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "CLOSE completed."));
				}
			}
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x00072ED4 File Offset: 0x00071ED4
		private void FETCH(bool uid, string cmdTag, string cmdText)
		{
			long ticks = DateTime.Now.Ticks;
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					}, 2);
					bool flag3 = array.Length != 2;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						IMAP_t_SeqSet seqSet = null;
						try
						{
							seqSet = IMAP_t_SeqSet.Parse(array[0]);
						}
						catch
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments: Invalid 'sequence-set' value."));
							return;
						}
						List<IMAP_t_Fetch_i> dataItems = new List<IMAP_t_Fetch_i>();
						bool flag4 = false;
						string text = array[1].Trim();
						bool flag5 = text.StartsWith("(") && text.EndsWith(")");
						if (flag5)
						{
							text = text.Substring(1, text.Length - 2).Trim();
						}
						text = text.Replace("ALL", "FLAGS INTERNALDATE RFC822.SIZE ENVELOPE");
						text = text.Replace("FAST", "FLAGS INTERNALDATE RFC822.SIZE");
						text = text.Replace("FULL", "FLAGS INTERNALDATE RFC822.SIZE ENVELOPE BODY");
						StringReader stringReader = new StringReader(text);
						IMAP_Fetch_DataType imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
						while (stringReader.Available > 0L)
						{
							stringReader.ReadToFirstChar();
							bool flag6 = stringReader.StartsWith("BODYSTRUCTURE", false);
							if (flag6)
							{
								stringReader.ReadWord();
								dataItems.Add(new IMAP_t_Fetch_i_BodyStructure());
								flag4 = true;
								bool flag7 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage;
								if (flag7)
								{
									imap_Fetch_DataType = IMAP_Fetch_DataType.MessageStructure;
								}
							}
							else
							{
								bool flag8 = stringReader.StartsWith("BODY[", false) || stringReader.StartsWith("BODY.PEEK[", false);
								if (flag8)
								{
									bool flag9 = stringReader.StartsWith("BODY.PEEK[", false);
									stringReader.ReadWord();
									string text2 = stringReader.ReadParenthesized();
									bool flag10 = string.IsNullOrEmpty(text2);
									if (flag10)
									{
										imap_Fetch_DataType = IMAP_Fetch_DataType.FullMessage;
									}
									else
									{
										StringReader stringReader2 = new StringReader(text2);
										string text3 = stringReader2.ReadWord();
										while (text3.Length > 0)
										{
											string[] array2 = text3.Split(new char[]
											{
												'.'
											}, 2);
											bool flag11 = !Net_Utils.IsInteger(array2[0]);
											if (flag11)
											{
												bool flag12 = text3.Equals("HEADER", StringComparison.InvariantCultureIgnoreCase);
												if (flag12)
												{
													bool flag13 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage && imap_Fetch_DataType != IMAP_Fetch_DataType.MessageStructure;
													if (flag13)
													{
														imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
													}
												}
												else
												{
													bool flag14 = text3.Equals("HEADER.FIELDS", StringComparison.InvariantCultureIgnoreCase);
													if (flag14)
													{
														stringReader2.ReadToFirstChar();
														bool flag15 = !stringReader2.StartsWith("(");
														if (flag15)
														{
															this.WriteLine(cmdTag + " BAD Error in arguments.");
															return;
														}
														stringReader2.ReadParenthesized();
														bool flag16 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage && imap_Fetch_DataType != IMAP_Fetch_DataType.MessageStructure;
														if (flag16)
														{
															imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
														}
													}
													else
													{
														bool flag17 = text3.Equals("HEADER.FIELDS.NOT", StringComparison.InvariantCultureIgnoreCase);
														if (flag17)
														{
															stringReader2.ReadToFirstChar();
															bool flag18 = !stringReader2.StartsWith("(");
															if (flag18)
															{
																this.WriteLine(cmdTag + " BAD Error in arguments.");
																return;
															}
															stringReader2.ReadParenthesized();
															bool flag19 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage && imap_Fetch_DataType != IMAP_Fetch_DataType.MessageStructure;
															if (flag19)
															{
																imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
															}
														}
														else
														{
															bool flag20 = text3.Equals("MIME", StringComparison.InvariantCultureIgnoreCase);
															if (flag20)
															{
																imap_Fetch_DataType = IMAP_Fetch_DataType.FullMessage;
															}
															else
															{
																bool flag21 = text3.Equals("TEXT", StringComparison.InvariantCultureIgnoreCase);
																if (!flag21)
																{
																	this.WriteLine(cmdTag + " BAD Error in arguments.");
																	return;
																}
																imap_Fetch_DataType = IMAP_Fetch_DataType.FullMessage;
															}
														}
													}
												}
												break;
											}
											bool flag22 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage;
											if (flag22)
											{
												imap_Fetch_DataType = IMAP_Fetch_DataType.MessageStructure;
											}
											bool flag23 = array2.Length == 2;
											if (flag23)
											{
												text3 = array2[1];
											}
											else
											{
												text3 = "";
											}
										}
									}
									int offset = -1;
									int maxCount = -1;
									bool flag24 = stringReader.StartsWith("<");
									if (flag24)
									{
										string[] array3 = stringReader.ReadParenthesized().Split(new char[]
										{
											'.'
										});
										bool flag25 = array3.Length > 2;
										if (flag25)
										{
											this.WriteLine(cmdTag + " BAD Error in arguments.");
											return;
										}
										bool flag26 = !int.TryParse(array3[0], out offset);
										if (flag26)
										{
											this.WriteLine(cmdTag + " BAD Error in arguments.");
											return;
										}
										bool flag27 = array3.Length == 2;
										if (flag27)
										{
											bool flag28 = !int.TryParse(array3[1], out maxCount);
											if (flag28)
											{
												this.WriteLine(cmdTag + " BAD Error in arguments.");
												return;
											}
										}
									}
									bool flag29 = flag9;
									if (flag29)
									{
										dataItems.Add(new IMAP_t_Fetch_i_BodyPeek(text2, offset, maxCount));
									}
									else
									{
										dataItems.Add(new IMAP_t_Fetch_i_Body(text2, offset, maxCount));
									}
									flag4 = true;
								}
								else
								{
									bool flag30 = stringReader.StartsWith("BODY", false);
									if (flag30)
									{
										stringReader.ReadWord();
										dataItems.Add(new IMAP_t_Fetch_i_BodyS());
										flag4 = true;
										bool flag31 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage;
										if (flag31)
										{
											imap_Fetch_DataType = IMAP_Fetch_DataType.MessageStructure;
										}
									}
									else
									{
										bool flag32 = stringReader.StartsWith("ENVELOPE", false);
										if (flag32)
										{
											stringReader.ReadWord();
											dataItems.Add(new IMAP_t_Fetch_i_Envelope());
											flag4 = true;
											bool flag33 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage && imap_Fetch_DataType != IMAP_Fetch_DataType.MessageStructure;
											if (flag33)
											{
												imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
											}
										}
										else
										{
											bool flag34 = stringReader.StartsWith("FLAGS", false);
											if (flag34)
											{
												stringReader.ReadWord();
												dataItems.Add(new IMAP_t_Fetch_i_Flags());
											}
											else
											{
												bool flag35 = stringReader.StartsWith("INTERNALDATE", false);
												if (flag35)
												{
													stringReader.ReadWord();
													dataItems.Add(new IMAP_t_Fetch_i_InternalDate());
												}
												else
												{
													bool flag36 = stringReader.StartsWith("RFC822.HEADER", false);
													if (flag36)
													{
														stringReader.ReadWord();
														dataItems.Add(new IMAP_t_Fetch_i_Rfc822Header());
														flag4 = true;
														bool flag37 = imap_Fetch_DataType != IMAP_Fetch_DataType.FullMessage && imap_Fetch_DataType != IMAP_Fetch_DataType.MessageStructure;
														if (flag37)
														{
															imap_Fetch_DataType = IMAP_Fetch_DataType.MessageHeader;
														}
													}
													else
													{
														bool flag38 = stringReader.StartsWith("RFC822.SIZE", false);
														if (flag38)
														{
															stringReader.ReadWord();
															dataItems.Add(new IMAP_t_Fetch_i_Rfc822Size());
														}
														else
														{
															bool flag39 = stringReader.StartsWith("RFC822.TEXT", false);
															if (flag39)
															{
																stringReader.ReadWord();
																dataItems.Add(new IMAP_t_Fetch_i_Rfc822Text());
																flag4 = true;
																imap_Fetch_DataType = IMAP_Fetch_DataType.FullMessage;
															}
															else
															{
																bool flag40 = stringReader.StartsWith("RFC822", false);
																if (flag40)
																{
																	stringReader.ReadWord();
																	dataItems.Add(new IMAP_t_Fetch_i_Rfc822());
																	flag4 = true;
																	imap_Fetch_DataType = IMAP_Fetch_DataType.FullMessage;
																}
																else
																{
																	bool flag41 = stringReader.StartsWith("UID", false);
																	if (!flag41)
																	{
																		this.WriteLine(cmdTag + " BAD Error in arguments: Unknown FETCH data-item.");
																		return;
																	}
																	stringReader.ReadWord();
																	dataItems.Add(new IMAP_t_Fetch_i_Uid());
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
						if (uid)
						{
							bool flag42 = true;
							foreach (IMAP_t_Fetch_i imap_t_Fetch_i in dataItems)
							{
								bool flag43 = imap_t_Fetch_i is IMAP_t_Fetch_i_Uid;
								if (flag43)
								{
									flag42 = false;
									break;
								}
							}
							bool flag44 = flag42;
							if (flag44)
							{
								dataItems.Add(new IMAP_t_Fetch_i_Uid());
							}
						}
						this.UpdateSelectedFolderAndSendChanges();
						IMAP_e_Fetch imap_e_Fetch = new IMAP_e_Fetch(this.m_pSelectedFolder.Filter(uid, seqSet), imap_Fetch_DataType, new IMAP_r_ServerStatus(cmdTag, "OK", "FETCH command completed in %exectime seconds."));
						imap_e_Fetch.NewMessageData += delegate(object s, IMAP_e_Fetch.e_NewMessageData e)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append("* " + e.MessageInfo.SeqNo + " FETCH (");
							Mail_Message messageData = e.MessageData;
							for (int j = 0; j < dataItems.Count; j++)
							{
								IMAP_t_Fetch_i imap_t_Fetch_i2 = dataItems[j];
								bool flag46 = j > 0;
								if (flag46)
								{
									stringBuilder.Append(" ");
								}
								bool flag47 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_BodyS;
								if (flag47)
								{
									stringBuilder.Append(this.ConstructBodyStructure(messageData, false));
								}
								else
								{
									bool flag48 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Body || imap_t_Fetch_i2 is IMAP_t_Fetch_i_BodyPeek;
									if (flag48)
									{
										string text4 = "";
										int num = -1;
										int num2 = -1;
										bool flag49 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Body;
										if (flag49)
										{
											text4 = ((IMAP_t_Fetch_i_Body)imap_t_Fetch_i2).Section;
											num = ((IMAP_t_Fetch_i_Body)imap_t_Fetch_i2).Offset;
											num2 = ((IMAP_t_Fetch_i_Body)imap_t_Fetch_i2).MaxCount;
										}
										else
										{
											text4 = ((IMAP_t_Fetch_i_BodyPeek)imap_t_Fetch_i2).Section;
											num = ((IMAP_t_Fetch_i_BodyPeek)imap_t_Fetch_i2).Offset;
											num2 = ((IMAP_t_Fetch_i_BodyPeek)imap_t_Fetch_i2).MaxCount;
										}
										using (MemoryStreamEx memoryStreamEx = new MemoryStreamEx(32000))
										{
											bool flag50 = string.IsNullOrEmpty(text4);
											if (flag50)
											{
												messageData.ToStream(memoryStreamEx, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
												memoryStreamEx.Position = 0L;
											}
											else
											{
												MIME_Entity mimeEntity = this.GetMimeEntity(messageData, this.ParsePartNumberFromSection(text4));
												bool flag51 = mimeEntity != null;
												if (flag51)
												{
													string a = this.ParsePartSpecifierFromSection(text4);
													bool flag52 = string.Equals(a, "HEADER", StringComparison.InvariantCultureIgnoreCase);
													if (flag52)
													{
														mimeEntity.Header.ToStream(memoryStreamEx, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
														bool flag53 = memoryStreamEx.Length > 0L;
														if (flag53)
														{
															memoryStreamEx.WriteByte(13);
															memoryStreamEx.WriteByte(10);
														}
														memoryStreamEx.Position = 0L;
													}
													else
													{
														bool flag54 = string.Equals(a, "HEADER.FIELDS", StringComparison.InvariantCultureIgnoreCase);
														if (flag54)
														{
															string text5 = text4.Split(new char[]
															{
																' '
															}, 2)[1];
															string[] array5 = text5.Substring(1, text5.Length - 2).Split(new char[]
															{
																' '
															});
															foreach (string name in array5)
															{
																MIME_h[] array7 = mimeEntity.Header[name];
																bool flag55 = array7 != null;
																if (flag55)
																{
																	foreach (MIME_h mime_h in array7)
																	{
																		byte[] bytes = Encoding.UTF8.GetBytes(mime_h.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8));
																		memoryStreamEx.Write(bytes, 0, bytes.Length);
																	}
																}
															}
															bool flag56 = memoryStreamEx.Length > 0L;
															if (flag56)
															{
																memoryStreamEx.WriteByte(13);
																memoryStreamEx.WriteByte(10);
															}
															memoryStreamEx.Position = 0L;
														}
														else
														{
															bool flag57 = string.Equals(a, "HEADER.FIELDS.NOT", StringComparison.InvariantCultureIgnoreCase);
															if (flag57)
															{
																string text6 = text4.Split(new char[]
																{
																	' '
																}, 2)[1];
																string[] array9 = text6.Substring(1, text6.Length - 2).Split(new char[]
																{
																	' '
																});
																foreach (object obj in mimeEntity.Header)
																{
																	MIME_h mime_h2 = (MIME_h)obj;
																	bool flag58 = false;
																	foreach (string b in array9)
																	{
																		bool flag59 = string.Equals(mime_h2.Name, b, StringComparison.InvariantCultureIgnoreCase);
																		if (flag59)
																		{
																			flag58 = true;
																			break;
																		}
																	}
																	bool flag60 = !flag58;
																	if (flag60)
																	{
																		byte[] bytes2 = Encoding.UTF8.GetBytes(mime_h2.ToString(new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8));
																		memoryStreamEx.Write(bytes2, 0, bytes2.Length);
																	}
																}
																bool flag61 = memoryStreamEx.Length > 0L;
																if (flag61)
																{
																	memoryStreamEx.WriteByte(13);
																	memoryStreamEx.WriteByte(10);
																}
																memoryStreamEx.Position = 0L;
															}
															else
															{
																bool flag62 = string.Equals(a, "MIME", StringComparison.InvariantCultureIgnoreCase);
																if (flag62)
																{
																	mimeEntity.Header.ToStream(memoryStreamEx, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
																	bool flag63 = memoryStreamEx.Length > 0L;
																	if (flag63)
																	{
																		memoryStreamEx.WriteByte(13);
																		memoryStreamEx.WriteByte(10);
																	}
																	memoryStreamEx.Position = 0L;
																}
																else
																{
																	bool flag64 = string.Equals(a, "TEXT", StringComparison.InvariantCultureIgnoreCase);
																	if (flag64)
																	{
																		mimeEntity.Body.ToStream(memoryStreamEx, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8, false);
																		memoryStreamEx.Position = 0L;
																	}
																	else
																	{
																		mimeEntity.Body.ToStream(memoryStreamEx, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8, false);
																		memoryStreamEx.Position = 0L;
																	}
																}
															}
														}
													}
												}
											}
											bool flag65 = num < 0;
											if (flag65)
											{
												stringBuilder.Append(string.Concat(new object[]
												{
													"BODY[",
													text4,
													"] {",
													memoryStreamEx.Length,
													"}\r\n"
												}));
												this.WriteLine(stringBuilder.ToString());
												stringBuilder = new StringBuilder();
												this.TcpStream.WriteStream(memoryStreamEx);
												this.LogAddWrite(memoryStreamEx.Length, "Wrote " + memoryStreamEx.Length + " bytes.");
											}
											else
											{
												bool flag66 = (long)num >= memoryStreamEx.Length;
												if (flag66)
												{
													stringBuilder.Append(string.Concat(new object[]
													{
														"BODY[",
														text4,
														"]<",
														num,
														"> \"\""
													}));
												}
												else
												{
													memoryStreamEx.Position = (long)num;
													int num3 = (num2 > -1) ? ((int)Math.Min((long)num2, memoryStreamEx.Length - memoryStreamEx.Position)) : ((int)(memoryStreamEx.Length - memoryStreamEx.Position));
													stringBuilder.Append(string.Concat(new object[]
													{
														"BODY[",
														text4,
														"]<",
														num,
														"> {",
														num3,
														"}"
													}));
													this.WriteLine(stringBuilder.ToString());
													stringBuilder = new StringBuilder();
													this.TcpStream.WriteStream(memoryStreamEx, (long)num3);
													this.LogAddWrite(memoryStreamEx.Length, "Wrote " + num3 + " bytes.");
												}
											}
										}
										bool flag67 = !this.m_pSelectedFolder.IsReadOnly && imap_t_Fetch_i2 is IMAP_t_Fetch_i_Body;
										if (flag67)
										{
											try
											{
												this.OnStore(e.MessageInfo, IMAP_Flags_SetType.Add, new string[]
												{
													"Seen"
												}, new IMAP_r_ServerStatus("dummy", "OK", "This is FETCH set Seen flag, this response not used."));
											}
											catch
											{
											}
										}
									}
									else
									{
										bool flag68 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_BodyStructure;
										if (flag68)
										{
											stringBuilder.Append(this.ConstructBodyStructure(messageData, true));
										}
										else
										{
											bool flag69 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Envelope;
											if (flag69)
											{
												stringBuilder.Append(IMAP_t_Fetch_r_i_Envelope.ConstructEnvelope(messageData));
											}
											else
											{
												bool flag70 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Flags;
												if (flag70)
												{
													stringBuilder.Append("FLAGS " + e.MessageInfo.FlagsToImapString());
												}
												else
												{
													bool flag71 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_InternalDate;
													if (flag71)
													{
														stringBuilder.Append("INTERNALDATE \"" + IMAP_Utils.DateTimeToString(e.MessageInfo.InternalDate) + "\"");
													}
													else
													{
														bool flag72 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Rfc822;
														if (flag72)
														{
															using (MemoryStreamEx memoryStreamEx2 = new MemoryStreamEx(32000))
															{
																messageData.ToStream(memoryStreamEx2, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
																memoryStreamEx2.Position = 0L;
																stringBuilder.Append("RFC822 {" + memoryStreamEx2.Length + "}\r\n");
																this.WriteLine(stringBuilder.ToString());
																stringBuilder = new StringBuilder();
																this.TcpStream.WriteStream(memoryStreamEx2);
																this.LogAddWrite(memoryStreamEx2.Length, "Wrote " + memoryStreamEx2.Length + " bytes.");
															}
														}
														else
														{
															bool flag73 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Rfc822Header;
															if (flag73)
															{
																MemoryStream memoryStream = new MemoryStream();
																messageData.Header.ToStream(memoryStream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8);
																memoryStream.Position = 0L;
																stringBuilder.Append("RFC822.HEADER {" + memoryStream.Length + "}\r\n");
																this.WriteLine(stringBuilder.ToString());
																stringBuilder = new StringBuilder();
																this.TcpStream.WriteStream(memoryStream);
																this.LogAddWrite(memoryStream.Length, "Wrote " + memoryStream.Length + " bytes.");
															}
															else
															{
																bool flag74 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Rfc822Size;
																if (flag74)
																{
																	stringBuilder.Append("RFC822.SIZE " + e.MessageInfo.Size);
																}
																else
																{
																	bool flag75 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Rfc822Text;
																	if (flag75)
																	{
																		using (MemoryStreamEx memoryStreamEx3 = new MemoryStreamEx(32000))
																		{
																			messageData.Body.ToStream(memoryStreamEx3, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8), Encoding.UTF8, false);
																			memoryStreamEx3.Position = 0L;
																			stringBuilder.Append("RFC822.TEXT {" + memoryStreamEx3.Length + "}\r\n");
																			this.WriteLine(stringBuilder.ToString());
																			stringBuilder = new StringBuilder();
																			this.TcpStream.WriteStream(memoryStreamEx3);
																			this.LogAddWrite(memoryStreamEx3.Length, "Wrote " + memoryStreamEx3.Length + " bytes.");
																		}
																	}
																	else
																	{
																		bool flag76 = imap_t_Fetch_i2 is IMAP_t_Fetch_i_Uid;
																		if (flag76)
																		{
																			stringBuilder.Append("UID " + e.MessageInfo.UID);
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
							stringBuilder.Append(")\r\n");
							this.WriteLine(stringBuilder.ToString());
						};
						bool flag45 = !flag4;
						if (flag45)
						{
							foreach (IMAP_MessageInfo msgInfo in this.m_pSelectedFolder.Filter(uid, seqSet))
							{
								imap_e_Fetch.AddData(msgInfo);
							}
						}
						else
						{
							this.OnFetch(imap_e_Fetch);
						}
						this.WriteLine(imap_e_Fetch.Response.ToString().Replace("%exectime", ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2")));
					}
				}
			}
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x000737DC File Offset: 0x000727DC
		private void SEARCH(bool uid, string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					long ticks = DateTime.Now.Ticks;
					StringReader stringReader = new StringReader(cmdText);
					bool flag3 = stringReader.StartsWith("CHARSET", false);
					if (flag3)
					{
						stringReader.ReadWord();
						string a = stringReader.ReadWord();
						bool flag4 = !string.Equals(a, "US-ASCII", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(a, "UTF-8", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", new IMAP_t_orc_BadCharset(new string[]
							{
								"US-ASCII",
								"UTF-8"
							}), "Not supported charset."));
							return;
						}
					}
					try
					{
						IMAP_Search_Key_Group criteria = IMAP_Search_Key_Group.Parse(stringReader);
						this.UpdateSelectedFolderAndSendChanges();
						List<int> matchedValues = new List<int>();
						IMAP_e_Search imap_e_Search = new IMAP_e_Search(criteria, new IMAP_r_ServerStatus(cmdTag, "OK", "SEARCH completed in %exectime seconds."));
						imap_e_Search.Matched += delegate(object s, EventArgs<long> e)
						{
							bool uid2 = uid;
							if (uid2)
							{
								matchedValues.Add((int)e.Value);
							}
							else
							{
								int seqNo = this.m_pSelectedFolder.GetSeqNo(e.Value);
								bool flag5 = seqNo != -1;
								if (flag5)
								{
									matchedValues.Add(seqNo);
								}
							}
						};
						this.OnSearch(imap_e_Search);
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Search(matchedValues.ToArray()));
						this.m_pResponseSender.SendResponseAsync(IMAP_r_ServerStatus.Parse(imap_e_Search.Response.ToString().TrimEnd(new char[0]).Replace("%exectime", ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2"))));
					}
					catch
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
				}
			}
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00073A14 File Offset: 0x00072A14
		private void STORE(bool uid, string cmdTag, string cmdText)
		{
			long ticks = DateTime.Now.Ticks;
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					}, 3);
					bool flag3 = array.Length != 3;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						IMAP_t_SeqSet seqSet = null;
						try
						{
							seqSet = IMAP_t_SeqSet.Parse(array[0]);
						}
						catch
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
							return;
						}
						bool flag4 = false;
						bool flag5 = string.Equals(array[1], "FLAGS", StringComparison.InvariantCultureIgnoreCase);
						IMAP_Flags_SetType setType;
						if (flag5)
						{
							setType = IMAP_Flags_SetType.Replace;
						}
						else
						{
							bool flag6 = string.Equals(array[1], "FLAGS.SILENT", StringComparison.InvariantCultureIgnoreCase);
							if (flag6)
							{
								setType = IMAP_Flags_SetType.Replace;
								flag4 = true;
							}
							else
							{
								bool flag7 = string.Equals(array[1], "+FLAGS", StringComparison.InvariantCultureIgnoreCase);
								if (flag7)
								{
									setType = IMAP_Flags_SetType.Add;
								}
								else
								{
									bool flag8 = string.Equals(array[1], "+FLAGS.SILENT", StringComparison.InvariantCultureIgnoreCase);
									if (flag8)
									{
										setType = IMAP_Flags_SetType.Add;
										flag4 = true;
									}
									else
									{
										bool flag9 = string.Equals(array[1], "-FLAGS", StringComparison.InvariantCultureIgnoreCase);
										if (flag9)
										{
											setType = IMAP_Flags_SetType.Remove;
										}
										else
										{
											bool flag10 = string.Equals(array[1], "-FLAGS.SILENT", StringComparison.InvariantCultureIgnoreCase);
											if (!flag10)
											{
												this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
												return;
											}
											setType = IMAP_Flags_SetType.Remove;
											flag4 = true;
										}
									}
								}
							}
						}
						bool flag11 = !array[2].StartsWith("(") || !array[2].EndsWith(")");
						if (flag11)
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
						}
						else
						{
							List<string> list = new List<string>();
							foreach (string text in array[2].Substring(1, array[2].Length - 2).Split(new char[]
							{
								' '
							}))
							{
								bool flag12 = text.Length > 0 && !list.Contains(text.Substring(1));
								if (flag12)
								{
									list.Add(text.Substring(1));
								}
							}
							IMAP_r_ServerStatus imap_r_ServerStatus = new IMAP_r_ServerStatus(cmdTag, "OK", "STORE command completed in %exectime seconds.");
							foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pSelectedFolder.Filter(uid, seqSet))
							{
								IMAP_e_Store imap_e_Store = this.OnStore(imap_MessageInfo, setType, list.ToArray(), imap_r_ServerStatus);
								imap_r_ServerStatus = imap_e_Store.Response;
								bool flag13 = !string.Equals(imap_e_Store.Response.ResponseCode, "OK", StringComparison.InvariantCultureIgnoreCase);
								if (flag13)
								{
									break;
								}
								imap_MessageInfo.UpdateFlags(setType, list.ToArray());
								bool flag14 = !flag4;
								if (flag14)
								{
									if (uid)
									{
										this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Fetch(this.m_pSelectedFolder.GetSeqNo(imap_MessageInfo), new IMAP_t_Fetch_r_i[]
										{
											new IMAP_t_Fetch_r_i_Flags(IMAP_t_MsgFlags.Parse(imap_MessageInfo.FlagsToImapString())),
											new IMAP_t_Fetch_r_i_Uid(imap_MessageInfo.UID)
										}));
									}
									else
									{
										this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Fetch(this.m_pSelectedFolder.GetSeqNo(imap_MessageInfo), new IMAP_t_Fetch_r_i[]
										{
											new IMAP_t_Fetch_r_i_Flags(new IMAP_t_MsgFlags(imap_MessageInfo.Flags))
										}));
									}
								}
							}
							this.m_pResponseSender.SendResponseAsync(IMAP_r_ServerStatus.Parse(imap_r_ServerStatus.ToString().TrimEnd(new char[0]).Replace("%exectime", ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2"))));
						}
					}
				}
			}
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x00073E48 File Offset: 0x00072E48
		private void COPY(bool uid, string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					}, 2);
					bool flag3 = array.Length != 2;
					if (flag3)
					{
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
					}
					else
					{
						IMAP_t_SeqSet seqSet = null;
						try
						{
							seqSet = IMAP_t_SeqSet.Parse(array[0]);
						}
						catch
						{
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
							return;
						}
						string targetFolder = IMAP_Utils.DecodeMailbox(TextUtils.UnQuoteString(array[1]));
						this.UpdateSelectedFolderAndSendChanges();
						IMAP_e_Copy imap_e_Copy = this.OnCopy(targetFolder, this.m_pSelectedFolder.Filter(uid, seqSet), new IMAP_r_ServerStatus(cmdTag, "OK", "COPY completed."));
						this.m_pResponseSender.SendResponseAsync(imap_e_Copy.Response);
					}
				}
			}
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x00073F84 File Offset: 0x00072F84
		private void UID(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					string[] array = cmdText.Split(new char[]
					{
						' '
					}, 2);
					bool flag3 = string.Equals(array[0], "COPY", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						this.COPY(true, cmdTag, array[1]);
					}
					else
					{
						bool flag4 = string.Equals(array[0], "FETCH", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							this.FETCH(true, cmdTag, array[1]);
						}
						else
						{
							bool flag5 = string.Equals(array[0], "STORE", StringComparison.InvariantCultureIgnoreCase);
							if (flag5)
							{
								this.STORE(true, cmdTag, array[1]);
							}
							else
							{
								bool flag6 = string.Equals(array[0], "SEARCH", StringComparison.InvariantCultureIgnoreCase);
								if (flag6)
								{
									this.SEARCH(true, cmdTag, array[1]);
								}
								else
								{
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "BAD", "Error in arguments."));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060012A1 RID: 4769 RVA: 0x000740B4 File Offset: 0x000730B4
		private void EXPUNGE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
			}
			else
			{
				bool flag2 = this.m_pSelectedFolder == null;
				if (flag2)
				{
					this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Error: This command is valid only in selected state."));
				}
				else
				{
					long ticks = DateTime.Now.Ticks;
					IMAP_r_ServerStatus response = new IMAP_r_ServerStatus(cmdTag, "OK", "EXPUNGE completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds.");
					for (int i = 0; i < this.m_pSelectedFolder.MessagesInfo.Length; i++)
					{
						IMAP_MessageInfo imap_MessageInfo = this.m_pSelectedFolder.MessagesInfo[i];
						bool flag3 = imap_MessageInfo.ContainsFlag("Deleted");
						if (flag3)
						{
							IMAP_e_Expunge imap_e_Expunge = this.OnExpunge(imap_MessageInfo, response);
							bool flag4 = !string.Equals(imap_e_Expunge.Response.ResponseCode, "OK", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								this.m_pResponseSender.SendResponseAsync(imap_e_Expunge.Response);
								return;
							}
							this.m_pSelectedFolder.RemoveMessage(imap_MessageInfo);
							this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Expunge(i + 1));
						}
					}
					this.m_pSelectedFolder.Reindex();
					this.m_pResponseSender.SendResponseAsync(response);
				}
			}
		}

		// Token: 0x060012A2 RID: 4770 RVA: 0x00074248 File Offset: 0x00073248
		private bool IDLE(string cmdTag, string cmdText)
		{
			bool flag = !base.IsAuthenticated;
			bool result;
			if (flag)
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "NO", "Authentication required."));
				result = true;
			}
			else
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus("+", "idling"));
				TimerEx timer = new TimerEx(30000.0, true);
				timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
				{
					try
					{
						this.UpdateSelectedFolderAndSendChanges();
					}
					catch
					{
					}
				};
				timer.Enabled = true;
				SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
				readLineOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
				{
					try
					{
						bool flag4 = readLineOP.Error != null;
						if (flag4)
						{
							this.LogAddText("Error: " + readLineOP.Error.Message);
							timer.Dispose();
						}
						else
						{
							bool flag5 = readLineOP.BytesInBuffer == 0;
							if (flag5)
							{
								this.LogAddText("Remote host(connected client) closed IMAP connection.");
								timer.Dispose();
								this.Dispose();
							}
							else
							{
								this.LogAddRead((long)readLineOP.BytesInBuffer, readLineOP.LineUtf8);
								bool flag6 = string.Equals(readLineOP.LineUtf8, "DONE", StringComparison.InvariantCultureIgnoreCase);
								if (flag6)
								{
									timer.Dispose();
									this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "IDLE terminated."));
									this.BeginReadCmd();
								}
								else
								{
									while (this.TcpStream.ReadLine(readLineOP, true))
									{
										bool flag7 = readLineOP.Error != null;
										if (flag7)
										{
											this.LogAddText("Error: " + readLineOP.Error.Message);
											timer.Dispose();
											break;
										}
										this.LogAddRead((long)readLineOP.BytesInBuffer, readLineOP.LineUtf8);
										bool flag8 = string.Equals(readLineOP.LineUtf8, "DONE", StringComparison.InvariantCultureIgnoreCase);
										if (flag8)
										{
											timer.Dispose();
											this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "IDLE terminated."));
											this.BeginReadCmd();
											break;
										}
									}
								}
							}
						}
					}
					catch (Exception x)
					{
						timer.Dispose();
						this.OnError(x);
					}
				};
				while (this.TcpStream.ReadLine(readLineOP, true))
				{
					bool flag2 = readLineOP.Error != null;
					if (flag2)
					{
						this.LogAddText("Error: " + readLineOP.Error.Message);
						timer.Dispose();
						break;
					}
					this.LogAddRead((long)readLineOP.BytesInBuffer, readLineOP.LineUtf8);
					bool flag3 = string.Equals(readLineOP.LineUtf8, "DONE", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						timer.Dispose();
						this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "IDLE terminated."));
						this.BeginReadCmd();
						break;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x00074400 File Offset: 0x00073400
		private void CAPABILITY(string cmdTag, string cmdText)
		{
			List<string> list = new List<string>();
			bool flag = !this.IsSecureConnection && base.Certificate != null;
			if (flag)
			{
				list.Add("STARTTLS");
			}
			foreach (string item in this.m_pCapabilities)
			{
				list.Add(item);
			}
			foreach (AUTH_SASL_ServerMechanism auth_SASL_ServerMechanism in this.Authentications.Values)
			{
				list.Add("AUTH=" + auth_SASL_ServerMechanism.Name);
			}
			this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Capability(list.ToArray()));
			this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "CAPABILITY completed."));
		}

		// Token: 0x060012A4 RID: 4772 RVA: 0x00074518 File Offset: 0x00073518
		private void NOOP(string cmdTag, string cmdText)
		{
			long ticks = DateTime.Now.Ticks;
			bool flag = this.m_pSelectedFolder != null;
			if (flag)
			{
				this.UpdateSelectedFolderAndSendChanges();
			}
			this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "NOOP Completed in " + ((DateTime.Now.Ticks - ticks) / 10000000m).ToString("f2") + " seconds."));
		}

		// Token: 0x060012A5 RID: 4773 RVA: 0x000745A0 File Offset: 0x000735A0
		private void LOGOUT(string cmdTag, string cmdText)
		{
			try
			{
				this.m_pResponseSender.SendResponseAsync(new IMAP_r_u_Bye("IMAP4rev1 Server logging out."));
				EventHandler<EventArgs<Exception>> completedAsyncCallback = delegate(object s, EventArgs<Exception> e)
				{
					try
					{
						this.Disconnect();
						this.Dispose();
					}
					catch
					{
					}
				};
				bool flag = !this.m_pResponseSender.SendResponseAsync(new IMAP_r_ServerStatus(cmdTag, "OK", "LOGOUT completed."), completedAsyncCallback);
				if (flag)
				{
					this.Disconnect();
					this.Dispose();
				}
			}
			catch
			{
				this.Disconnect();
				this.Dispose();
			}
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x0007462C File Offset: 0x0007362C
		private void WriteLine(string line)
		{
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			bool flag2 = line.EndsWith("\r\n");
			byte[] bytes;
			if (flag2)
			{
				bytes = Encoding.UTF8.GetBytes(line);
			}
			else
			{
				bytes = Encoding.UTF8.GetBytes(line + "\r\n");
			}
			this.TcpStream.Write(bytes, 0, bytes.Length);
			bool flag3 = this.Server.Logger != null;
			if (flag3)
			{
				this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, (long)bytes.Length, line, this.LocalEndPoint, this.RemoteEndPoint);
			}
		}

		// Token: 0x060012A7 RID: 4775 RVA: 0x000746DC File Offset: 0x000736DC
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

		// Token: 0x060012A8 RID: 4776 RVA: 0x00074744 File Offset: 0x00073744
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

		// Token: 0x060012A9 RID: 4777 RVA: 0x000747AC File Offset: 0x000737AC
		public void LogAddText(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			try
			{
				bool flag2 = this.Server.Logger != null;
				if (flag2)
				{
					this.Server.Logger.AddText(this.ID, this.AuthenticatedUserIdentity, text, this.LocalEndPoint, this.RemoteEndPoint);
				}
			}
			catch
			{
			}
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x00074828 File Offset: 0x00073828
		public void LogAddException(Exception exception)
		{
			bool flag = exception == null;
			if (flag)
			{
				throw new ArgumentNullException("exception");
			}
			try
			{
				bool flag2 = this.Server.Logger != null;
				if (flag2)
				{
					this.Server.Logger.AddException(this.ID, this.AuthenticatedUserIdentity, exception.Message, this.LocalEndPoint, this.RemoteEndPoint, exception);
				}
			}
			catch
			{
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x000748A8 File Offset: 0x000738A8
		private void UpdateSelectedFolderAndSendChanges()
		{
			bool flag = this.m_pSelectedFolder == null;
			if (!flag)
			{
				IMAP_e_MessagesInfo imap_e_MessagesInfo = this.OnGetMessagesInfo(this.m_pSelectedFolder.Folder);
				int num = this.m_pSelectedFolder.MessagesInfo.Length;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (IMAP_MessageInfo imap_MessageInfo in imap_e_MessagesInfo.MessagesInfo)
				{
					dictionary.Add(imap_MessageInfo.ID, null);
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IMAP_MessageInfo imap_MessageInfo2 in this.m_pSelectedFolder.MessagesInfo)
				{
					bool flag2 = !dictionary.ContainsKey(imap_MessageInfo2.ID);
					if (flag2)
					{
						stringBuilder.Append("* " + this.m_pSelectedFolder.GetSeqNo(imap_MessageInfo2) + " EXPUNGE\r\n");
						this.m_pSelectedFolder.RemoveMessage(imap_MessageInfo2);
					}
				}
				bool flag3 = num != imap_e_MessagesInfo.MessagesInfo.Count;
				if (flag3)
				{
					stringBuilder.Append("* " + imap_e_MessagesInfo.MessagesInfo.Count + " EXISTS\r\n");
				}
				bool flag4 = stringBuilder.Length > 0;
				if (flag4)
				{
					this.WriteLine(stringBuilder.ToString());
				}
				this.m_pSelectedFolder = new IMAP_Session._SelectedFolder(this.m_pSelectedFolder.Folder, this.m_pSelectedFolder.IsReadOnly, imap_e_MessagesInfo.MessagesInfo);
			}
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x00074A48 File Offset: 0x00073A48
		private bool SupportsCap(string capability)
		{
			foreach (string a in this.m_pCapabilities)
			{
				bool flag = string.Equals(a, capability, StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060012AD RID: 4781 RVA: 0x00074AB0 File Offset: 0x00073AB0
		private string ParsePartNumberFromSection(string section)
		{
			bool flag = section == null;
			if (flag)
			{
				throw new ArgumentNullException("section");
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = section.Split(new char[]
			{
				'.'
			});
			foreach (string value in array)
			{
				bool flag2 = Net_Utils.IsInteger(value);
				if (!flag2)
				{
					break;
				}
				bool flag3 = stringBuilder.Length > 0;
				if (flag3)
				{
					stringBuilder.Append(".");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060012AE RID: 4782 RVA: 0x00074B50 File Offset: 0x00073B50
		private string ParsePartSpecifierFromSection(string section)
		{
			bool flag = section == null;
			if (flag)
			{
				throw new ArgumentNullException("section");
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = section.Split(new char[]
			{
				' '
			})[0].Split(new char[]
			{
				'.'
			});
			foreach (string value in array)
			{
				bool flag2 = !Net_Utils.IsInteger(value);
				if (flag2)
				{
					bool flag3 = stringBuilder.Length > 0;
					if (flag3)
					{
						stringBuilder.Append(".");
					}
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00074C00 File Offset: 0x00073C00
		public MIME_Entity GetMimeEntity(Mail_Message message, string partNumber)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			bool flag2 = partNumber == string.Empty;
			MIME_Entity result;
			if (flag2)
			{
				result = message;
			}
			else
			{
				bool flag3 = message.ContentType == null || message.ContentType.Type.ToLower() != "multipart";
				if (flag3)
				{
					bool flag4 = Convert.ToInt32(partNumber) == 1;
					if (flag4)
					{
						result = message;
					}
					else
					{
						result = null;
					}
				}
				else
				{
					MIME_Entity mime_Entity = message;
					string[] array = partNumber.Split(new char[]
					{
						'.'
					});
					foreach (string value in array)
					{
						int num = Convert.ToInt32(value) - 1;
						bool flag5 = mime_Entity.Body is MIME_b_Multipart;
						if (!flag5)
						{
							return null;
						}
						MIME_b_Multipart mime_b_Multipart = (MIME_b_Multipart)mime_Entity.Body;
						bool flag6 = num > -1 && num < mime_b_Multipart.BodyParts.Count;
						if (!flag6)
						{
							return null;
						}
						mime_Entity = mime_b_Multipart.BodyParts[num];
					}
					result = mime_Entity;
				}
			}
			return result;
		}

		// Token: 0x060012B0 RID: 4784 RVA: 0x00074D34 File Offset: 0x00073D34
		public string ConstructBodyStructure(Mail_Message message, bool bodystructure)
		{
			string result;
			if (bodystructure)
			{
				result = "BODYSTRUCTURE " + this.ConstructParts(message, bodystructure);
			}
			else
			{
				result = "BODY " + this.ConstructParts(message, bodystructure);
			}
			return result;
		}

		// Token: 0x060012B1 RID: 4785 RVA: 0x00074D74 File Offset: 0x00073D74
		private string ConstructParts(MIME_Entity entity, bool bodystructure)
		{
			MIME_Encoding_EncodedWord mime_Encoding_EncodedWord = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);
			mime_Encoding_EncodedWord.Split = false;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = entity.Body is MIME_b_Multipart;
			if (flag)
			{
				stringBuilder.Append("(");
				foreach (object obj in ((MIME_b_Multipart)entity.Body).BodyParts)
				{
					MIME_Entity entity2 = (MIME_Entity)obj;
					stringBuilder.Append(this.ConstructParts(entity2, bodystructure));
				}
				bool flag2 = entity.ContentType != null && entity.ContentType.SubType != null;
				if (flag2)
				{
					stringBuilder.Append(" \"" + entity.ContentType.SubType + "\"");
				}
				else
				{
					stringBuilder.Append(" \"plain\"");
				}
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append("(");
				bool flag3 = entity.ContentType != null && entity.ContentType.Type != null;
				if (flag3)
				{
					stringBuilder.Append("\"" + entity.ContentType.Type + "\"");
				}
				else
				{
					stringBuilder.Append("\"text\"");
				}
				bool flag4 = entity.ContentType != null && entity.ContentType.SubType != null;
				if (flag4)
				{
					stringBuilder.Append(" \"" + entity.ContentType.SubType + "\"");
				}
				else
				{
					stringBuilder.Append(" \"plain\"");
				}
				bool flag5 = entity.ContentType != null;
				if (flag5)
				{
					bool flag6 = entity.ContentType.Parameters.Count > 0;
					if (flag6)
					{
						stringBuilder.Append(" (");
						bool flag7 = true;
						foreach (object obj2 in entity.ContentType.Parameters)
						{
							MIME_h_Parameter mime_h_Parameter = (MIME_h_Parameter)obj2;
							bool flag8 = flag7;
							if (flag8)
							{
								flag7 = false;
							}
							else
							{
								stringBuilder.Append(" ");
							}
							stringBuilder.Append(string.Concat(new string[]
							{
								"\"",
								mime_h_Parameter.Name,
								"\" \"",
								mime_Encoding_EncodedWord.Encode(mime_h_Parameter.Value),
								"\""
							}));
						}
						stringBuilder.Append(")");
					}
					else
					{
						stringBuilder.Append(" NIL");
					}
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				string contentID = entity.ContentID;
				bool flag9 = contentID != null;
				if (flag9)
				{
					stringBuilder.Append(" \"" + mime_Encoding_EncodedWord.Encode(contentID) + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				string contentDescription = entity.ContentDescription;
				bool flag10 = contentDescription != null;
				if (flag10)
				{
					stringBuilder.Append(" \"" + mime_Encoding_EncodedWord.Encode(contentDescription) + "\"");
				}
				else
				{
					stringBuilder.Append(" NIL");
				}
				bool flag11 = entity.ContentTransferEncoding != null;
				if (flag11)
				{
					stringBuilder.Append(" \"" + mime_Encoding_EncodedWord.Encode(entity.ContentTransferEncoding) + "\"");
				}
				else
				{
					stringBuilder.Append(" \"7bit\"");
				}
				bool flag12 = entity.Body is MIME_b_SinglepartBase;
				if (flag12)
				{
					stringBuilder.Append(" " + ((MIME_b_SinglepartBase)entity.Body).EncodedData.Length.ToString());
				}
				else
				{
					stringBuilder.Append(" 0");
				}
				bool flag13 = entity.Body is MIME_b_MessageRfc822;
				if (flag13)
				{
					stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructEnvelope(((MIME_b_MessageRfc822)entity.Body).Message));
					stringBuilder.Append(" NIL");
					stringBuilder.Append(" NIL");
				}
				bool flag14 = entity.Body is MIME_b_Text;
				if (flag14)
				{
					long num = 0L;
					StreamLineReader streamLineReader = new StreamLineReader(new MemoryStream(((MIME_b_SinglepartBase)entity.Body).EncodedData));
					for (byte[] array = streamLineReader.ReadLine(); array != null; array = streamLineReader.ReadLine())
					{
						num += 1L;
					}
					stringBuilder.Append(" " + num.ToString());
				}
				if (bodystructure)
				{
					stringBuilder.Append(" NIL");
					bool flag15 = entity.ContentDisposition != null && entity.ContentDisposition.Parameters.Count > 0;
					if (flag15)
					{
						stringBuilder.Append(" (\"" + entity.ContentDisposition.DispositionType + "\"");
						bool flag16 = entity.ContentDisposition.Parameters.Count > 0;
						if (flag16)
						{
							stringBuilder.Append(" (");
							bool flag17 = true;
							foreach (object obj3 in entity.ContentDisposition.Parameters)
							{
								MIME_h_Parameter mime_h_Parameter2 = (MIME_h_Parameter)obj3;
								bool flag18 = flag17;
								if (flag18)
								{
									flag17 = false;
								}
								else
								{
									stringBuilder.Append(" ");
								}
								stringBuilder.Append(string.Concat(new string[]
								{
									"\"",
									mime_h_Parameter2.Name,
									"\" \"",
									mime_Encoding_EncodedWord.Encode(mime_h_Parameter2.Value),
									"\""
								}));
							}
							stringBuilder.Append(")");
						}
						else
						{
							stringBuilder.Append(" NIL");
						}
						stringBuilder.Append(")");
					}
					else
					{
						stringBuilder.Append(" NIL");
					}
					stringBuilder.Append(" NIL");
					stringBuilder.Append(" NIL");
				}
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x000753EC File Offset: 0x000743EC
		public new IMAP_Server Server
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (IMAP_Server)base.Server;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x00075428 File Offset: 0x00074428
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

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x060012B4 RID: 4788 RVA: 0x0007545C File Offset: 0x0007445C
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

		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00075490 File Offset: 0x00074490
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

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x060012B6 RID: 4790 RVA: 0x000754C4 File Offset: 0x000744C4
		public List<string> Capabilities
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pCapabilities;
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x060012B7 RID: 4791 RVA: 0x000754F8 File Offset: 0x000744F8
		public string SelectedFolderName
		{
			get
			{
				bool flag = this.m_pSelectedFolder == null;
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.m_pSelectedFolder.Folder;
				}
				return result;
			}
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x060012B8 RID: 4792 RVA: 0x00075528 File Offset: 0x00074528
		internal IMAP_Mailbox_Encoding MailboxEncoding
		{
			get
			{
				return this.m_MailboxEncoding;
			}
		}

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x060012B9 RID: 4793 RVA: 0x00075540 File Offset: 0x00074540
		// (remove) Token: 0x060012BA RID: 4794 RVA: 0x00075578 File Offset: 0x00074578
		
		public event EventHandler<IMAP_e_Started> Started = null;

		// Token: 0x060012BB RID: 4795 RVA: 0x000755B0 File Offset: 0x000745B0
		private IMAP_e_Started OnStarted(IMAP_r_u_ServerStatus response)
		{
			IMAP_e_Started imap_e_Started = new IMAP_e_Started(response);
			bool flag = this.Started != null;
			if (flag)
			{
				this.Started(this, imap_e_Started);
			}
			return imap_e_Started;
		}

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x060012BC RID: 4796 RVA: 0x000755E8 File Offset: 0x000745E8
		// (remove) Token: 0x060012BD RID: 4797 RVA: 0x00075620 File Offset: 0x00074620
		
		public event EventHandler<IMAP_e_Login> Login = null;

		// Token: 0x060012BE RID: 4798 RVA: 0x00075658 File Offset: 0x00074658
		private IMAP_e_Login OnLogin(string user, string password)
		{
			IMAP_e_Login imap_e_Login = new IMAP_e_Login(user, password);
			bool flag = this.Login != null;
			if (flag)
			{
				this.Login(this, imap_e_Login);
			}
			return imap_e_Login;
		}

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x060012BF RID: 4799 RVA: 0x00075690 File Offset: 0x00074690
		// (remove) Token: 0x060012C0 RID: 4800 RVA: 0x000756C8 File Offset: 0x000746C8
		
		public event EventHandler<IMAP_e_Namespace> Namespace = null;

		// Token: 0x060012C1 RID: 4801 RVA: 0x00075700 File Offset: 0x00074700
		private IMAP_e_Namespace OnNamespace(IMAP_r_ServerStatus response)
		{
			IMAP_e_Namespace imap_e_Namespace = new IMAP_e_Namespace(response);
			bool flag = this.Namespace != null;
			if (flag)
			{
				this.Namespace(this, imap_e_Namespace);
			}
			return imap_e_Namespace;
		}

		// Token: 0x14000060 RID: 96
		// (add) Token: 0x060012C2 RID: 4802 RVA: 0x00075738 File Offset: 0x00074738
		// (remove) Token: 0x060012C3 RID: 4803 RVA: 0x00075770 File Offset: 0x00074770
		
		public event EventHandler<IMAP_e_List> List = null;

		// Token: 0x060012C4 RID: 4804 RVA: 0x000757A8 File Offset: 0x000747A8
		private IMAP_e_List OnList(string refName, string folder)
		{
			IMAP_e_List imap_e_List = new IMAP_e_List(refName, folder);
			bool flag = this.List != null;
			if (flag)
			{
				this.List(this, imap_e_List);
			}
			return imap_e_List;
		}

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x060012C5 RID: 4805 RVA: 0x000757E0 File Offset: 0x000747E0
		// (remove) Token: 0x060012C6 RID: 4806 RVA: 0x00075818 File Offset: 0x00074818
		
		public event EventHandler<IMAP_e_Folder> Create = null;

		// Token: 0x060012C7 RID: 4807 RVA: 0x00075850 File Offset: 0x00074850
		private IMAP_e_Folder OnCreate(string cmdTag, string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_Folder imap_e_Folder = new IMAP_e_Folder(cmdTag, folder, response);
			bool flag = this.Create != null;
			if (flag)
			{
				this.Create(this, imap_e_Folder);
			}
			return imap_e_Folder;
		}

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x060012C8 RID: 4808 RVA: 0x0007588C File Offset: 0x0007488C
		// (remove) Token: 0x060012C9 RID: 4809 RVA: 0x000758C4 File Offset: 0x000748C4
		
		public event EventHandler<IMAP_e_Folder> Delete = null;

		// Token: 0x060012CA RID: 4810 RVA: 0x000758FC File Offset: 0x000748FC
		private IMAP_e_Folder OnDelete(string cmdTag, string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_Folder imap_e_Folder = new IMAP_e_Folder(cmdTag, folder, response);
			bool flag = this.Delete != null;
			if (flag)
			{
				this.Delete(this, imap_e_Folder);
			}
			return imap_e_Folder;
		}

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x060012CB RID: 4811 RVA: 0x00075938 File Offset: 0x00074938
		// (remove) Token: 0x060012CC RID: 4812 RVA: 0x00075970 File Offset: 0x00074970
		
		public event EventHandler<IMAP_e_Rename> Rename = null;

		// Token: 0x060012CD RID: 4813 RVA: 0x000759A8 File Offset: 0x000749A8
		private IMAP_e_Rename OnRename(string cmdTag, string currentFolder, string newFolder)
		{
			IMAP_e_Rename imap_e_Rename = new IMAP_e_Rename(cmdTag, currentFolder, newFolder);
			bool flag = this.Rename != null;
			if (flag)
			{
				this.Rename(this, imap_e_Rename);
			}
			return imap_e_Rename;
		}

		// Token: 0x14000064 RID: 100
		// (add) Token: 0x060012CE RID: 4814 RVA: 0x000759E4 File Offset: 0x000749E4
		// (remove) Token: 0x060012CF RID: 4815 RVA: 0x00075A1C File Offset: 0x00074A1C
		
		public event EventHandler<IMAP_e_LSub> LSub = null;

		// Token: 0x060012D0 RID: 4816 RVA: 0x00075A54 File Offset: 0x00074A54
		private IMAP_e_LSub OnLSub(string refName, string folder)
		{
			IMAP_e_LSub imap_e_LSub = new IMAP_e_LSub(refName, folder);
			bool flag = this.LSub != null;
			if (flag)
			{
				this.LSub(this, imap_e_LSub);
			}
			return imap_e_LSub;
		}

		// Token: 0x14000065 RID: 101
		// (add) Token: 0x060012D1 RID: 4817 RVA: 0x00075A8C File Offset: 0x00074A8C
		// (remove) Token: 0x060012D2 RID: 4818 RVA: 0x00075AC4 File Offset: 0x00074AC4
		
		public event EventHandler<IMAP_e_Folder> Subscribe = null;

		// Token: 0x060012D3 RID: 4819 RVA: 0x00075AFC File Offset: 0x00074AFC
		private IMAP_e_Folder OnSubscribe(string cmdTag, string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_Folder imap_e_Folder = new IMAP_e_Folder(cmdTag, folder, response);
			bool flag = this.Subscribe != null;
			if (flag)
			{
				this.Subscribe(this, imap_e_Folder);
			}
			return imap_e_Folder;
		}

		// Token: 0x14000066 RID: 102
		// (add) Token: 0x060012D4 RID: 4820 RVA: 0x00075B38 File Offset: 0x00074B38
		// (remove) Token: 0x060012D5 RID: 4821 RVA: 0x00075B70 File Offset: 0x00074B70
		
		public event EventHandler<IMAP_e_Folder> Unsubscribe = null;

		// Token: 0x060012D6 RID: 4822 RVA: 0x00075BA8 File Offset: 0x00074BA8
		private IMAP_e_Folder OnUnsubscribe(string cmdTag, string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_Folder imap_e_Folder = new IMAP_e_Folder(cmdTag, folder, response);
			bool flag = this.Unsubscribe != null;
			if (flag)
			{
				this.Unsubscribe(this, imap_e_Folder);
			}
			return imap_e_Folder;
		}

		// Token: 0x14000067 RID: 103
		// (add) Token: 0x060012D7 RID: 4823 RVA: 0x00075BE4 File Offset: 0x00074BE4
		// (remove) Token: 0x060012D8 RID: 4824 RVA: 0x00075C1C File Offset: 0x00074C1C
		
		public event EventHandler<IMAP_e_Select> Select = null;

		// Token: 0x060012D9 RID: 4825 RVA: 0x00075C54 File Offset: 0x00074C54
		private IMAP_e_Select OnSelect(string cmdTag, string folder)
		{
			IMAP_e_Select imap_e_Select = new IMAP_e_Select(cmdTag, folder);
			bool flag = this.Select != null;
			if (flag)
			{
				this.Select(this, imap_e_Select);
			}
			return imap_e_Select;
		}

		// Token: 0x14000068 RID: 104
		// (add) Token: 0x060012DA RID: 4826 RVA: 0x00075C8C File Offset: 0x00074C8C
		// (remove) Token: 0x060012DB RID: 4827 RVA: 0x00075CC4 File Offset: 0x00074CC4
		
		public event EventHandler<IMAP_e_MessagesInfo> GetMessagesInfo = null;

		// Token: 0x060012DC RID: 4828 RVA: 0x00075CFC File Offset: 0x00074CFC
		private IMAP_e_MessagesInfo OnGetMessagesInfo(string folder)
		{
			IMAP_e_MessagesInfo imap_e_MessagesInfo = new IMAP_e_MessagesInfo(folder);
			bool flag = this.GetMessagesInfo != null;
			if (flag)
			{
				this.GetMessagesInfo(this, imap_e_MessagesInfo);
			}
			return imap_e_MessagesInfo;
		}

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x060012DD RID: 4829 RVA: 0x00075D34 File Offset: 0x00074D34
		// (remove) Token: 0x060012DE RID: 4830 RVA: 0x00075D6C File Offset: 0x00074D6C
		
		public event EventHandler<IMAP_e_Append> Append = null;

		// Token: 0x060012DF RID: 4831 RVA: 0x00075DA4 File Offset: 0x00074DA4
		private IMAP_e_Append OnAppend(string folder, string[] flags, DateTime date, int size, IMAP_r_ServerStatus response)
		{
			IMAP_e_Append imap_e_Append = new IMAP_e_Append(folder, flags, date, size, response);
			bool flag = this.Append != null;
			if (flag)
			{
				this.Append(this, imap_e_Append);
			}
			return imap_e_Append;
		}

		// Token: 0x1400006A RID: 106
		// (add) Token: 0x060012E0 RID: 4832 RVA: 0x00075DE4 File Offset: 0x00074DE4
		// (remove) Token: 0x060012E1 RID: 4833 RVA: 0x00075E1C File Offset: 0x00074E1C
		
		public event EventHandler<IMAP_e_GetQuotaRoot> GetQuotaRoot = null;

		// Token: 0x060012E2 RID: 4834 RVA: 0x00075E54 File Offset: 0x00074E54
		private IMAP_e_GetQuotaRoot OnGetGuotaRoot(string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_GetQuotaRoot imap_e_GetQuotaRoot = new IMAP_e_GetQuotaRoot(folder, response);
			bool flag = this.GetQuotaRoot != null;
			if (flag)
			{
				this.GetQuotaRoot(this, imap_e_GetQuotaRoot);
			}
			return imap_e_GetQuotaRoot;
		}

		// Token: 0x1400006B RID: 107
		// (add) Token: 0x060012E3 RID: 4835 RVA: 0x00075E8C File Offset: 0x00074E8C
		// (remove) Token: 0x060012E4 RID: 4836 RVA: 0x00075EC4 File Offset: 0x00074EC4
		
		public event EventHandler<IMAP_e_GetQuota> GetQuota = null;

		// Token: 0x060012E5 RID: 4837 RVA: 0x00075EFC File Offset: 0x00074EFC
		private IMAP_e_GetQuota OnGetQuota(string quotaRoot, IMAP_r_ServerStatus response)
		{
			IMAP_e_GetQuota imap_e_GetQuota = new IMAP_e_GetQuota(quotaRoot, response);
			bool flag = this.GetQuota != null;
			if (flag)
			{
				this.GetQuota(this, imap_e_GetQuota);
			}
			return imap_e_GetQuota;
		}

		// Token: 0x1400006C RID: 108
		// (add) Token: 0x060012E6 RID: 4838 RVA: 0x00075F34 File Offset: 0x00074F34
		// (remove) Token: 0x060012E7 RID: 4839 RVA: 0x00075F6C File Offset: 0x00074F6C
		
		public event EventHandler<IMAP_e_GetAcl> GetAcl = null;

		// Token: 0x060012E8 RID: 4840 RVA: 0x00075FA4 File Offset: 0x00074FA4
		private IMAP_e_GetAcl OnGetAcl(string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_GetAcl imap_e_GetAcl = new IMAP_e_GetAcl(folder, response);
			bool flag = this.GetAcl != null;
			if (flag)
			{
				this.GetAcl(this, imap_e_GetAcl);
			}
			return imap_e_GetAcl;
		}

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x060012E9 RID: 4841 RVA: 0x00075FDC File Offset: 0x00074FDC
		// (remove) Token: 0x060012EA RID: 4842 RVA: 0x00076014 File Offset: 0x00075014
		
		public event EventHandler<IMAP_e_SetAcl> SetAcl = null;

		// Token: 0x060012EB RID: 4843 RVA: 0x0007604C File Offset: 0x0007504C
		private IMAP_e_SetAcl OnSetAcl(string folder, string identifier, IMAP_Flags_SetType flagsSetType, string rights, IMAP_r_ServerStatus response)
		{
			IMAP_e_SetAcl imap_e_SetAcl = new IMAP_e_SetAcl(folder, identifier, flagsSetType, rights, response);
			bool flag = this.SetAcl != null;
			if (flag)
			{
				this.SetAcl(this, imap_e_SetAcl);
			}
			return imap_e_SetAcl;
		}

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x060012EC RID: 4844 RVA: 0x0007608C File Offset: 0x0007508C
		// (remove) Token: 0x060012ED RID: 4845 RVA: 0x000760C4 File Offset: 0x000750C4
		
		public event EventHandler<IMAP_e_DeleteAcl> DeleteAcl = null;

		// Token: 0x060012EE RID: 4846 RVA: 0x000760FC File Offset: 0x000750FC
		private IMAP_e_DeleteAcl OnDeleteAcl(string folder, string identifier, IMAP_r_ServerStatus response)
		{
			IMAP_e_DeleteAcl imap_e_DeleteAcl = new IMAP_e_DeleteAcl(folder, identifier, response);
			bool flag = this.DeleteAcl != null;
			if (flag)
			{
				this.DeleteAcl(this, imap_e_DeleteAcl);
			}
			return imap_e_DeleteAcl;
		}

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x060012EF RID: 4847 RVA: 0x00076138 File Offset: 0x00075138
		// (remove) Token: 0x060012F0 RID: 4848 RVA: 0x00076170 File Offset: 0x00075170
		
		public event EventHandler<IMAP_e_ListRights> ListRights = null;

		// Token: 0x060012F1 RID: 4849 RVA: 0x000761A8 File Offset: 0x000751A8
		private IMAP_e_ListRights OnListRights(string folder, string identifier, IMAP_r_ServerStatus response)
		{
			IMAP_e_ListRights imap_e_ListRights = new IMAP_e_ListRights(folder, identifier, response);
			bool flag = this.ListRights != null;
			if (flag)
			{
				this.ListRights(this, imap_e_ListRights);
			}
			return imap_e_ListRights;
		}

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x060012F2 RID: 4850 RVA: 0x000761E4 File Offset: 0x000751E4
		// (remove) Token: 0x060012F3 RID: 4851 RVA: 0x0007621C File Offset: 0x0007521C
		
		public event EventHandler<IMAP_e_MyRights> MyRights = null;

		// Token: 0x060012F4 RID: 4852 RVA: 0x00076254 File Offset: 0x00075254
		private IMAP_e_MyRights OnMyRights(string folder, IMAP_r_ServerStatus response)
		{
			IMAP_e_MyRights imap_e_MyRights = new IMAP_e_MyRights(folder, response);
			bool flag = this.MyRights != null;
			if (flag)
			{
				this.MyRights(this, imap_e_MyRights);
			}
			return imap_e_MyRights;
		}

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x060012F5 RID: 4853 RVA: 0x0007628C File Offset: 0x0007528C
		// (remove) Token: 0x060012F6 RID: 4854 RVA: 0x000762C4 File Offset: 0x000752C4
		
		public event EventHandler<IMAP_e_Fetch> Fetch = null;

		// Token: 0x060012F7 RID: 4855 RVA: 0x000762FC File Offset: 0x000752FC
		private void OnFetch(IMAP_e_Fetch e)
		{
			bool flag = this.Fetch != null;
			if (flag)
			{
				this.Fetch(this, e);
			}
		}

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x060012F8 RID: 4856 RVA: 0x00076328 File Offset: 0x00075328
		// (remove) Token: 0x060012F9 RID: 4857 RVA: 0x00076360 File Offset: 0x00075360
		
		public event EventHandler<IMAP_e_Search> Search = null;

		// Token: 0x060012FA RID: 4858 RVA: 0x00076398 File Offset: 0x00075398
		private void OnSearch(IMAP_e_Search e)
		{
			bool flag = this.Search != null;
			if (flag)
			{
				this.Search(this, e);
			}
		}

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x060012FB RID: 4859 RVA: 0x000763C4 File Offset: 0x000753C4
		// (remove) Token: 0x060012FC RID: 4860 RVA: 0x000763FC File Offset: 0x000753FC
		
		public event EventHandler<IMAP_e_Store> Store = null;

		// Token: 0x060012FD RID: 4861 RVA: 0x00076434 File Offset: 0x00075434
		private IMAP_e_Store OnStore(IMAP_MessageInfo msgInfo, IMAP_Flags_SetType setType, string[] flags, IMAP_r_ServerStatus response)
		{
			IMAP_e_Store imap_e_Store = new IMAP_e_Store(this.m_pSelectedFolder.Folder, msgInfo, setType, flags, response);
			bool flag = this.Store != null;
			if (flag)
			{
				this.Store(this, imap_e_Store);
			}
			return imap_e_Store;
		}

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x060012FE RID: 4862 RVA: 0x0007647C File Offset: 0x0007547C
		// (remove) Token: 0x060012FF RID: 4863 RVA: 0x000764B4 File Offset: 0x000754B4
		
		public event EventHandler<IMAP_e_Copy> Copy = null;

		// Token: 0x06001300 RID: 4864 RVA: 0x000764EC File Offset: 0x000754EC
		private IMAP_e_Copy OnCopy(string targetFolder, IMAP_MessageInfo[] messagesInfo, IMAP_r_ServerStatus response)
		{
			IMAP_e_Copy imap_e_Copy = new IMAP_e_Copy(this.m_pSelectedFolder.Folder, targetFolder, messagesInfo, response);
			bool flag = this.Copy != null;
			if (flag)
			{
				this.Copy(this, imap_e_Copy);
			}
			return imap_e_Copy;
		}

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06001301 RID: 4865 RVA: 0x00076530 File Offset: 0x00075530
		// (remove) Token: 0x06001302 RID: 4866 RVA: 0x00076568 File Offset: 0x00075568
		
		public event EventHandler<IMAP_e_Expunge> Expunge = null;

		// Token: 0x06001303 RID: 4867 RVA: 0x000765A0 File Offset: 0x000755A0
		private IMAP_e_Expunge OnExpunge(IMAP_MessageInfo msgInfo, IMAP_r_ServerStatus response)
		{
			IMAP_e_Expunge imap_e_Expunge = new IMAP_e_Expunge(this.m_pSelectedFolder.Folder, msgInfo, response);
			bool flag = this.Expunge != null;
			if (flag)
			{
				this.Expunge(this, imap_e_Expunge);
			}
			return imap_e_Expunge;
		}

		// Token: 0x04000742 RID: 1858
		private Dictionary<string, AUTH_SASL_ServerMechanism> m_pAuthentications = null;

		// Token: 0x04000743 RID: 1859
		private bool m_SessionRejected = false;

		// Token: 0x04000744 RID: 1860
		private int m_BadCommands = 0;

		// Token: 0x04000745 RID: 1861
		private List<string> m_pCapabilities = null;

		// Token: 0x04000746 RID: 1862
		private char m_FolderSeparator = '/';

		// Token: 0x04000747 RID: 1863
		private GenericIdentity m_pUser = null;

		// Token: 0x04000748 RID: 1864
		private IMAP_Session._SelectedFolder m_pSelectedFolder = null;

		// Token: 0x04000749 RID: 1865
		private IMAP_Mailbox_Encoding m_MailboxEncoding = IMAP_Mailbox_Encoding.ImapUtf7;

		// Token: 0x0400074A RID: 1866
		private IMAP_Session.ResponseSender m_pResponseSender = null;

		// Token: 0x02000318 RID: 792
		private class _SelectedFolder
		{
			// Token: 0x06001A27 RID: 6695 RVA: 0x000A0D0C File Offset: 0x0009FD0C
			public _SelectedFolder(string folder, bool isReadOnly, List<IMAP_MessageInfo> messagesInfo)
			{
				bool flag = folder == null;
				if (flag)
				{
					throw new ArgumentNullException("folder");
				}
				bool flag2 = messagesInfo == null;
				if (flag2)
				{
					throw new ArgumentNullException("messagesInfo");
				}
				this.m_Folder = folder;
				this.m_IsReadOnly = isReadOnly;
				this.m_pMessagesInfo = messagesInfo;
				this.Reindex();
			}

			// Token: 0x06001A28 RID: 6696 RVA: 0x000A0D7C File Offset: 0x0009FD7C
			internal IMAP_MessageInfo[] Filter(bool uid, IMAP_t_SeqSet seqSet)
			{
				bool flag = seqSet == null;
				if (flag)
				{
					throw new ArgumentNullException("seqSet");
				}
				List<IMAP_MessageInfo> list = new List<IMAP_MessageInfo>();
				for (int i = 0; i < this.m_pMessagesInfo.Count; i++)
				{
					IMAP_MessageInfo imap_MessageInfo = this.m_pMessagesInfo[i];
					if (uid)
					{
						bool flag2 = seqSet.Contains(imap_MessageInfo.UID);
						if (flag2)
						{
							list.Add(imap_MessageInfo);
						}
					}
					else
					{
						bool flag3 = seqSet.Contains((long)(i + 1));
						if (flag3)
						{
							list.Add(imap_MessageInfo);
						}
					}
				}
				return list.ToArray();
			}

			// Token: 0x06001A29 RID: 6697 RVA: 0x000A0E20 File Offset: 0x0009FE20
			internal void RemoveMessage(IMAP_MessageInfo message)
			{
				bool flag = message == null;
				if (flag)
				{
					throw new ArgumentNullException("message");
				}
				this.m_pMessagesInfo.Remove(message);
			}

			// Token: 0x06001A2A RID: 6698 RVA: 0x000A0E50 File Offset: 0x0009FE50
			internal int GetSeqNo(IMAP_MessageInfo msgInfo)
			{
				bool flag = msgInfo == null;
				if (flag)
				{
					throw new ArgumentNullException("msgInfo");
				}
				return this.m_pMessagesInfo.IndexOf(msgInfo) + 1;
			}

			// Token: 0x06001A2B RID: 6699 RVA: 0x000A0E84 File Offset: 0x0009FE84
			internal int GetSeqNo(long uid)
			{
				foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pMessagesInfo)
				{
					bool flag = imap_MessageInfo.UID == uid;
					if (flag)
					{
						return imap_MessageInfo.SeqNo;
					}
				}
				return -1;
			}

			// Token: 0x06001A2C RID: 6700 RVA: 0x000A0EF4 File Offset: 0x0009FEF4
			internal void Reindex()
			{
				for (int i = 0; i < this.m_pMessagesInfo.Count; i++)
				{
					this.m_pMessagesInfo[i].SeqNo = i + 1;
				}
			}

			// Token: 0x1700085D RID: 2141
			// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000A0F34 File Offset: 0x0009FF34
			public string Folder
			{
				get
				{
					return this.m_Folder;
				}
			}

			// Token: 0x1700085E RID: 2142
			// (get) Token: 0x06001A2E RID: 6702 RVA: 0x000A0F4C File Offset: 0x0009FF4C
			public bool IsReadOnly
			{
				get
				{
					return this.m_IsReadOnly;
				}
			}

			// Token: 0x1700085F RID: 2143
			// (get) Token: 0x06001A2F RID: 6703 RVA: 0x000A0F64 File Offset: 0x0009FF64
			internal IMAP_MessageInfo[] MessagesInfo
			{
				get
				{
					return this.m_pMessagesInfo.ToArray();
				}
			}

			// Token: 0x04000BD6 RID: 3030
			private string m_Folder = null;

			// Token: 0x04000BD7 RID: 3031
			private bool m_IsReadOnly = false;

			// Token: 0x04000BD8 RID: 3032
			private List<IMAP_MessageInfo> m_pMessagesInfo = null;
		}

		// Token: 0x02000319 RID: 793
		private class _CmdReader
		{
			// Token: 0x06001A30 RID: 6704 RVA: 0x000A0F84 File Offset: 0x0009FF84
			public _CmdReader(IMAP_Session session, string initialCmdLine, Encoding charset)
			{
				bool flag = session == null;
				if (flag)
				{
					throw new ArgumentNullException("session");
				}
				bool flag2 = initialCmdLine == null;
				if (flag2)
				{
					throw new ArgumentNullException("initialCmdLine");
				}
				bool flag3 = charset == null;
				if (flag3)
				{
					throw new ArgumentNullException("charset");
				}
				this.m_pSession = session;
				this.m_InitialCmdLine = initialCmdLine;
				this.m_pCharset = charset;
			}

			// Token: 0x06001A31 RID: 6705 RVA: 0x000A1014 File Offset: 0x000A0014
			public void Start()
			{
				bool flag = IMAP_Session._CmdReader.EndsWithLiteralString(this.m_InitialCmdLine);
				if (flag)
				{
					StringBuilder stringBuilder = new StringBuilder();
					int literalSize = this.GetLiteralSize(this.m_InitialCmdLine);
					stringBuilder.Append(this.RemoveLiteralSpecifier(this.m_InitialCmdLine));
					SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
					for (;;)
					{
						bool flag2 = literalSize > this.m_MaxLiteralSize;
						if (flag2)
						{
							break;
						}
						this.m_pSession.WriteLine("+ Continue.");
						MemoryStream memoryStream = new MemoryStream();
						this.m_pSession.TcpStream.ReadFixedCount(memoryStream, (long)literalSize);
						this.m_pSession.LogAddRead((long)literalSize, this.m_pCharset.GetString(memoryStream.ToArray()));
						stringBuilder.Append(TextUtils.QuoteString(this.m_pCharset.GetString(memoryStream.ToArray())));
						this.m_pSession.TcpStream.ReadLine(readLineAsyncOP, false);
						bool flag3 = readLineAsyncOP.Error != null;
						if (flag3)
						{
							goto Block_3;
						}
						string lineUtf = readLineAsyncOP.LineUtf8;
						this.m_pSession.LogAddRead((long)readLineAsyncOP.BytesInBuffer, lineUtf);
						bool flag4 = IMAP_Session._CmdReader.EndsWithLiteralString(lineUtf);
						if (flag4)
						{
							stringBuilder.Append(this.RemoveLiteralSpecifier(lineUtf));
						}
						else
						{
							stringBuilder.Append(lineUtf);
						}
						bool flag5 = !IMAP_Session._CmdReader.EndsWithLiteralString(lineUtf);
						if (flag5)
						{
							goto Block_5;
						}
						literalSize = this.GetLiteralSize(lineUtf);
					}
					throw new DataSizeExceededException();
					Block_3:
					throw readLineAsyncOP.Error;
					Block_5:
					this.m_CmdLine = stringBuilder.ToString();
				}
				else
				{
					this.m_CmdLine = this.m_InitialCmdLine;
				}
			}

			// Token: 0x06001A32 RID: 6706 RVA: 0x000A11AC File Offset: 0x000A01AC
			public static bool EndsWithLiteralString(string value)
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.EndsWith("}");
				if (flag2)
				{
					int num = 0;
					char[] array = value.ToCharArray();
					for (int i = array.Length - 2; i >= 0; i--)
					{
						bool flag3 = array[i] == '{';
						if (flag3)
						{
							break;
						}
						bool flag4 = char.IsDigit(array[i]);
						if (!flag4)
						{
							return false;
						}
						num++;
					}
					bool flag5 = num > 0;
					if (flag5)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06001A33 RID: 6707 RVA: 0x000A124C File Offset: 0x000A024C
			private int GetLiteralSize(string cmdLine)
			{
				bool flag = cmdLine == null;
				if (flag)
				{
					throw new ArgumentNullException("cmdLine");
				}
				return Convert.ToInt32(cmdLine.Substring(cmdLine.LastIndexOf('{') + 1, cmdLine.Length - cmdLine.LastIndexOf('{') - 2));
			}

			// Token: 0x06001A34 RID: 6708 RVA: 0x000A1298 File Offset: 0x000A0298
			private string RemoveLiteralSpecifier(string value)
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				return value.Substring(0, value.LastIndexOf('{'));
			}

			// Token: 0x17000860 RID: 2144
			// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000A12D0 File Offset: 0x000A02D0
			public string CmdLine
			{
				get
				{
					return this.m_CmdLine;
				}
			}

			// Token: 0x04000BD9 RID: 3033
			private IMAP_Session m_pSession = null;

			// Token: 0x04000BDA RID: 3034
			private string m_InitialCmdLine = null;

			// Token: 0x04000BDB RID: 3035
			private Encoding m_pCharset = null;

			// Token: 0x04000BDC RID: 3036
			private string m_CmdLine = null;

			// Token: 0x04000BDD RID: 3037
			private int m_MaxLiteralSize = 32000;
		}

		// Token: 0x0200031A RID: 794
		private class ResponseSender
		{
			// Token: 0x06001A36 RID: 6710 RVA: 0x000A12E8 File Offset: 0x000A02E8
			public ResponseSender(IMAP_Session session)
			{
				bool flag = session == null;
				if (flag)
				{
					throw new ArgumentNullException("session");
				}
				this.m_pImap = session;
				this.m_pResponses = new Queue<IMAP_Session.ResponseSender.QueueItem>();
			}

			// Token: 0x06001A37 RID: 6711 RVA: 0x000091B8 File Offset: 0x000081B8
			public void Dispose()
			{
			}

			// Token: 0x06001A38 RID: 6712 RVA: 0x000A1344 File Offset: 0x000A0344
			public void SendResponseAsync(IMAP_r response)
			{
				bool flag = response == null;
				if (flag)
				{
					throw new ArgumentNullException("response");
				}
				this.SendResponseAsync(response, null);
			}

			// Token: 0x06001A39 RID: 6713 RVA: 0x000A1370 File Offset: 0x000A0370
			public bool SendResponseAsync(IMAP_r response, EventHandler<EventArgs<Exception>> completedAsyncCallback)
			{
				bool flag = response == null;
				if (flag)
				{
					throw new ArgumentNullException("response");
				}
				IMAP_Session.ResponseSender.QueueItem queueItem = new IMAP_Session.ResponseSender.QueueItem(response, completedAsyncCallback);
				this.m_pResponses.Enqueue(queueItem);
				this.SendResponsesAsync(false);
				return !queueItem.IsSent;
			}

			// Token: 0x06001A3A RID: 6714 RVA: 0x000A13BC File Offset: 0x000A03BC
			private void SendResponsesAsync(bool calledFromAsync)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool flag2 = this.m_IsSending || this.m_pResponses.Count == 0;
					if (flag2)
					{
						return;
					}
					this.m_IsSending = true;
				}
				IMAP_Session.ResponseSender.QueueItem responseItem = null;
				EventHandler<EventArgs<Exception>> completedAsyncCallback = delegate(object s, EventArgs<Exception> e)
				{
					try
					{
						bool flag7 = responseItem.CompletedAsyncCallback != null;
						if (flag7)
						{
							responseItem.CompletedAsyncCallback(this, e);
						}
						this.m_IsSending = false;
						this.SendResponsesAsync(true);
					}
					catch (Exception x2)
					{
						this.m_pImap.OnError(x2);
						this.m_IsSending = false;
					}
				};
				try
				{
					while (this.m_pResponses.Count > 0)
					{
						responseItem = this.m_pResponses.Dequeue();
						bool flag3 = responseItem.Response.SendAsync(this.m_pImap, completedAsyncCallback);
						if (flag3)
						{
							break;
						}
						responseItem.IsSent = true;
						bool flag4 = calledFromAsync && responseItem.CompletedAsyncCallback != null;
						if (flag4)
						{
							responseItem.CompletedAsyncCallback(this, new EventArgs<Exception>(null));
						}
						object pLock2 = this.m_pLock;
						lock (pLock2)
						{
							bool flag6 = this.m_pResponses.Count == 0;
							if (flag6)
							{
								this.m_IsSending = false;
								break;
							}
						}
					}
				}
				catch (Exception x)
				{
					this.m_pImap.OnError(x);
					this.m_IsSending = false;
				}
			}

			// Token: 0x04000BDE RID: 3038
			private object m_pLock = new object();

			// Token: 0x04000BDF RID: 3039
			private IMAP_Session m_pImap = null;

			// Token: 0x04000BE0 RID: 3040
			private bool m_IsSending = false;

			// Token: 0x04000BE1 RID: 3041
			private Queue<IMAP_Session.ResponseSender.QueueItem> m_pResponses = null;

			// Token: 0x020003DB RID: 987
			private class QueueItem
			{
				// Token: 0x06001C74 RID: 7284 RVA: 0x000AD6C8 File Offset: 0x000AC6C8
				public QueueItem(IMAP_r response, EventHandler<EventArgs<Exception>> completedAsyncCallback)
				{
					bool flag = response == null;
					if (flag)
					{
						throw new ArgumentNullException("response");
					}
					this.m_pResponse = response;
					this.m_pCompletedAsyncCallback = completedAsyncCallback;
				}

				// Token: 0x170008AE RID: 2222
				// (get) Token: 0x06001C75 RID: 7285 RVA: 0x000AD714 File Offset: 0x000AC714
				// (set) Token: 0x06001C76 RID: 7286 RVA: 0x000AD72C File Offset: 0x000AC72C
				public bool IsSent
				{
					get
					{
						return this.m_IsSent;
					}
					set
					{
						this.m_IsSent = value;
					}
				}

				// Token: 0x170008AF RID: 2223
				// (get) Token: 0x06001C77 RID: 7287 RVA: 0x000AD738 File Offset: 0x000AC738
				public IMAP_r Response
				{
					get
					{
						return this.m_pResponse;
					}
				}

				// Token: 0x170008B0 RID: 2224
				// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000AD750 File Offset: 0x000AC750
				public EventHandler<EventArgs<Exception>> CompletedAsyncCallback
				{
					get
					{
						return this.m_pCompletedAsyncCallback;
					}
				}

				// Token: 0x04000DE6 RID: 3558
				private bool m_IsSent = false;

				// Token: 0x04000DE7 RID: 3559
				private IMAP_r m_pResponse = null;

				// Token: 0x04000DE8 RID: 3560
				private EventHandler<EventArgs<Exception>> m_pCompletedAsyncCallback = null;
			}
		}
	}
}
