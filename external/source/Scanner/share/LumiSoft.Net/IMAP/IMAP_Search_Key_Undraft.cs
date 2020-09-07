using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F3 RID: 499
	public class IMAP_Search_Key_Undraft : IMAP_Search_Key
	{
		// Token: 0x060011E3 RID: 4579 RVA: 0x0006C8E8 File Offset: 0x0006B8E8
		internal static IMAP_Search_Key_Undraft Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNDRAFT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNDRAFT' key.");
			}
			return new IMAP_Search_Key_Undraft();
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x0006C93C File Offset: 0x0006B93C
		public override string ToString()
		{
			return "UNDRAFT";
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x0006C954 File Offset: 0x0006B954
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
