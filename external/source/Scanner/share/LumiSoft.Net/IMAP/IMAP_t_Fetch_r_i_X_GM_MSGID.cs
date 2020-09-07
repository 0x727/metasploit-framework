using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BF RID: 447
	public class IMAP_t_Fetch_r_i_X_GM_MSGID : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010FE RID: 4350 RVA: 0x00068DB4 File Offset: 0x00067DB4
		public IMAP_t_Fetch_r_i_X_GM_MSGID(ulong msgID)
		{
			this.m_MsgID = msgID;
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060010FF RID: 4351 RVA: 0x00068DD0 File Offset: 0x00067DD0
		public ulong MsgID
		{
			get
			{
				return this.m_MsgID;
			}
		}

		// Token: 0x040006C7 RID: 1735
		private ulong m_MsgID = 0UL;
	}
}
