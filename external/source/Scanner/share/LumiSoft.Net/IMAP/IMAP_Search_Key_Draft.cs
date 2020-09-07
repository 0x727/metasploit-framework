using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001DA RID: 474
	public class IMAP_Search_Key_Draft : IMAP_Search_Key
	{
		// Token: 0x0600116A RID: 4458 RVA: 0x0006AE2C File Offset: 0x00069E2C
		internal static IMAP_Search_Key_Draft Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "DRAFT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'DRAFT' key.");
			}
			return new IMAP_Search_Key_Draft();
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x0006AE80 File Offset: 0x00069E80
		public override string ToString()
		{
			return "DRAFT";
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0006AE98 File Offset: 0x00069E98
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
