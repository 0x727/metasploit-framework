using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Util
{
	// Token: 0x0200000B RID: 11
	public class Hmact64 : MessageDigest
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00004CA0 File Offset: 0x00002EA0
		public Hmact64(byte[] key)
		{
			int num = Math.Min(key.Length, 64);
			for (int i = 0; i < num; i++)
			{
				this._ipad[i] = (byte)(key[i] ^ 54);
				this._opad[i] = (byte)(key[i] ^ 92);
			}
			for (int j = num; j < 64; j++)
			{
				this._ipad[j] = 54;
				this._opad[j] = 92;
			}
			try
			{
				this._md5 = MessageDigest.GetInstance("MD5");
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
			this.EngineReset();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004D70 File Offset: 0x00002F70
		protected byte[] EngineDigest()
		{
			byte[] buffer = this._md5.Digest();
			this._md5.Update(this._opad);
			return this._md5.Digest(buffer);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004DAC File Offset: 0x00002FAC
		protected int EngineDigest(byte[] buf, int offset, int len)
		{
			byte[] b = this._md5.Digest();
			this._md5.Update(this._opad);
			this._md5.Update(b);
			try
			{
				this._md5.Digest(buf, offset, len);
			}
			catch (Exception)
			{
				throw new InvalidOperationException();
			}
			return len;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004E14 File Offset: 0x00003014
		protected int EngineGetDigestLength()
		{
			return this._md5.GetDigestLength();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004E31 File Offset: 0x00003031
		protected void EngineReset()
		{
			this._md5.Reset();
			this._md5.Update(this._ipad);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004E52 File Offset: 0x00003052
		protected void EngineUpdate(byte b)
		{
			this._md5.Update(b);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004E62 File Offset: 0x00003062
		protected void EngineUpdate(byte[] input, int offset, int len)
		{
			this._md5.Update(input, offset, len);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004E74 File Offset: 0x00003074
		public override byte[] Digest()
		{
			return this.EngineDigest();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004E8C File Offset: 0x0000308C
		public override int GetDigestLength()
		{
			return this.EngineGetDigestLength();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004EA4 File Offset: 0x000030A4
		public override void Reset()
		{
			this.EngineReset();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004EAE File Offset: 0x000030AE
		public override void Update(byte[] b)
		{
			this.EngineUpdate(b, 0, b.Length);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004EBD File Offset: 0x000030BD
		public override void Update(byte b)
		{
			this.EngineUpdate(b);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004EC8 File Offset: 0x000030C8
		public override void Update(byte[] b, int offset, int len)
		{
			this.EngineUpdate(b, offset, len);
		}

		// Token: 0x04000033 RID: 51
		private const int BlockLength = 64;

		// Token: 0x04000034 RID: 52
		private const byte Ipad = 54;

		// Token: 0x04000035 RID: 53
		private const byte Opad = 92;

		// Token: 0x04000036 RID: 54
		private MessageDigest _md5;

		// Token: 0x04000037 RID: 55
		private byte[] _ipad = new byte[64];

		// Token: 0x04000038 RID: 56
		private byte[] _opad = new byte[64];
	}
}
