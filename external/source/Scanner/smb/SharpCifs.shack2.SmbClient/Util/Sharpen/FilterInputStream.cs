using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000041 RID: 65
	public class FilterInputStream : InputStream
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x00008B33 File Offset: 0x00006D33
		public FilterInputStream(InputStream s)
		{
			this.In = s;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00008B44 File Offset: 0x00006D44
		public override int Available()
		{
			return this.In.Available();
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00008B61 File Offset: 0x00006D61
		public override void Close()
		{
			this.In.Close();
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00008B70 File Offset: 0x00006D70
		public override void Mark(int readlimit)
		{
			this.In.Mark(readlimit);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00008B80 File Offset: 0x00006D80
		public override bool MarkSupported()
		{
			return this.In.MarkSupported();
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00008BA0 File Offset: 0x00006DA0
		public override int Read()
		{
			return this.In.Read();
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00008BC0 File Offset: 0x00006DC0
		public override int Read(byte[] buf)
		{
			return this.In.Read(buf);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00008BE0 File Offset: 0x00006DE0
		public override int Read(byte[] b, int off, int len)
		{
			return this.In.Read(b, off, len);
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00008C00 File Offset: 0x00006E00
		public override void Reset()
		{
			this.In.Reset();
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00008C10 File Offset: 0x00006E10
		public override long Skip(long cnt)
		{
			return this.In.Skip(cnt);
		}

		// Token: 0x0400005B RID: 91
		protected InputStream In;
	}
}
