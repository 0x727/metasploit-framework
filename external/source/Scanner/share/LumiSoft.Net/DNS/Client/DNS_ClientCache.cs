using System;
using System.Collections.Generic;
using System.Timers;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x02000257 RID: 599
	public class DNS_ClientCache
	{
		// Token: 0x06001572 RID: 5490 RVA: 0x00085234 File Offset: 0x00084234
		internal DNS_ClientCache()
		{
			this.m_pCache = new Dictionary<string, DNS_ClientCache.CacheEntry>();
			this.m_pTimerTimeout = new TimerEx(60000.0);
			this.m_pTimerTimeout.Elapsed += this.m_pTimerTimeout_Elapsed;
			this.m_pTimerTimeout.Start();
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x000852B0 File Offset: 0x000842B0
		internal void Dispose()
		{
			this.m_pCache = null;
			this.m_pTimerTimeout.Dispose();
			this.m_pTimerTimeout = null;
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x000852D0 File Offset: 0x000842D0
		private void m_pTimerTimeout_Elapsed(object sender, ElapsedEventArgs e)
		{
			Dictionary<string, DNS_ClientCache.CacheEntry> pCache = this.m_pCache;
			lock (pCache)
			{
				List<KeyValuePair<string, DNS_ClientCache.CacheEntry>> list = new List<KeyValuePair<string, DNS_ClientCache.CacheEntry>>();
				foreach (KeyValuePair<string, DNS_ClientCache.CacheEntry> item in this.m_pCache)
				{
					list.Add(item);
				}
				foreach (KeyValuePair<string, DNS_ClientCache.CacheEntry> keyValuePair in list)
				{
					bool flag2 = DateTime.Now > keyValuePair.Value.Expires;
					if (flag2)
					{
						this.m_pCache.Remove(keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x000853CC File Offset: 0x000843CC
		public DnsServerResponse GetFromCache(string qname, int qtype)
		{
			bool flag = qname == null;
			if (flag)
			{
				throw new ArgumentNullException("qname");
			}
			bool flag2 = qname == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'qname' value must be specified.", "qname");
			}
			DNS_ClientCache.CacheEntry cacheEntry = null;
			bool flag3 = this.m_pCache.TryGetValue(qname + qtype, out cacheEntry);
			DnsServerResponse result;
			if (flag3)
			{
				bool flag4 = DateTime.Now > cacheEntry.Expires;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = cacheEntry.Response;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06001576 RID: 5494 RVA: 0x00085460 File Offset: 0x00084460
		public void AddToCache(string qname, int qtype, DnsServerResponse response)
		{
			bool flag = qname == null;
			if (flag)
			{
				throw new ArgumentNullException("qname");
			}
			bool flag2 = qname == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'qname' value must be specified.", "qname");
			}
			bool flag3 = response == null;
			if (flag3)
			{
				throw new ArgumentNullException("response");
			}
			Dictionary<string, DNS_ClientCache.CacheEntry> pCache = this.m_pCache;
			lock (pCache)
			{
				bool flag5 = this.m_pCache.ContainsKey(qname + qtype);
				if (flag5)
				{
					this.m_pCache.Remove(qname + qtype);
				}
				bool flag6 = response.ResponseCode == DNS_RCode.NO_ERROR;
				if (flag6)
				{
					int num = this.m_MaxCacheTtl;
					foreach (DNS_rr dns_rr in response.AllAnswers)
					{
						bool flag7 = dns_rr.TTL < num;
						if (flag7)
						{
							num = dns_rr.TTL;
						}
					}
					this.m_pCache.Add(qname + qtype, new DNS_ClientCache.CacheEntry(response, DateTime.Now.AddSeconds((double)num)));
				}
				else
				{
					this.m_pCache.Add(qname + qtype, new DNS_ClientCache.CacheEntry(response, DateTime.Now.AddSeconds((double)this.m_MaxNegativeCacheTtl)));
				}
			}
		}

		// Token: 0x06001577 RID: 5495 RVA: 0x000855EC File Offset: 0x000845EC
		public void ClearCache()
		{
			Dictionary<string, DNS_ClientCache.CacheEntry> pCache = this.m_pCache;
			lock (pCache)
			{
				this.m_pCache.Clear();
			}
		}

		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06001578 RID: 5496 RVA: 0x00085638 File Offset: 0x00084638
		// (set) Token: 0x06001579 RID: 5497 RVA: 0x00085650 File Offset: 0x00084650
		public int MaxCacheTtl
		{
			get
			{
				return this.m_MaxCacheTtl;
			}
			set
			{
				this.m_MaxCacheTtl = value;
			}
		}

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x0600157A RID: 5498 RVA: 0x0008565C File Offset: 0x0008465C
		// (set) Token: 0x0600157B RID: 5499 RVA: 0x00085674 File Offset: 0x00084674
		public int MaxNegativeCacheTtl
		{
			get
			{
				return this.m_MaxNegativeCacheTtl;
			}
			set
			{
				this.m_MaxNegativeCacheTtl = value;
			}
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x0600157C RID: 5500 RVA: 0x00085680 File Offset: 0x00084680
		public int Count
		{
			get
			{
				return this.m_pCache.Count;
			}
		}

		// Token: 0x0400087B RID: 2171
		private Dictionary<string, DNS_ClientCache.CacheEntry> m_pCache = null;

		// Token: 0x0400087C RID: 2172
		private int m_MaxCacheTtl = 86400;

		// Token: 0x0400087D RID: 2173
		private int m_MaxNegativeCacheTtl = 900;

		// Token: 0x0400087E RID: 2174
		private TimerEx m_pTimerTimeout = null;

		// Token: 0x02000380 RID: 896
		private class CacheEntry
		{
			// Token: 0x06001BA2 RID: 7074 RVA: 0x000AAC8C File Offset: 0x000A9C8C
			public CacheEntry(DnsServerResponse response, DateTime expires)
			{
				bool flag = response == null;
				if (flag)
				{
					throw new ArgumentNullException("response");
				}
				this.m_pResponse = response;
				this.m_Expires = expires;
			}

			// Token: 0x17000892 RID: 2194
			// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x000AACCC File Offset: 0x000A9CCC
			public DnsServerResponse Response
			{
				get
				{
					return this.m_pResponse;
				}
			}

			// Token: 0x17000893 RID: 2195
			// (get) Token: 0x06001BA4 RID: 7076 RVA: 0x000AACE4 File Offset: 0x000A9CE4
			public DateTime Expires
			{
				get
				{
					return this.m_Expires;
				}
			}

			// Token: 0x04000CD8 RID: 3288
			private DnsServerResponse m_pResponse = null;

			// Token: 0x04000CD9 RID: 3289
			private DateTime m_Expires;
		}
	}
}
