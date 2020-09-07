using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000AF RID: 175
	public class SmbNamedPipe : SmbFile
	{
		// Token: 0x06000571 RID: 1393 RVA: 0x0001CABA File Offset: 0x0001ACBA
		public SmbNamedPipe(string url, int pipeType) : base(url)
		{
			this.PipeType = pipeType;
			this.Type = 16;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0001CAD4 File Offset: 0x0001ACD4
		public SmbNamedPipe(string url, int pipeType, NtlmPasswordAuthentication auth) : base(url, auth)
		{
			this.PipeType = pipeType;
			this.Type = 16;
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x0001CAEF File Offset: 0x0001ACEF
		public SmbNamedPipe(Uri url, int pipeType, NtlmPasswordAuthentication auth) : base(url, auth)
		{
			this.PipeType = pipeType;
			this.Type = 16;
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0001CB0C File Offset: 0x0001AD0C
		public virtual InputStream GetNamedPipeInputStream()
		{
			bool flag = this.PipeIn == null;
			if (flag)
			{
				bool flag2 = (this.PipeType & 256) == 256 || (this.PipeType & 512) == 512;
				if (flag2)
				{
					this.PipeIn = new TransactNamedPipeInputStream(this);
				}
				else
				{
					this.PipeIn = new SmbFileInputStream(this, (this.PipeType & -65281) | 32);
				}
			}
			return this.PipeIn;
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x0001CB90 File Offset: 0x0001AD90
		public virtual OutputStream GetNamedPipeOutputStream()
		{
			bool flag = this.PipeOut == null;
			if (flag)
			{
				bool flag2 = (this.PipeType & 256) == 256 || (this.PipeType & 512) == 512;
				if (flag2)
				{
					this.PipeOut = new TransactNamedPipeOutputStream(this);
				}
				else
				{
					this.PipeOut = new SmbFileOutputStream(this, false, (this.PipeType & -65281) | 32);
				}
			}
			return this.PipeOut;
		}

		// Token: 0x04000333 RID: 819
		public const int PipeTypeRdonly = 1;

		// Token: 0x04000334 RID: 820
		public const int PipeTypeWronly = 2;

		// Token: 0x04000335 RID: 821
		public const int PipeTypeRdwr = 3;

		// Token: 0x04000336 RID: 822
		public const int PipeTypeCall = 256;

		// Token: 0x04000337 RID: 823
		public const int PipeTypeTransact = 512;

		// Token: 0x04000338 RID: 824
		public const int PipeTypeDceTransact = 1536;

		// Token: 0x04000339 RID: 825
		internal InputStream PipeIn;

		// Token: 0x0400033A RID: 826
		internal OutputStream PipeOut;

		// Token: 0x0400033B RID: 827
		internal int PipeType;
	}
}
