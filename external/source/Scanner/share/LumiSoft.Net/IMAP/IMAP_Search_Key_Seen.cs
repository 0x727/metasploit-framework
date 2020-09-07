using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E6 RID: 486
	public class IMAP_Search_Key_Seen : IMAP_Search_Key
	{
		// Token: 0x060011A3 RID: 4515 RVA: 0x0006BA74 File Offset: 0x0006AA74
		internal static IMAP_Search_Key_Seen Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SEEN", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SEEN' key.");
			}
			return new IMAP_Search_Key_Seen();
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x0006BAC8 File Offset: 0x0006AAC8
		public override string ToString()
		{
			return "SEEN";
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x0006BAE0 File Offset: 0x0006AAE0
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
