using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200019C RID: 412
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_X_GM_THRID : IMAP_Fetch_DataItem
	{
		// Token: 0x0600107B RID: 4219 RVA: 0x000660D4 File Offset: 0x000650D4
		public override string ToString()
		{
			return "X-GM-THRID";
		}
	}
}
