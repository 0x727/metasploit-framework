using System;
using System.Diagnostics;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021F RID: 543
	public class IMAP_e_Search : EventArgs
	{
		// Token: 0x06001362 RID: 4962 RVA: 0x000776C8 File Offset: 0x000766C8
		internal IMAP_e_Search(IMAP_Search_Key criteria, IMAP_r_ServerStatus response)
		{
			bool flag = criteria == null;
			if (flag)
			{
				throw new ArgumentNullException("criteria");
			}
			this.m_pResponse = response;
			this.m_pCriteria = criteria;
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x00077714 File Offset: 0x00076714
		public void AddMessage(long uid)
		{
			this.OnMatched(uid);
		}

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06001364 RID: 4964 RVA: 0x00077720 File Offset: 0x00076720
		// (set) Token: 0x06001365 RID: 4965 RVA: 0x00077738 File Offset: 0x00076738
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

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001366 RID: 4966 RVA: 0x00077764 File Offset: 0x00076764
		public IMAP_Search_Key Criteria
		{
			get
			{
				return this.m_pCriteria;
			}
		}

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06001367 RID: 4967 RVA: 0x0007777C File Offset: 0x0007677C
		// (remove) Token: 0x06001368 RID: 4968 RVA: 0x000777B4 File Offset: 0x000767B4
		
		internal event EventHandler<EventArgs<long>> Matched = null;

		// Token: 0x06001369 RID: 4969 RVA: 0x000777EC File Offset: 0x000767EC
		private void OnMatched(long uid)
		{
			bool flag = this.Matched != null;
			if (flag)
			{
				this.Matched(this, new EventArgs<long>(uid));
			}
		}

		// Token: 0x04000797 RID: 1943
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000798 RID: 1944
		private IMAP_Search_Key m_pCriteria = null;
	}
}
