using System;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000B2 RID: 178
	[Flags]
	public enum SIP_ProxyMode
	{
		// Token: 0x040002E8 RID: 744
		Registrar = 1,
		// Token: 0x040002E9 RID: 745
		Presence = 2,
		// Token: 0x040002EA RID: 746
		Stateless = 4,
		// Token: 0x040002EB RID: 747
		Statefull = 8,
		// Token: 0x040002EC RID: 748
		B2BUA = 16
	}
}
