using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013D RID: 317
	public class SMTP_e_RcptTo : EventArgs
	{
		// Token: 0x06000C52 RID: 3154 RVA: 0x0004B474 File Offset: 0x0004A474
		public SMTP_e_RcptTo(SMTP_Session session, SMTP_RcptTo to, SMTP_Reply reply)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = to == null;
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
			this.m_pRcptTo = to;
			this.m_pReply = reply;
		}

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x0004B4F0 File Offset: 0x0004A4F0
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x0004B508 File Offset: 0x0004A508
		public SMTP_RcptTo RcptTo
		{
			get
			{
				return this.m_pRcptTo;
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000C55 RID: 3157 RVA: 0x0004B520 File Offset: 0x0004A520
		// (set) Token: 0x06000C56 RID: 3158 RVA: 0x0004B538 File Offset: 0x0004A538
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

		// Token: 0x0400053E RID: 1342
		private SMTP_Session m_pSession = null;

		// Token: 0x0400053F RID: 1343
		private SMTP_RcptTo m_pRcptTo = null;

		// Token: 0x04000540 RID: 1344
		private SMTP_Reply m_pReply = null;
	}
}
