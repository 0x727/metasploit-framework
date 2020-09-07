using System;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D3 RID: 467
	public class IMAP_Search_Key_Group : IMAP_Search_Key
	{
		// Token: 0x06001148 RID: 4424 RVA: 0x0006A644 File Offset: 0x00069644
		public IMAP_Search_Key_Group()
		{
			this.m_pKeys = new List<IMAP_Search_Key>();
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x0006A660 File Offset: 0x00069660
		public static IMAP_Search_Key_Group Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			bool flag2 = r.StartsWith("(");
			if (flag2)
			{
				r = new StringReader(r.ReadParenthesized());
			}
			IMAP_Search_Key_Group imap_Search_Key_Group = new IMAP_Search_Key_Group();
			r.ReadToFirstChar();
			while (r.Available > 0L)
			{
				imap_Search_Key_Group.m_pKeys.Add(IMAP_Search_Key.ParseKey(r));
			}
			return imap_Search_Key_Group;
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0006A6D8 File Offset: 0x000696D8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			for (int i = 0; i < this.m_pKeys.Count; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(this.m_pKeys[i].ToString());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0006A75C File Offset: 0x0006975C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, "("));
			for (int i = 0; i < this.m_pKeys.Count; i++)
			{
				bool flag2 = i > 0;
				if (flag2)
				{
					list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, " "));
				}
				this.m_pKeys[i].ToCmdParts(list);
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, ")"));
		}

		// Token: 0x170005D9 RID: 1497
		// (get) Token: 0x0600114C RID: 4428 RVA: 0x0006A7F0 File Offset: 0x000697F0
		public List<IMAP_Search_Key> Keys
		{
			get
			{
				return this.m_pKeys;
			}
		}

		// Token: 0x040006E4 RID: 1764
		private List<IMAP_Search_Key> m_pKeys = null;
	}
}
