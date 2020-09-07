using System;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x0200016B RID: 363
	public class DeliveryAddress
	{
		// Token: 0x06000EC8 RID: 3784 RVA: 0x0005C540 File Offset: 0x0005B540
		internal DeliveryAddress(Item item, DeliveryAddressType_enum addressType, string postOfficeAddress, string extendedAddress, string street, string locality, string region, string postalCode, string country)
		{
			this.m_pItem = item;
			this.m_Type = addressType;
			this.m_PostOfficeAddress = postOfficeAddress;
			this.m_ExtendedAddress = extendedAddress;
			this.m_Street = street;
			this.m_Locality = locality;
			this.m_Region = region;
			this.m_PostalCode = postalCode;
			this.m_Country = country;
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0005C5F8 File Offset: 0x0005B5F8
		private void Changed()
		{
			string value = string.Concat(new string[]
			{
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_PostOfficeAddress),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_ExtendedAddress),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_Street),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_Locality),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_Region),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_PostalCode),
				";",
				vCard_Utils.Encode(this.m_pItem.Owner.Version, this.m_pItem.Owner.Charset, this.m_Country)
			});
			this.m_pItem.ParametersString = DeliveryAddress.AddressTypeToString(this.m_Type);
			Item pItem = this.m_pItem;
			pItem.ParametersString += ";CHARSET=utf-8";
			this.m_pItem.Value = value;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0005C7CC File Offset: 0x0005B7CC
		internal static DeliveryAddress Parse(Item item)
		{
			DeliveryAddressType_enum deliveryAddressType_enum = DeliveryAddressType_enum.NotSpecified;
			bool flag = item.ParametersString.ToUpper().IndexOf("PREF") != -1;
			if (flag)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Preferred;
			}
			bool flag2 = item.ParametersString.ToUpper().IndexOf("DOM") != -1;
			if (flag2)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Domestic;
			}
			bool flag3 = item.ParametersString.ToUpper().IndexOf("INTL") != -1;
			if (flag3)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Ineternational;
			}
			bool flag4 = item.ParametersString.ToUpper().IndexOf("POSTAL") != -1;
			if (flag4)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Postal;
			}
			bool flag5 = item.ParametersString.ToUpper().IndexOf("PARCEL") != -1;
			if (flag5)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Parcel;
			}
			bool flag6 = item.ParametersString.ToUpper().IndexOf("HOME") != -1;
			if (flag6)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Home;
			}
			bool flag7 = item.ParametersString.ToUpper().IndexOf("WORK") != -1;
			if (flag7)
			{
				deliveryAddressType_enum |= DeliveryAddressType_enum.Work;
			}
			string[] array = item.DecodedValue.Split(new char[]
			{
				';'
			});
			return new DeliveryAddress(item, deliveryAddressType_enum, (array.Length >= 1) ? array[0] : "", (array.Length >= 2) ? array[1] : "", (array.Length >= 3) ? array[2] : "", (array.Length >= 4) ? array[3] : "", (array.Length >= 5) ? array[4] : "", (array.Length >= 6) ? array[5] : "", (array.Length >= 7) ? array[6] : "");
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0005C980 File Offset: 0x0005B980
		internal static string AddressTypeToString(DeliveryAddressType_enum type)
		{
			string text = "";
			bool flag = (type & DeliveryAddressType_enum.Domestic) > DeliveryAddressType_enum.NotSpecified;
			if (flag)
			{
				text += "DOM,";
			}
			bool flag2 = (type & DeliveryAddressType_enum.Home) > DeliveryAddressType_enum.NotSpecified;
			if (flag2)
			{
				text += "HOME,";
			}
			bool flag3 = (type & DeliveryAddressType_enum.Ineternational) > DeliveryAddressType_enum.NotSpecified;
			if (flag3)
			{
				text += "INTL,";
			}
			bool flag4 = (type & DeliveryAddressType_enum.Parcel) > DeliveryAddressType_enum.NotSpecified;
			if (flag4)
			{
				text += "PARCEL,";
			}
			bool flag5 = (type & DeliveryAddressType_enum.Postal) > DeliveryAddressType_enum.NotSpecified;
			if (flag5)
			{
				text += "POSTAL,";
			}
			bool flag6 = (type & DeliveryAddressType_enum.Preferred) > DeliveryAddressType_enum.NotSpecified;
			if (flag6)
			{
				text += "Preferred,";
			}
			bool flag7 = (type & DeliveryAddressType_enum.Work) > DeliveryAddressType_enum.NotSpecified;
			if (flag7)
			{
				text += "Work,";
			}
			bool flag8 = text.EndsWith(",");
			if (flag8)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06000ECC RID: 3788 RVA: 0x0005CA74 File Offset: 0x0005BA74
		public Item Item
		{
			get
			{
				return this.m_pItem;
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06000ECD RID: 3789 RVA: 0x0005CA8C File Offset: 0x0005BA8C
		// (set) Token: 0x06000ECE RID: 3790 RVA: 0x0005CAA4 File Offset: 0x0005BAA4
		public DeliveryAddressType_enum AddressType
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
				this.Changed();
			}
		}

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06000ECF RID: 3791 RVA: 0x0005CAB8 File Offset: 0x0005BAB8
		// (set) Token: 0x06000ED0 RID: 3792 RVA: 0x0005CAD0 File Offset: 0x0005BAD0
		public string PostOfficeAddress
		{
			get
			{
				return this.m_PostOfficeAddress;
			}
			set
			{
				this.m_PostOfficeAddress = value;
				this.Changed();
			}
		}

		// Token: 0x170004E9 RID: 1257
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x0005CAE4 File Offset: 0x0005BAE4
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x0005CAFC File Offset: 0x0005BAFC
		public string ExtendedAddress
		{
			get
			{
				return this.m_ExtendedAddress;
			}
			set
			{
				this.m_ExtendedAddress = value;
				this.Changed();
			}
		}

		// Token: 0x170004EA RID: 1258
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x0005CB10 File Offset: 0x0005BB10
		// (set) Token: 0x06000ED4 RID: 3796 RVA: 0x0005CB28 File Offset: 0x0005BB28
		public string Street
		{
			get
			{
				return this.m_Street;
			}
			set
			{
				this.m_Street = value;
				this.Changed();
			}
		}

		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06000ED5 RID: 3797 RVA: 0x0005CB3C File Offset: 0x0005BB3C
		// (set) Token: 0x06000ED6 RID: 3798 RVA: 0x0005CB54 File Offset: 0x0005BB54
		public string Locality
		{
			get
			{
				return this.m_Locality;
			}
			set
			{
				this.m_Locality = value;
				this.Changed();
			}
		}

		// Token: 0x170004EC RID: 1260
		// (get) Token: 0x06000ED7 RID: 3799 RVA: 0x0005CB68 File Offset: 0x0005BB68
		// (set) Token: 0x06000ED8 RID: 3800 RVA: 0x0005CB80 File Offset: 0x0005BB80
		public string Region
		{
			get
			{
				return this.m_Region;
			}
			set
			{
				this.m_Region = value;
				this.Changed();
			}
		}

		// Token: 0x170004ED RID: 1261
		// (get) Token: 0x06000ED9 RID: 3801 RVA: 0x0005CB94 File Offset: 0x0005BB94
		// (set) Token: 0x06000EDA RID: 3802 RVA: 0x0005CBAC File Offset: 0x0005BBAC
		public string PostalCode
		{
			get
			{
				return this.m_PostalCode;
			}
			set
			{
				this.m_PostalCode = value;
				this.Changed();
			}
		}

		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0005CBC0 File Offset: 0x0005BBC0
		// (set) Token: 0x06000EDC RID: 3804 RVA: 0x0005CBD8 File Offset: 0x0005BBD8
		public string Country
		{
			get
			{
				return this.m_Country;
			}
			set
			{
				this.m_Country = value;
				this.Changed();
			}
		}

		// Token: 0x04000619 RID: 1561
		private Item m_pItem = null;

		// Token: 0x0400061A RID: 1562
		private DeliveryAddressType_enum m_Type = (DeliveryAddressType_enum)92;

		// Token: 0x0400061B RID: 1563
		private string m_PostOfficeAddress = "";

		// Token: 0x0400061C RID: 1564
		private string m_ExtendedAddress = "";

		// Token: 0x0400061D RID: 1565
		private string m_Street = "";

		// Token: 0x0400061E RID: 1566
		private string m_Locality = "";

		// Token: 0x0400061F RID: 1567
		private string m_Region = "";

		// Token: 0x04000620 RID: 1568
		private string m_PostalCode = "";

		// Token: 0x04000621 RID: 1569
		private string m_Country = "";
	}
}
