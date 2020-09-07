using System;

namespace LumiSoft.Net.SIP.UA
{
	// Token: 0x0200007F RID: 127
	[Obsolete("Use SIP stack instead.")]
	public enum SIP_UA_CallState
	{
		// Token: 0x04000174 RID: 372
		WaitingForStart,
		// Token: 0x04000175 RID: 373
		Calling,
		// Token: 0x04000176 RID: 374
		Ringing,
		// Token: 0x04000177 RID: 375
		Queued,
		// Token: 0x04000178 RID: 376
		WaitingToAccept,
		// Token: 0x04000179 RID: 377
		Active,
		// Token: 0x0400017A RID: 378
		Terminating,
		// Token: 0x0400017B RID: 379
		Terminated,
		// Token: 0x0400017C RID: 380
		Disposed
	}
}
