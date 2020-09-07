using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x0200024F RID: 591
	[Serializable]
	public class DNS_rr_CNAME : DNS_rr
	{
		// Token: 0x06001550 RID: 5456 RVA: 0x00084B18 File Offset: 0x00083B18
		public DNS_rr_CNAME(string name, string alias, int ttl) : base(name, DNS_QType.CNAME, ttl)
		{
			this.m_Alias = alias;
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x00084B38 File Offset: 0x00083B38
		public static DNS_rr_CNAME Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string alias = "";
			bool qname = Dns_Client.GetQName(reply, ref offset, ref alias);
			if (qname)
			{
				return new DNS_rr_CNAME(name, alias, ttl);
			}
			throw new ArgumentException("Invalid CNAME resource record data !");
		}

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06001552 RID: 5458 RVA: 0x00084B74 File Offset: 0x00083B74
		public string Alias
		{
			get
			{
				return this.m_Alias;
			}
		}

		// Token: 0x04000869 RID: 2153
		private string m_Alias = "";
	}
}
