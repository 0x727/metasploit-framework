using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023A RID: 570
	public class FTP_e_Cwd : EventArgs
	{
		// Token: 0x060014A9 RID: 5289 RVA: 0x000816BC File Offset: 0x000806BC
		public FTP_e_Cwd(string dirName)
		{
			bool flag = dirName == null;
			if (flag)
			{
				throw new ArgumentNullException("dirName");
			}
			this.m_DirName = dirName;
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x000816FC File Offset: 0x000806FC
		// (set) Token: 0x060014AB RID: 5291 RVA: 0x00081714 File Offset: 0x00080714
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

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x00081720 File Offset: 0x00080720
		public string DirName
		{
			get
			{
				return this.m_DirName;
			}
		}

		// Token: 0x0400080D RID: 2061
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x0400080E RID: 2062
		private string m_DirName = null;
	}
}
