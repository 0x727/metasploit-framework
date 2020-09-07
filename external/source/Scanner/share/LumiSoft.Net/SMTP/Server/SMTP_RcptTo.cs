using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013C RID: 316
	public class SMTP_RcptTo
	{
		// Token: 0x06000C4E RID: 3150 RVA: 0x0004B3D0 File Offset: 0x0004A3D0
		public SMTP_RcptTo(string mailbox, SMTP_DSN_Notify notify, string orcpt)
		{
			bool flag = mailbox == null;
			if (flag)
			{
				throw new ArgumentNullException("mailbox");
			}
			this.m_Mailbox = mailbox;
			this.m_Notify = notify;
			this.m_ORCPT = orcpt;
		}

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x0004B42C File Offset: 0x0004A42C
		public string Mailbox
		{
			get
			{
				return this.m_Mailbox;
			}
		}

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x0004B444 File Offset: 0x0004A444
		public SMTP_DSN_Notify Notify
		{
			get
			{
				return this.m_Notify;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x0004B45C File Offset: 0x0004A45C
		public string ORCPT
		{
			get
			{
				return this.m_ORCPT;
			}
		}

		// Token: 0x0400053B RID: 1339
		private string m_Mailbox = "";

		// Token: 0x0400053C RID: 1340
		private SMTP_DSN_Notify m_Notify = SMTP_DSN_Notify.NotSpecified;

		// Token: 0x0400053D RID: 1341
		private string m_ORCPT = "";
	}
}
