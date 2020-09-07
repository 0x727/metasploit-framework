using System;
using System.Text;

namespace LumiSoft.Net.SMTP.Server
{
	// Token: 0x0200013E RID: 318
	public class SMTP_Reply
	{
		// Token: 0x06000C57 RID: 3159 RVA: 0x0004B564 File Offset: 0x0004A564
		public SMTP_Reply(int replyCode, string replyLine) : this(replyCode, new string[]
		{
			replyLine
		})
		{
			bool flag = replyLine == null;
			if (flag)
			{
				throw new ArgumentNullException("replyLine");
			}
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x0004B598 File Offset: 0x0004A598
		public SMTP_Reply(int replyCode, string[] replyLines)
		{
			bool flag = replyCode < 200 || replyCode > 599;
			if (flag)
			{
				throw new ArgumentException("Argument 'replyCode' value must be >= 200 and <= 599.", "replyCode");
			}
			bool flag2 = replyLines == null;
			if (flag2)
			{
				throw new ArgumentNullException("replyLines");
			}
			bool flag3 = replyLines.Length == 0;
			if (flag3)
			{
				throw new ArgumentException("Argument 'replyLines' must conatin at least one line.", "replyLines");
			}
			this.m_ReplyCode = replyCode;
			this.m_pReplyLines = replyLines;
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0004B620 File Offset: 0x0004A620
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_pReplyLines.Length; i++)
			{
				bool flag = i == this.m_pReplyLines.Length - 1;
				if (flag)
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						this.m_ReplyCode,
						" ",
						this.m_pReplyLines[i],
						"\r\n"
					}));
				}
				else
				{
					stringBuilder.Append(string.Concat(new object[]
					{
						this.m_ReplyCode,
						"-",
						this.m_pReplyLines[i],
						"\r\n"
					}));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0004B6EC File Offset: 0x0004A6EC
		public int ReplyCode
		{
			get
			{
				return this.m_ReplyCode;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000C5B RID: 3163 RVA: 0x0004B704 File Offset: 0x0004A704
		public string[] ReplyLines
		{
			get
			{
				return this.m_pReplyLines;
			}
		}

		// Token: 0x04000541 RID: 1345
		private int m_ReplyCode = 0;

		// Token: 0x04000542 RID: 1346
		private string[] m_pReplyLines = null;
	}
}
