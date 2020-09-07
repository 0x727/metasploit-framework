using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E0 RID: 480
	public class IMAP_Search_Key_New : IMAP_Search_Key
	{
		// Token: 0x06001187 RID: 4487 RVA: 0x0006B4C0 File Offset: 0x0006A4C0
		internal static IMAP_Search_Key_New Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "NEW", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'NEW' key.");
			}
			return new IMAP_Search_Key_New();
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x0006B514 File Offset: 0x0006A514
		public override string ToString()
		{
			return "NEW";
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x0006B52C File Offset: 0x0006A52C
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
