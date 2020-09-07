using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000091 RID: 145
	public enum SIP_TransactionState
	{
		// Token: 0x040001CF RID: 463
		WaitingToStart,
		// Token: 0x040001D0 RID: 464
		Calling,
		// Token: 0x040001D1 RID: 465
		Trying,
		// Token: 0x040001D2 RID: 466
		Proceeding,
		// Token: 0x040001D3 RID: 467
		Accpeted,
		// Token: 0x040001D4 RID: 468
		Completed,
		// Token: 0x040001D5 RID: 469
		Confirmed,
		// Token: 0x040001D6 RID: 470
		Terminated,
		// Token: 0x040001D7 RID: 471
		Disposed
	}
}
