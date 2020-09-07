using System;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x0200025C RID: 604
	[Serializable]
	internal struct DnsCacheEntry
	{
		// Token: 0x06001593 RID: 5523 RVA: 0x00085E08 File Offset: 0x00084E08
		public DnsCacheEntry(DnsServerResponse answers, DateTime addTime)
		{
			this.m_pResponse = answers;
			this.m_Time = addTime;
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06001594 RID: 5524 RVA: 0x00085E1C File Offset: 0x00084E1C
		public DnsServerResponse Answers
		{
			get
			{
				return this.m_pResponse;
			}
		}

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06001595 RID: 5525 RVA: 0x00085E34 File Offset: 0x00084E34
		public DateTime Time
		{
			get
			{
				return this.m_Time;
			}
		}

		// Token: 0x04000896 RID: 2198
		private DnsServerResponse m_pResponse;

		// Token: 0x04000897 RID: 2199
		private DateTime m_Time;
	}
}
