using System;
using System.IO;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000071 RID: 113
	public class Dfs
	{
		// Token: 0x0600033B RID: 827 RVA: 0x0000D368 File Offset: 0x0000B568
		public virtual Hashtable GetTrustedDomains(NtlmPasswordAuthentication auth)
		{
			bool flag = Dfs.Disabled || auth.Domain == "?";
			Hashtable result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.Domains != null && Runtime.CurrentTimeMillis() > this.Domains.Expiration;
				if (flag2)
				{
					this.Domains = null;
				}
				bool flag3 = this.Domains != null;
				if (flag3)
				{
					result = this.Domains.Map;
				}
				else
				{
					try
					{
						UniAddress byName = UniAddress.GetByName(auth.Domain, true);
						SmbTransport smbTransport = SmbTransport.GetSmbTransport(byName, 0);
						Dfs.CacheEntry cacheEntry = new Dfs.CacheEntry(Dfs.Ttl * 10L);
						DfsReferral dfsReferral = smbTransport.GetDfsReferrals(auth, string.Empty, 0);
						bool flag4 = dfsReferral != null;
						if (flag4)
						{
							DfsReferral dfsReferral2 = dfsReferral;
							do
							{
								string key = dfsReferral.Server.ToLower();
								cacheEntry.Map.Put(key, new Hashtable());
								dfsReferral = dfsReferral.Next;
							}
							while (dfsReferral != dfsReferral2);
							this.Domains = cacheEntry;
							return this.Domains.Map;
						}
					}
					catch (IOException ex)
					{
						bool flag5 = Dfs.Log.Level >= 3;
						if (flag5)
						{
							Runtime.PrintStackTrace(ex, Dfs.Log);
						}
						bool flag6 = Dfs.StrictView && ex is SmbAuthException;
						if (flag6)
						{
							throw (SmbAuthException)ex;
						}
					}
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
		public virtual bool IsTrustedDomain(string domain, NtlmPasswordAuthentication auth)
		{
			Hashtable trustedDomains = this.GetTrustedDomains(auth);
			bool flag = trustedDomains == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				domain = domain.ToLower();
				result = (trustedDomains.Get(domain) != null);
			}
			return result;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000D524 File Offset: 0x0000B724
		public virtual SmbTransport GetDc(string domain, NtlmPasswordAuthentication auth)
		{
			bool disabled = Dfs.Disabled;
			SmbTransport result;
			if (disabled)
			{
				result = null;
			}
			else
			{
				try
				{
					UniAddress byName = UniAddress.GetByName(domain, true);
					SmbTransport smbTransport = SmbTransport.GetSmbTransport(byName, 0);
					DfsReferral dfsReferral = smbTransport.GetDfsReferrals(auth, "\\" + domain, 1);
					bool flag = dfsReferral != null;
					if (flag)
					{
						DfsReferral dfsReferral2 = dfsReferral;
						IOException ex = null;
						do
						{
							try
							{
								byName = UniAddress.GetByName(dfsReferral.Server);
								return SmbTransport.GetSmbTransport(byName, 0);
							}
							catch (IOException ex2)
							{
								ex = ex2;
							}
							dfsReferral = dfsReferral.Next;
						}
						while (dfsReferral != dfsReferral2);
						throw ex;
					}
				}
				catch (IOException ex3)
				{
					bool flag2 = Dfs.Log.Level >= 3;
					if (flag2)
					{
						Runtime.PrintStackTrace(ex3, Dfs.Log);
					}
					bool flag3 = Dfs.StrictView && ex3 is SmbAuthException;
					if (flag3)
					{
						throw (SmbAuthException)ex3;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000D630 File Offset: 0x0000B830
		public virtual DfsReferral GetReferral(SmbTransport trans, string domain, string root, string path, NtlmPasswordAuthentication auth)
		{
			bool disabled = Dfs.Disabled;
			DfsReferral result;
			if (disabled)
			{
				result = null;
			}
			else
			{
				try
				{
					string text = "\\" + domain + "\\" + root;
					bool flag = path != null;
					if (flag)
					{
						text += path;
					}
					DfsReferral dfsReferrals = trans.GetDfsReferrals(auth, text, 0);
					bool flag2 = dfsReferrals != null;
					if (flag2)
					{
						return dfsReferrals;
					}
				}
				catch (IOException ex)
				{
					bool flag3 = Dfs.Log.Level >= 4;
					if (flag3)
					{
						Runtime.PrintStackTrace(ex, Dfs.Log);
					}
					bool flag4 = Dfs.StrictView && ex is SmbAuthException;
					if (flag4)
					{
						throw (SmbAuthException)ex;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000D6FC File Offset: 0x0000B8FC
		public virtual DfsReferral Resolve(string domain, string root, string path, NtlmPasswordAuthentication auth)
		{
			DfsReferral result;
			lock (this)
			{
				DfsReferral dfsReferral = null;
				long num = Runtime.CurrentTimeMillis();
				bool flag2 = Dfs.Disabled || root.Equals("IPC$");
				if (flag2)
				{
					result = null;
				}
				else
				{
					Hashtable trustedDomains = this.GetTrustedDomains(auth);
					bool flag3 = trustedDomains != null;
					if (flag3)
					{
						domain = domain.ToLower();
						Hashtable hashtable = (Hashtable)trustedDomains.Get(domain);
						bool flag4 = hashtable != null;
						if (flag4)
						{
							SmbTransport smbTransport = null;
							root = root.ToLower();
							Dfs.CacheEntry cacheEntry = (Dfs.CacheEntry)hashtable.Get(root);
							bool flag5 = cacheEntry != null && num > cacheEntry.Expiration;
							if (flag5)
							{
								hashtable.Remove(root);
								cacheEntry = null;
							}
							bool flag6 = cacheEntry == null;
							if (flag6)
							{
								bool flag7 = (smbTransport = this.GetDc(domain, auth)) == null;
								if (flag7)
								{
									return null;
								}
								dfsReferral = this.GetReferral(smbTransport, domain, root, path, auth);
								bool flag8 = dfsReferral != null;
								if (flag8)
								{
									int num2 = 1 + domain.Length + 1 + root.Length;
									cacheEntry = new Dfs.CacheEntry(0L);
									DfsReferral dfsReferral2 = dfsReferral;
									do
									{
										bool flag9 = path == null;
										if (flag9)
										{
											dfsReferral2.Key = "\\";
										}
										dfsReferral2.PathConsumed -= num2;
										dfsReferral2 = dfsReferral2.Next;
									}
									while (dfsReferral2 != dfsReferral);
									bool flag10 = dfsReferral.Key != null;
									if (flag10)
									{
										cacheEntry.Map.Put(dfsReferral.Key, dfsReferral);
									}
									hashtable.Put(root, cacheEntry);
								}
								else
								{
									bool flag11 = path == null;
									if (flag11)
									{
										hashtable.Put(root, Dfs.FalseEntry);
									}
								}
							}
							else
							{
								bool flag12 = cacheEntry == Dfs.FalseEntry;
								if (flag12)
								{
									cacheEntry = null;
								}
							}
							bool flag13 = cacheEntry != null;
							if (flag13)
							{
								string text = "\\";
								dfsReferral = (DfsReferral)cacheEntry.Map.Get(text);
								bool flag14 = dfsReferral != null && num > dfsReferral.Expiration;
								if (flag14)
								{
									cacheEntry.Map.Remove(text);
									dfsReferral = null;
								}
								bool flag15 = dfsReferral == null;
								if (flag15)
								{
									bool flag16 = smbTransport == null;
									if (flag16)
									{
										bool flag17 = (smbTransport = this.GetDc(domain, auth)) == null;
										if (flag17)
										{
											return null;
										}
									}
									dfsReferral = this.GetReferral(smbTransport, domain, root, path, auth);
									bool flag18 = dfsReferral != null;
									if (flag18)
									{
										dfsReferral.PathConsumed -= 1 + domain.Length + 1 + root.Length;
										dfsReferral.Link = text;
										cacheEntry.Map.Put(text, dfsReferral);
									}
								}
							}
						}
					}
					bool flag19 = dfsReferral == null && path != null;
					if (flag19)
					{
						bool flag20 = this.Referrals != null && num > this.Referrals.Expiration;
						if (flag20)
						{
							this.Referrals = null;
						}
						bool flag21 = this.Referrals == null;
						if (flag21)
						{
							this.Referrals = new Dfs.CacheEntry(0L);
						}
						string text2 = "\\" + domain + "\\" + root;
						bool flag22 = !path.Equals("\\");
						if (flag22)
						{
							text2 += path;
						}
						text2 = text2.ToLower();
						foreach (object obj in this.Referrals.Map.Keys)
						{
							string text3 = (string)obj;
							int length = text3.Length;
							bool flag23 = false;
							bool flag24 = length == text2.Length;
							if (flag24)
							{
								flag23 = text3.Equals(text2);
							}
							else
							{
								bool flag25 = length < text2.Length;
								if (flag25)
								{
									flag23 = (text3.RegionMatches(false, 0, text2, 0, length) && text2[length] == '\\');
								}
							}
							bool flag26 = flag23;
							if (flag26)
							{
								dfsReferral = (DfsReferral)this.Referrals.Map.Get(text3);
							}
						}
					}
					result = dfsReferral;
				}
			}
			return result;
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0000DB60 File Offset: 0x0000BD60
		internal virtual void Insert(string path, DfsReferral dr)
		{
			lock (this)
			{
				bool disabled = Dfs.Disabled;
				if (!disabled)
				{
					int num = path.IndexOf('\\', 1);
					int endIndex = path.IndexOf('\\', num + 1);
					string text = Runtime.Substring(path, 1, num);
					string text2 = Runtime.Substring(path, num + 1, endIndex);
					string text3 = Runtime.Substring(path, 0, dr.PathConsumed).ToLower();
					int num2 = text3.Length;
					while (num2 > 1 && text3[num2 - 1] == '\\')
					{
						num2--;
					}
					bool flag2 = num2 < text3.Length;
					if (flag2)
					{
						text3 = Runtime.Substring(text3, 0, num2);
					}
					dr.PathConsumed -= 1 + text.Length + 1 + text2.Length;
					bool flag3 = this.Referrals != null && Runtime.CurrentTimeMillis() + 10000L > this.Referrals.Expiration;
					if (flag3)
					{
						this.Referrals = null;
					}
					bool flag4 = this.Referrals == null;
					if (flag4)
					{
						this.Referrals = new Dfs.CacheEntry(0L);
					}
					this.Referrals.Map.Put(text3, dr);
				}
			}
		}

		// Token: 0x040000BF RID: 191
		internal static LogStream Log = LogStream.GetInstance();

		// Token: 0x040000C0 RID: 192
		internal static readonly bool StrictView = Config.GetBoolean("jcifs.smb.client.dfs.strictView", false);

		// Token: 0x040000C1 RID: 193
		internal static readonly long Ttl = Config.GetLong("jcifs.smb.client.dfs.ttl", 300L);

		// Token: 0x040000C2 RID: 194
		internal static readonly bool Disabled = Config.GetBoolean("jcifs.smb.client.dfs.disabled", false);

		// Token: 0x040000C3 RID: 195
		internal static Dfs.CacheEntry FalseEntry = new Dfs.CacheEntry(0L);

		// Token: 0x040000C4 RID: 196
		internal Dfs.CacheEntry Domains;

		// Token: 0x040000C5 RID: 197
		internal Dfs.CacheEntry Referrals;

		// Token: 0x02000115 RID: 277
		internal class CacheEntry
		{
			// Token: 0x06000812 RID: 2066 RVA: 0x0002B0D4 File Offset: 0x000292D4
			internal CacheEntry(long ttl)
			{
				bool flag = ttl == 0L;
				if (flag)
				{
					ttl = Dfs.Ttl;
				}
				this.Expiration = Runtime.CurrentTimeMillis() + ttl * 1000L;
				this.Map = new Hashtable();
			}

			// Token: 0x04000557 RID: 1367
			internal long Expiration;

			// Token: 0x04000558 RID: 1368
			internal Hashtable Map;
		}
	}
}
