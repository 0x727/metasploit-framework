using System;
using System.Text;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000053 RID: 83
	public sealed class Md5Core
	{
		// Token: 0x060001FD RID: 509 RVA: 0x00003195 File Offset: 0x00001395
		private Md5Core()
		{
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00009450 File Offset: 0x00007650
		public static byte[] GetHash(string input, Encoding encoding)
		{
			bool flag = input == null;
			if (flag)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			bool flag2 = encoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
			}
			byte[] bytes = encoding.GetBytes(input);
			return Md5Core.GetHash(bytes);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x000094A0 File Offset: 0x000076A0
		public static byte[] GetHash(string input)
		{
			return Md5Core.GetHash(input, new UTF8Encoding());
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000094C0 File Offset: 0x000076C0
		public static string GetHashString(byte[] input)
		{
			bool flag = input == null;
			if (flag)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			string text = BitConverter.ToString(Md5Core.GetHash(input));
			return text.Replace("-", "");
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00009508 File Offset: 0x00007708
		public static string GetHashString(string input, Encoding encoding)
		{
			bool flag = input == null;
			if (flag)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			bool flag2 = encoding == null;
			if (flag2)
			{
				throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
			}
			byte[] bytes = encoding.GetBytes(input);
			return Md5Core.GetHashString(bytes);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00009558 File Offset: 0x00007758
		public static string GetHashString(string input)
		{
			return Md5Core.GetHashString(input, new UTF8Encoding());
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00009578 File Offset: 0x00007778
		public static byte[] GetHash(byte[] input)
		{
			bool flag = input == null;
			if (flag)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			AbcdStruct abcd = default(AbcdStruct);
			abcd.A = 1732584193U;
			abcd.B = 4023233417U;
			abcd.C = 2562383102U;
			abcd.D = 271733878U;
			int i;
			for (i = 0; i <= input.Length - 64; i += 64)
			{
				Md5Core.GetHashBlock(input, ref abcd, i);
			}
			return Md5Core.GetHashFinalBlock(input, i, input.Length - i, abcd, (long)input.Length * 8L);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00009618 File Offset: 0x00007818
		internal static byte[] GetHashFinalBlock(byte[] input, int ibStart, int cbSize, AbcdStruct abcd, long len)
		{
			byte[] array = new byte[64];
			byte[] bytes = BitConverter.GetBytes(len);
			Array.Copy(input, ibStart, array, 0, cbSize);
			array[cbSize] = 128;
			bool flag = cbSize < 56;
			if (flag)
			{
				Array.Copy(bytes, 0, array, 56, 8);
				Md5Core.GetHashBlock(array, ref abcd, 0);
			}
			else
			{
				Md5Core.GetHashBlock(array, ref abcd, 0);
				array = new byte[64];
				Array.Copy(bytes, 0, array, 56, 8);
				Md5Core.GetHashBlock(array, ref abcd, 0);
			}
			byte[] array2 = new byte[16];
			Array.Copy(BitConverter.GetBytes(abcd.A), 0, array2, 0, 4);
			Array.Copy(BitConverter.GetBytes(abcd.B), 0, array2, 4, 4);
			Array.Copy(BitConverter.GetBytes(abcd.C), 0, array2, 8, 4);
			Array.Copy(BitConverter.GetBytes(abcd.D), 0, array2, 12, 4);
			return array2;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000096FC File Offset: 0x000078FC
		internal static void GetHashBlock(byte[] input, ref AbcdStruct abcdValue, int ibStart)
		{
			uint[] array = Md5Core.Converter(input, ibStart);
			uint num = abcdValue.A;
			uint num2 = abcdValue.B;
			uint num3 = abcdValue.C;
			uint num4 = abcdValue.D;
			num = Md5Core.R1(num, num2, num3, num4, array[0], 7, 3614090360U);
			num4 = Md5Core.R1(num4, num, num2, num3, array[1], 12, 3905402710U);
			num3 = Md5Core.R1(num3, num4, num, num2, array[2], 17, 606105819U);
			num2 = Md5Core.R1(num2, num3, num4, num, array[3], 22, 3250441966U);
			num = Md5Core.R1(num, num2, num3, num4, array[4], 7, 4118548399U);
			num4 = Md5Core.R1(num4, num, num2, num3, array[5], 12, 1200080426U);
			num3 = Md5Core.R1(num3, num4, num, num2, array[6], 17, 2821735955U);
			num2 = Md5Core.R1(num2, num3, num4, num, array[7], 22, 4249261313U);
			num = Md5Core.R1(num, num2, num3, num4, array[8], 7, 1770035416U);
			num4 = Md5Core.R1(num4, num, num2, num3, array[9], 12, 2336552879U);
			num3 = Md5Core.R1(num3, num4, num, num2, array[10], 17, 4294925233U);
			num2 = Md5Core.R1(num2, num3, num4, num, array[11], 22, 2304563134U);
			num = Md5Core.R1(num, num2, num3, num4, array[12], 7, 1804603682U);
			num4 = Md5Core.R1(num4, num, num2, num3, array[13], 12, 4254626195U);
			num3 = Md5Core.R1(num3, num4, num, num2, array[14], 17, 2792965006U);
			num2 = Md5Core.R1(num2, num3, num4, num, array[15], 22, 1236535329U);
			num = Md5Core.R2(num, num2, num3, num4, array[1], 5, 4129170786U);
			num4 = Md5Core.R2(num4, num, num2, num3, array[6], 9, 3225465664U);
			num3 = Md5Core.R2(num3, num4, num, num2, array[11], 14, 643717713U);
			num2 = Md5Core.R2(num2, num3, num4, num, array[0], 20, 3921069994U);
			num = Md5Core.R2(num, num2, num3, num4, array[5], 5, 3593408605U);
			num4 = Md5Core.R2(num4, num, num2, num3, array[10], 9, 38016083U);
			num3 = Md5Core.R2(num3, num4, num, num2, array[15], 14, 3634488961U);
			num2 = Md5Core.R2(num2, num3, num4, num, array[4], 20, 3889429448U);
			num = Md5Core.R2(num, num2, num3, num4, array[9], 5, 568446438U);
			num4 = Md5Core.R2(num4, num, num2, num3, array[14], 9, 3275163606U);
			num3 = Md5Core.R2(num3, num4, num, num2, array[3], 14, 4107603335U);
			num2 = Md5Core.R2(num2, num3, num4, num, array[8], 20, 1163531501U);
			num = Md5Core.R2(num, num2, num3, num4, array[13], 5, 2850285829U);
			num4 = Md5Core.R2(num4, num, num2, num3, array[2], 9, 4243563512U);
			num3 = Md5Core.R2(num3, num4, num, num2, array[7], 14, 1735328473U);
			num2 = Md5Core.R2(num2, num3, num4, num, array[12], 20, 2368359562U);
			num = Md5Core.R3(num, num2, num3, num4, array[5], 4, 4294588738U);
			num4 = Md5Core.R3(num4, num, num2, num3, array[8], 11, 2272392833U);
			num3 = Md5Core.R3(num3, num4, num, num2, array[11], 16, 1839030562U);
			num2 = Md5Core.R3(num2, num3, num4, num, array[14], 23, 4259657740U);
			num = Md5Core.R3(num, num2, num3, num4, array[1], 4, 2763975236U);
			num4 = Md5Core.R3(num4, num, num2, num3, array[4], 11, 1272893353U);
			num3 = Md5Core.R3(num3, num4, num, num2, array[7], 16, 4139469664U);
			num2 = Md5Core.R3(num2, num3, num4, num, array[10], 23, 3200236656U);
			num = Md5Core.R3(num, num2, num3, num4, array[13], 4, 681279174U);
			num4 = Md5Core.R3(num4, num, num2, num3, array[0], 11, 3936430074U);
			num3 = Md5Core.R3(num3, num4, num, num2, array[3], 16, 3572445317U);
			num2 = Md5Core.R3(num2, num3, num4, num, array[6], 23, 76029189U);
			num = Md5Core.R3(num, num2, num3, num4, array[9], 4, 3654602809U);
			num4 = Md5Core.R3(num4, num, num2, num3, array[12], 11, 3873151461U);
			num3 = Md5Core.R3(num3, num4, num, num2, array[15], 16, 530742520U);
			num2 = Md5Core.R3(num2, num3, num4, num, array[2], 23, 3299628645U);
			num = Md5Core.R4(num, num2, num3, num4, array[0], 6, 4096336452U);
			num4 = Md5Core.R4(num4, num, num2, num3, array[7], 10, 1126891415U);
			num3 = Md5Core.R4(num3, num4, num, num2, array[14], 15, 2878612391U);
			num2 = Md5Core.R4(num2, num3, num4, num, array[5], 21, 4237533241U);
			num = Md5Core.R4(num, num2, num3, num4, array[12], 6, 1700485571U);
			num4 = Md5Core.R4(num4, num, num2, num3, array[3], 10, 2399980690U);
			num3 = Md5Core.R4(num3, num4, num, num2, array[10], 15, 4293915773U);
			num2 = Md5Core.R4(num2, num3, num4, num, array[1], 21, 2240044497U);
			num = Md5Core.R4(num, num2, num3, num4, array[8], 6, 1873313359U);
			num4 = Md5Core.R4(num4, num, num2, num3, array[15], 10, 4264355552U);
			num3 = Md5Core.R4(num3, num4, num, num2, array[6], 15, 2734768916U);
			num2 = Md5Core.R4(num2, num3, num4, num, array[13], 21, 1309151649U);
			num = Md5Core.R4(num, num2, num3, num4, array[4], 6, 4149444226U);
			num4 = Md5Core.R4(num4, num, num2, num3, array[11], 10, 3174756917U);
			num3 = Md5Core.R4(num3, num4, num, num2, array[2], 15, 718787259U);
			num2 = Md5Core.R4(num2, num3, num4, num, array[9], 21, 3951481745U);
			abcdValue.A = num + abcdValue.A;
			abcdValue.B = num2 + abcdValue.B;
			abcdValue.C = num3 + abcdValue.C;
			abcdValue.D = num4 + abcdValue.D;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00009CC4 File Offset: 0x00007EC4
		private static uint R1(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + Md5Core.Lsr(a + ((b & c) | ((b ^ uint.MaxValue) & d)) + x + t, s);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009CF0 File Offset: 0x00007EF0
		private static uint R2(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + Md5Core.Lsr(a + ((b & d) | (c & (d ^ uint.MaxValue))) + x + t, s);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00009D1C File Offset: 0x00007F1C
		private static uint R3(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + Md5Core.Lsr(a + (b ^ c ^ d) + x + t, s);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00009D44 File Offset: 0x00007F44
		private static uint R4(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + Md5Core.Lsr(a + (c ^ (b | (d ^ uint.MaxValue))) + x + t, s);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00009D70 File Offset: 0x00007F70
		private static uint Lsr(uint i, int s)
		{
			return i << s | i >> 32 - s;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00009D94 File Offset: 0x00007F94
		private static uint[] Converter(byte[] input, int ibStart)
		{
			bool flag = input == null;
			if (flag)
			{
				throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
			}
			uint[] array = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = (uint)input[ibStart + i * 4];
				array[i] += (uint)((uint)input[ibStart + i * 4 + 1] << 8);
				array[i] += (uint)((uint)input[ibStart + i * 4 + 2] << 16);
				array[i] += (uint)((uint)input[ibStart + i * 4 + 3] << 24);
			}
			return array;
		}
	}
}
