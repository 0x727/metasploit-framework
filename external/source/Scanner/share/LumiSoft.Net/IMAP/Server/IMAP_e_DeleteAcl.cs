using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200020F RID: 527
	public class IMAP_e_DeleteAcl : EventArgs
	{
		// Token: 0x06001306 RID: 4870 RVA: 0x0007664C File Offset: 0x0007564C
		internal IMAP_e_DeleteAcl(string folder, string identifier, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = identifier == null;
			if (flag2)
			{
				throw new ArgumentNullException("identifier");
			}
			bool flag3 = response == null;
			if (flag3)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
			this.m_Folder = folder;
			this.m_Identifier = identifier;
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x000766C8 File Offset: 0x000756C8
		// (set) Token: 0x06001308 RID: 4872 RVA: 0x000766E0 File Offset: 0x000756E0
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pResponse = value;
			}
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x0007670C File Offset: 0x0007570C
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x0600130A RID: 4874 RVA: 0x00076724 File Offset: 0x00075724
		public string Identifier
		{
			get
			{
				return this.m_Identifier;
			}
		}

		// Token: 0x04000764 RID: 1892
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000765 RID: 1893
		private string m_Folder = null;

		// Token: 0x04000766 RID: 1894
		private string m_Identifier = null;
	}
}
