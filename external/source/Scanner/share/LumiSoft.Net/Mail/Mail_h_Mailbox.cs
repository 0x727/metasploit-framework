using System;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017B RID: 379
	public class Mail_h_Mailbox : MIME_h
	{
		// Token: 0x06000F65 RID: 3941 RVA: 0x0005F624 File Offset: 0x0005E624
		public Mail_h_Mailbox(string fieldName, Mail_t_Mailbox mailbox)
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
			bool flag3 = mailbox == null;
			if (flag3)
			{
				throw new ArgumentNullException("mailbox");
			}
			this.m_Name = fieldName;
			this.m_pAddress = mailbox;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x0005F6A0 File Offset: 0x0005E6A0
		public static Mail_h_Mailbox Parse(string value)
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
			MIME_Reader mime_Reader = new MIME_Reader(array[1].Trim());
			string text = mime_Reader.QuotedReadToDelimiter(new char[]
			{
				',',
				'<',
				':'
			});
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Invalid header field value '" + value + "'.");
			}
			bool flag4 = mime_Reader.Peek(true) == 60;
			Mail_h_Mailbox result;
			if (flag4)
			{
				result = new Mail_h_Mailbox(array[0], new Mail_t_Mailbox((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text)) : null, mime_Reader.ReadParenthesized()))
				{
					m_ParseValue = value
				};
			}
			else
			{
				result = new Mail_h_Mailbox(array[0], new Mail_t_Mailbox(null, text))
				{
					m_ParseValue = value
				};
			}
			return result;
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0005F7AC File Offset: 0x0005E7AC
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = !reEncode && this.m_ParseValue != null;
			string result;
			if (flag)
			{
				result = this.m_ParseValue;
			}
			else
			{
				result = this.m_Name + ": " + this.m_pAddress.ToString(wordEncoder) + "\r\n";
			}
			return result;
		}

		// Token: 0x1700051F RID: 1311
		// (get) Token: 0x06000F68 RID: 3944 RVA: 0x0005F800 File Offset: 0x0005E800
		public override bool IsModified
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x06000F69 RID: 3945 RVA: 0x0005F814 File Offset: 0x0005E814
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x06000F6A RID: 3946 RVA: 0x0005F82C File Offset: 0x0005E82C
		public Mail_t_Mailbox Address
		{
			get
			{
				return this.m_pAddress;
			}
		}

		// Token: 0x04000665 RID: 1637
		private string m_ParseValue = null;

		// Token: 0x04000666 RID: 1638
		private string m_Name = null;

		// Token: 0x04000667 RID: 1639
		private Mail_t_Mailbox m_pAddress = null;
	}
}
