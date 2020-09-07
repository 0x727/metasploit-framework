using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000241 RID: 577
	public class FTP_e_Rnto : EventArgs
	{
		// Token: 0x060014CA RID: 5322 RVA: 0x00081AE4 File Offset: 0x00080AE4
		public FTP_e_Rnto(string sourcePath, string targetPath)
		{
			bool flag = sourcePath == null;
			if (flag)
			{
				throw new ArgumentNullException("sourcePath");
			}
			bool flag2 = targetPath == null;
			if (flag2)
			{
				throw new ArgumentNullException("targetPath");
			}
			this.m_SourcePath = sourcePath;
			this.m_TargetPath = targetPath;
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x060014CB RID: 5323 RVA: 0x00081B44 File Offset: 0x00080B44
		// (set) Token: 0x060014CC RID: 5324 RVA: 0x00081B5C File Offset: 0x00080B5C
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

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x060014CD RID: 5325 RVA: 0x00081B68 File Offset: 0x00080B68
		public string SourcePath
		{
			get
			{
				return this.m_SourcePath;
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x060014CE RID: 5326 RVA: 0x00081B80 File Offset: 0x00080B80
		public string TargetPath
		{
			get
			{
				return this.m_TargetPath;
			}
		}

		// Token: 0x0400081E RID: 2078
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x0400081F RID: 2079
		private string m_SourcePath = null;

		// Token: 0x04000820 RID: 2080
		private string m_TargetPath = null;
	}
}
