using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BC RID: 444
	public class IMAP_t_Fetch_r_i_Rfc822Size : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010F7 RID: 4343 RVA: 0x00068C8C File Offset: 0x00067C8C
		public IMAP_t_Fetch_r_i_Rfc822Size(int size)
		{
			bool flag = size < 0;
			if (flag)
			{
				throw new ArgumentException("Argument 'size' value must be >= 0.", "size");
			}
			this.m_Size = size;
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x060010F8 RID: 4344 RVA: 0x00068CC8 File Offset: 0x00067CC8
		public int Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x040006C4 RID: 1732
		private int m_Size = 0;
	}
}
