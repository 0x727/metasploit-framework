using System;
using System.Collections.Generic;
using System.Text;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D1 RID: 465
	[Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
	public class IMAP_Envelope
	{
		// Token: 0x06001131 RID: 4401 RVA: 0x000696C4 File Offset: 0x000686C4
		public IMAP_Envelope(DateTime date, string subject, Mail_t_Address[] from, Mail_t_Address[] sender, Mail_t_Address[] replyTo, Mail_t_Address[] to, Mail_t_Address[] cc, Mail_t_Address[] bcc, string inReplyTo, string messageID)
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

		// Token: 0x06001132 RID: 4402 RVA: 0x00069770 File Offset: 0x00068770
		public static IMAP_Envelope Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadWord();
			r.ReadToFirstChar();
			r.ReadSpecifiedLength(1);
			DateTime date = DateTime.MinValue;
			string text = r.ReadWord();
			bool flag2 = text != null;
			if (flag2)
			{
				date = MIME_Utils.ParseRfc2822DateTime(text);
			}
			string subject = IMAP_Envelope.ReadAndDecodeWord(r.ReadWord());
			Mail_t_Address[] from = IMAP_Envelope.ReadAddresses(r);
			Mail_t_Address[] sender = IMAP_Envelope.ReadAddresses(r);
			Mail_t_Address[] replyTo = IMAP_Envelope.ReadAddresses(r);
			Mail_t_Address[] to = IMAP_Envelope.ReadAddresses(r);
			Mail_t_Address[] cc = IMAP_Envelope.ReadAddresses(r);
			Mail_t_Address[] bcc = IMAP_Envelope.ReadAddresses(r);
			string inReplyTo = r.ReadWord();
			string messageID = r.ReadWord();
			r.ReadToFirstChar();
			r.ReadSpecifiedLength(1);
			return new IMAP_Envelope(date, subject, from, sender, replyTo, to, cc, bcc, inReplyTo, messageID);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00069840 File Offset: 0x00068840
		internal static IMAP_Envelope Parse(IMAP_Client._FetchResponseReader fetchReader)
		{
			bool flag = fetchReader == null;
			if (flag)
			{
				throw new ArgumentNullException("fetchReader");
			}
			fetchReader.GetReader().ReadWord();
			fetchReader.GetReader().ReadToFirstChar();
			fetchReader.GetReader().ReadSpecifiedLength(1);
			DateTime date = DateTime.MinValue;
			string text = fetchReader.ReadString();
			bool flag2 = text != null;
			if (flag2)
			{
				date = MIME_Utils.ParseRfc2822DateTime(text);
			}
			string subject = IMAP_Envelope.ReadAndDecodeWord(fetchReader.ReadString());
			Mail_t_Address[] from = IMAP_Envelope.ReadAddresses(fetchReader);
			Mail_t_Address[] sender = IMAP_Envelope.ReadAddresses(fetchReader);
			Mail_t_Address[] replyTo = IMAP_Envelope.ReadAddresses(fetchReader);
			Mail_t_Address[] to = IMAP_Envelope.ReadAddresses(fetchReader);
			Mail_t_Address[] cc = IMAP_Envelope.ReadAddresses(fetchReader);
			Mail_t_Address[] bcc = IMAP_Envelope.ReadAddresses(fetchReader);
			string inReplyTo = fetchReader.ReadString();
			string messageID = fetchReader.ReadString();
			fetchReader.GetReader().ReadToFirstChar();
			fetchReader.GetReader().ReadSpecifiedLength(1);
			return new IMAP_Envelope(date, subject, from, sender, replyTo, to, cc, bcc, inReplyTo, messageID);
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0006992C File Offset: 0x0006892C
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
				stringBuilder.Append(" " + IMAP_Envelope.ConstructAddresses(entity.From.ToArray(), mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag4 = entity.Sender != null;
			if (flag4)
			{
				stringBuilder.Append(" (");
				stringBuilder.Append(IMAP_Envelope.ConstructAddress(entity.Sender, mime_Encoding_EncodedWord));
				stringBuilder.Append(")");
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag5 = entity.ReplyTo != null;
			if (flag5)
			{
				stringBuilder.Append(" " + IMAP_Envelope.ConstructAddresses(entity.ReplyTo.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag6 = entity.To != null && entity.To.Count > 0;
			if (flag6)
			{
				stringBuilder.Append(" " + IMAP_Envelope.ConstructAddresses(entity.To.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag7 = entity.Cc != null && entity.Cc.Count > 0;
			if (flag7)
			{
				stringBuilder.Append(" " + IMAP_Envelope.ConstructAddresses(entity.Cc.Mailboxes, mime_Encoding_EncodedWord));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			bool flag8 = entity.Bcc != null && entity.Bcc.Count > 0;
			if (flag8)
			{
				stringBuilder.Append(" " + IMAP_Envelope.ConstructAddresses(entity.Bcc.Mailboxes, mime_Encoding_EncodedWord));
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

		// Token: 0x06001135 RID: 4405 RVA: 0x00069CA0 File Offset: 0x00068CA0
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
				r.ReadSpecifiedLength(1);
				while (r.Available > 0L)
				{
					bool flag3 = r.StartsWith(")");
					if (flag3)
					{
						r.ReadSpecifiedLength(1);
						break;
					}
					r.ReadSpecifiedLength(1);
					string displayName = IMAP_Envelope.ReadAndDecodeWord(r.ReadWord());
					string text = r.ReadWord();
					string str = r.ReadWord();
					string str2 = r.ReadWord();
					list.Add(new Mail_t_Mailbox(displayName, str + "@" + str2));
					r.ReadSpecifiedLength(1);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x00069D84 File Offset: 0x00068D84
		private static Mail_t_Address[] ReadAddresses(IMAP_Client._FetchResponseReader fetchReader)
		{
			bool flag = fetchReader == null;
			if (flag)
			{
				throw new ArgumentNullException("fetchReader");
			}
			fetchReader.GetReader().ReadToFirstChar();
			bool flag2 = fetchReader.GetReader().StartsWith("NIL", false);
			Mail_t_Address[] result;
			if (flag2)
			{
				fetchReader.GetReader().ReadWord();
				result = null;
			}
			else
			{
				List<Mail_t_Address> list = new List<Mail_t_Address>();
				fetchReader.GetReader().ReadSpecifiedLength(1);
				while (fetchReader.GetReader().Available > 0L)
				{
					bool flag3 = fetchReader.GetReader().StartsWith(")");
					if (flag3)
					{
						fetchReader.GetReader().ReadSpecifiedLength(1);
						break;
					}
					fetchReader.GetReader().ReadSpecifiedLength(1);
					string displayName = IMAP_Envelope.ReadAndDecodeWord(fetchReader.ReadString());
					string text = fetchReader.ReadString();
					string str = fetchReader.ReadString();
					string str2 = fetchReader.ReadString();
					list.Add(new Mail_t_Mailbox(displayName, str + "@" + str2));
					fetchReader.GetReader().ReadSpecifiedLength(1);
					fetchReader.GetReader().ReadToFirstChar();
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00069EA8 File Offset: 0x00068EA8
		private static string ConstructAddresses(Mail_t_Mailbox[] mailboxes, MIME_Encoding_EncodedWord wordEncoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			foreach (Mail_t_Mailbox address in mailboxes)
			{
				stringBuilder.Append(IMAP_Envelope.ConstructAddress(address, wordEncoder));
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00069F08 File Offset: 0x00068F08
		private static string ConstructAddress(Mail_t_Mailbox address, MIME_Encoding_EncodedWord wordEncoder)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			bool flag = address.DisplayName != null;
			if (flag)
			{
				stringBuilder.Append(TextUtils.QuoteString(wordEncoder.Encode(IMAP_Envelope.RemoveCrlf(address.DisplayName))));
			}
			else
			{
				stringBuilder.Append("NIL");
			}
			stringBuilder.Append(" NIL");
			stringBuilder.Append(" " + TextUtils.QuoteString(wordEncoder.Encode(IMAP_Envelope.RemoveCrlf(address.LocalPart))));
			bool flag2 = address.Domain != null;
			if (flag2)
			{
				stringBuilder.Append(" " + TextUtils.QuoteString(wordEncoder.Encode(IMAP_Envelope.RemoveCrlf(address.Domain))));
			}
			else
			{
				stringBuilder.Append(" NIL");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00069FF4 File Offset: 0x00068FF4
		private static string RemoveCrlf(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			return value.Replace("\r", "").Replace("\n", "");
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x0006A03C File Offset: 0x0006903C
		private static string ReadAndDecodeWord(string text)
		{
			bool flag = text == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = string.Equals(text, "NIL", StringComparison.InvariantCultureIgnoreCase);
				if (flag2)
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

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x0006A07C File Offset: 0x0006907C
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x0006A094 File Offset: 0x00069094
		public string Subject
		{
			get
			{
				return this.m_Subject;
			}
		}

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x0006A0AC File Offset: 0x000690AC
		public Mail_t_Address[] From
		{
			get
			{
				return this.m_pFrom;
			}
		}

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x0600113E RID: 4414 RVA: 0x0006A0C4 File Offset: 0x000690C4
		public Mail_t_Address[] Sender
		{
			get
			{
				return this.m_pSender;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x0600113F RID: 4415 RVA: 0x0006A0DC File Offset: 0x000690DC
		public Mail_t_Address[] ReplyTo
		{
			get
			{
				return this.m_pReplyTo;
			}
		}

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x0006A0F4 File Offset: 0x000690F4
		public Mail_t_Address[] To
		{
			get
			{
				return this.m_pTo;
			}
		}

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x0006A10C File Offset: 0x0006910C
		public Mail_t_Address[] Cc
		{
			get
			{
				return this.m_pCc;
			}
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001142 RID: 4418 RVA: 0x0006A124 File Offset: 0x00069124
		public Mail_t_Address[] Bcc
		{
			get
			{
				return this.m_pBcc;
			}
		}

		// Token: 0x170005D7 RID: 1495
		// (get) Token: 0x06001143 RID: 4419 RVA: 0x0006A13C File Offset: 0x0006913C
		public string InReplyTo
		{
			get
			{
				return this.m_InReplyTo;
			}
		}

		// Token: 0x170005D8 RID: 1496
		// (get) Token: 0x06001144 RID: 4420 RVA: 0x0006A154 File Offset: 0x00069154
		public string MessageID
		{
			get
			{
				return this.m_MessageID;
			}
		}

		// Token: 0x040006DA RID: 1754
		private DateTime m_Date = DateTime.MinValue;

		// Token: 0x040006DB RID: 1755
		private string m_Subject = null;

		// Token: 0x040006DC RID: 1756
		private Mail_t_Address[] m_pFrom = null;

		// Token: 0x040006DD RID: 1757
		private Mail_t_Address[] m_pSender = null;

		// Token: 0x040006DE RID: 1758
		private Mail_t_Address[] m_pReplyTo = null;

		// Token: 0x040006DF RID: 1759
		private Mail_t_Address[] m_pTo = null;

		// Token: 0x040006E0 RID: 1760
		private Mail_t_Address[] m_pCc = null;

		// Token: 0x040006E1 RID: 1761
		private Mail_t_Address[] m_pBcc = null;

		// Token: 0x040006E2 RID: 1762
		private string m_InReplyTo = null;

		// Token: 0x040006E3 RID: 1763
		private string m_MessageID = null;
	}
}
