using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E5 RID: 485
	public class IMAP_Search_Key_Recent : IMAP_Search_Key
	{
		// Token: 0x0600119F RID: 4511 RVA: 0x0006B9D0 File Offset: 0x0006A9D0
		internal static IMAP_Search_Key_Recent Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "RECENT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'RECENT' key.");
			}
			return new IMAP_Search_Key_Recent();
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x0006BA24 File Offset: 0x0006AA24
		public override string ToString()
		{
			return "RECENT";
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x0006BA3C File Offset: 0x0006AA3C
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
