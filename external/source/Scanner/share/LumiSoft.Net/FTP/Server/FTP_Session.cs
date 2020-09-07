using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Timers;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000245 RID: 581
	public class FTP_Session : TCP_ServerSession
	{
		// Token: 0x060014E4 RID: 5348 RVA: 0x00081F28 File Offset: 0x00080F28
		public override void Dispose()
		{
			bool isDisposed = base.IsDisposed;
			if (!isDisposed)
			{
				base.Dispose();
				bool flag = this.m_pDataConnection != null;
				if (flag)
				{
					this.m_pDataConnection.Dispose();
					this.m_pDataConnection = null;
				}
				bool flag2 = this.m_pPassiveSocket != null;
				if (flag2)
				{
					this.m_pPassiveSocket.Close();
					this.m_pPassiveSocket = null;
				}
			}
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00081F90 File Offset: 0x00080F90
		protected override void Start()
		{
			base.Start();
			try
			{
				bool flag = string.IsNullOrEmpty(this.Server.GreetingText);
				string text;
				if (flag)
				{
					text = "220 [" + Net_Utils.GetLocalHostName(base.LocalHostName) + "] FTP Service Ready.";
				}
				else
				{
					text = "220 " + this.Server.GreetingText;
				}
				FTP_e_Started ftp_e_Started = this.OnStarted(text);
				bool flag2 = !string.IsNullOrEmpty(ftp_e_Started.Response);
				if (flag2)
				{
					this.WriteLine(text.ToString());
				}
				bool flag3 = string.IsNullOrEmpty(ftp_e_Started.Response) || ftp_e_Started.Response.ToUpper().StartsWith("500");
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

		// Token: 0x060014E6 RID: 5350 RVA: 0x0008207C File Offset: 0x0008107C
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

		// Token: 0x060014E7 RID: 5351 RVA: 0x00082128 File Offset: 0x00081128
		protected override void OnTimeout()
		{
			try
			{
				this.WriteLine("500 Idle timeout, closing connection.");
			}
			catch
			{
			}
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x0008215C File Offset: 0x0008115C
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

		// Token: 0x060014E9 RID: 5353 RVA: 0x00082208 File Offset: 0x00081208
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
				string argsText = (array.Length == 2) ? array[1] : "";
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
				bool flag5 = text == "AUTH";
				if (flag5)
				{
					this.USER(argsText);
				}
				else
				{
					bool flag6 = text == "USER";
					if (flag6)
					{
						this.USER(argsText);
					}
					else
					{
						bool flag7 = text == "PASS";
						if (flag7)
						{
							this.PASS(argsText);
						}
						else
						{
							bool flag8 = text == "CWD" || text == "XCWD";
							if (flag8)
							{
								this.CWD(argsText);
							}
							else
							{
								bool flag9 = text == "CDUP" || text == "XCUP";
								if (flag9)
								{
									this.CDUP(argsText);
								}
								else
								{
									bool flag10 = text == "PWD" || text == "XPWD";
									if (flag10)
									{
										this.PWD(argsText);
									}
									else
									{
										bool flag11 = text == "ABOR";
										if (flag11)
										{
											this.ABOR(argsText);
										}
										else
										{
											bool flag12 = text == "RETR";
											if (flag12)
											{
												this.RETR(argsText);
											}
											else
											{
												bool flag13 = text == "STOR";
												if (flag13)
												{
													this.STOR(argsText);
												}
												else
												{
													bool flag14 = text == "DELE";
													if (flag14)
													{
														this.DELE(argsText);
													}
													else
													{
														bool flag15 = text == "APPE";
														if (flag15)
														{
															this.APPE(argsText);
														}
														else
														{
															bool flag16 = text == "SIZE";
															if (flag16)
															{
																this.SIZE(argsText);
															}
															else
															{
																bool flag17 = text == "RNFR";
																if (flag17)
																{
																	this.RNFR(argsText);
																}
																else
																{
																	bool flag18 = text == "DELE";
																	if (flag18)
																	{
																		this.DELE(argsText);
																	}
																	else
																	{
																		bool flag19 = text == "RNTO";
																		if (flag19)
																		{
																			this.RNTO(argsText);
																		}
																		else
																		{
																			bool flag20 = text == "RMD" || text == "XRMD";
																			if (flag20)
																			{
																				this.RMD(argsText);
																			}
																			else
																			{
																				bool flag21 = text == "MKD" || text == "XMKD";
																				if (flag21)
																				{
																					this.MKD(argsText);
																				}
																				else
																				{
																					bool flag22 = text == "LIST";
																					if (flag22)
																					{
																						this.LIST(argsText);
																					}
																					else
																					{
																						bool flag23 = text == "NLST";
																						if (flag23)
																						{
																							this.NLST(argsText);
																						}
																						else
																						{
																							bool flag24 = text == "TYPE";
																							if (flag24)
																							{
																								this.TYPE(argsText);
																							}
																							else
																							{
																								bool flag25 = text == "PORT";
																								if (flag25)
																								{
																									this.PORT(argsText);
																								}
																								else
																								{
																									bool flag26 = text == "PASV";
																									if (flag26)
																									{
																										this.PASV(argsText);
																									}
																									else
																									{
																										bool flag27 = text == "SYST";
																										if (flag27)
																										{
																											this.SYST(argsText);
																										}
																										else
																										{
																											bool flag28 = text == "NOOP";
																											if (flag28)
																											{
																												this.NOOP(argsText);
																											}
																											else
																											{
																												bool flag29 = text == "QUIT";
																												if (flag29)
																												{
																													this.QUIT(argsText);
																													result = false;
																												}
																												else
																												{
																													bool flag30 = text == "FEAT";
																													if (flag30)
																													{
																														this.FEAT(argsText);
																													}
																													else
																													{
																														bool flag31 = text == "OPTS";
																														if (flag31)
																														{
																															this.OPTS(argsText);
																														}
																														else
																														{
																															this.m_BadCommands++;
																															bool flag32 = this.Server.MaxBadCommands != 0 && this.m_BadCommands > this.Server.MaxBadCommands;
																															if (flag32)
																															{
																																this.WriteLine("500 Too many bad commands, closing transmission channel.");
																																this.Disconnect();
																																return false;
																															}
																															this.WriteLine("500 Error: command '" + text + "' not recognized.");
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

		// Token: 0x060014EA RID: 5354 RVA: 0x000827B8 File Offset: 0x000817B8
		private void AUTH(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = base.Certificate == null;
				if (flag)
				{
					this.WriteLine("500 TLS not available: Server has no SSL certificate.");
				}
				else
				{
					bool flag2 = string.Equals("TLS", argsText);
					if (flag2)
					{
						this.WriteLine("234 Ready to start TLS.");
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
					else
					{
						this.WriteLine("500 Error in arguments.");
					}
				}
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0008287C File Offset: 0x0008187C
		private void USER(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.WriteLine("500 You are already authenticated");
				}
				else
				{
					bool flag = !string.IsNullOrEmpty(this.m_UserName);
					if (flag)
					{
						this.WriteLine("500 username is already specified, please specify password");
					}
					else
					{
						string[] array = argsText.Split(new char[]
						{
							' '
						});
						bool flag2 = argsText.Length > 0 && array.Length == 1;
						if (flag2)
						{
							string text = array[0];
							this.WriteLine("331 Password required or user:'" + text + "'");
							this.m_UserName = text;
						}
						else
						{
							this.WriteLine("500 Syntax error. Syntax:{USER username}");
						}
					}
				}
			}
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00082944 File Offset: 0x00081944
		private void PASS(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool isAuthenticated = base.IsAuthenticated;
				if (isAuthenticated)
				{
					this.WriteLine("500 You are already authenticated");
				}
				else
				{
					bool flag = this.m_UserName.Length == 0;
					if (flag)
					{
						this.WriteLine("503 please specify username first");
					}
					else
					{
						string[] array = argsText.Split(new char[]
						{
							' '
						});
						bool flag2 = array.Length == 1;
						if (flag2)
						{
							string password = array[0];
							bool isAuthenticated2 = this.OnAuthenticate(this.m_UserName, password).IsAuthenticated;
							if (isAuthenticated2)
							{
								this.WriteLine("230 Password ok");
								this.m_pUser = new GenericIdentity(this.m_UserName, "FTP-USER/PASS");
							}
							else
							{
								this.WriteLine("530 UserName or Password is incorrect");
								this.m_UserName = "";
							}
						}
						else
						{
							this.WriteLine("500 Syntax error. Syntax:{PASS userName}");
						}
					}
				}
			}
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00082A3C File Offset: 0x00081A3C
		private void CWD(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					FTP_e_Cwd ftp_e_Cwd = new FTP_e_Cwd(argsText);
					this.OnCwd(ftp_e_Cwd);
					bool flag2 = ftp_e_Cwd.Response == null;
					if (flag2)
					{
						this.WriteLine("500 Internal server error: FTP server didn't provide response for CWD command.");
					}
					else
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Cwd.Response)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
				}
			}
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x00082AE0 File Offset: 0x00081AE0
		private void CDUP(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = !string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Error in arguments.");
					}
					FTP_e_Cdup ftp_e_Cdup = new FTP_e_Cdup();
					this.OnCdup(ftp_e_Cdup);
					bool flag3 = ftp_e_Cdup.Response == null;
					if (flag3)
					{
						this.WriteLine("500 Internal server error: FTP server didn't provide response for CDUP command.");
					}
					else
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Cdup.Response)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
				}
			}
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x00082BA4 File Offset: 0x00081BA4
		private void PWD(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					this.WriteLine("257 \"" + this.m_CurrentDir + "\" is current directory.");
				}
			}
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x00082C04 File Offset: 0x00081C04
		private void ABOR(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = !string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Error in arguments. !");
					}
					else
					{
						bool flag3 = this.m_pDataConnection != null;
						if (flag3)
						{
							this.m_pDataConnection.Abort();
						}
						this.WriteLine("226 ABOR command successful.");
					}
				}
			}
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x00082C8C File Offset: 0x00081C8C
		private void RETR(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid file name. !");
					}
					else
					{
						FTP_e_GetFile ftp_e_GetFile = new FTP_e_GetFile(argsText);
						this.OnGetFile(ftp_e_GetFile);
						bool flag3 = ftp_e_GetFile.Error != null;
						if (flag3)
						{
							foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_GetFile.Error)
							{
								this.WriteLine(ftp_t_ReplyLine.ToString());
							}
						}
						else
						{
							bool flag4 = ftp_e_GetFile.FileStream == null;
							if (flag4)
							{
								this.WriteLine("500 Internal server error: File stream not provided by server.");
							}
							else
							{
								this.m_pDataConnection = new FTP_Session.DataConnection(this, ftp_e_GetFile.FileStream, false);
								this.m_pDataConnection.Start();
							}
						}
					}
				}
			}
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00082D88 File Offset: 0x00081D88
		private void STOR(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid file name.");
					}
					FTP_e_Stor ftp_e_Stor = new FTP_e_Stor(argsText);
					this.OnStor(ftp_e_Stor);
					bool flag3 = ftp_e_Stor.Error != null;
					if (flag3)
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Stor.Error)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
					else
					{
						bool flag4 = ftp_e_Stor.FileStream == null;
						if (flag4)
						{
							this.WriteLine("500 Internal server error: File stream not provided by server.");
						}
						else
						{
							this.m_pDataConnection = new FTP_Session.DataConnection(this, ftp_e_Stor.FileStream, true);
							this.m_pDataConnection.Start();
						}
					}
				}
			}
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00082E80 File Offset: 0x00081E80
		private void DELE(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid file name.");
					}
					FTP_e_Dele ftp_e_Dele = new FTP_e_Dele(argsText);
					this.OnDele(ftp_e_Dele);
					bool flag3 = ftp_e_Dele.Response == null;
					if (flag3)
					{
						this.WriteLine("500 Internal server error: FTP server didn't provide response for DELE command.");
					}
					else
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Dele.Response)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
				}
			}
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00082F44 File Offset: 0x00081F44
		private void APPE(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid file name.");
					}
					FTP_e_Appe ftp_e_Appe = new FTP_e_Appe(argsText);
					this.OnAppe(ftp_e_Appe);
					bool flag3 = ftp_e_Appe.Error != null;
					if (flag3)
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Appe.Error)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
					else
					{
						bool flag4 = ftp_e_Appe.FileStream == null;
						if (flag4)
						{
							this.WriteLine("500 Internal server error: File stream not provided by server.");
						}
						else
						{
							this.m_pDataConnection = new FTP_Session.DataConnection(this, ftp_e_Appe.FileStream, true);
							this.m_pDataConnection.Start();
						}
					}
				}
			}
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x0008303C File Offset: 0x0008203C
		private void SIZE(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid file name.");
					}
					FTP_e_GetFileSize ftp_e_GetFileSize = new FTP_e_GetFileSize(argsText);
					this.OnGetFileSize(ftp_e_GetFileSize);
					bool flag3 = ftp_e_GetFileSize.Error != null;
					if (flag3)
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_GetFileSize.Error)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
					else
					{
						this.WriteLine("213 " + ftp_e_GetFileSize.FileSize);
					}
				}
			}
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x00083110 File Offset: 0x00082110
		private void RNFR(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid path value.");
					}
					this.m_RenameFrom = argsText;
					this.WriteLine("350 OK, waiting for RNTO command.");
				}
			}
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x00083180 File Offset: 0x00082180
		private void RNTO(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid path value.");
					}
					bool flag3 = this.m_RenameFrom.Length == 0;
					if (flag3)
					{
						this.WriteLine("503 Bad sequence of commands.");
					}
					else
					{
						FTP_e_Rnto ftp_e_Rnto = new FTP_e_Rnto(this.m_RenameFrom, argsText);
						this.OnRnto(ftp_e_Rnto);
						bool flag4 = ftp_e_Rnto.Response == null;
						if (flag4)
						{
							this.WriteLine("500 Internal server error: FTP server didn't provide response for RNTO command.");
						}
						else
						{
							foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Rnto.Response)
							{
								this.WriteLine(ftp_t_ReplyLine.ToString());
							}
						}
					}
				}
			}
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x00083270 File Offset: 0x00082270
		private void RMD(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid directory name.");
					}
					FTP_e_Rmd ftp_e_Rmd = new FTP_e_Rmd(argsText);
					this.OnRmd(ftp_e_Rmd);
					bool flag3 = ftp_e_Rmd.Response == null;
					if (flag3)
					{
						this.WriteLine("500 Internal server error: FTP server didn't provide response for RMD command.");
					}
					else
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Rmd.Response)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
				}
			}
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00083334 File Offset: 0x00082334
		private void MKD(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = string.IsNullOrEmpty(argsText);
					if (flag2)
					{
						this.WriteLine("501 Invalid directory name.");
					}
					FTP_e_Mkd ftp_e_Mkd = new FTP_e_Mkd(argsText);
					this.OnMkd(ftp_e_Mkd);
					bool flag3 = ftp_e_Mkd.Response == null;
					if (flag3)
					{
						this.WriteLine("500 Internal server error: FTP server didn't provide response for MKD command.");
					}
					else
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_Mkd.Response)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
				}
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x000833F8 File Offset: 0x000823F8
		private void LIST(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					FTP_e_GetDirListing ftp_e_GetDirListing = new FTP_e_GetDirListing(argsText);
					this.OnGetDirListing(ftp_e_GetDirListing);
					bool flag2 = ftp_e_GetDirListing.Error != null;
					if (flag2)
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_GetDirListing.Error)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
					else
					{
						MemoryStreamEx memoryStreamEx = new MemoryStreamEx(8000);
						foreach (FTP_ListItem ftp_ListItem in ftp_e_GetDirListing.Items)
						{
							bool isDir = ftp_ListItem.IsDir;
							if (isDir)
							{
								byte[] bytes = Encoding.UTF8.GetBytes(ftp_ListItem.Modified.ToString("MM-dd-yy HH:mm") + " <DIR> " + ftp_ListItem.Name + "\r\n");
								memoryStreamEx.Write(bytes, 0, bytes.Length);
							}
							else
							{
								byte[] bytes2 = Encoding.UTF8.GetBytes(string.Concat(new string[]
								{
									ftp_ListItem.Modified.ToString("MM-dd-yy HH:mm"),
									" ",
									ftp_ListItem.Size.ToString(),
									" ",
									ftp_ListItem.Name,
									"\r\n"
								}));
								memoryStreamEx.Write(bytes2, 0, bytes2.Length);
							}
						}
						memoryStreamEx.Position = 0L;
						this.m_pDataConnection = new FTP_Session.DataConnection(this, memoryStreamEx, false);
						this.m_pDataConnection.Start();
					}
				}
			}
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x000835E4 File Offset: 0x000825E4
		private void NLST(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					FTP_e_GetDirListing ftp_e_GetDirListing = new FTP_e_GetDirListing(argsText);
					this.OnGetDirListing(ftp_e_GetDirListing);
					bool flag2 = ftp_e_GetDirListing.Error != null;
					if (flag2)
					{
						foreach (FTP_t_ReplyLine ftp_t_ReplyLine in ftp_e_GetDirListing.Error)
						{
							this.WriteLine(ftp_t_ReplyLine.ToString());
						}
					}
					else
					{
						MemoryStreamEx memoryStreamEx = new MemoryStreamEx(8000);
						foreach (FTP_ListItem ftp_ListItem in ftp_e_GetDirListing.Items)
						{
							byte[] bytes = Encoding.UTF8.GetBytes(ftp_ListItem.Name + "\r\n");
							memoryStreamEx.Write(bytes, 0, bytes.Length);
						}
						memoryStreamEx.Position = 0L;
						this.m_pDataConnection = new FTP_Session.DataConnection(this, memoryStreamEx, false);
						this.m_pDataConnection.Start();
					}
				}
			}
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x00083728 File Offset: 0x00082728
		private void TYPE(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					bool flag2 = argsText.Trim().ToUpper() == "A" || argsText.Trim().ToUpper() == "I";
					if (flag2)
					{
						this.WriteLine("200 Type is set to " + argsText + ".");
					}
					else
					{
						this.WriteLine("500 Invalid type " + argsText + ".");
					}
				}
			}
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x000837D8 File Offset: 0x000827D8
		private void PORT(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					string[] array = argsText.Split(new char[]
					{
						','
					});
					bool flag2 = array.Length != 6;
					if (flag2)
					{
						this.WriteLine("550 Invalid arguments.");
					}
					else
					{
						string ipString = string.Concat(new string[]
						{
							array[0],
							".",
							array[1],
							".",
							array[2],
							".",
							array[3]
						});
						int port = Convert.ToInt32(array[4]) << 8 | Convert.ToInt32(array[5]);
						this.m_pDataConEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);
						this.WriteLine("200 PORT Command successful.");
					}
				}
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x000838C4 File Offset: 0x000828C4
		private void PASV(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					int passiveStartPort = this.Server.PassiveStartPort;
					bool flag2 = this.m_pPassiveSocket != null;
					if (!flag2)
					{
						this.m_pPassiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						for (int i = passiveStartPort; i < 65535; i++)
						{
							try
							{
								this.m_pPassiveSocket.Bind(new IPEndPoint(IPAddress.Any, passiveStartPort));
								break;
							}
							catch
							{
							}
						}
						this.m_pPassiveSocket.Listen(1);
					}
					bool flag3 = this.Server.PassivePublicIP != null;
					if (flag3)
					{
						this.WriteLine(string.Concat(new object[]
						{
							"227 Entering Passive Mode (",
							this.Server.PassivePublicIP.ToString().Replace(".", ","),
							",",
							passiveStartPort >> 8,
							",",
							passiveStartPort & 255,
							")."
						}));
					}
					else
					{
						this.WriteLine(string.Concat(new object[]
						{
							"227 Entering Passive Mode (",
							this.LocalEndPoint.Address.ToString().Replace(".", ","),
							",",
							passiveStartPort >> 8,
							",",
							passiveStartPort & 255,
							")."
						}));
					}
					this.m_PassiveMode = true;
				}
			}
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x00083A90 File Offset: 0x00082A90
		private void SYST(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = !base.IsAuthenticated;
				if (flag)
				{
					this.WriteLine("530 Please authenticate firtst !");
				}
				else
				{
					this.WriteLine("215 Windows_NT");
				}
			}
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00083AE0 File Offset: 0x00082AE0
		private void NOOP(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				this.WriteLine("200 OK");
			}
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x00083B14 File Offset: 0x00082B14
		private void QUIT(string argsText)
		{
			try
			{
				this.WriteLine("221 FTP server signing off");
			}
			catch
			{
			}
			this.Disconnect();
			this.Dispose();
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00083B58 File Offset: 0x00082B58
		private void FEAT(string argsText)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("211-Extensions supported:\r\n");
			bool flag = !this.IsSecureConnection && base.Certificate != null;
			if (flag)
			{
				stringBuilder.Append(" TLS\r\n");
			}
			stringBuilder.Append(" SIZE\r\n");
			stringBuilder.Append("211 End of extentions.\r\n");
			this.WriteLine(stringBuilder.ToString());
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x00083BC4 File Offset: 0x00082BC4
		private void OPTS(string argsText)
		{
			bool sessionRejected = this.m_SessionRejected;
			if (sessionRejected)
			{
				this.WriteLine("500 Bad sequence of commands: Session rejected.");
			}
			else
			{
				bool flag = string.Equals(argsText, "UTF8 ON", StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					this.WriteLine("200 Ok.");
				}
				else
				{
					this.WriteLine("501 OPTS parameter not supported.");
				}
			}
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00083C1C File Offset: 0x00082C1C
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
				this.Server.Logger.AddWrite(this.ID, this.AuthenticatedUserIdentity, (long)num, line.TrimEnd(new char[0]), this.LocalEndPoint, this.RemoteEndPoint);
			}
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00083C98 File Offset: 0x00082C98
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

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06001506 RID: 5382 RVA: 0x00083CE8 File Offset: 0x00082CE8
		public new FTP_Server Server
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return (FTP_Server)base.Server;
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x00083D24 File Offset: 0x00082D24
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

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06001508 RID: 5384 RVA: 0x00083D58 File Offset: 0x00082D58
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

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x00083D8C File Offset: 0x00082D8C
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

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x0600150A RID: 5386 RVA: 0x00083DC0 File Offset: 0x00082DC0
		// (set) Token: 0x0600150B RID: 5387 RVA: 0x00083DF4 File Offset: 0x00082DF4
		public string CurrentDir
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CurrentDir;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_CurrentDir = value;
			}
		}

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x0600150C RID: 5388 RVA: 0x00083E28 File Offset: 0x00082E28
		public bool PassiveMode
		{
			get
			{
				return this.m_PassiveMode;
			}
		}

		// Token: 0x1400008B RID: 139
		// (add) Token: 0x0600150D RID: 5389 RVA: 0x00083E40 File Offset: 0x00082E40
		// (remove) Token: 0x0600150E RID: 5390 RVA: 0x00083E78 File Offset: 0x00082E78
		
		public event EventHandler<FTP_e_Started> Started = null;

		// Token: 0x0600150F RID: 5391 RVA: 0x00083EB0 File Offset: 0x00082EB0
		private FTP_e_Started OnStarted(string reply)
		{
			FTP_e_Started ftp_e_Started = new FTP_e_Started(reply);
			bool flag = this.Started != null;
			if (flag)
			{
				this.Started(this, ftp_e_Started);
			}
			return ftp_e_Started;
		}

		// Token: 0x1400008C RID: 140
		// (add) Token: 0x06001510 RID: 5392 RVA: 0x00083EE8 File Offset: 0x00082EE8
		// (remove) Token: 0x06001511 RID: 5393 RVA: 0x00083F20 File Offset: 0x00082F20
		
		public event EventHandler<FTP_e_Authenticate> Authenticate = null;

		// Token: 0x06001512 RID: 5394 RVA: 0x00083F58 File Offset: 0x00082F58
		private FTP_e_Authenticate OnAuthenticate(string user, string password)
		{
			FTP_e_Authenticate ftp_e_Authenticate = new FTP_e_Authenticate(user, password);
			bool flag = this.Authenticate != null;
			if (flag)
			{
				this.Authenticate(this, ftp_e_Authenticate);
			}
			return ftp_e_Authenticate;
		}

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x06001513 RID: 5395 RVA: 0x00083F90 File Offset: 0x00082F90
		// (remove) Token: 0x06001514 RID: 5396 RVA: 0x00083FC8 File Offset: 0x00082FC8
		
		public event EventHandler<FTP_e_GetFile> GetFile = null;

		// Token: 0x06001515 RID: 5397 RVA: 0x00084000 File Offset: 0x00083000
		private void OnGetFile(FTP_e_GetFile e)
		{
			bool flag = this.GetFile != null;
			if (flag)
			{
				this.GetFile(this, e);
			}
		}

		// Token: 0x1400008E RID: 142
		// (add) Token: 0x06001516 RID: 5398 RVA: 0x0008402C File Offset: 0x0008302C
		// (remove) Token: 0x06001517 RID: 5399 RVA: 0x00084064 File Offset: 0x00083064
		
		public event EventHandler<FTP_e_Stor> Stor = null;

		// Token: 0x06001518 RID: 5400 RVA: 0x0008409C File Offset: 0x0008309C
		private void OnStor(FTP_e_Stor e)
		{
			bool flag = this.Stor != null;
			if (flag)
			{
				this.Stor(this, e);
			}
		}

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06001519 RID: 5401 RVA: 0x000840C8 File Offset: 0x000830C8
		// (remove) Token: 0x0600151A RID: 5402 RVA: 0x00084100 File Offset: 0x00083100
		
		public event EventHandler<FTP_e_GetFileSize> GetFileSize = null;

		// Token: 0x0600151B RID: 5403 RVA: 0x00084138 File Offset: 0x00083138
		private void OnGetFileSize(FTP_e_GetFileSize e)
		{
			bool flag = this.GetFileSize != null;
			if (flag)
			{
				this.GetFileSize(this, e);
			}
		}

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x0600151C RID: 5404 RVA: 0x00084164 File Offset: 0x00083164
		// (remove) Token: 0x0600151D RID: 5405 RVA: 0x0008419C File Offset: 0x0008319C
		
		public event EventHandler<FTP_e_Dele> Dele = null;

		// Token: 0x0600151E RID: 5406 RVA: 0x000841D4 File Offset: 0x000831D4
		private void OnDele(FTP_e_Dele e)
		{
			bool flag = this.Dele != null;
			if (flag)
			{
				this.Dele(this, e);
			}
		}

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x0600151F RID: 5407 RVA: 0x00084200 File Offset: 0x00083200
		// (remove) Token: 0x06001520 RID: 5408 RVA: 0x00084238 File Offset: 0x00083238
		
		public event EventHandler<FTP_e_Appe> Appe = null;

		// Token: 0x06001521 RID: 5409 RVA: 0x00084270 File Offset: 0x00083270
		private void OnAppe(FTP_e_Appe e)
		{
			bool flag = this.Appe != null;
			if (flag)
			{
				this.Appe(this, e);
			}
		}

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06001522 RID: 5410 RVA: 0x0008429C File Offset: 0x0008329C
		// (remove) Token: 0x06001523 RID: 5411 RVA: 0x000842D4 File Offset: 0x000832D4
		
		public event EventHandler<FTP_e_Cwd> Cwd = null;

		// Token: 0x06001524 RID: 5412 RVA: 0x0008430C File Offset: 0x0008330C
		private void OnCwd(FTP_e_Cwd e)
		{
			bool flag = this.Cwd != null;
			if (flag)
			{
				this.Cwd(this, e);
			}
		}

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x06001525 RID: 5413 RVA: 0x00084338 File Offset: 0x00083338
		// (remove) Token: 0x06001526 RID: 5414 RVA: 0x00084370 File Offset: 0x00083370
		
		public event EventHandler<FTP_e_Cdup> Cdup = null;

		// Token: 0x06001527 RID: 5415 RVA: 0x000843A8 File Offset: 0x000833A8
		private void OnCdup(FTP_e_Cdup e)
		{
			bool flag = this.Cdup != null;
			if (flag)
			{
				this.Cdup(this, e);
			}
		}

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x06001528 RID: 5416 RVA: 0x000843D4 File Offset: 0x000833D4
		// (remove) Token: 0x06001529 RID: 5417 RVA: 0x0008440C File Offset: 0x0008340C
		
		public event EventHandler<FTP_e_Rmd> Rmd = null;

		// Token: 0x0600152A RID: 5418 RVA: 0x00084444 File Offset: 0x00083444
		private void OnRmd(FTP_e_Rmd e)
		{
			bool flag = this.Rmd != null;
			if (flag)
			{
				this.Rmd(this, e);
			}
		}

		// Token: 0x14000095 RID: 149
		// (add) Token: 0x0600152B RID: 5419 RVA: 0x00084470 File Offset: 0x00083470
		// (remove) Token: 0x0600152C RID: 5420 RVA: 0x000844A8 File Offset: 0x000834A8
		
		public event EventHandler<FTP_e_Mkd> Mkd = null;

		// Token: 0x0600152D RID: 5421 RVA: 0x000844E0 File Offset: 0x000834E0
		private void OnMkd(FTP_e_Mkd e)
		{
			bool flag = this.Mkd != null;
			if (flag)
			{
				this.Mkd(this, e);
			}
		}

		// Token: 0x14000096 RID: 150
		// (add) Token: 0x0600152E RID: 5422 RVA: 0x0008450C File Offset: 0x0008350C
		// (remove) Token: 0x0600152F RID: 5423 RVA: 0x00084544 File Offset: 0x00083544
		
		public event EventHandler<FTP_e_GetDirListing> GetDirListing = null;

		// Token: 0x06001530 RID: 5424 RVA: 0x0008457C File Offset: 0x0008357C
		private void OnGetDirListing(FTP_e_GetDirListing e)
		{
			bool flag = this.GetDirListing != null;
			if (flag)
			{
				this.GetDirListing(this, e);
			}
		}

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x06001531 RID: 5425 RVA: 0x000845A8 File Offset: 0x000835A8
		// (remove) Token: 0x06001532 RID: 5426 RVA: 0x000845E0 File Offset: 0x000835E0
		
		public event EventHandler<FTP_e_Rnto> Rnto = null;

		// Token: 0x06001533 RID: 5427 RVA: 0x00084618 File Offset: 0x00083618
		private void OnRnto(FTP_e_Rnto e)
		{
			bool flag = this.Rnto != null;
			if (flag)
			{
				this.Rnto(this, e);
			}
		}

		// Token: 0x04000829 RID: 2089
		private Dictionary<string, AUTH_SASL_ServerMechanism> m_pAuthentications = null;

		// Token: 0x0400082A RID: 2090
		private bool m_SessionRejected = false;

		// Token: 0x0400082B RID: 2091
		private int m_BadCommands = 0;

		// Token: 0x0400082C RID: 2092
		private string m_UserName = null;

		// Token: 0x0400082D RID: 2093
		private GenericIdentity m_pUser = null;

		// Token: 0x0400082E RID: 2094
		private string m_CurrentDir = "/";

		// Token: 0x0400082F RID: 2095
		private string m_RenameFrom = "";

		// Token: 0x04000830 RID: 2096
		private FTP_Session.DataConnection m_pDataConnection = null;

		// Token: 0x04000831 RID: 2097
		private bool m_PassiveMode = false;

		// Token: 0x04000832 RID: 2098
		private Socket m_pPassiveSocket = null;

		// Token: 0x04000833 RID: 2099
		private IPEndPoint m_pDataConEndPoint = null;

		// Token: 0x0200037E RID: 894
		private class DataConnection
		{
			// Token: 0x06001B98 RID: 7064 RVA: 0x000AA844 File Offset: 0x000A9844
			public DataConnection(FTP_Session session, Stream stream, bool read_write)
			{
				bool flag = session == null;
				if (flag)
				{
					throw new ArgumentNullException("session");
				}
				bool flag2 = stream == null;
				if (flag2)
				{
					throw new ArgumentNullException("stream");
				}
				this.m_pSession = session;
				this.m_pStream = stream;
				this.m_Read_Write = read_write;
			}

			// Token: 0x06001B99 RID: 7065 RVA: 0x000AA8BC File Offset: 0x000A98BC
			public void Dispose()
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					bool flag = this.m_pSession.m_pPassiveSocket != null;
					if (flag)
					{
						this.m_pSession.m_pPassiveSocket.Close();
						this.m_pSession.m_pPassiveSocket = null;
					}
					this.m_pSession.m_PassiveMode = false;
					this.m_pSession = null;
					bool flag2 = this.m_pStream != null;
					if (flag2)
					{
						this.m_pStream.Dispose();
						this.m_pStream = null;
					}
					bool flag3 = this.m_pSocket != null;
					if (flag3)
					{
						this.m_pSocket.Close();
						this.m_pSocket = null;
					}
				}
			}

			// Token: 0x06001B9A RID: 7066 RVA: 0x000AA96C File Offset: 0x000A996C
			public void Start()
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool passiveMode = this.m_pSession.PassiveMode;
				if (passiveMode)
				{
					this.WriteLine("150 Waiting data connection on port '" + ((IPEndPoint)this.m_pSession.m_pPassiveSocket.LocalEndPoint).Port + "'.");
					TimerEx timer = new TimerEx(10000.0, false);
					timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
					{
						this.WriteLine("550 Data connection wait timeout.");
						this.Dispose();
					};
					timer.Enabled = true;
					this.m_pSession.m_pPassiveSocket.BeginAccept(delegate(IAsyncResult ar)
					{
						try
						{
							timer.Dispose();
							this.m_pSocket = this.m_pSession.m_pPassiveSocket.EndAccept(ar);
							this.m_pSession.LogAddText("Data connection opened.");
							this.StartDataTransfer();
						}
						catch
						{
							this.WriteLine("425 Opening data connection failed.");
							this.Dispose();
						}
					}, null);
				}
				else
				{
					this.WriteLine("150 Opening data connection to '" + this.m_pSession.m_pDataConEndPoint.ToString() + "'.");
					this.m_pSocket = new Socket(this.m_pSession.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					this.m_pSocket.BeginConnect(this.m_pSession.m_pDataConEndPoint, delegate(IAsyncResult ar)
					{
						try
						{
							this.m_pSocket.EndConnect(ar);
							this.m_pSession.LogAddText("Data connection opened.");
							this.StartDataTransfer();
						}
						catch
						{
							this.WriteLine("425 Opening data connection to '" + this.m_pSession.m_pDataConEndPoint.ToString() + "' failed.");
							this.Dispose();
						}
					}, null);
				}
			}

			// Token: 0x06001B9B RID: 7067 RVA: 0x000AAAB0 File Offset: 0x000A9AB0
			public void Abort()
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.WriteLine("426 Data connection closed; transfer aborted.");
				this.Dispose();
			}

			// Token: 0x06001B9C RID: 7068 RVA: 0x000AAAF0 File Offset: 0x000A9AF0
			private void WriteLine(string line)
			{
				bool flag = line == null;
				if (flag)
				{
					throw new ArgumentNullException("line");
				}
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_pSession.WriteLine(line);
				}
			}

			// Token: 0x06001B9D RID: 7069 RVA: 0x000AAB2C File Offset: 0x000A9B2C
			private void StartDataTransfer()
			{
				try
				{
					bool read_Write = this.m_Read_Write;
					if (read_Write)
					{
						Net_Utils.StreamCopy(new NetworkStream(this.m_pSocket, false), this.m_pStream, 64000);
					}
					else
					{
						Net_Utils.StreamCopy(this.m_pStream, new NetworkStream(this.m_pSocket, false), 64000);
					}
					this.m_pSocket.Shutdown(SocketShutdown.Both);
					this.m_pSession.WriteLine("226 Transfer Complete.");
				}
				catch
				{
				}
				this.Dispose();
			}

			// Token: 0x04000CD1 RID: 3281
			private bool m_IsDisposed = false;

			// Token: 0x04000CD2 RID: 3282
			private FTP_Session m_pSession = null;

			// Token: 0x04000CD3 RID: 3283
			private Stream m_pStream = null;

			// Token: 0x04000CD4 RID: 3284
			private bool m_Read_Write = false;

			// Token: 0x04000CD5 RID: 3285
			private Socket m_pSocket = null;
		}
	}
}
