using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001B9 RID: 441
	public class IMAP_t_Fetch_r_i_InternalDate : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010EF RID: 4335 RVA: 0x00068B68 File Offset: 0x00067B68
		public IMAP_t_Fetch_r_i_InternalDate(DateTime date)
		{
			this.m_Date = date;
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x060010F0 RID: 4336 RVA: 0x00068B7C File Offset: 0x00067B7C
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006C1 RID: 1729
		private DateTime m_Date;
	}
}
