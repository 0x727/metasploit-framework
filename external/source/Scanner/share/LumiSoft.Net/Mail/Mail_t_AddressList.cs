using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x0200017F RID: 383
	public class Mail_t_AddressList : IEnumerable
	{
		// Token: 0x06000F79 RID: 3961 RVA: 0x0005FC08 File Offset: 0x0005EC08
		public Mail_t_AddressList()
		{
			this.m_pList = new List<Mail_t_Address>();
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0005FC2C File Offset: 0x0005EC2C
		public static Mail_t_AddressList Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			MIME_Reader mime_Reader = new MIME_Reader(value);
			Mail_t_AddressList mail_t_AddressList = new Mail_t_AddressList();
			for (;;)
			{
				string text = mime_Reader.QuotedReadToDelimiter(new char[]
				{
					',',
					'<',
					':'
				});
				bool flag2 = text == null && mime_Reader.Available == 0;
				if (flag2)
				{
					break;
				}
				bool flag3 = mime_Reader.Peek(true) == 58;
				if (flag3)
				{
					Mail_t_Group mail_t_Group = new Mail_t_Group((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text)) : null);
					mime_Reader.Char(true);
					for (;;)
					{
						text = mime_Reader.QuotedReadToDelimiter(new char[]
						{
							',',
							'<',
							':',
							';'
						});
						bool flag4 = (text == null && mime_Reader.Available == 0) || mime_Reader.Peek(false) == 59;
						if (flag4)
						{
							break;
						}
						bool flag5 = text == string.Empty;
						if (flag5)
						{
							goto Block_8;
						}
						bool flag6 = mime_Reader.Peek(true) == 60;
						if (flag6)
						{
							mail_t_Group.Members.Add(new Mail_t_Mailbox((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text)) : null, mime_Reader.ReadParenthesized()));
						}
						else
						{
							mail_t_Group.Members.Add(new Mail_t_Mailbox(null, text));
						}
						bool flag7 = mime_Reader.Peek(true) == 59;
						if (flag7)
						{
							goto Block_11;
						}
						bool flag8 = mime_Reader.Peek(true) == 44;
						if (flag8)
						{
							mime_Reader.Char(false);
						}
					}
					IL_18F:
					mail_t_AddressList.Add(mail_t_Group);
					goto IL_1E8;
					goto IL_18F;
					Block_11:
					mime_Reader.Char(true);
					goto IL_18F;
				}
				bool flag9 = mime_Reader.Peek(true) == 60;
				if (flag9)
				{
					mail_t_AddressList.Add(new Mail_t_Mailbox((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text.Trim())) : null, mime_Reader.ReadParenthesized()));
				}
				else
				{
					mail_t_AddressList.Add(new Mail_t_Mailbox(null, text));
				}
				IL_1E8:
				bool flag10 = mime_Reader.Peek(true) == 44;
				if (flag10)
				{
					mime_Reader.Char(false);
				}
			}
			return mail_t_AddressList;
			Block_8:
			throw new ParseException("Invalid address-list value '" + value + "'.");
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0005FE4C File Offset: 0x0005EE4C
		public void Insert(int index, Mail_t_Address value)
		{
			bool flag = index < 0 || index > this.m_pList.Count;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			bool flag2 = value == null;
			if (flag2)
			{
				throw new ArgumentNullException("value");
			}
			this.m_pList.Insert(index, value);
			this.m_IsModified = true;
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x0005FEA8 File Offset: 0x0005EEA8
		public void Add(Mail_t_Address value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_pList.Add(value);
			this.m_IsModified = true;
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x0005FEE0 File Offset: 0x0005EEE0
		public void Remove(Mail_t_Address value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_pList.Remove(value);
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0005FF0F File Offset: 0x0005EF0F
		public void Clear()
		{
			this.m_pList.Clear();
			this.m_IsModified = true;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0005FF28 File Offset: 0x0005EF28
		public Mail_t_Address[] ToArray()
		{
			return this.m_pList.ToArray();
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x0005FF48 File Offset: 0x0005EF48
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_pList.Count; i++)
			{
				bool flag = i == this.m_pList.Count - 1;
				if (flag)
				{
					stringBuilder.Append(this.m_pList[i].ToString());
				}
				else
				{
					stringBuilder.Append(this.m_pList[i].ToString() + ",");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x0005FFD8 File Offset: 0x0005EFD8
		internal void AcceptChanges()
		{
			this.m_IsModified = false;
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x0005FFE4 File Offset: 0x0005EFE4
		public IEnumerator GetEnumerator()
		{
			return this.m_pList.GetEnumerator();
		}

		// Token: 0x17000528 RID: 1320
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x00060008 File Offset: 0x0005F008
		public bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000529 RID: 1321
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00060020 File Offset: 0x0005F020
		public int Count
		{
			get
			{
				return this.m_pList.Count;
			}
		}

		// Token: 0x1700052A RID: 1322
		public Mail_t_Address this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this.m_pList.Count;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.m_pList[index];
			}
		}

		// Token: 0x1700052B RID: 1323
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x00060088 File Offset: 0x0005F088
		public Mail_t_Mailbox[] Mailboxes
		{
			get
			{
				List<Mail_t_Mailbox> list = new List<Mail_t_Mailbox>();
				foreach (object obj in this)
				{
					Mail_t_Address mail_t_Address = (Mail_t_Address)obj;
					bool flag = mail_t_Address is Mail_t_Mailbox;
					if (flag)
					{
						list.Add((Mail_t_Mailbox)mail_t_Address);
					}
					else
					{
						list.AddRange(((Mail_t_Group)mail_t_Address).Members);
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x0400066D RID: 1645
		private bool m_IsModified = false;

		// Token: 0x0400066E RID: 1646
		private List<Mail_t_Address> m_pList = null;
	}
}
