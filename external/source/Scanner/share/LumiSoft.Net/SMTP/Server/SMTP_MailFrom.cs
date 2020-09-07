using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x02000138 RID: 312
	public class SMTP_MailFrom
	{
		// Token: 0x06000C3A RID: 3130 RVA: 0x0004B064 File Offset: 0x0004A064
		public SMTP_MailFrom(string mailbox, int size, string body, SMTP_DSN_Ret ret, string envid)
		{
			bool flag = mailbox == null;
			if (flag)
			{
				throw new ArgumentNullException("mailbox");
			}
			this.m_Mailbox = mailbox;
			this.m_Size = size;
			this.m_Body = body;
			this.m_RET = ret;
			this.m_ENVID = envid;
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000C3B RID: 3131 RVA: 0x0004B0DC File Offset: 0x0004A0DC
		public string Mailbox
		{
			get
			{
				return this.m_Mailbox;
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000C3C RID: 3132 RVA: 0x0004B0F4 File Offset: 0x0004A0F4
		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000C3D RID: 3133 RVA: 0x0004B10C File Offset: 0x0004A10C
		public string Body
		{
			get
			{
				return this.m_Body;
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x0004B124 File Offset: 0x0004A124
		public SMTP_DSN_Ret RET
		{
			get
			{
				return this.m_RET;
			}
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000C3F RID: 3135 RVA: 0x0004B13C File Offset: 0x0004A13C
		public string ENVID
		{
			get
			{
				return this.m_ENVID;
			}
		}

		// Token: 0x0400052E RID: 1326
		private string m_Mailbox = "";

		// Token: 0x0400052F RID: 1327
		private int m_Size = -1;

		// Token: 0x04000530 RID: 1328
		private string m_Body = null;

		// Token: 0x04000531 RID: 1329
		private SMTP_DSN_Ret m_RET = SMTP_DSN_Ret.NotSpecified;

		// Token: 0x04000532 RID: 1330
		private string m_ENVID = null;
	}
}
