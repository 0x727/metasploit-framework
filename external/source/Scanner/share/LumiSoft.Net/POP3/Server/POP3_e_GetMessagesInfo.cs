using System;
using System.Collections.Generic;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000EA RID: 234
	public class POP3_e_GetMessagesInfo : EventArgs
	{
		// Token: 0x06000976 RID: 2422 RVA: 0x0003998C File Offset: 0x0003898C
		internal POP3_e_GetMessagesInfo()
		{
			this.m_pMessages = new List<POP3_ServerMessage>();
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000977 RID: 2423 RVA: 0x000399A8 File Offset: 0x000389A8
		public List<POP3_ServerMessage> Messages
		{
			get
			{
				return this.m_pMessages;
			}
		}

		// Token: 0x04000433 RID: 1075
		private List<POP3_ServerMessage> m_pMessages = null;
	}
}
