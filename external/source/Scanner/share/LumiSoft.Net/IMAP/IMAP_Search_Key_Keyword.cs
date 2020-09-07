using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DE RID: 478
	public class IMAP_Search_Key_Keyword : IMAP_Search_Key
	{
		// Token: 0x0600117C RID: 4476 RVA: 0x0006B278 File Offset: 0x0006A278
		public IMAP_Search_Key_Keyword(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x0006B2B4 File Offset: 0x0006A2B4
		internal static IMAP_Search_Key_Keyword Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "KEYWORD", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'KEYWORD' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'KEYWORD' value.");
			}
			return new IMAP_Search_Key_Keyword(text);
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0006B328 File Offset: 0x0006A328
		public override string ToString()
		{
			return "KEYWORD " + this.m_Value;
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x0006B34C File Offset: 0x0006A34C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001180 RID: 4480 RVA: 0x0006B384 File Offset: 0x0006A384
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006EC RID: 1772
		private string m_Value = "";
	}
}
