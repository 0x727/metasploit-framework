using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D0 RID: 464
	public class IMAP_Acl_Entry
	{
		// Token: 0x0600112E RID: 4398 RVA: 0x00069610 File Offset: 0x00068610
		public IMAP_Acl_Entry(string identifier, string rights)
		{
			bool flag = identifier == null;
			if (flag)
			{
				throw new ArgumentNullException("identifier");
			}
			bool flag2 = identifier == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'identifier' value must be specified.", "identifier");
			}
			bool flag3 = rights == null;
			if (flag3)
			{
				throw new ArgumentNullException("rights");
			}
			this.m_Identifier = identifier;
			this.m_Rights = rights;
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x0600112F RID: 4399 RVA: 0x00069694 File Offset: 0x00068694
		public string Identifier
		{
			get
			{
				return this.m_Identifier;
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x000696AC File Offset: 0x000686AC
		public string Rights
		{
			get
			{
				return this.m_Rights;
			}
		}

		// Token: 0x040006D8 RID: 1752
		private string m_Identifier = "";

		// Token: 0x040006D9 RID: 1753
		private string m_Rights = "";
	}
}
