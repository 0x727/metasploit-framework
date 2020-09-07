using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DB RID: 475
	public class IMAP_Search_Key_Flagged : IMAP_Search_Key
	{
		// Token: 0x0600116E RID: 4462 RVA: 0x0006AED0 File Offset: 0x00069ED0
		internal static IMAP_Search_Key_Flagged Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "FLAGGED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'FLAGGED' key.");
			}
			return new IMAP_Search_Key_Flagged();
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0006AF24 File Offset: 0x00069F24
		public override string ToString()
		{
			return "FLAGGED";
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0006AF3C File Offset: 0x00069F3C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}
	}
}
