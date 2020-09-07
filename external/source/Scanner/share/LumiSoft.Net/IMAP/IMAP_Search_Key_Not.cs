using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E1 RID: 481
	public class IMAP_Search_Key_Not : IMAP_Search_Key
	{
		// Token: 0x0600118A RID: 4490 RVA: 0x0006B564 File Offset: 0x0006A564
		public IMAP_Search_Key_Not(IMAP_Search_Key key)
		{
			bool flag = key == null;
			if (flag)
			{
				throw new ArgumentNullException("key");
			}
			this.m_pSearchKey = key;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x0006B59C File Offset: 0x0006A59C
		internal static IMAP_Search_Key_Not Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "NOT", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'NOT' key.");
			}
			return new IMAP_Search_Key_Not(IMAP_Search_Key.ParseKey(r));
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x0006B5F4 File Offset: 0x0006A5F4
		public override string ToString()
		{
			return "NOT " + this.m_pSearchKey.ToString();
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x0006B61C File Offset: 0x0006A61C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "NOT "));
			this.m_pSearchKey.ToCmdParts(list);
		}

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x0600118E RID: 4494 RVA: 0x0006B660 File Offset: 0x0006A660
		public IMAP_Search_Key SearchKey
		{
			get
			{
				return this.m_pSearchKey;
			}
		}

		// Token: 0x040006EE RID: 1774
		private IMAP_Search_Key m_pSearchKey = null;
	}
}
