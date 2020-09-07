using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001ED RID: 493
	public class IMAP_Search_Key_Subject : IMAP_Search_Key
	{
		// Token: 0x060011C5 RID: 4549 RVA: 0x0006C264 File Offset: 0x0006B264
		public IMAP_Search_Key_Subject(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x0006C2A0 File Offset: 0x0006B2A0
		internal static IMAP_Search_Key_Subject Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SUBJECT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SUBJECT' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SUBJECT' value.");
			}
			return new IMAP_Search_Key_Subject(text);
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x0006C314 File Offset: 0x0006B314
		public override string ToString()
		{
			return "SUBJECT " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0006C33C File Offset: 0x0006B33C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "SUBJECT "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x060011C9 RID: 4553 RVA: 0x0006C384 File Offset: 0x0006B384
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006F8 RID: 1784
		private string m_Value = "";
	}
}
