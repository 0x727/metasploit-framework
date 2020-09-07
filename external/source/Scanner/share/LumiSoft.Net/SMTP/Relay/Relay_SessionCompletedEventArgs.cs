using System;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000148 RID: 328
	public class Relay_SessionCompletedEventArgs
	{
		// Token: 0x06000D13 RID: 3347 RVA: 0x000516CC File Offset: 0x000506CC
		public Relay_SessionCompletedEventArgs(Relay_Session session, Exception exception)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			this.m_pSession = session;
			this.m_pException = exception;
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x00051714 File Offset: 0x00050714
		public Relay_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06000D15 RID: 3349 RVA: 0x0005172C File Offset: 0x0005072C
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}

		// Token: 0x0400058C RID: 1420
		private Relay_Session m_pSession = null;

		// Token: 0x0400058D RID: 1421
		private Exception m_pException = null;
	}
}
