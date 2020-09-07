using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F6 RID: 502
	public class IMAP_Search_Key_Unseen : IMAP_Search_Key
	{
		// Token: 0x060011F0 RID: 4592 RVA: 0x0006CB54 File Offset: 0x0006BB54
		internal static IMAP_Search_Key_Unseen Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNSEEN", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNSEEN' key.");
			}
			return new IMAP_Search_Key_Unseen();
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x0006CBA8 File Offset: 0x0006BBA8
		public override string ToString()
		{
			return "UNSEEN";
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x0006CBC0 File Offset: 0x0006BBC0
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
