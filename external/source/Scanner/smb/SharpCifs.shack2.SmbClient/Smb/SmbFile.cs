using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SharpCifs.Dcerpc;
using SharpCifs.Dcerpc.Msrpc;
using SharpCifs.Netbios;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000A9 RID: 169
	public class SmbFile : UrlConnection
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x00017F44 File Offset: 0x00016144
		public LogStream Log
		{
			get
			{
				return LogStream.GetInstance();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060004E7 RID: 1255 RVA: 0x00017FB8 File Offset: 0x000161B8
		// (set) Token: 0x060004E8 RID: 1256 RVA: 0x00017FD0 File Offset: 0x000161D0
		public bool EnableDfs
		{
			get
			{
				return this._enableDfs;
			}
			set
			{
				this._enableDfs = value;
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00017FDA File Offset: 0x000161DA
		public SmbFile(string url) : this(new Uri(url))
		{
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x00017FEC File Offset: 0x000161EC
		public SmbFile(SmbFile context, string name) : this(context.IsWorkgroup0() ? new Uri("smb://" + name) : new Uri(context.Url.AbsoluteUri + name), context.Auth)
		{
			this._enableDfs = context.EnableDfs;
			bool flag = !context.IsWorkgroup0();
			if (flag)
			{
				this.Addresses = context.Addresses;
				bool flag2 = context._share != null;
				if (flag2)
				{
					this.Tree = context.Tree;
					this._dfsReferral = context._dfsReferral;
				}
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00018085 File Offset: 0x00016285
		public SmbFile(string context, string name) : this(new Uri(context + name))
		{
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001809B File Offset: 0x0001629B
		public SmbFile(string url, NtlmPasswordAuthentication auth) : this(new Uri(url, UriKind.RelativeOrAbsolute), auth)
		{
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x000180B0 File Offset: 0x000162B0
		public SmbFile(string url, NtlmPasswordAuthentication auth, int shareAccess) : this(new Uri(url), auth)
		{
			bool flag = (shareAccess & -8) != 0;
			if (flag)
			{
				throw new RuntimeException("Illegal shareAccess parameter");
			}
			this._shareAccess = shareAccess;
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x000180EA File Offset: 0x000162EA
		public SmbFile(string context, string name, NtlmPasswordAuthentication auth) : this(new Uri(context + name), auth)
		{
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x00018104 File Offset: 0x00016304
		public SmbFile(string context, string name, NtlmPasswordAuthentication auth, int shareAccess) : this(new Uri(context + name), auth)
		{
			bool flag = (shareAccess & -8) != 0;
			if (flag)
			{
				throw new RuntimeException("Illegal shareAccess parameter");
			}
			this._shareAccess = shareAccess;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00018148 File Offset: 0x00016348
		public SmbFile(SmbFile context, string name, int shareAccess) : this(context.IsWorkgroup0() ? new Uri("smb://" + name) : new Uri(context.Url.AbsoluteUri + name), context.Auth)
		{
			bool flag = (shareAccess & -8) != 0;
			if (flag)
			{
				throw new RuntimeException("Illegal shareAccess parameter");
			}
			bool flag2 = !context.IsWorkgroup0();
			if (flag2)
			{
				this.Addresses = context.Addresses;
				bool flag3 = context._share != null || context.Tree != null;
				if (flag3)
				{
					this.Tree = context.Tree;
					this._dfsReferral = context._dfsReferral;
				}
			}
			this._shareAccess = shareAccess;
			this._enableDfs = context.EnableDfs;
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001820A File Offset: 0x0001640A
		protected SmbFile(Uri url) : this(url, new NtlmPasswordAuthentication(url.GetUserInfo()))
		{
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00018220 File Offset: 0x00016420
		public SmbFile(Uri url, NtlmPasswordAuthentication auth)
		{
			this.Auth = (auth ?? new NtlmPasswordAuthentication(url.GetUserInfo()));
			this.Url = url;
			this.GetUncPath0();
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00018274 File Offset: 0x00016474
		internal SmbFile(SmbFile context, string name, int type, int attributes, long createTime, long lastModified, long size) : this(context.IsWorkgroup0() ? new Uri("smb://" + name + "/") : new Uri(context.Url.AbsoluteUri + name + (((attributes & 16) > 0) ? "/" : string.Empty)))
		{
			this.Auth = context.Auth;
			bool flag = context._share != null;
			if (flag)
			{
				this.Tree = context.Tree;
				this._dfsReferral = context._dfsReferral;
			}
			int num = name.Length - 1;
			bool flag2 = name[num] == '/';
			if (flag2)
			{
				name = Runtime.Substring(name, 0, num);
			}
			bool flag3 = context._share == null;
			if (flag3)
			{
				this.Unc = "\\";
			}
			else
			{
				bool flag4 = context.Unc.Equals("\\");
				if (flag4)
				{
					this.Unc = "\\" + name;
				}
				else
				{
					this.Unc = context.Unc + "\\" + name;
				}
			}
			bool flag5 = !context.IsWorkgroup0();
			if (flag5)
			{
				this.Addresses = context.Addresses;
			}
			this._enableDfs = context.EnableDfs;
			this.Type = type;
			this._attributes = attributes;
			this._createTime = createTime;
			this._lastModified = lastModified;
			this._size = size;
			this._isExists = true;
			this._attrExpiration = (this._sizeExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod);
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00018400 File Offset: 0x00016600
		private SmbComBlankResponse Blank_resp()
		{
			bool flag = this._blankResp == null;
			if (flag)
			{
				this._blankResp = new SmbComBlankResponse();
			}
			return this._blankResp;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00018434 File Offset: 0x00016634
		internal virtual void ResolveDfs(ServerMessageBlock request)
		{
			bool flag = !this._enableDfs;
			if (flag)
			{
				this.Connect0();
			}
			else
			{
				bool flag2 = request is SmbComClose;
				if (!flag2)
				{
					this.Connect0();
					DfsReferral dfsReferral = SmbFile.Dfs.Resolve(this.Tree.Session.transport.TconHostName, this.Tree.Share, this.Unc, this.Auth);
					bool flag3 = dfsReferral != null;
					if (flag3)
					{
						string service = null;
						bool flag4 = request != null;
						if (flag4)
						{
							byte command = request.Command;
							if (command != 37 && command != 50)
							{
								service = "A:";
							}
							else
							{
								int num = (int)(((SmbComTransaction)request).SubCommand & byte.MaxValue);
								if (num != 16)
								{
									service = "A:";
								}
							}
						}
						DfsReferral dfsReferral2 = dfsReferral;
						SmbException ex = null;
						do
						{
							try
							{
								bool flag5 = this.Log.Level >= 2;
								if (flag5)
								{
									this.Log.WriteLine("DFS redirect: " + dfsReferral);
								}
								UniAddress byName = UniAddress.GetByName(dfsReferral.Server);
								SmbTransport smbTransport = SmbTransport.GetSmbTransport(byName, this.Url.Port);
								smbTransport.Connect();
								this.Tree = smbTransport.GetSmbSession(this.Auth).GetSmbTree(dfsReferral.Share, service);
								bool flag6 = dfsReferral != dfsReferral2 && dfsReferral.Key != null;
								if (flag6)
								{
									dfsReferral.Map.Put(dfsReferral.Key, dfsReferral);
								}
								ex = null;
								break;
							}
							catch (IOException ex2)
							{
								bool flag7 = ex2 is SmbException;
								if (flag7)
								{
									ex = (SmbException)ex2;
								}
								else
								{
									ex = new SmbException(dfsReferral.Server, ex2);
								}
							}
							dfsReferral = dfsReferral.Next;
						}
						while (dfsReferral != dfsReferral2);
						bool flag8 = ex != null;
						if (flag8)
						{
							throw ex;
						}
						bool flag9 = this.Log.Level >= 3;
						if (flag9)
						{
							this.Log.WriteLine(dfsReferral);
						}
						this._dfsReferral = dfsReferral;
						bool flag10 = dfsReferral.PathConsumed < 0;
						if (flag10)
						{
							dfsReferral.PathConsumed = 0;
						}
						else
						{
							bool flag11 = dfsReferral.PathConsumed > this.Unc.Length;
							if (flag11)
							{
								dfsReferral.PathConsumed = this.Unc.Length;
							}
						}
						string text = Runtime.Substring(this.Unc, dfsReferral.PathConsumed);
						bool flag12 = text.Equals(string.Empty);
						if (flag12)
						{
							text = "\\";
						}
						bool flag13 = !dfsReferral.Path.Equals(string.Empty);
						if (flag13)
						{
							text = "\\" + dfsReferral.Path + text;
						}
						this.Unc = text;
						bool flag14 = request != null && request.Path != null && request.Path.EndsWith("\\") && !text.EndsWith("\\");
						if (flag14)
						{
							text += "\\";
						}
						bool flag15 = request != null;
						if (flag15)
						{
							request.Path = text;
							request.Flags2 |= SmbConstants.Flags2ResolvePathsInDfs;
						}
					}
					else
					{
						bool flag16 = this.Tree.InDomainDfs && !(request is NtTransQuerySecurityDesc) && !(request is SmbComClose) && !(request is SmbComFindClose2);
						if (flag16)
						{
							throw new SmbException(-1073741275, false);
						}
						bool flag17 = request != null;
						if (flag17)
						{
							request.Flags2 &= ~SmbConstants.Flags2ResolvePathsInDfs;
						}
					}
				}
			}
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x000187DC File Offset: 0x000169DC
		internal virtual void Send(ServerMessageBlock request, ServerMessageBlock response)
		{
			for (;;)
			{
				this.ResolveDfs(request);
				try
				{
					this.Tree.Send(request, response);
					break;
				}
				catch (DfsReferral dfsReferral)
				{
					bool resolveHashes = dfsReferral.ResolveHashes;
					if (resolveHashes)
					{
						throw;
					}
					request.Reset();
				}
			}
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x00018834 File Offset: 0x00016A34
		internal static string QueryLookup(string query, string param)
		{
			char[] array = query.ToCharArray();
			int num2;
			int num = num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = (int)array[i];
				bool flag = num3 == 38;
				if (flag)
				{
					bool flag2 = num > num2;
					if (flag2)
					{
						string s = new string(array, num2, num - num2);
						bool flag3 = Runtime.EqualsIgnoreCase(s, param);
						if (flag3)
						{
							num++;
							return new string(array, num, i - num);
						}
					}
					num2 = i + 1;
				}
				else
				{
					bool flag4 = num3 == 61;
					if (flag4)
					{
						num = i;
					}
				}
			}
			bool flag5 = num > num2;
			if (flag5)
			{
				string s2 = new string(array, num2, num - num2);
				bool flag6 = Runtime.EqualsIgnoreCase(s2, param);
				if (flag6)
				{
					num++;
					return new string(array, num, array.Length - num);
				}
			}
			return null;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x00018914 File Offset: 0x00016B14
		internal virtual UniAddress GetAddress()
		{
			bool flag = this.AddressIndex == 0;
			UniAddress result;
			if (flag)
			{
				result = this.GetFirstAddress();
			}
			else
			{
				result = this.Addresses[this.AddressIndex - 1];
			}
			return result;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001894C File Offset: 0x00016B4C
		internal virtual UniAddress GetFirstAddress()
		{
			this.AddressIndex = 0;
			string host = this.Url.GetHost();
			string absolutePath = this.Url.AbsolutePath;
			string query = this.Url.GetQuery();
			bool flag = this.Addresses != null && this.Addresses.Length != 0;
			UniAddress nextAddress;
			if (flag)
			{
				nextAddress = this.GetNextAddress();
			}
			else
			{
				bool flag2 = query != null;
				if (flag2)
				{
					string text = SmbFile.QueryLookup(query, "server");
					bool flag3 = !string.IsNullOrEmpty(text);
					if (flag3)
					{
						this.Addresses = new UniAddress[1];
						this.Addresses[0] = UniAddress.GetByName(text);
						return this.GetNextAddress();
					}
					string text2 = SmbFile.QueryLookup(query, "address");
					bool flag4 = !string.IsNullOrEmpty(text2);
					if (flag4)
					{
						byte[] addressBytes = Extensions.GetAddressByName(text2).GetAddressBytes();
						this.Addresses = new UniAddress[1];
						this.Addresses[0] = new UniAddress(IPAddress.Parse(host));
						return this.GetNextAddress();
					}
				}
				bool flag5 = host.Length == 0;
				if (flag5)
				{
					try
					{
						NbtAddress byName = NbtAddress.GetByName(NbtAddress.MasterBrowserName, 1, null);
						this.Addresses = new UniAddress[1];
						this.Addresses[0] = UniAddress.GetByName(byName.GetHostAddress());
					}
					catch (UnknownHostException ex)
					{
						NtlmPasswordAuthentication.InitDefaults();
						bool flag6 = NtlmPasswordAuthentication.DefaultDomain.Equals("?");
						if (flag6)
						{
							throw;
						}
						this.Addresses = UniAddress.GetAllByName(NtlmPasswordAuthentication.DefaultDomain, true);
					}
				}
				else
				{
					bool flag7 = absolutePath.Length == 0 || absolutePath.Equals("/");
					if (flag7)
					{
						this.Addresses = UniAddress.GetAllByName(host, true);
					}
					else
					{
						this.Addresses = UniAddress.GetAllByName(host, false);
					}
				}
				nextAddress = this.GetNextAddress();
			}
			return nextAddress;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00018B30 File Offset: 0x00016D30
		internal virtual UniAddress GetNextAddress()
		{
			UniAddress result = null;
			bool flag = this.AddressIndex < this.Addresses.Length;
			if (flag)
			{
				UniAddress[] addresses = this.Addresses;
				int addressIndex = this.AddressIndex;
				this.AddressIndex = addressIndex + 1;
				result = addresses[addressIndex];
			}
			return result;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00018B74 File Offset: 0x00016D74
		internal virtual bool HasNextAddress()
		{
			return this.AddressIndex < this.Addresses.Length;
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00018B98 File Offset: 0x00016D98
		internal virtual void Connect0()
		{
			try
			{
				this.Connect();
			}
			catch (UnknownHostException rootCause)
			{
				throw new SmbException("Failed to connect to server", rootCause);
			}
			catch (SmbException ex)
			{
				throw;
			}
			catch (IOException rootCause2)
			{
				throw new SmbException("Failed to connect to server", rootCause2);
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00018BFC File Offset: 0x00016DFC
		internal virtual void DoConnect()
		{
			UniAddress address = this.GetAddress();
			bool flag = this.Tree != null && this.Tree.Session.transport.Address.Equals(address);
			SmbTransport smbTransport;
			if (flag)
			{
				smbTransport = this.Tree.Session.transport;
			}
			else
			{
				smbTransport = SmbTransport.GetSmbTransport(address, this.Url.Port);
				this.Tree = smbTransport.GetSmbSession(this.Auth).GetSmbTree(this._share, null);
			}
			string serverWithDfs = this.GetServerWithDfs();
			bool enableDfs = this._enableDfs;
			if (enableDfs)
			{
				this.Tree.InDomainDfs = (SmbFile.Dfs.Resolve(serverWithDfs, this.Tree.Share, null, this.Auth) != null);
			}
			bool inDomainDfs = this.Tree.InDomainDfs;
			if (inDomainDfs)
			{
				this.Tree.ConnectionState = 2;
			}
			try
			{
				bool flag2 = this.Log.Level >= 3;
				if (flag2)
				{
					this.Log.WriteLine("doConnect: " + address);
				}
				this.Tree.TreeConnect(null, null);
			}
			catch (SmbAuthException ex)
			{
				bool flag3 = this._share == null;
				if (flag3)
				{
					SmbSession smbSession = smbTransport.GetSmbSession(NtlmPasswordAuthentication.Null);
					this.Tree = smbSession.GetSmbTree(null, null);
					this.Tree.TreeConnect(null, null);
				}
				else
				{
					NtlmPasswordAuthentication auth;
					bool flag4 = (auth = NtlmAuthenticator.RequestNtlmPasswordAuthentication(this.Url.ToString(), ex)) != null;
					if (!flag4)
					{
						bool flag5 = this.Log.Level >= 1 && this.HasNextAddress();
						if (flag5)
						{
							Runtime.PrintStackTrace(ex, this.Log);
						}
						throw;
					}
					this.Auth = auth;
					SmbSession smbSession = smbTransport.GetSmbSession(this.Auth);
					this.Tree = smbSession.GetSmbTree(this._share, null);
					this.Tree.InDomainDfs = (SmbFile.Dfs.Resolve(serverWithDfs, this.Tree.Share, null, this.Auth) != null);
					bool inDomainDfs2 = this.Tree.InDomainDfs;
					if (inDomainDfs2)
					{
						this.Tree.ConnectionState = 2;
					}
					this.Tree.TreeConnect(null, null);
				}
			}
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00018E64 File Offset: 0x00017064
		public void Connect()
		{
			bool flag = this.IsConnected();
			if (!flag)
			{
				this.GetUncPath0();
				this.GetFirstAddress();
				for (;;)
				{
					try
					{
						this.DoConnect();
						break;
					}
					catch (SmbAuthException ex)
					{
						throw;
					}
					catch (SmbException ex2)
					{
						bool flag2 = this.GetNextAddress() == null;
						if (flag2)
						{
							throw;
						}
						this.RemoveCurrentAddress();
						bool flag3 = this.Log.Level >= 3;
						if (flag3)
						{
							Runtime.PrintStackTrace(ex2, this.Log);
						}
					}
				}
			}
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x00018F08 File Offset: 0x00017108
		internal virtual bool IsConnected()
		{
			return this.Tree != null && this.Tree.ConnectionState == 2;
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00018F34 File Offset: 0x00017134
		internal virtual int Open0(int flags, int access, int attrs, int options)
		{
			this.Connect0();
			bool flag = this.Log.Level >= 3;
			if (flag)
			{
				this.Log.WriteLine("open0: " + this.Unc);
			}
			bool flag2 = this.Tree.Session.transport.HasCapability(SmbConstants.CapNtSmbs);
			int fid;
			if (flag2)
			{
				SmbComNtCreateAndXResponse smbComNtCreateAndXResponse = new SmbComNtCreateAndXResponse();
				SmbComNtCreateAndX smbComNtCreateAndX = new SmbComNtCreateAndX(this.Unc, flags, access, this._shareAccess, attrs, options, null);
				bool flag3 = this is SmbNamedPipe;
				if (flag3)
				{
					smbComNtCreateAndX.Flags0 |= 22;
					smbComNtCreateAndX.DesiredAccess |= 131072;
					smbComNtCreateAndXResponse.IsExtended = true;
				}
				this.Send(smbComNtCreateAndX, smbComNtCreateAndXResponse);
				fid = smbComNtCreateAndXResponse.Fid;
				this._attributes = (smbComNtCreateAndXResponse.ExtFileAttributes & 32767);
				this._attrExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
				this._isExists = true;
			}
			else
			{
				SmbComOpenAndXResponse smbComOpenAndXResponse = new SmbComOpenAndXResponse();
				this.Send(new SmbComOpenAndX(this.Unc, access, flags, null), smbComOpenAndXResponse);
				fid = smbComOpenAndXResponse.Fid;
			}
			return fid;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00019068 File Offset: 0x00017268
		internal virtual void Open(int flags, int access, int attrs, int options)
		{
			bool flag = this.IsOpen();
			if (!flag)
			{
				this.Fid = this.Open0(flags, access, attrs, options);
				this.Opened = true;
				this.TreeNum = this.Tree.TreeNum;
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000190AC File Offset: 0x000172AC
		internal virtual bool IsOpen()
		{
			return this.Opened && this.IsConnected() && this.TreeNum == this.Tree.TreeNum;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x000190E8 File Offset: 0x000172E8
		internal virtual void Close(int f, long lastWriteTime)
		{
			bool flag = this.Log.Level >= 3;
			if (flag)
			{
				this.Log.WriteLine("close: " + f);
			}
			this.Send(new SmbComClose(f, lastWriteTime), this.Blank_resp());
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x00019140 File Offset: 0x00017340
		internal virtual void Close(long lastWriteTime)
		{
			bool flag = !this.IsOpen();
			if (!flag)
			{
				this.Close(this.Fid, lastWriteTime);
				this.Opened = false;
			}
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x00019173 File Offset: 0x00017373
		internal virtual void Close()
		{
			this.Close(0L);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00019180 File Offset: 0x00017380
		public virtual Principal GetPrincipal()
		{
			return this.Auth;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x00019198 File Offset: 0x00017398
		public virtual string GetName()
		{
			this.GetUncPath0();
			bool flag = this._canon.Length > 1;
			string result;
			if (flag)
			{
				int num = this._canon.Length - 2;
				while (this._canon[num] != '/')
				{
					num--;
				}
				result = Runtime.Substring(this._canon, num + 1);
			}
			else
			{
				bool flag2 = this._share != null;
				if (flag2)
				{
					result = this._share + "/";
				}
				else
				{
					bool flag3 = this.Url.GetHost().Length > 0;
					if (flag3)
					{
						result = this.Url.GetHost() + "/";
					}
					else
					{
						result = "smb://";
					}
				}
			}
			return result;
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001925C File Offset: 0x0001745C
		public virtual string GetParent()
		{
			string text = this.Url.Authority;
			bool flag = text.Length > 0;
			string result;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder("smb://");
				stringBuilder.Append(text);
				this.GetUncPath0();
				bool flag2 = this._canon.Length > 1;
				if (flag2)
				{
					stringBuilder.Append(this._canon);
				}
				else
				{
					stringBuilder.Append('/');
				}
				text = stringBuilder.ToString();
				int num = text.Length - 2;
				while (text[num] != '/')
				{
					num--;
				}
				result = Runtime.Substring(text, 0, num + 1);
			}
			else
			{
				result = "smb://";
			}
			return result;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00019318 File Offset: 0x00017518
		public virtual string GetPath()
		{
			return this.Url.ToString();
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00019338 File Offset: 0x00017538
		internal virtual string GetUncPath0()
		{
			bool flag = this.Unc == null;
			if (flag)
			{
				char[] array = this.Url.LocalPath.ToCharArray();
				char[] array2 = new char[array.Length];
				int num = array.Length;
				int num2 = 0;
				int num3 = 0;
				int i = 0;
				while (i < num)
				{
					switch (num2)
					{
					case 0:
					{
						bool flag2 = array[i] != '/';
						if (flag2)
						{
							return null;
						}
						array2[num3++] = array[i];
						num2 = 1;
						break;
					}
					case 1:
					{
						bool flag3 = array[i] == '/';
						if (!flag3)
						{
							bool flag4 = array[i] == '.' && (i + 1 >= num || array[i + 1] == '/');
							if (flag4)
							{
								i++;
							}
							else
							{
								bool flag5 = i + 1 < num && array[i] == '.' && array[i + 1] == '.' && (i + 2 >= num || array[i + 2] == '/');
								if (!flag5)
								{
									num2 = 2;
									goto IL_146;
								}
								i += 2;
								bool flag6 = num3 == 1;
								if (!flag6)
								{
									do
									{
										num3--;
									}
									while (num3 > 1 && array2[num3 - 1] != '/');
								}
							}
						}
						break;
					}
					case 2:
						goto IL_146;
					}
					IL_169:
					i++;
					continue;
					IL_146:
					bool flag7 = array[i] == '/';
					if (flag7)
					{
						num2 = 1;
					}
					array2[num3++] = array[i];
					goto IL_169;
				}
				this._canon = new string(array2, 0, num3);
				bool flag8 = num3 > 1;
				if (flag8)
				{
					num3--;
					i = this._canon.IndexOf('/', 1);
					bool flag9 = i < 0;
					if (flag9)
					{
						this._share = Runtime.Substring(this._canon, 1);
						this.Unc = "\\";
					}
					else
					{
						bool flag10 = i == num3;
						if (flag10)
						{
							this._share = Runtime.Substring(this._canon, 1, i);
							this.Unc = "\\";
						}
						else
						{
							this._share = Runtime.Substring(this._canon, 1, i);
							this.Unc = Runtime.Substring(this._canon, i, (array2[num3] == '/') ? num3 : (num3 + 1));
							this.Unc = this.Unc.Replace('/', '\\');
						}
					}
				}
				else
				{
					this._share = null;
					this.Unc = "\\";
				}
			}
			return this.Unc;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x000195C8 File Offset: 0x000177C8
		public virtual string GetUncPath()
		{
			this.GetUncPath0();
			bool flag = this._share == null;
			string result;
			if (flag)
			{
				result = "\\\\" + this.Url.GetHost();
			}
			else
			{
				result = "\\\\" + this.Url.GetHost() + this._canon.Replace('/', '\\');
			}
			return result;
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001962C File Offset: 0x0001782C
		public virtual string GetCanonicalPath()
		{
			string authority = this.Url.Authority;
			this.GetUncPath0();
			bool flag = authority.Length > 0;
			string result;
			if (flag)
			{
				result = "smb://" + this.Url.Authority + this._canon;
			}
			else
			{
				result = "smb://";
			}
			return result;
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00019684 File Offset: 0x00017884
		public virtual string GetShare()
		{
			return this._share;
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001969C File Offset: 0x0001789C
		internal virtual string GetServerWithDfs()
		{
			bool flag = this._dfsReferral != null;
			string server;
			if (flag)
			{
				server = this._dfsReferral.Server;
			}
			else
			{
				server = this.GetServer();
			}
			return server;
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x000196D0 File Offset: 0x000178D0
		public virtual string GetServer()
		{
			string host = this.Url.GetHost();
			bool flag = host.Length == 0;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = host;
			}
			return result;
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x00019704 File Offset: 0x00017904
		public new virtual int GetType()
		{
			bool flag = this.Type == 0;
			if (flag)
			{
				bool flag2 = this.GetUncPath0().Length > 1;
				if (flag2)
				{
					this.Type = 1;
				}
				else
				{
					bool flag3 = this._share != null;
					if (flag3)
					{
						this.Connect0();
						bool flag4 = this._share.Equals("IPC$");
						if (flag4)
						{
							this.Type = 16;
						}
						else
						{
							bool flag5 = this.Tree.Service.Equals("LPT1:");
							if (flag5)
							{
								this.Type = 32;
							}
							else
							{
								bool flag6 = this.Tree.Service.Equals("COMM");
								if (flag6)
								{
									this.Type = 64;
								}
								else
								{
									this.Type = 8;
								}
							}
						}
					}
					else
					{
						bool flag7 = string.IsNullOrEmpty(this.Url.Authority);
						if (flag7)
						{
							this.Type = 2;
						}
						else
						{
							UniAddress address;
							try
							{
								address = this.GetAddress();
							}
							catch (UnknownHostException rootCause)
							{
								throw new SmbException(this.Url.ToString(), rootCause);
							}
							bool flag8 = address.GetAddress() is NbtAddress;
							if (flag8)
							{
								int nameType = ((NbtAddress)address.GetAddress()).GetNameType();
								bool flag9 = nameType == 29 || nameType == 27;
								if (flag9)
								{
									this.Type = 2;
									return this.Type;
								}
							}
							this.Type = 4;
						}
					}
				}
			}
			return this.Type;
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001989C File Offset: 0x00017A9C
		internal virtual bool IsWorkgroup0()
		{
			bool flag = this.Type == 2 || this.Url.GetHost().Length == 0;
			bool result;
			if (flag)
			{
				this.Type = 2;
				result = true;
			}
			else
			{
				this.GetUncPath0();
				bool flag2 = this._share == null;
				if (flag2)
				{
					UniAddress address = this.GetAddress();
					bool flag3 = address.GetAddress() is NbtAddress;
					if (flag3)
					{
						int nameType = ((NbtAddress)address.GetAddress()).GetNameType();
						bool flag4 = nameType == 29 || nameType == 27;
						if (flag4)
						{
							this.Type = 2;
							return true;
						}
					}
					this.Type = 4;
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001994C File Offset: 0x00017B4C
		internal virtual IInfo QueryPath(string path, int infoLevel)
		{
			this.Connect0();
			bool flag = this.Log.Level >= 3;
			if (flag)
			{
				this.Log.WriteLine("queryPath: " + path);
			}
			bool flag2 = this.Tree.Session.transport.HasCapability(SmbConstants.CapNtSmbs);
			IInfo result;
			if (flag2)
			{
				Trans2QueryPathInformationResponse trans2QueryPathInformationResponse = new Trans2QueryPathInformationResponse(infoLevel);
				this.Send(new Trans2QueryPathInformation(path, infoLevel), trans2QueryPathInformationResponse);
				result = trans2QueryPathInformationResponse.Info;
			}
			else
			{
				SmbComQueryInformationResponse smbComQueryInformationResponse = new SmbComQueryInformationResponse((long)(this.Tree.Session.transport.Server.ServerTimeZone * 1000) * 60L);
				this.Send(new SmbComQueryInformation(path), smbComQueryInformationResponse);
				result = smbComQueryInformationResponse;
			}
			return result;
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x00019A10 File Offset: 0x00017C10
		public virtual bool Exists()
		{
			bool flag = this._attrExpiration > Runtime.CurrentTimeMillis();
			bool isExists;
			if (flag)
			{
				isExists = this._isExists;
			}
			else
			{
				this._attributes = 17;
				this._createTime = 0L;
				this._lastModified = 0L;
				this._isExists = false;
				try
				{
					bool flag2 = this.Url.GetHost().Length == 0;
					if (!flag2)
					{
						bool flag3 = this._share == null;
						if (flag3)
						{
							bool flag4 = this.GetType() == 2;
							if (flag4)
							{
								UniAddress.GetByName(this.Url.GetHost(), true);
							}
							else
							{
								UniAddress.GetByName(this.Url.GetHost()).GetHostName();
							}
						}
						else
						{
							bool flag5 = this.GetUncPath0().Length == 1 || Runtime.EqualsIgnoreCase(this._share, "IPC$");
							if (flag5)
							{
								this.Connect0();
							}
							else
							{
								IInfo info = this.QueryPath(this.GetUncPath0(), 257);
								this._attributes = info.GetAttributes();
								this._createTime = info.GetCreateTime();
								this._lastModified = info.GetLastWriteTime();
							}
						}
					}
					this._isExists = true;
				}
				catch (UnknownHostException)
				{
				}
				catch (SmbException ex)
				{
					int ntStatus = ex.GetNtStatus();
					if (ntStatus <= -1073741773)
					{
						if (ntStatus != -1073741809 && ntStatus != -1073741773)
						{
							goto IL_170;
						}
					}
					else if (ntStatus != -1073741772 && ntStatus != -1073741766)
					{
						goto IL_170;
					}
					goto IL_176;
					IL_170:
					throw;
				}
				IL_176:
				this._attrExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
				isExists = this._isExists;
			}
			return isExists;
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x00019BCC File Offset: 0x00017DCC
		public virtual bool CanRead()
		{
			bool flag = this.GetType() == 16;
			return flag || this.Exists();
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00019BF8 File Offset: 0x00017DF8
		public virtual bool CanWrite()
		{
			bool flag = this.GetType() == 16;
			return flag || (this.Exists() && (this._attributes & 1) == 0);
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x00019C34 File Offset: 0x00017E34
		public virtual bool IsDirectory()
		{
			bool flag = this.GetUncPath0().Length == 1;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !this.Exists();
				result = (!flag2 && (this._attributes & 16) == 16);
			}
			return result;
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00019C7C File Offset: 0x00017E7C
		public virtual bool IsFile()
		{
			bool flag = this.GetUncPath0().Length == 1;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Exists();
				result = ((this._attributes & 16) == 0);
			}
			return result;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00019CB8 File Offset: 0x00017EB8
		public virtual bool IsHidden()
		{
			bool flag = this._share == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.GetUncPath0().Length == 1;
				if (flag2)
				{
					bool flag3 = this._share.EndsWith("$");
					result = flag3;
				}
				else
				{
					this.Exists();
					result = ((this._attributes & 2) == 2);
				}
			}
			return result;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00019D20 File Offset: 0x00017F20
		public virtual string GetDfsPath()
		{
			this.ResolveDfs(null);
			bool flag = this._dfsReferral == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string text = string.Concat(new string[]
				{
					"smb:/",
					this._dfsReferral.Server,
					"/",
					this._dfsReferral.Share,
					this.Unc
				});
				text = text.Replace('\\', '/');
				bool flag2 = this.IsDirectory();
				if (flag2)
				{
					text += "/";
				}
				result = text;
			}
			return result;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00019DB4 File Offset: 0x00017FB4
		public virtual long CreateTime()
		{
			bool flag = this.GetUncPath0().Length > 1;
			long result;
			if (flag)
			{
				this.Exists();
				result = this._createTime;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00019DEC File Offset: 0x00017FEC
		public virtual long LastModified()
		{
			bool flag = this.GetUncPath0().Length > 1;
			long result;
			if (flag)
			{
				this.Exists();
				result = this._lastModified;
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00019E24 File Offset: 0x00018024
		public virtual string[] List()
		{
			return this.List("*", 22, null, null);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00019E48 File Offset: 0x00018048
		public virtual string[] List(ISmbFilenameFilter filter)
		{
			return this.List("*", 22, filter, null);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x00019E6C File Offset: 0x0001806C
		public virtual SmbFile[] ListFiles()
		{
			return this.ListFiles("*", 22, null, null);
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00019E90 File Offset: 0x00018090
		public virtual SmbFile[] ListFiles(string wildcard)
		{
			return this.ListFiles(wildcard, 22, null, null);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00019EB0 File Offset: 0x000180B0
		public virtual SmbFile[] ListFiles(ISmbFilenameFilter filter)
		{
			return this.ListFiles("*", 22, filter, null);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00019ED4 File Offset: 0x000180D4
		public virtual SmbFile[] ListFiles(ISmbFileFilter filter)
		{
			return this.ListFiles("*", 22, null, filter);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x00019EF8 File Offset: 0x000180F8
		internal virtual string[] List(string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			List<object> list = new List<object>();
			this.DoEnum(list, false, wildcard, searchAttributes, fnf, ff);
			return Collections.ToArray<string>(list);
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00019F24 File Offset: 0x00018124
		internal virtual SmbFile[] ListFiles(string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			List<object> list = new List<object>();
			this.DoEnum(list, true, wildcard, searchAttributes, fnf, ff);
			return Collections.ToArray<SmbFile>(list);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00019F50 File Offset: 0x00018150
		internal virtual void DoEnum(List<object> list, bool files, string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			bool flag = ff != null && ff is DosFileFilter;
			if (flag)
			{
				DosFileFilter dosFileFilter = (DosFileFilter)ff;
				bool flag2 = dosFileFilter.Wildcard != null;
				if (flag2)
				{
					wildcard = dosFileFilter.Wildcard;
				}
				searchAttributes = dosFileFilter.Attributes;
			}
			try
			{
				int num = (this.Url.GetHost() != null) ? this.Url.GetHost().Length : 0;
				bool flag3 = num == 0 || this.GetType() == 2;
				if (flag3)
				{
					this.DoNetServerEnum(list, files, wildcard, searchAttributes, fnf, ff);
				}
				else
				{
					bool flag4 = this._share == null;
					if (flag4)
					{
						this.DoShareEnum(list, files, wildcard, searchAttributes, fnf, ff);
					}
					else
					{
						this.DoFindFirstNext(list, files, wildcard, searchAttributes, fnf, ff);
					}
				}
			}
			catch (UnknownHostException rootCause)
			{
				throw new SmbException(this.Url.ToString(), rootCause);
			}
			catch (UriFormatException rootCause2)
			{
				throw new SmbException(this.Url.ToString(), rootCause2);
			}
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001A06C File Offset: 0x0001826C
		private void RemoveCurrentAddress()
		{
			bool flag = this.AddressIndex >= 1;
			if (flag)
			{
				UniAddress[] array = new UniAddress[this.Addresses.Length - 1];
				Array.Copy(this.Addresses, 1, array, 0, this.Addresses.Length - 1);
				this.Addresses = array;
				this.AddressIndex--;
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001A0CC File Offset: 0x000182CC
		internal virtual void DoShareEnum(List<object> list, bool files, string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			string absolutePath = this.Url.AbsolutePath;
			IOException ex = null;
			bool flag = absolutePath.LastIndexOf('/') != absolutePath.Length - 1;
			if (flag)
			{
				throw new SmbException(this.Url + " directory must end with '/'");
			}
			bool flag2 = this.GetType() != 4;
			if (flag2)
			{
				throw new SmbException("The requested list operations is invalid: " + this.Url);
			}
			Hashtable hashtable = new Hashtable();
			bool flag3 = this._enableDfs && SmbFile.Dfs.IsTrustedDomain(this.GetServer(), this.Auth);
			if (flag3)
			{
				try
				{
					foreach (IFileEntry fileEntry in this.DoDfsRootEnum())
					{
						bool flag4 = !hashtable.ContainsKey(fileEntry);
						if (flag4)
						{
							hashtable.Put(fileEntry, fileEntry);
						}
					}
				}
				catch (IOException ex2)
				{
					bool flag5 = this.Log.Level >= 4;
					if (flag5)
					{
						Runtime.PrintStackTrace(ex2, this.Log);
					}
				}
			}
			UniAddress uniAddress = this.GetFirstAddress();
			while (uniAddress != null)
			{
				try
				{
					ex = null;
					this.DoConnect();
					IFileEntry[] array;
					try
					{
						array = this.DoMsrpcShareEnum();
					}
					catch (IOException ex3)
					{
						bool flag6 = this.Log.Level >= 3;
						if (flag6)
						{
							Runtime.PrintStackTrace(ex3, this.Log);
						}
						array = this.DoNetShareEnum();
					}
					foreach (IFileEntry fileEntry in array)
					{
						bool flag7 = !hashtable.ContainsKey(fileEntry);
						if (flag7)
						{
							hashtable.Put(fileEntry, fileEntry);
						}
					}
					break;
				}
				catch (IOException ex4)
				{
					bool flag8 = this.Log.Level >= 3;
					if (flag8)
					{
						Runtime.PrintStackTrace(ex4, this.Log);
					}
					ex = ex4;
					bool flag9 = !(ex4 is SmbAuthException);
					if (!flag9)
					{
						break;
					}
					this.RemoveCurrentAddress();
					uniAddress = this.GetNextAddress();
				}
			}
			bool flag10 = ex != null && hashtable.Count == 0;
			if (!flag10)
			{
				foreach (object obj in hashtable.Keys)
				{
					IFileEntry fileEntry = (IFileEntry)obj;
					string name = fileEntry.GetName();
					bool flag11 = fnf != null && !fnf.Accept(this, name);
					if (!flag11)
					{
						bool flag12 = name.Length > 0;
						if (flag12)
						{
							SmbFile smbFile = new SmbFile(this, name, fileEntry.GetType(), 17, 0L, 0L, 0L);
							bool flag13 = ff != null && !ff.Accept(smbFile);
							if (!flag13)
							{
								if (files)
								{
									list.Add(smbFile);
								}
								else
								{
									list.Add(name);
								}
							}
						}
					}
				}
				return;
			}
			bool flag14 = !(ex is SmbException);
			if (flag14)
			{
				throw new SmbException(this.Url.ToString(), ex);
			}
			throw (SmbException)ex;
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0001A434 File Offset: 0x00018634
		internal virtual IFileEntry[] DoDfsRootEnum()
		{
			DcerpcHandle dcerpcHandle = null;
			dcerpcHandle = DcerpcHandle.GetHandle("ncacn_np:" + this.GetAddress().GetHostAddress() + "[\\PIPE\\netdfs]", this.Auth);
			IFileEntry[] entries;
			try
			{
				MsrpcDfsRootEnum msrpcDfsRootEnum = new MsrpcDfsRootEnum(this.GetServer());
				dcerpcHandle.Sendrecv(msrpcDfsRootEnum);
				bool flag = msrpcDfsRootEnum.Retval != 0;
				if (flag)
				{
					throw new SmbException(msrpcDfsRootEnum.Retval, true);
				}
				entries = msrpcDfsRootEnum.GetEntries();
			}
			finally
			{
				try
				{
					dcerpcHandle.Close();
				}
				catch (IOException ex)
				{
					bool flag2 = this.Log.Level >= 4;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex, this.Log);
					}
				}
			}
			return entries;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0001A500 File Offset: 0x00018700
		internal virtual IFileEntry[] DoMsrpcShareEnum()
		{
			MsrpcShareEnum msrpcShareEnum = new MsrpcShareEnum(this.Url.GetHost());
			DcerpcHandle handle = DcerpcHandle.GetHandle("ncacn_np:" + this.GetAddress().GetHostAddress() + "[\\PIPE\\srvsvc]", this.Auth);
			IFileEntry[] entries;
			try
			{
				handle.Sendrecv(msrpcShareEnum);
				bool flag = msrpcShareEnum.Retval != 0;
				if (flag)
				{
					throw new SmbException(msrpcShareEnum.Retval, true);
				}
				entries = msrpcShareEnum.GetEntries();
			}
			finally
			{
				try
				{
					handle.Close();
				}
				catch (IOException ex)
				{
					bool flag2 = this.Log.Level >= 4;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex, this.Log);
					}
				}
			}
			return entries;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0001A5CC File Offset: 0x000187CC
		internal virtual IFileEntry[] DoNetShareEnum()
		{
			SmbComTransaction request = new NetShareEnum();
			SmbComTransactionResponse smbComTransactionResponse = new NetShareEnumResponse();
			this.Send(request, smbComTransactionResponse);
			bool flag = smbComTransactionResponse.Status != WinError.ErrorSuccess;
			if (flag)
			{
				throw new SmbException(smbComTransactionResponse.Status, true);
			}
			return smbComTransactionResponse.Results;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001A61C File Offset: 0x0001881C
		internal virtual void DoNetServerEnum(List<object> list, bool files, string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			int num = (this.Url.GetHost().Length == 0) ? 0 : this.GetType();
			bool flag = num == 0;
			SmbComTransaction smbComTransaction;
			SmbComTransactionResponse smbComTransactionResponse;
			if (flag)
			{
				this.Connect0();
				smbComTransaction = new NetServerEnum2(this.Tree.Session.transport.Server.OemDomainName, int.MinValue);
				smbComTransactionResponse = new NetServerEnum2Response();
			}
			else
			{
				bool flag2 = num == 2;
				if (!flag2)
				{
					throw new SmbException("The requested list operations is invalid: " + this.Url);
				}
				smbComTransaction = new NetServerEnum2(this.Url.GetHost(), -1);
				smbComTransactionResponse = new NetServerEnum2Response();
			}
			for (;;)
			{
				this.Send(smbComTransaction, smbComTransactionResponse);
				bool flag3 = smbComTransactionResponse.Status != WinError.ErrorSuccess && smbComTransactionResponse.Status != WinError.ErrorMoreData;
				if (flag3)
				{
					break;
				}
				bool flag4 = smbComTransactionResponse.Status == WinError.ErrorMoreData;
				int num2 = flag4 ? (smbComTransactionResponse.NumEntries - 1) : smbComTransactionResponse.NumEntries;
				for (int i = 0; i < num2; i++)
				{
					IFileEntry fileEntry = smbComTransactionResponse.Results[i];
					string name = fileEntry.GetName();
					bool flag5 = fnf != null && !fnf.Accept(this, name);
					if (!flag5)
					{
						bool flag6 = name.Length > 0;
						if (flag6)
						{
							SmbFile smbFile = new SmbFile(this, name, fileEntry.GetType(), 17, 0L, 0L, 0L);
							bool flag7 = ff != null && !ff.Accept(smbFile);
							if (!flag7)
							{
								if (files)
								{
									list.Add(smbFile);
								}
								else
								{
									list.Add(name);
								}
							}
						}
					}
				}
				bool flag8 = this.GetType() != 2;
				if (flag8)
				{
					return;
				}
				smbComTransaction.SubCommand = 215;
				smbComTransaction.Reset(0, ((NetServerEnum2Response)smbComTransactionResponse).LastName);
				smbComTransactionResponse.Reset();
				if (!flag4)
				{
					return;
				}
			}
			throw new SmbException(smbComTransactionResponse.Status, true);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001A824 File Offset: 0x00018A24
		internal virtual void DoFindFirstNext(List<object> list, bool files, string wildcard, int searchAttributes, ISmbFilenameFilter fnf, ISmbFileFilter ff)
		{
			string uncPath = this.GetUncPath0();
			string absolutePath = this.Url.AbsolutePath;
			bool flag = absolutePath.LastIndexOf('/') != absolutePath.Length - 1;
			if (flag)
			{
				throw new SmbException(this.Url + " directory must end with '/'");
			}
			SmbComTransaction smbComTransaction = new Trans2FindFirst2(uncPath, wildcard, searchAttributes);
			Trans2FindFirst2Response trans2FindFirst2Response = new Trans2FindFirst2Response();
			bool flag2 = this.Log.Level >= 3;
			if (flag2)
			{
				this.Log.WriteLine("doFindFirstNext: " + smbComTransaction.Path);
			}
			this.Send(smbComTransaction, trans2FindFirst2Response);
			int sid = trans2FindFirst2Response.Sid;
			smbComTransaction = new Trans2FindNext2(sid, trans2FindFirst2Response.ResumeKey, trans2FindFirst2Response.LastName);
			trans2FindFirst2Response.SubCommand = 2;
			for (;;)
			{
				int i = 0;
				while (i < trans2FindFirst2Response.NumEntries)
				{
					IFileEntry fileEntry = trans2FindFirst2Response.Results[i];
					string name = fileEntry.GetName();
					bool flag3 = name.Length < 3;
					if (flag3)
					{
						int hashCode = name.GetHashCode();
						bool flag4 = hashCode == SmbFile.HashDot || hashCode == SmbFile.HashDotDot;
						if (flag4)
						{
							bool flag5 = name.Equals(".") || name.Equals("..");
							if (flag5)
							{
								goto IL_1CE;
							}
						}
						goto IL_13C;
					}
					goto IL_13C;
					IL_1CE:
					i++;
					continue;
					IL_13C:
					bool flag6 = fnf != null && !fnf.Accept(this, name);
					if (flag6)
					{
						goto IL_1CE;
					}
					bool flag7 = name.Length > 0;
					if (flag7)
					{
						SmbFile smbFile = new SmbFile(this, name, 1, fileEntry.GetAttributes(), fileEntry.CreateTime(), fileEntry.LastModified(), fileEntry.Length());
						bool flag8 = ff != null && !ff.Accept(smbFile);
						if (!flag8)
						{
							if (files)
							{
								list.Add(smbFile);
							}
							else
							{
								list.Add(name);
							}
						}
					}
					goto IL_1CE;
				}
				bool flag9 = trans2FindFirst2Response.IsEndOfSearch || trans2FindFirst2Response.NumEntries == 0;
				if (flag9)
				{
					break;
				}
				smbComTransaction.Reset(trans2FindFirst2Response.ResumeKey, trans2FindFirst2Response.LastName);
				trans2FindFirst2Response.Reset();
				this.Send(smbComTransaction, trans2FindFirst2Response);
			}
			try
			{
				this.Send(new SmbComFindClose2(sid), this.Blank_resp());
			}
			catch (SmbException ex)
			{
				bool flag10 = this.Log.Level >= 4;
				if (flag10)
				{
					Runtime.PrintStackTrace(ex, this.Log);
				}
			}
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001AAB4 File Offset: 0x00018CB4
		public virtual void RenameTo(SmbFile dest)
		{
			bool flag = this.GetUncPath0().Length == 1 || dest.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			this.ResolveDfs(null);
			dest.ResolveDfs(null);
			bool flag2 = !this.Tree.Equals(dest.Tree);
			if (flag2)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			bool flag3 = this.Log.Level >= 3;
			if (flag3)
			{
				this.Log.WriteLine("renameTo: " + this.Unc + " -> " + dest.Unc);
			}
			this._attrExpiration = (this._sizeExpiration = 0L);
			dest._attrExpiration = 0L;
			this.Send(new SmbComRename(this.Unc, dest.Unc), this.Blank_resp());
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001AB9C File Offset: 0x00018D9C
		internal virtual void CopyTo0(SmbFile dest, byte[][] b, int bsize, SmbFile.WriterThread w, SmbComReadAndX req, SmbComReadAndXResponse resp)
		{
			bool flag = this._attrExpiration < Runtime.CurrentTimeMillis();
			if (flag)
			{
				this._attributes = 17;
				this._createTime = 0L;
				this._lastModified = 0L;
				this._isExists = false;
				IInfo info = this.QueryPath(this.GetUncPath0(), 257);
				this._attributes = info.GetAttributes();
				this._createTime = info.GetCreateTime();
				this._lastModified = info.GetLastWriteTime();
				this._isExists = true;
				this._attrExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
			}
			bool flag2 = this.IsDirectory();
			if (flag2)
			{
				string uncPath = dest.GetUncPath0();
				bool flag3 = uncPath.Length > 1;
				if (flag3)
				{
					try
					{
						dest.Mkdir();
						dest.SetPathInformation(this._attributes, this._createTime, this._lastModified);
					}
					catch (SmbException ex)
					{
						bool flag4 = ex.GetNtStatus() != -1073741790 && ex.GetNtStatus() != -1073741771;
						if (flag4)
						{
							throw;
						}
					}
				}
				SmbFile[] array = this.ListFiles("*", 22, null, null);
				try
				{
					for (int i = 0; i < array.Length; i++)
					{
						SmbFile dest2 = new SmbFile(dest, array[i].GetName(), array[i].Type, array[i]._attributes, array[i]._createTime, array[i]._lastModified, array[i]._size);
						array[i].CopyTo0(dest2, b, bsize, w, req, resp);
					}
				}
				catch (UnknownHostException rootCause)
				{
					throw new SmbException(this.Url.ToString(), rootCause);
				}
				catch (UriFormatException rootCause2)
				{
					throw new SmbException(this.Url.ToString(), rootCause2);
				}
			}
			else
			{
				try
				{
					this.Open(1, 0, 128, 0);
					try
					{
						dest.Open(82, SmbConstants.FileWriteData | SmbConstants.FileWriteAttributes, this._attributes, 0);
					}
					catch (SmbAuthException ex2)
					{
						bool flag5 = (dest._attributes & 1) != 0;
						if (!flag5)
						{
							throw;
						}
						dest.SetPathInformation(dest._attributes & -2, 0L, 0L);
						dest.Open(82, SmbConstants.FileWriteData | SmbConstants.FileWriteAttributes, this._attributes, 0);
					}
					int i = 0;
					long num = 0L;
					for (;;)
					{
						req.SetParam(this.Fid, num, bsize);
						resp.SetParam(b[i], 0);
						this.Send(req, resp);
						lock (w)
						{
							bool flag7 = w.E != null;
							if (flag7)
							{
								throw w.E;
							}
							while (!w.Ready)
							{
								try
								{
									Runtime.Wait(w);
								}
								catch (Exception rootCause3)
								{
									throw new SmbException(dest.Url.ToString(), rootCause3);
								}
							}
							bool flag8 = w.E != null;
							if (flag8)
							{
								throw w.E;
							}
							bool flag9 = resp.DataLength <= 0;
							if (flag9)
							{
								break;
							}
							w.Write(b[i], resp.DataLength, dest, num);
						}
						i = ((i == 1) ? 0 : 1);
						num += (long)resp.DataLength;
					}
					dest.Send(new Trans2SetFileInformation(dest.Fid, this._attributes, this._createTime, this._lastModified), new Trans2SetFileInformationResponse());
					dest.Close(0L);
				}
				catch (SmbException ex3)
				{
					bool flag10 = !SmbFile.IgnoreCopyToException;
					if (flag10)
					{
						throw new SmbException(string.Concat(new object[]
						{
							"Failed to copy file from [",
							this.ToString(),
							"] to [",
							dest,
							"]"
						}), ex3);
					}
					bool flag11 = this.Log.Level > 1;
					if (flag11)
					{
						Runtime.PrintStackTrace(ex3, this.Log);
					}
				}
				finally
				{
					this.Close();
				}
			}
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001B050 File Offset: 0x00019250
		public virtual void CopyTo(SmbFile dest)
		{
			bool flag = this._share == null || dest._share == null;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups or servers");
			}
			SmbComReadAndX req = new SmbComReadAndX();
			SmbComReadAndXResponse resp = new SmbComReadAndXResponse();
			this.Connect0();
			dest.Connect0();
			this.ResolveDfs(null);
			try
			{
				bool flag2 = this.GetAddress().Equals(dest.GetAddress()) && this._canon.RegionMatches(true, 0, dest._canon, 0, Math.Min(this._canon.Length, dest._canon.Length));
				if (flag2)
				{
					throw new SmbException("Source and destination paths overlap.");
				}
			}
			catch (UnknownHostException)
			{
			}
			SmbFile.WriterThread writerThread = new SmbFile.WriterThread(this);
			writerThread.SetDaemon(true);
			writerThread.Start();
			SmbTransport transport = this.Tree.Session.transport;
			SmbTransport transport2 = dest.Tree.Session.transport;
			bool flag3 = transport.SndBufSize < transport2.SndBufSize;
			if (flag3)
			{
				transport2.SndBufSize = transport.SndBufSize;
			}
			else
			{
				transport.SndBufSize = transport2.SndBufSize;
			}
			int num = Math.Min(transport.RcvBufSize - 70, transport.SndBufSize - 70);
			byte[][] b = new byte[][]
			{
				new byte[num],
				new byte[num]
			};
			try
			{
				this.CopyTo0(dest, b, num, writerThread, req, resp);
			}
			finally
			{
				writerThread.Write(null, -1, null, 0L);
			}
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001B1EC File Offset: 0x000193EC
		public virtual void Delete()
		{
			this.Exists();
			this.GetUncPath0();
			this.Delete(this.Unc);
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x0001B20C File Offset: 0x0001940C
		internal virtual void Delete(string fileName)
		{
			bool flag = this.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			bool flag2 = Runtime.CurrentTimeMillis() > this._attrExpiration;
			if (flag2)
			{
				this._attributes = 17;
				this._createTime = 0L;
				this._lastModified = 0L;
				this._isExists = false;
				IInfo info = this.QueryPath(this.GetUncPath0(), 257);
				this._attributes = info.GetAttributes();
				this._createTime = info.GetCreateTime();
				this._lastModified = info.GetLastWriteTime();
				this._attrExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
				this._isExists = true;
			}
			bool flag3 = (this._attributes & 1) != 0;
			if (flag3)
			{
				this.SetReadWrite();
			}
			bool flag4 = this.Log.Level >= 3;
			if (flag4)
			{
				this.Log.WriteLine("delete: " + fileName);
			}
			bool flag5 = (this._attributes & 16) != 0;
			if (flag5)
			{
				try
				{
					SmbFile[] array = this.ListFiles("*", 22, null, null);
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Delete();
					}
				}
				catch (SmbException ex)
				{
					bool flag6 = ex.GetNtStatus() != -1073741809;
					if (flag6)
					{
						throw;
					}
				}
				this.Send(new SmbComDeleteDirectory(fileName), this.Blank_resp());
			}
			else
			{
				this.Send(new SmbComDelete(fileName), this.Blank_resp());
			}
			this._attrExpiration = (this._sizeExpiration = 0L);
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x0001B3BC File Offset: 0x000195BC
		public virtual long Length()
		{
			bool flag = this._sizeExpiration > Runtime.CurrentTimeMillis();
			long size;
			if (flag)
			{
				size = this._size;
			}
			else
			{
				bool flag2 = this.GetType() == 8;
				if (flag2)
				{
					int informationLevel = 1;
					Trans2QueryFsInformationResponse trans2QueryFsInformationResponse = new Trans2QueryFsInformationResponse(informationLevel);
					this.Send(new Trans2QueryFsInformation(informationLevel), trans2QueryFsInformationResponse);
					this._size = trans2QueryFsInformationResponse.Info.GetCapacity();
				}
				else
				{
					bool flag3 = this.GetUncPath0().Length > 1 && this.Type != 16;
					if (flag3)
					{
						IInfo info = this.QueryPath(this.GetUncPath0(), 258);
						this._size = info.GetSize();
					}
					else
					{
						this._size = 0L;
					}
				}
				this._sizeExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
				size = this._size;
			}
			return size;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001B494 File Offset: 0x00019694
		public virtual long GetDiskFreeSpace()
		{
			bool flag = this.GetType() == 8 || this.Type == 1;
			if (flag)
			{
				int level = 1007;
				try
				{
					return this.QueryFsInformation(level);
				}
				catch (SmbException ex)
				{
					int ntStatus = ex.GetNtStatus();
					if (ntStatus != -1073741823 && ntStatus != -1073741821)
					{
						throw;
					}
					level = 1;
					return this.QueryFsInformation(level);
				}
			}
			return 0L;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001B514 File Offset: 0x00019714
		private long QueryFsInformation(int level)
		{
			Trans2QueryFsInformationResponse trans2QueryFsInformationResponse = new Trans2QueryFsInformationResponse(level);
			this.Send(new Trans2QueryFsInformation(level), trans2QueryFsInformationResponse);
			bool flag = this.Type == 8;
			if (flag)
			{
				this._size = trans2QueryFsInformationResponse.Info.GetCapacity();
				this._sizeExpiration = Runtime.CurrentTimeMillis() + SmbFile.AttrExpirationPeriod;
			}
			return trans2QueryFsInformationResponse.Info.GetFree();
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001B578 File Offset: 0x00019778
		public virtual void Mkdir()
		{
			string uncPath = this.GetUncPath0();
			bool flag = uncPath.Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			bool flag2 = this.Log.Level >= 3;
			if (flag2)
			{
				this.Log.WriteLine("mkdir: " + uncPath);
			}
			this.Send(new SmbComCreateDirectory(uncPath), this.Blank_resp());
			this._attrExpiration = (this._sizeExpiration = 0L);
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001B5F8 File Offset: 0x000197F8
		public virtual void Mkdirs()
		{
			SmbFile smbFile;
			try
			{
				smbFile = new SmbFile(this.GetParent(), this.Auth);
			}
			catch (IOException)
			{
				return;
			}
			bool flag = !smbFile.Exists();
			if (flag)
			{
				smbFile.Mkdirs();
			}
			this.Mkdir();
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001B650 File Offset: 0x00019850
		public virtual void CreateNewFile()
		{
			bool flag = this.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			this.Close(this.Open0(51, 0, 128, 0), 0L);
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x0001B694 File Offset: 0x00019894
		internal virtual void SetPathInformation(int attrs, long ctime, long mtime)
		{
			this.Exists();
			int num = this._attributes & 16;
			int num2 = this.Open0(1, SmbConstants.FileWriteAttributes, num, (num != 0) ? 1 : 64);
			this.Send(new Trans2SetFileInformation(num2, attrs | num, ctime, mtime), new Trans2SetFileInformationResponse());
			this.Close(num2, 0L);
			this._attrExpiration = 0L;
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001B6F4 File Offset: 0x000198F4
		public virtual void SetCreateTime(long time)
		{
			bool flag = this.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			this.SetPathInformation(0, time, 0L);
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001B72C File Offset: 0x0001992C
		public virtual void SetLastModified(long time)
		{
			bool flag = this.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			this.SetPathInformation(0, 0L, time);
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001B764 File Offset: 0x00019964
		public virtual int GetAttributes()
		{
			bool flag = this.GetUncPath0().Length == 1;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				this.Exists();
				result = (this._attributes & 32767);
			}
			return result;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001B7A0 File Offset: 0x000199A0
		public virtual void SetAttributes(int attrs)
		{
			bool flag = this.GetUncPath0().Length == 1;
			if (flag)
			{
				throw new SmbException("Invalid operation for workgroups, servers, or shares");
			}
			this.SetPathInformation(attrs & 12455, 0L, 0L);
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001B7DE File Offset: 0x000199DE
		public virtual void SetReadOnly()
		{
			this.SetAttributes(this.GetAttributes() | 1);
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001B7F0 File Offset: 0x000199F0
		public virtual void SetReadWrite()
		{
			this.SetAttributes(this.GetAttributes() & -2);
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001B804 File Offset: 0x00019A04
		[Obsolete("Use getURL() instead")]
		public virtual Uri ToUrl()
		{
			return this.Url;
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001B81C File Offset: 0x00019A1C
		public override int GetHashCode()
		{
			int hashCode;
			try
			{
				hashCode = this.GetAddress().GetHashCode();
			}
			catch (UnknownHostException)
			{
				hashCode = this.GetServer().ToUpper().GetHashCode();
			}
			this.GetUncPath0();
			return hashCode + this._canon.ToUpper().GetHashCode();
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001B880 File Offset: 0x00019A80
		protected internal virtual bool PathNamesPossiblyEqual(string path1, string path2)
		{
			int num = path1.LastIndexOf('/');
			int num2 = path2.LastIndexOf('/');
			int num3 = path1.Length - num;
			int num4 = path2.Length - num2;
			bool flag = num3 > 1 && path1[num + 1] == '.';
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = num4 > 1 && path2[num2 + 1] == '.';
				result = (flag2 || (num3 == num4 && path1.RegionMatches(true, num, path2, num2, num3)));
			}
			return result;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001B90C File Offset: 0x00019B0C
		public override bool Equals(object obj)
		{
			bool flag = obj is SmbFile;
			if (flag)
			{
				SmbFile smbFile = (SmbFile)obj;
				bool flag2 = this == smbFile;
				if (flag2)
				{
					return true;
				}
				bool flag3 = this.PathNamesPossiblyEqual(this.Url.AbsolutePath, smbFile.Url.AbsolutePath);
				if (flag3)
				{
					this.GetUncPath0();
					smbFile.GetUncPath0();
					bool flag4 = Runtime.EqualsIgnoreCase(this._canon, smbFile._canon);
					if (flag4)
					{
						bool result;
						try
						{
							result = this.GetAddress().Equals(smbFile.GetAddress());
						}
						catch (UnknownHostException)
						{
							result = Runtime.EqualsIgnoreCase(this.GetServer(), smbFile.GetServer());
						}
						return result;
					}
				}
			}
			return false;
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001B9D8 File Offset: 0x00019BD8
		public override string ToString()
		{
			return this.Url.ToString();
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001B9F8 File Offset: 0x00019BF8
		public int GetContentLength()
		{
			try
			{
				return (int)(this.Length() & (long)(-1));
			}
			catch (SmbException)
			{
			}
			return 0;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001BA30 File Offset: 0x00019C30
		public long GetDate()
		{
			try
			{
				return this.LastModified();
			}
			catch (SmbException)
			{
			}
			return 0L;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001BA64 File Offset: 0x00019C64
		public long GetLastModified()
		{
			try
			{
				return this.LastModified();
			}
			catch (SmbException)
			{
			}
			return 0L;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001BA98 File Offset: 0x00019C98
		public InputStream GetInputStream()
		{
			return new SmbFileInputStream(this);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001BAB0 File Offset: 0x00019CB0
		public OutputStream GetOutputStream()
		{
			return new SmbFileOutputStream(this);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001BAC8 File Offset: 0x00019CC8
		private void ProcessAces(Ace[] aces, bool resolveSids)
		{
			string serverWithDfs = this.GetServerWithDfs();
			if (resolveSids)
			{
				Sid[] array = new Sid[aces.Length];
				for (int i = 0; i < aces.Length; i++)
				{
					array[i] = aces[i].Sid;
				}
				for (int j = 0; j < array.Length; j += 64)
				{
					int num = array.Length - j;
					bool flag = num > 64;
					if (flag)
					{
						num = 64;
					}
					Sid.ResolveSids(serverWithDfs, this.Auth, array, j, num);
				}
			}
			else
			{
				for (int i = 0; i < aces.Length; i++)
				{
					aces[i].Sid.OriginServer = serverWithDfs;
					aces[i].Sid.OriginAuth = this.Auth;
				}
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001BB94 File Offset: 0x00019D94
		public virtual Ace[] GetSecurity(bool resolveSids)
		{
			int num = this.Open0(1, SmbConstants.ReadControl, 0, this.IsDirectory() ? 1 : 0);
			NtTransQuerySecurityDesc request = new NtTransQuerySecurityDesc(num, 4);
			NtTransQuerySecurityDescResponse ntTransQuerySecurityDescResponse = new NtTransQuerySecurityDescResponse();
			try
			{
				this.Send(request, ntTransQuerySecurityDescResponse);
			}
			finally
			{
				this.Close(num, 0L);
			}
			Ace[] aces = ntTransQuerySecurityDescResponse.SecurityDescriptor.Aces;
			bool flag = aces != null;
			if (flag)
			{
				this.ProcessAces(aces, resolveSids);
			}
			return aces;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001BC1C File Offset: 0x00019E1C
		public virtual Ace[] GetShareSecurity(bool resolveSids)
		{
			string absolutePath = this.Url.AbsolutePath;
			this.ResolveDfs(null);
			string serverWithDfs = this.GetServerWithDfs();
			MsrpcShareGetInfo msrpcShareGetInfo = new MsrpcShareGetInfo(serverWithDfs, this.Tree.Share);
			DcerpcHandle handle = DcerpcHandle.GetHandle("ncacn_np:" + serverWithDfs + "[\\PIPE\\srvsvc]", this.Auth);
			Ace[] security;
			try
			{
				handle.Sendrecv(msrpcShareGetInfo);
				bool flag = msrpcShareGetInfo.Retval != 0;
				if (flag)
				{
					throw new SmbException(msrpcShareGetInfo.Retval, true);
				}
				security = msrpcShareGetInfo.GetSecurity();
				bool flag2 = security != null;
				if (flag2)
				{
					this.ProcessAces(security, resolveSids);
				}
			}
			finally
			{
				try
				{
					handle.Close();
				}
				catch (IOException ex)
				{
					bool flag3 = this.Log.Level >= 1;
					if (flag3)
					{
						Runtime.PrintStackTrace(ex, this.Log);
					}
				}
			}
			return security;
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001BD1C File Offset: 0x00019F1C
		public virtual Ace[] GetSecurity()
		{
			return this.GetSecurity(false);
		}

		// Token: 0x040002E8 RID: 744
		internal const int ORdonly = 1;

		// Token: 0x040002E9 RID: 745
		internal const int OWronly = 2;

		// Token: 0x040002EA RID: 746
		internal const int ORdwr = 3;

		// Token: 0x040002EB RID: 747
		internal const int OAppend = 4;

		// Token: 0x040002EC RID: 748
		internal const int OCreat = 16;

		// Token: 0x040002ED RID: 749
		internal const int OExcl = 32;

		// Token: 0x040002EE RID: 750
		internal const int OTrunc = 64;

		// Token: 0x040002EF RID: 751
		public const int FileNoShare = 0;

		// Token: 0x040002F0 RID: 752
		public const int FileShareRead = 1;

		// Token: 0x040002F1 RID: 753
		public const int FileShareWrite = 2;

		// Token: 0x040002F2 RID: 754
		public const int FileShareDelete = 4;

		// Token: 0x040002F3 RID: 755
		public const int AttrReadonly = 1;

		// Token: 0x040002F4 RID: 756
		public const int AttrHidden = 2;

		// Token: 0x040002F5 RID: 757
		public const int AttrSystem = 4;

		// Token: 0x040002F6 RID: 758
		public const int AttrVolume = 8;

		// Token: 0x040002F7 RID: 759
		public const int AttrDirectory = 16;

		// Token: 0x040002F8 RID: 760
		public const int AttrArchive = 32;

		// Token: 0x040002F9 RID: 761
		internal const int AttrCompressed = 2048;

		// Token: 0x040002FA RID: 762
		internal const int AttrNormal = 128;

		// Token: 0x040002FB RID: 763
		internal const int AttrTemporary = 256;

		// Token: 0x040002FC RID: 764
		internal const int AttrGetMask = 32767;

		// Token: 0x040002FD RID: 765
		internal const int AttrSetMask = 12455;

		// Token: 0x040002FE RID: 766
		internal const int DefaultAttrExpirationPeriod = 5000;

		// Token: 0x040002FF RID: 767
		internal static readonly int HashDot = ".".GetHashCode();

		// Token: 0x04000300 RID: 768
		internal static readonly int HashDotDot = "..".GetHashCode();

		// Token: 0x04000301 RID: 769
		internal static long AttrExpirationPeriod = Config.GetLong("jcifs.smb.client.attrExpirationPeriod", 5000L);

		// Token: 0x04000302 RID: 770
		internal static bool IgnoreCopyToException = Config.GetBoolean("jcifs.smb.client.ignoreCopyToException", true);

		// Token: 0x04000303 RID: 771
		public const int TypeFilesystem = 1;

		// Token: 0x04000304 RID: 772
		public const int TypeWorkgroup = 2;

		// Token: 0x04000305 RID: 773
		public const int TypeServer = 4;

		// Token: 0x04000306 RID: 774
		public const int TypeShare = 8;

		// Token: 0x04000307 RID: 775
		public const int TypeNamedPipe = 16;

		// Token: 0x04000308 RID: 776
		public const int TypePrinter = 32;

		// Token: 0x04000309 RID: 777
		public const int TypeComm = 64;

		// Token: 0x0400030A RID: 778
		private string _canon;

		// Token: 0x0400030B RID: 779
		private string _share;

		// Token: 0x0400030C RID: 780
		private long _createTime;

		// Token: 0x0400030D RID: 781
		private long _lastModified;

		// Token: 0x0400030E RID: 782
		private int _attributes;

		// Token: 0x0400030F RID: 783
		private long _attrExpiration;

		// Token: 0x04000310 RID: 784
		private long _size;

		// Token: 0x04000311 RID: 785
		private long _sizeExpiration;

		// Token: 0x04000312 RID: 786
		private bool _isExists;

		// Token: 0x04000313 RID: 787
		private int _shareAccess = 7;

		// Token: 0x04000314 RID: 788
		private bool _enableDfs = Config.GetBoolean("jcifs.smb.client.enabledfs", false);

		// Token: 0x04000315 RID: 789
		private SmbComBlankResponse _blankResp;

		// Token: 0x04000316 RID: 790
		private DfsReferral _dfsReferral;

		// Token: 0x04000317 RID: 791
		protected internal static Dfs Dfs = new Dfs();

		// Token: 0x04000318 RID: 792
		internal NtlmPasswordAuthentication Auth;

		// Token: 0x04000319 RID: 793
		internal SmbTree Tree;

		// Token: 0x0400031A RID: 794
		internal string Unc;

		// Token: 0x0400031B RID: 795
		internal int Fid;

		// Token: 0x0400031C RID: 796
		internal int Type;

		// Token: 0x0400031D RID: 797
		internal bool Opened;

		// Token: 0x0400031E RID: 798
		internal int TreeNum;

		// Token: 0x0400031F RID: 799
		internal UniAddress[] Addresses;

		// Token: 0x04000320 RID: 800
		internal int AddressIndex;

		// Token: 0x02000117 RID: 279
		internal class WriterThread : Thread
		{
			// Token: 0x0600081B RID: 2075 RVA: 0x0002B248 File Offset: 0x00029448
			public WriterThread(SmbFile enclosing) : base("JCIFS-WriterThread")
			{
				this._enclosing = enclosing;
				this.UseNtSmbs = this._enclosing.Tree.Session.transport.HasCapability(SmbConstants.CapNtSmbs);
				bool useNtSmbs = this.UseNtSmbs;
				if (useNtSmbs)
				{
					this.Reqx = new SmbComWriteAndX();
					this.Resp = new SmbComWriteAndXResponse();
				}
				else
				{
					this.Req = new SmbComWrite();
					this.Resp = new SmbComWriteResponse();
				}
				this.Ready = false;
			}

			// Token: 0x0600081C RID: 2076 RVA: 0x0002B2D4 File Offset: 0x000294D4
			internal virtual void Write(byte[] b, int n, SmbFile dest, long off)
			{
				lock (this)
				{
					this.B = b;
					this.N = n;
					this.Dest = dest;
					this.Off = off;
					this.Ready = false;
					Runtime.Notify(this);
				}
			}

			// Token: 0x0600081D RID: 2077 RVA: 0x0002B33C File Offset: 0x0002953C
			public override void Run()
			{
				lock (this)
				{
					try
					{
						for (;;)
						{
							Runtime.Notify(this);
							this.Ready = true;
							while (this.Ready)
							{
								Runtime.Wait(this);
							}
							bool flag2 = this.N == -1;
							if (flag2)
							{
								break;
							}
							bool useNtSmbs = this.UseNtSmbs;
							if (useNtSmbs)
							{
								this.Reqx.SetParam(this.Dest.Fid, this.Off, this.N, this.B, 0, this.N);
								this.Dest.Send(this.Reqx, this.Resp);
							}
							else
							{
								this.Req.SetParam(this.Dest.Fid, this.Off, this.N, this.B, 0, this.N);
								this.Dest.Send(this.Req, this.Resp);
							}
						}
						return;
					}
					catch (SmbException e)
					{
						this.E = e;
					}
					catch (Exception rootCause)
					{
						this.E = new SmbException("WriterThread", rootCause);
					}
					Runtime.Notify(this);
				}
			}

			// Token: 0x0400055F RID: 1375
			internal byte[] B;

			// Token: 0x04000560 RID: 1376
			internal int N;

			// Token: 0x04000561 RID: 1377
			internal long Off;

			// Token: 0x04000562 RID: 1378
			internal bool Ready;

			// Token: 0x04000563 RID: 1379
			internal SmbFile Dest;

			// Token: 0x04000564 RID: 1380
			internal SmbException E;

			// Token: 0x04000565 RID: 1381
			internal bool UseNtSmbs;

			// Token: 0x04000566 RID: 1382
			internal SmbComWriteAndX Reqx;

			// Token: 0x04000567 RID: 1383
			internal SmbComWrite Req;

			// Token: 0x04000568 RID: 1384
			internal ServerMessageBlock Resp;

			// Token: 0x04000569 RID: 1385
			private readonly SmbFile _enclosing;
		}
	}
}
