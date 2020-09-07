using System;
using System.IO;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013A RID: 314
	public class SMTP_e_MessageStored : EventArgs
	{
		// Token: 0x06000C45 RID: 3141 RVA: 0x0004B244 File Offset: 0x0004A244
		public SMTP_e_MessageStored(SMTP_Session session, Stream stream, SMTP_Reply reply)
		{
			bool flag = session == null;
			if (flag)
			{
				throw new ArgumentNullException("session");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = reply == null;
			if (flag3)
			{
				throw new ArgumentNullException("reply");
			}
			this.m_pSession = session;
			this.m_pStream = stream;
			this.m_pReply = reply;
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x0004B2C0 File Offset: 0x0004A2C0
		public SMTP_Session Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000C47 RID: 3143 RVA: 0x0004B2D8 File Offset: 0x0004A2D8
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000C48 RID: 3144 RVA: 0x0004B2F0 File Offset: 0x0004A2F0
		// (set) Token: 0x06000C49 RID: 3145 RVA: 0x0004B308 File Offset: 0x0004A308
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

		// Token: 0x04000536 RID: 1334
		private SMTP_Session m_pSession = null;

		// Token: 0x04000537 RID: 1335
		private Stream m_pStream = null;

		// Token: 0x04000538 RID: 1336
		private SMTP_Reply m_pReply = null;
	}
}
