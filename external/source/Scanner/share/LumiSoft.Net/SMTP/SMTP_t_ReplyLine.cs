using System;

namespace LumiSoft.Net.SMTP
{
	// Token: 0x02000131 RID: 305
	public class SMTP_t_ReplyLine
	{
		// Token: 0x06000C27 RID: 3111 RVA: 0x0004AB58 File Offset: 0x00049B58
		public SMTP_t_ReplyLine(int replyCode, string text, bool isLastLine)
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

		// Token: 0x06000C28 RID: 3112 RVA: 0x0004ABA8 File Offset: 0x00049BA8
		public static SMTP_t_ReplyLine Parse(string line)
		{
			bool flag = line == null;
			if (flag)
			{
				throw new ArgumentNullException("line");
			}
			bool flag2 = line.Length < 3;
			if (flag2)
			{
				throw new ParseException("Invalid SMTP server reply-line '" + line + "'.");
			}
			int replyCode = 0;
			bool flag3 = !int.TryParse(line.Substring(0, 3), out replyCode);
			if (flag3)
			{
				throw new ParseException("Invalid SMTP server reply-line '" + line + "' reply-code.");
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
			return new SMTP_t_ReplyLine(replyCode, text, isLastLine);
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x0004AC70 File Offset: 0x00049C70
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

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x0004ACD4 File Offset: 0x00049CD4
		public int ReplyCode
		{
			get
			{
				return this.m_ReplyCode;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000C2B RID: 3115 RVA: 0x0004ACEC File Offset: 0x00049CEC
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x0004AD04 File Offset: 0x00049D04
		public bool IsLastLine
		{
			get
			{
				return this.m_IsLastLine;
			}
		}

		// Token: 0x04000505 RID: 1285
		private int m_ReplyCode = 0;

		// Token: 0x04000506 RID: 1286
		private string m_Text = null;

		// Token: 0x04000507 RID: 1287
		private bool m_IsLastLine = true;
	}
}
