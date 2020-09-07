using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F7 RID: 503
	public class IMAP_Namespace_Entry
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x0006CBF8 File Offset: 0x0006BBF8
		public IMAP_Namespace_Entry(string name, char delimiter)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			this.m_NamespaceName = name;
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x060011F4 RID: 4596 RVA: 0x0006CC3C File Offset: 0x0006BC3C
		public string NamespaceName
		{
			get
			{
				return this.m_NamespaceName;
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x0006CC54 File Offset: 0x0006BC54
		public char HierarchyDelimiter
		{
			get
			{
				return this.m_Delimiter;
			}
		}

		// Token: 0x040006FD RID: 1789
		private string m_NamespaceName = "";

		// Token: 0x040006FE RID: 1790
		private char m_Delimiter = '/';
	}
}
