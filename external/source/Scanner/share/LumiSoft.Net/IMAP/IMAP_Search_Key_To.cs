using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001EF RID: 495
	public class IMAP_Search_Key_To : IMAP_Search_Key
	{
		// Token: 0x060011CF RID: 4559 RVA: 0x0006C4D4 File Offset: 0x0006B4D4
		public IMAP_Search_Key_To(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x0006C510 File Offset: 0x0006B510
		internal static IMAP_Search_Key_To Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "TO", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'TO' key.");
			}
			string text = IMAP_Utils.ReadString(r);
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'TO' value.");
			}
			return new IMAP_Search_Key_To(text);
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x0006C584 File Offset: 0x0006B584
		public override string ToString()
		{
			return "TO " + TextUtils.QuoteString(this.m_Value);
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x0006C5AC File Offset: 0x0006B5AC
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "TO "));
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String, this.m_Value));
		}

		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x060011D3 RID: 4563 RVA: 0x0006C5F4 File Offset: 0x0006B5F4
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006FA RID: 1786
		private string m_Value = "";
	}
}
