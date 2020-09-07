using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000242 RID: 578
	public class FTP_e_Started : EventArgs
	{
		// Token: 0x060014CF RID: 5327 RVA: 0x00081B98 File Offset: 0x00080B98
		internal FTP_e_Started(string response)
		{
			this.m_Response = response;
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00081BB0 File Offset: 0x00080BB0
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x00081BC8 File Offset: 0x00080BC8
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

		// Token: 0x04000821 RID: 2081
		private string m_Response = null;
	}
}
