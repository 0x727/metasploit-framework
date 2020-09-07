using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D5 RID: 469
	public class IMAP_Search_Key_Bcc : IMAP_Search_Key
	{
		// Token: 0x06001151 RID: 4433 RVA: 0x0006A8AC File Offset: 0x000698AC
		public IMAP_Search_Key_Bcc(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0006A8E8 File Offset: 0x000698E8
		internal static IMAP_Search_Key_Bcc Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "BCC", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'BCC' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'BCC' value.");
			}
			return new IMAP_Search_Key_Bcc(text);
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0006A95C File Offset: 0x0006995C
		public override string ToString()
		{
			return "BCC " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0006A984 File Offset: 0x00069984
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "BCC "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005DA RID: 1498
		// (get) Token: 0x06001155 RID: 4437 RVA: 0x0006A9CC File Offset: 0x000699CC
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006E5 RID: 1765
		private string m_Value = "";
	}
}
