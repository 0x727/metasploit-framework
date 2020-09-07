using System;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x02000137 RID: 311
	public class SMTP_e_Ehlo : EventArgs
	{
		// Token: 0x06000C35 RID: 3125 RVA: 0x0004AF50 File Offset: 0x00049F50
		public SMTP_e_Ehlo(SMTP_Session session, string domain, SMTP_Reply reply)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = domain == null;
			if (flag2)
			{
				throw new ArgumentNullException("domain");
			}
			bool flag3 = domain == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'domain' value must be sepcified.", "domain");
			}
			bool flag4 = reply == null;
			if (flag4)
			{
				throw new ArgumentNullException("reply");
			}
			this.m_pSession = session;
			this.m_Domain = domain;
			this.m_pReply = reply;
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000C36 RID: 3126 RVA: 0x0004AFF0 File Offset: 0x00049FF0
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000C37 RID: 3127 RVA: 0x0004B008 File Offset: 0x0004A008
		public string Domain
		{
			get
			{
				return this.m_Domain;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000C38 RID: 3128 RVA: 0x0004B020 File Offset: 0x0004A020
		// (set) Token: 0x06000C39 RID: 3129 RVA: 0x0004B038 File Offset: 0x0004A038
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

		// Token: 0x0400052B RID: 1323
		private SMTP_Session m_pSession = null;

		// Token: 0x0400052C RID: 1324
		private string m_Domain = "";

		// Token: 0x0400052D RID: 1325
		private SMTP_Reply m_pReply = null;
	}
}
