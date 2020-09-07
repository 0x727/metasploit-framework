using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017C RID: 380
	public class Mail_h_MailboxList : MIME_h
	{
		// Token: 0x06000F6B RID: 3947 RVA: 0x0005F844 File Offset: 0x0005E844
		public Mail_h_MailboxList(string filedName, Mail_t_MailboxList values)
		{
			bool flag = filedName == null;
			if (flag)
			{
				throw new ArgumentNullException("filedName");
			}
			bool flag2 = filedName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'filedName' value must be specified.");
			}
			bool flag3 = values == null;
			if (flag3)
			{
				throw new ArgumentNullException("values");
			}
			this.m_Name = filedName;
			this.m_pAddresses = values;
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0005F8C0 File Offset: 0x0005E8C0
		public static Mail_h_MailboxList Parse(string value)
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
			Mail_h_MailboxList mail_h_MailboxList = new Mail_h_MailboxList(array[0], Mail_t_MailboxList.Parse(array[1].Trim()));
			mail_h_MailboxList.m_ParseValue = value;
			mail_h_MailboxList.m_pAddresses.AcceptChanges();
			return mail_h_MailboxList;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0005F94C File Offset: 0x0005E94C
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

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x06000F6E RID: 3950 RVA: 0x0005FA60 File Offset: 0x0005EA60
		public override bool IsModified
		{
			get
			{
				return this.m_pAddresses.IsModified;
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x06000F6F RID: 3951 RVA: 0x0005FA80 File Offset: 0x0005EA80
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0005FA98 File Offset: 0x0005EA98
		public Mail_t_MailboxList Addresses
		{
			get
			{
				return this.m_pAddresses;
			}
		}

		// Token: 0x04000668 RID: 1640
		private string m_ParseValue = null;

		// Token: 0x04000669 RID: 1641
		private string m_Name = null;

		// Token: 0x0400066A RID: 1642
		private Mail_t_MailboxList m_pAddresses = null;
	}
}
