using System;
using System.IO;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022A RID: 554
	public class IMAP_Client_e_FetchGetStoreStream : EventArgs
	{
		// Token: 0x060013A7 RID: 5031 RVA: 0x00078350 File Offset: 0x00077350
		public IMAP_Client_e_FetchGetStoreStream(IMAP_r_u_Fetch fetch, IMAP_t_Fetch_r_i dataItem)
		{
			bool flag = fetch == null;
			if (flag)
			{
				throw new ArgumentNullException("fetch");
			}
			bool flag2 = dataItem == null;
			if (flag2)
			{
				throw new ArgumentNullException("dataItem");
			}
			this.m_pFetchResponse = fetch;
			this.m_pDataItem = dataItem;
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x060013A8 RID: 5032 RVA: 0x000783B0 File Offset: 0x000773B0
		public IMAP_r_u_Fetch FetchResponse
		{
			get
			{
				return this.m_pFetchResponse;
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x060013A9 RID: 5033 RVA: 0x000783C8 File Offset: 0x000773C8
		public IMAP_t_Fetch_r_i DataItem
		{
			get
			{
				return this.m_pDataItem;
			}
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x060013AA RID: 5034 RVA: 0x000783E0 File Offset: 0x000773E0
		// (set) Token: 0x060013AB RID: 5035 RVA: 0x000783F8 File Offset: 0x000773F8
		public Stream Stream
		{
			get
			{
				return this.m_pStream;
			}
			set
			{
				this.m_pStream = value;
			}
		}

		// Token: 0x040007C4 RID: 1988
		private IMAP_r_u_Fetch m_pFetchResponse = null;

		// Token: 0x040007C5 RID: 1989
		private IMAP_t_Fetch_r_i m_pDataItem = null;

		// Token: 0x040007C6 RID: 1990
		private Stream m_pStream = null;
	}
}
