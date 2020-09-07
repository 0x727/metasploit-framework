using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BB RID: 443
	public class IMAP_t_Fetch_r_i_Rfc822Header : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010F4 RID: 4340 RVA: 0x00068C10 File Offset: 0x00067C10
		public IMAP_t_Fetch_r_i_Rfc822Header(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00068C48 File Offset: 0x00067C48
		internal void SetStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x060010F6 RID: 4342 RVA: 0x00068C74 File Offset: 0x00067C74
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x040006C3 RID: 1731
		private Stream m_pStream = null;
	}
}
