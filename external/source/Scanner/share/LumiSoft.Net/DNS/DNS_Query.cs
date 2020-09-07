using System;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000247 RID: 583
	public class DNS_Query
	{
		// Token: 0x06001537 RID: 5431 RVA: 0x00084698 File Offset: 0x00083698
		public DNS_Query(DNS_QType qtype, string qname) : this(DNS_QClass.IN, qtype, qname)
		{
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x000846A8 File Offset: 0x000836A8
		public DNS_Query(DNS_QClass qclass, DNS_QType qtype, string qname)
		{
			bool flag = qname == null;
			if (flag)
			{
				throw new ArgumentNullException("qname");
			}
			this.m_QClass = qclass;
			this.m_QType = qtype;
			this.m_QName = qname;
		}

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x00084704 File Offset: 0x00083704
		public DNS_QClass QueryClass
		{
			get
			{
				return this.m_QClass;
			}
		}

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x0600153A RID: 5434 RVA: 0x0008471C File Offset: 0x0008371C
		public DNS_QType QueryType
		{
			get
			{
				return this.m_QType;
			}
		}

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x00084734 File Offset: 0x00083734
		public string QueryName
		{
			get
			{
				return this.m_QName;
			}
		}

		// Token: 0x04000842 RID: 2114
		private DNS_QClass m_QClass = DNS_QClass.IN;

		// Token: 0x04000843 RID: 2115
		private DNS_QType m_QType = DNS_QType.ANY;

		// Token: 0x04000844 RID: 2116
		private string m_QName = "";
	}
}
