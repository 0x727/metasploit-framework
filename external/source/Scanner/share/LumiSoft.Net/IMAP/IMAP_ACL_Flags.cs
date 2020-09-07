using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000209 RID: 521
	public enum IMAP_ACL_Flags
	{
		// Token: 0x04000729 RID: 1833
		None,
		// Token: 0x0400072A RID: 1834
		l,
		// Token: 0x0400072B RID: 1835
		r,
		// Token: 0x0400072C RID: 1836
		s = 4,
		// Token: 0x0400072D RID: 1837
		w = 8,
		// Token: 0x0400072E RID: 1838
		i = 16,
		// Token: 0x0400072F RID: 1839
		p = 32,
		// Token: 0x04000730 RID: 1840
		c = 64,
		// Token: 0x04000731 RID: 1841
		d = 128,
		// Token: 0x04000732 RID: 1842
		a = 256,
		// Token: 0x04000733 RID: 1843
		All = 65535
	}
}
