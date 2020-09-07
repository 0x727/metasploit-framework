using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C5 RID: 453
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_BodyPeek : IMAP_Fetch_DataItem
	{
		// Token: 0x06001116 RID: 4374 RVA: 0x000693F0 File Offset: 0x000683F0
		public IMAP_Fetch_DataItem_BodyPeek()
		{
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0006940F File Offset: 0x0006840F
		public IMAP_Fetch_DataItem_BodyPeek(string section, int offset, int maxCount)
		{
			this.m_Section = section;
			this.m_Offset = offset;
			this.m_MaxCount = maxCount;
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00069444 File Offset: 0x00068444
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BODY.PEEK[");
			bool flag = this.m_Section != null;
			if (flag)
			{
				stringBuilder.Append(this.m_Section);
			}
			stringBuilder.Append("]");
			bool flag2 = this.m_Offset > -1;
			if (flag2)
			{
				stringBuilder.Append("<" + this.m_Offset);
				bool flag3 = this.m_MaxCount > -1;
				if (flag3)
				{
					stringBuilder.Append("." + this.m_MaxCount);
				}
				stringBuilder.Append(">");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x06001119 RID: 4377 RVA: 0x000694FC File Offset: 0x000684FC
		public string Section
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x0600111A RID: 4378 RVA: 0x00069514 File Offset: 0x00068514
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x0600111B RID: 4379 RVA: 0x0006952C File Offset: 0x0006852C
		public int MaxCount
		{
			get
			{
				return this.m_MaxCount;
			}
		}

		// Token: 0x040006D5 RID: 1749
		private string m_Section = null;

		// Token: 0x040006D6 RID: 1750
		private int m_Offset = -1;

		// Token: 0x040006D7 RID: 1751
		private int m_MaxCount = -1;
	}
}
