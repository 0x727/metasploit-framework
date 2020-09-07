using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023B RID: 571
	public class FTP_e_Dele : EventArgs
	{
		// Token: 0x060014AD RID: 5293 RVA: 0x00081738 File Offset: 0x00080738
		public FTP_e_Dele(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			this.m_FileName = fileName;
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x060014AE RID: 5294 RVA: 0x00081778 File Offset: 0x00080778
		// (set) Token: 0x060014AF RID: 5295 RVA: 0x00081790 File Offset: 0x00080790
		public FTP_t_ReplyLine[] Response
		{
			get
			{
				return this.m_pReplyLines;
			}
			set
			{
				this.m_pReplyLines = value;
			}
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x060014B0 RID: 5296 RVA: 0x0008179C File Offset: 0x0008079C
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
		}

		// Token: 0x0400080F RID: 2063
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x04000810 RID: 2064
		private string m_FileName = null;
	}
}
