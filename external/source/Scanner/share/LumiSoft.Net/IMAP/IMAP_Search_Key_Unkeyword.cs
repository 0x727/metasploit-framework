using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F5 RID: 501
	public class IMAP_Search_Key_Unkeyword : IMAP_Search_Key
	{
		// Token: 0x060011EA RID: 4586 RVA: 0x0006CA30 File Offset: 0x0006BA30
		public IMAP_Search_Key_Unkeyword(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0006CA6C File Offset: 0x0006BA6C
		internal static IMAP_Search_Key_Unkeyword Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNKEYWORD", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNKEYWORD' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'UNKEYWORD' value.");
			}
			return new IMAP_Search_Key_Unkeyword(text);
		}

		// Token: 0x060011EC RID: 4588 RVA: 0x0006CAE0 File Offset: 0x0006BAE0
		public override string ToString()
		{
			return "UNKEYWORD " + this.m_Value;
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0006CB04 File Offset: 0x0006BB04
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x060011EE RID: 4590 RVA: 0x0006CB3C File Offset: 0x0006BB3C
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006FC RID: 1788
		private string m_Value = "";
	}
}
