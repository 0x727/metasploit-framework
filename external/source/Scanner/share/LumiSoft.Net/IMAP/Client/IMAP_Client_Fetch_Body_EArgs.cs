using System;
using System.Diagnostics;
using System.IO;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022D RID: 557
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Client_Fetch_Body_EArgs : EventArgs
	{
		// Token: 0x06001441 RID: 5185 RVA: 0x0007EF28 File Offset: 0x0007DF28
		internal IMAP_Client_Fetch_Body_EArgs(string bodySection, int offset)
		{
			this.m_Section = bodySection;
			this.m_Offset = offset;
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x0007EF5C File Offset: 0x0007DF5C
		public string BodySection
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x0007EF74 File Offset: 0x0007DF74
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x0007EF8C File Offset: 0x0007DF8C
		// (set) Token: 0x06001445 RID: 5189 RVA: 0x0007EFA4 File Offset: 0x0007DFA4
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

		// Token: 0x14000089 RID: 137
		// (add) Token: 0x06001446 RID: 5190 RVA: 0x0007EFB0 File Offset: 0x0007DFB0
		// (remove) Token: 0x06001447 RID: 5191 RVA: 0x0007EFE8 File Offset: 0x0007DFE8
		
		public event EventHandler StoringCompleted = null;

		// Token: 0x06001448 RID: 5192 RVA: 0x0007F020 File Offset: 0x0007E020
		internal void OnStoringCompleted()
		{
			bool flag = this.StoringCompleted != null;
			if (flag)
			{
				this.StoringCompleted(this, new EventArgs());
			}
		}

		// Token: 0x040007E0 RID: 2016
		private string m_Section = null;

		// Token: 0x040007E1 RID: 2017
		private int m_Offset = -1;

		// Token: 0x040007E2 RID: 2018
		private Stream m_pStream = null;
	}
}
