using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CC RID: 460
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Rfc822Header : IMAP_Fetch_DataItem
	{
		// Token: 0x06001127 RID: 4391 RVA: 0x000695B0 File Offset: 0x000685B0
		public override string ToString()
		{
			return "RFC822.HEADER";
		}
	}
}
