using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000240 RID: 576
	public class FTP_e_Rmd : EventArgs
	{
		// Token: 0x060014C6 RID: 5318 RVA: 0x00081A68 File Offset: 0x00080A68
		public FTP_e_Rmd(string dirName)
		{
			bool flag = dirName == null;
			if (flag)
			{
				throw new ArgumentNullException("dirName");
			}
			this.m_DirName = dirName;
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x060014C7 RID: 5319 RVA: 0x00081AA8 File Offset: 0x00080AA8
		// (set) Token: 0x060014C8 RID: 5320 RVA: 0x00081AC0 File Offset: 0x00080AC0
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

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x060014C9 RID: 5321 RVA: 0x00081ACC File Offset: 0x00080ACC
		public string DirName
		{
			get
			{
				return this.m_DirName;
			}
		}

		// Token: 0x0400081C RID: 2076
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x0400081D RID: 2077
		private string m_DirName = null;
	}
}
