using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DC RID: 476
	public class IMAP_Search_Key_From : IMAP_Search_Key
	{
		// Token: 0x06001171 RID: 4465 RVA: 0x0006AF74 File Offset: 0x00069F74
		public IMAP_Search_Key_From(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0006AFB0 File Offset: 0x00069FB0
		internal static IMAP_Search_Key_From Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "FROM", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'FROM' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'FROM' value.");
			}
			return new IMAP_Search_Key_From(text);
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0006B024 File Offset: 0x0006A024
		public override string ToString()
		{
			return "FROM " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x0006B04C File Offset: 0x0006A04C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "FROM "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005DE RID: 1502
		// (get) Token: 0x06001175 RID: 4469 RVA: 0x0006B094 File Offset: 0x0006A094
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006E9 RID: 1769
		private string m_Value = "";
	}
}
