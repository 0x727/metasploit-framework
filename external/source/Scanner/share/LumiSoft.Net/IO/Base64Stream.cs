using System;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x02000120 RID: 288
	public class Base64Stream : Stream, IDisposable
	{
		// Token: 0x06000B5B RID: 2907 RVA: 0x00045BF1 File Offset: 0x00044BF1
		public Base64Stream(Stream stream, bool owner, bool addLineBreaks) : this(stream, owner, addLineBreaks, FileAccess.ReadWrite)
		{
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x00045C00 File Offset: 0x00044C00
		public Base64Stream(Stream stream, bool owner, bool addLineBreaks, FileAccess access)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
			this.m_IsOwner = owner;
			this.m_AddLineBreaks = addLineBreaks;
			this.m_AccessMode = access;
			this.m_pDecodedBlock = new byte[8000];
			this.m_pBase64 = new Base64();
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x00045CD8 File Offset: 0x00044CD8
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				try
				{
					this.Finish();
				}
				catch
				{
				}
				this.m_IsDisposed = true;
				bool isOwner = this.m_IsOwner;
				if (isOwner)
				{
					this.m_pStream.Close();
				}
			}
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x00045D34 File Offset: 0x00044D34
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x00045D58 File Offset: 0x00044D58
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x00045D84 File Offset: 0x00044D84
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000B61 RID: 2913 RVA: 0x00045DB0 File Offset: 0x00044DB0
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("Base64Stream");
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Argument 'offset' value must be >= 0.");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' value must be >= 0.");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count", "Argument 'count' is bigger than than argument 'buffer' can store.");
			}
			bool flag5 = (this.m_AccessMode & FileAccess.Read) == (FileAccess)0;
			if (flag5)
			{
				throw new NotSupportedException();
			}
			bool flag6 = this.m_DecodedBlockCount - this.m_DecodedBlockOffset == 0;
			if (flag6)
			{
				byte[] array = new byte[this.m_pDecodedBlock.Length + 3];
				int num = this.m_pStream.Read(array, 0, array.Length - 3);
				bool flag7 = num == 0;
				if (flag7)
				{
					return 0;
				}
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					byte b = array[i];
					bool flag8 = b == 61 || Base64Stream.BASE64_DECODE_TABLE[(int)b] != -1;
					if (flag8)
					{
						num2++;
					}
				}
				while (num2 % 4 != 0)
				{
					int num3 = this.m_pStream.ReadByte();
					bool flag9 = num3 == -1;
					if (flag9)
					{
						bool ignoreInvalidPadding = this.m_IgnoreInvalidPadding;
						if (!ignoreInvalidPadding)
						{
							break;
						}
						bool flag10 = num2 % 4 == 1;
						if (flag10)
						{
							array[num++] = 65;
							num2++;
						}
						else
						{
							array[num++] = 61;
							num2++;
						}
					}
					else
					{
						bool flag11 = num3 == 61 || Base64Stream.BASE64_DECODE_TABLE[num3] != -1;
						if (flag11)
						{
							array[num++] = (byte)num3;
							num2++;
						}
					}
				}
				this.m_DecodedBlockCount = this.m_pBase64.Decode(array, 0, num, this.m_pDecodedBlock, 0, true);
				this.m_DecodedBlockOffset = 0;
			}
			int val = this.m_DecodedBlockCount - this.m_DecodedBlockOffset;
			int num4 = Math.Min(count, val);
			Array.Copy(this.m_pDecodedBlock, this.m_DecodedBlockOffset, buffer, offset, num4);
			this.m_DecodedBlockOffset += num4;
			return num4;
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x00046008 File Offset: 0x00045008
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isFinished = this.m_IsFinished;
			if (isFinished)
			{
				throw new InvalidOperationException("Stream is marked as finished by calling Finish method.");
			}
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
			bool flag3 = count < 0 || count > buffer.Length - offset;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'count' value.");
			}
			bool flag4 = (this.m_AccessMode & FileAccess.Write) == (FileAccess)0;
			if (flag4)
			{
				throw new NotSupportedException();
			}
			int num = this.m_pEncodeBuffer.Length;
			for (int i = 0; i < count; i++)
			{
				byte[] pEncode3x8Block = this.m_pEncode3x8Block;
				int num2 = this.m_OffsetInEncode3x8Block;
				this.m_OffsetInEncode3x8Block = num2 + 1;
				pEncode3x8Block[num2] = buffer[offset + i];
				bool flag5 = this.m_OffsetInEncode3x8Block == 3;
				if (flag5)
				{
					byte[] pEncodeBuffer = this.m_pEncodeBuffer;
					num2 = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = num2 + 1;
					pEncodeBuffer[num2] = Base64Stream.BASE64_ENCODE_TABLE[this.m_pEncode3x8Block[0] >> 2];
					byte[] pEncodeBuffer2 = this.m_pEncodeBuffer;
					num2 = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = num2 + 1;
					pEncodeBuffer2[num2] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[0] & 3) << 4 | this.m_pEncode3x8Block[1] >> 4];
					byte[] pEncodeBuffer3 = this.m_pEncodeBuffer;
					num2 = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = num2 + 1;
					pEncodeBuffer3[num2] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[1] & 15) << 2 | this.m_pEncode3x8Block[2] >> 6];
					byte[] pEncodeBuffer4 = this.m_pEncodeBuffer;
					num2 = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = num2 + 1;
					pEncodeBuffer4[num2] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[2] & 63)];
					bool flag6 = this.m_EncodeBufferOffset >= num - 2;
					if (flag6)
					{
						bool addLineBreaks = this.m_AddLineBreaks;
						if (addLineBreaks)
						{
							byte[] pEncodeBuffer5 = this.m_pEncodeBuffer;
							num2 = this.m_EncodeBufferOffset;
							this.m_EncodeBufferOffset = num2 + 1;
							pEncodeBuffer5[num2] = 13;
							byte[] pEncodeBuffer6 = this.m_pEncodeBuffer;
							num2 = this.m_EncodeBufferOffset;
							this.m_EncodeBufferOffset = num2 + 1;
							pEncodeBuffer6[num2] = 10;
						}
						this.m_pStream.Write(this.m_pEncodeBuffer, 0, this.m_EncodeBufferOffset);
						this.m_EncodeBufferOffset = 0;
					}
					this.m_OffsetInEncode3x8Block = 0;
				}
			}
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x00046260 File Offset: 0x00045260
		public void Finish()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool isFinished = this.m_IsFinished;
			if (!isFinished)
			{
				this.m_IsFinished = true;
				bool flag = this.m_OffsetInEncode3x8Block == 1;
				if (flag)
				{
					byte[] pEncodeBuffer = this.m_pEncodeBuffer;
					int encodeBufferOffset = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = encodeBufferOffset + 1;
					pEncodeBuffer[encodeBufferOffset] = Base64Stream.BASE64_ENCODE_TABLE[this.m_pEncode3x8Block[0] >> 2];
					byte[] pEncodeBuffer2 = this.m_pEncodeBuffer;
					encodeBufferOffset = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = encodeBufferOffset + 1;
					pEncodeBuffer2[encodeBufferOffset] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[0] & 3) << 4];
					byte[] pEncodeBuffer3 = this.m_pEncodeBuffer;
					encodeBufferOffset = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = encodeBufferOffset + 1;
					pEncodeBuffer3[encodeBufferOffset] = 61;
					byte[] pEncodeBuffer4 = this.m_pEncodeBuffer;
					encodeBufferOffset = this.m_EncodeBufferOffset;
					this.m_EncodeBufferOffset = encodeBufferOffset + 1;
					pEncodeBuffer4[encodeBufferOffset] = 61;
				}
				else
				{
					bool flag2 = this.m_OffsetInEncode3x8Block == 2;
					if (flag2)
					{
						byte[] pEncodeBuffer5 = this.m_pEncodeBuffer;
						int encodeBufferOffset = this.m_EncodeBufferOffset;
						this.m_EncodeBufferOffset = encodeBufferOffset + 1;
						pEncodeBuffer5[encodeBufferOffset] = Base64Stream.BASE64_ENCODE_TABLE[this.m_pEncode3x8Block[0] >> 2];
						byte[] pEncodeBuffer6 = this.m_pEncodeBuffer;
						encodeBufferOffset = this.m_EncodeBufferOffset;
						this.m_EncodeBufferOffset = encodeBufferOffset + 1;
						pEncodeBuffer6[encodeBufferOffset] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[0] & 3) << 4 | this.m_pEncode3x8Block[1] >> 4];
						byte[] pEncodeBuffer7 = this.m_pEncodeBuffer;
						encodeBufferOffset = this.m_EncodeBufferOffset;
						this.m_EncodeBufferOffset = encodeBufferOffset + 1;
						pEncodeBuffer7[encodeBufferOffset] = Base64Stream.BASE64_ENCODE_TABLE[(int)(this.m_pEncode3x8Block[1] & 15) << 2];
						byte[] pEncodeBuffer8 = this.m_pEncodeBuffer;
						encodeBufferOffset = this.m_EncodeBufferOffset;
						this.m_EncodeBufferOffset = encodeBufferOffset + 1;
						pEncodeBuffer8[encodeBufferOffset] = 61;
					}
				}
				bool flag3 = this.m_EncodeBufferOffset > 0;
				if (flag3)
				{
					this.m_pStream.Write(this.m_pEncodeBuffer, 0, this.m_EncodeBufferOffset);
				}
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x00046420 File Offset: 0x00045420
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000B65 RID: 2917 RVA: 0x00046438 File Offset: 0x00045438
		public override bool CanRead
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return true;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x00046464 File Offset: 0x00045464
		public override bool CanSeek
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000B67 RID: 2919 RVA: 0x00046490 File Offset: 0x00045490
		public override bool CanWrite
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x000464BC File Offset: 0x000454BC
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000B69 RID: 2921 RVA: 0x000464E8 File Offset: 0x000454E8
		// (set) Token: 0x06000B6A RID: 2922 RVA: 0x00046514 File Offset: 0x00045514
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000B6B RID: 2923 RVA: 0x00046540 File Offset: 0x00045540
		// (set) Token: 0x06000B6C RID: 2924 RVA: 0x00046558 File Offset: 0x00045558
		public bool IgnoreInvalidPadding
		{
			get
			{
				return this.m_IgnoreInvalidPadding;
			}
			set
			{
				this.m_IgnoreInvalidPadding = value;
			}
		}

		// Token: 0x040004A6 RID: 1190
		private static readonly byte[] BASE64_ENCODE_TABLE = new byte[]
		{
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			43,
			47
		};

		// Token: 0x040004A7 RID: 1191
		private static readonly short[] BASE64_DECODE_TABLE = new short[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			62,
			-1,
			-1,
			-1,
			63,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x040004A8 RID: 1192
		private bool m_IsDisposed = false;

		// Token: 0x040004A9 RID: 1193
		private bool m_IsFinished = false;

		// Token: 0x040004AA RID: 1194
		private Stream m_pStream = null;

		// Token: 0x040004AB RID: 1195
		private bool m_IsOwner = false;

		// Token: 0x040004AC RID: 1196
		private bool m_AddLineBreaks = true;

		// Token: 0x040004AD RID: 1197
		private FileAccess m_AccessMode = FileAccess.ReadWrite;

		// Token: 0x040004AE RID: 1198
		private int m_EncodeBufferOffset = 0;

		// Token: 0x040004AF RID: 1199
		private int m_OffsetInEncode3x8Block = 0;

		// Token: 0x040004B0 RID: 1200
		private byte[] m_pEncode3x8Block = new byte[3];

		// Token: 0x040004B1 RID: 1201
		private byte[] m_pEncodeBuffer = new byte[78];

		// Token: 0x040004B2 RID: 1202
		private byte[] m_pDecodedBlock = null;

		// Token: 0x040004B3 RID: 1203
		private int m_DecodedBlockOffset = 0;

		// Token: 0x040004B4 RID: 1204
		private int m_DecodedBlockCount = 0;

		// Token: 0x040004B5 RID: 1205
		private Base64 m_pBase64 = null;

		// Token: 0x040004B6 RID: 1206
		private bool m_IgnoreInvalidPadding = false;
	}
}
