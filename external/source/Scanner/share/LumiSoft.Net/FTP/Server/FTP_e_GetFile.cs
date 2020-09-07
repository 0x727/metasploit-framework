using System;
using System.IO;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023D RID: 573
	public class FTP_e_GetFile : EventArgs
	{
		// Token: 0x060014B6 RID: 5302 RVA: 0x00081858 File Offset: 0x00080858
		public FTP_e_GetFile(string file)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = file == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'file' name must be specified.", "file");
			}
			this.m_FileName = file;
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x060014B7 RID: 5303 RVA: 0x000818C0 File Offset: 0x000808C0
		// (set) Token: 0x060014B8 RID: 5304 RVA: 0x000818D8 File Offset: 0x000808D8
		public FTP_t_ReplyLine[] Error
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

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x060014B9 RID: 5305 RVA: 0x000818E4 File Offset: 0x000808E4
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x060014BA RID: 5306 RVA: 0x000818FC File Offset: 0x000808FC
		// (set) Token: 0x060014BB RID: 5307 RVA: 0x00081914 File Offset: 0x00080914
		public Stream FileStream
		{
			get
			{
				return this.m_pFileStream;
			}
			set
			{
				this.m_pFileStream = value;
			}
		}

		// Token: 0x04000814 RID: 2068
		private string m_FileName = null;

		// Token: 0x04000815 RID: 2069
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x04000816 RID: 2070
		private Stream m_pFileStream = null;
	}
}
