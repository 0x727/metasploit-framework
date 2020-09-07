using System;
using System.Text.RegularExpressions;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000181 RID: 385
	public class Mail_t_Mailbox : Mail_t_Address
	{
		// Token: 0x06000F8D RID: 3981 RVA: 0x000602A0 File Offset: 0x0005F2A0
		public Mail_t_Mailbox(string displayName, string address)
		{
			bool flag = address == null;
			if (flag)
			{
				throw new ArgumentNullException("address");
			}
			this.m_DisplayName = displayName;
			this.m_Address = address;
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x000602E8 File Offset: 0x0005F2E8
		public static Mail_t_Mailbox Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			MIME_Reader mime_Reader = new MIME_Reader(value);
			Mail_t_MailboxList mail_t_MailboxList = new Mail_t_MailboxList();
			string text = mime_Reader.QuotedReadToDelimiter(new char[]
			{
				',',
				'<'
			});
			bool flag2 = string.IsNullOrEmpty(text) && mime_Reader.Available == 0;
			if (flag2)
			{
				throw new ParseException("Not valid 'mailbox' value '" + value + "'.");
			}
			bool flag3 = mime_Reader.Peek(true) == 60;
			Mail_t_Mailbox result;
			if (flag3)
			{
				result = new Mail_t_Mailbox((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text.Trim())) : null, mime_Reader.ReadParenthesized());
			}
			else
			{
				result = new Mail_t_Mailbox(null, text);
			}
			return result;
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x000603B4 File Offset: 0x0005F3B4
		public override string ToString()
		{
			return this.ToString(null);
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x000603D0 File Offset: 0x0005F3D0
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder)
		{
			bool flag = string.IsNullOrEmpty(this.m_DisplayName);
			string result;
			if (flag)
			{
				result = this.m_Address;
			}
			else
			{
				bool flag2 = wordEncoder != null && MIME_Encoding_EncodedWord.MustEncode(this.m_DisplayName);
				if (flag2)
				{
					result = wordEncoder.Encode(this.m_DisplayName) + " <" + this.m_Address + ">";
				}
				else
				{
					bool flag3 = Regex.IsMatch(this.m_DisplayName, "[\"(),:;<>@\\[\\\\\\]]");
					if (flag3)
					{
						result = TextUtils.QuoteString(this.m_DisplayName) + " <" + this.m_Address + ">";
					}
					else
					{
						result = this.m_DisplayName + " <" + this.m_Address + ">";
					}
				}
			}
			return result;
		}

		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00060490 File Offset: 0x0005F490
		public string DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x000604A8 File Offset: 0x0005F4A8
		public string Address
		{
			get
			{
				return this.m_Address;
			}
		}

		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x000604C0 File Offset: 0x0005F4C0
		public string LocalPart
		{
			get
			{
				string[] array = this.m_Address.Split(new char[]
				{
					'@'
				});
				return array[0];
			}
		}

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x000604EC File Offset: 0x0005F4EC
		public string Domain
		{
			get
			{
				string[] array = this.m_Address.Split(new char[]
				{
					'@'
				});
				bool flag = array.Length == 2;
				string result;
				if (flag)
				{
					result = array[1];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x04000671 RID: 1649
		private string m_DisplayName = null;

		// Token: 0x04000672 RID: 1650
		private string m_Address = null;
	}
}
