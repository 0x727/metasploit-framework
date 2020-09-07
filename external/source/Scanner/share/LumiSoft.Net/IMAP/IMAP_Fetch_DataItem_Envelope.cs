using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C8 RID: 456
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Envelope : IMAP_Fetch_DataItem
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x00069550 File Offset: 0x00068550
		public override string ToString()
		{
			return "ENVELOPE";
		}
	}
}
