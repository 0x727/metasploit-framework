using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x0200016F RID: 367
	public class EmailAddressCollection : IEnumerable
	{
		// Token: 0x06000EED RID: 3821 RVA: 0x0005D124 File Offset: 0x0005C124
		internal EmailAddressCollection(vCard owner)
		{
			this.m_pOwner = owner;
			this.m_pCollection = new List<EmailAddress>();
			foreach (Item item in owner.Items.Get("EMAIL"))
			{
				this.m_pCollection.Add(EmailAddress.Parse(item));
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x0005D194 File Offset: 0x0005C194
		public EmailAddress Add(EmailAddressType_enum type, string email)
		{
			Item item = this.m_pOwner.Items.Add("EMAIL", EmailAddress.EmailTypeToString(type), "");
			item.SetDecodedValue(email);
			EmailAddress emailAddress = new EmailAddress(item, type, email);
			this.m_pCollection.Add(emailAddress);
			return emailAddress;
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x0005D1E6 File Offset: 0x0005C1E6
		public void Remove(EmailAddress item)
		{
			this.m_pOwner.Items.Remove(item.Item);
			this.m_pCollection.Remove(item);
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x0005D210 File Offset: 0x0005C210
		public void Clear()
		{
			foreach (EmailAddress emailAddress in this.m_pCollection)
			{
				this.m_pOwner.Items.Remove(emailAddress.Item);
			}
			this.m_pCollection.Clear();
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0005D284 File Offset: 0x0005C284
		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0005D2A8 File Offset: 0x0005C2A8
		public int Count
		{
			get
			{
				return this.m_pCollection.Count;
			}
		}

		// Token: 0x170004F5 RID: 1269
		public EmailAddress this[int index]
		{
			get
			{
				return this.m_pCollection[index];
			}
		}

		// Token: 0x04000630 RID: 1584
		private vCard m_pOwner = null;

		// Token: 0x04000631 RID: 1585
		private List<EmailAddress> m_pCollection = null;
	}
}
