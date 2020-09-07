using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001EE RID: 494
	public class IMAP_Search_Key_Text : IMAP_Search_Key
	{
		// Token: 0x060011CA RID: 4554 RVA: 0x0006C39C File Offset: 0x0006B39C
		public IMAP_Search_Key_Text(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0006C3D8 File Offset: 0x0006B3D8
		internal static IMAP_Search_Key_Text Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "TEXT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'TEXT' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'TEXT' value.");
			}
			return new IMAP_Search_Key_Text(text);
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0006C44C File Offset: 0x0006B44C
		public override string ToString()
		{
			return "TEXT " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0006C474 File Offset: 0x0006B474
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "TEXT "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x0006C4BC File Offset: 0x0006B4BC
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006F9 RID: 1785
		private string m_Value = "";
	}
}
