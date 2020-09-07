using System;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001B7 RID: 439
	public class IMAP_t_Fetch_r_i_Envelope : IMAP_t_Fetch_r_i
	{
		// Token: 0x060010DB RID: 4315 RVA: 0x00068200 File Offset: 0x00067200
		public IMAP_t_Fetch_r_i_Envelope(DateTime date, string subject, Mail_t_Address[] from, Mail_t_Address[] sender, Mail_t_Address[] replyTo, Mail_t_Address[] to, Mail_t_Address[] cc, Mail_t_Address[] bcc, string inReplyTo, string messageID)
		{
			this.m_Date = date;
			this.m_Subject = subject;
			this.m_pFrom = from;
			this.m_pSender = sender;
			this.m_pReplyTo = replyTo;
			this.m_pTo = to;
			this.m_pCc = cc;
			this.m_pBcc = bcc;
			this.m_InReplyTo = inReplyTo;
			this.m_MessageID = messageID;
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x000682AC File Offset: 0x000672AC
		public static IMAP_t_Fetch_r_i_Envelope Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			DateTime date = DateTime.MinValue;
			string text = r.ReadWord();
			bool flag2 = !string.IsNullOrEmpty(text) && !text.Equals("NIL", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				date = MIME_Utils.ParseRfc2822DateTime(text);
			}
			string subject = IMAP_t_Fetch_r_i_Envelope.ReadAndDecodeWord(r);
			Mail_t_Address[] from = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			Mail_t_Address[] sender = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			Mail_t_Address[] replyTo = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			Mail_t_Address[] to = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			Mail_t_Address[] cc = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			Mail_t_Address[] bcc = IMAP_t_Fetch_r_i_Envelope.ReadAddresses(r);
			string inReplyTo = r.ReadWord();
			string messageID = r.ReadWord();
			return new IMAP_t_Fetch_r_i_Envelope(date, subject, from, sender, replyTo, to, cc, bcc, inReplyTo, messageID);
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00068368 File Offset: 0x00067368
		public static string ConstructEnvelope(Mail_Message entity)
		{
			MIME_Encoding_EncodedWord mime_Encoding_EncodedWord = new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.B, Encoding.UTF8);
			mime_Encoding_EncodedWord.Split = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ENVELOPE (");
			try
			{
				bool flag = entity.Date != DateTime.MinValue;
				if (flag)
				{
					stringBuilder.Append(TextUtils.QuoteString(MIME_Utils.DateTimeToRfc2822(entity.Date)));
				}
				else
				{
					stringBuilder.Append("NIL");
				}
			}
			catch
			{
				stringBuilder.Append("NIL");
			}
			bool flag2 = entity.Subject != null;
			if (flag2)
			{
				string text = mime_Encoding_EncodedWord.Encode(entity.Subject);
				stringBuilder.Append(string.Concat(new object[]
				{
					" {",
					text.Length,
					"}\r\n",
					text
				}));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag3 = entity.From != null && entity.From.Count > 0;
			if (flag3)
			{
				stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructAddresses(entity.From.ToArray(), mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag4 = entity.Sender != null;
			if (flag4)
			{
				stringBuilder.Append(" (");
				stringBuilder.Append(IMAP_t_Fetch_r_i_Envelope.ConstructAddress(entity.Sender, mime_Encoding_EncodedWord));
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag5 = entity.ReplyTo != null;
			if (flag5)
			{
				stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructAddresses(entity.ReplyTo.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag6 = entity.To != null && entity.To.Count > 0;
			if (flag6)
			{
				stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructAddresses(entity.To.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag7 = entity.Cc != null && entity.Cc.Count > 0;
			if (flag7)
			{
				stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructAddresses(entity.Cc.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag8 = entity.Bcc != null && entity.Bcc.Count > 0;
			if (flag8)
			{
				stringBuilder.Append(" " + IMAP_t_Fetch_r_i_Envelope.ConstructAddresses(entity.Bcc.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag9 = entity.InReplyTo != null;
			if (flag9)
			{
				stringBuilder.Append(" " + TextUtils.QuoteString(mime_Encoding_EncodedWord.Encode(entity.InReplyTo)));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag10 = entity.MessageID != null;
			if (flag10)
			{
				stringBuilder.Append(" " + TextUtils.QuoteString(mime_Encoding_EncodedWord.Encode(entity.MessageID)));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x000686DC File Offset: 0x000676DC
		private static Mail_t_Address[] ReadAddresses(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadToFirstChar();
			bool flag2 = r.StartsWith("NIL", false);
			Mail_t_Address[] result;
			if (flag2)
			{
				r.ReadWord();
				result = null;
			}
			else
			{
				List<Mail_t_Address> list = new List<Mail_t_Address>();
				StringReader stringReader = new StringReader(r.ReadParenthesized());
				stringReader.ReadToFirstChar();
				while (stringReader.Available > 0L)
				{
					bool flag3 = stringReader.StartsWith("(");
					if (flag3)
					{
						stringReader.ReadSpecifiedLength(1);
					}
					string displayName = IMAP_t_Fetch_r_i_Envelope.ReadAndDecodeWord(stringReader);
					string text = stringReader.ReadWord();
					string str = stringReader.ReadWord();
					string str2 = stringReader.ReadWord();
					list.Add(new Mail_t_Mailbox(displayName, str + "@" + str2));
					bool flag4 = stringReader.EndsWith(")");
					if (flag4)
					{
						stringReader.ReadSpecifiedLength(1);
					}
					stringReader.ReadToFirstChar();
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x000687E8 File Offset: 0x000677E8
		private static string ReadAndDecodeWord(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadToFirstChar();
			bool flag2 = r.SourceString.StartsWith("{");
			string result;
			if (flag2)
			{
				int length = Convert.ToInt32(r.ReadParenthesized());
				r.ReadSpecifiedLength(2);
				result = MIME_Encoding_EncodedWord.DecodeTextS(r.ReadSpecifiedLength(length));
			}
			else
			{
				string text = r.ReadWord();
				bool flag3 = text == null;
				if (flag3)
				{
					throw new ParseException("Excpetcted quoted-string or string-literal, but non available.");
				}
				bool flag4 = string.Equals(text, "NIL", StringComparison.InvariantCultureIgnoreCase);
				if (flag4)
				{
					result = "";
				}
				else
				{
					result = MIME_Encoding_EncodedWord.DecodeTextS(text);
				}
			}
			return result;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00068894 File Offset: 0x00067894
		private static string ConstructAddresses(Mail_t_Mailbox[] mailboxes, MIME_Encoding_EncodedWord wordEncoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			foreach (Mail_t_Mailbox address in mailboxes)
			{
				stringBuilder.Append(IMAP_t_Fetch_r_i_Envelope.ConstructAddress(address, wordEncoder));
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x000688F4 File Offset: 0x000678F4
		private static string ConstructAddress(Mail_t_Mailbox address, MIME_Encoding_EncodedWord wordEncoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			bool flag = address.DisplayName != null;
			if (flag)
			{
				stringBuilder.Append(TextUtils.QuoteString(wordEncoder.Encode(IMAP_t_Fetch_r_i_Envelope.RemoveCrlf(address.DisplayName))));
			}
			else
			{
				stringBuilder.Append("NIL");
			}
			stringBuilder.Append(" NIL");
			stringBuilder.Append(" " + TextUtils.QuoteString(wordEncoder.Encode(IMAP_t_Fetch_r_i_Envelope.RemoveCrlf(address.LocalPart))));
			bool flag2 = address.Domain != null;
			if (flag2)
			{
				stringBuilder.Append(" " + TextUtils.QuoteString(wordEncoder.Encode(IMAP_t_Fetch_r_i_Envelope.RemoveCrlf(address.Domain))));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x000689E0 File Offset: 0x000679E0
		private static string RemoveCrlf(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			return value.Replace("\r", "").Replace("\n", "");
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x00068A28 File Offset: 0x00067A28
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x060010E4 RID: 4324 RVA: 0x00068A40 File Offset: 0x00067A40
		public string Subject
		{
			get
			{
				return this.m_Subject;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x00068A58 File Offset: 0x00067A58
		public Mail_t_Address[] From
		{
			get
			{
				return this.m_pFrom;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x060010E6 RID: 4326 RVA: 0x00068A70 File Offset: 0x00067A70
		public Mail_t_Address[] Sender
		{
			get
			{
				return this.m_pSender;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x00068A88 File Offset: 0x00067A88
		public Mail_t_Address[] ReplyTo
		{
			get
			{
				return this.m_pReplyTo;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x060010E8 RID: 4328 RVA: 0x00068AA0 File Offset: 0x00067AA0
		public Mail_t_Address[] To
		{
			get
			{
				return this.m_pTo;
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x060010E9 RID: 4329 RVA: 0x00068AB8 File Offset: 0x00067AB8
		public Mail_t_Address[] Cc
		{
			get
			{
				return this.m_pCc;
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x060010EA RID: 4330 RVA: 0x00068AD0 File Offset: 0x00067AD0
		public Mail_t_Address[] Bcc
		{
			get
			{
				return this.m_pBcc;
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x060010EB RID: 4331 RVA: 0x00068AE8 File Offset: 0x00067AE8
		public string InReplyTo
		{
			get
			{
				return this.m_InReplyTo;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x060010EC RID: 4332 RVA: 0x00068B00 File Offset: 0x00067B00
		public string MessageID
		{
			get
			{
				return this.m_MessageID;
			}
		}

		// Token: 0x040006B6 RID: 1718
		private DateTime m_Date = DateTime.MinValue;

		// Token: 0x040006B7 RID: 1719
		private string m_Subject = null;

		// Token: 0x040006B8 RID: 1720
		private Mail_t_Address[] m_pFrom = null;

		// Token: 0x040006B9 RID: 1721
		private Mail_t_Address[] m_pSender = null;

		// Token: 0x040006BA RID: 1722
		private Mail_t_Address[] m_pReplyTo = null;

		// Token: 0x040006BB RID: 1723
		private Mail_t_Address[] m_pTo = null;

		// Token: 0x040006BC RID: 1724
		private Mail_t_Address[] m_pCc = null;

		// Token: 0x040006BD RID: 1725
		private Mail_t_Address[] m_pBcc = null;

		// Token: 0x040006BE RID: 1726
		private string m_InReplyTo = null;

		// Token: 0x040006BF RID: 1727
		private string m_MessageID = null;
	}
}
