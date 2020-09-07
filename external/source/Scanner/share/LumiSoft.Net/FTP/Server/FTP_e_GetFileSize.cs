using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023E RID: 574
	public class FTP_e_GetFileSize : EventArgs
	{
		// Token: 0x060014BC RID: 5308 RVA: 0x00081920 File Offset: 0x00080920
		public FTP_e_GetFileSize(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			this.m_FileName = fileName;
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x060014BD RID: 5309 RVA: 0x00081968 File Offset: 0x00080968
		// (set) Token: 0x060014BE RID: 5310 RVA: 0x00081980 File Offset: 0x00080980
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

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x060014BF RID: 5311 RVA: 0x0008198C File Offset: 0x0008098C
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x000819A4 File Offset: 0x000809A4
		// (set) Token: 0x060014C1 RID: 5313 RVA: 0x000819BC File Offset: 0x000809BC
		public long FileSize
		{
			get
			{
				return this.m_FileSize;
			}
			set
			{
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentException("Property 'FileSize' value must be >= 0.", "FileSize");
				}
				this.m_FileSize = value;
			}
		}

		// Token: 0x04000817 RID: 2071
		private string m_FileName = null;

		// Token: 0x04000818 RID: 2072
		private long m_FileSize = 0L;

		// Token: 0x04000819 RID: 2073
		private FTP_t_ReplyLine[] m_pReplyLines = null;
	}
}
