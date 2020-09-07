using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util
{
	// Token: 0x02000009 RID: 9
	public class Encdec
	{
		// Token: 0x06000042 RID: 66 RVA: 0x00003FB8 File Offset: 0x000021B8
		public static int Enc_uint16be(short s, byte[] dst, int di)
		{
			dst[di++] = (byte)(s >> 8 & 255);
			dst[di] = (byte)(s & 255);
			return 2;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003FE8 File Offset: 0x000021E8
		public static int Enc_uint32be(int i, byte[] dst, int di)
		{
			dst[di++] = (byte)(i >> 24 & 255);
			dst[di++] = (byte)(i >> 16 & 255);
			dst[di++] = (byte)(i >> 8 & 255);
			dst[di] = (byte)(i & 255);
			return 4;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004040 File Offset: 0x00002240
		public static int Enc_uint16le(short s, byte[] dst, int di)
		{
			dst[di++] = (byte)(s & 255);
			dst[di] = (byte)(s >> 8 & 255);
			return 2;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004070 File Offset: 0x00002270
		public static int Enc_uint32le(int i, byte[] dst, int di)
		{
			dst[di++] = (byte)(i & 255);
			dst[di++] = (byte)(i >> 8 & 255);
			dst[di++] = (byte)(i >> 16 & 255);
			dst[di] = (byte)(i >> 24 & 255);
			return 4;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000040C8 File Offset: 0x000022C8
		public static short Dec_uint16be(byte[] src, int si)
		{
			return (short)((int)(src[si] & byte.MaxValue) << 8 | (int)(src[si + 1] & byte.MaxValue));
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000040F4 File Offset: 0x000022F4
		public static int Dec_uint32be(byte[] src, int si)
		{
			return (int)(src[si] & byte.MaxValue) << 24 | (int)(src[si + 1] & byte.MaxValue) << 16 | (int)(src[si + 2] & byte.MaxValue) << 8 | (int)(src[si + 3] & byte.MaxValue);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000413C File Offset: 0x0000233C
		public static short Dec_uint16le(byte[] src, int si)
		{
			return (short)((int)(src[si] & byte.MaxValue) | (int)(src[si + 1] & byte.MaxValue) << 8);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004168 File Offset: 0x00002368
		public static int Dec_uint32le(byte[] src, int si)
		{
			return (int)(src[si] & byte.MaxValue) | (int)(src[si + 1] & byte.MaxValue) << 8 | (int)(src[si + 2] & byte.MaxValue) << 16 | (int)(src[si + 3] & byte.MaxValue) << 24;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000041B0 File Offset: 0x000023B0
		public static int Enc_uint64be(long l, byte[] dst, int di)
		{
			Encdec.Enc_uint32be((int)(l & (long)(-1)), dst, di + 4);
			Encdec.Enc_uint32be((int)(l >> 32 & (long)(-1)), dst, di);
			return 8;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000041E4 File Offset: 0x000023E4
		public static int Enc_uint64le(long l, byte[] dst, int di)
		{
			Encdec.Enc_uint32le((int)(l & (long)(-1)), dst, di);
			Encdec.Enc_uint32le((int)(l >> 32 & (long)(-1)), dst, di + 4);
			return 8;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004218 File Offset: 0x00002418
		public static long Dec_uint64be(byte[] src, int si)
		{
			long num = (long)Encdec.Dec_uint32be(src, si) & (long)(-1);
			num <<= 32;
			return num | ((long)Encdec.Dec_uint32be(src, si + 4) & (long)(-1));
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000424C File Offset: 0x0000244C
		public static long Dec_uint64le(byte[] src, int si)
		{
			long num = (long)Encdec.Dec_uint32le(src, si + 4) & (long)(-1);
			num <<= 32;
			return num | ((long)Encdec.Dec_uint32le(src, si) & (long)(-1));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004280 File Offset: 0x00002480
		public static int Enc_floatle(float f, byte[] dst, int di)
		{
			return Encdec.Enc_uint32le((int)BitConverter.DoubleToInt64Bits((double)f), dst, di);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000042A4 File Offset: 0x000024A4
		public static int Enc_floatbe(float f, byte[] dst, int di)
		{
			return Encdec.Enc_uint32be((int)BitConverter.DoubleToInt64Bits((double)f), dst, di);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000042C8 File Offset: 0x000024C8
		public static float Dec_floatle(byte[] src, int si)
		{
			return (float)BitConverter.Int64BitsToDouble((long)Encdec.Dec_uint32le(src, si));
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000042E8 File Offset: 0x000024E8
		public static float Dec_floatbe(byte[] src, int si)
		{
			return (float)BitConverter.Int64BitsToDouble((long)Encdec.Dec_uint32be(src, si));
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004308 File Offset: 0x00002508
		public static int Enc_doublele(double d, byte[] dst, int di)
		{
			return Encdec.Enc_uint64le(BitConverter.DoubleToInt64Bits(d), dst, di);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004328 File Offset: 0x00002528
		public static int Enc_doublebe(double d, byte[] dst, int di)
		{
			return Encdec.Enc_uint64be(BitConverter.DoubleToInt64Bits(d), dst, di);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004348 File Offset: 0x00002548
		public static double Dec_doublele(byte[] src, int si)
		{
			return BitConverter.Int64BitsToDouble(Encdec.Dec_uint64le(src, si));
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00004368 File Offset: 0x00002568
		public static double Dec_doublebe(byte[] src, int si)
		{
			return BitConverter.Int64BitsToDouble(Encdec.Dec_uint64be(src, si));
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004388 File Offset: 0x00002588
		public static int Enc_time(DateTime date, byte[] dst, int di, int enc)
		{
			int result;
			switch (enc)
			{
			case 1:
				result = Encdec.Enc_uint32be((int)(date.GetTime() / 1000L), dst, di);
				break;
			case 2:
				result = Encdec.Enc_uint32le((int)(date.GetTime() / 1000L), dst, di);
				break;
			case 3:
				result = Encdec.Enc_uint32be((int)(date.GetTime() / 1000L + 2082844800L & -1L), dst, di);
				break;
			case 4:
				result = Encdec.Enc_uint32le((int)(date.GetTime() / 1000L + 2082844800L & -1L), dst, di);
				break;
			case 5:
			{
				long l = (date.GetTime() + 11644473600000L) * 10000L;
				result = Encdec.Enc_uint64le(l, dst, di);
				break;
			}
			case 6:
			{
				long l = (date.GetTime() + 11644473600000L) * 10000L;
				result = Encdec.Enc_uint64be(l, dst, di);
				break;
			}
			case 7:
				result = Encdec.Enc_uint64be(date.GetTime(), dst, di);
				break;
			case 8:
				result = Encdec.Enc_uint64le(date.GetTime(), dst, di);
				break;
			default:
				throw new ArgumentException("Unsupported time encoding");
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000044C0 File Offset: 0x000026C0
		public static DateTime Dec_time(byte[] src, int si, int enc)
		{
			DateTime result;
			switch (enc)
			{
			case 1:
				result = Extensions.CreateDate((long)Encdec.Dec_uint32be(src, si) * 1000L);
				break;
			case 2:
				result = Extensions.CreateDate((long)Encdec.Dec_uint32le(src, si) * 1000L);
				break;
			case 3:
				result = Extensions.CreateDate((((long)Encdec.Dec_uint32be(src, si) & (long)(-1)) - 2082844800L) * 1000L);
				break;
			case 4:
				result = Extensions.CreateDate((((long)Encdec.Dec_uint32le(src, si) & (long)(-1)) - 2082844800L) * 1000L);
				break;
			case 5:
			{
				long num = Encdec.Dec_uint64le(src, si);
				result = Extensions.CreateDate(num / 10000L - 11644473600000L);
				break;
			}
			case 6:
			{
				long num = Encdec.Dec_uint64be(src, si);
				result = Extensions.CreateDate(num / 10000L - 11644473600000L);
				break;
			}
			case 7:
				result = Extensions.CreateDate(Encdec.Dec_uint64be(src, si));
				break;
			case 8:
				result = Extensions.CreateDate(Encdec.Dec_uint64le(src, si));
				break;
			default:
				throw new ArgumentException("Unsupported time encoding");
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000045F0 File Offset: 0x000027F0
		public static int Enc_utf8(string str, byte[] dst, int di, int dlim)
		{
			int num = di;
			int length = str.Length;
			int num2 = 0;
			while (di < dlim && num2 < length)
			{
				int num3 = (int)str[num2];
				bool flag = num3 >= 1 && num3 <= 127;
				if (flag)
				{
					dst[di++] = (byte)num3;
				}
				else
				{
					bool flag2 = num3 > 2047;
					if (flag2)
					{
						bool flag3 = dlim - di < 3;
						if (flag3)
						{
							break;
						}
						dst[di++] = (byte)(224 | (num3 >> 12 & 15));
						dst[di++] = (byte)(128 | (num3 >> 6 & 63));
						dst[di++] = (byte)(128 | (num3 & 63));
					}
					else
					{
						bool flag4 = dlim - di < 2;
						if (flag4)
						{
							break;
						}
						dst[di++] = (byte)(192 | (num3 >> 6 & 31));
						dst[di++] = (byte)(128 | (num3 & 63));
					}
				}
				num2++;
			}
			return di - num;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000046FC File Offset: 0x000028FC
		public static string Dec_utf8(byte[] src, int si, int slim)
		{
			char[] array = new char[slim - si];
			int num = 0;
			int num2;
			while (si < slim && (num2 = (int)(src[si++] & 255)) != 0)
			{
				bool flag = num2 < 128;
				if (flag)
				{
					array[num] = (char)num2;
				}
				else
				{
					bool flag2 = (num2 & 224) == 192;
					if (flag2)
					{
						bool flag3 = slim - si < 2;
						if (flag3)
						{
							break;
						}
						array[num] = (char)((num2 & 31) << 6);
						num2 = (int)(src[si++] & byte.MaxValue);
						char[] array2 = array;
						int num3 = num;
						array2[num3] |= (char)((ushort)num2 & 63);
						bool flag4 = (num2 & 192) != 128 || array[num] < '\u0080';
						if (flag4)
						{
							throw new IOException("Invalid UTF-8 sequence");
						}
					}
					else
					{
						bool flag5 = (num2 & 240) == 224;
						if (!flag5)
						{
							throw new IOException("Unsupported UTF-8 sequence");
						}
						bool flag6 = slim - si < 3;
						if (flag6)
						{
							break;
						}
						array[num] = (char)((num2 & 15) << 12);
						num2 = (int)(src[si++] & byte.MaxValue);
						bool flag7 = (num2 & 192) != 128;
						if (flag7)
						{
							throw new IOException("Invalid UTF-8 sequence");
						}
						char[] array3 = array;
						int num4 = num;
						array3[num4] |= (char)((ushort)(num2 & 63) << 6);
						num2 = (int)(src[si++] & byte.MaxValue);
						char[] array4 = array;
						int num5 = num;
						array4[num5] |= (char)((ushort)num2 & 63);
						bool flag8 = (num2 & 192) != 128 || array[num] < 'ࠀ';
						if (flag8)
						{
							throw new IOException("Invalid UTF-8 sequence");
						}
					}
				}
				num++;
			}
			return new string(array, 0, num);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000048CC File Offset: 0x00002ACC
		public static string Dec_ucs2le(byte[] src, int si, int slim, char[] buf)
		{
			int num = 0;
			while (si + 1 < slim)
			{
				buf[num] = (char)Encdec.Dec_uint16le(src, si);
				bool flag = buf[num] == '\0';
				if (flag)
				{
					break;
				}
				num++;
				si += 2;
			}
			return new string(buf, 0, num);
		}

		// Token: 0x04000025 RID: 37
		public const long MillisecondsBetween1970And1601 = 11644473600000L;

		// Token: 0x04000026 RID: 38
		public const long SecBetweeen1904And1970 = 2082844800L;

		// Token: 0x04000027 RID: 39
		public const int Time1970Sec32Be = 1;

		// Token: 0x04000028 RID: 40
		public const int Time1970Sec32Le = 2;

		// Token: 0x04000029 RID: 41
		public const int Time1904Sec32Be = 3;

		// Token: 0x0400002A RID: 42
		public const int Time1904Sec32Le = 4;

		// Token: 0x0400002B RID: 43
		public const int Time1601Nanos64Le = 5;

		// Token: 0x0400002C RID: 44
		public const int Time1601Nanos64Be = 6;

		// Token: 0x0400002D RID: 45
		public const int Time1970Millis64Be = 7;

		// Token: 0x0400002E RID: 46
		public const int Time1970Millis64Le = 8;
	}
}
