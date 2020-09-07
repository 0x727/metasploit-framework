using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021C RID: 540
	public class IMAP_e_Namespace : EventArgs
	{
		// Token: 0x06001352 RID: 4946 RVA: 0x00077464 File Offset: 0x00076464
		internal IMAP_e_Namespace(IMAP_r_ServerStatus response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06001353 RID: 4947 RVA: 0x000774A4 File Offset: 0x000764A4
		// (set) Token: 0x06001354 RID: 4948 RVA: 0x000774BC File Offset: 0x000764BC
		public IMAP_r_u_Namespace NamespaceResponse
		{
			get
			{
				return this.m_pNamespaceResponse;
			}
			set
			{
				this.m_pNamespaceResponse = value;
			}
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06001355 RID: 4949 RVA: 0x000774C8 File Offset: 0x000764C8
		// (set) Token: 0x06001356 RID: 4950 RVA: 0x000774E0 File Offset: 0x000764E0
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

		// Token: 0x0400078E RID: 1934
		private IMAP_r_u_Namespace m_pNamespaceResponse = null;

		// Token: 0x0400078F RID: 1935
		private IMAP_r_ServerStatus m_pResponse = null;
	}
}
