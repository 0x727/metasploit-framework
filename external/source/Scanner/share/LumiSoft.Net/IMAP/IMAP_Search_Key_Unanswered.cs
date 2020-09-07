using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F1 RID: 497
	public class IMAP_Search_Key_Unanswered : IMAP_Search_Key
	{
		// Token: 0x060011DB RID: 4571 RVA: 0x0006C7A0 File Offset: 0x0006B7A0
		internal static IMAP_Search_Key_Unanswered Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNANSWERED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNANSWERED' key.");
			}
			return new IMAP_Search_Key_Unanswered();
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x0006C7F4 File Offset: 0x0006B7F4
		public override string ToString()
		{
			return "UNANSWERED";
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x0006C80C File Offset: 0x0006B80C
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
