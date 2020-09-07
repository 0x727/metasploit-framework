using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C0 RID: 448
	public class IMAP_t_Fetch_r_i_X_GM_THRID : IMAP_t_Fetch_r_i
	{
		// Token: 0x06001100 RID: 4352 RVA: 0x00068DE8 File Offset: 0x00067DE8
		public IMAP_t_Fetch_r_i_X_GM_THRID(ulong threadID)
		{
			this.m_ThreadID = threadID;
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x06001101 RID: 4353 RVA: 0x00068E04 File Offset: 0x00067E04
		public ulong ThreadID
		{
			get
			{
				return this.m_ThreadID;
			}
		}

		// Token: 0x040006C8 RID: 1736
		private ulong m_ThreadID = 0UL;
	}
}
