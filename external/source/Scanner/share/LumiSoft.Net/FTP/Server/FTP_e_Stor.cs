using System;
using System.IO;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000243 RID: 579
	public class FTP_e_Stor : EventArgs
	{
		// Token: 0x060014D2 RID: 5330 RVA: 0x00081BD4 File Offset: 0x00080BD4
		public FTP_e_Stor(string file)
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

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x060014D3 RID: 5331 RVA: 0x00081C3C File Offset: 0x00080C3C
		// (set) Token: 0x060014D4 RID: 5332 RVA: 0x00081C54 File Offset: 0x00080C54
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

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x060014D5 RID: 5333 RVA: 0x00081C60 File Offset: 0x00080C60
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x060014D6 RID: 5334 RVA: 0x00081C78 File Offset: 0x00080C78
		// (set) Token: 0x060014D7 RID: 5335 RVA: 0x00081C90 File Offset: 0x00080C90
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

		// Token: 0x04000822 RID: 2082
		private string m_FileName = null;

		// Token: 0x04000823 RID: 2083
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x04000824 RID: 2084
		private Stream m_pFileStream = null;
	}
}
