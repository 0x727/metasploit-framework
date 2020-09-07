using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x0200016C RID: 364
	public class DeliveryAddressCollection : IEnumerable
	{
		// Token: 0x06000EDD RID: 3805 RVA: 0x0005CBEC File Offset: 0x0005BBEC
		internal DeliveryAddressCollection(vCard owner)
		{
			this.m_pOwner = owner;
			this.m_pCollection = new List<DeliveryAddress>();
			foreach (Item item in owner.Items.Get("ADR"))
			{
				this.m_pCollection.Add(DeliveryAddress.Parse(item));
			}
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0005CC5C File Offset: 0x0005BC5C
		public void Add(DeliveryAddressType_enum type, string postOfficeAddress, string extendedAddress, string street, string locality, string region, string postalCode, string country)
		{
			string value = string.Concat(new string[]
			{
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, postOfficeAddress),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, extendedAddress),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, street),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, locality),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, region),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, postalCode),
				";",
				vCard_Utils.Encode(this.m_pOwner.Version, this.m_pOwner.Charset, country)
			});
			Item item = this.m_pOwner.Items.Add("ADR", DeliveryAddress.AddressTypeToString(type), "");
			item.FoldLongLines = false;
			bool flag = this.m_pOwner.Version.StartsWith("2");
			if (flag)
			{
				Item item2 = item;
				item2.ParametersString += ";ENCODING=QUOTED-PRINTABLE";
			}
			Item item3 = item;
			item3.ParametersString = item3.ParametersString + ";CHARSET=" + this.m_pOwner.Charset.WebName;
			item.Value = value;
			this.m_pCollection.Add(new DeliveryAddress(item, type, postOfficeAddress, extendedAddress, street, locality, region, postalCode, country));
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0005CE32 File Offset: 0x0005BE32
		public void Remove(DeliveryAddress item)
		{
			this.m_pOwner.Items.Remove(item.Item);
			this.m_pCollection.Remove(item);
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0005CE5C File Offset: 0x0005BE5C
		public void Clear()
		{
			foreach (DeliveryAddress deliveryAddress in this.m_pCollection)
			{
				this.m_pOwner.Items.Remove(deliveryAddress.Item);
			}
			this.m_pCollection.Clear();
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x0005CED0 File Offset: 0x0005BED0
		public IEnumerator GetEnumerator()
		{
			return this.m_pCollection.GetEnumerator();
		}

		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0005CEF4 File Offset: 0x0005BEF4
		public int Count
		{
			get
			{
				return this.m_pCollection.Count;
			}
		}

		// Token: 0x170004F0 RID: 1264
		public DeliveryAddress this[int index]
		{
			get
			{
				return this.m_pCollection[index];
			}
		}

		// Token: 0x04000622 RID: 1570
		private vCard m_pOwner = null;

		// Token: 0x04000623 RID: 1571
		private List<DeliveryAddress> m_pCollection = null;
	}
}
