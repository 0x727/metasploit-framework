using System;
using System.Text;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x0200022F RID: 559
	public class IMAP_Client_SelectedFolder
	{
		// Token: 0x0600144F RID: 5199 RVA: 0x0007F12C File Offset: 0x0007E12C
		public IMAP_Client_SelectedFolder(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("The argument 'name' value must be specified.", "name");
			}
			this.m_Name = name;
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0007F1CC File Offset: 0x0007E1CC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Name: " + this.Name);
			stringBuilder.AppendLine("UidValidity: " + this.UidValidity);
			stringBuilder.AppendLine("Flags: " + this.StringArrayToString(this.Flags));
			stringBuilder.AppendLine("PermanentFlags: " + this.StringArrayToString(this.PermanentFlags));
			stringBuilder.AppendLine("IsReadOnly: " + this.IsReadOnly.ToString());
			stringBuilder.AppendLine("UidNext: " + this.UidNext);
			stringBuilder.AppendLine("FirstUnseen: " + this.FirstUnseen);
			stringBuilder.AppendLine("MessagesCount: " + this.MessagesCount);
			stringBuilder.AppendLine("RecentMessagesCount: " + this.RecentMessagesCount);
			return stringBuilder.ToString();
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0007F2E6 File Offset: 0x0007E2E6
		internal void SetUidValidity(long value)
		{
			this.m_UidValidity = value;
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0007F2F0 File Offset: 0x0007E2F0
		internal void SetFlags(string[] value)
		{
			this.m_pFlags = value;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0007F2FA File Offset: 0x0007E2FA
		internal void SetPermanentFlags(string[] value)
		{
			this.m_pPermanentFlags = value;
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0007F304 File Offset: 0x0007E304
		internal void SetReadOnly(bool value)
		{
			this.m_IsReadOnly = value;
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0007F30E File Offset: 0x0007E30E
		internal void SetUidNext(long value)
		{
			this.m_UidNext = value;
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0007F318 File Offset: 0x0007E318
		internal void SetFirstUnseen(int value)
		{
			this.m_FirstUnseen = value;
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0007F322 File Offset: 0x0007E322
		internal void SetMessagesCount(int value)
		{
			this.m_MessagesCount = value;
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0007F32C File Offset: 0x0007E32C
		internal void SetRecentMessagesCount(int value)
		{
			this.m_RecentMessagesCount = value;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0007F338 File Offset: 0x0007E338
		private string StringArrayToString(string[] value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				bool flag = i == value.Length - 1;
				if (flag)
				{
					stringBuilder.Append(value[i]);
				}
				else
				{
					stringBuilder.Append(value[i] + ",");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x0007F39C File Offset: 0x0007E39C
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x0600145B RID: 5211 RVA: 0x0007F3B4 File Offset: 0x0007E3B4
		public long UidValidity
		{
			get
			{
				return this.m_UidValidity;
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x0007F3CC File Offset: 0x0007E3CC
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x0007F3E4 File Offset: 0x0007E3E4
		public string[] PermanentFlags
		{
			get
			{
				return this.m_pPermanentFlags;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x0007F3FC File Offset: 0x0007E3FC
		public bool IsReadOnly
		{
			get
			{
				return this.m_IsReadOnly;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x0007F414 File Offset: 0x0007E414
		public long UidNext
		{
			get
			{
				return this.m_UidNext;
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x0007F42C File Offset: 0x0007E42C
		public int FirstUnseen
		{
			get
			{
				return this.m_FirstUnseen;
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x0007F444 File Offset: 0x0007E444
		public int MessagesCount
		{
			get
			{
				return this.m_MessagesCount;
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x0007F45C File Offset: 0x0007E45C
		public int RecentMessagesCount
		{
			get
			{
				return this.m_RecentMessagesCount;
			}
		}

		// Token: 0x040007E6 RID: 2022
		private string m_Name = "";

		// Token: 0x040007E7 RID: 2023
		private long m_UidValidity = -1L;

		// Token: 0x040007E8 RID: 2024
		private string[] m_pFlags = new string[0];

		// Token: 0x040007E9 RID: 2025
		private string[] m_pPermanentFlags = new string[0];

		// Token: 0x040007EA RID: 2026
		private bool m_IsReadOnly = false;

		// Token: 0x040007EB RID: 2027
		private long m_UidNext = -1L;

		// Token: 0x040007EC RID: 2028
		private int m_FirstUnseen = -1;

		// Token: 0x040007ED RID: 2029
		private int m_MessagesCount = 0;

		// Token: 0x040007EE RID: 2030
		private int m_RecentMessagesCount = 0;
	}
}
