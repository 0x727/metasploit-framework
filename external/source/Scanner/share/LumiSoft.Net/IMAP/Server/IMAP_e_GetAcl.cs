using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000214 RID: 532
	public class IMAP_e_GetAcl : EventArgs
	{
		// Token: 0x06001325 RID: 4901 RVA: 0x00076C20 File Offset: 0x00075C20
		internal IMAP_e_GetAcl(string folder, IMAP_r_ServerStatus response)
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
			this.m_pAclResponses = new List<IMAP_r_u_Acl>();
		}

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06001326 RID: 4902 RVA: 0x00076C8C File Offset: 0x00075C8C
		public List<IMAP_r_u_Acl> AclResponses
		{
			get
			{
				return this.m_pAclResponses;
			}
		}

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x00076CA4 File Offset: 0x00075CA4
		// (set) Token: 0x06001328 RID: 4904 RVA: 0x00076CBC File Offset: 0x00075CBC
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

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06001329 RID: 4905 RVA: 0x00076CE8 File Offset: 0x00075CE8
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x04000775 RID: 1909
		private List<IMAP_r_u_Acl> m_pAclResponses = null;

		// Token: 0x04000776 RID: 1910
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000777 RID: 1911
		private string m_Folder = null;
	}
}
