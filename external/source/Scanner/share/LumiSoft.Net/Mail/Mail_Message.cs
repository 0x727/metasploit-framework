using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LumiSoft.Net.IO;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000185 RID: 389
	public class Mail_Message : MIME_Message
	{
		// Token: 0x06000FBB RID: 4027 RVA: 0x000611E4 File Offset: 0x000601E4
		public Mail_Message()
		{
			base.Header.FieldsProvider.HeaderFields.Add("From", typeof(Mail_h_MailboxList));
			base.Header.FieldsProvider.HeaderFields.Add("Sender", typeof(Mail_h_Mailbox));
			base.Header.FieldsProvider.HeaderFields.Add("Reply-To", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("To", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Cc", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Bcc", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-From", typeof(Mail_h_MailboxList));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-Sender", typeof(Mail_h_Mailbox));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-To", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-Cc", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-Bcc", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Resent-Reply-To", typeof(Mail_h_AddressList));
			base.Header.FieldsProvider.HeaderFields.Add("Return-Path", typeof(Mail_h_ReturnPath));
			base.Header.FieldsProvider.HeaderFields.Add("Received", typeof(Mail_h_Received));
			base.Header.FieldsProvider.HeaderFields.Add("Disposition-Notification-To", typeof(Mail_h_MailboxList));
			base.Header.FieldsProvider.HeaderFields.Add("Disposition-Notification-Options", typeof(Mail_h_DispositionNotificationOptions));
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0006144C File Offset: 0x0006044C
		public static Mail_Message Create(Mail_t_Mailbox from, Mail_t_Address[] to, Mail_t_Address[] cc, Mail_t_Address[] bcc, string subject, string text, string html, Mail_t_Attachment[] attachments)
		{
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.MessageID = MIME_Utils.CreateMessageID();
			mail_Message.Date = DateTime.Now;
			bool flag = from != null;
			if (flag)
			{
				mail_Message.From = new Mail_t_MailboxList();
				mail_Message.From.Add(from);
			}
			bool flag2 = to != null;
			if (flag2)
			{
				mail_Message.To = new Mail_t_AddressList();
				foreach (Mail_t_Address value in to)
				{
					mail_Message.To.Add(value);
				}
			}
			mail_Message.Subject = subject;
			bool flag3 = attachments == null || attachments.Length == 0;
			if (flag3)
			{
				bool flag4 = string.IsNullOrEmpty(html);
				if (flag4)
				{
					MIME_b_Text mime_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
					mail_Message.Body = mime_b_Text;
					mime_b_Text.SetText(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text);
				}
				else
				{
					MIME_b_MultipartAlternative mime_b_MultipartAlternative = new MIME_b_MultipartAlternative();
					mail_Message.Body = mime_b_MultipartAlternative;
					mime_b_MultipartAlternative.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					mime_b_MultipartAlternative.BodyParts.Add(MIME_Entity.CreateEntity_Text_Html(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, html));
				}
			}
			else
			{
				bool flag5 = string.IsNullOrEmpty(html);
				if (flag5)
				{
					MIME_b_MultipartMixed mime_b_MultipartMixed = new MIME_b_MultipartMixed();
					mail_Message.Body = mime_b_MultipartMixed;
					mime_b_MultipartMixed.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					foreach (Mail_t_Attachment mail_t_Attachment in attachments)
					{
						try
						{
							mime_b_MultipartMixed.BodyParts.Add(MIME_Entity.CreateEntity_Attachment(mail_t_Attachment.Name, mail_t_Attachment.GetStream()));
						}
						finally
						{
							mail_t_Attachment.CloseStream();
						}
					}
				}
				else
				{
					MIME_b_MultipartMixed mime_b_MultipartMixed2 = new MIME_b_MultipartMixed();
					mail_Message.Body = mime_b_MultipartMixed2;
					MIME_Entity mime_Entity = new MIME_Entity();
					MIME_b_MultipartAlternative mime_b_MultipartAlternative2 = new MIME_b_MultipartAlternative();
					mime_Entity.Body = mime_b_MultipartAlternative2;
					mime_b_MultipartMixed2.BodyParts.Add(mime_Entity);
					mime_b_MultipartAlternative2.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					mime_b_MultipartAlternative2.BodyParts.Add(MIME_Entity.CreateEntity_Text_Html(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, html));
					foreach (Mail_t_Attachment mail_t_Attachment2 in attachments)
					{
						try
						{
							mime_b_MultipartMixed2.BodyParts.Add(MIME_Entity.CreateEntity_Attachment(mail_t_Attachment2.Name, mail_t_Attachment2.GetStream()));
						}
						finally
						{
							mail_t_Attachment2.CloseStream();
						}
					}
				}
			}
			return mail_Message;
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x00061718 File Offset: 0x00060718
		public static Mail_Message Create_MultipartSigned(X509Certificate2 signerCert, Mail_t_Mailbox from, Mail_t_Address[] to, Mail_t_Address[] cc, Mail_t_Address[] bcc, string subject, string text, string html, Mail_t_Attachment[] attachments)
		{
			bool flag = signerCert == null;
			if (flag)
			{
				throw new ArgumentNullException("signerCert");
			}
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.MimeVersion = "1.0";
			mail_Message.MessageID = MIME_Utils.CreateMessageID();
			mail_Message.Date = DateTime.Now;
			bool flag2 = from != null;
			if (flag2)
			{
				mail_Message.From = new Mail_t_MailboxList();
				mail_Message.From.Add(from);
			}
			bool flag3 = to != null;
			if (flag3)
			{
				mail_Message.To = new Mail_t_AddressList();
				foreach (Mail_t_Address value in to)
				{
					mail_Message.To.Add(value);
				}
			}
			mail_Message.Subject = subject;
			bool flag4 = attachments == null || attachments.Length == 0;
			if (flag4)
			{
				bool flag5 = string.IsNullOrEmpty(html);
				if (flag5)
				{
					MIME_b_MultipartSigned mime_b_MultipartSigned = new MIME_b_MultipartSigned();
					mail_Message.Body = mime_b_MultipartSigned;
					mime_b_MultipartSigned.SetCertificate(signerCert);
					mime_b_MultipartSigned.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
				}
				else
				{
					MIME_b_MultipartSigned mime_b_MultipartSigned2 = new MIME_b_MultipartSigned();
					mail_Message.Body = mime_b_MultipartSigned2;
					mime_b_MultipartSigned2.SetCertificate(signerCert);
					MIME_Entity mime_Entity = new MIME_Entity();
					MIME_b_MultipartAlternative mime_b_MultipartAlternative = new MIME_b_MultipartAlternative();
					mime_Entity.Body = mime_b_MultipartAlternative;
					mime_b_MultipartSigned2.BodyParts.Add(mime_Entity);
					mime_b_MultipartAlternative.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					mime_b_MultipartAlternative.BodyParts.Add(MIME_Entity.CreateEntity_Text_Html(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, html));
				}
			}
			else
			{
				bool flag6 = string.IsNullOrEmpty(html);
				if (flag6)
				{
					MIME_b_MultipartSigned mime_b_MultipartSigned3 = new MIME_b_MultipartSigned();
					mail_Message.Body = mime_b_MultipartSigned3;
					mime_b_MultipartSigned3.SetCertificate(signerCert);
					MIME_Entity mime_Entity2 = new MIME_Entity();
					MIME_b_MultipartMixed mime_b_MultipartMixed = new MIME_b_MultipartMixed();
					mime_Entity2.Body = mime_b_MultipartMixed;
					mime_b_MultipartSigned3.BodyParts.Add(mime_Entity2);
					mime_b_MultipartMixed.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					foreach (Mail_t_Attachment mail_t_Attachment in attachments)
					{
						try
						{
							mime_b_MultipartMixed.BodyParts.Add(MIME_Entity.CreateEntity_Attachment(mail_t_Attachment.Name, mail_t_Attachment.GetStream()));
						}
						finally
						{
							mail_t_Attachment.CloseStream();
						}
					}
				}
				else
				{
					MIME_b_MultipartSigned mime_b_MultipartSigned4 = new MIME_b_MultipartSigned();
					mail_Message.Body = mime_b_MultipartSigned4;
					mime_b_MultipartSigned4.SetCertificate(signerCert);
					MIME_Entity mime_Entity3 = new MIME_Entity();
					MIME_b_MultipartMixed mime_b_MultipartMixed2 = new MIME_b_MultipartMixed();
					mime_Entity3.Body = mime_b_MultipartMixed2;
					mime_b_MultipartSigned4.BodyParts.Add(mime_Entity3);
					MIME_Entity mime_Entity4 = new MIME_Entity();
					MIME_b_MultipartAlternative mime_b_MultipartAlternative2 = new MIME_b_MultipartAlternative();
					mime_Entity4.Body = mime_b_MultipartAlternative2;
					mime_b_MultipartMixed2.BodyParts.Add(mime_Entity4);
					mime_b_MultipartAlternative2.BodyParts.Add(MIME_Entity.CreateEntity_Text_Plain(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, text));
					mime_b_MultipartAlternative2.BodyParts.Add(MIME_Entity.CreateEntity_Text_Html(MIME_TransferEncodings.QuotedPrintable, Encoding.UTF8, html));
					foreach (Mail_t_Attachment mail_t_Attachment2 in attachments)
					{
						try
						{
							mime_b_MultipartMixed2.BodyParts.Add(MIME_Entity.CreateEntity_Attachment(mail_t_Attachment2.Name, mail_t_Attachment2.GetStream()));
						}
						finally
						{
							mail_t_Attachment2.CloseStream();
						}
					}
				}
			}
			return mail_Message;
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x00061A98 File Offset: 0x00060A98
		public static Mail_Message ParseFromByte(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			return Mail_Message.ParseFromStream(new MemoryStream(data));
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00061ACC File Offset: 0x00060ACC
		public static Mail_Message ParseFromByte(byte[] data, Encoding headerEncoding)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = headerEncoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("headerEncoding");
			}
			return Mail_Message.ParseFromStream(new MemoryStream(data), headerEncoding);
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x00061B14 File Offset: 0x00060B14
		public new static Mail_Message ParseFromFile(string file)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = file == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'file' value must be specified.");
			}
			Mail_Message result;
			using (FileStream fileStream = File.OpenRead(file))
			{
				result = Mail_Message.ParseFromStream(fileStream);
			}
			return result;
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x00061B80 File Offset: 0x00060B80
		public new static Mail_Message ParseFromFile(string file, Encoding headerEncoding)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = file == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'file' value must be specified.");
			}
			bool flag3 = headerEncoding == null;
			if (flag3)
			{
				throw new ArgumentNullException("headerEncoding");
			}
			Mail_Message result;
			using (FileStream fileStream = File.OpenRead(file))
			{
				result = Mail_Message.ParseFromStream(fileStream, headerEncoding);
			}
			return result;
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00061C04 File Offset: 0x00060C04
		public new static Mail_Message ParseFromStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			return Mail_Message.ParseFromStream(stream, Encoding.UTF8);
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x00061C38 File Offset: 0x00060C38
		public new static Mail_Message ParseFromStream(Stream stream, Encoding headerEncoding)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = headerEncoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("headerEncoding");
			}
			Mail_Message mail_Message = new Mail_Message();
			mail_Message.Parse(new SmartStream(stream, false), headerEncoding, new MIME_h_ContentType("text/plain"));
			return mail_Message;
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x00061C94 File Offset: 0x00060C94
		public Mail_Message Clone()
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			MemoryStreamEx memoryStreamEx = new MemoryStreamEx(64000);
			base.ToStream(memoryStreamEx, null, null);
			memoryStreamEx.Position = 0L;
			return Mail_Message.ParseFromStream(memoryStreamEx);
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x00061CE8 File Offset: 0x00060CE8
		public MIME_Entity[] GetAttachments(bool includeInline)
		{
			return this.GetAttachments(includeInline, true);
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x00061D04 File Offset: 0x00060D04
		public MIME_Entity[] GetAttachments(bool includeInline, bool includeEmbbedMessage)
		{
			bool isDisposed = base.IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			List<MIME_Entity> list = new List<MIME_Entity>();
			foreach (MIME_Entity mime_Entity in base.GetAllEntities(includeEmbbedMessage))
			{
				MIME_h_ContentType mime_h_ContentType = null;
				try
				{
					mime_h_ContentType = mime_Entity.ContentType;
				}
				catch
				{
				}
				MIME_h_ContentDisposition mime_h_ContentDisposition = null;
				try
				{
					mime_h_ContentDisposition = mime_Entity.ContentDisposition;
				}
				catch
				{
				}
				bool flag = mime_h_ContentDisposition != null && string.Equals(mime_h_ContentDisposition.DispositionType, "attachment", StringComparison.InvariantCultureIgnoreCase);
				if (flag)
				{
					list.Add(mime_Entity);
				}
				else
				{
					bool flag2 = mime_h_ContentDisposition != null && string.Equals(mime_h_ContentDisposition.DispositionType, "inline", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						if (includeInline)
						{
							list.Add(mime_Entity);
						}
					}
					else
					{
						bool flag3 = mime_h_ContentType != null && mime_h_ContentType.Type.ToLower() == "application";
						if (flag3)
						{
							list.Add(mime_Entity);
						}
						else
						{
							bool flag4 = mime_h_ContentType != null && mime_h_ContentType.Type.ToLower() == "image";
							if (flag4)
							{
								list.Add(mime_Entity);
							}
							else
							{
								bool flag5 = mime_h_ContentType != null && mime_h_ContentType.Type.ToLower() == "video";
								if (flag5)
								{
									list.Add(mime_Entity);
								}
								else
								{
									bool flag6 = mime_h_ContentType != null && mime_h_ContentType.Type.ToLower() == "audio";
									if (flag6)
									{
										list.Add(mime_Entity);
									}
									else
									{
										bool flag7 = mime_h_ContentType != null && mime_h_ContentType.Type.ToLower() == "message";
										if (flag7)
										{
											list.Add(mime_Entity);
										}
									}
								}
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x00061F08 File Offset: 0x00060F08
		// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x00061F88 File Offset: 0x00060F88
		public DateTime Date
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Date");
				bool flag = first != null;
				if (flag)
				{
					try
					{
						return MIME_Utils.ParseRfc2822DateTime(((MIME_h_Unstructured)first).Value);
					}
					catch
					{
						throw new ParseException("Header field 'Date' parsing failed.");
					}
				}
				return DateTime.MinValue;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					base.Header.RemoveAll("Date");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Date");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Date", MIME_Utils.DateTimeToRfc2822(value)));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Date", MIME_Utils.DateTimeToRfc2822(value)));
					}
				}
			}
		}

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x00062030 File Offset: 0x00061030
		// (set) Token: 0x06000FCA RID: 4042 RVA: 0x000620A8 File Offset: 0x000610A8
		public Mail_t_MailboxList From
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("From");
				bool flag = first != null;
				Mail_t_MailboxList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_MailboxList);
					if (flag2)
					{
						throw new ParseException("Header field 'From' parsing failed.");
					}
					result = ((Mail_h_MailboxList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("From");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("From");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_MailboxList("From", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_MailboxList("From", value));
					}
				}
			}
		}

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x00062140 File Offset: 0x00061140
		// (set) Token: 0x06000FCC RID: 4044 RVA: 0x000621B8 File Offset: 0x000611B8
		public Mail_t_Mailbox Sender
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Sender");
				bool flag = first != null;
				Mail_t_Mailbox result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_Mailbox);
					if (flag2)
					{
						throw new ParseException("Header field 'Sender' parsing failed.");
					}
					result = ((Mail_h_Mailbox)first).Address;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Sender");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Sender");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_Mailbox("Sender", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_Mailbox("Sender", value));
					}
				}
			}
		}

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06000FCD RID: 4045 RVA: 0x00062250 File Offset: 0x00061250
		// (set) Token: 0x06000FCE RID: 4046 RVA: 0x000622C8 File Offset: 0x000612C8
		public Mail_t_AddressList ReplyTo
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Reply-To");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Reply-To' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Reply-To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Reply-To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Reply-To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Reply-To", value));
					}
				}
			}
		}

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06000FCF RID: 4047 RVA: 0x00062360 File Offset: 0x00061360
		// (set) Token: 0x06000FD0 RID: 4048 RVA: 0x000623D8 File Offset: 0x000613D8
		public Mail_t_AddressList To
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("To");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'To' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("To", value));
					}
				}
			}
		}

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x00062470 File Offset: 0x00061470
		// (set) Token: 0x06000FD2 RID: 4050 RVA: 0x000624E8 File Offset: 0x000614E8
		public Mail_t_AddressList Cc
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Cc");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Cc' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Cc");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Cc");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Cc", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Cc", value));
					}
				}
			}
		}

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x00062580 File Offset: 0x00061580
		// (set) Token: 0x06000FD4 RID: 4052 RVA: 0x000625F8 File Offset: 0x000615F8
		public Mail_t_AddressList Bcc
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Bcc");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Bcc' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Bcc");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Bcc");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Bcc", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Bcc", value));
					}
				}
			}
		}

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00062690 File Offset: 0x00061690
		// (set) Token: 0x06000FD6 RID: 4054 RVA: 0x000626E8 File Offset: 0x000616E8
		public string MessageID
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Message-ID");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Message-ID");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Message-ID");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Message-ID", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Message-ID", value));
					}
				}
			}
		}

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x00062780 File Offset: 0x00061780
		// (set) Token: 0x06000FD8 RID: 4056 RVA: 0x000627D8 File Offset: 0x000617D8
		public string InReplyTo
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("In-Reply-To");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("In-Reply-To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("In-Reply-To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("In-Reply-To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("In-Reply-To", value));
					}
				}
			}
		}

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x06000FD9 RID: 4057 RVA: 0x00062870 File Offset: 0x00061870
		// (set) Token: 0x06000FDA RID: 4058 RVA: 0x000628C8 File Offset: 0x000618C8
		public string References
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("References");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("References");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("References");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("References", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("References", value));
					}
				}
			}
		}

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06000FDB RID: 4059 RVA: 0x00062960 File Offset: 0x00061960
		// (set) Token: 0x06000FDC RID: 4060 RVA: 0x000629B8 File Offset: 0x000619B8
		public string Subject
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Subject");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Subject");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Subject");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Subject", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Subject", value));
					}
				}
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06000FDD RID: 4061 RVA: 0x00062A50 File Offset: 0x00061A50
		// (set) Token: 0x06000FDE RID: 4062 RVA: 0x00062AA8 File Offset: 0x00061AA8
		public string Comments
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Comments");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Comments");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Comments");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Comments", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Comments", value));
					}
				}
			}
		}

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06000FDF RID: 4063 RVA: 0x00062B40 File Offset: 0x00061B40
		// (set) Token: 0x06000FE0 RID: 4064 RVA: 0x00062B98 File Offset: 0x00061B98
		public string Keywords
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Keywords");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Keywords");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Keywords");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Keywords", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Keywords", value));
					}
				}
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06000FE1 RID: 4065 RVA: 0x00062C30 File Offset: 0x00061C30
		// (set) Token: 0x06000FE2 RID: 4066 RVA: 0x00062CB0 File Offset: 0x00061CB0
		public DateTime ResentDate
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Date");
				bool flag = first != null;
				if (flag)
				{
					try
					{
						return MIME_Utils.ParseRfc2822DateTime(((MIME_h_Unstructured)first).Value);
					}
					catch
					{
						throw new ParseException("Header field 'Resent-Date' parsing failed.");
					}
				}
				return DateTime.MinValue;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == DateTime.MinValue;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Date");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Date");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Resent-Date", MIME_Utils.DateTimeToRfc2822(value)));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Resent-Date", MIME_Utils.DateTimeToRfc2822(value)));
					}
				}
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06000FE3 RID: 4067 RVA: 0x00062D58 File Offset: 0x00061D58
		// (set) Token: 0x06000FE4 RID: 4068 RVA: 0x00062DD0 File Offset: 0x00061DD0
		public Mail_t_MailboxList ResentFrom
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-From");
				bool flag = first != null;
				Mail_t_MailboxList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_MailboxList);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-From' parsing failed.");
					}
					result = ((Mail_h_MailboxList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-From");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-From");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_MailboxList("Resent-From", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_MailboxList("Resent-From", value));
					}
				}
			}
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x00062E68 File Offset: 0x00061E68
		// (set) Token: 0x06000FE6 RID: 4070 RVA: 0x00062EE0 File Offset: 0x00061EE0
		public Mail_t_Mailbox ResentSender
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Sender");
				bool flag = first != null;
				Mail_t_Mailbox result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_Mailbox);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-Sender' parsing failed.");
					}
					result = ((Mail_h_Mailbox)first).Address;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Sender");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Sender");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_Mailbox("Resent-Sender", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_Mailbox("Resent-Sender", value));
					}
				}
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x00062F78 File Offset: 0x00061F78
		// (set) Token: 0x06000FE8 RID: 4072 RVA: 0x00062FF0 File Offset: 0x00061FF0
		public Mail_t_AddressList ResentTo
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-To");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-To' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Resent-To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Resent-To", value));
					}
				}
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x00063088 File Offset: 0x00062088
		// (set) Token: 0x06000FEA RID: 4074 RVA: 0x00063100 File Offset: 0x00062100
		public Mail_t_AddressList ResentCc
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Cc");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-Cc' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Cc");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Cc");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Resent-Cc", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Resent-Cc", value));
					}
				}
			}
		}

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06000FEB RID: 4075 RVA: 0x00063198 File Offset: 0x00062198
		// (set) Token: 0x06000FEC RID: 4076 RVA: 0x00063210 File Offset: 0x00062210
		public Mail_t_AddressList ResentBcc
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Bcc");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-Bcc' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Bcc");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Bcc");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Resent-Bcc", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Resent-Bcc", value));
					}
				}
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06000FED RID: 4077 RVA: 0x000632A8 File Offset: 0x000622A8
		// (set) Token: 0x06000FEE RID: 4078 RVA: 0x00063320 File Offset: 0x00062320
		public Mail_t_AddressList ResentReplyTo
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Reply-To");
				bool flag = first != null;
				Mail_t_AddressList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_AddressList);
					if (flag2)
					{
						throw new ParseException("Header field 'Resent-Reply-To' parsing failed.");
					}
					result = ((Mail_h_AddressList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Reply-To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Reply-To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_AddressList("Resent-Reply-To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_AddressList("Resent-Reply-To", value));
					}
				}
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06000FEF RID: 4079 RVA: 0x000633B8 File Offset: 0x000623B8
		// (set) Token: 0x06000FF0 RID: 4080 RVA: 0x00063410 File Offset: 0x00062410
		public string ResentMessageID
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Resent-Message-ID");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Resent-Message-ID");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Resent-Message-ID");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Resent-Message-ID", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Resent-Message-ID", value));
					}
				}
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x000634A8 File Offset: 0x000624A8
		// (set) Token: 0x06000FF2 RID: 4082 RVA: 0x0006351C File Offset: 0x0006251C
		public Mail_h_ReturnPath ReturnPath
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Return-Path");
				bool flag = first != null;
				Mail_h_ReturnPath result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_ReturnPath);
					if (flag2)
					{
						throw new ParseException("Header field 'Return-Path' parsing failed.");
					}
					result = (Mail_h_ReturnPath)first;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Return-Path");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Return-Path");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(value);
					}
					else
					{
						base.Header.ReplaceFirst(value);
					}
				}
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x000635A0 File Offset: 0x000625A0
		public Mail_h_Received[] Received
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h[] array = base.Header["Received"];
				bool flag = array != null;
				Mail_h_Received[] result;
				if (flag)
				{
					List<Mail_h_Received> list = new List<Mail_h_Received>();
					for (int i = 0; i < array.Length; i++)
					{
						bool flag2 = !(array[i] is Mail_h_Received);
						if (flag2)
						{
							throw new ParseException("Header field 'Received' parsing failed.");
						}
						list.Add((Mail_h_Received)array[i]);
					}
					result = list.ToArray();
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x00063648 File Offset: 0x00062648
		// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x000636C0 File Offset: 0x000626C0
		public Mail_t_MailboxList DispositionNotificationTo
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Disposition-Notification-To");
				bool flag = first != null;
				Mail_t_MailboxList result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_MailboxList);
					if (flag2)
					{
						throw new ParseException("Header field 'From' parsing failed.");
					}
					result = ((Mail_h_MailboxList)first).Addresses;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Disposition-Notification-To");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Disposition-Notification-To");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new Mail_h_MailboxList("Disposition-Notification-To", value));
					}
					else
					{
						base.Header.ReplaceFirst(new Mail_h_MailboxList("Disposition-Notification-To", value));
					}
				}
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x00063758 File Offset: 0x00062758
		// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x000637CC File Offset: 0x000627CC
		public Mail_h_DispositionNotificationOptions DispositionNotificationOptions
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Disposition-Notification-Options");
				bool flag = first != null;
				Mail_h_DispositionNotificationOptions result;
				if (flag)
				{
					bool flag2 = !(first is Mail_h_DispositionNotificationOptions);
					if (flag2)
					{
						throw new ParseException("Header field 'Disposition-Notification-Options' parsing failed.");
					}
					result = (Mail_h_DispositionNotificationOptions)first;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Disposition-Notification-Options");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Disposition-Notification-Options");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(value);
					}
					else
					{
						base.Header.ReplaceFirst(value);
					}
				}
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x00063850 File Offset: 0x00062850
		// (set) Token: 0x06000FF9 RID: 4089 RVA: 0x000638A8 File Offset: 0x000628A8
		public string AcceptLanguage
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Accept-Language");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Accept-Language");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Accept-Language");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Accept-Language", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Accept-Language", value));
					}
				}
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x06000FFA RID: 4090 RVA: 0x00063940 File Offset: 0x00062940
		// (set) Token: 0x06000FFB RID: 4091 RVA: 0x00063998 File Offset: 0x00062998
		public string OriginalMessageID
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Original-Message-ID");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Original-Message-ID");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Original-Message-ID");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Original-Message-ID", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Original-Message-ID", value));
					}
				}
			}
		}

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x00063A30 File Offset: 0x00062A30
		// (set) Token: 0x06000FFD RID: 4093 RVA: 0x00063A88 File Offset: 0x00062A88
		public string PICSLabel
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("PICS-Label");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("PICS-LabelD");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("PICS-Label");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("PICS-Label", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("PICS-Label", value));
					}
				}
			}
		}

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x00063B20 File Offset: 0x00062B20
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x00063B78 File Offset: 0x00062B78
		public string ListArchive
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Archive");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Archive");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Archive");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Archive", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Archive", value));
					}
				}
			}
		}

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x00063C10 File Offset: 0x00062C10
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x00063C68 File Offset: 0x00062C68
		public string ListHelp
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Help");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Help");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Help");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Help", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Help", value));
					}
				}
			}
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x00063D00 File Offset: 0x00062D00
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x00063D58 File Offset: 0x00062D58
		public string ListID
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-ID");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-ID");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-ID");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-ID", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-ID", value));
					}
				}
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x00063DF0 File Offset: 0x00062DF0
		// (set) Token: 0x06001005 RID: 4101 RVA: 0x00063E48 File Offset: 0x00062E48
		public string ListOwner
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Owner");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Owner");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Owner");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Owner", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Owner", value));
					}
				}
			}
		}

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x00063EE0 File Offset: 0x00062EE0
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x00063F38 File Offset: 0x00062F38
		public string ListPost
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Post");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Post");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Post");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Post", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Post", value));
					}
				}
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x00063FD0 File Offset: 0x00062FD0
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x00064028 File Offset: 0x00063028
		public string ListSubscribe
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Subscribe");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Subscribe");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Subscribe");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Subscribe", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Subscribe", value));
					}
				}
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x000640C0 File Offset: 0x000630C0
		// (set) Token: 0x0600100B RID: 4107 RVA: 0x00064118 File Offset: 0x00063118
		public string ListUnsubscribe
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("List-Unsubscribe");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("List-Unsubscribe");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("List-Unsubscribe");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("List-Unsubscribe", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("List-Unsubscribe", value));
					}
				}
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x000641B0 File Offset: 0x000631B0
		// (set) Token: 0x0600100D RID: 4109 RVA: 0x00064208 File Offset: 0x00063208
		public string MessageContext
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Message-Context");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Message-Context");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Message-Context");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Message-Context", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Message-Context", value));
					}
				}
			}
		}

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x0600100E RID: 4110 RVA: 0x000642A0 File Offset: 0x000632A0
		// (set) Token: 0x0600100F RID: 4111 RVA: 0x000642F8 File Offset: 0x000632F8
		public string Importance
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Importance");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Importance");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Importance");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Importance", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Importance", value));
					}
				}
			}
		}

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x06001010 RID: 4112 RVA: 0x00064390 File Offset: 0x00063390
		// (set) Token: 0x06001011 RID: 4113 RVA: 0x000643E8 File Offset: 0x000633E8
		public string Priority
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = base.Header.GetFirst("Priority");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					base.Header.RemoveAll("Priority");
				}
				else
				{
					MIME_h first = base.Header.GetFirst("Priority");
					bool flag2 = first == null;
					if (flag2)
					{
						base.Header.Add(new MIME_h_Unstructured("Priority", value));
					}
					else
					{
						base.Header.ReplaceFirst(new MIME_h_Unstructured("Priority", value));
					}
				}
			}
		}

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x06001012 RID: 4114 RVA: 0x00064480 File Offset: 0x00063480
		public MIME_Entity[] Attachments
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.GetAttachments(false);
			}
		}

		// Token: 0x17000567 RID: 1383
		// (get) Token: 0x06001013 RID: 4115 RVA: 0x000644B8 File Offset: 0x000634B8
		public Encoding BodyTextEncoding
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (MIME_Entity mime_Entity in base.GetAllEntities(false))
				{
					bool flag = mime_Entity.Body.MediaType.ToLower() == MIME_MediaTypes.Text.plain;
					if (flag)
					{
						return ((MIME_b_Text)mime_Entity.Body).GetCharset();
					}
				}
				return null;
			}
		}

		// Token: 0x17000568 RID: 1384
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x0006453C File Offset: 0x0006353C
		public string BodyText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (MIME_Entity mime_Entity in base.GetAllEntities(false))
				{
					bool flag = mime_Entity.Body.MediaType.ToLower() == MIME_MediaTypes.Text.plain;
					if (flag)
					{
						return ((MIME_b_Text)mime_Entity.Body).Text;
					}
				}
				return null;
			}
		}

		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001015 RID: 4117 RVA: 0x000645C0 File Offset: 0x000635C0
		public Encoding BodyHtmlTextEncoding
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (MIME_Entity mime_Entity in base.GetAllEntities(false))
				{
					bool flag = mime_Entity.Body.MediaType.ToLower() == MIME_MediaTypes.Text.html;
					if (flag)
					{
						return ((MIME_b_Text)mime_Entity.Body).GetCharset();
					}
				}
				return null;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001016 RID: 4118 RVA: 0x00064644 File Offset: 0x00063644
		public string BodyHtmlText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (MIME_Entity mime_Entity in base.GetAllEntities(false))
				{
					bool flag = mime_Entity.Body.MediaType.ToLower() == MIME_MediaTypes.Text.html;
					if (flag)
					{
						return ((MIME_b_Text)mime_Entity.Body).Text;
					}
				}
				return null;
			}
		}
	}
}
