using System;
using System.Net;

namespace LumiSoft.Net.DNS
{
	// Token: 0x0200024E RID: 590
	[Serializable]
	public class DNS_rr_A : DNS_rr
	{
		// Token: 0x0600154D RID: 5453 RVA: 0x00084AA8 File Offset: 0x00083AA8
		public DNS_rr_A(string name, IPAddress ip, int ttl) : base(name, DNS_QType.A, ttl)
		{
			this.m_IP = ip;
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00084AC4 File Offset: 0x00083AC4
		public static DNS_rr_A Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			byte[] array = new byte[rdLength];
			Array.Copy(reply, offset, array, 0, rdLength);
			offset += rdLength;
			return new DNS_rr_A(name, new IPAddress(array), ttl);
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x00084B00 File Offset: 0x00083B00
		public IPAddress IP
		{
			get
			{
				return this.m_IP;
			}
		}

		// Token: 0x04000868 RID: 2152
		private IPAddress m_IP = null;
	}
}
