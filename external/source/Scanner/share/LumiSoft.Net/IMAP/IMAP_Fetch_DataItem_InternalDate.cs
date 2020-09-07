using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001CA RID: 458
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_InternalDate : IMAP_Fetch_DataItem
	{
		// Token: 0x06001123 RID: 4387 RVA: 0x00069580 File Offset: 0x00068580
		public override string ToString()
		{
			return "INTERNALDATE";
		}
	}
}
