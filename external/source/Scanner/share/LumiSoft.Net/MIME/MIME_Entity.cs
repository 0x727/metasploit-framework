using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000115 RID: 277
	public class MIME_Entity : IDisposable
	{
		// Token: 0x06000AAB RID: 2731 RVA: 0x00040C9C File Offset: 0x0003FC9C
		public MIME_Entity()
		{
			this.m_pHeader = new MIME_h_Collection(new MIME_h_Provider());
			this.m_pBodyProvider = new MIME_b_Provider();
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x00040CF0 File Offset: 0x0003FCF0
		public void Dispose()
		{
			lock (this)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					this.m_pHeader = null;
					this.m_pParent = null;
				}
			}
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x00040D4C File Offset: 0x0003FD4C
		public static MIME_Entity CreateEntity_Text_Plain(string transferEncoding, Encoding charset, string text)
		{
			bool flag = transferEncoding == null;
			if (flag)
			{
				throw new ArgumentNullException("transferEncoding");
			}
			bool flag2 = charset == null;
			if (flag2)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ArgumentNullException("text");
			}
			MIME_Entity mime_Entity = new MIME_Entity();
			MIME_b_Text mime_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.plain);
			mime_Entity.Body = mime_b_Text;
			mime_b_Text.SetText(transferEncoding, charset, text);
			return mime_Entity;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x00040DC4 File Offset: 0x0003FDC4
		public static MIME_Entity CreateEntity_Text_Html(string transferEncoding, Encoding charset, string text)
		{
			bool flag = transferEncoding == null;
			if (flag)
			{
				throw new ArgumentNullException("transferEncoding");
			}
			bool flag2 = charset == null;
			if (flag2)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ArgumentNullException("text");
			}
			MIME_Entity mime_Entity = new MIME_Entity();
			MIME_b_Text mime_b_Text = new MIME_b_Text(MIME_MediaTypes.Text.html);
			mime_Entity.Body = mime_b_Text;
			mime_b_Text.SetText(transferEncoding, charset, text);
			return mime_Entity;
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x00040E3C File Offset: 0x0003FE3C
		public static MIME_Entity CreateEntity_Attachment(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			MIME_Entity result;
			using (Stream stream = File.OpenRead(fileName))
			{
				result = MIME_Entity.CreateEntity_Attachment(Path.GetFileName(fileName), stream);
			}
			return result;
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x00040E94 File Offset: 0x0003FE94
		public static MIME_Entity CreateEntity_Attachment(string attachmentName, string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			MIME_Entity result;
			using (Stream stream = File.OpenRead(fileName))
			{
				result = MIME_Entity.CreateEntity_Attachment(string.IsNullOrEmpty(attachmentName) ? Path.GetFileName(fileName) : attachmentName, stream);
			}
			return result;
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x00040EF4 File Offset: 0x0003FEF4
		public static MIME_Entity CreateEntity_Attachment(string attachmentName, Stream stream)
		{
			bool flag = attachmentName == null;
			if (flag)
			{
				throw new ArgumentNullException("attachmentName");
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			long param_Size = stream.CanSeek ? (stream.Length - stream.Position) : -1L;
			MIME_Entity mime_Entity = new MIME_Entity();
			MIME_b_Application mime_b_Application = new MIME_b_Application(MIME_MediaTypes.Application.octet_stream);
			mime_Entity.Body = mime_b_Application;
			mime_b_Application.SetData(stream, MIME_TransferEncodings.Base64);
			mime_Entity.ContentType.Param_Name = Path.GetFileName(attachmentName);
			mime_Entity.ContentDisposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Attachment)
			{
				Param_FileName = Path.GetFileName(attachmentName),
				Param_Size = param_Size
			};
			return mime_Entity;
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x00040FAF File Offset: 0x0003FFAF
		public void ToFile(string file, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
		{
			this.ToFile(file, headerWordEncoder, headerParmetersCharset, false);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x00040FC0 File Offset: 0x0003FFC0
		public void ToFile(string file, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
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
			using (FileStream fileStream = File.Create(file))
			{
				this.ToStream(fileStream, headerWordEncoder, headerParmetersCharset, headerReencode);
			}
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x00041030 File Offset: 0x00040030
		public void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
		{
			this.ToStream(stream, headerWordEncoder, headerParmetersCharset, false);
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x00041040 File Offset: 0x00040040
		public void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pHeader.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
			stream.Write(new byte[]
			{
				13,
				10
			}, 0, 2);
			this.m_pBody.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x000410A0 File Offset: 0x000400A0
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x000410BC File Offset: 0x000400BC
		public string ToString(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
		{
			return this.ToString(headerWordEncoder, headerParmetersCharset, false);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x000410D8 File Offset: 0x000400D8
		public string ToString(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			string @string;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.ToStream(memoryStream, headerWordEncoder, headerParmetersCharset, headerReencode);
				@string = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			return @string;
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x00041128 File Offset: 0x00040128
		public byte[] ToByte(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset)
		{
			return this.ToByte(headerWordEncoder, headerParmetersCharset, false);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x00041144 File Offset: 0x00040144
		public byte[] ToByte(MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.ToStream(memoryStream, headerWordEncoder, headerParmetersCharset, headerReencode);
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x0004118C File Offset: 0x0004018C
		public void DataToStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = this.Body == null;
			if (flag2)
			{
				throw new InvalidOperationException("Mime entity body has been not set yet.");
			}
			bool flag3 = !(this.Body is MIME_b_SinglepartBase);
			if (flag3)
			{
				throw new InvalidOperationException("This method is available only for single part entities, not for multipart.");
			}
			MIME_b_SinglepartBase mime_b_SinglepartBase = (MIME_b_SinglepartBase)this.Body;
			using (Stream dataStream = mime_b_SinglepartBase.GetDataStream())
			{
				Net_Utils.StreamCopy(dataStream, stream, 64000);
			}
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0004122C File Offset: 0x0004022C
		public void DataToFile(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = fileName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'fileName' value must be specified.");
			}
			bool flag3 = this.Body == null;
			if (flag3)
			{
				throw new InvalidOperationException("Mime entity body has been not set yet.");
			}
			bool flag4 = !(this.Body is MIME_b_SinglepartBase);
			if (flag4)
			{
				throw new InvalidOperationException("This method is available only for single part entities, not for multipart.");
			}
			MIME_b_SinglepartBase mime_b_SinglepartBase = (MIME_b_SinglepartBase)this.Body;
			using (Stream stream = File.Create(fileName))
			{
				using (Stream dataStream = mime_b_SinglepartBase.GetDataStream())
				{
					Net_Utils.StreamCopy(dataStream, stream, 64000);
				}
			}
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x00041310 File Offset: 0x00040310
		public byte[] DataToByte()
		{
			bool flag = this.Body == null;
			if (flag)
			{
				throw new InvalidOperationException("Mime entity body has been not set yet.");
			}
			bool flag2 = !(this.Body is MIME_b_SinglepartBase);
			if (flag2)
			{
				throw new InvalidOperationException("This method is available only for single part entities, not for multipart.");
			}
			MemoryStream memoryStream = new MemoryStream();
			MIME_b_SinglepartBase mime_b_SinglepartBase = (MIME_b_SinglepartBase)this.Body;
			using (Stream dataStream = mime_b_SinglepartBase.GetDataStream())
			{
				Net_Utils.StreamCopy(dataStream, memoryStream, 64000);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000413B0 File Offset: 0x000403B0
		protected internal void Parse(SmartStream stream, Encoding headerEncoding, MIME_h_ContentType defaultContentType)
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
			bool flag3 = defaultContentType == null;
			if (flag3)
			{
				throw new ArgumentNullException("defaultContentType");
			}
			this.m_pHeader.Parse(stream, headerEncoding);
			this.m_pBody = this.m_pBodyProvider.Parse(this, stream, defaultContentType);
			this.m_pBody.SetParent(this, false);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0004142A File Offset: 0x0004042A
		internal void SetParent(MIME_Entity parent)
		{
			this.m_pParent = parent;
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00041434 File Offset: 0x00040434
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x0004144C File Offset: 0x0004044C
		public bool IsModified
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pHeader.IsModified || this.m_pBody.IsModified;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06000AC2 RID: 2754 RVA: 0x00041498 File Offset: 0x00040498
		public MIME_Entity Parent
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pParent;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06000AC3 RID: 2755 RVA: 0x000414CC File Offset: 0x000404CC
		public MIME_h_Collection Header
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pHeader;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06000AC4 RID: 2756 RVA: 0x00041500 File Offset: 0x00040500
		// (set) Token: 0x06000AC5 RID: 2757 RVA: 0x00041558 File Offset: 0x00040558
		public string MimeVersion
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("MIME-Version");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("MIME-Version");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("MIME-Version");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("MIME-Version", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x000415E8 File Offset: 0x000405E8
		// (set) Token: 0x06000AC7 RID: 2759 RVA: 0x00041640 File Offset: 0x00040640
		public string ContentID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-ID");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-ID");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-ID");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-ID", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x000416D0 File Offset: 0x000406D0
		// (set) Token: 0x06000AC9 RID: 2761 RVA: 0x00041728 File Offset: 0x00040728
		public string ContentDescription
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Description");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Description");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Description");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Description", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06000ACA RID: 2762 RVA: 0x000417B8 File Offset: 0x000407B8
		// (set) Token: 0x06000ACB RID: 2763 RVA: 0x00041818 File Offset: 0x00040818
		public string ContentTransferEncoding
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Transfer-Encoding");
				bool flag = first != null;
				string result;
				if (flag)
				{
					result = ((MIME_h_Unstructured)first).Value.Trim();
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Transfer-Encoding");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Transfer-Encoding");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Transfer-Encoding", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06000ACC RID: 2764 RVA: 0x000418A8 File Offset: 0x000408A8
		// (set) Token: 0x06000ACD RID: 2765 RVA: 0x0004191C File Offset: 0x0004091C
		public MIME_h_ContentType ContentType
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Type");
				bool flag = first != null;
				MIME_h_ContentType result;
				if (flag)
				{
					bool flag2 = !(first is MIME_h_ContentType);
					if (flag2)
					{
						throw new ParseException("Header field 'ContentType' parsing failed.");
					}
					result = (MIME_h_ContentType)first;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Type");
				}
				else
				{
					MIME_h first = this.m_pHeader.GetFirst("Content-Type");
					bool flag2 = first == null;
					if (flag2)
					{
						this.m_pHeader.Add(value);
					}
					else
					{
						this.m_pHeader.ReplaceFirst(value);
					}
				}
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06000ACE RID: 2766 RVA: 0x000419A0 File Offset: 0x000409A0
		// (set) Token: 0x06000ACF RID: 2767 RVA: 0x000419F8 File Offset: 0x000409F8
		public string ContentBase
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Base");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Base");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Base");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Base", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x00041A88 File Offset: 0x00040A88
		// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x00041AE0 File Offset: 0x00040AE0
		public string ContentLocation
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Location");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Location");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Location");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Location", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06000AD2 RID: 2770 RVA: 0x00041B70 File Offset: 0x00040B70
		// (set) Token: 0x06000AD3 RID: 2771 RVA: 0x00041BC8 File Offset: 0x00040BC8
		public string Contentfeatures
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-features");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-features");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-features");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-features", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x00041C58 File Offset: 0x00040C58
		// (set) Token: 0x06000AD5 RID: 2773 RVA: 0x00041CCC File Offset: 0x00040CCC
		public MIME_h_ContentDisposition ContentDisposition
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Disposition");
				bool flag = first != null;
				MIME_h_ContentDisposition result;
				if (flag)
				{
					bool flag2 = !(first is MIME_h_ContentDisposition);
					if (flag2)
					{
						throw new ParseException("Header field 'ContentDisposition' parsing failed.");
					}
					result = (MIME_h_ContentDisposition)first;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Disposition");
				}
				else
				{
					MIME_h first = this.m_pHeader.GetFirst("Content-Disposition");
					bool flag2 = first == null;
					if (flag2)
					{
						this.m_pHeader.Add(value);
					}
					else
					{
						this.m_pHeader.ReplaceFirst(value);
					}
				}
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x00041D50 File Offset: 0x00040D50
		// (set) Token: 0x06000AD7 RID: 2775 RVA: 0x00041DA8 File Offset: 0x00040DA8
		public string ContentLanguage
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Language");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Language");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Language");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Language", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000AD8 RID: 2776 RVA: 0x00041E38 File Offset: 0x00040E38
		// (set) Token: 0x06000AD9 RID: 2777 RVA: 0x00041E90 File Offset: 0x00040E90
		public string ContentAlternative
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Alternative");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Alternative");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Alternative");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Alternative", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x00041F20 File Offset: 0x00040F20
		// (set) Token: 0x06000ADB RID: 2779 RVA: 0x00041F78 File Offset: 0x00040F78
		public string ContentMD5
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-MD5");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-MD5");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-MD5");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-MD5", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000ADC RID: 2780 RVA: 0x00042008 File Offset: 0x00041008
		// (set) Token: 0x06000ADD RID: 2781 RVA: 0x00042060 File Offset: 0x00041060
		public string ContentDuration
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				MIME_h first = this.m_pHeader.GetFirst("Content-Duration");
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
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.RemoveAll("Content-Duration");
				}
				else
				{
					MIME_h mime_h = this.m_pHeader.GetFirst("Content-Duration");
					bool flag2 = mime_h == null;
					if (flag2)
					{
						mime_h = new MIME_h_Unstructured("Content-Duration", value);
						this.m_pHeader.Add(mime_h);
					}
					else
					{
						((MIME_h_Unstructured)mime_h).Value = value;
					}
				}
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000ADE RID: 2782 RVA: 0x000420F0 File Offset: 0x000410F0
		// (set) Token: 0x06000ADF RID: 2783 RVA: 0x00042108 File Offset: 0x00041108
		public MIME_b Body
		{
			get
			{
				return this.m_pBody;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Body");
				}
				this.m_pBody = value;
				this.m_pBody.SetParent(this, true);
			}
		}

		// Token: 0x04000480 RID: 1152
		private bool m_IsDisposed = false;

		// Token: 0x04000481 RID: 1153
		private MIME_Entity m_pParent = null;

		// Token: 0x04000482 RID: 1154
		private MIME_h_Collection m_pHeader = null;

		// Token: 0x04000483 RID: 1155
		private MIME_b m_pBody = null;

		// Token: 0x04000484 RID: 1156
		private MIME_b_Provider m_pBodyProvider = null;
	}
}
