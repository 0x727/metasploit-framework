using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000239 RID: 569
	public class FTP_e_Cdup : EventArgs
	{
		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x060014A7 RID: 5287 RVA: 0x00081698 File Offset: 0x00080698
		// (set) Token: 0x060014A8 RID: 5288 RVA: 0x000816B0 File Offset: 0x000806B0
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

		// Token: 0x0400080C RID: 2060
		private FTP_t_ReplyLine[] m_pReplyLines = null;
	}
}
