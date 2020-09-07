using System;
using System.IO;

namespace SharpCifs.Util
{
	// Token: 0x0200000A RID: 10
	public class Hexdump
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00004918 File Offset: 0x00002B18
		private static bool IsIsoControl(char c)
		{
			return (c >= '\0' && c <= '\u001f') || (c >= '\u007f' && c <= '\u009f');
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000494C File Offset: 0x00002B4C
		public static void ToHexdump(TextWriter ps, byte[] src, int srcIndex, int length)
		{
			bool flag = length == 0;
			if (!flag)
			{
				int num = length % 16;
				int num2 = (num == 0) ? (length / 16) : (length / 16 + 1);
				char[] array = new char[num2 * (74 + Hexdump.NlLength)];
				char[] array2 = new char[16];
				int num3 = 0;
				int num4 = 0;
				for (;;)
				{
					Hexdump.ToHexChars(num3, array, num4, 5);
					num4 += 5;
					array[num4++] = ':';
					do
					{
						bool flag2 = num3 == length;
						if (flag2)
						{
							goto Block_3;
						}
						array[num4++] = ' ';
						int num5 = (int)(src[srcIndex + num3] & byte.MaxValue);
						Hexdump.ToHexChars(num5, array, num4, 2);
						num4 += 2;
						bool flag3 = num5 < 0 || Hexdump.IsIsoControl((char)num5);
						if (flag3)
						{
							array2[num3 % 16] = '.';
						}
						else
						{
							array2[num3 % 16] = (char)num5;
						}
					}
					while (++num3 % 16 != 0);
					IL_114:
					array[num4++] = ' ';
					array[num4++] = ' ';
					array[num4++] = '|';
					Array.Copy(array2, 0, array, num4, 16);
					num4 += 16;
					array[num4++] = '|';
					array = Hexdump.Nl.ToCharArray(0, Hexdump.NlLength);
					num4 += Hexdump.NlLength;
					if (num3 >= length)
					{
						break;
					}
					continue;
					Block_3:
					int num6 = 16 - num;
					Array.Copy(Hexdump.SpaceChars, 0, array, num4, num6 * 3);
					num4 += num6 * 3;
					Array.Copy(Hexdump.SpaceChars, 0, array2, num, num6);
					goto IL_114;
				}
				ps.WriteLine(array);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004AE0 File Offset: 0x00002CE0
		public static string ToHexString(int val, int size)
		{
			char[] array = new char[size];
			Hexdump.ToHexChars(val, array, 0, size);
			return new string(array);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004B0C File Offset: 0x00002D0C
		public static string ToHexString(long val, int size)
		{
			char[] array = new char[size];
			Hexdump.ToHexChars(val, array, 0, size);
			return new string(array);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004B38 File Offset: 0x00002D38
		public static string ToHexString(byte[] src, int srcIndex, int size)
		{
			char[] array = new char[size];
			size = ((size % 2 == 0) ? (size / 2) : (size / 2 + 1));
			int i = 0;
			int num = 0;
			while (i < size)
			{
				array[num++] = Hexdump.HexDigits[src[i] >> 4 & 15];
				bool flag = num == array.Length;
				if (flag)
				{
					break;
				}
				array[num++] = Hexdump.HexDigits[(int)(src[i] & 15)];
				i++;
			}
			return new string(array);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public static void ToHexChars(int val, char[] dst, int dstIndex, int size)
		{
			while (size > 0)
			{
				int num = dstIndex + size - 1;
				bool flag = num < dst.Length;
				if (flag)
				{
					dst[num] = Hexdump.HexDigits[val & 15];
				}
				bool flag2 = val != 0;
				if (flag2)
				{
					val = (int)((uint)val >> 4);
				}
				size--;
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004C08 File Offset: 0x00002E08
		public static void ToHexChars(long val, char[] dst, int dstIndex, int size)
		{
			while (size > 0)
			{
				dst[dstIndex + size - 1] = Hexdump.HexDigits[(int)(val & 15L)];
				bool flag = val != 0L;
				if (flag)
				{
					val = (long)((ulong)val >> 4);
				}
				size--;
			}
		}

		// Token: 0x0400002F RID: 47
		private static readonly string Nl = "\\r\\n";

		// Token: 0x04000030 RID: 48
		private static readonly int NlLength = Hexdump.Nl.Length;

		// Token: 0x04000031 RID: 49
		private static readonly char[] SpaceChars = new char[]
		{
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' '
		};

		// Token: 0x04000032 RID: 50
		public static readonly char[] HexDigits = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};
	}
}
