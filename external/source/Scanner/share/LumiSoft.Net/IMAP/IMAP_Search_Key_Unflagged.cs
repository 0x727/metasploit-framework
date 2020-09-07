using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F4 RID: 500
	public class IMAP_Search_Key_Unflagged : IMAP_Search_Key
	{
		// Token: 0x060011E7 RID: 4583 RVA: 0x0006C98C File Offset: 0x0006B98C
		internal static IMAP_Search_Key_Unflagged Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNFLAGGED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNFLAGGED' key.");
			}
			return new IMAP_Search_Key_Unflagged();
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x0006C9E0 File Offset: 0x0006B9E0
		public override string ToString()
		{
			return "UNFLAGGED";
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x0006C9F8 File Offset: 0x0006B9F8
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
