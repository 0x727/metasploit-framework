using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x02000139 RID: 313
	public class SMTP_e_MailFrom : EventArgs
	{
		// Token: 0x06000C40 RID: 3136 RVA: 0x0004B154 File Offset: 0x0004A154
		public SMTP_e_MailFrom(SMTP_Session session, SMTP_MailFrom from, SMTP_Reply reply)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = from == null;
			if (flag2)
			{
				throw new ArgumentNullException("from");
			}
			bool flag3 = reply == null;
			if (flag3)
			{
				throw new ArgumentNullException("reply");
			}
			this.m_pSession = session;
			this.m_pMailFrom = from;
			this.m_pReply = reply;
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000C41 RID: 3137 RVA: 0x0004B1D0 File Offset: 0x0004A1D0
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x0004B1E8 File Offset: 0x0004A1E8
		public SMTP_MailFrom MailFrom
		{
			get
			{
				return this.m_pMailFrom;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000C43 RID: 3139 RVA: 0x0004B200 File Offset: 0x0004A200
		// (set) Token: 0x06000C44 RID: 3140 RVA: 0x0004B218 File Offset: 0x0004A218
		public SMTP_Reply Reply
		{
			get
			{
				return this.m_pReply;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Reply");
				}
				this.m_pReply = value;
			}
		}

		// Token: 0x04000533 RID: 1331
		private SMTP_Session m_pSession = null;

		// Token: 0x04000534 RID: 1332
		private SMTP_MailFrom m_pMailFrom = null;

		// Token: 0x04000535 RID: 1333
		private SMTP_Reply m_pReply = null;
	}
}
