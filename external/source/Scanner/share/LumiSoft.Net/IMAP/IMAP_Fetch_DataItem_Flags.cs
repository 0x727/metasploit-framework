using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C9 RID: 457
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Flags : IMAP_Fetch_DataItem
	{
		// Token: 0x06001121 RID: 4385 RVA: 0x00069568 File Offset: 0x00068568
		public override string ToString()
		{
			return "FLAGS";
		}
	}
}
