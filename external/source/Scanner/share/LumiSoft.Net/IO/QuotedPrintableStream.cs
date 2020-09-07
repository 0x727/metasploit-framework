using System;
using System.Globalization;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x0200011E RID: 286
	public class QuotedPrintableStream : Stream
	{
		// Token: 0x06000B43 RID: 2883 RVA: 0x00045348 File Offset: 0x00044348
		public QuotedPrintableStream(SmartStream stream, FileAccess access)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
			this.m_AccessMode = access;
			this.m_pDecodedBuffer = new byte[32000];
			this.m_pEncodedBuffer = new byte[78];
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x000453D0 File Offset: 0x000443D0
		public override void Flush()
		{
			bool flag = this.m_EncodedCount > 0;
			if (flag)
			{
				this.m_pStream.Write(this.m_pEncodedBuffer, 0, this.m_EncodedCount);
				this.m_EncodedCount = 0;
			}
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x0004540D File Offset: 0x0004440D
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x0004540D File Offset: 0x0004440D
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x00045418 File Offset: 0x00044418
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'offset' value.");
			}
			bool flag3 = offset + count > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'count' value.");
			}
			bool flag4 = (this.m_AccessMode & FileAccess.Read) == (FileAccess)0;
			if (flag4)
			{
				throw new NotSupportedException();
			}
			SmartStream.ReadLineAsyncOP readLineAsyncOP;
			for (;;)
			{
				bool flag5 = this.m_DecodedOffset >= this.m_DecodedCount;
				if (flag5)
				{
					this.m_DecodedOffset = 0;
					this.m_DecodedCount = 0;
					readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.ThrowException);
					this.m_pStream.ReadLine(readLineAsyncOP, false);
					bool flag6 = readLineAsyncOP.Error != null;
					if (flag6)
					{
						break;
					}
					bool flag7 = readLineAsyncOP.BytesInBuffer == 0;
					if (flag7)
					{
						goto Block_8;
					}
					bool flag8 = false;
					int lineBytesInBuffer = readLineAsyncOP.LineBytesInBuffer;
					for (int i = 0; i < readLineAsyncOP.LineBytesInBuffer; i++)
					{
						byte b = readLineAsyncOP.Buffer[i];
						bool flag9 = b == 61 && i == lineBytesInBuffer - 1;
						if (flag9)
						{
							flag8 = true;
						}
						else
						{
							bool flag10 = b == 61;
							if (flag10)
							{
								byte b2 = readLineAsyncOP.Buffer[++i];
								byte b3 = readLineAsyncOP.Buffer[++i];
								byte b4 = 0;
								bool flag11 = byte.TryParse(new string(new char[]
								{
									(char)b2,
									(char)b3
								}), NumberStyles.HexNumber, null, out b4);
								if (flag11)
								{
									byte[] pDecodedBuffer = this.m_pDecodedBuffer;
									int decodedCount = this.m_DecodedCount;
									this.m_DecodedCount = decodedCount + 1;
									pDecodedBuffer[decodedCount] = b4;
								}
								else
								{
									byte[] pDecodedBuffer2 = this.m_pDecodedBuffer;
									int decodedCount = this.m_DecodedCount;
									this.m_DecodedCount = decodedCount + 1;
									pDecodedBuffer2[decodedCount] = 61;
									byte[] pDecodedBuffer3 = this.m_pDecodedBuffer;
									decodedCount = this.m_DecodedCount;
									this.m_DecodedCount = decodedCount + 1;
									pDecodedBuffer3[decodedCount] = b2;
									byte[] pDecodedBuffer4 = this.m_pDecodedBuffer;
									decodedCount = this.m_DecodedCount;
									this.m_DecodedCount = decodedCount + 1;
									pDecodedBuffer4[decodedCount] = b3;
								}
							}
							else
							{
								byte[] pDecodedBuffer5 = this.m_pDecodedBuffer;
								int decodedCount = this.m_DecodedCount;
								this.m_DecodedCount = decodedCount + 1;
								pDecodedBuffer5[decodedCount] = b;
							}
						}
					}
					bool flag12 = readLineAsyncOP.LineBytesInBuffer != readLineAsyncOP.BytesInBuffer && !flag8;
					if (flag12)
					{
						byte[] pDecodedBuffer6 = this.m_pDecodedBuffer;
						int decodedCount = this.m_DecodedCount;
						this.m_DecodedCount = decodedCount + 1;
						pDecodedBuffer6[decodedCount] = 13;
						byte[] pDecodedBuffer7 = this.m_pDecodedBuffer;
						decodedCount = this.m_DecodedCount;
						this.m_DecodedCount = decodedCount + 1;
						pDecodedBuffer7[decodedCount] = 10;
					}
				}
				bool flag13 = this.m_DecodedOffset < this.m_DecodedCount;
				if (flag13)
				{
					goto Block_16;
				}
			}
			throw readLineAsyncOP.Error;
			Block_8:
			return 0;
			Block_16:
			int num = Math.Min(count, this.m_DecodedCount - this.m_DecodedOffset);
			Array.Copy(this.m_pDecodedBuffer, this.m_DecodedOffset, buffer, offset, num);
			this.m_DecodedOffset += num;
			return num;
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0004571C File Offset: 0x0004471C
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'offset' value.");
			}
			bool flag3 = offset + count > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'count' value.");
			}
			bool flag4 = (this.m_AccessMode & FileAccess.Write) == (FileAccess)0;
			if (flag4)
			{
				throw new NotSupportedException();
			}
			for (int i = 0; i < count; i++)
			{
				byte b = buffer[offset + i];
				bool flag5 = (b >= 33 && b <= 60) || (b >= 62 && b <= 126);
				if (flag5)
				{
					bool flag6 = this.m_EncodedCount >= 75;
					int encodedCount;
					if (flag6)
					{
						byte[] pEncodedBuffer = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer[encodedCount] = 61;
						byte[] pEncodedBuffer2 = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer2[encodedCount] = 13;
						byte[] pEncodedBuffer3 = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer3[encodedCount] = 10;
						this.Flush();
					}
					byte[] pEncodedBuffer4 = this.m_pEncodedBuffer;
					encodedCount = this.m_EncodedCount;
					this.m_EncodedCount = encodedCount + 1;
					pEncodedBuffer4[encodedCount] = b;
				}
				else
				{
					bool flag7 = this.m_EncodedCount >= 73;
					int encodedCount;
					if (flag7)
					{
						byte[] pEncodedBuffer5 = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer5[encodedCount] = 61;
						byte[] pEncodedBuffer6 = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer6[encodedCount] = 13;
						byte[] pEncodedBuffer7 = this.m_pEncodedBuffer;
						encodedCount = this.m_EncodedCount;
						this.m_EncodedCount = encodedCount + 1;
						pEncodedBuffer7[encodedCount] = 10;
						this.Flush();
					}
					byte[] pEncodedBuffer8 = this.m_pEncodedBuffer;
					encodedCount = this.m_EncodedCount;
					this.m_EncodedCount = encodedCount + 1;
					pEncodedBuffer8[encodedCount] = 61;
					byte[] pEncodedBuffer9 = this.m_pEncodedBuffer;
					encodedCount = this.m_EncodedCount;
					this.m_EncodedCount = encodedCount + 1;
					pEncodedBuffer9[encodedCount] = (byte)(b >> 4).ToString("X")[0];
					byte[] pEncodedBuffer10 = this.m_pEncodedBuffer;
					encodedCount = this.m_EncodedCount;
					this.m_EncodedCount = encodedCount + 1;
					pEncodedBuffer10[encodedCount] = (byte)((int)(b & 15)).ToString("X")[0];
				}
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0004596C File Offset: 0x0004496C
		public override bool CanRead
		{
			get
			{
				return (this.m_AccessMode & FileAccess.Read) > (FileAccess)0;
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x0004598C File Offset: 0x0004498C
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06000B4B RID: 2891 RVA: 0x000459A0 File Offset: 0x000449A0
		public override bool CanWrite
		{
			get
			{
				return (this.m_AccessMode & FileAccess.Write) > (FileAccess)0;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x0004540D File Offset: 0x0004440D
		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x0004540D File Offset: 0x0004440D
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x0004540D File Offset: 0x0004440D
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

		// Token: 0x0400049D RID: 1181
		private SmartStream m_pStream = null;

		// Token: 0x0400049E RID: 1182
		private FileAccess m_AccessMode = FileAccess.ReadWrite;

		// Token: 0x0400049F RID: 1183
		private byte[] m_pDecodedBuffer = null;

		// Token: 0x040004A0 RID: 1184
		private int m_DecodedOffset = 0;

		// Token: 0x040004A1 RID: 1185
		private int m_DecodedCount = 0;

		// Token: 0x040004A2 RID: 1186
		private byte[] m_pEncodedBuffer = null;

		// Token: 0x040004A3 RID: 1187
		private int m_EncodedCount = 0;
	}
}
