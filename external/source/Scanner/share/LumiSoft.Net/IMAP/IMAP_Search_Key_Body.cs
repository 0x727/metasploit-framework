using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D7 RID: 471
	public class IMAP_Search_Key_Body : IMAP_Search_Key
	{
		// Token: 0x0600115B RID: 4443 RVA: 0x0006AB18 File Offset: 0x00069B18
		public IMAP_Search_Key_Body(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0006AB54 File Offset: 0x00069B54
		internal static IMAP_Search_Key_Body Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "BODY", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'BODY' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'BODY' value.");
			}
			return new IMAP_Search_Key_Body(text);
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0006ABC8 File Offset: 0x00069BC8
		public override string ToString()
		{
			return "BODY " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0006ABF0 File Offset: 0x00069BF0
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "BODY "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x0600115F RID: 4447 RVA: 0x0006AC38 File Offset: 0x00069C38
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006E7 RID: 1767
		private string m_Value = "";
	}
}
