using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001EC RID: 492
	public class IMAP_Search_Key_Smaller : IMAP_Search_Key
	{
		// Token: 0x060011C0 RID: 4544 RVA: 0x0006C140 File Offset: 0x0006B140
		public IMAP_Search_Key_Smaller(int value)
		{
			this.m_Value = value;
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x0006C158 File Offset: 0x0006B158
		internal static IMAP_Search_Key_Smaller Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SMALLER", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SMALLER' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SMALLER' value.");
			}
			int value = 0;
			bool flag4 = !int.TryParse(text, out value);
			if (flag4)
			{
				throw new ParseException("Parse error: Invalid 'SMALLER' value.");
			}
			return new IMAP_Search_Key_Smaller(value);
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x0006C1EC File Offset: 0x0006B1EC
		public override string ToString()
		{
			return "SMALLER " + this.m_Value;
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x0006C214 File Offset: 0x0006B214
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x0006C24C File Offset: 0x0006B24C
		public int Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040006F7 RID: 1783
		private int m_Value = 0;
	}
}
