using System;
using System.IO;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013B RID: 315
	public class SMTP_e_Message : EventArgs
	{
		// Token: 0x06000C4A RID: 3146 RVA: 0x0004B334 File Offset: 0x0004A334
		public SMTP_e_Message(SMTP_Session session)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			this.m_pSession = session;
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000C4B RID: 3147 RVA: 0x0004B374 File Offset: 0x0004A374
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x0004B38C File Offset: 0x0004A38C
		// (set) Token: 0x06000C4D RID: 3149 RVA: 0x0004B3A4 File Offset: 0x0004A3A4
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Stream");
				}
				this.m_pStream = value;
			}
		}

		// Token: 0x04000539 RID: 1337
		private SMTP_Session m_pSession = null;

		// Token: 0x0400053A RID: 1338
		private Stream m_pStream = null;
	}
}
