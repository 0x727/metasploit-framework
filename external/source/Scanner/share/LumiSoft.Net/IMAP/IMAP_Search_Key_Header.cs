using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DD RID: 477
	public class IMAP_Search_Key_Header : IMAP_Search_Key
	{
		// Token: 0x06001176 RID: 4470 RVA: 0x0006B0AC File Offset: 0x0006A0AC
		public IMAP_Search_Key_Header(string fieldName, string value)
		{
			bool flag = fieldName == null;
			if (flag)
			{
				throw new ArgumentNullException("fieldName");
			}
			bool flag2 = value == null;
			if (flag2)
			{
				throw new ArgumentNullException("value");
			}
			this.m_FieldName = fieldName;
			this.m_Value = value;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0006B110 File Offset: 0x0006A110
		internal static IMAP_Search_Key_Header Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "HEADER", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'HEADER' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'HEADER' field-name value.");
			}
			string text2 = IMAP_Utils.ReadString(r);
			bool flag4 = text2 == null;
			if (flag4)
			{
				throw new ParseException("Parse error: Invalid 'HEADER' string value.");
			}
			return new IMAP_Search_Key_Header(text, text2);
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0006B1A4 File Offset: 0x0006A1A4
		public override string ToString()
		{
			return "HEADER " + TextUtils.QuoteString(this.m_FieldName) + " " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0006B1DC File Offset: 0x0006A1DC
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "HEADER "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_FieldName));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, " "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x0600117A RID: 4474 RVA: 0x0006B248 File Offset: 0x0006A248
		public string FieldName
		{
			get
			{
				return this.m_FieldName;
			}
		}

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x0600117B RID: 4475 RVA: 0x0006B260 File Offset: 0x0006A260
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006EA RID: 1770
		private string m_FieldName = "";

		// Token: 0x040006EB RID: 1771
		private string m_Value = "";
	}
}
