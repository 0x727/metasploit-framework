using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.Mime
{
	// Token: 0x0200015A RID: 346
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class AddressList : IEnumerable
	{
		// Token: 0x06000DF8 RID: 3576 RVA: 0x00056CC6 File Offset: 0x00055CC6
		public AddressList()
		{
			this.m_pAddresses = new List<Address>();
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x00056CE9 File Offset: 0x00055CE9
		public void Add(Address address)
		{
			address.Owner = this;
			this.m_pAddresses.Add(address);
			this.OnCollectionChanged();
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x00056D08 File Offset: 0x00055D08
		public void Insert(int index, Address address)
		{
			address.Owner = this;
			this.m_pAddresses.Insert(index, address);
			this.OnCollectionChanged();
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x00056D28 File Offset: 0x00055D28
		public void Remove(int index)
		{
			this.Remove(this.m_pAddresses[index]);
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x00056D3E File Offset: 0x00055D3E
		public void Remove(Address address)
		{
			address.Owner = null;
			this.m_pAddresses.Remove(address);
			this.OnCollectionChanged();
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x00056D60 File Offset: 0x00055D60
		public void Clear()
		{
			foreach (Address address in this.m_pAddresses)
			{
				address.Owner = null;
			}
			this.m_pAddresses.Clear();
			this.OnCollectionChanged();
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x00056DCC File Offset: 0x00055DCC
		public void Parse(string addressList)
		{
			addressList = addressList.Trim();
			StringReader stringReader = new StringReader(addressList);
			while (stringReader.SourceString.Length > 0)
			{
				int num = TextUtils.QuotedIndexOf(stringReader.SourceString, ',');
				int num2 = TextUtils.QuotedIndexOf(stringReader.SourceString, ':');
				bool flag = num2 == -1 || (num < num2 && num != -1);
				if (flag)
				{
					MailboxAddress mailboxAddress = MailboxAddress.Parse(stringReader.QuotedReadToDelimiter(','));
					this.m_pAddresses.Add(mailboxAddress);
					mailboxAddress.Owner = this;
				}
				else
				{
					GroupAddress groupAddress = GroupAddress.Parse(stringReader.QuotedReadToDelimiter(';'));
					this.m_pAddresses.Add(groupAddress);
					groupAddress.Owner = this;
					bool flag2 = stringReader.SourceString.Length > 0;
					if (flag2)
					{
						stringReader.QuotedReadToDelimiter(',');
					}
				}
			}
			this.OnCollectionChanged();
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x00056EB4 File Offset: 0x00055EB4
		public string ToAddressListString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_pAddresses.Count; i++)
			{
				bool flag = this.m_pAddresses[i] is MailboxAddress;
				if (flag)
				{
					bool flag2 = i == this.m_pAddresses.Count - 1;
					if (flag2)
					{
						stringBuilder.Append(((MailboxAddress)this.m_pAddresses[i]).ToMailboxAddressString());
					}
					else
					{
						stringBuilder.Append(((MailboxAddress)this.m_pAddresses[i]).ToMailboxAddressString() + ",\t");
					}
				}
				else
				{
					bool flag3 = this.m_pAddresses[i] is GroupAddress;
					if (flag3)
					{
						bool flag4 = i == this.m_pAddresses.Count - 1;
						if (flag4)
						{
							stringBuilder.Append(((GroupAddress)this.m_pAddresses[i]).GroupString);
						}
						else
						{
							stringBuilder.Append(((GroupAddress)this.m_pAddresses[i]).GroupString + ",\t");
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x00056FF0 File Offset: 0x00055FF0
		internal void OnCollectionChanged()
		{
			bool flag = this.m_HeaderField != null;
			if (flag)
			{
				this.m_HeaderField.Value = this.ToAddressListString();
			}
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00057020 File Offset: 0x00056020
		public IEnumerator GetEnumerator()
		{
			return this.m_pAddresses.GetEnumerator();
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06000E02 RID: 3586 RVA: 0x00057044 File Offset: 0x00056044
		public MailboxAddress[] Mailboxes
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				foreach (object obj in this)
				{
					Address address = (Address)obj;
					bool flag = !address.IsGroupAddress;
					if (flag)
					{
						arrayList.Add((MailboxAddress)address);
					}
					else
					{
						foreach (object obj2 in ((GroupAddress)address).GroupMembers)
						{
							MailboxAddress value = (MailboxAddress)obj2;
							arrayList.Add(value);
						}
					}
				}
				MailboxAddress[] array = new MailboxAddress[arrayList.Count];
				arrayList.CopyTo(array);
				return array;
			}
		}

		// Token: 0x170004A7 RID: 1191
		public Address this[int index]
		{
			get
			{
				return this.m_pAddresses[index];
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x0005715C File Offset: 0x0005615C
		public int Count
		{
			get
			{
				return this.m_pAddresses.Count;
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0005717C File Offset: 0x0005617C
		// (set) Token: 0x06000E06 RID: 3590 RVA: 0x00057194 File Offset: 0x00056194
		internal HeaderField BoundedHeaderField
		{
			get
			{
				return this.m_HeaderField;
			}
			set
			{
				this.m_HeaderField = value;
			}
		}

		// Token: 0x040005DC RID: 1500
		private HeaderField m_HeaderField = null;

		// Token: 0x040005DD RID: 1501
		private List<Address> m_pAddresses = null;
	}
}
