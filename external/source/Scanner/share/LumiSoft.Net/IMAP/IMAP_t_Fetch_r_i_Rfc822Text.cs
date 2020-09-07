using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001BD RID: 445
	public class IMAP_t_Fetch_r_i_Rfc822Text : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010F9 RID: 4345 RVA: 0x00068CE0 File Offset: 0x00067CE0
		public IMAP_t_Fetch_r_i_Rfc822Text(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00068D18 File Offset: 0x00067D18
		internal void SetStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x060010FB RID: 4347 RVA: 0x00068D44 File Offset: 0x00067D44
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x040006C5 RID: 1733
		private Stream m_pStream = null;
	}
}
