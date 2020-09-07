using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000246 RID: 582
	[Serializable]
	public class DNS_rr_SPF : DNS_rr
	{
		// Token: 0x06001534 RID: 5428 RVA: 0x00084643 File Offset: 0x00083643
		public DNS_rr_SPF(string name, string text, int ttl) : base(name, DNS_QType.SPF, ttl)
		{
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x0008465C File Offset: 0x0008365C
		public static DNS_rr_SPF Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string text = Dns_Client.ReadCharacterString(reply, ref offset);
			return new DNS_rr_SPF(name, text, ttl);
		}

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06001536 RID: 5430 RVA: 0x00084680 File Offset: 0x00083680
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x04000841 RID: 2113
		private string m_Text = "";
	}
}
