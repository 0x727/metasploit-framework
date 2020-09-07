using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001B4 RID: 436
	public class IMAP_t_Fetch_r_i_Body : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010CC RID: 4300 RVA: 0x00067CF0 File Offset: 0x00066CF0
		public IMAP_t_Fetch_r_i_Body(string section, int offset, Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_Section = section;
			this.m_Offset = offset;
			this.m_pStream = stream;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00067D44 File Offset: 0x00066D44
		internal void SetStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x060010CE RID: 4302 RVA: 0x00067D70 File Offset: 0x00066D70
		public string BodySection
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x060010CF RID: 4303 RVA: 0x00067D88 File Offset: 0x00066D88
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x060010D0 RID: 4304 RVA: 0x00067DA0 File Offset: 0x00066DA0
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x040006B2 RID: 1714
		private string m_Section = null;

		// Token: 0x040006B3 RID: 1715
		private int m_Offset = -1;

		// Token: 0x040006B4 RID: 1716
		private Stream m_pStream = null;
	}
}
