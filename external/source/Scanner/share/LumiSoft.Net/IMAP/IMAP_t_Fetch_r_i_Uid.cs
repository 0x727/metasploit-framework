using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BE RID: 446
	public class IMAP_t_Fetch_r_i_Uid : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010FC RID: 4348 RVA: 0x00068D5C File Offset: 0x00067D5C
		public IMAP_t_Fetch_r_i_Uid(long uid)
		{
			bool flag = uid < 0L;
			if (flag)
			{
				throw new ArgumentException("Argument 'uid' value must be >= 0.", "uid");
			}
			this.m_UID = uid;
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x060010FD RID: 4349 RVA: 0x00068D9C File Offset: 0x00067D9C
		public long UID
		{
			get
			{
				return this.m_UID;
			}
		}

		// Token: 0x040006C6 RID: 1734
		private long m_UID = 0L;
	}
}
