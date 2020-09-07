using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000253 RID: 595
	[Serializable]
	public class DNS_rr_NS : DNS_rr
	{
		// Token: 0x06001560 RID: 5472 RVA: 0x00084DD4 File Offset: 0x00083DD4
		public DNS_rr_NS(string name, string nameServer, int ttl) : base(name, DNS_QType.NS, ttl)
		{
			this.m_NameServer = nameServer;
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x00084DF4 File Offset: 0x00083DF4
		public static DNS_rr_NS Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string nameServer = "";
			bool qname = Dns_Client.GetQName(reply, ref offset, ref nameServer);
			if (qname)
			{
				return new DNS_rr_NS(name, nameServer, ttl);
			}
			throw new ArgumentException("Invalid NS resource record data !");
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06001562 RID: 5474 RVA: 0x00084E30 File Offset: 0x00083E30
		public string NameServer
		{
			get
			{
				return this.m_NameServer;
			}
		}

		// Token: 0x04000871 RID: 2161
		private string m_NameServer = "";
	}
}
