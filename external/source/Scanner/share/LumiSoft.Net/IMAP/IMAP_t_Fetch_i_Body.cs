using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001A5 RID: 421
	public class IMAP_t_Fetch_i_Body : IMAP_t_Fetch_i
	{
		// Token: 0x060010A8 RID: 4264 RVA: 0x0006792C File Offset: 0x0006692C
		public IMAP_t_Fetch_i_Body()
		{
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x0006794B File Offset: 0x0006694B
		public IMAP_t_Fetch_i_Body(string section, int offset, int maxCount)
		{
			this.m_Section = section;
			this.m_Offset = offset;
			this.m_MaxCount = maxCount;
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x00067980 File Offset: 0x00066980
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

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060010AB RID: 4267 RVA: 0x00067A38 File Offset: 0x00066A38
		public string Section
		{
			get
			{
				return this.m_Section;
			}
		}

		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x060010AC RID: 4268 RVA: 0x00067A50 File Offset: 0x00066A50
		public int Offset
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x060010AD RID: 4269 RVA: 0x00067A68 File Offset: 0x00066A68
		public int MaxCount
		{
			get
			{
				return this.m_MaxCount;
			}
		}

		// Token: 0x040006AC RID: 1708
		private string m_Section = null;

		// Token: 0x040006AD RID: 1709
		private int m_Offset = -1;

		// Token: 0x040006AE RID: 1710
		private int m_MaxCount = -1;
	}
}
