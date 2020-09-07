using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SharpCifs.Netbios;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000B1 RID: 177
	public sealed class SmbSession
	{
		// Token: 0x0600059C RID: 1436 RVA: 0x0001D5F4 File Offset: 0x0001B7F4
		private static NtlmChallenge Interrogate(NbtAddress addr)
		{
			UniAddress uniAddress = new UniAddress(addr);
			SmbTransport smbTransport = SmbTransport.GetSmbTransport(uniAddress, 0);
			bool flag = SmbSession.Username == null;
			if (flag)
			{
				smbTransport.Connect();
				bool flag2 = SmbTransport.LogStatic.Level >= 3;
				if (flag2)
				{
					SmbTransport.LogStatic.WriteLine("Default credentials (jcifs.smb.client.username/password) not specified. SMB signing may not work propertly.  Skipping DC interrogation.");
				}
			}
			else
			{
				SmbSession smbSession = smbTransport.GetSmbSession(NtlmPasswordAuthentication.Default);
				smbSession.GetSmbTree(SmbSession.LogonShare, null).TreeConnect(null, null);
			}
			return new NtlmChallenge(smbTransport.Server.EncryptionKey, uniAddress);
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0001D68C File Offset: 0x0001B88C
		public static NtlmChallenge GetChallengeForDomain()
		{
			bool flag = SmbSession.Domain == null;
			if (flag)
			{
				throw new SmbException("A domain was not specified");
			}
			string domain = SmbSession.Domain;
			lock (domain)
			{
				long num = Runtime.CurrentTimeMillis();
				int num2 = 1;
				do
				{
					bool flag3 = SmbSession.DcListExpiration < num;
					if (flag3)
					{
						NbtAddress[] allByName = NbtAddress.GetAllByName(SmbSession.Domain, 28, null, null);
						SmbSession.DcListExpiration = num + (long)SmbSession.CachePolicy * 1000L;
						bool flag4 = allByName != null && allByName.Length != 0;
						if (flag4)
						{
							SmbSession.DcList = allByName;
						}
						else
						{
							SmbSession.DcListExpiration = num + 900000L;
							bool flag5 = SmbTransport.LogStatic.Level >= 2;
							if (flag5)
							{
								SmbTransport.LogStatic.WriteLine("Failed to retrieve DC list from WINS");
							}
						}
					}
					int num3 = Math.Min(SmbSession.DcList.Length, SmbSession.LookupRespLimit);
					for (int i = 0; i < num3; i++)
					{
						int num4 = SmbSession.DcListCounter++ % num3;
						bool flag6 = SmbSession.DcList[num4] != null;
						if (flag6)
						{
							try
							{
								return SmbSession.Interrogate(SmbSession.DcList[num4]);
							}
							catch (SmbException ex)
							{
								bool flag7 = SmbTransport.LogStatic.Level >= 2;
								if (flag7)
								{
									SmbTransport.LogStatic.WriteLine("Failed validate DC: " + SmbSession.DcList[num4]);
									bool flag8 = SmbTransport.LogStatic.Level > 2;
									if (flag8)
									{
										Runtime.PrintStackTrace(ex, SmbTransport.LogStatic);
									}
								}
							}
							SmbSession.DcList[num4] = null;
						}
					}
					SmbSession.DcListExpiration = 0L;
				}
				while (num2-- > 0);
				SmbSession.DcListExpiration = num + 900000L;
			}
			throw new UnknownHostException("Failed to negotiate with a suitable domain controller for " + SmbSession.Domain);
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0001D8AC File Offset: 0x0001BAAC
		public static byte[] GetChallenge(UniAddress dc)
		{
			return SmbSession.GetChallenge(dc, 0);
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0001D8C8 File Offset: 0x0001BAC8
		public static byte[] GetChallenge(UniAddress dc, int port)
		{
			SmbTransport smbTransport = SmbTransport.GetSmbTransport(dc, port);
			smbTransport.Connect();
			return smbTransport.Server.EncryptionKey;
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0001D8F4 File Offset: 0x0001BAF4
		public static void Logon(UniAddress dc, NtlmPasswordAuthentication auth)
		{
			SmbSession.Logon(dc, -1, auth);
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001D900 File Offset: 0x0001BB00
		public static void Logon(UniAddress dc, int port, NtlmPasswordAuthentication auth)
		{
			SmbTransport smbTransport = SmbTransport.GetSmbTransport(dc, port);
			SmbSession smbSession = smbTransport.GetSmbSession(auth);
			SmbTree smbTree = smbSession.GetSmbTree(SmbSession.LogonShare, null);
			bool flag = SmbSession.LogonShare == null;
			if (flag)
			{
				smbTree.TreeConnect(null, null);
			}
			else
			{
				Trans2FindFirst2 request = new Trans2FindFirst2("\\", "*", 16);
				Trans2FindFirst2Response response = new Trans2FindFirst2Response();
				smbTree.Send(request, response);
			}
			smbTree.TreeDisconnect(false);
			smbSession.Logoff(false);
			smbTransport.Disconnect(false);
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0001D984 File Offset: 0x0001BB84
		internal SmbSession(UniAddress address, int port, IPAddress localAddr, int localPort, NtlmPasswordAuthentication auth)
		{
			this._address = address;
			this._port = port;
			this._localAddr = localAddr;
			this._localPort = localPort;
			this.Auth = auth;
			this.Trees = new List<object>();
			this.ConnectionState = 0;
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0001D9D0 File Offset: 0x0001BBD0
		internal SmbTree GetSmbTree(string share, string service)
		{
			SmbTree result;
			lock (this)
			{
				bool flag2 = share == null;
				if (flag2)
				{
					share = "IPC$";
				}
				SmbTree smbTree;
				foreach (object obj in this.Trees)
				{
					smbTree = (SmbTree)obj;
					bool flag3 = smbTree.Matches(share, service);
					if (flag3)
					{
						return smbTree;
					}
				}
				smbTree = new SmbTree(this, share, service);
				this.Trees.Add(smbTree);
				result = smbTree;
			}
			return result;
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0001DA94 File Offset: 0x0001BC94
		internal bool Matches(NtlmPasswordAuthentication auth)
		{
			return this.Auth == auth || this.Auth.Equals(auth);
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0001DAC0 File Offset: 0x0001BCC0
		internal SmbTransport Transport()
		{
			SmbTransport result;
			lock (this)
			{
				bool flag2 = this.transport == null;
				if (flag2)
				{
					this.transport = SmbTransport.GetSmbTransport(this._address, this._port, this._localAddr, this._localPort, null);
				}
				result = this.transport;
			}
			return result;
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0001DB34 File Offset: 0x0001BD34
		internal void Send(ServerMessageBlock request, ServerMessageBlock response)
		{
			SmbTransport obj = this.Transport();
			lock (obj)
			{
				bool flag2 = response != null;
				if (flag2)
				{
					response.Received = false;
				}
				this.Expiration = Runtime.CurrentTimeMillis() + (long)SmbConstants.SoTimeout;
				this.SessionSetup(request, response);
				bool flag3 = response != null && response.Received;
				if (!flag3)
				{
					bool flag4 = request is SmbComTreeConnectAndX;
					if (flag4)
					{
						SmbComTreeConnectAndX smbComTreeConnectAndX = (SmbComTreeConnectAndX)request;
						bool flag5 = this.NetbiosName != null && smbComTreeConnectAndX.path.EndsWith("\\IPC$");
						if (flag5)
						{
							smbComTreeConnectAndX.path = "\\\\" + this.NetbiosName + "\\IPC$";
						}
					}
					request.Uid = this.Uid;
					request.Auth = this.Auth;
					try
					{
						this.transport.Send(request, response);
					}
					catch (SmbException ex)
					{
						bool flag6 = request is SmbComTreeConnectAndX;
						if (flag6)
						{
							this.Logoff(true);
						}
						request.Digest = null;
						throw;
					}
				}
			}
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0001DC6C File Offset: 0x0001BE6C
		internal void SessionSetup(ServerMessageBlock andx, ServerMessageBlock andxResponse)
		{
			SmbTransport obj = this.Transport();
			lock (obj)
			{
				NtlmContext ntlmContext = null;
				SmbException ex = null;
				byte[] array = new byte[0];
				int num = 10;
				while (this.ConnectionState != 0)
				{
					bool flag2 = this.ConnectionState == 2 || this.ConnectionState == 3;
					if (flag2)
					{
						return;
					}
					try
					{
						Runtime.Wait(this.transport);
					}
					catch (Exception ex2)
					{
						throw new SmbException(ex2.Message, ex2);
					}
				}
				this.ConnectionState = 1;
				try
				{
					this.transport.Connect();
					bool flag3 = this.transport.Log.Level >= 4;
					if (flag3)
					{
						this.transport.Log.WriteLine("sessionSetup: accountName=" + this.Auth.Username + ",primaryDomain=" + this.Auth.Domain);
					}
					this.Uid = 0;
					for (;;)
					{
						int num2 = num;
						if (num2 != 10)
						{
							if (num2 != 20)
							{
								break;
							}
							bool flag4 = ntlmContext == null;
							if (flag4)
							{
								bool doSigning = (this.transport.Flags2 & SmbConstants.Flags2SecuritySignatures) != 0;
								ntlmContext = new NtlmContext(this.Auth, doSigning);
							}
							bool flag5 = SmbTransport.LogStatic.Level >= 4;
							if (flag5)
							{
								SmbTransport.LogStatic.WriteLine(ntlmContext);
							}
							bool flag6 = ntlmContext.IsEstablished();
							if (flag6)
							{
								this.NetbiosName = ntlmContext.GetNetbiosName();
								this.ConnectionState = 2;
								num = 0;
							}
							else
							{
								try
								{
									array = ntlmContext.InitSecContext(array, 0, array.Length);
								}
								catch (SmbException ex3)
								{
									try
									{
										this.transport.Disconnect(true);
									}
									catch (IOException)
									{
									}
									this.Uid = 0;
									throw;
								}
								bool flag7 = array != null;
								if (flag7)
								{
									SmbComSessionSetupAndX smbComSessionSetupAndX = new SmbComSessionSetupAndX(this, null, array);
									SmbComSessionSetupAndXResponse smbComSessionSetupAndXResponse = new SmbComSessionSetupAndXResponse(null);
									bool flag8 = this.transport.IsSignatureSetupRequired(this.Auth);
									if (flag8)
									{
										byte[] signingKey = ntlmContext.GetSigningKey();
										bool flag9 = signingKey != null;
										if (flag9)
										{
											smbComSessionSetupAndX.Digest = new SigningDigest(signingKey, true);
										}
									}
									smbComSessionSetupAndX.Uid = this.Uid;
									this.Uid = 0;
									try
									{
										this.transport.Send(smbComSessionSetupAndX, smbComSessionSetupAndXResponse);
									}
									catch (SmbAuthException ex4)
									{
										throw;
									}
									catch (SmbException ex5)
									{
										ex = ex5;
										try
										{
											this.transport.Disconnect(true);
										}
										catch (Exception)
										{
										}
									}
									bool flag10 = smbComSessionSetupAndXResponse.IsLoggedInAsGuest && !Runtime.EqualsIgnoreCase("GUEST", this.Auth.Username);
									if (flag10)
									{
										goto Block_36;
									}
									bool flag11 = ex != null;
									if (flag11)
									{
										goto Block_37;
									}
									this.Uid = smbComSessionSetupAndXResponse.Uid;
									bool flag12 = smbComSessionSetupAndX.Digest != null;
									if (flag12)
									{
										this.transport.Digest = smbComSessionSetupAndX.Digest;
									}
									array = smbComSessionSetupAndXResponse.Blob;
								}
							}
						}
						else
						{
							bool flag13 = this.Auth != NtlmPasswordAuthentication.Anonymous && this.transport.HasCapability(SmbConstants.CapExtendedSecurity);
							if (flag13)
							{
								num = 20;
							}
							else
							{
								SmbComSessionSetupAndX smbComSessionSetupAndX = new SmbComSessionSetupAndX(this, andx, this.Auth);
								SmbComSessionSetupAndXResponse smbComSessionSetupAndXResponse = new SmbComSessionSetupAndXResponse(andxResponse);
								bool flag14 = this.transport.IsSignatureSetupRequired(this.Auth);
								if (flag14)
								{
									bool flag15 = this.Auth.HashesExternal && NtlmPasswordAuthentication.DefaultPassword != NtlmPasswordAuthentication.Blank;
									if (flag15)
									{
										this.transport.GetSmbSession(NtlmPasswordAuthentication.Default).GetSmbTree(SmbSession.LogonShare, null).TreeConnect(null, null);
									}
									else
									{
										byte[] signingKey2 = this.Auth.GetSigningKey(this.transport.Server.EncryptionKey);
										smbComSessionSetupAndX.Digest = new SigningDigest(signingKey2, false);
									}
								}
								smbComSessionSetupAndX.Auth = this.Auth;
								try
								{
									this.transport.Send(smbComSessionSetupAndX, smbComSessionSetupAndXResponse);
								}
								catch (SmbAuthException ex6)
								{
									throw;
								}
								catch (SmbException ex7)
								{
									ex = ex7;
								}
								bool flag16 = smbComSessionSetupAndXResponse.IsLoggedInAsGuest && !Runtime.EqualsIgnoreCase("GUEST", this.Auth.Username) && this.transport.Server.Security != SmbConstants.SecurityShare && this.Auth != NtlmPasswordAuthentication.Anonymous;
								if (flag16)
								{
									goto Block_24;
								}
								bool flag17 = ex != null;
								if (flag17)
								{
									goto Block_25;
								}
								this.Uid = smbComSessionSetupAndXResponse.Uid;
								bool flag18 = smbComSessionSetupAndX.Digest != null;
								if (flag18)
								{
									this.transport.Digest = smbComSessionSetupAndX.Digest;
								}
								this.ConnectionState = 2;
								num = 0;
							}
						}
						if (num == 0)
						{
							goto Block_39;
						}
					}
					throw new SmbException("Unexpected session setup state: " + num);
					Block_24:
					throw new SmbAuthException(-1073741715);
					Block_25:
					throw ex;
					Block_36:
					throw new SmbAuthException(-1073741715);
					Block_37:
					throw ex;
					Block_39:;
				}
				catch (SmbException ex8)
				{
					this.Logoff(true);
					this.ConnectionState = 0;
					throw;
				}
				finally
				{
					Runtime.NotifyAll(this.transport);
				}
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001E278 File Offset: 0x0001C478
		internal void Logoff(bool inError)
		{
			SmbTransport obj = this.Transport();
			lock (obj)
			{
				bool flag2 = this.ConnectionState != 2;
				if (!flag2)
				{
					this.ConnectionState = 3;
					this.NetbiosName = null;
					foreach (object obj2 in this.Trees)
					{
						SmbTree smbTree = (SmbTree)obj2;
						smbTree.TreeDisconnect(inError);
					}
					bool flag3 = !inError && this.transport.Server.Security != SmbConstants.SecurityShare;
					if (flag3)
					{
						SmbComLogoffAndX smbComLogoffAndX = new SmbComLogoffAndX(null);
						smbComLogoffAndX.Uid = this.Uid;
						try
						{
							this.transport.Send(smbComLogoffAndX, null);
						}
						catch (SmbException)
						{
						}
						this.Uid = 0;
					}
					this.ConnectionState = 0;
					Runtime.NotifyAll(this.transport);
				}
			}
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0001E3A4 File Offset: 0x0001C5A4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbSession[accountName=",
				this.Auth.Username,
				",primaryDomain=",
				this.Auth.Domain,
				",uid=",
				this.Uid,
				",connectionState=",
				this.ConnectionState,
				"]"
			});
		}

		// Token: 0x04000347 RID: 839
		private static readonly string LogonShare = Config.GetProperty("jcifs.smb.client.logonShare", null);

		// Token: 0x04000348 RID: 840
		private static readonly int LookupRespLimit = Config.GetInt("jcifs.netbios.lookupRespLimit", 3);

		// Token: 0x04000349 RID: 841
		private static readonly string Domain = Config.GetProperty("jcifs.smb.client.domain", null);

		// Token: 0x0400034A RID: 842
		private static readonly string Username = Config.GetProperty("jcifs.smb.client.username", null);

		// Token: 0x0400034B RID: 843
		private static readonly int CachePolicy = Config.GetInt("jcifs.netbios.cachePolicy", 600) * 60;

		// Token: 0x0400034C RID: 844
		internal static NbtAddress[] DcList;

		// Token: 0x0400034D RID: 845
		internal static long DcListExpiration;

		// Token: 0x0400034E RID: 846
		internal static int DcListCounter;

		// Token: 0x0400034F RID: 847
		internal int ConnectionState;

		// Token: 0x04000350 RID: 848
		internal int Uid;

		// Token: 0x04000351 RID: 849
		internal List<object> Trees;

		// Token: 0x04000352 RID: 850
		private UniAddress _address;

		// Token: 0x04000353 RID: 851
		private int _port;

		// Token: 0x04000354 RID: 852
		private int _localPort;

		// Token: 0x04000355 RID: 853
		private IPAddress _localAddr;

		// Token: 0x04000356 RID: 854
		internal SmbTransport transport;

		// Token: 0x04000357 RID: 855
		internal NtlmPasswordAuthentication Auth;

		// Token: 0x04000358 RID: 856
		internal long Expiration;

		// Token: 0x04000359 RID: 857
		internal string NetbiosName;
	}
}
