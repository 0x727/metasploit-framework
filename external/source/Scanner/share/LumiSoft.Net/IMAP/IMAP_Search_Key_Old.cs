using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E2 RID: 482
	public class IMAP_Search_Key_Old : IMAP_Search_Key
	{
		// Token: 0x06001190 RID: 4496 RVA: 0x0006B678 File Offset: 0x0006A678
		internal static IMAP_Search_Key_Old Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "OLD", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'OLD' key.");
			}
			return new IMAP_Search_Key_Old();
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x0006B6CC File Offset: 0x0006A6CC
		public override string ToString()
		{
			return "OLD";
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x0006B6E4 File Offset: 0x0006A6E4
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
