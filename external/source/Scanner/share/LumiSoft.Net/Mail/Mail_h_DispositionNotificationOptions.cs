using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017A RID: 378
	public class Mail_h_DispositionNotificationOptions : MIME_h
	{
		// Token: 0x06000F61 RID: 3937 RVA: 0x0005F5C8 File Offset: 0x0005E5C8
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			return "TODO:";
		}

		// Token: 0x1700051C RID: 1308
		// (get) Token: 0x06000F62 RID: 3938 RVA: 0x0005F5E0 File Offset: 0x0005E5E0
		public override bool IsModified
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700051D RID: 1309
		// (get) Token: 0x06000F63 RID: 3939 RVA: 0x0005F5F4 File Offset: 0x0005E5F4
		public override string Name
		{
			get
			{
				return "Disposition-Notification-Options";
			}
		}

		// Token: 0x1700051E RID: 1310
		// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0005F60C File Offset: 0x0005E60C
		public string Address
		{
			get
			{
				return "TODO:";
			}
		}
	}
}
