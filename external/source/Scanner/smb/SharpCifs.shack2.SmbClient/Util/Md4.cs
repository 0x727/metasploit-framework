using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util
{
	// Token: 0x0200000D RID: 13
	public class Md4 : MessageDigest
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00004F38 File Offset: 0x00003138
		public Md4()
		{
			this.EngineReset();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004F70 File Offset: 0x00003170
		private Md4(Md4 md) : this()
		{
			this._context = (int[])md._context.Clone();
			this._buffer = (byte[])md._buffer.Clone();
			this._count = md._count;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004FC0 File Offset: 0x000031C0
		public object Clone()
		{
			return new Md4(this);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004FD8 File Offset: 0x000031D8
		protected void EngineReset()
		{
			this._context[0] = 1732584193;
			this._context[1] = -271733879;
			this._context[2] = -1732584194;
			this._context[3] = 271733878;
			this._count = 0L;
			for (int i = 0; i < 64; i++)
			{
				this._buffer[i] = 0;
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005040 File Offset: 0x00003240
		protected void EngineUpdate(byte b)
		{
			int num = (int)(this._count % 64L);
			this._count += 1L;
			this._buffer[num] = b;
			bool flag = num == 63;
			if (flag)
			{
				this.Transform(this._buffer, 0);
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000508C File Offset: 0x0000328C
		protected void EngineUpdate(byte[] input, int offset, int len)
		{
			bool flag = offset < 0 || len < 0 || (long)offset + (long)len > (long)input.Length;
			if (flag)
			{
				throw new IndexOutOfRangeException();
			}
			int num = (int)(this._count % 64L);
			this._count += (long)len;
			int num2 = 64 - num;
			int num3 = 0;
			bool flag2 = len >= num2;
			if (flag2)
			{
				Array.Copy(input, offset, this._buffer, num, num2);
				this.Transform(this._buffer, 0);
				num3 = num2;
				while (num3 + 64 - 1 < len)
				{
					this.Transform(input, offset + num3);
					num3 += 64;
				}
				num = 0;
			}
			bool flag3 = num3 < len;
			if (flag3)
			{
				Array.Copy(input, offset + num3, this._buffer, num, len - num3);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005154 File Offset: 0x00003354
		protected byte[] EngineDigest()
		{
			int num = (int)(this._count % 64L);
			int num2 = (num < 56) ? (56 - num) : (120 - num);
			byte[] array = new byte[num2 + 8];
			array[0] = 128;
			for (int i = 0; i < 8; i++)
			{
				array[num2 + i] = (byte)((ulong)(this._count * 8L) >> 8 * i);
			}
			this.EngineUpdate(array, 0, array.Length);
			byte[] array2 = new byte[16];
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					array2[j * 4 + k] = (byte)((uint)this._context[j] >> 8 * k);
				}
			}
			this.EngineReset();
			return array2;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000522C File Offset: 0x0000342C
		private void Transform(byte[] block, int offset)
		{
			for (int i = 0; i < 16; i++)
			{
				this._x[i] = ((int)(block[offset++] & byte.MaxValue) | (int)(block[offset++] & byte.MaxValue) << 8 | (int)(block[offset++] & byte.MaxValue) << 16 | (int)(block[offset++] & byte.MaxValue) << 24);
			}
			int num = this._context[0];
			int num2 = this._context[1];
			int num3 = this._context[2];
			int num4 = this._context[3];
			num = this.Ff(num, num2, num3, num4, this._x[0], 3);
			num4 = this.Ff(num4, num, num2, num3, this._x[1], 7);
			num3 = this.Ff(num3, num4, num, num2, this._x[2], 11);
			num2 = this.Ff(num2, num3, num4, num, this._x[3], 19);
			num = this.Ff(num, num2, num3, num4, this._x[4], 3);
			num4 = this.Ff(num4, num, num2, num3, this._x[5], 7);
			num3 = this.Ff(num3, num4, num, num2, this._x[6], 11);
			num2 = this.Ff(num2, num3, num4, num, this._x[7], 19);
			num = this.Ff(num, num2, num3, num4, this._x[8], 3);
			num4 = this.Ff(num4, num, num2, num3, this._x[9], 7);
			num3 = this.Ff(num3, num4, num, num2, this._x[10], 11);
			num2 = this.Ff(num2, num3, num4, num, this._x[11], 19);
			num = this.Ff(num, num2, num3, num4, this._x[12], 3);
			num4 = this.Ff(num4, num, num2, num3, this._x[13], 7);
			num3 = this.Ff(num3, num4, num, num2, this._x[14], 11);
			num2 = this.Ff(num2, num3, num4, num, this._x[15], 19);
			num = this.Gg(num, num2, num3, num4, this._x[0], 3);
			num4 = this.Gg(num4, num, num2, num3, this._x[4], 5);
			num3 = this.Gg(num3, num4, num, num2, this._x[8], 9);
			num2 = this.Gg(num2, num3, num4, num, this._x[12], 13);
			num = this.Gg(num, num2, num3, num4, this._x[1], 3);
			num4 = this.Gg(num4, num, num2, num3, this._x[5], 5);
			num3 = this.Gg(num3, num4, num, num2, this._x[9], 9);
			num2 = this.Gg(num2, num3, num4, num, this._x[13], 13);
			num = this.Gg(num, num2, num3, num4, this._x[2], 3);
			num4 = this.Gg(num4, num, num2, num3, this._x[6], 5);
			num3 = this.Gg(num3, num4, num, num2, this._x[10], 9);
			num2 = this.Gg(num2, num3, num4, num, this._x[14], 13);
			num = this.Gg(num, num2, num3, num4, this._x[3], 3);
			num4 = this.Gg(num4, num, num2, num3, this._x[7], 5);
			num3 = this.Gg(num3, num4, num, num2, this._x[11], 9);
			num2 = this.Gg(num2, num3, num4, num, this._x[15], 13);
			num = this.Hh(num, num2, num3, num4, this._x[0], 3);
			num4 = this.Hh(num4, num, num2, num3, this._x[8], 9);
			num3 = this.Hh(num3, num4, num, num2, this._x[4], 11);
			num2 = this.Hh(num2, num3, num4, num, this._x[12], 15);
			num = this.Hh(num, num2, num3, num4, this._x[2], 3);
			num4 = this.Hh(num4, num, num2, num3, this._x[10], 9);
			num3 = this.Hh(num3, num4, num, num2, this._x[6], 11);
			num2 = this.Hh(num2, num3, num4, num, this._x[14], 15);
			num = this.Hh(num, num2, num3, num4, this._x[1], 3);
			num4 = this.Hh(num4, num, num2, num3, this._x[9], 9);
			num3 = this.Hh(num3, num4, num, num2, this._x[5], 11);
			num2 = this.Hh(num2, num3, num4, num, this._x[13], 15);
			num = this.Hh(num, num2, num3, num4, this._x[3], 3);
			num4 = this.Hh(num4, num, num2, num3, this._x[11], 9);
			num3 = this.Hh(num3, num4, num, num2, this._x[7], 11);
			num2 = this.Hh(num2, num3, num4, num, this._x[15], 15);
			this._context[0] += num;
			this._context[1] += num2;
			this._context[2] += num3;
			this._context[3] += num4;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000056F8 File Offset: 0x000038F8
		private int Ff(int a, int b, int c, int d, int x, int s)
		{
			int num = a + ((b & c) | (~b & d)) + x;
			return num << s | (int)((uint)num >> 32 - s);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000572C File Offset: 0x0000392C
		private int Gg(int a, int b, int c, int d, int x, int s)
		{
			int num = a + ((b & (c | d)) | (c & d)) + x + 1518500249;
			return num << s | (int)((uint)num >> 32 - s);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00005768 File Offset: 0x00003968
		private int Hh(int a, int b, int c, int d, int x, int s)
		{
			int num = a + (b ^ c ^ d) + x + 1859775393;
			return num << s | (int)((uint)num >> 32 - s);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000057A0 File Offset: 0x000039A0
		public override byte[] Digest()
		{
			return this.EngineDigest();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000057B8 File Offset: 0x000039B8
		public override int GetDigestLength()
		{
			return this.EngineDigest().Length;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000057D2 File Offset: 0x000039D2
		public override void Reset()
		{
			this.EngineReset();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000057DC File Offset: 0x000039DC
		public override void Update(byte[] b)
		{
			this.EngineUpdate(b, 0, b.Length);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000057EB File Offset: 0x000039EB
		public override void Update(byte b)
		{
			this.EngineUpdate(b);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000057F6 File Offset: 0x000039F6
		public override void Update(byte[] b, int offset, int len)
		{
			this.EngineUpdate(b, offset, len);
		}

		// Token: 0x0400003B RID: 59
		private const int BlockLength = 64;

		// Token: 0x0400003C RID: 60
		private int[] _context = new int[4];

		// Token: 0x0400003D RID: 61
		private long _count;

		// Token: 0x0400003E RID: 62
		private byte[] _buffer = new byte[64];

		// Token: 0x0400003F RID: 63
		private int[] _x = new int[16];
	}
}
