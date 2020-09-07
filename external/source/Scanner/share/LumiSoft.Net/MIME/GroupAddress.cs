using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x0200015D RID: 349
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class GroupAddress : Address
	{
		// Token: 0x06000E07 RID: 3591 RVA: 0x0005719E File Offset: 0x0005619E
		public GroupAddress() : base(true)
		{
			this.m_pGroupMembers = new MailboxAddressCollection();
			this.m_pGroupMembers.Owner = this;
		}

		// Token: 0x06000E08 RID: 3592 RVA: 0x000571D4 File Offset: 0x000561D4
		public static GroupAddress Parse(string group)
		{
			GroupAddress groupAddress = new GroupAddress();
			string[] array = TextUtils.SplitQuotedString(group, ':');
			bool flag = array.Length > -1;
			if (flag)
			{
				groupAddress.DisplayName = TextUtils.UnQuoteString(array[0]);
			}
			bool flag2 = array.Length > 1;
			if (flag2)
			{
				groupAddress.GroupMembers.Parse(array[1]);
			}
			return groupAddress;
		}

		// Token: 0x06000E09 RID: 3593 RVA: 0x00057230 File Offset: 0x00056230
		internal void OnChanged()
		{
			bool flag = base.Owner != null;
			if (flag)
			{
				bool flag2 = base.Owner is AddressList;
				if (flag2)
				{
					((AddressList)base.Owner).OnCollectionChanged();
				}
			}
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x00057274 File Offset: 0x00056274
		public string GroupString
		{
			get
			{
				return TextUtils.QuoteString(this.DisplayName) + ":" + this.GroupMembers.ToMailboxListString() + ";";
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000572AC File Offset: 0x000562AC
		// (set) Token: 0x06000E0C RID: 3596 RVA: 0x000572C4 File Offset: 0x000562C4
		public string DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
				this.OnChanged();
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06000E0D RID: 3597 RVA: 0x000572D8 File Offset: 0x000562D8
		public MailboxAddressCollection GroupMembers
		{
			get
			{
				return this.m_pGroupMembers;
			}
		}

		// Token: 0x040005EB RID: 1515
		private string m_DisplayName = "";

		// Token: 0x040005EC RID: 1516
		private MailboxAddressCollection m_pGroupMembers = null;
	}
}
