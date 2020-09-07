using System;

namespace LumiSoft.Net.SMTP
{
	// Token: 0x02000134 RID: 308
	[Flags]
	public enum SMTP_DSN_Notify
	{
		// Token: 0x0400050F RID: 1295
		NotSpecified = 0,
		// Token: 0x04000510 RID: 1296
		Never = 255,
		// Token: 0x04000511 RID: 1297
		Success = 2,
		// Token: 0x04000512 RID: 1298
		Failure = 4,
		// Token: 0x04000513 RID: 1299
		Delay = 8
	}
}
