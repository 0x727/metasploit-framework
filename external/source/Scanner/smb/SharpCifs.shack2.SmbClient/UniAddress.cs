using System;
using System.IO;
using System.Linq;
using System.Net;
using SharpCifs.Netbios;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs
{
	// Token: 0x02000004 RID: 4
	public class UniAddress
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002844 File Offset: 0x00000A44
		static UniAddress()
		{
			string property = Config.GetProperty("jcifs.resolveOrder");
			IPAddress winsAddress = NbtAddress.GetWinsAddress();
			try
			{
				UniAddress._baddr = Config.GetInetAddress("jcifs.netbios.baddr", Extensions.GetAddressByName("255.255.255.255"));
			}
			catch (UnknownHostException)
			{
			}
			bool flag = string.IsNullOrEmpty(property);
			if (flag)
			{
				bool flag2 = winsAddress == null;
				if (flag2)
				{
					UniAddress._resolveOrder = new int[3];
					UniAddress._resolveOrder[0] = 3;
					UniAddress._resolveOrder[1] = 2;
					UniAddress._resolveOrder[2] = 1;
				}
				else
				{
					UniAddress._resolveOrder = new int[4];
					UniAddress._resolveOrder[0] = 3;
					UniAddress._resolveOrder[1] = 0;
					UniAddress._resolveOrder[2] = 2;
					UniAddress._resolveOrder[3] = 1;
				}
			}
			else
			{
				int[] array = new int[4];
				StringTokenizer stringTokenizer = new StringTokenizer(property, ",");
				int num = 0;
				while (stringTokenizer.HasMoreTokens())
				{
					string text = stringTokenizer.NextToken().Trim();
					bool flag3 = Runtime.EqualsIgnoreCase(text, "LMHOSTS");
					if (flag3)
					{
						array[num++] = 3;
					}
					else
					{
						bool flag4 = Runtime.EqualsIgnoreCase(text, "WINS");
						if (flag4)
						{
							bool flag5 = winsAddress == null;
							if (flag5)
							{
								bool flag6 = UniAddress._log.Level > 1;
								if (flag6)
								{
									UniAddress._log.WriteLine("UniAddress resolveOrder specifies WINS however the jcifs.netbios.wins property has not been set");
								}
							}
							else
							{
								array[num++] = 0;
							}
						}
						else
						{
							bool flag7 = Runtime.EqualsIgnoreCase(text, "BCAST");
							if (flag7)
							{
								array[num++] = 1;
							}
							else
							{
								bool flag8 = Runtime.EqualsIgnoreCase(text, "DNS");
								if (flag8)
								{
									array[num++] = 2;
								}
								else
								{
									bool flag9 = UniAddress._log.Level > 1;
									if (flag9)
									{
										UniAddress._log.WriteLine("unknown resolver method: " + text);
									}
								}
							}
						}
					}
				}
				UniAddress._resolveOrder = new int[num];
				Array.Copy(array, 0, UniAddress._resolveOrder, 0, num);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002A58 File Offset: 0x00000C58
		internal static NbtAddress[] LookupServerOrWorkgroup(string name, IPAddress svr)
		{
			UniAddress.Sem sem = new UniAddress.Sem(2);
			int type = NbtAddress.IsWins(svr) ? 27 : 29;
			UniAddress.QueryThread queryThread = new UniAddress.QueryThread(sem, name, type, null, svr);
			UniAddress.QueryThread queryThread2 = new UniAddress.QueryThread(sem, name, 32, null, svr);
			queryThread.SetDaemon(true);
			queryThread2.SetDaemon(true);
			try
			{
				UniAddress.Sem obj = sem;
				lock (obj)
				{
					queryThread.Start();
					queryThread2.Start();
					while (sem.Count > 0 && queryThread.Ans == null && queryThread2.Ans == null)
					{
						Runtime.Wait(sem);
					}
				}
			}
			catch (Exception)
			{
				throw new UnknownHostException(name);
			}
			bool flag2 = queryThread.Ans != null;
			NbtAddress[] ans;
			if (flag2)
			{
				ans = queryThread.Ans;
			}
			else
			{
				bool flag3 = queryThread2.Ans != null;
				if (!flag3)
				{
					throw queryThread.Uhe;
				}
				ans = queryThread2.Ans;
			}
			return ans;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002B68 File Offset: 0x00000D68
		public static UniAddress GetByName(string hostname)
		{
			return UniAddress.GetByName(hostname, false);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002B84 File Offset: 0x00000D84
		internal static bool IsDotQuadIp(string hostname)
		{
			bool flag = char.IsDigit(hostname[0]);
			if (flag)
			{
				int num2;
				int num = num2 = 0;
				int length = hostname.Length;
				char[] array = hostname.ToCharArray();
				while (num2 < length && char.IsDigit(array[num2++]))
				{
					bool flag2 = num2 == length && num == 3;
					if (flag2)
					{
						return true;
					}
					bool flag3 = num2 < length && array[num2] == '.';
					if (flag3)
					{
						num++;
						num2++;
					}
				}
			}
			return false;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002C14 File Offset: 0x00000E14
		internal static bool IsAllDigits(string hostname)
		{
			for (int i = 0; i < hostname.Length; i++)
			{
				bool flag = !char.IsDigit(hostname[i]);
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002C58 File Offset: 0x00000E58
		public static UniAddress GetByName(string hostname, bool possibleNtDomainOrWorkgroup)
		{
			UniAddress[] allByName = UniAddress.GetAllByName(hostname, possibleNtDomainOrWorkgroup);
			return allByName[0];
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002C78 File Offset: 0x00000E78
		public static UniAddress[] GetAllByName(string hostname, bool possibleNtDomainOrWorkgroup)
		{
			bool flag = string.IsNullOrEmpty(hostname);
			if (flag)
			{
				throw new UnknownHostException();
			}
			bool flag2 = UniAddress.IsDotQuadIp(hostname);
			if (!flag2)
			{
				int i = 0;
				while (i < UniAddress._resolveOrder.Length)
				{
					try
					{
						object addr;
						switch (UniAddress._resolveOrder[i])
						{
						case 0:
						{
							bool flag3 = hostname == NbtAddress.MasterBrowserName || hostname.Length > 15;
							if (flag3)
							{
								goto IL_205;
							}
							if (possibleNtDomainOrWorkgroup)
							{
								addr = UniAddress.LookupServerOrWorkgroup(hostname, NbtAddress.GetWinsAddress());
							}
							else
							{
								addr = NbtAddress.GetByName(hostname, 32, null, NbtAddress.GetWinsAddress());
							}
							break;
						}
						case 1:
						{
							bool flag4 = hostname.Length > 15;
							if (flag4)
							{
								goto IL_205;
							}
							try
							{
								if (possibleNtDomainOrWorkgroup)
								{
									NbtAddress[] array = UniAddress.LookupServerOrWorkgroup(hostname, UniAddress._baddr);
									UniAddress[] array2 = new UniAddress[array.Length];
									for (int j = 0; j < array.Length; j++)
									{
										array2[j] = new UniAddress(array[j]);
									}
									return array2;
								}
								addr = NbtAddress.GetByName(hostname, 32, null, UniAddress._baddr);
							}
							catch (Exception ex)
							{
								bool flag5 = i == UniAddress._resolveOrder.Length - 1;
								if (flag5)
								{
									throw ex;
								}
								goto IL_205;
							}
							break;
						}
						case 2:
						{
							bool flag6 = UniAddress.IsAllDigits(hostname);
							if (flag6)
							{
								throw new UnknownHostException(hostname);
							}
							IPAddress[] addressesByName = Extensions.GetAddressesByName(hostname);
							bool flag7 = addressesByName == null || addressesByName.Length == 0;
							if (flag7)
							{
								goto IL_205;
							}
							return (from iaddr in addressesByName
							select new UniAddress(iaddr)).ToArray<UniAddress>();
						}
						case 3:
						{
							// modify 0901
							//bool flag8 = (addr = Lmhosts.GetByName(hostname)) == null;
							//if (flag8)
							//{
							//	goto IL_205;
							//}
							addr = "";
							// end modify 0901
							break;
						}
						default:
							throw new UnknownHostException(hostname);
						}
						return new UniAddress[]
						{
							new UniAddress(addr)
						};
					}
					catch (IOException)
					{
					}
					IL_205:
					i++;
					continue;
					goto IL_205;
				}
				throw new UnknownHostException(hostname);
			}
			return new UniAddress[]
			{
				new UniAddress(NbtAddress.GetByName(hostname))
			};
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002EE0 File Offset: 0x000010E0
		public UniAddress(object addr)
		{
			bool flag = addr == null;
			if (flag)
			{
				throw new ArgumentException();
			}
			this.Addr = addr;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002F0C File Offset: 0x0000110C
		public override int GetHashCode()
		{
			return this.Addr.GetHashCode();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002F2C File Offset: 0x0000112C
		public override bool Equals(object obj)
		{
			return obj is UniAddress && this.Addr.Equals(((UniAddress)obj).Addr);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002F60 File Offset: 0x00001160
		public virtual string FirstCalledName()
		{
			bool flag = this.Addr is NbtAddress;
			string result;
			if (flag)
			{
				result = ((NbtAddress)this.Addr).FirstCalledName();
			}
			else
			{
				this.CalledName = ((IPAddress)this.Addr).GetHostAddress();
				bool flag2 = UniAddress.IsDotQuadIp(this.CalledName);
				if (flag2)
				{
					this.CalledName = NbtAddress.SmbserverName;
				}
				else
				{
					int num = this.CalledName.IndexOf('.');
					bool flag3 = num > 1 && num < 15;
					if (flag3)
					{
						this.CalledName = Runtime.Substring(this.CalledName, 0, num).ToUpper();
					}
					else
					{
						bool flag4 = this.CalledName.Length > 15;
						if (flag4)
						{
							this.CalledName = NbtAddress.SmbserverName;
						}
						else
						{
							this.CalledName = this.CalledName.ToUpper();
						}
					}
				}
				result = this.CalledName;
			}
			return result;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000304C File Offset: 0x0000124C
		public virtual string NextCalledName()
		{
			bool flag = this.Addr is NbtAddress;
			string result;
			if (flag)
			{
				result = ((NbtAddress)this.Addr).NextCalledName();
			}
			else
			{
				bool flag2 = this.CalledName != NbtAddress.SmbserverName;
				if (flag2)
				{
					this.CalledName = NbtAddress.SmbserverName;
					result = this.CalledName;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000030B0 File Offset: 0x000012B0
		public virtual object GetAddress()
		{
			return this.Addr;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000030C8 File Offset: 0x000012C8
		public virtual string GetHostName()
		{
			bool flag = this.Addr is NbtAddress;
			string result;
			if (flag)
			{
				result = ((NbtAddress)this.Addr).GetHostName();
			}
			else
			{
				result = ((IPAddress)this.Addr).GetHostAddress();
			}
			return result;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003110 File Offset: 0x00001310
		public virtual string GetHostAddress()
		{
			bool flag = this.Addr is NbtAddress;
			string hostAddress;
			if (flag)
			{
				hostAddress = ((NbtAddress)this.Addr).GetHostAddress();
			}
			else
			{
				hostAddress = ((IPAddress)this.Addr).GetHostAddress();
			}
			return hostAddress;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003158 File Offset: 0x00001358
		public virtual IPAddress GetHostIpAddress()
		{
			return (IPAddress)this.Addr;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003178 File Offset: 0x00001378
		public override string ToString()
		{
			return this.Addr.ToString();
		}

		// Token: 0x04000008 RID: 8
		private const int ResolverWins = 0;

		// Token: 0x04000009 RID: 9
		private const int ResolverBcast = 1;

		// Token: 0x0400000A RID: 10
		private const int ResolverDns = 2;

		// Token: 0x0400000B RID: 11
		private const int ResolverLmhosts = 3;

		// Token: 0x0400000C RID: 12
		private static int[] _resolveOrder;

		// Token: 0x0400000D RID: 13
		private static IPAddress _baddr;

		// Token: 0x0400000E RID: 14
		private static LogStream _log = LogStream.GetInstance();

		// Token: 0x0400000F RID: 15
		internal object Addr;

		// Token: 0x04000010 RID: 16
		internal string CalledName;

		// Token: 0x02000106 RID: 262
		internal class Sem
		{
			// Token: 0x060007F0 RID: 2032 RVA: 0x0002AE23 File Offset: 0x00029023
			internal Sem(int count)
			{
				this.Count = count;
			}

			// Token: 0x0400053D RID: 1341
			internal int Count;
		}

		// Token: 0x02000107 RID: 263
		internal class QueryThread : Thread
		{
			// Token: 0x060007F1 RID: 2033 RVA: 0x0002AE34 File Offset: 0x00029034
			internal QueryThread(UniAddress.Sem sem, string host, int type, string scope, IPAddress svr) : base("JCIFS-QueryThread: " + host)
			{
				this.Sem = sem;
				this.Host = host;
				this.Type = type;
				this.Scope = scope;
				this.Svr = svr;
			}

			// Token: 0x060007F2 RID: 2034 RVA: 0x0002AE70 File Offset: 0x00029070
			public override void Run()
			{
				try
				{
					this.Ans = NbtAddress.GetAllByName(this.Host, this.Type, this.Scope, this.Svr);
				}
				catch (UnknownHostException uhe)
				{
					this.Uhe = uhe;
				}
				catch (Exception ex)
				{
					this.Uhe = new UnknownHostException(ex.Message);
				}
				finally
				{
					UniAddress.Sem sem = this.Sem;
					lock (sem)
					{
						this.Sem.Count--;
						Runtime.Notify(this.Sem);
					}
				}
			}

			// Token: 0x0400053E RID: 1342
			internal UniAddress.Sem Sem;

			// Token: 0x0400053F RID: 1343
			internal string Host;

			// Token: 0x04000540 RID: 1344
			internal string Scope;

			// Token: 0x04000541 RID: 1345
			internal int Type;

			// Token: 0x04000542 RID: 1346
			internal NbtAddress[] Ans;

			// Token: 0x04000543 RID: 1347
			internal IPAddress Svr;

			// Token: 0x04000544 RID: 1348
			internal UnknownHostException Uhe;
		}
	}
}
