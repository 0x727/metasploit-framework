using System;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000ED RID: 237
	public class POP3_e_Started : EventArgs
	{
		// Token: 0x06000983 RID: 2435 RVA: 0x00039B22 File Offset: 0x00038B22
		internal POP3_e_Started(string response)
		{
			this.m_Response = response;
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000984 RID: 2436 RVA: 0x00039B3C File Offset: 0x00038B3C
		// (set) Token: 0x06000985 RID: 2437 RVA: 0x00039B54 File Offset: 0x00038B54
		public string Response
		{
			get
			{
				return this.m_Response;
			}
			set
			{
				this.m_Response = value;
			}
		}

		// Token: 0x0400043A RID: 1082
		private string m_Response = null;
	}
}
