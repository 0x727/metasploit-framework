using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000104 RID: 260
	public abstract class MIME_b_SinglepartBase : MIME_b
	{
		// Token: 0x06000A13 RID: 2579 RVA: 0x0003DC78 File Offset: 0x0003CC78
		public MIME_b_SinglepartBase(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = contentType == null;
			if (flag)
			{
				throw new ArgumentNullException("contentType");
			}
			this.m_pEncodedDataStream = new MemoryStreamEx();
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x0003DCBC File Offset: 0x0003CCBC
		~MIME_b_SinglepartBase()
		{
			try
			{
				bool flag = this.m_pEncodedDataStream != null;
				if (flag)
				{
					this.m_pEncodedDataStream.Close();
				}
			}
			finally
			{
				
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0003DD00 File Offset: 0x0003CD00
		internal override void SetParent(MIME_Entity entity, bool setContentType)
		{
			base.SetParent(entity, setContentType);
			bool flag = setContentType && (base.Entity.ContentType == null || !string.Equals(base.Entity.ContentType.TypeWithSubtype, base.MediaType, StringComparison.InvariantCultureIgnoreCase));
			if (flag)
			{
				base.Entity.ContentType = new MIME_h_ContentType(base.MediaType);
			}
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x0003DD6C File Offset: 0x0003CD6C
		protected internal override void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			Net_Utils.StreamCopy(this.GetEncodedDataStream(), stream, 84000);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x0003DDA0 File Offset: 0x0003CDA0
		protected void SetModified(bool isModified)
		{
			this.m_IsModified = isModified;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0003DDAC File Offset: 0x0003CDAC
		public Stream GetEncodedDataStream()
		{
			bool flag = base.Entity == null;
			if (flag)
			{
				throw new InvalidOperationException("Body must be bounded to some entity first.");
			}
			this.m_pEncodedDataStream.Position = 0L;
			return this.m_pEncodedDataStream;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x0003DDEC File Offset: 0x0003CDEC
		public void SetEncodedData(string contentTransferEncoding, Stream stream)
		{
			bool flag = contentTransferEncoding == null;
			if (flag)
			{
				throw new ArgumentNullException("contentTransferEncoding");
			}
			bool flag2 = contentTransferEncoding == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'contentTransferEncoding' value must be specified.");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag4 = base.Entity == null;
			if (flag4)
			{
				throw new InvalidOperationException("Body must be bounded to some entity first.");
			}
			bool flag5 = base.Entity.ContentType == null || !string.Equals(base.Entity.ContentType.TypeWithSubtype, base.MediaType, StringComparison.InvariantCultureIgnoreCase);
			if (flag5)
			{
				base.Entity.ContentType = new MIME_h_ContentType(base.MediaType);
			}
			base.Entity.ContentTransferEncoding = contentTransferEncoding;
			this.m_pEncodedDataStream.SetLength(0L);
			Net_Utils.StreamCopy(stream, this.m_pEncodedDataStream, 84000);
			this.m_IsModified = true;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0003DED8 File Offset: 0x0003CED8
		public Stream GetDataStream()
		{
			bool flag = base.Entity == null;
			if (flag)
			{
				throw new InvalidOperationException("Body must be bounded to some entity first.");
			}
			string a = MIME_TransferEncodings.SevenBit;
			bool flag2 = base.Entity.ContentTransferEncoding != null;
			if (flag2)
			{
				a = base.Entity.ContentTransferEncoding.ToLowerInvariant();
			}
			this.m_pEncodedDataStream.Position = 0L;
			bool flag3 = a == MIME_TransferEncodings.QuotedPrintable;
			Stream result;
			if (flag3)
			{
				result = new QuotedPrintableStream(new SmartStream(this.m_pEncodedDataStream, false), FileAccess.Read);
			}
			else
			{
				bool flag4 = a == MIME_TransferEncodings.Base64;
				if (flag4)
				{
					result = new Base64Stream(this.m_pEncodedDataStream, false, true, FileAccess.Read)
					{
						IgnoreInvalidPadding = true
					};
				}
				else
				{
					bool flag5 = a == MIME_TransferEncodings.Binary;
					if (flag5)
					{
						result = new ReadWriteControlledStream(this.m_pEncodedDataStream, FileAccess.Read);
					}
					else
					{
						bool flag6 = a == MIME_TransferEncodings.EightBit;
						if (flag6)
						{
							result = new ReadWriteControlledStream(this.m_pEncodedDataStream, FileAccess.Read);
						}
						else
						{
							bool flag7 = a == MIME_TransferEncodings.SevenBit;
							if (!flag7)
							{
								throw new NotSupportedException("Not supported Content-Transfer-Encoding '" + base.Entity.ContentTransferEncoding + "'.");
							}
							result = new ReadWriteControlledStream(this.m_pEncodedDataStream, FileAccess.Read);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x0003E020 File Offset: 0x0003D020
		public void SetData(Stream stream, string transferEncoding)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = transferEncoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("transferEncoding");
			}
			bool flag3 = string.Equals(transferEncoding, MIME_TransferEncodings.QuotedPrintable, StringComparison.InvariantCultureIgnoreCase);
			if (flag3)
			{
				using (MemoryStreamEx memoryStreamEx = new MemoryStreamEx())
				{
					QuotedPrintableStream quotedPrintableStream = new QuotedPrintableStream(new SmartStream(memoryStreamEx, false), FileAccess.ReadWrite);
					Net_Utils.StreamCopy(stream, quotedPrintableStream, 84000);
					quotedPrintableStream.Flush();
					memoryStreamEx.Position = 0L;
					this.SetEncodedData(transferEncoding, memoryStreamEx);
				}
			}
			else
			{
				bool flag4 = string.Equals(transferEncoding, MIME_TransferEncodings.Base64, StringComparison.InvariantCultureIgnoreCase);
				if (flag4)
				{
					using (MemoryStreamEx memoryStreamEx2 = new MemoryStreamEx())
					{
						Base64Stream base64Stream = new Base64Stream(memoryStreamEx2, false, true, FileAccess.ReadWrite);
						Net_Utils.StreamCopy(stream, base64Stream, 84000);
						base64Stream.Finish();
						memoryStreamEx2.Position = 0L;
						this.SetEncodedData(transferEncoding, memoryStreamEx2);
					}
				}
				else
				{
					bool flag5 = string.Equals(transferEncoding, MIME_TransferEncodings.Binary, StringComparison.InvariantCultureIgnoreCase);
					if (flag5)
					{
						this.SetEncodedData(transferEncoding, stream);
					}
					else
					{
						bool flag6 = string.Equals(transferEncoding, MIME_TransferEncodings.EightBit, StringComparison.InvariantCultureIgnoreCase);
						if (flag6)
						{
							this.SetEncodedData(transferEncoding, stream);
						}
						else
						{
							bool flag7 = string.Equals(transferEncoding, MIME_TransferEncodings.SevenBit, StringComparison.InvariantCultureIgnoreCase);
							if (!flag7)
							{
								throw new NotSupportedException("Not supported Content-Transfer-Encoding '" + transferEncoding + "'.");
							}
							this.SetEncodedData(transferEncoding, stream);
						}
					}
				}
			}
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x0003E1B0 File Offset: 0x0003D1B0
		public void SetDataFromFile(string file, string transferEncoding)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			using (FileStream fileStream = File.OpenRead(file))
			{
				this.SetData(fileStream, transferEncoding);
			}
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0003E204 File Offset: 0x0003D204
		public void DataToStream(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			using (Stream dataStream = this.GetDataStream())
			{
				Net_Utils.StreamCopy(dataStream, stream, 64000);
			}
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x0003E25C File Offset: 0x0003D25C
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
			using (Stream stream = File.Create(fileName))
			{
				using (Stream dataStream = this.GetDataStream())
				{
					Net_Utils.StreamCopy(dataStream, stream, 64000);
				}
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06000A1F RID: 2591 RVA: 0x0003E2F0 File Offset: 0x0003D2F0
		public override bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06000A20 RID: 2592 RVA: 0x0003E308 File Offset: 0x0003D308
		public int EncodedDataSize
		{
			get
			{
				return (int)this.m_pEncodedDataStream.Length;
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x0003E328 File Offset: 0x0003D328
		public byte[] EncodedData
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				Net_Utils.StreamCopy(this.GetEncodedDataStream(), memoryStream, 84000);
				return memoryStream.ToArray();
			}
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000A22 RID: 2594 RVA: 0x0003E358 File Offset: 0x0003D358
		public byte[] Data
		{
			get
			{
				MemoryStream memoryStream = new MemoryStream();
				Net_Utils.StreamCopy(this.GetDataStream(), memoryStream, 84000);
				return memoryStream.ToArray();
			}
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x0003E388 File Offset: 0x0003D388
		protected Stream EncodedStream
		{
			get
			{
				return this.m_pEncodedDataStream;
			}
		}

		// Token: 0x04000459 RID: 1113
		private bool m_IsModified = false;

		// Token: 0x0400045A RID: 1114
		private Stream m_pEncodedDataStream = null;
	}
}
