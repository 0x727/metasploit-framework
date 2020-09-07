using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000164 RID: 356
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public enum MediaType_enum
	{
		// Token: 0x040005F8 RID: 1528
		Text = 1,
		// Token: 0x040005F9 RID: 1529
		Text_plain = 3,
		// Token: 0x040005FA RID: 1530
		Text_html = 5,
		// Token: 0x040005FB RID: 1531
		Text_xml = 9,
		// Token: 0x040005FC RID: 1532
		Text_rtf = 17,
		// Token: 0x040005FD RID: 1533
		Image = 32,
		// Token: 0x040005FE RID: 1534
		Image_gif = 96,
		// Token: 0x040005FF RID: 1535
		Image_tiff = 160,
		// Token: 0x04000600 RID: 1536
		Image_jpeg = 288,
		// Token: 0x04000601 RID: 1537
		Audio = 256,
		// Token: 0x04000602 RID: 1538
		Video = 1024,
		// Token: 0x04000603 RID: 1539
		Application = 2048,
		// Token: 0x04000604 RID: 1540
		Application_octet_stream = 6144,
		// Token: 0x04000605 RID: 1541
		Multipart = 8192,
		// Token: 0x04000606 RID: 1542
		Multipart_mixed = 24576,
		// Token: 0x04000607 RID: 1543
		Multipart_alternative = 40960,
		// Token: 0x04000608 RID: 1544
		Multipart_parallel = 73728,
		// Token: 0x04000609 RID: 1545
		Multipart_related = 139264,
		// Token: 0x0400060A RID: 1546
		Multipart_signed = 270336,
		// Token: 0x0400060B RID: 1547
		Message = 524288,
		// Token: 0x0400060C RID: 1548
		Message_rfc822 = 1572864,
		// Token: 0x0400060D RID: 1549
		NotSpecified = 2097152,
		// Token: 0x0400060E RID: 1550
		Unknown = 4194304
	}
}
