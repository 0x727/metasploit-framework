using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CB RID: 459
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Rfc822 : IMAP_Fetch_DataItem
	{
		// Token: 0x06001125 RID: 4389 RVA: 0x00069598 File Offset: 0x00068598
		public override string ToString()
		{
			return "RFC822";
		}
	}
}
