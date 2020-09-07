using System;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000162 RID: 354
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MailboxAddress : Address
	{
		// Token: 0x06000E34 RID: 3636 RVA: 0x00057C8A File Offset: 0x00056C8A
		public MailboxAddress() : base(false)
		{
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x00057CAB File Offset: 0x00056CAB
		public MailboxAddress(string emailAddress) : base(false)
		{
			this.m_EmailAddress = emailAddress;
		}

		// Token: 0x06000E36 RID: 3638 RVA: 0x00057CD3 File Offset: 0x00056CD3
		public MailboxAddress(string displayName, string emailAddress) : base(false)
		{
			this.m_DisplayName = displayName;
			this.m_EmailAddress = emailAddress;
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00057D04 File Offset: 0x00056D04
		public static MailboxAddress Parse(string mailbox)
		{
			mailbox = mailbox.Trim();
			string displayName = "";
			string text = mailbox;
			bool flag = mailbox.IndexOf("<") > -1 && mailbox.IndexOf(">") > -1;
			if (flag)
			{
				displayName = MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(mailbox.Substring(0, mailbox.LastIndexOf("<"))));
				text = mailbox.Substring(mailbox.LastIndexOf("<") + 1, mailbox.Length - mailbox.LastIndexOf("<") - 2).Trim();
			}
			else
			{
				bool flag2 = mailbox.StartsWith("\"");
				if (flag2)
				{
					int num = mailbox.IndexOf("\"");
					bool flag3 = num > -1 && mailbox.LastIndexOf("\"") > num;
					if (flag3)
					{
						displayName = MIME_Encoding_EncodedWord.DecodeS(mailbox.Substring(num + 1, mailbox.LastIndexOf("\"") - num - 1).Trim());
					}
					text = mailbox.Substring(mailbox.LastIndexOf("\"") + 1).Trim();
				}
				text = text.Replace("<", "").Replace(">", "").Trim();
			}
			return new MailboxAddress(displayName, text);
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00057E48 File Offset: 0x00056E48
		public string ToMailboxAddressString()
		{
			string str = "";
			bool flag = this.m_DisplayName.Length > 0;
			if (flag)
			{
				bool flag2 = Core.IsAscii(this.m_DisplayName);
				if (flag2)
				{
					str = TextUtils.QuoteString(this.m_DisplayName) + " ";
				}
				else
				{
					str = MimeUtils.EncodeWord(this.m_DisplayName) + " ";
				}
			}
			return str + "<" + this.EmailAddress + ">";
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00057ED0 File Offset: 0x00056ED0
		internal void OnChanged()
		{
			bool flag = base.Owner != null;
			if (flag)
			{
				bool flag2 = base.Owner is AddressList;
				if (flag2)
				{
					((AddressList)base.Owner).OnCollectionChanged();
				}
				else
				{
					bool flag3 = base.Owner is MailboxAddressCollection;
					if (flag3)
					{
						((MailboxAddressCollection)base.Owner).OnCollectionChanged();
					}
				}
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06000E3A RID: 3642 RVA: 0x00057F3C File Offset: 0x00056F3C
		[Obsolete("Use ToMailboxAddressString instead !")]
		public string MailboxString
		{
			get
			{
				string str = "";
				bool flag = this.DisplayName != "";
				if (flag)
				{
					str = str + TextUtils.QuoteString(this.DisplayName) + " ";
				}
				return str + "<" + this.EmailAddress + ">";
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06000E3B RID: 3643 RVA: 0x00057F9C File Offset: 0x00056F9C
		// (set) Token: 0x06000E3C RID: 3644 RVA: 0x00057FB4 File Offset: 0x00056FB4
		public string DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
				this.OnChanged();
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06000E3D RID: 3645 RVA: 0x00057FC8 File Offset: 0x00056FC8
		// (set) Token: 0x06000E3E RID: 3646 RVA: 0x00057FE0 File Offset: 0x00056FE0
		public string EmailAddress
		{
			get
			{
				return this.m_EmailAddress;
			}
			set
			{
				bool flag = !Core.IsAscii(value);
				if (flag)
				{
					throw new Exception("Email address can contain ASCII chars only !");
				}
				this.m_EmailAddress = value;
				this.OnChanged();
			}
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x00058018 File Offset: 0x00057018
		public string LocalPart
		{
			get
			{
				bool flag = this.EmailAddress.IndexOf("@") > -1;
				string result;
				if (flag)
				{
					result = this.EmailAddress.Substring(0, this.EmailAddress.IndexOf("@"));
				}
				else
				{
					result = this.EmailAddress;
				}
				return result;
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x00058068 File Offset: 0x00057068
		public string Domain
		{
			get
			{
				bool flag = this.EmailAddress.IndexOf("@") != -1;
				string result;
				if (flag)
				{
					result = this.EmailAddress.Substring(this.EmailAddress.IndexOf("@") + 1);
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x040005F3 RID: 1523
		private string m_DisplayName = "";

		// Token: 0x040005F4 RID: 1524
		private string m_EmailAddress = "";
	}
}
