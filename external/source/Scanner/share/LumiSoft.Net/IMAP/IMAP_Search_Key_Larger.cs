using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DF RID: 479
	public class IMAP_Search_Key_Larger : IMAP_Search_Key
	{
		// Token: 0x06001181 RID: 4481 RVA: 0x0006B39C File Offset: 0x0006A39C
		public IMAP_Search_Key_Larger(int value)
		{
			this.m_Value = value;
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x0006B3B4 File Offset: 0x0006A3B4
		internal static IMAP_Search_Key_Larger Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "LARGER", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'LARGER' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'LARGER' value.");
			}
			int value = 0;
			bool flag4 = !int.TryParse(text, out value);
			if (flag4)
			{
				throw new ParseException("Parse error: Invalid 'LARGER' value.");
			}
			return new IMAP_Search_Key_Larger(value);
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x0006B448 File Offset: 0x0006A448
		public override string ToString()
		{
			return "LARGER " + this.m_Value;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x0006B470 File Offset: 0x0006A470
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06001185 RID: 4485 RVA: 0x0006B4A8 File Offset: 0x0006A4A8
		public int Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006ED RID: 1773
		private int m_Value = 0;
	}
}
