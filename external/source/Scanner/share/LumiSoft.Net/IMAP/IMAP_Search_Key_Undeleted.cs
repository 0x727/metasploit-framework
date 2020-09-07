using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F2 RID: 498
	public class IMAP_Search_Key_Undeleted : IMAP_Search_Key
	{
		// Token: 0x060011DF RID: 4575 RVA: 0x0006C844 File Offset: 0x0006B844
		internal static IMAP_Search_Key_Undeleted Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UNDELETED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UNDELETED' key.");
			}
			return new IMAP_Search_Key_Undeleted();
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x0006C898 File Offset: 0x0006B898
		public override string ToString()
		{
			return "UNDELETED";
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x0006C8B0 File Offset: 0x0006B8B0
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
