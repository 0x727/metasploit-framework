using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000175 RID: 373
	public class PhoneNumberCollection : IEnumerable
	{
		// Token: 0x06000F20 RID: 3872 RVA: 0x0005E2F8 File Offset: 0x0005D2F8
		internal PhoneNumberCollection(vCard owner)
		{
			this.m_pOwner = owner;
			this.m_pCollection = new List<PhoneNumber>();
			foreach (Item item in owner.Items.Get("TEL"))
			{
				this.m_pCollection.Add(PhoneNumber.Parse(item));
			}
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0005E368 File Offset: 0x0005D368
		public void Add(PhoneNumberType_enum type, string number)
		{
			Item item = this.m_pOwner.Items.Add("TEL", PhoneNumber.PhoneTypeToString(type), number);
			this.m_pCollection.Add(new PhoneNumber(item, type, number));
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0005E3A7 File Offset: 0x0005D3A7
		public void Remove(PhoneNumber item)
		{
			this.m_pOwner.Items.Remove(item.Item);
			this.m_pCollection.Remove(item);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x0005E3D0 File Offset: 0x0005D3D0
		public void Clear()
		{
			foreach (PhoneNumber phoneNumber in this.m_pCollection)
			{
				this.m_pOwner.Items.Remove(phoneNumber.Item);
			}
			this.m_pCollection.Clear();
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x0005E444 File Offset: 0x0005D444
		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0005E468 File Offset: 0x0005D468
		public int Count
		{
			get
			{
				return this.m_pCollection.Count;
			}
		}

		// Token: 0x04000646 RID: 1606
		private vCard m_pOwner = null;

		// Token: 0x04000647 RID: 1607
		private List<PhoneNumber> m_pCollection = null;
	}
}
