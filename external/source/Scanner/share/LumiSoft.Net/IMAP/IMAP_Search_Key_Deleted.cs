using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D9 RID: 473
	public class IMAP_Search_Key_Deleted : IMAP_Search_Key
	{
		// Token: 0x06001166 RID: 4454 RVA: 0x0006AD88 File Offset: 0x00069D88
		internal static IMAP_Search_Key_Deleted Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "DELETED", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'DELETED' key.");
			}
			return new IMAP_Search_Key_Deleted();
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0006ADDC File Offset: 0x00069DDC
		public override string ToString()
		{
			return "DELETED";
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0006ADF4 File Offset: 0x00069DF4
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
