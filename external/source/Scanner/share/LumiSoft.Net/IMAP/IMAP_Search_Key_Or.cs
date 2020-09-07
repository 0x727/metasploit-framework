using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E4 RID: 484
	public class IMAP_Search_Key_Or : IMAP_Search_Key
	{
		// Token: 0x06001198 RID: 4504 RVA: 0x0006B84C File Offset: 0x0006A84C
		public IMAP_Search_Key_Or(IMAP_Search_Key key1, IMAP_Search_Key key2)
		{
			bool flag = key1 == null;
			if (flag)
			{
				throw new ArgumentNullException("key1");
			}
			bool flag2 = key2 == null;
			if (flag2)
			{
				throw new ArgumentNullException("key2");
			}
			this.m_pSearchKey1 = key1;
			this.m_pSearchKey2 = key2;
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0006B8A8 File Offset: 0x0006A8A8
		internal static IMAP_Search_Key_Or Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "OR", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'OR' key.");
			}
			return new IMAP_Search_Key_Or(IMAP_Search_Key.ParseKey(r), IMAP_Search_Key.ParseKey(r));
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0006B908 File Offset: 0x0006A908
		public override string ToString()
		{
			return "OR " + this.m_pSearchKey1.ToString() + " " + this.m_pSearchKey2.ToString();
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x0006B940 File Offset: 0x0006A940
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "OR "));
			this.m_pSearchKey1.ToCmdParts(list);
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, " "));
			this.m_pSearchKey2.ToCmdParts(list);
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x0600119C RID: 4508 RVA: 0x0006B9A0 File Offset: 0x0006A9A0
		public IMAP_Search_Key SearchKey1
		{
			get
			{
				return this.m_pSearchKey1;
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x0600119D RID: 4509 RVA: 0x0006B9B8 File Offset: 0x0006A9B8
		public IMAP_Search_Key SearchKey2
		{
			get
			{
				return this.m_pSearchKey2;
			}
		}

		// Token: 0x040006F0 RID: 1776
		private IMAP_Search_Key m_pSearchKey1 = null;

		// Token: 0x040006F1 RID: 1777
		private IMAP_Search_Key m_pSearchKey2 = null;
	}
}
