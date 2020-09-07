using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CE RID: 462
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Rfc822Text : IMAP_Fetch_DataItem
	{
		// Token: 0x0600112B RID: 4395 RVA: 0x000695E0 File Offset: 0x000685E0
		public override string ToString()
		{
			return "RFC822.TEXT";
		}
	}
}
