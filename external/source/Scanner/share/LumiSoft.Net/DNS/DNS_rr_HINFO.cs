using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000251 RID: 593
	public class DNS_rr_HINFO : DNS_rr
	{
		// Token: 0x06001557 RID: 5463 RVA: 0x00084C0C File Offset: 0x00083C0C
		public DNS_rr_HINFO(string name, string cpu, string os, int ttl) : base(name, DNS_QType.HINFO, ttl)
		{
			this.m_CPU = cpu;
			this.m_OS = os;
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x00084C40 File Offset: 0x00083C40
		public static DNS_rr_HINFO Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string cpu = Dns_Client.ReadCharacterString(reply, ref offset);
			string os = Dns_Client.ReadCharacterString(reply, ref offset);
			return new DNS_rr_HINFO(name, cpu, os, ttl);
		}

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x00084C6C File Offset: 0x00083C6C
		public string CPU
		{
			get
			{
				return this.m_CPU;
			}
		}

		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x0600155A RID: 5466 RVA: 0x00084C84 File Offset: 0x00083C84
		public string OS
		{
			get
			{
				return this.m_OS;
			}
		}

		// Token: 0x0400086D RID: 2157
		private string m_CPU = "";

		// Token: 0x0400086E RID: 2158
		private string m_OS = "";
	}
}
