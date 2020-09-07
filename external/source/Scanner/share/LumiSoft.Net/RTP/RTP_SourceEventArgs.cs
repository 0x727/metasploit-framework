using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000DF RID: 223
	public class RTP_SourceEventArgs : EventArgs
	{
		// Token: 0x060008FC RID: 2300 RVA: 0x00036088 File Offset: 0x00035088
		public RTP_SourceEventArgs(RTP_Source source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			this.m_pSource = source;
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060008FD RID: 2301 RVA: 0x000360C0 File Offset: 0x000350C0
		public RTP_Source Source
		{
			get
			{
				return this.m_pSource;
			}
		}

		// Token: 0x0400040F RID: 1039
		private RTP_Source m_pSource = null;
	}
}
