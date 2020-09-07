using System;

namespace LumiSoft.Net.FTP
{
	// Token: 0x02000231 RID: 561
	public class FTP_t_ReplyLine
	{
		// Token: 0x06001469 RID: 5225 RVA: 0x0007F5F4 File Offset: 0x0007E5F4
		public FTP_t_ReplyLine(int replyCode, string text, bool isLastLine)
		{
			bool flag = text == null;
			if (flag)
			{
				text = "";
			}
			this.m_ReplyCode = replyCode;
			this.m_Text = text;
			this.m_IsLastLine = isLastLine;
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0007F644 File Offset: 0x0007E644
		public static FTP_t_ReplyLine Parse(string line)
		{
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			bool flag2 = line.Length < 3;
			if (flag2)
			{
				throw new ParseException("Invalid FTP server reply-line '" + line + "'.");
			}
			int replyCode = 0;
			bool flag3 = !int.TryParse(line.Substring(0, 3), out replyCode);
			if (flag3)
			{
				throw new ParseException("Invalid FTP server reply-line '" + line + "' reply-code.");
			}
			bool isLastLine = true;
			bool flag4 = line.Length > 3;
			if (flag4)
			{
				isLastLine = (line[3] == ' ');
			}
			string text = "";
			bool flag5 = line.Length > 5;
			if (flag5)
			{
				text = line.Substring(4);
			}
			return new FTP_t_ReplyLine(replyCode, text, isLastLine);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0007F70C File Offset: 0x0007E70C
		public override string ToString()
		{
			bool isLastLine = this.m_IsLastLine;
			string result;
			if (isLastLine)
			{
				result = this.m_ReplyCode.ToString() + " " + this.m_Text + "\r\n";
			}
			else
			{
				result = this.m_ReplyCode.ToString() + "-" + this.m_Text + "\r\n";
			}
			return result;
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x0600146C RID: 5228 RVA: 0x0007F770 File Offset: 0x0007E770
		public int ReplyCode
		{
			get
			{
				return this.m_ReplyCode;
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x0600146D RID: 5229 RVA: 0x0007F788 File Offset: 0x0007E788
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x0600146E RID: 5230 RVA: 0x0007F7A0 File Offset: 0x0007E7A0
		public bool IsLastLine
		{
			get
			{
				return this.m_IsLastLine;
			}
		}

		// Token: 0x040007F0 RID: 2032
		private int m_ReplyCode = 0;

		// Token: 0x040007F1 RID: 2033
		private string m_Text = null;

		// Token: 0x040007F2 RID: 2034
		private bool m_IsLastLine = true;
	}
}
