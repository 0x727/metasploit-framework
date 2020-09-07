using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x0200015C RID: 348
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public enum ContentTransferEncoding_enum
	{
		// Token: 0x040005E4 RID: 1508
		_7bit = 1,
		// Token: 0x040005E5 RID: 1509
		_8bit,
		// Token: 0x040005E6 RID: 1510
		Binary,
		// Token: 0x040005E7 RID: 1511
		QuotedPrintable,
		// Token: 0x040005E8 RID: 1512
		Base64,
		// Token: 0x040005E9 RID: 1513
		NotSpecified = 30,
		// Token: 0x040005EA RID: 1514
		Unknown = 40
	}
}
