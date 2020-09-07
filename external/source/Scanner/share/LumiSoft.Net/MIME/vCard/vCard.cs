using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x02000177 RID: 375
	public class vCard
	{
		// Token: 0x06000F26 RID: 3878 RVA: 0x0005E488 File Offset: 0x0005D488
		public vCard()
		{
			this.m_pCharset = Encoding.UTF8;
			this.m_pItems = new ItemCollection(this);
			this.Version = "3.0";
			this.UID = Guid.NewGuid().ToString();
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0005E500 File Offset: 0x0005D500
		public byte[] ToByte()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.ToStream(memoryStream);
			return memoryStream.ToArray();
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0005E528 File Offset: 0x0005D528
		public void ToFile(string file)
		{
			using (FileStream fileStream = File.Create(file))
			{
				this.ToStream(fileStream);
			}
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0005E564 File Offset: 0x0005D564
		public void ToStream(Stream stream)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("BEGIN:VCARD\r\n");
			foreach (object obj in this.m_pItems)
			{
				Item item = (Item)obj;
				stringBuilder.Append(item.ToItemString() + "\r\n");
			}
			stringBuilder.Append("END:VCARD\r\n");
			byte[] bytes = this.m_pCharset.GetBytes(stringBuilder.ToString());
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x0005E614 File Offset: 0x0005D614
		public static List<vCard> ParseMultiple(string file)
		{
			List<vCard> list = new List<vCard>();
			List<string> list2 = new List<string>();
			string text = "";
			bool flag = false;
			using (FileStream fileStream = File.OpenRead(file))
			{
				TextReader textReader = new StreamReader(fileStream, Encoding.Default);
				while (text != null)
				{
					text = textReader.ReadLine();
					bool flag2 = text != null && text.ToUpper() == "BEGIN:VCARD";
					if (flag2)
					{
						flag = true;
					}
					bool flag3 = flag;
					if (flag3)
					{
						list2.Add(text);
						bool flag4 = text != null && text.ToUpper() == "END:VCARD";
						if (flag4)
						{
							vCard vCard = new vCard();
							vCard.ParseStrings(list2);
							list.Add(vCard);
							list2.Clear();
							flag = false;
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x0005E700 File Offset: 0x0005D700
		public void Parse(string file)
		{
			List<string> list = new List<string>();
			string[] array = File.ReadAllLines(file, Encoding.Default);
			foreach (string item in array)
			{
				list.Add(item);
			}
			this.ParseStrings(list);
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0005E74C File Offset: 0x0005D74C
		public void Parse(FileStream stream)
		{
			List<string> list = new List<string>();
			string text = "";
			TextReader textReader = new StreamReader(stream, Encoding.Default);
			while (text != null)
			{
				text = textReader.ReadLine();
				list.Add(text);
			}
			this.ParseStrings(list);
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0005E798 File Offset: 0x0005D798
		public void Parse(Stream stream)
		{
			List<string> list = new List<string>();
			string text = "";
			TextReader textReader = new StreamReader(stream, Encoding.Default);
			while (text != null)
			{
				text = textReader.ReadLine();
				list.Add(text);
			}
			this.ParseStrings(list);
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x0005E7E4 File Offset: 0x0005D7E4
		public void ParseStrings(List<string> fileStrings)
		{
			this.m_pItems.Clear();
			this.m_pPhoneNumbers = null;
			this.m_pEmailAddresses = null;
			int index = 0;
			string text = fileStrings[index];
			while (text != null && text.ToUpper() != "BEGIN:VCARD")
			{
				text = fileStrings[index++];
			}
			text = fileStrings[index++];
			while (text != null && text.ToUpper() != "END:VCARD")
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				text = fileStrings[index++];
				while (text != null && (text.StartsWith("\t") || text.StartsWith(" ")))
				{
					stringBuilder.Append(text.Substring(1));
					text = fileStrings[index++];
				}
				string[] array = stringBuilder.ToString().Split(new char[]
				{
					':'
				}, 2);
				string[] array2 = array[0].Split(new char[]
				{
					';'
				}, 2);
				string name = array2[0];
				string parametes = "";
				bool flag = array2.Length == 2;
				if (flag)
				{
					parametes = array2[1];
				}
				string value = "";
				bool flag2 = array.Length == 2;
				if (flag2)
				{
					value = array[1];
				}
				this.m_pItems.Add(name, parametes, value);
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x06000F2F RID: 3887 RVA: 0x0005E954 File Offset: 0x0005D954
		// (set) Token: 0x06000F30 RID: 3888 RVA: 0x0005E96C File Offset: 0x0005D96C
		public Encoding Charset
		{
			get
			{
				return this.m_pCharset;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pCharset = value;
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x06000F31 RID: 3889 RVA: 0x0005E998 File Offset: 0x0005D998
		public ItemCollection Items
		{
			get
			{
				return this.m_pItems;
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x0005E9B0 File Offset: 0x0005D9B0
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x0005E9E7 File Offset: 0x0005D9E7
		public string Version
		{
			get
			{
				Item first = this.m_pItems.GetFirst("VERSION");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetValue("VERSION", value);
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06000F34 RID: 3892 RVA: 0x0005E9FC File Offset: 0x0005D9FC
		// (set) Token: 0x06000F35 RID: 3893 RVA: 0x0005EA34 File Offset: 0x0005DA34
		public Name Name
		{
			get
			{
				Item first = this.m_pItems.GetFirst("N");
				bool flag = first != null;
				Name result;
				if (flag)
				{
					result = Name.Parse(first);
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					this.m_pItems.SetDecodedValue("N", value.ToValueString());
				}
				else
				{
					this.m_pItems.SetDecodedValue("N", null);
				}
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06000F36 RID: 3894 RVA: 0x0005EA7C File Offset: 0x0005DA7C
		// (set) Token: 0x06000F37 RID: 3895 RVA: 0x0005EAB3 File Offset: 0x0005DAB3
		public string FormattedName
		{
			get
			{
				Item first = this.m_pItems.GetFirst("FN");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("FN", value);
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x0005EAC8 File Offset: 0x0005DAC8
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x0005EAFF File Offset: 0x0005DAFF
		public string NickName
		{
			get
			{
				Item first = this.m_pItems.GetFirst("NICKNAME");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("NICKNAME", value);
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06000F3A RID: 3898 RVA: 0x0005EB14 File Offset: 0x0005DB14
		// (set) Token: 0x06000F3B RID: 3899 RVA: 0x0005EB60 File Offset: 0x0005DB60
		public Image Photo
		{
			get
			{
				Item first = this.m_pItems.GetFirst("PHOTO");
				bool flag = first != null;
				Image result;
				if (flag)
				{
					result = Image.FromStream(new MemoryStream(Encoding.Default.GetBytes(first.DecodedValue)));
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					MemoryStream memoryStream = new MemoryStream();
					value.Save(memoryStream, ImageFormat.Jpeg);
					this.m_pItems.SetValue("PHOTO", "ENCODING=b;TYPE=JPEG", Convert.ToBase64String(memoryStream.ToArray()));
				}
				else
				{
					this.m_pItems.SetValue("PHOTO", null);
				}
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x0005EBC4 File Offset: 0x0005DBC4
		// (set) Token: 0x06000F3D RID: 3901 RVA: 0x0005EC38 File Offset: 0x0005DC38
		public DateTime BirthDate
		{
			get
			{
				Item first = this.m_pItems.GetFirst("BDAY");
				bool flag = first != null;
				DateTime result;
				if (flag)
				{
					string s = first.DecodedValue.Replace("-", "");
					string[] formats = new string[]
					{
						"yyyyMMdd",
						"yyyyMMddz"
					};
					result = DateTime.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
				}
				else
				{
					result = DateTime.MinValue;
				}
				return result;
			}
			set
			{
				bool flag = value != DateTime.MinValue;
				if (flag)
				{
					this.m_pItems.SetValue("BDAY", value.ToString("yyyyMMdd"));
				}
				else
				{
					this.m_pItems.SetValue("BDAY", null);
				}
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06000F3E RID: 3902 RVA: 0x0005EC8C File Offset: 0x0005DC8C
		public DeliveryAddressCollection Addresses
		{
			get
			{
				bool flag = this.m_pAddresses == null;
				if (flag)
				{
					this.m_pAddresses = new DeliveryAddressCollection(this);
				}
				return this.m_pAddresses;
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06000F3F RID: 3903 RVA: 0x0005ECC0 File Offset: 0x0005DCC0
		public PhoneNumberCollection PhoneNumbers
		{
			get
			{
				bool flag = this.m_pPhoneNumbers == null;
				if (flag)
				{
					this.m_pPhoneNumbers = new PhoneNumberCollection(this);
				}
				return this.m_pPhoneNumbers;
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06000F40 RID: 3904 RVA: 0x0005ECF4 File Offset: 0x0005DCF4
		public EmailAddressCollection EmailAddresses
		{
			get
			{
				bool flag = this.m_pEmailAddresses == null;
				if (flag)
				{
					this.m_pEmailAddresses = new EmailAddressCollection(this);
				}
				return this.m_pEmailAddresses;
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06000F41 RID: 3905 RVA: 0x0005ED28 File Offset: 0x0005DD28
		// (set) Token: 0x06000F42 RID: 3906 RVA: 0x0005ED5F File Offset: 0x0005DD5F
		public string Title
		{
			get
			{
				Item first = this.m_pItems.GetFirst("TITLE");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("TITLE", value);
			}
		}

		// Token: 0x17000512 RID: 1298
		// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0005ED74 File Offset: 0x0005DD74
		// (set) Token: 0x06000F44 RID: 3908 RVA: 0x0005EDAB File Offset: 0x0005DDAB
		public string Role
		{
			get
			{
				Item first = this.m_pItems.GetFirst("ROLE");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("ROLE", value);
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06000F45 RID: 3909 RVA: 0x0005EDC0 File Offset: 0x0005DDC0
		// (set) Token: 0x06000F46 RID: 3910 RVA: 0x0005EDF7 File Offset: 0x0005DDF7
		public string Organization
		{
			get
			{
				Item first = this.m_pItems.GetFirst("ORG");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("ORG", value);
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0005EE0C File Offset: 0x0005DE0C
		// (set) Token: 0x06000F48 RID: 3912 RVA: 0x0005EE43 File Offset: 0x0005DE43
		public string NoteText
		{
			get
			{
				Item first = this.m_pItems.GetFirst("NOTE");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("NOTE", value);
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x06000F49 RID: 3913 RVA: 0x0005EE58 File Offset: 0x0005DE58
		// (set) Token: 0x06000F4A RID: 3914 RVA: 0x0005EE8F File Offset: 0x0005DE8F
		public string UID
		{
			get
			{
				Item first = this.m_pItems.GetFirst("UID");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = first.DecodedValue;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.m_pItems.SetDecodedValue("UID", value);
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0005EEA4 File Offset: 0x0005DEA4
		// (set) Token: 0x06000F4C RID: 3916 RVA: 0x0005EF20 File Offset: 0x0005DF20
		public string HomeURL
		{
			get
			{
				Item[] array = this.m_pItems.Get("URL");
				foreach (Item item in array)
				{
					bool flag = item.ParametersString == "" || item.ParametersString.ToUpper().IndexOf("HOME") > -1;
					if (flag)
					{
						return item.DecodedValue;
					}
				}
				return null;
			}
			set
			{
				Item[] array = this.m_pItems.Get("URL");
				foreach (Item item in array)
				{
					bool flag = item.ParametersString.ToUpper().IndexOf("HOME") > -1;
					if (flag)
					{
						bool flag2 = value != null;
						if (flag2)
						{
							item.Value = value;
						}
						else
						{
							this.m_pItems.Remove(item);
						}
						return;
					}
				}
				bool flag3 = value != null;
				if (flag3)
				{
					this.m_pItems.Add("URL", "HOME", value);
					return;
				}
			}
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0005EFC0 File Offset: 0x0005DFC0
		// (set) Token: 0x06000F4E RID: 3918 RVA: 0x0005F028 File Offset: 0x0005E028
		public string WorkURL
		{
			get
			{
				Item[] array = this.m_pItems.Get("URL");
				foreach (Item item in array)
				{
					bool flag = item.ParametersString.ToUpper().IndexOf("WORK") > -1;
					if (flag)
					{
						return item.DecodedValue;
					}
				}
				return null;
			}
			set
			{
				Item[] array = this.m_pItems.Get("URL");
				foreach (Item item in array)
				{
					bool flag = item.ParametersString.ToUpper().IndexOf("WORK") > -1;
					if (flag)
					{
						bool flag2 = value != null;
						if (flag2)
						{
							item.Value = value;
						}
						else
						{
							this.m_pItems.Remove(item);
						}
						return;
					}
				}
				bool flag3 = value != null;
				if (flag3)
				{
					this.m_pItems.Add("URL", "WORK", value);
					return;
				}
			}
		}

		// Token: 0x04000658 RID: 1624
		private Encoding m_pCharset = null;

		// Token: 0x04000659 RID: 1625
		private ItemCollection m_pItems = null;

		// Token: 0x0400065A RID: 1626
		private DeliveryAddressCollection m_pAddresses = null;

		// Token: 0x0400065B RID: 1627
		private PhoneNumberCollection m_pPhoneNumbers = null;

		// Token: 0x0400065C RID: 1628
		private EmailAddressCollection m_pEmailAddresses = null;
	}
}
