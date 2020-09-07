using System;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.MIME
{
	// Token: 0x020000F9 RID: 249
	public class MIME_b_Multipart : MIME_b
	{
		// Token: 0x060009EA RID: 2538 RVA: 0x0003C850 File Offset: 0x0003B850
		public MIME_b_Multipart(MIME_h_ContentType contentType) : base(contentType)
		{
			bool flag = contentType == null;
			if (flag)
			{
				throw new ArgumentNullException("contentType");
			}
			bool flag2 = string.IsNullOrEmpty(contentType.Param_Boundary);
			if (flag2)
			{
				throw new ArgumentException("Argument 'contentType' doesn't contain required boundary parameter.");
			}
			this.m_pBodyParts = new MIME_EntityCollection();
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0003C8BD File Offset: 0x0003B8BD
		internal MIME_b_Multipart()
		{
			this.m_pBodyParts = new MIME_EntityCollection();
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0003C8F0 File Offset: 0x0003B8F0
		protected new static MIME_b Parse(MIME_Entity owner, MIME_h_ContentType defaultContentType, SmartStream stream)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = defaultContentType == null;
			if (flag2)
			{
				throw new ArgumentNullException("defaultContentType");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag4 = owner.ContentType == null || owner.ContentType.Param_Boundary == null;
			if (flag4)
			{
				throw new ParseException("Multipart entity has not required 'boundary' paramter.");
			}
			MIME_b_Multipart mime_b_Multipart = new MIME_b_Multipart(owner.ContentType);
			MIME_b_Multipart.ParseInternal(owner, owner.ContentType.TypeWithSubtype, stream, mime_b_Multipart);
			return mime_b_Multipart;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0003C98C File Offset: 0x0003B98C
		protected static void ParseInternal(MIME_Entity owner, string mediaType, SmartStream stream, MIME_b_Multipart body)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = mediaType == null;
			if (flag2)
			{
				throw new ArgumentNullException("mediaType");
			}
			bool flag3 = stream == null;
			if (flag3)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag4 = owner.ContentType == null || owner.ContentType.Param_Boundary == null;
			if (flag4)
			{
				throw new ParseException("Multipart entity has not required 'boundary' parameter.");
			}
			bool flag5 = body == null;
			if (flag5)
			{
				throw new ArgumentNullException("body");
			}
			MIME_b_Multipart._MultipartReader multipartReader = new MIME_b_Multipart._MultipartReader(stream, owner.ContentType.Param_Boundary);
			while (multipartReader.Next())
			{
				MIME_Entity mime_Entity = new MIME_Entity();
				mime_Entity.Parse(new SmartStream(multipartReader, false), Encoding.UTF8, body.DefaultBodyPartContentType);
				body.m_pBodyParts.Add(mime_Entity);
				mime_Entity.SetParent(owner);
			}
			body.m_TextPreamble = multipartReader.TextPreamble;
			body.m_TextEpilogue = multipartReader.TextEpilogue;
			body.BodyParts.SetModified(false);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0003CA98 File Offset: 0x0003BA98
		internal override void SetParent(MIME_Entity entity, bool setContentType)
		{
			base.SetParent(entity, setContentType);
			bool flag = setContentType && (base.Entity.ContentType == null || !string.Equals(base.Entity.ContentType.TypeWithSubtype, base.MediaType, StringComparison.InvariantCultureIgnoreCase));
			if (flag)
			{
				base.Entity.ContentType = base.ContentType;
			}
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0003CAFC File Offset: 0x0003BAFC
		protected internal override void ToStream(Stream stream, MIME_Encoding_EncodedWord headerWordEncoder, Encoding headerParmetersCharset, bool headerReencode)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = !string.IsNullOrEmpty(this.m_TextPreamble);
			if (flag2)
			{
				bool flag3 = this.m_TextPreamble.EndsWith("\r\n");
				if (flag3)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(this.m_TextPreamble);
					stream.Write(bytes, 0, bytes.Length);
				}
				else
				{
					byte[] bytes2 = Encoding.UTF8.GetBytes(this.m_TextPreamble + "\r\n");
					stream.Write(bytes2, 0, bytes2.Length);
				}
			}
			for (int i = 0; i < this.m_pBodyParts.Count; i++)
			{
				MIME_Entity mime_Entity = this.m_pBodyParts[i];
				bool flag4 = i == 0;
				if (flag4)
				{
					byte[] bytes3 = Encoding.UTF8.GetBytes("--" + base.Entity.ContentType.Param_Boundary + "\r\n");
					stream.Write(bytes3, 0, bytes3.Length);
				}
				else
				{
					byte[] bytes4 = Encoding.UTF8.GetBytes("\r\n--" + base.Entity.ContentType.Param_Boundary + "\r\n");
					stream.Write(bytes4, 0, bytes4.Length);
				}
				mime_Entity.ToStream(stream, headerWordEncoder, headerParmetersCharset, headerReencode);
				bool flag5 = i == this.m_pBodyParts.Count - 1;
				if (flag5)
				{
					byte[] bytes5 = Encoding.UTF8.GetBytes("\r\n--" + base.Entity.ContentType.Param_Boundary + "--");
					stream.Write(bytes5, 0, bytes5.Length);
				}
			}
			bool flag6 = !string.IsNullOrEmpty(this.m_TextEpilogue);
			if (flag6)
			{
				bool flag7 = this.m_TextEpilogue.StartsWith("\r\n");
				if (flag7)
				{
					byte[] bytes6 = Encoding.UTF8.GetBytes(this.m_TextEpilogue);
					stream.Write(bytes6, 0, bytes6.Length);
				}
				else
				{
					byte[] bytes7 = Encoding.UTF8.GetBytes("\r\n" + this.m_TextEpilogue);
					stream.Write(bytes7, 0, bytes7.Length);
				}
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060009F0 RID: 2544 RVA: 0x0003CD28 File Offset: 0x0003BD28
		public override bool IsModified
		{
			get
			{
				return this.m_pBodyParts.IsModified;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060009F1 RID: 2545 RVA: 0x0003CD48 File Offset: 0x0003BD48
		public virtual MIME_h_ContentType DefaultBodyPartContentType
		{
			get
			{
				return new MIME_h_ContentType("text/plain")
				{
					Param_Charset = "US-ASCII"
				};
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060009F2 RID: 2546 RVA: 0x0003CD74 File Offset: 0x0003BD74
		public MIME_EntityCollection BodyParts
		{
			get
			{
				return this.m_pBodyParts;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060009F3 RID: 2547 RVA: 0x0003CD8C File Offset: 0x0003BD8C
		// (set) Token: 0x060009F4 RID: 2548 RVA: 0x0003CDA4 File Offset: 0x0003BDA4
		public string TextPreamble
		{
			get
			{
				return this.m_TextPreamble;
			}
			set
			{
				this.m_TextPreamble = value;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060009F5 RID: 2549 RVA: 0x0003CDB0 File Offset: 0x0003BDB0
		// (set) Token: 0x060009F6 RID: 2550 RVA: 0x0003CDC8 File Offset: 0x0003BDC8
		public string TextEpilogue
		{
			get
			{
				return this.m_TextEpilogue;
			}
			set
			{
				this.m_TextEpilogue = value;
			}
		}

		// Token: 0x04000454 RID: 1108
		private MIME_EntityCollection m_pBodyParts = null;

		// Token: 0x04000455 RID: 1109
		private string m_TextPreamble = "";

		// Token: 0x04000456 RID: 1110
		private string m_TextEpilogue = "";

		// Token: 0x020002BF RID: 703
		public class _MultipartReader : Stream
		{
			// Token: 0x0600183C RID: 6204 RVA: 0x00095D74 File Offset: 0x00094D74
			public _MultipartReader(SmartStream stream, string boundary)
			{
				bool flag = stream == null;
				if (flag)
				{
					throw new ArgumentNullException("stream");
				}
				bool flag2 = boundary == null;
				if (flag2)
				{
					throw new ArgumentNullException("boundary");
				}
				this.m_pStream = stream;
				this.m_Boundary = boundary;
				this.m_pReadLineOP = new SmartStream.ReadLineAsyncOP(new byte[stream.LineBufferSize], SizeExceededAction.ThrowException);
				this.m_pTextPreamble = new StringBuilder();
				this.m_pTextEpilogue = new StringBuilder();
			}

			// Token: 0x0600183D RID: 6205 RVA: 0x00095E24 File Offset: 0x00094E24
			public bool Next()
			{
				bool flag = this.m_State == MIME_b_Multipart._MultipartReader.State.InBoundary;
				if (flag)
				{
					throw new InvalidOperationException("You must read all boundary data, before calling this method.");
				}
				bool flag2 = this.m_State == MIME_b_Multipart._MultipartReader.State.Done;
				bool result;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.m_State == MIME_b_Multipart._MultipartReader.State.SeekFirst;
					if (flag3)
					{
						this.m_pPreviousLine = null;
						for (;;)
						{
							this.m_pStream.ReadLine(this.m_pReadLineOP, false);
							bool flag4 = this.m_pReadLineOP.Error != null;
							if (flag4)
							{
								break;
							}
							bool flag5 = this.m_pReadLineOP.BytesInBuffer == 0;
							if (flag5)
							{
								goto Block_5;
							}
							bool flag6 = this.m_pReadLineOP.LineUtf8.Trim() == "--" + this.m_Boundary;
							if (flag6)
							{
								goto Block_6;
							}
							this.m_pTextPreamble.Append(this.m_pReadLineOP.LineUtf8 + "\r\n");
						}
						throw this.m_pReadLineOP.Error;
						Block_5:
						this.m_State = MIME_b_Multipart._MultipartReader.State.Done;
						return false;
						Block_6:
						this.m_State = MIME_b_Multipart._MultipartReader.State.InBoundary;
						result = true;
					}
					else
					{
						bool flag7 = this.m_State == MIME_b_Multipart._MultipartReader.State.ReadNext;
						if (flag7)
						{
							this.m_pPreviousLine = null;
							this.m_State = MIME_b_Multipart._MultipartReader.State.InBoundary;
							result = true;
						}
						else
						{
							result = false;
						}
					}
				}
				return result;
			}

			// Token: 0x0600183E RID: 6206 RVA: 0x000091B8 File Offset: 0x000081B8
			public override void Flush()
			{
			}

			// Token: 0x0600183F RID: 6207 RVA: 0x0004540D File Offset: 0x0004440D
			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06001840 RID: 6208 RVA: 0x0004540D File Offset: 0x0004440D
			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06001841 RID: 6209 RVA: 0x00095F60 File Offset: 0x00094F60
			public override int Read(byte[] buffer, int offset, int count)
			{
				bool flag = buffer == null;
				if (flag)
				{
					throw new ArgumentNullException("buffer");
				}
				bool flag2 = this.m_State == MIME_b_Multipart._MultipartReader.State.SeekFirst;
				if (flag2)
				{
					throw new InvalidOperationException("Read method is not valid in '" + this.m_State + "' state.");
				}
				bool flag3 = this.m_State == MIME_b_Multipart._MultipartReader.State.ReadNext || this.m_State == MIME_b_Multipart._MultipartReader.State.Done;
				int result;
				if (flag3)
				{
					result = 0;
				}
				else
				{
					bool flag4 = this.m_pPreviousLine == null;
					if (flag4)
					{
						this.m_pPreviousLine = new MIME_b_Multipart._MultipartReader._DataLine(this.m_pStream.LineBufferSize);
						this.m_pStream.ReadLine(this.m_pReadLineOP, false);
						bool flag5 = this.m_pReadLineOP.Error != null;
						if (flag5)
						{
							throw this.m_pReadLineOP.Error;
						}
						bool flag6 = this.m_pReadLineOP.BytesInBuffer == 0;
						if (flag6)
						{
							this.m_State = MIME_b_Multipart._MultipartReader.State.Done;
							return 0;
						}
						bool flag7 = this.m_pReadLineOP.Buffer[0] == 45 && string.Equals("--" + this.m_Boundary + "--", this.m_pReadLineOP.LineUtf8);
						if (flag7)
						{
							this.m_State = MIME_b_Multipart._MultipartReader.State.Done;
							for (;;)
							{
								this.m_pStream.ReadLine(this.m_pReadLineOP, false);
								bool flag8 = this.m_pReadLineOP.Error != null;
								if (flag8)
								{
									break;
								}
								bool flag9 = this.m_pReadLineOP.BytesInBuffer == 0;
								if (flag9)
								{
									goto Block_11;
								}
								this.m_pTextEpilogue.Append(this.m_pReadLineOP.LineUtf8 + "\r\n");
							}
							throw this.m_pReadLineOP.Error;
							Block_11:
							return 0;
						}
						bool flag10 = this.m_pReadLineOP.Buffer[0] == 45 && string.Equals("--" + this.m_Boundary, this.m_pReadLineOP.LineUtf8);
						if (flag10)
						{
							this.m_State = MIME_b_Multipart._MultipartReader.State.ReadNext;
							return 0;
						}
						this.m_pPreviousLine.AssignFrom(this.m_pReadLineOP);
					}
					this.m_pStream.ReadLine(this.m_pReadLineOP, false);
					bool flag11 = this.m_pReadLineOP.Error != null;
					if (flag11)
					{
						throw this.m_pReadLineOP.Error;
					}
					bool flag12 = this.m_pReadLineOP.BytesInBuffer == 0;
					if (flag12)
					{
						this.m_State = MIME_b_Multipart._MultipartReader.State.Done;
						bool flag13 = count < this.m_pPreviousLine.BytesInBuffer;
						if (flag13)
						{
							throw new ArgumentException("Argument 'buffer' is to small. This should never happen.");
						}
						bool flag14 = this.m_pPreviousLine.BytesInBuffer > 0;
						if (flag14)
						{
							Array.Copy(this.m_pPreviousLine.LineBuffer, 0, buffer, offset, this.m_pPreviousLine.BytesInBuffer);
						}
						result = this.m_pPreviousLine.BytesInBuffer;
					}
					else
					{
						bool flag15 = this.m_pReadLineOP.Buffer[0] == 45 && string.Equals("--" + this.m_Boundary + "--", this.m_pReadLineOP.LineUtf8);
						if (flag15)
						{
							this.m_State = MIME_b_Multipart._MultipartReader.State.Done;
							bool flag16 = this.m_pReadLineOP.Buffer[this.m_pReadLineOP.BytesInBuffer - 1] == 10;
							if (flag16)
							{
								this.m_pTextEpilogue.Append("\r\n");
							}
							for (;;)
							{
								this.m_pStream.ReadLine(this.m_pReadLineOP, false);
								bool flag17 = this.m_pReadLineOP.Error != null;
								if (flag17)
								{
									break;
								}
								bool flag18 = this.m_pReadLineOP.BytesInBuffer == 0;
								if (flag18)
								{
									goto Block_22;
								}
								this.m_pTextEpilogue.Append(this.m_pReadLineOP.LineUtf8 + "\r\n");
							}
							throw this.m_pReadLineOP.Error;
							Block_22:
							bool flag19 = count < this.m_pPreviousLine.BytesInBuffer;
							if (flag19)
							{
								throw new ArgumentException("Argument 'buffer' is to small. This should never happen.");
							}
							bool flag20 = this.m_pPreviousLine.BytesInBuffer > 2;
							if (flag20)
							{
								Array.Copy(this.m_pPreviousLine.LineBuffer, 0, buffer, offset, this.m_pPreviousLine.BytesInBuffer - 2);
								result = this.m_pPreviousLine.BytesInBuffer - 2;
							}
							else
							{
								result = 0;
							}
						}
						else
						{
							bool flag21 = this.m_pReadLineOP.Buffer[0] == 45 && string.Equals("--" + this.m_Boundary, this.m_pReadLineOP.LineUtf8);
							if (flag21)
							{
								this.m_State = MIME_b_Multipart._MultipartReader.State.ReadNext;
								bool flag22 = count < this.m_pPreviousLine.BytesInBuffer;
								if (flag22)
								{
									throw new ArgumentException("Argument 'buffer' is to small. This should never happen.");
								}
								bool flag23 = this.m_pPreviousLine.BytesInBuffer > 2;
								if (flag23)
								{
									Array.Copy(this.m_pPreviousLine.LineBuffer, 0, buffer, offset, this.m_pPreviousLine.BytesInBuffer - 2);
									result = this.m_pPreviousLine.BytesInBuffer - 2;
								}
								else
								{
									result = 0;
								}
							}
							else
							{
								bool flag24 = count < this.m_pPreviousLine.BytesInBuffer;
								if (flag24)
								{
									throw new ArgumentException("Argument 'buffer' is to small. This should never happen.");
								}
								Array.Copy(this.m_pPreviousLine.LineBuffer, 0, buffer, offset, this.m_pPreviousLine.BytesInBuffer);
								int bytesInBuffer = this.m_pPreviousLine.BytesInBuffer;
								this.m_pPreviousLine.AssignFrom(this.m_pReadLineOP);
								result = bytesInBuffer;
							}
						}
					}
				}
				return result;
			}

			// Token: 0x06001842 RID: 6210 RVA: 0x0004540D File Offset: 0x0004440D
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			// Token: 0x170007E1 RID: 2017
			// (get) Token: 0x06001843 RID: 6211 RVA: 0x000964A8 File Offset: 0x000954A8
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170007E2 RID: 2018
			// (get) Token: 0x06001844 RID: 6212 RVA: 0x000964BC File Offset: 0x000954BC
			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170007E3 RID: 2019
			// (get) Token: 0x06001845 RID: 6213 RVA: 0x000964D0 File Offset: 0x000954D0
			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170007E4 RID: 2020
			// (get) Token: 0x06001846 RID: 6214 RVA: 0x0004540D File Offset: 0x0004440D
			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			// Token: 0x170007E5 RID: 2021
			// (get) Token: 0x06001847 RID: 6215 RVA: 0x0004540D File Offset: 0x0004440D
			// (set) Token: 0x06001848 RID: 6216 RVA: 0x0004540D File Offset: 0x0004440D
			public override long Position
			{
				get
				{
					throw new NotSupportedException();
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			// Token: 0x170007E6 RID: 2022
			// (get) Token: 0x06001849 RID: 6217 RVA: 0x000964E4 File Offset: 0x000954E4
			public string TextPreamble
			{
				get
				{
					return this.m_pTextPreamble.ToString();
				}
			}

			// Token: 0x170007E7 RID: 2023
			// (get) Token: 0x0600184A RID: 6218 RVA: 0x00096504 File Offset: 0x00095504
			public string TextEpilogue
			{
				get
				{
					return this.m_pTextEpilogue.ToString();
				}
			}

			// Token: 0x170007E8 RID: 2024
			// (get) Token: 0x0600184B RID: 6219 RVA: 0x00096524 File Offset: 0x00095524
			internal MIME_b_Multipart._MultipartReader.State ReaderState
			{
				get
				{
					return this.m_State;
				}
			}

			// Token: 0x04000A34 RID: 2612
			private MIME_b_Multipart._MultipartReader.State m_State = MIME_b_Multipart._MultipartReader.State.SeekFirst;

			// Token: 0x04000A35 RID: 2613
			private SmartStream m_pStream = null;

			// Token: 0x04000A36 RID: 2614
			private string m_Boundary = "";

			// Token: 0x04000A37 RID: 2615
			private MIME_b_Multipart._MultipartReader._DataLine m_pPreviousLine = null;

			// Token: 0x04000A38 RID: 2616
			private SmartStream.ReadLineAsyncOP m_pReadLineOP = null;

			// Token: 0x04000A39 RID: 2617
			private StringBuilder m_pTextPreamble = null;

			// Token: 0x04000A3A RID: 2618
			private StringBuilder m_pTextEpilogue = null;

			// Token: 0x020003B3 RID: 947
			internal enum State
			{
				// Token: 0x04000D37 RID: 3383
				SeekFirst,
				// Token: 0x04000D38 RID: 3384
				ReadNext,
				// Token: 0x04000D39 RID: 3385
				InBoundary,
				// Token: 0x04000D3A RID: 3386
				Done
			}

			// Token: 0x020003B4 RID: 948
			private class _DataLine
			{
				// Token: 0x06001C1C RID: 7196 RVA: 0x000ACFC0 File Offset: 0x000ABFC0
				public _DataLine(int lineBufferSize)
				{
					this.m_pLineBuffer = new byte[lineBufferSize];
				}

				// Token: 0x06001C1D RID: 7197 RVA: 0x000ACFE4 File Offset: 0x000ABFE4
				public void AssignFrom(SmartStream.ReadLineAsyncOP op)
				{
					bool flag = op == null;
					if (flag)
					{
						throw new ArgumentNullException();
					}
					this.m_BytesInBuffer = op.BytesInBuffer;
					Array.Copy(op.Buffer, this.m_pLineBuffer, op.BytesInBuffer);
				}

				// Token: 0x170008A3 RID: 2211
				// (get) Token: 0x06001C1E RID: 7198 RVA: 0x000AD028 File Offset: 0x000AC028
				public byte[] LineBuffer
				{
					get
					{
						return this.m_pLineBuffer;
					}
				}

				// Token: 0x170008A4 RID: 2212
				// (get) Token: 0x06001C1F RID: 7199 RVA: 0x000AD040 File Offset: 0x000AC040
				public int BytesInBuffer
				{
					get
					{
						return this.m_BytesInBuffer;
					}
				}

				// Token: 0x04000D3B RID: 3387
				private byte[] m_pLineBuffer = null;

				// Token: 0x04000D3C RID: 3388
				private int m_BytesInBuffer = 0;
			}
		}
	}
}
