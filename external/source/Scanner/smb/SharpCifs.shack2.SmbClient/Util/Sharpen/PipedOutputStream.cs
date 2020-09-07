using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005C RID: 92
	internal class PipedOutputStream : OutputStream
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0000A8E0 File Offset: 0x00008AE0
		public PipedOutputStream()
		{
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000A8EA File Offset: 0x00008AEA
		public PipedOutputStream(PipedInputStream iss) : this()
		{
			this.Attach(iss);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000A8FC File Offset: 0x00008AFC
		public override void Close()
		{
			this._ips.Close();
			base.Close();
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000A912 File Offset: 0x00008B12
		internal void Attach(PipedInputStream iss)
		{
			this._ips = iss;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000A91C File Offset: 0x00008B1C
		public override void Write(int b)
		{
			this._ips.Write(b);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000A92C File Offset: 0x00008B2C
		public override void Write(byte[] b, int offset, int len)
		{
			this._ips.Write(b, offset, len);
		}

		// Token: 0x04000079 RID: 121
		private PipedInputStream _ips;
	}
}
