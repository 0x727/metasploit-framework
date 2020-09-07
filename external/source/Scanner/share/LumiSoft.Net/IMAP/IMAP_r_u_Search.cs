using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001A3 RID: 419
	public class IMAP_r_u_Search : IMAP_r_u
	{
		// Token: 0x060010A3 RID: 4259 RVA: 0x000677B4 File Offset: 0x000667B4
		public IMAP_r_u_Search(int[] values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			this.m_pValues = values;
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x000677EC File Offset: 0x000667EC
		public static IMAP_r_u_Search Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			List<int> list = new List<int>();
			bool flag2 = response.Split(new char[]
			{
				' '
			}).Length > 2;
			if (flag2)
			{
				foreach (string value in response.Split(new char[]
				{
					' '
				}, 3)[2].Trim().Split(new char[]
				{
					' '
				}))
				{
					bool flag3 = !string.IsNullOrEmpty(value);
					if (flag3)
					{
						list.Add(Convert.ToInt32(value));
					}
				}
			}
			return new IMAP_r_u_Search(list.ToArray());
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x000678A8 File Offset: 0x000668A8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* SEARCH");
			foreach (int num in this.m_pValues)
			{
				stringBuilder.Append(" " + num.ToString());
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x00067914 File Offset: 0x00066914
		public int[] Values
		{
			get
			{
				return this.m_pValues;
			}
		}

		// Token: 0x040006AB RID: 1707
		private int[] m_pValues = null;
	}
}
