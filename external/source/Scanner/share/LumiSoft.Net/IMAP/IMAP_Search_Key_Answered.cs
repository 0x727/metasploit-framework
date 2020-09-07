using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D4 RID: 468
	public class IMAP_Search_Key_Answered : IMAP_Search_Key
	{
		// Token: 0x0600114E RID: 4430 RVA: 0x0006A808 File Offset: 0x00069808
		internal static IMAP_Search_Key_Answered Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "ANSWERED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'ANSWERED' key.");
			}
			return new IMAP_Search_Key_Answered();
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0006A85C File Offset: 0x0006985C
		public override string ToString()
		{
			return "ANSWERED";
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0006A874 File Offset: 0x00069874
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
