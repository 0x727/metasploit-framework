using System;
using System.Diagnostics;
using System.IO;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022E RID: 558
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Client_Fetch_Rfc822_EArgs : EventArgs
	{
		// Token: 0x06001449 RID: 5193 RVA: 0x0007F04F File Offset: 0x0007E04F
		internal IMAP_Client_Fetch_Rfc822_EArgs()
		{
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x0007F068 File Offset: 0x0007E068
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x0007F080 File Offset: 0x0007E080
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

		// Token: 0x1400008A RID: 138
		// (add) Token: 0x0600144C RID: 5196 RVA: 0x0007F08C File Offset: 0x0007E08C
		// (remove) Token: 0x0600144D RID: 5197 RVA: 0x0007F0C4 File Offset: 0x0007E0C4
		
		public event EventHandler StoringCompleted = null;

		// Token: 0x0600144E RID: 5198 RVA: 0x0007F0FC File Offset: 0x0007E0FC
		internal void OnStoringCompleted()
		{
			bool flag = this.StoringCompleted != null;
			if (flag)
			{
				this.StoringCompleted(this, new EventArgs());
			}
		}

		// Token: 0x040007E4 RID: 2020
		private Stream m_pStream = null;
	}
}
