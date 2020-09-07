using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023F RID: 575
	public class FTP_e_Mkd : EventArgs
	{
		// Token: 0x060014C2 RID: 5314 RVA: 0x000819EC File Offset: 0x000809EC
		public FTP_e_Mkd(string dirName)
		{
			bool flag = dirName == null;
			if (flag)
			{
				throw new ArgumentNullException("dirName");
			}
			this.m_DirName = dirName;
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x060014C3 RID: 5315 RVA: 0x00081A2C File Offset: 0x00080A2C
		// (set) Token: 0x060014C4 RID: 5316 RVA: 0x00081A44 File Offset: 0x00080A44
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

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x060014C5 RID: 5317 RVA: 0x00081A50 File Offset: 0x00080A50
		public string DirName
		{
			get
			{
				return this.m_DirName;
			}
		}

		// Token: 0x0400081A RID: 2074
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x0400081B RID: 2075
		private string m_DirName = null;
	}
}
