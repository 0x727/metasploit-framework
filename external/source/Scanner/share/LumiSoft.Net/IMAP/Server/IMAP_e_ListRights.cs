using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000217 RID: 535
	public class IMAP_e_ListRights : EventArgs
	{
		// Token: 0x06001334 RID: 4916 RVA: 0x00076E8C File Offset: 0x00075E8C
		internal IMAP_e_ListRights(string folder, string identifier, IMAP_r_ServerStatus response)
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
			this.m_Folder = folder;
			this.m_Identifier = identifier;
			this.m_pResponse = response;
		}

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x00076F10 File Offset: 0x00075F10
		// (set) Token: 0x06001336 RID: 4918 RVA: 0x00076F28 File Offset: 0x00075F28
		public IMAP_r_u_ListRights ListRightsResponse
		{
			get
			{
				return this.m_pListRightsResponse;
			}
			set
			{
				this.m_pListRightsResponse = value;
			}
		}

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x00076F34 File Offset: 0x00075F34
		// (set) Token: 0x06001338 RID: 4920 RVA: 0x00076F4C File Offset: 0x00075F4C
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

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x00076F78 File Offset: 0x00075F78
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x00076F90 File Offset: 0x00075F90
		public string Identifier
		{
			get
			{
				return this.m_Identifier;
			}
		}

		// Token: 0x0400077F RID: 1919
		private IMAP_r_u_ListRights m_pListRightsResponse = null;

		// Token: 0x04000780 RID: 1920
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000781 RID: 1921
		private string m_Folder = null;

		// Token: 0x04000782 RID: 1922
		private string m_Identifier = null;
	}
}
