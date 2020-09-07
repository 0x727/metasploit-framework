using System;
using System.IO;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000237 RID: 567
	public class FTP_e_Appe : EventArgs
	{
		// Token: 0x0600149B RID: 5275 RVA: 0x00081500 File Offset: 0x00080500
		public FTP_e_Appe(string file)
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

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x0600149C RID: 5276 RVA: 0x00081568 File Offset: 0x00080568
		// (set) Token: 0x0600149D RID: 5277 RVA: 0x00081580 File Offset: 0x00080580
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

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x0600149E RID: 5278 RVA: 0x0008158C File Offset: 0x0008058C
		public string FileName
		{
			get
			{
				return this.m_FileName;
			}
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x0600149F RID: 5279 RVA: 0x000815A4 File Offset: 0x000805A4
		// (set) Token: 0x060014A0 RID: 5280 RVA: 0x000815BC File Offset: 0x000805BC
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

		// Token: 0x04000806 RID: 2054
		private string m_FileName = null;

		// Token: 0x04000807 RID: 2055
		private FTP_t_ReplyLine[] m_pReplyLines = null;

		// Token: 0x04000808 RID: 2056
		private Stream m_pFileStream = null;
	}
}
