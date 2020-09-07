using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000182 RID: 386
	public class Mail_t_MailboxList : IEnumerable
	{
		// Token: 0x06000F95 RID: 3989 RVA: 0x0006052C File Offset: 0x0005F52C
		public Mail_t_MailboxList()
		{
			this.m_pList = new List<Mail_t_Mailbox>();
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00060550 File Offset: 0x0005F550
		public static Mail_t_MailboxList Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			MIME_Reader mime_Reader = new MIME_Reader(value);
			Mail_t_MailboxList mail_t_MailboxList = new Mail_t_MailboxList();
			for (;;)
			{
				string text = mime_Reader.QuotedReadToDelimiter(new char[]
				{
					',',
					'<'
				});
				bool flag2 = string.IsNullOrEmpty(text) && mime_Reader.Available == 0;
				if (flag2)
				{
					break;
				}
				bool flag3 = mime_Reader.Peek(true) == 60;
				if (flag3)
				{
					mail_t_MailboxList.Add(new Mail_t_Mailbox((text != null) ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(text.Trim())) : null, mime_Reader.ReadParenthesized()));
				}
				else
				{
					mail_t_MailboxList.Add(new Mail_t_Mailbox(null, text));
				}
				bool flag4 = mime_Reader.Peek(true) == 44;
				if (flag4)
				{
					mime_Reader.Char(false);
				}
			}
			return mail_t_MailboxList;
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x00060634 File Offset: 0x0005F634
		public void Insert(int index, Mail_t_Mailbox value)
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

		// Token: 0x06000F98 RID: 3992 RVA: 0x00060690 File Offset: 0x0005F690
		public void Add(Mail_t_Mailbox value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_pList.Add(value);
			this.m_IsModified = true;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x000606C8 File Offset: 0x0005F6C8
		public void Remove(Mail_t_Mailbox value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_pList.Remove(value);
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x000606F7 File Offset: 0x0005F6F7
		public void Clear()
		{
			this.m_pList.Clear();
			this.m_IsModified = true;
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x00060710 File Offset: 0x0005F710
		public Mail_t_Mailbox[] ToArray()
		{
			return this.m_pList.ToArray();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00060730 File Offset: 0x0005F730
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

		// Token: 0x06000F9D RID: 3997 RVA: 0x000607C0 File Offset: 0x0005F7C0
		internal void AcceptChanges()
		{
			this.m_IsModified = false;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000607CC File Offset: 0x0005F7CC
		public IEnumerator GetEnumerator()
		{
			return this.m_pList.GetEnumerator();
		}

		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x000607F0 File Offset: 0x0005F7F0
		public bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x00060808 File Offset: 0x0005F808
		public int Count
		{
			get
			{
				return this.m_pList.Count;
			}
		}

		// Token: 0x17000534 RID: 1332
		public Mail_t_Mailbox this[int index]
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

		// Token: 0x04000673 RID: 1651
		private bool m_IsModified = false;

		// Token: 0x04000674 RID: 1652
		private List<Mail_t_Mailbox> m_pList = null;
	}
}
