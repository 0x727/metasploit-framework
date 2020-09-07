using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021D RID: 541
	public class IMAP_e_GetQuota : EventArgs
	{
		// Token: 0x06001357 RID: 4951 RVA: 0x0007750C File Offset: 0x0007650C
		internal IMAP_e_GetQuota(string quotaRoot, IMAP_r_ServerStatus response)
		{
			bool flag = quotaRoot == null;
			if (flag)
			{
				throw new ArgumentNullException("quotaRoot");
			}
			this.m_QuotaRoot = quotaRoot;
			this.m_pResponse = response;
			this.m_pQuotaResponses = new List<IMAP_r_u_Quota>();
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06001358 RID: 4952 RVA: 0x00077564 File Offset: 0x00076564
		public List<IMAP_r_u_Quota> QuotaResponses
		{
			get
			{
				return this.m_pQuotaResponses;
			}
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06001359 RID: 4953 RVA: 0x0007757C File Offset: 0x0007657C
		// (set) Token: 0x0600135A RID: 4954 RVA: 0x00077594 File Offset: 0x00076594
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

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x0600135B RID: 4955 RVA: 0x000775C0 File Offset: 0x000765C0
		public string QuotaRoot
		{
			get
			{
				return this.m_QuotaRoot;
			}
		}

		// Token: 0x04000790 RID: 1936
		private List<IMAP_r_u_Quota> m_pQuotaResponses = null;

		// Token: 0x04000791 RID: 1937
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000792 RID: 1938
		private string m_QuotaRoot = null;
	}
}
