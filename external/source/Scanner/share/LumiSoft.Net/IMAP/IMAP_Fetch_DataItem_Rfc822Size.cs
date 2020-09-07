using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CD RID: 461
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Rfc822Size : IMAP_Fetch_DataItem
	{
		// Token: 0x06001129 RID: 4393 RVA: 0x000695C8 File Offset: 0x000685C8
		public override string ToString()
		{
			return "RFC822.SIZE";
		}
	}
}
