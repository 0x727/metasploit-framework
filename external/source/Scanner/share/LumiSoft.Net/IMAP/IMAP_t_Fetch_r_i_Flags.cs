using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001B8 RID: 440
	public class IMAP_t_Fetch_r_i_Flags : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010ED RID: 4333 RVA: 0x00068B18 File Offset: 0x00067B18
		public IMAP_t_Fetch_r_i_Flags(IMAP_t_MsgFlags flags)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			this.m_pFlags = flags;
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x060010EE RID: 4334 RVA: 0x00068B50 File Offset: 0x00067B50
		public IMAP_t_MsgFlags Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x040006C0 RID: 1728
		private IMAP_t_MsgFlags m_pFlags = null;
	}
}
