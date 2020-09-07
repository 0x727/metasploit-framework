using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C4 RID: 452
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Fetch_DataItem_Body : IMAP_Fetch_DataItem
	{
		// Token: 0x06001110 RID: 4368 RVA: 0x0006929D File Offset: 0x0006829D
		public IMAP_Fetch_DataItem_Body()
		{
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x000692BC File Offset: 0x000682BC
		public IMAP_Fetch_DataItem_Body(string section, int offset, int maxCount)
		{
			this.m_Section = section;
			this.m_Offset = offset;
			this.m_MaxCount = maxCount;
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x000692F0 File Offset: 0x000682F0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BODY[");
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

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x000693A8 File Offset: 0x000683A8
		public string Section
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x000693C0 File Offset: 0x000683C0
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x000693D8 File Offset: 0x000683D8
		public int MaxCount
		{
			get
			{
				return this.m_MaxCount;
			}
		}

		// Token: 0x040006D2 RID: 1746
		private string m_Section = null;

		// Token: 0x040006D3 RID: 1747
		private int m_Offset = -1;

		// Token: 0x040006D4 RID: 1748
		private int m_MaxCount = -1;
	}
}
