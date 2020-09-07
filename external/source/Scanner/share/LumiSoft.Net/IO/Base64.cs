using System;
using System.Text;

namespace LumiSoft.Net.IO
{
	// Token: 0x0200011B RID: 283
	public class Base64
	{
		// Token: 0x06000B25 RID: 2853 RVA: 0x00017E58 File Offset: 0x00016E58
		public byte[] Encode(byte[] buffer, int offset, int count, bool last)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x000448A4 File Offset: 0x000438A4
		public byte[] Decode(string value, bool ignoreNonBase64Chars)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			byte[] bytes = Encoding.ASCII.GetBytes(value);
			byte[] array = new byte[bytes.Length];
			int num = this.Decode(bytes, 0, bytes.Length, array, 0, ignoreNonBase64Chars);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x00044904 File Offset: 0x00043904
		public byte[] Decode(byte[] data, int offset, int count, bool ignoreNonBase64Chars)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = new byte[data.Length];
			int num = this.Decode(data, offset, count, array, 0, ignoreNonBase64Chars);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00044954 File Offset: 0x00043954
		public int Decode(byte[] encBuffer, int encOffset, int encCount, byte[] buffer, int offset, bool ignoreNonBase64Chars)
		{
			bool flag = encBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("encBuffer");
			}
			bool flag2 = encOffset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("encOffset", "Argument 'encOffset' value must be >= 0.");
			}
			bool flag3 = encCount < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("encCount", "Argument 'encCount' value must be >= 0.");
			}
			bool flag4 = encOffset + encCount > encBuffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("encCount", "Argument 'count' is bigger than than argument 'encBuffer'.");
			}
			bool flag5 = buffer == null;
			if (flag5)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag6 = offset < 0 || offset >= buffer.Length;
			if (flag6)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			int num = encOffset;
			int result = 0;
			byte[] array = new byte[4];
			while (num - encOffset < encCount)
			{
				int i = 0;
				while (i < 4)
				{
					bool flag7 = num - encOffset >= encCount;
					if (flag7)
					{
						bool flag8 = i == 0;
						if (flag8)
						{
							break;
						}
						throw new FormatException("Invalid incomplete base64 4-char block");
					}
					else
					{
						short num2 = (short)encBuffer[num++];
						bool flag9 = num2 == 61;
						if (flag9)
						{
							bool flag10 = i < 2;
							if (flag10)
							{
								throw new FormatException("Invalid base64 padding.");
							}
							bool flag11 = i == 2;
							if (flag11)
							{
								num++;
							}
							break;
						}
						else
						{
							bool flag12 = num2 > 127 || Base64.BASE64_DECODE_TABLE[(int)num2] == -1;
							if (flag12)
							{
								bool flag13 = !ignoreNonBase64Chars;
								if (flag13)
								{
									throw new FormatException("Invalid base64 char '" + num2 + "'.");
								}
							}
							else
							{
								array[i++] = (byte)Base64.BASE64_DECODE_TABLE[(int)num2];
							}
						}
					}
				}
				bool flag14 = i > 1;
				if (flag14)
				{
					buffer[result++] = (byte)((int)array[0] << 2 | array[1] >> 4);
				}
				bool flag15 = i > 2;
				if (flag15)
				{
					buffer[result++] = (byte)((int)(array[1] & 15) << 4 | array[2] >> 2);
				}
				bool flag16 = i > 3;
				if (flag16)
				{
					buffer[result++] = (byte)((int)(array[2] & 3) << 6 | (int)array[3]);
				}
			}
			return result;
		}

		// Token: 0x04000493 RID: 1171
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

		// Token: 0x04000494 RID: 1172
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
	}
}
