using System;
using System.Net;

namespace LumiSoft.Net.DNS
{
	// Token: 0x0200024B RID: 587
	public class DNS_rr_AAAA : DNS_rr
	{
		// Token: 0x0600153C RID: 5436 RVA: 0x0008474C File Offset: 0x0008374C
		public DNS_rr_AAAA(string name, IPAddress ip, int ttl) : base(name, DNS_QType.AAAA, ttl)
		{
			this.m_IP = ip;
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00084768 File Offset: 0x00083768
		public static DNS_rr_AAAA Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			byte[] array = new byte[rdLength];
			Array.Copy(reply, offset, array, 0, rdLength);
			offset += rdLength;
			return new DNS_rr_AAAA(name, new IPAddress(array), ttl);
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x0600153E RID: 5438 RVA: 0x000847A4 File Offset: 0x000837A4
		public IPAddress IP
		{
			get
			{
				return this.m_IP;
			}
		}

		// Token: 0x0400085D RID: 2141
		private IPAddress m_IP = null;
	}
}
