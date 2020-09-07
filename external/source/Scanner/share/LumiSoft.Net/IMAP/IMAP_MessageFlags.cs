using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200020A RID: 522
	[Obsolete("Use IMAP_t_MsgFlags instead.")]
	public enum IMAP_MessageFlags
	{
		// Token: 0x04000735 RID: 1845
		None,
		// Token: 0x04000736 RID: 1846
		Seen = 2,
		// Token: 0x04000737 RID: 1847
		Answered = 4,
		// Token: 0x04000738 RID: 1848
		Flagged = 8,
		// Token: 0x04000739 RID: 1849
		Deleted = 16,
		// Token: 0x0400073A RID: 1850
		Draft = 32,
		// Token: 0x0400073B RID: 1851
		Recent = 64
	}
}
