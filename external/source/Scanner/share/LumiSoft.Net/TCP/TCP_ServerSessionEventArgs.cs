using System;

namespace LumiSoft.Net.TCP
{
	// Token: 0x02000034 RID: 52
	public class TCP_ServerSessionEventArgs<T> : EventArgs where T : TCP_ServerSession, new()
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x0000C736 File Offset: 0x0000B736
		internal TCP_ServerSessionEventArgs(TCP_Server<T> server, T session)
		{
			this.m_pServer = server;
			this.m_pSession = session;
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x0000C764 File Offset: 0x0000B764
		public TCP_Server<T> Server
		{
			get
			{
				return this.m_pServer;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000C77C File Offset: 0x0000B77C
		public T Session
		{
			get
			{
				return this.m_pSession;
			}
		}

		// Token: 0x040000CF RID: 207
		private TCP_Server<T> m_pServer = null;

		// Token: 0x040000D0 RID: 208
		private T m_pSession = default(T);
	}
}
