using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000163 RID: 355
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MailboxAddressCollection : IEnumerable
	{
		// Token: 0x06000E41 RID: 3649 RVA: 0x000580BB File Offset: 0x000570BB
		public MailboxAddressCollection()
		{
			this.m_pMailboxes = new List<MailboxAddress>();
		}

		// Token: 0x06000E42 RID: 3650 RVA: 0x000580DE File Offset: 0x000570DE
		public void Add(MailboxAddress mailbox)
		{
			this.m_pMailboxes.Add(mailbox);
			this.OnCollectionChanged();
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x000580F5 File Offset: 0x000570F5
		public void Insert(int index, MailboxAddress mailbox)
		{
			this.m_pMailboxes.Insert(index, mailbox);
			this.OnCollectionChanged();
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x0005810D File Offset: 0x0005710D
		public void Remove(int index)
		{
			this.m_pMailboxes.RemoveAt(index);
			this.OnCollectionChanged();
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x00058124 File Offset: 0x00057124
		public void Remove(MailboxAddress mailbox)
		{
			this.m_pMailboxes.Remove(mailbox);
			this.OnCollectionChanged();
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x0005813B File Offset: 0x0005713B
		public void Clear()
		{
			this.m_pMailboxes.Clear();
			this.OnCollectionChanged();
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x00058154 File Offset: 0x00057154
		public void Parse(string mailboxList)
		{
			string[] array = TextUtils.SplitQuotedString(mailboxList, ',');
			foreach (string mailbox in array)
			{
				this.m_pMailboxes.Add(MailboxAddress.Parse(mailbox));
			}
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x00058194 File Offset: 0x00057194
		public string ToMailboxListString()
		{
			string text = "";
			for (int i = 0; i < this.m_pMailboxes.Count; i++)
			{
				bool flag = i == this.m_pMailboxes.Count - 1;
				if (flag)
				{
					text += this.m_pMailboxes[i].ToMailboxAddressString();
				}
				else
				{
					text = text + this.m_pMailboxes[i].ToMailboxAddressString() + ",\t";
				}
			}
			return text;
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x0005821C File Offset: 0x0005721C
		internal void OnCollectionChanged()
		{
			bool flag = this.m_pOwner != null;
			if (flag)
			{
				bool flag2 = this.m_pOwner is GroupAddress;
				if (flag2)
				{
					((GroupAddress)this.m_pOwner).OnChanged();
				}
			}
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x00058260 File Offset: 0x00057260
		public IEnumerator GetEnumerator()
		{
			return this.m_pMailboxes.GetEnumerator();
		}

		// Token: 0x170004BB RID: 1211
		public MailboxAddress this[int index]
		{
			get
			{
				return this.m_pMailboxes[index];
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06000E4C RID: 3660 RVA: 0x000582A4 File Offset: 0x000572A4
		public int Count
		{
			get
			{
				return this.m_pMailboxes.Count;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x000582C4 File Offset: 0x000572C4
		// (set) Token: 0x06000E4E RID: 3662 RVA: 0x000582DC File Offset: 0x000572DC
		internal Address Owner
		{
			get
			{
				return this.m_pOwner;
			}
			set
			{
				this.m_pOwner = value;
			}
		}

		// Token: 0x040005F5 RID: 1525
		private Address m_pOwner = null;

		// Token: 0x040005F6 RID: 1526
		private List<MailboxAddress> m_pMailboxes = null;
	}
}
