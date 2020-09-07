using System;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000249 RID: 585
	public enum DNS_QType
	{
		// Token: 0x04000849 RID: 2121
		A = 1,
		// Token: 0x0400084A RID: 2122
		NS,
		// Token: 0x0400084B RID: 2123
		CNAME = 5,
		// Token: 0x0400084C RID: 2124
		SOA,
		// Token: 0x0400084D RID: 2125
		PTR = 12,
		// Token: 0x0400084E RID: 2126
		HINFO,
		// Token: 0x0400084F RID: 2127
		MX = 15,
		// Token: 0x04000850 RID: 2128
		TXT,
		// Token: 0x04000851 RID: 2129
		AAAA = 28,
		// Token: 0x04000852 RID: 2130
		SRV = 33,
		// Token: 0x04000853 RID: 2131
		NAPTR = 35,
		// Token: 0x04000854 RID: 2132
		SPF = 99,
		// Token: 0x04000855 RID: 2133
		ANY = 255
	}
}
