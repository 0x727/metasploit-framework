using System;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x0200016E RID: 366
	public class EmailAddress
	{
		// Token: 0x06000EE4 RID: 3812 RVA: 0x0005CF32 File Offset: 0x0005BF32
		internal EmailAddress(Item item, EmailAddressType_enum type, string emailAddress)
		{
			this.m_pItem = item;
			this.m_Type = type;
			this.m_EmailAddress = emailAddress;
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0005CF6A File Offset: 0x0005BF6A
		private void Changed()
		{
			this.m_pItem.ParametersString = EmailAddress.EmailTypeToString(this.m_Type);
			this.m_pItem.SetDecodedValue(this.m_EmailAddress);
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0005CF98 File Offset: 0x0005BF98
		internal static EmailAddress Parse(Item item)
		{
			EmailAddressType_enum emailAddressType_enum = EmailAddressType_enum.NotSpecified;
			bool flag = item.ParametersString.ToUpper().IndexOf("PREF") != -1;
			if (flag)
			{
				emailAddressType_enum |= EmailAddressType_enum.Preferred;
			}
			bool flag2 = item.ParametersString.ToUpper().IndexOf("INTERNET") != -1;
			if (flag2)
			{
				emailAddressType_enum |= EmailAddressType_enum.Internet;
			}
			bool flag3 = item.ParametersString.ToUpper().IndexOf("X400") != -1;
			if (flag3)
			{
				emailAddressType_enum |= EmailAddressType_enum.X400;
			}
			return new EmailAddress(item, emailAddressType_enum, item.DecodedValue);
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0005D02C File Offset: 0x0005C02C
		internal static string EmailTypeToString(EmailAddressType_enum type)
		{
			string text = "";
			bool flag = (type & EmailAddressType_enum.Internet) > EmailAddressType_enum.NotSpecified;
			if (flag)
			{
				text += "INTERNET,";
			}
			bool flag2 = (type & EmailAddressType_enum.Preferred) > EmailAddressType_enum.NotSpecified;
			if (flag2)
			{
				text += "PREF,";
			}
			bool flag3 = (type & EmailAddressType_enum.X400) > EmailAddressType_enum.NotSpecified;
			if (flag3)
			{
				text += "X400,";
			}
			bool flag4 = text.EndsWith(",");
			if (flag4)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0005D0B4 File Offset: 0x0005C0B4
		public Item Item
		{
			get
			{
				return this.m_pItem;
			}
		}

		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x0005D0CC File Offset: 0x0005C0CC
		// (set) Token: 0x06000EEA RID: 3818 RVA: 0x0005D0E4 File Offset: 0x0005C0E4
		public EmailAddressType_enum EmailType
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

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x06000EEB RID: 3819 RVA: 0x0005D0F8 File Offset: 0x0005C0F8
		// (set) Token: 0x06000EEC RID: 3820 RVA: 0x0005D110 File Offset: 0x0005C110
		public string Email
		{
			get
			{
				return this.m_EmailAddress;
			}
			set
			{
				this.m_EmailAddress = value;
				this.Changed();
			}
		}

		// Token: 0x0400062D RID: 1581
		private Item m_pItem = null;

		// Token: 0x0400062E RID: 1582
		private EmailAddressType_enum m_Type = EmailAddressType_enum.Internet;

		// Token: 0x0400062F RID: 1583
		private string m_EmailAddress = "";
	}
}
