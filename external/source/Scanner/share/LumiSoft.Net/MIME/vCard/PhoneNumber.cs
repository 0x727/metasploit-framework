using System;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000174 RID: 372
	public class PhoneNumber
	{
		// Token: 0x06000F17 RID: 3863 RVA: 0x0005DDFC File Offset: 0x0005CDFC
		internal PhoneNumber(Item item, PhoneNumberType_enum type, string number)
		{
			this.m_pItem = item;
			this.m_Type = type;
			this.m_Number = number;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0005DE35 File Offset: 0x0005CE35
		private void Changed()
		{
			this.m_pItem.ParametersString = PhoneNumber.PhoneTypeToString(this.m_Type);
			this.m_pItem.Value = this.m_Number;
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0005DE64 File Offset: 0x0005CE64
		internal static PhoneNumber Parse(Item item)
		{
			PhoneNumberType_enum phoneNumberType_enum = PhoneNumberType_enum.NotSpecified;
			bool flag = item.ParametersString.ToUpper().IndexOf("PREF") != -1;
			if (flag)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Preferred;
			}
			bool flag2 = item.ParametersString.ToUpper().IndexOf("HOME") != -1;
			if (flag2)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Home;
			}
			bool flag3 = item.ParametersString.ToUpper().IndexOf("MSG") != -1;
			if (flag3)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Msg;
			}
			bool flag4 = item.ParametersString.ToUpper().IndexOf("WORK") != -1;
			if (flag4)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Work;
			}
			bool flag5 = item.ParametersString.ToUpper().IndexOf("VOICE") != -1;
			if (flag5)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Voice;
			}
			bool flag6 = item.ParametersString.ToUpper().IndexOf("FAX") != -1;
			if (flag6)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Fax;
			}
			bool flag7 = item.ParametersString.ToUpper().IndexOf("CELL") != -1;
			if (flag7)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Cellular;
			}
			bool flag8 = item.ParametersString.ToUpper().IndexOf("VIDEO") != -1;
			if (flag8)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Video;
			}
			bool flag9 = item.ParametersString.ToUpper().IndexOf("PAGER") != -1;
			if (flag9)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Pager;
			}
			bool flag10 = item.ParametersString.ToUpper().IndexOf("BBS") != -1;
			if (flag10)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.BBS;
			}
			bool flag11 = item.ParametersString.ToUpper().IndexOf("MODEM") != -1;
			if (flag11)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Modem;
			}
			bool flag12 = item.ParametersString.ToUpper().IndexOf("CAR") != -1;
			if (flag12)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.Car;
			}
			bool flag13 = item.ParametersString.ToUpper().IndexOf("ISDN") != -1;
			if (flag13)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.ISDN;
			}
			bool flag14 = item.ParametersString.ToUpper().IndexOf("PCS") != -1;
			if (flag14)
			{
				phoneNumberType_enum |= PhoneNumberType_enum.PCS;
			}
			return new PhoneNumber(item, phoneNumberType_enum, item.Value);
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0005E0C4 File Offset: 0x0005D0C4
		internal static string PhoneTypeToString(PhoneNumberType_enum type)
		{
			string text = "";
			bool flag = (type & PhoneNumberType_enum.BBS) > PhoneNumberType_enum.NotSpecified;
			if (flag)
			{
				text += "BBS,";
			}
			bool flag2 = (type & PhoneNumberType_enum.Car) > PhoneNumberType_enum.NotSpecified;
			if (flag2)
			{
				text += "CAR,";
			}
			bool flag3 = (type & PhoneNumberType_enum.Cellular) > PhoneNumberType_enum.NotSpecified;
			if (flag3)
			{
				text += "CELL,";
			}
			bool flag4 = (type & PhoneNumberType_enum.Fax) > PhoneNumberType_enum.NotSpecified;
			if (flag4)
			{
				text += "FAX,";
			}
			bool flag5 = (type & PhoneNumberType_enum.Home) > PhoneNumberType_enum.NotSpecified;
			if (flag5)
			{
				text += "HOME,";
			}
			bool flag6 = (type & PhoneNumberType_enum.ISDN) > PhoneNumberType_enum.NotSpecified;
			if (flag6)
			{
				text += "ISDN,";
			}
			bool flag7 = (type & PhoneNumberType_enum.Modem) > PhoneNumberType_enum.NotSpecified;
			if (flag7)
			{
				text += "MODEM,";
			}
			bool flag8 = (type & PhoneNumberType_enum.Msg) > PhoneNumberType_enum.NotSpecified;
			if (flag8)
			{
				text += "MSG,";
			}
			bool flag9 = (type & PhoneNumberType_enum.Pager) > PhoneNumberType_enum.NotSpecified;
			if (flag9)
			{
				text += "PAGER,";
			}
			bool flag10 = (type & PhoneNumberType_enum.PCS) > PhoneNumberType_enum.NotSpecified;
			if (flag10)
			{
				text += "PCS,";
			}
			bool flag11 = (type & PhoneNumberType_enum.Preferred) > PhoneNumberType_enum.NotSpecified;
			if (flag11)
			{
				text += "PREF,";
			}
			bool flag12 = (type & PhoneNumberType_enum.Video) > PhoneNumberType_enum.NotSpecified;
			if (flag12)
			{
				text += "VIDEO,";
			}
			bool flag13 = (type & PhoneNumberType_enum.Voice) > PhoneNumberType_enum.NotSpecified;
			if (flag13)
			{
				text += "VOICE,";
			}
			bool flag14 = (type & PhoneNumberType_enum.Work) > PhoneNumberType_enum.NotSpecified;
			if (flag14)
			{
				text += "WORK,";
			}
			bool flag15 = text.EndsWith(",");
			if (flag15)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0005E288 File Offset: 0x0005D288
		public Item Item
		{
			get
			{
				return this.m_pItem;
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0005E2A0 File Offset: 0x0005D2A0
		// (set) Token: 0x06000F1D RID: 3869 RVA: 0x0005E2B8 File Offset: 0x0005D2B8
		public PhoneNumberType_enum NumberType
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

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0005E2CC File Offset: 0x0005D2CC
		// (set) Token: 0x06000F1F RID: 3871 RVA: 0x0005E2E4 File Offset: 0x0005D2E4
		public string Number
		{
			get
			{
				return this.m_Number;
			}
			set
			{
				this.m_Number = value;
				this.Changed();
			}
		}

		// Token: 0x04000643 RID: 1603
		private Item m_pItem = null;

		// Token: 0x04000644 RID: 1604
		private PhoneNumberType_enum m_Type = PhoneNumberType_enum.Voice;

		// Token: 0x04000645 RID: 1605
		private string m_Number = "";
	}
}
