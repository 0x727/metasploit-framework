using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000205 RID: 517
	public class IMAP_r_u_Quota : IMAP_r_u
	{
		// Token: 0x06001248 RID: 4680 RVA: 0x0006E6BC File Offset: 0x0006D6BC
		public IMAP_r_u_Quota(string quotaRootName, IMAP_Quota_Entry[] entries)
		{
			bool flag = quotaRootName == null;
			if (flag)
			{
				throw new ArgumentNullException("quotaRootName");
			}
			bool flag2 = entries == null;
			if (flag2)
			{
				throw new ArgumentNullException("entries");
			}
			this.m_QuotaRootName = quotaRootName;
			this.m_pEntries = entries;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0006E71C File Offset: 0x0006D71C
		public static IMAP_r_u_Quota Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string quotaRootName = stringReader.ReadWord();
			string[] array = stringReader.ReadParenthesized().Split(new char[]
			{
				' '
			});
			List<IMAP_Quota_Entry> list = new List<IMAP_Quota_Entry>();
			for (int i = 0; i < array.Length; i += 3)
			{
				list.Add(new IMAP_Quota_Entry(array[i], Convert.ToInt64(array[i + 1]), Convert.ToInt64(array[i + 2])));
			}
			return new IMAP_r_u_Quota(quotaRootName, list.ToArray());
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0006E7CC File Offset: 0x0006D7CC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* QUOTA \"" + this.m_QuotaRootName + "\" (");
			for (int i = 0; i < this.m_pEntries.Length; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(string.Concat(new object[]
				{
					this.m_pEntries[i].ResourceName,
					" ",
					this.m_pEntries[i].CurrentUsage,
					" ",
					this.m_pEntries[i].MaxUsage
				}));
			}
			stringBuilder.Append(")\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x0600124B RID: 4683 RVA: 0x0006E8A4 File Offset: 0x0006D8A4
		public string QuotaRootName
		{
			get
			{
				return this.m_QuotaRootName;
			}
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x0600124C RID: 4684 RVA: 0x0006E8BC File Offset: 0x0006D8BC
		public IMAP_Quota_Entry[] Entries
		{
			get
			{
				return this.m_pEntries;
			}
		}

		// Token: 0x0400071C RID: 1820
		private string m_QuotaRootName = "";

		// Token: 0x0400071D RID: 1821
		private IMAP_Quota_Entry[] m_pEntries = null;
	}
}
