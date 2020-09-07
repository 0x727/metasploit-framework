using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000179 RID: 377
	public class Mail_h_AddressList : MIME_h
	{
		// Token: 0x06000F5A RID: 3930 RVA: 0x0005F350 File Offset: 0x0005E350
		public Mail_h_AddressList(string fieldName, Mail_t_AddressList values)
		{
			bool flag = fieldName == null;
			if (flag)
			{
				throw new ArgumentNullException("fieldName");
			}
			bool flag2 = fieldName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'fieldName' value must be specified.");
			}
			bool flag3 = values == null;
			if (flag3)
			{
				throw new ArgumentNullException("values");
			}
			this.m_Name = fieldName;
			this.m_pAddresses = values;
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x0005F3CC File Offset: 0x0005E3CC
		public static Mail_h_AddressList Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string[] array = value.Split(new char[]
			{
				':'
			}, 2);
			bool flag2 = array.Length != 2;
			if (flag2)
			{
				throw new ParseException("Invalid header field value '" + value + "'.");
			}
			Mail_h_AddressList mail_h_AddressList = new Mail_h_AddressList(array[0], Mail_t_AddressList.Parse(array[1].Trim()));
			mail_h_AddressList.m_ParseValue = value;
			mail_h_AddressList.m_pAddresses.AcceptChanges();
			return mail_h_AddressList;
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0005F458 File Offset: 0x0005E458
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = reEncode || this.IsModified;
			string result;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.Name + ": ");
				for (int i = 0; i < this.m_pAddresses.Count; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						stringBuilder.Append("\t");
					}
					bool flag3 = i == this.m_pAddresses.Count - 1;
					if (flag3)
					{
						stringBuilder.Append(this.m_pAddresses[i].ToString(wordEncoder) + "\r\n");
					}
					else
					{
						stringBuilder.Append(this.m_pAddresses[i].ToString(wordEncoder) + ",\r\n");
					}
				}
				bool flag4 = this.m_pAddresses.Count == 0;
				if (flag4)
				{
					stringBuilder.Append("\r\n");
				}
				result = stringBuilder.ToString();
			}
			else
			{
				result = this.m_ParseValue;
			}
			return result;
		}

		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0005F56C File Offset: 0x0005E56C
		public override bool IsModified
		{
			get
			{
				return this.m_pAddresses.IsModified;
			}
		}

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x06000F5E RID: 3934 RVA: 0x0005F58C File Offset: 0x0005E58C
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x1700051B RID: 1307
		// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0005F5A4 File Offset: 0x0005E5A4
		public Mail_t_AddressList Addresses
		{
			get
			{
				return this.m_pAddresses;
			}
		}

		// Token: 0x04000662 RID: 1634
		private string m_ParseValue = null;

		// Token: 0x04000663 RID: 1635
		private string m_Name = null;

		// Token: 0x04000664 RID: 1636
		private Mail_t_AddressList m_pAddresses = null;
	}
}
