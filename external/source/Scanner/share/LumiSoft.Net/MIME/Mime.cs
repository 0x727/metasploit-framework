using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000165 RID: 357
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class Mime
	{
		// Token: 0x06000E4F RID: 3663 RVA: 0x000582E8 File Offset: 0x000572E8
		public Mime()
		{
			this.m_pMainEntity = new MimeEntity();
			this.m_pMainEntity.MessageID = MimeUtils.CreateMessageID();
			this.m_pMainEntity.Date = DateTime.Now;
			this.m_pMainEntity.MimeVersion = "1.0";
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00058344 File Offset: 0x00057344
		public static Mime Parse(byte[] data)
		{
			Mime result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = Mime.Parse(memoryStream);
			}
			return result;
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00058380 File Offset: 0x00057380
		public static Mime Parse(string fileName)
		{
			Mime result;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				result = Mime.Parse(fileStream);
			}
			return result;
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x000583BC File Offset: 0x000573BC
		public static Mime Parse(Stream stream)
		{
			Mime mime = new Mime();
			mime.MainEntity.Parse(new SmartStream(stream, false), null);
			return mime;
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x000583EC File Offset: 0x000573EC
		public static Mime CreateSimple(AddressList from, AddressList to, string subject, string bodyText, string bodyHtml)
		{
			return Mime.CreateSimple(from, to, subject, bodyText, bodyHtml, null);
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x0005840C File Offset: 0x0005740C
		public static Mime CreateSimple(AddressList from, AddressList to, string subject, string bodyText, string bodyHtml, string[] attachmentFileNames)
		{
			Mime mime = new Mime();
			MimeEntity mainEntity = mime.MainEntity;
			mainEntity.From = from;
			mainEntity.To = to;
			mainEntity.Subject = subject;
			bool flag = attachmentFileNames == null || attachmentFileNames.Length == 0;
			if (flag)
			{
				bool flag2 = bodyText != null && bodyHtml != null;
				if (flag2)
				{
					mainEntity.ContentType = MediaType_enum.Multipart_alternative;
					MimeEntity mimeEntity = mainEntity.ChildEntities.Add();
					mimeEntity.ContentType = MediaType_enum.Text_plain;
					mimeEntity.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
					mimeEntity.DataText = bodyText;
					MimeEntity mimeEntity2 = mainEntity.ChildEntities.Add();
					mimeEntity2.ContentType = MediaType_enum.Text_html;
					mimeEntity2.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
					mimeEntity2.DataText = bodyHtml;
				}
				else
				{
					bool flag3 = bodyText != null;
					if (flag3)
					{
						MimeEntity mimeEntity3 = mainEntity;
						mimeEntity3.ContentType = MediaType_enum.Text_plain;
						mimeEntity3.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
						mimeEntity3.DataText = bodyText;
					}
					else
					{
						bool flag4 = bodyHtml != null;
						if (flag4)
						{
							MimeEntity mimeEntity4 = mainEntity;
							mimeEntity4.ContentType = MediaType_enum.Text_html;
							mimeEntity4.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
							mimeEntity4.DataText = bodyHtml;
						}
					}
				}
			}
			else
			{
				mainEntity.ContentType = MediaType_enum.Multipart_mixed;
				bool flag5 = bodyText != null && bodyHtml != null;
				if (flag5)
				{
					MimeEntity mimeEntity5 = mainEntity.ChildEntities.Add();
					mimeEntity5.ContentType = MediaType_enum.Multipart_alternative;
					MimeEntity mimeEntity6 = mimeEntity5.ChildEntities.Add();
					mimeEntity6.ContentType = MediaType_enum.Text_plain;
					mimeEntity6.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
					mimeEntity6.DataText = bodyText;
					MimeEntity mimeEntity7 = mimeEntity5.ChildEntities.Add();
					mimeEntity7.ContentType = MediaType_enum.Text_html;
					mimeEntity7.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
					mimeEntity7.DataText = bodyHtml;
				}
				else
				{
					bool flag6 = bodyText != null;
					if (flag6)
					{
						MimeEntity mimeEntity8 = mainEntity.ChildEntities.Add();
						mimeEntity8.ContentType = MediaType_enum.Text_plain;
						mimeEntity8.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
						mimeEntity8.DataText = bodyText;
					}
					else
					{
						bool flag7 = bodyHtml != null;
						if (flag7)
						{
							MimeEntity mimeEntity9 = mainEntity.ChildEntities.Add();
							mimeEntity9.ContentType = MediaType_enum.Text_html;
							mimeEntity9.ContentTransferEncoding = ContentTransferEncoding_enum.QuotedPrintable;
							mimeEntity9.DataText = bodyHtml;
						}
					}
				}
				foreach (string text in attachmentFileNames)
				{
					MimeEntity mimeEntity10 = mainEntity.ChildEntities.Add();
					mimeEntity10.ContentType = MediaType_enum.Application_octet_stream;
					mimeEntity10.ContentDisposition = ContentDisposition_enum.Attachment;
					mimeEntity10.ContentTransferEncoding = ContentTransferEncoding_enum.Base64;
					mimeEntity10.ContentDisposition_FileName = Core.GetFileNameFromPath(text);
					mimeEntity10.DataFromFile(text);
				}
			}
			return mime;
		}

		// Token: 0x06000E55 RID: 3669 RVA: 0x0005868C File Offset: 0x0005768C
		public string ToStringData()
		{
			return Encoding.Default.GetString(this.ToByteData());
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x000586B0 File Offset: 0x000576B0
		public byte[] ToByteData()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.ToStream(memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x000586F4 File Offset: 0x000576F4
		public void ToFile(string fileName)
		{
			using (FileStream fileStream = File.Create(fileName))
			{
				this.ToStream(fileStream);
			}
		}

		// Token: 0x06000E58 RID: 3672 RVA: 0x00058730 File Offset: 0x00057730
		public void ToStream(Stream storeStream)
		{
			this.m_pMainEntity.ToStream(storeStream);
		}

		// Token: 0x06000E59 RID: 3673 RVA: 0x00058740 File Offset: 0x00057740
		private void GetEntities(MimeEntityCollection entities, List<MimeEntity> allEntries)
		{
			bool flag = entities != null;
			if (flag)
			{
				foreach (object obj in entities)
				{
					MimeEntity mimeEntity = (MimeEntity)obj;
					allEntries.Add(mimeEntity);
					bool flag2 = mimeEntity.ChildEntities.Count > 0;
					if (flag2)
					{
						this.GetEntities(mimeEntity.ChildEntities, allEntries);
					}
				}
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06000E5A RID: 3674 RVA: 0x000587CC File Offset: 0x000577CC
		public MimeEntity MainEntity
		{
			get
			{
				return this.m_pMainEntity;
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06000E5B RID: 3675 RVA: 0x000587E4 File Offset: 0x000577E4
		public MimeEntity[] MimeEntities
		{
			get
			{
				List<MimeEntity> list = new List<MimeEntity>();
				list.Add(this.m_pMainEntity);
				this.GetEntities(this.m_pMainEntity.ChildEntities, list);
				return list.ToArray();
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00058824 File Offset: 0x00057824
		public MimeEntity[] Attachments
		{
			get
			{
				List<MimeEntity> list = new List<MimeEntity>();
				MimeEntity[] mimeEntities = this.MimeEntities;
				foreach (MimeEntity mimeEntity in mimeEntities)
				{
					bool flag = mimeEntity.ContentDisposition == ContentDisposition_enum.Attachment;
					if (flag)
					{
						list.Add(mimeEntity);
					}
					else
					{
						bool flag2 = mimeEntity.ContentType_Name != null;
						if (flag2)
						{
							list.Add(mimeEntity);
						}
						else
						{
							bool flag3 = mimeEntity.ContentDisposition_FileName != null;
							if (flag3)
							{
								list.Add(mimeEntity);
							}
						}
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06000E5D RID: 3677 RVA: 0x000588B8 File Offset: 0x000578B8
		public string BodyText
		{
			get
			{
				bool flag = this.MainEntity.ContentType == MediaType_enum.NotSpecified;
				if (flag)
				{
					bool flag2 = this.MainEntity.DataEncoded != null;
					if (flag2)
					{
						return Encoding.ASCII.GetString(this.MainEntity.Data);
					}
				}
				else
				{
					MimeEntity[] mimeEntities = this.MimeEntities;
					foreach (MimeEntity mimeEntity in mimeEntities)
					{
						bool flag3 = mimeEntity.ContentType == MediaType_enum.Text_plain;
						if (flag3)
						{
							return mimeEntity.DataText;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06000E5E RID: 3678 RVA: 0x00058954 File Offset: 0x00057954
		public string BodyHtml
		{
			get
			{
				MimeEntity[] mimeEntities = this.MimeEntities;
				foreach (MimeEntity mimeEntity in mimeEntities)
				{
					bool flag = mimeEntity.ContentType == MediaType_enum.Text_html;
					if (flag)
					{
						return mimeEntity.DataText;
					}
				}
				return null;
			}
		}

		// Token: 0x0400060F RID: 1551
		private MimeEntity m_pMainEntity = null;
	}
}
