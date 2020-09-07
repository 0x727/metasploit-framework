using System;

namespace SharpCifs.Util
{
	// Token: 0x0200000E RID: 14
	public class Rc4
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00003195 File Offset: 0x00001395
		public Rc4()
		{
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005803 File Offset: 0x00003A03
		public Rc4(byte[] key)
		{
			this.Init(key, 0, key.Length);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000581C File Offset: 0x00003A1C
		public virtual void Init(byte[] key, int ki, int klen)
		{
			this.S = new byte[256];
			this.I = 0;
			while (this.I < 256)
			{
				this.S[this.I] = (byte)this.I;
				this.I++;
			}
			this.I = (this.J = 0);
			while (this.I < 256)
			{
				this.J = (this.J + (int)key[ki + this.I % klen] + (int)this.S[this.I] & 255);
				byte b = this.S[this.I];
				this.S[this.I] = this.S[this.J];
				this.S[this.J] = b;
				this.I++;
			}
			this.I = (this.J = 0);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005924 File Offset: 0x00003B24
		public virtual void Update(byte[] src, int soff, int slen, byte[] dst, int doff)
		{
			int num = soff + slen;
			while (soff < num)
			{
				this.I = (this.I + 1 & 255);
				this.J = (this.J + (int)this.S[this.I] & 255);
				byte b = this.S[this.I];
				this.S[this.I] = this.S[this.J];
				this.S[this.J] = b;
				dst[doff++] = (byte)(src[soff++] ^ this.S[(int)(this.S[this.I] + this.S[this.J] & byte.MaxValue)]);
			}
		}

		// Token: 0x04000040 RID: 64
		internal byte[] S;

		// Token: 0x04000041 RID: 65
		internal int I;

		// Token: 0x04000042 RID: 66
		internal int J;
	}
}
