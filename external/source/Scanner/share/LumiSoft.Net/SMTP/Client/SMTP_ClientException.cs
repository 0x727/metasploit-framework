using System;

namespace LumiSoft.Net.SMTP.Client
{
	// Token: 0x0200014B RID: 331
	public class SMTP_ClientException : Exception
	{
		// Token: 0x06000D69 RID: 3433 RVA: 0x0005471C File Offset: 0x0005371C
		public SMTP_ClientException(string responseLine) : base(responseLine.TrimEnd(new char[0]))
		{
			bool flag = responseLine == null;
			if (flag)
			{
				throw new ArgumentNullException("responseLine");
			}
			this.m_pReplyLines = new SMTP_t_ReplyLine[]
			{
				SMTP_t_ReplyLine.Parse(responseLine)
			};
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x00054770 File Offset: 0x00053770
		public SMTP_ClientException(SMTP_t_ReplyLine[] replyLines) : base(replyLines[0].ToString().TrimEnd(new char[0]))
		{
			bool flag = replyLines == null;
			if (flag)
			{
				throw new ArgumentNullException("replyLines");
			}
			this.m_pReplyLines = replyLines;
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06000D6B RID: 3435 RVA: 0x000547BC File Offset: 0x000537BC
		[Obsolete("Use property 'ReplyLines' insead.")]
		public int StatusCode
		{
			get
			{
				return this.m_pReplyLines[0].ReplyCode;
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x000547DC File Offset: 0x000537DC
		[Obsolete("Use property 'ReplyLines' insead.")]
		public string ResponseText
		{
			get
			{
				return this.m_pReplyLines[0].Text;
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06000D6D RID: 3437 RVA: 0x000547FC File Offset: 0x000537FC
		public SMTP_t_ReplyLine[] ReplyLines
		{
			get
			{
				return this.m_pReplyLines;
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x00054814 File Offset: 0x00053814
		public bool IsPermanentError
		{
			get
			{
				return this.m_pReplyLines[0].ReplyCode >= 500 && this.m_pReplyLines[0].ReplyCode <= 599;
			}
		}

		// Token: 0x0400059C RID: 1436
		private SMTP_t_ReplyLine[] m_pReplyLines = null;
	}
}
