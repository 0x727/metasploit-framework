using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000254 RID: 596
	[Serializable]
	public class DNS_rr_PTR : DNS_rr
	{
		// Token: 0x06001563 RID: 5475 RVA: 0x00084E48 File Offset: 0x00083E48
		public DNS_rr_PTR(string name, string domainName, int ttl) : base(name, DNS_QType.PTR, ttl)
		{
			this.m_DomainName = domainName;
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00084E68 File Offset: 0x00083E68
		public static DNS_rr_PTR Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string domainName = "";
			bool qname = Dns_Client.GetQName(reply, ref offset, ref domainName);
			if (qname)
			{
				return new DNS_rr_PTR(name, domainName, ttl);
			}
			throw new ArgumentException("Invalid PTR resource record data !");
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06001565 RID: 5477 RVA: 0x00084EA4 File Offset: 0x00083EA4
		public string DomainName
		{
			get
			{
				return this.m_DomainName;
			}
		}

		// Token: 0x04000872 RID: 2162
		private string m_DomainName = "";
	}
}
