using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200019B RID: 411
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_X_GM_MSGID : IMAP_Fetch_DataItem
	{
		// Token: 0x06001079 RID: 4217 RVA: 0x000660BC File Offset: 0x000650BC
		public override string ToString()
		{
			return "X-GM-MSGID";
		}
	}
}
