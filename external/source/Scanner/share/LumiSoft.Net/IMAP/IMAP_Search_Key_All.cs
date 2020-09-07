using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000188 RID: 392
	public class IMAP_Search_Key_All : IMAP_Search_Key
	{
		// Token: 0x06001020 RID: 4128 RVA: 0x00064880 File Offset: 0x00063880
		internal static IMAP_Search_Key_All Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "ALL", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'ALL' key.");
			}
			return new IMAP_Search_Key_All();
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x000648D4 File Offset: 0x000638D4
		public override string ToString()
		{
			return "ALL";
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x000648EC File Offset: 0x000638EC
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
