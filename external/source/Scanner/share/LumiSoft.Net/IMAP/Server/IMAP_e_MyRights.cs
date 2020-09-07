using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021B RID: 539
	public class IMAP_e_MyRights : EventArgs
	{
		// Token: 0x0600134C RID: 4940 RVA: 0x00077384 File Offset: 0x00076384
		internal IMAP_e_MyRights(string folder, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			this.m_Folder = folder;
			this.m_pResponse = response;
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x0600134D RID: 4941 RVA: 0x000773E4 File Offset: 0x000763E4
		// (set) Token: 0x0600134E RID: 4942 RVA: 0x000773FC File Offset: 0x000763FC
		public IMAP_r_u_MyRights MyRightsResponse
		{
			get
			{
				return this.m_pMyRightsResponse;
			}
			set
			{
				this.m_pMyRightsResponse = value;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x0600134F RID: 4943 RVA: 0x00077408 File Offset: 0x00076408
		// (set) Token: 0x06001350 RID: 4944 RVA: 0x00077420 File Offset: 0x00076420
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

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001351 RID: 4945 RVA: 0x0007744C File Offset: 0x0007644C
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x0400078B RID: 1931
		private IMAP_r_u_MyRights m_pMyRightsResponse = null;

		// Token: 0x0400078C RID: 1932
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x0400078D RID: 1933
		private string m_Folder = null;
	}
}
