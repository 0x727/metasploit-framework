using System;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000E9 RID: 233
	public class POP3_e_DeleteMessage : EventArgs
	{
		// Token: 0x06000974 RID: 2420 RVA: 0x0003993C File Offset: 0x0003893C
		internal POP3_e_DeleteMessage(POP3_ServerMessage message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			this.m_pMessage = message;
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x00039974 File Offset: 0x00038974
		public POP3_ServerMessage Message
		{
			get
			{
				return this.m_pMessage;
			}
		}

		// Token: 0x04000432 RID: 1074
		private POP3_ServerMessage m_pMessage = null;
	}
}
