using System;

namespace LumiSoft.Net
{
	// Token: 0x0200000A RID: 10
	internal class MD4Managed : _MD4
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002428 File Offset: 0x00001428
		public MD4Managed()
		{
			this.state = new uint[4];
			this.count = new uint[2];
			this.buffer = new byte[64];
			this.digest = new byte[16];
			this.x = new uint[16];
			this.Initialize();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002484 File Offset: 0x00001484
		public override void Initialize()
		{
			this.count[0] = 0U;
			this.count[1] = 0U;
			this.state[0] = 1732584193U;
			this.state[1] = 4023233417U;
			this.state[2] = 2562383102U;
			this.state[3] = 271733878U;
			Array.Clear(this.buffer, 0, 64);
			Array.Clear(this.x, 0, 16);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000024F8 File Offset: 0x000014F8
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			int num = (int)(this.count[0] >> 3 & 63U);
			this.count[0] += (uint)((uint)cbSize << 3);
			bool flag = (ulong)this.count[0] < (ulong)((long)((long)cbSize << 3));
			if (flag)
			{
				this.count[1] += 1U;
			}
			this.count[1] += (uint)(cbSize >> 29);
			int num2 = 64 - num;
			int num3 = 0;
			bool flag2 = cbSize >= num2;
			if (flag2)
			{
				Buffer.BlockCopy(array, ibStart, this.buffer, num, num2);
				this.MD4Transform(this.state, this.buffer, 0);
				num3 = num2;
				while (num3 + 63 < cbSize)
				{
					this.MD4Transform(this.state, array, num3);
					num3 += 64;
				}
				num = 0;
			}
			Buffer.BlockCopy(array, ibStart + num3, this.buffer, num, cbSize - num3);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025D8 File Offset: 0x000015D8
		protected override byte[] HashFinal()
		{
			byte[] array = new byte[8];
			this.Encode(array, this.count);
			uint num = this.count[0] >> 3 & 63U;
			int num2 = (int)((num < 56U) ? (56U - num) : (120U - num));
			this.HashCore(this.Padding(num2), 0, num2);
			this.HashCore(array, 0, 8);
			this.Encode(this.digest, this.state);
			this.Initialize();
			return this.digest;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002658 File Offset: 0x00001658
		private byte[] Padding(int nLength)
		{
			bool flag = nLength > 0;
			byte[] result;
			if (flag)
			{
				byte[] array = new byte[nLength];
				array[0] = 128;
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002688 File Offset: 0x00001688
		private uint F(uint x, uint y, uint z)
		{
			return (x & y) | (~x & z);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026A4 File Offset: 0x000016A4
		private uint G(uint x, uint y, uint z)
		{
			return (x & y) | (x & z) | (y & z);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000026C4 File Offset: 0x000016C4
		private uint H(uint x, uint y, uint z)
		{
			return x ^ y ^ z;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000026DC File Offset: 0x000016DC
		private uint ROL(uint x, byte n)
		{
			return x << (int)n | x >> (int)(32 - n);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000026FE File Offset: 0x000016FE
		private void FF(ref uint a, uint b, uint c, uint d, uint x, byte s)
		{
			a += this.F(b, c, d) + x;
			a = this.ROL(a, s);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000271F File Offset: 0x0000171F
		private void GG(ref uint a, uint b, uint c, uint d, uint x, byte s)
		{
			a += this.G(b, c, d) + x + 1518500249U;
			a = this.ROL(a, s);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002746 File Offset: 0x00001746
		private void HH(ref uint a, uint b, uint c, uint d, uint x, byte s)
		{
			a += this.H(b, c, d) + x + 1859775393U;
			a = this.ROL(a, s);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002770 File Offset: 0x00001770
		private void Encode(byte[] output, uint[] input)
		{
			int num = 0;
			for (int i = 0; i < output.Length; i += 4)
			{
				output[i] = (byte)input[num];
				output[i + 1] = (byte)(input[num] >> 8);
				output[i + 2] = (byte)(input[num] >> 16);
				output[i + 3] = (byte)(input[num] >> 24);
				num++;
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000027C4 File Offset: 0x000017C4
		private void Decode(uint[] output, byte[] input, int index)
		{
			int i = 0;
			int num = index;
			while (i < output.Length)
			{
				output[i] = (uint)((int)input[num] | (int)input[num + 1] << 8 | (int)input[num + 2] << 16 | (int)input[num + 3] << 24);
				i++;
				num += 4;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x0000280C File Offset: 0x0000180C
		private void MD4Transform(uint[] state, byte[] block, int index)
		{
			uint num = state[0];
			uint num2 = state[1];
			uint num3 = state[2];
			uint num4 = state[3];
			this.Decode(this.x, block, index);
			this.FF(ref num, num2, num3, num4, this.x[0], 3);
			this.FF(ref num4, num, num2, num3, this.x[1], 7);
			this.FF(ref num3, num4, num, num2, this.x[2], 11);
			this.FF(ref num2, num3, num4, num, this.x[3], 19);
			this.FF(ref num, num2, num3, num4, this.x[4], 3);
			this.FF(ref num4, num, num2, num3, this.x[5], 7);
			this.FF(ref num3, num4, num, num2, this.x[6], 11);
			this.FF(ref num2, num3, num4, num, this.x[7], 19);
			this.FF(ref num, num2, num3, num4, this.x[8], 3);
			this.FF(ref num4, num, num2, num3, this.x[9], 7);
			this.FF(ref num3, num4, num, num2, this.x[10], 11);
			this.FF(ref num2, num3, num4, num, this.x[11], 19);
			this.FF(ref num, num2, num3, num4, this.x[12], 3);
			this.FF(ref num4, num, num2, num3, this.x[13], 7);
			this.FF(ref num3, num4, num, num2, this.x[14], 11);
			this.FF(ref num2, num3, num4, num, this.x[15], 19);
			this.GG(ref num, num2, num3, num4, this.x[0], 3);
			this.GG(ref num4, num, num2, num3, this.x[4], 5);
			this.GG(ref num3, num4, num, num2, this.x[8], 9);
			this.GG(ref num2, num3, num4, num, this.x[12], 13);
			this.GG(ref num, num2, num3, num4, this.x[1], 3);
			this.GG(ref num4, num, num2, num3, this.x[5], 5);
			this.GG(ref num3, num4, num, num2, this.x[9], 9);
			this.GG(ref num2, num3, num4, num, this.x[13], 13);
			this.GG(ref num, num2, num3, num4, this.x[2], 3);
			this.GG(ref num4, num, num2, num3, this.x[6], 5);
			this.GG(ref num3, num4, num, num2, this.x[10], 9);
			this.GG(ref num2, num3, num4, num, this.x[14], 13);
			this.GG(ref num, num2, num3, num4, this.x[3], 3);
			this.GG(ref num4, num, num2, num3, this.x[7], 5);
			this.GG(ref num3, num4, num, num2, this.x[11], 9);
			this.GG(ref num2, num3, num4, num, this.x[15], 13);
			this.HH(ref num, num2, num3, num4, this.x[0], 3);
			this.HH(ref num4, num, num2, num3, this.x[8], 9);
			this.HH(ref num3, num4, num, num2, this.x[4], 11);
			this.HH(ref num2, num3, num4, num, this.x[12], 15);
			this.HH(ref num, num2, num3, num4, this.x[2], 3);
			this.HH(ref num4, num, num2, num3, this.x[10], 9);
			this.HH(ref num3, num4, num, num2, this.x[6], 11);
			this.HH(ref num2, num3, num4, num, this.x[14], 15);
			this.HH(ref num, num2, num3, num4, this.x[1], 3);
			this.HH(ref num4, num, num2, num3, this.x[9], 9);
			this.HH(ref num3, num4, num, num2, this.x[5], 11);
			this.HH(ref num2, num3, num4, num, this.x[13], 15);
			this.HH(ref num, num2, num3, num4, this.x[3], 3);
			this.HH(ref num4, num, num2, num3, this.x[11], 9);
			this.HH(ref num3, num4, num, num2, this.x[7], 11);
			this.HH(ref num2, num3, num4, num, this.x[15], 15);
			state[0] += num;
			state[1] += num2;
			state[2] += num3;
			state[3] += num4;
		}

		// Token: 0x04000015 RID: 21
		private uint[] state;

		// Token: 0x04000016 RID: 22
		private byte[] buffer;

		// Token: 0x04000017 RID: 23
		private uint[] count;

		// Token: 0x04000018 RID: 24
		private uint[] x;

		// Token: 0x04000019 RID: 25
		private const int S11 = 3;

		// Token: 0x0400001A RID: 26
		private const int S12 = 7;

		// Token: 0x0400001B RID: 27
		private const int S13 = 11;

		// Token: 0x0400001C RID: 28
		private const int S14 = 19;

		// Token: 0x0400001D RID: 29
		private const int S21 = 3;

		// Token: 0x0400001E RID: 30
		private const int S22 = 5;

		// Token: 0x0400001F RID: 31
		private const int S23 = 9;

		// Token: 0x04000020 RID: 32
		private const int S24 = 13;

		// Token: 0x04000021 RID: 33
		private const int S31 = 3;

		// Token: 0x04000022 RID: 34
		private const int S32 = 9;

		// Token: 0x04000023 RID: 35
		private const int S33 = 11;

		// Token: 0x04000024 RID: 36
		private const int S34 = 15;

		// Token: 0x04000025 RID: 37
		private byte[] digest;
	}
}
