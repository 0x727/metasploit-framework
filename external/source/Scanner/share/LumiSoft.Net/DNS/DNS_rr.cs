using System;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000250 RID: 592
	public abstract class DNS_rr
	{
		// Token: 0x06001553 RID: 5459 RVA: 0x00084B8C File Offset: 0x00083B8C
		public DNS_rr(string name, DNS_QType recordType, int ttl)
		{
			this.m_Name = name;
			this.m_Type = recordType;
			this.m_TTL = ttl;
		}

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06001554 RID: 5460 RVA: 0x00084BC4 File Offset: 0x00083BC4
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170006EE RID: 1774
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x00084BDC File Offset: 0x00083BDC
		public DNS_QType RecordType
		{
			get
			{
				return this.m_Type;
			}
		}

		// Token: 0x170006EF RID: 1775
		// (get) Token: 0x06001556 RID: 5462 RVA: 0x00084BF4 File Offset: 0x00083BF4
		public int TTL
		{
			get
			{
				return this.m_TTL;
			}
		}

		// Token: 0x0400086A RID: 2154
		private string m_Name = "";

		// Token: 0x0400086B RID: 2155
		private DNS_QType m_Type = DNS_QType.A;

		// Token: 0x0400086C RID: 2156
		private int m_TTL = -1;
	}
}
