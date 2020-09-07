using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001AB RID: 427
	public class IMAP_t_Fetch_i_BodyPeek : IMAP_t_Fetch_i
	{
		// Token: 0x060010B7 RID: 4279 RVA: 0x00067AF3 File Offset: 0x00066AF3
		public IMAP_t_Fetch_i_BodyPeek()
		{
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x00067B12 File Offset: 0x00066B12
		public IMAP_t_Fetch_i_BodyPeek(string section, int offset, int maxCount)
		{
			this.m_Section = section;
			this.m_Offset = offset;
			this.m_MaxCount = maxCount;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x00067B48 File Offset: 0x00066B48
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

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x00067C00 File Offset: 0x00066C00
		public string Section
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x060010BB RID: 4283 RVA: 0x00067C18 File Offset: 0x00066C18
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x00067C30 File Offset: 0x00066C30
		public int MaxCount
		{
			get
			{
				return this.m_MaxCount;
			}
		}

		// Token: 0x040006AF RID: 1711
		private string m_Section = null;

		// Token: 0x040006B0 RID: 1712
		private int m_Offset = -1;

		// Token: 0x040006B1 RID: 1713
		private int m_MaxCount = -1;
	}
}
