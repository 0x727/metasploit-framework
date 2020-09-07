using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BA RID: 442
	public class IMAP_t_Fetch_r_i_Rfc822 : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010F1 RID: 4337 RVA: 0x00068B94 File Offset: 0x00067B94
		public IMAP_t_Fetch_r_i_Rfc822(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00068BCC File Offset: 0x00067BCC
		internal void SetStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x060010F3 RID: 4339 RVA: 0x00068BF8 File Offset: 0x00067BF8
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x040006C2 RID: 1730
		private Stream m_pStream = null;
	}
}
