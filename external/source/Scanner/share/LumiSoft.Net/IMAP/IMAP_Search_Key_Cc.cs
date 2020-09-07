using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D8 RID: 472
	public class IMAP_Search_Key_Cc : IMAP_Search_Key
	{
		// Token: 0x06001160 RID: 4448 RVA: 0x0006AC50 File Offset: 0x00069C50
		public IMAP_Search_Key_Cc(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0006AC8C File Offset: 0x00069C8C
		internal static IMAP_Search_Key_Cc Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "CC", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'CC' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'CC' value.");
			}
			return new IMAP_Search_Key_Cc(text);
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0006AD00 File Offset: 0x00069D00
		public override string ToString()
		{
			return "CC " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0006AD28 File Offset: 0x00069D28
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "CC "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06001164 RID: 4452 RVA: 0x0006AD70 File Offset: 0x00069D70
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006E8 RID: 1768
		private string m_Value = "";
	}
}
