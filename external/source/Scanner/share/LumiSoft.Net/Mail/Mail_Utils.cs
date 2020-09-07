using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000183 RID: 387
	public class Mail_Utils
	{
		// Token: 0x06000FA2 RID: 4002 RVA: 0x00060870 File Offset: 0x0005F870
		internal static string SMTP_Mailbox(MIME_Reader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag2 = reader.Peek(true) == 34;
			if (flag2)
			{
				stringBuilder.Append("\"" + reader.QuotedString() + "\"");
			}
			else
			{
				stringBuilder.Append(reader.DotAtom());
			}
			bool flag3 = reader.Peek(true) != 64;
			string result;
			if (flag3)
			{
				result = null;
			}
			else
			{
				reader.Char(true);
				stringBuilder.Append('@');
				stringBuilder.Append(reader.DotAtom());
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
