using System;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017E RID: 382
	public abstract class Mail_t_Address
	{
		// Token: 0x06000F77 RID: 3959 RVA: 0x00009954 File Offset: 0x00008954
		public Mail_t_Address()
		{
		}

		// Token: 0x06000F78 RID: 3960
		public abstract string ToString(MIME_Encoding_EncodedWord wordEncoder);
	}
}
