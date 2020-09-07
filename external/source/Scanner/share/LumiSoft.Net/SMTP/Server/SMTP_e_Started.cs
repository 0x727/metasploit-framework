using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x02000141 RID: 321
	public class SMTP_e_Started : EventArgs
	{
		// Token: 0x06000CA6 RID: 3238 RVA: 0x0004E4C4 File Offset: 0x0004D4C4
		public SMTP_e_Started(SMTP_Session session, SMTP_Reply reply)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = reply == null;
			if (flag2)
			{
				throw new ArgumentNullException("reply");
			}
			this.m_pSession = session;
			this.m_pReply = reply;
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000CA7 RID: 3239 RVA: 0x0004E520 File Offset: 0x0004D520
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x0004E538 File Offset: 0x0004D538
		// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x0004E550 File Offset: 0x0004D550
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

		// Token: 0x0400055A RID: 1370
		private SMTP_Session m_pSession = null;

		// Token: 0x0400055B RID: 1371
		private SMTP_Reply m_pReply = null;
	}
}
