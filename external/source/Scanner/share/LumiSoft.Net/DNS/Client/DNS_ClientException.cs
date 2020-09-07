using System;

namespace LumiSoft.Net.DNS.Client
{
	// Token: 0x0200025A RID: 602
	public class DNS_ClientException : Exception
	{
		// Token: 0x06001591 RID: 5521 RVA: 0x00085DC8 File Offset: 0x00084DC8
		public DNS_ClientException(DNS_RCode rcode) : base("Dns error: " + rcode + ".")
		{
			this.m_RCode = rcode;
		}

		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x06001592 RID: 5522 RVA: 0x00085DF0 File Offset: 0x00084DF0
		public DNS_RCode ErrorCode
		{
			get
			{
				return this.m_RCode;
			}
		}

		// Token: 0x04000891 RID: 2193
		private DNS_RCode m_RCode;
	}
}
