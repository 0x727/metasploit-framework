using System;

namespace SharpCifs.Util
{
	// Token: 0x02000008 RID: 8
	public class DES
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00003574 File Offset: 0x00001774
		public DES()
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000035A4 File Offset: 0x000017A4
		public DES(byte[] key)
		{
			bool flag = key.Length == 7;
			if (flag)
			{
				byte[] array = new byte[8];
				DES.MakeSmbKey(key, array);
				this.SetKey(array);
			}
			else
			{
				this.SetKey(key);
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003610 File Offset: 0x00001810
		public static void MakeSmbKey(byte[] key7, byte[] key8)
		{
			key8[0] = (byte)(key7[0] >> 1 & 255);
			key8[1] = (byte)(((int)(key7[0] & 1) << 6 | ((key7[1] & byte.MaxValue) >> 2 & 255)) & 255);
			key8[2] = (byte)(((int)(key7[1] & 3) << 5 | ((key7[2] & byte.MaxValue) >> 3 & 255)) & 255);
			key8[3] = (byte)(((int)(key7[2] & 7) << 4 | ((key7[3] & byte.MaxValue) >> 4 & 255)) & 255);
			key8[4] = (byte)(((int)(key7[3] & 15) << 3 | ((key7[4] & byte.MaxValue) >> 5 & 255)) & 255);
			key8[5] = (byte)(((int)(key7[4] & 31) << 2 | ((key7[5] & byte.MaxValue) >> 6 & 255)) & 255);
			key8[6] = (byte)(((int)(key7[5] & 63) << 1 | ((key7[6] & byte.MaxValue) >> 7 & 255)) & 255);
			key8[7] = (byte)(key7[6] & 127);
			for (int i = 0; i < 8; i++)
			{
				key8[i] = (byte)(key8[i] << 1);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003727 File Offset: 0x00001927
		public virtual void SetKey(byte[] key)
		{
			this.Deskey(key, true, this._encryptKeys);
			this.Deskey(key, false, this._decryptKeys);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003748 File Offset: 0x00001948
		private void Deskey(byte[] keyBlock, bool encrypting, int[] knL)
		{
			int[] array = new int[56];
			int[] array2 = new int[56];
			int[] array3 = new int[32];
			for (int i = 0; i < 56; i++)
			{
				int num = (int)DES._pc1[i];
				int num2 = num & 7;
				array[i] = (((keyBlock[(int)((uint)num >> 3)] & DES._bytebit[num2]) != 0) ? 1 : 0);
			}
			for (int j = 0; j < 16; j++)
			{
				int num2;
				if (encrypting)
				{
					num2 = j << 1;
				}
				else
				{
					num2 = 15 - j << 1;
				}
				int num3 = num2 + 1;
				array3[num2] = (array3[num3] = 0);
				for (int i = 0; i < 28; i++)
				{
					int num = i + DES._totrot[j];
					bool flag = num < 28;
					if (flag)
					{
						array2[i] = array[num];
					}
					else
					{
						array2[i] = array[num - 28];
					}
				}
				for (int i = 28; i < 56; i++)
				{
					int num = i + DES._totrot[j];
					bool flag2 = num < 56;
					if (flag2)
					{
						array2[i] = array[num];
					}
					else
					{
						array2[i] = array[num - 28];
					}
				}
				for (int i = 0; i < 24; i++)
				{
					bool flag3 = array2[(int)DES._pc2[i]] != 0;
					if (flag3)
					{
						array3[num2] |= DES._bigbyte[i];
					}
					bool flag4 = array2[(int)DES._pc2[i + 24]] != 0;
					if (flag4)
					{
						array3[num3] |= DES._bigbyte[i];
					}
				}
			}
			this.Cookey(array3, knL);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000038EC File Offset: 0x00001AEC
		private void Cookey(int[] raw, int[] knL)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			while (i < 16)
			{
				int num3 = raw[num++];
				int num4 = raw[num++];
				knL[num2] = (num3 & 16515072) << 6;
				knL[num2] |= (num3 & 4032) << 10;
				knL[num2] |= (int)((uint)(num4 & 16515072) >> 10);
				knL[num2] |= (int)((uint)(num4 & 4032) >> 6);
				num2++;
				knL[num2] = (num3 & 258048) << 12;
				knL[num2] |= (num3 & 63) << 16;
				knL[num2] |= (int)((uint)(num4 & 258048) >> 4);
				knL[num2] |= (num4 & 63);
				num2++;
				i++;
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000039C1 File Offset: 0x00001BC1
		private void Encrypt(byte[] clearText, int clearOff, byte[] cipherText, int cipherOff)
		{
			DES.SquashBytesToInts(clearText, clearOff, this._tempInts, 0, 2);
			this.Des(this._tempInts, this._tempInts, this._encryptKeys);
			DES.SpreadIntsToBytes(this._tempInts, 0, cipherText, cipherOff, 2);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000039FE File Offset: 0x00001BFE
		private void Decrypt(byte[] cipherText, int cipherOff, byte[] clearText, int clearOff)
		{
			DES.SquashBytesToInts(cipherText, cipherOff, this._tempInts, 0, 2);
			this.Des(this._tempInts, this._tempInts, this._decryptKeys);
			DES.SpreadIntsToBytes(this._tempInts, 0, clearText, clearOff, 2);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003A3C File Offset: 0x00001C3C
		private void Des(int[] inInts, int[] outInts, int[] keys)
		{
			int num = 0;
			int num2 = inInts[0];
			int num3 = inInts[1];
			int num4 = (int)(((uint)num2 >> 4 ^ (uint)num3) & 252645135U);
			num3 ^= num4;
			num2 ^= num4 << 4;
			num4 = (int)(((uint)num2 >> 16 ^ (uint)num3) & 65535U);
			num3 ^= num4;
			num2 ^= num4 << 16;
			num4 = (int)(((uint)num3 >> 2 ^ (uint)num2) & 858993459U);
			num2 ^= num4;
			num3 ^= num4 << 2;
			num4 = (int)(((uint)num3 >> 8 ^ (uint)num2) & 16711935U);
			num2 ^= num4;
			num3 ^= num4 << 8;
			num3 = (num3 << 1 | (int)((uint)num3 >> 31 & 1U));
			num4 = ((num2 ^ num3) & -1431655766);
			num2 ^= num4;
			num3 ^= num4;
			num2 = (num2 << 1 | (int)((uint)num2 >> 31 & 1U));
			for (int i = 0; i < 8; i++)
			{
				num4 = (num3 << 28 | (int)((uint)num3 >> 4));
				num4 ^= keys[num++];
				int num5 = DES._sp7[num4 & 63];
				num5 |= DES._sp5[(int)((uint)num4 >> 8 & 63U)];
				num5 |= DES._sp3[(int)((uint)num4 >> 16 & 63U)];
				num5 |= DES._sp1[(int)((uint)num4 >> 24 & 63U)];
				num4 = (num3 ^ keys[num++]);
				num5 |= DES._sp8[num4 & 63];
				num5 |= DES._sp6[(int)((uint)num4 >> 8 & 63U)];
				num5 |= DES._sp4[(int)((uint)num4 >> 16 & 63U)];
				num5 |= DES._sp2[(int)((uint)num4 >> 24 & 63U)];
				num2 ^= num5;
				num4 = (num2 << 28 | (int)((uint)num2 >> 4));
				num4 ^= keys[num++];
				num5 = DES._sp7[num4 & 63];
				num5 |= DES._sp5[(int)((uint)num4 >> 8 & 63U)];
				num5 |= DES._sp3[(int)((uint)num4 >> 16 & 63U)];
				num5 |= DES._sp1[(int)((uint)num4 >> 24 & 63U)];
				num4 = (num2 ^ keys[num++]);
				num5 |= DES._sp8[num4 & 63];
				num5 |= DES._sp6[(int)((uint)num4 >> 8 & 63U)];
				num5 |= DES._sp4[(int)((uint)num4 >> 16 & 63U)];
				num5 |= DES._sp2[(int)((uint)num4 >> 24 & 63U)];
				num3 ^= num5;
			}
			num3 = (num3 << 31 | (int)((uint)num3 >> 1));
			num4 = ((num2 ^ num3) & -1431655766);
			num2 ^= num4;
			num3 ^= num4;
			num2 = (num2 << 31 | (int)((uint)num2 >> 1));
			num4 = (int)(((uint)num2 >> 8 ^ (uint)num3) & 16711935U);
			num3 ^= num4;
			num2 ^= num4 << 8;
			num4 = (int)(((uint)num2 >> 2 ^ (uint)num3) & 858993459U);
			num3 ^= num4;
			num2 ^= num4 << 2;
			num4 = (int)(((uint)num3 >> 16 ^ (uint)num2) & 65535U);
			num2 ^= num4;
			num3 ^= num4 << 16;
			num4 = (int)(((uint)num3 >> 4 ^ (uint)num2) & 252645135U);
			num2 ^= num4;
			num3 ^= num4 << 4;
			outInts[0] = num3;
			outInts[1] = num2;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003CB1 File Offset: 0x00001EB1
		public virtual void Encrypt(byte[] clearText, byte[] cipherText)
		{
			this.Encrypt(clearText, 0, cipherText, 0);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003CBF File Offset: 0x00001EBF
		public virtual void Decrypt(byte[] cipherText, byte[] clearText)
		{
			this.Decrypt(cipherText, 0, clearText, 0);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003CD0 File Offset: 0x00001ED0
		public virtual byte[] Encrypt(byte[] clearText)
		{
			int num = clearText.Length;
			bool flag = num % 8 != 0;
			byte[] result;
			if (flag)
			{
				Console.Out.WriteLine("Array must be a multiple of 8");
				result = null;
			}
			else
			{
				byte[] array = new byte[num];
				int num2 = num / 8;
				for (int i = 0; i < num2; i++)
				{
					this.Encrypt(clearText, i * 8, array, i * 8);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003D40 File Offset: 0x00001F40
		public virtual byte[] Decrypt(byte[] cipherText)
		{
			int num = cipherText.Length;
			bool flag = num % 8 != 0;
			byte[] result;
			if (flag)
			{
				Console.Out.WriteLine("Array must be a multiple of 8");
				result = null;
			}
			else
			{
				byte[] array = new byte[num];
				int num2 = num / 8;
				for (int i = 0; i < num2; i++)
				{
					this.Encrypt(cipherText, i * 8, array, i * 8);
				}
				result = array;
			}
			return result;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003DB0 File Offset: 0x00001FB0
		public static void SquashBytesToInts(byte[] inBytes, int inOff, int[] outInts, int outOff, int intLen)
		{
			for (int i = 0; i < intLen; i++)
			{
				outInts[outOff + i] = ((int)(inBytes[inOff + i * 4] & byte.MaxValue) << 24 | (int)(inBytes[inOff + i * 4 + 1] & byte.MaxValue) << 16 | (int)(inBytes[inOff + i * 4 + 2] & byte.MaxValue) << 8 | (int)(inBytes[inOff + i * 4 + 3] & byte.MaxValue));
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003E1C File Offset: 0x0000201C
		public static void SpreadIntsToBytes(int[] inInts, int inOff, byte[] outBytes, int outOff, int intLen)
		{
			for (int i = 0; i < intLen; i++)
			{
				outBytes[outOff + i * 4] = (byte)((uint)inInts[inOff + i] >> 24);
				outBytes[outOff + i * 4 + 1] = (byte)((uint)inInts[inOff + i] >> 16);
				outBytes[outOff + i * 4 + 2] = (byte)((uint)inInts[inOff + i] >> 8);
				outBytes[outOff + i * 4 + 3] = (byte)inInts[inOff + i];
			}
		}

		// Token: 0x04000015 RID: 21
		private int[] _encryptKeys = new int[32];

		// Token: 0x04000016 RID: 22
		private int[] _decryptKeys = new int[32];

		// Token: 0x04000017 RID: 23
		private int[] _tempInts = new int[2];

		// Token: 0x04000018 RID: 24
		private static byte[] _bytebit = new byte[]
		{
			128,
			64,
			32,
			16,
			8,
			4,
			2,
			1
		};

		// Token: 0x04000019 RID: 25
		private static int[] _bigbyte = new int[]
		{
			8388608,
			4194304,
			2097152,
			1048576,
			524288,
			262144,
			131072,
			65536,
			32768,
			16384,
			8192,
			4096,
			2048,
			1024,
			512,
			256,
			128,
			64,
			32,
			16,
			8,
			4,
			2,
			1
		};

		// Token: 0x0400001A RID: 26
		private static byte[] _pc1 = new byte[]
		{
			56,
			48,
			40,
			32,
			24,
			16,
			8,
			0,
			57,
			49,
			41,
			33,
			25,
			17,
			9,
			1,
			58,
			50,
			42,
			34,
			26,
			18,
			10,
			2,
			59,
			51,
			43,
			35,
			62,
			54,
			46,
			38,
			30,
			22,
			14,
			6,
			61,
			53,
			45,
			37,
			29,
			21,
			13,
			5,
			60,
			52,
			44,
			36,
			28,
			20,
			12,
			4,
			27,
			19,
			11,
			3
		};

		// Token: 0x0400001B RID: 27
		private static int[] _totrot = new int[]
		{
			1,
			2,
			4,
			6,
			8,
			10,
			12,
			14,
			15,
			17,
			19,
			21,
			23,
			25,
			27,
			28
		};

		// Token: 0x0400001C RID: 28
		private static byte[] _pc2 = new byte[]
		{
			13,
			16,
			10,
			23,
			0,
			4,
			2,
			27,
			14,
			5,
			20,
			9,
			22,
			18,
			11,
			3,
			25,
			7,
			15,
			6,
			26,
			19,
			12,
			1,
			40,
			51,
			30,
			36,
			46,
			54,
			29,
			39,
			50,
			44,
			32,
			47,
			43,
			48,
			38,
			55,
			33,
			52,
			45,
			41,
			49,
			35,
			28,
			31
		};

		// Token: 0x0400001D RID: 29
		private static int[] _sp1 = new int[]
		{
			16843776,
			0,
			65536,
			16843780,
			16842756,
			66564,
			4,
			65536,
			1024,
			16843776,
			16843780,
			1024,
			16778244,
			16842756,
			16777216,
			4,
			1028,
			16778240,
			16778240,
			66560,
			66560,
			16842752,
			16842752,
			16778244,
			65540,
			16777220,
			16777220,
			65540,
			0,
			1028,
			66564,
			16777216,
			65536,
			16843780,
			4,
			16842752,
			16843776,
			16777216,
			16777216,
			1024,
			16842756,
			65536,
			66560,
			16777220,
			1024,
			4,
			16778244,
			66564,
			16843780,
			65540,
			16842752,
			16778244,
			16777220,
			1028,
			66564,
			16843776,
			1028,
			16778240,
			16778240,
			0,
			65540,
			66560,
			0,
			16842756
		};

		// Token: 0x0400001E RID: 30
		private static int[] _sp2 = new int[]
		{
			-2146402272,
			-2147450880,
			32768,
			1081376,
			1048576,
			32,
			-2146435040,
			-2147450848,
			-2147483616,
			-2146402272,
			-2146402304,
			int.MinValue,
			-2147450880,
			1048576,
			32,
			-2146435040,
			1081344,
			1048608,
			-2147450848,
			0,
			int.MinValue,
			32768,
			1081376,
			-2146435072,
			1048608,
			-2147483616,
			0,
			1081344,
			32800,
			-2146402304,
			-2146435072,
			32800,
			0,
			1081376,
			-2146435040,
			1048576,
			-2147450848,
			-2146435072,
			-2146402304,
			32768,
			-2146435072,
			-2147450880,
			32,
			-2146402272,
			1081376,
			32,
			32768,
			int.MinValue,
			32800,
			-2146402304,
			1048576,
			-2147483616,
			1048608,
			-2147450848,
			-2147483616,
			1048608,
			1081344,
			0,
			-2147450880,
			32800,
			int.MinValue,
			-2146435040,
			-2146402272,
			1081344
		};

		// Token: 0x0400001F RID: 31
		private static int[] _sp3 = new int[]
		{
			520,
			134349312,
			0,
			134348808,
			134218240,
			0,
			131592,
			134218240,
			131080,
			134217736,
			134217736,
			131072,
			134349320,
			131080,
			134348800,
			520,
			134217728,
			8,
			134349312,
			512,
			131584,
			134348800,
			134348808,
			131592,
			134218248,
			131584,
			131072,
			134218248,
			8,
			134349320,
			512,
			134217728,
			134349312,
			134217728,
			131080,
			520,
			131072,
			134349312,
			134218240,
			0,
			512,
			131080,
			134349320,
			134218240,
			134217736,
			512,
			0,
			134348808,
			134218248,
			131072,
			134217728,
			134349320,
			8,
			131592,
			131584,
			134217736,
			134348800,
			134218248,
			520,
			134348800,
			131592,
			8,
			134348808,
			131584
		};

		// Token: 0x04000020 RID: 32
		private static int[] _sp4 = new int[]
		{
			8396801,
			8321,
			8321,
			128,
			8396928,
			8388737,
			8388609,
			8193,
			0,
			8396800,
			8396800,
			8396929,
			129,
			0,
			8388736,
			8388609,
			1,
			8192,
			8388608,
			8396801,
			128,
			8388608,
			8193,
			8320,
			8388737,
			1,
			8320,
			8388736,
			8192,
			8396928,
			8396929,
			129,
			8388736,
			8388609,
			8396800,
			8396929,
			129,
			0,
			0,
			8396800,
			8320,
			8388736,
			8388737,
			1,
			8396801,
			8321,
			8321,
			128,
			8396929,
			129,
			1,
			8192,
			8388609,
			8193,
			8396928,
			8388737,
			8193,
			8320,
			8388608,
			8396801,
			128,
			8388608,
			8192,
			8396928
		};

		// Token: 0x04000021 RID: 33
		private static int[] _sp5 = new int[]
		{
			256,
			34078976,
			34078720,
			1107296512,
			524288,
			256,
			1073741824,
			34078720,
			1074266368,
			524288,
			33554688,
			1074266368,
			1107296512,
			1107820544,
			524544,
			1073741824,
			33554432,
			1074266112,
			1074266112,
			0,
			1073742080,
			1107820800,
			1107820800,
			33554688,
			1107820544,
			1073742080,
			0,
			1107296256,
			34078976,
			33554432,
			1107296256,
			524544,
			524288,
			1107296512,
			256,
			33554432,
			1073741824,
			34078720,
			1107296512,
			1074266368,
			33554688,
			1073741824,
			1107820544,
			34078976,
			1074266368,
			256,
			33554432,
			1107820544,
			1107820800,
			524544,
			1107296256,
			1107820800,
			34078720,
			0,
			1074266112,
			1107296256,
			524544,
			33554688,
			1073742080,
			524288,
			0,
			1074266112,
			34078976,
			1073742080
		};

		// Token: 0x04000022 RID: 34
		private static int[] _sp6 = new int[]
		{
			536870928,
			541065216,
			16384,
			541081616,
			541065216,
			16,
			541081616,
			4194304,
			536887296,
			4210704,
			4194304,
			536870928,
			4194320,
			536887296,
			536870912,
			16400,
			0,
			4194320,
			536887312,
			16384,
			4210688,
			536887312,
			16,
			541065232,
			541065232,
			0,
			4210704,
			541081600,
			16400,
			4210688,
			541081600,
			536870912,
			536887296,
			16,
			541065232,
			4210688,
			541081616,
			4194304,
			16400,
			536870928,
			4194304,
			536887296,
			536870912,
			16400,
			536870928,
			541081616,
			4210688,
			541065216,
			4210704,
			541081600,
			0,
			541065232,
			16,
			16384,
			541065216,
			4210704,
			16384,
			4194320,
			536887312,
			0,
			541081600,
			536870912,
			4194320,
			536887312
		};

		// Token: 0x04000023 RID: 35
		private static int[] _sp7 = new int[]
		{
			2097152,
			69206018,
			67110914,
			0,
			2048,
			67110914,
			2099202,
			69208064,
			69208066,
			2097152,
			0,
			67108866,
			2,
			67108864,
			69206018,
			2050,
			67110912,
			2099202,
			2097154,
			67110912,
			67108866,
			69206016,
			69208064,
			2097154,
			69206016,
			2048,
			2050,
			69208066,
			2099200,
			2,
			67108864,
			2099200,
			67108864,
			2099200,
			2097152,
			67110914,
			67110914,
			69206018,
			69206018,
			2,
			2097154,
			67108864,
			67110912,
			2097152,
			69208064,
			2050,
			2099202,
			69208064,
			2050,
			67108866,
			69208066,
			69206016,
			2099200,
			0,
			2,
			69208066,
			0,
			2099202,
			69206016,
			2048,
			67108866,
			67110912,
			2048,
			2097154
		};

		// Token: 0x04000024 RID: 36
		private static int[] _sp8 = new int[]
		{
			268439616,
			4096,
			262144,
			268701760,
			268435456,
			268439616,
			64,
			268435456,
			262208,
			268697600,
			268701760,
			266240,
			268701696,
			266304,
			4096,
			64,
			268697600,
			268435520,
			268439552,
			4160,
			266240,
			262208,
			268697664,
			268701696,
			4160,
			0,
			0,
			268697664,
			268435520,
			268439552,
			266304,
			262144,
			266304,
			262144,
			268701696,
			4096,
			64,
			268697664,
			4096,
			266304,
			268439552,
			64,
			268435520,
			268697600,
			268697664,
			268435456,
			262144,
			268439616,
			0,
			268701760,
			262208,
			268435520,
			268697600,
			268439552,
			268439616,
			0,
			268701760,
			266240,
			266240,
			4160,
			4160,
			262208,
			268435456,
			268701696
		};
	}
}
