using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000222 RID: 546
	public class IMAP_e_Started : EventArgs
	{
		// Token: 0x0600137C RID: 4988 RVA: 0x00077B44 File Offset: 0x00076B44
		internal IMAP_e_Started(IMAP_r_u_ServerStatus response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x00077B7C File Offset: 0x00076B7C
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x00077B94 File Offset: 0x00076B94
		public IMAP_r_u_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				this.m_pResponse = value;
			}
		}

		// Token: 0x040007A6 RID: 1958
		private IMAP_r_u_ServerStatus m_pResponse = null;
	}
}
