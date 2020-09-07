using System;
using System.Security.Cryptography;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000054 RID: 84
	public abstract class MessageDigest
	{
		// Token: 0x0600020C RID: 524 RVA: 0x00009E2C File Offset: 0x0000802C
		public void Digest(byte[] buffer, int o, int len)
		{
			byte[] array = this.Digest();
			array.CopyTo(buffer, o);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00009E4C File Offset: 0x0000804C
		public byte[] Digest(byte[] buffer)
		{
			this.Update(buffer);
			return this.Digest();
		}

		// Token: 0x0600020E RID: 526
		public abstract byte[] Digest();

		// Token: 0x0600020F RID: 527
		public abstract int GetDigestLength();

		// Token: 0x06000210 RID: 528 RVA: 0x00009E6C File Offset: 0x0000806C
		public static MessageDigest GetInstance(string algorithm)
		{
			string a = algorithm.ToLower();
			MessageDigest result;
			if (!(a == "sha-1"))
			{
				if (!(a == "md5"))
				{
					throw new NotSupportedException(string.Format("The requested algorithm \"{0}\" is not supported.", algorithm));
				}
				result = new MessageDigest<MD5Managed>();
			}
			else
			{
				result = new MessageDigest<SHA1Managed>();
			}
			return result;
		}

		// Token: 0x06000211 RID: 529
		public abstract void Reset();

		// Token: 0x06000212 RID: 530
		public abstract void Update(byte[] b);

		// Token: 0x06000213 RID: 531
		public abstract void Update(byte b);

		// Token: 0x06000214 RID: 532
		public abstract void Update(byte[] b, int offset, int len);
	}
}
