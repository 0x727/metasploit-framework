using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000215 RID: 533
	public class IMAP_e_GetQuotaRoot : EventArgs
	{
		// Token: 0x0600132A RID: 4906 RVA: 0x00076D00 File Offset: 0x00075D00
		internal IMAP_e_GetQuotaRoot(string folder, IMAP_r_ServerStatus response)
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
			this.m_pQuotaRootResponses = new List<IMAP_r_u_QuotaRoot>();
			this.m_pQuotaResponses = new List<IMAP_r_u_Quota>();
		}

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x0600132B RID: 4907 RVA: 0x00076D80 File Offset: 0x00075D80
		public List<IMAP_r_u_QuotaRoot> QuotaRootResponses
		{
			get
			{
				return this.m_pQuotaRootResponses;
			}
		}

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x00076D98 File Offset: 0x00075D98
		public List<IMAP_r_u_Quota> QuotaResponses
		{
			get
			{
				return this.m_pQuotaResponses;
			}
		}

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x0600132D RID: 4909 RVA: 0x00076DB0 File Offset: 0x00075DB0
		// (set) Token: 0x0600132E RID: 4910 RVA: 0x00076DC8 File Offset: 0x00075DC8
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

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x0600132F RID: 4911 RVA: 0x00076DF4 File Offset: 0x00075DF4
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x04000778 RID: 1912
		private List<IMAP_r_u_QuotaRoot> m_pQuotaRootResponses = null;

		// Token: 0x04000779 RID: 1913
		private List<IMAP_r_u_Quota> m_pQuotaResponses = null;

		// Token: 0x0400077A RID: 1914
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x0400077B RID: 1915
		private string m_Folder = null;
	}
}
