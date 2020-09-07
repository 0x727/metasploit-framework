using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CF RID: 463
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Uid : IMAP_Fetch_DataItem
	{
		// Token: 0x0600112D RID: 4397 RVA: 0x000695F8 File Offset: 0x000685F8
		public override string ToString()
		{
			return "UID";
		}
	}
}
