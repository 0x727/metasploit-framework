using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x0200025D RID: 605
	[Obsolete("Use DNS_Client.Cache instead.")]
	public class DnsCache
	{
		// Token: 0x06001596 RID: 5526 RVA: 0x00085E4C File Offset: 0x00084E4C
		static DnsCache()
		{
			DnsCache.m_pCache = new Hashtable();
		}

		// Token: 0x06001597 RID: 5527 RVA: 0x00085E6C File Offset: 0x00084E6C
		public static DnsServerResponse GetFromCache(string qname, int qtype)
		{
			try
			{
				bool flag = DnsCache.m_pCache.Contains(qname + qtype);
				if (flag)
				{
					DnsCacheEntry dnsCacheEntry = (DnsCacheEntry)DnsCache.m_pCache[qname + qtype];
					bool flag2 = dnsCacheEntry.Time.AddSeconds((double)DnsCache.m_CacheTime) > DateTime.Now;
					if (flag2)
					{
						return dnsCacheEntry.Answers;
					}
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x00085F04 File Offset: 0x00084F04
		public static void AddToCache(string qname, int qtype, DnsServerResponse answers)
		{
			bool flag = answers == null;
			if (!flag)
			{
				try
				{
					Hashtable pCache = DnsCache.m_pCache;
					lock (pCache)
					{
						bool flag3 = DnsCache.m_pCache.Contains(qname + qtype);
						if (flag3)
						{
							DnsCache.m_pCache.Remove(qname + qtype);
						}
						DnsCache.m_pCache.Add(qname + qtype, new DnsCacheEntry(answers, DateTime.Now));
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00085FC0 File Offset: 0x00084FC0
		public static void ClearCache()
		{
			Hashtable pCache = DnsCache.m_pCache;
			lock (pCache)
			{
				DnsCache.m_pCache.Clear();
			}
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x0008600C File Offset: 0x0008500C
		public static byte[] SerializeCache()
		{
			Hashtable pCache = DnsCache.m_pCache;
			byte[] result;
			lock (pCache)
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, DnsCache.m_pCache);
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x0008606C File Offset: 0x0008506C
		public static void DeSerializeCache(byte[] cacheData)
		{
			Hashtable pCache = DnsCache.m_pCache;
			lock (pCache)
			{
				MemoryStream serializationStream = new MemoryStream(cacheData);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				DnsCache.m_pCache = (Hashtable)binaryFormatter.Deserialize(serializationStream);
			}
		}

		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x0600159C RID: 5532 RVA: 0x000860C8 File Offset: 0x000850C8
		// (set) Token: 0x0600159D RID: 5533 RVA: 0x000860DF File Offset: 0x000850DF
		public static long CacheTime
		{
			get
			{
				return DnsCache.m_CacheTime;
			}
			set
			{
				DnsCache.m_CacheTime = value;
			}
		}

		// Token: 0x04000898 RID: 2200
		private static Hashtable m_pCache = null;

		// Token: 0x04000899 RID: 2201
		private static long m_CacheTime = 10000L;
	}
}
