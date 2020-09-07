using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000256 RID: 598
	[Serializable]
	public class DNS_rr_TXT : DNS_rr
	{
		// Token: 0x0600156F RID: 5487 RVA: 0x000851D8 File Offset: 0x000841D8
		public DNS_rr_TXT(string name, string text, int ttl) : base(name, DNS_QType.TXT, ttl)
		{
			this.m_Text = text;
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x000851F8 File Offset: 0x000841F8
		public static DNS_rr_TXT Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string text = Dns_Client.ReadCharacterString(reply, ref offset);
			return new DNS_rr_TXT(name, text, ttl);
		}

		// Token: 0x170006FD RID: 1789
		// (get) Token: 0x06001571 RID: 5489 RVA: 0x0008521C File Offset: 0x0008421C
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x0400087A RID: 2170
		private string m_Text = "";
	}
}
