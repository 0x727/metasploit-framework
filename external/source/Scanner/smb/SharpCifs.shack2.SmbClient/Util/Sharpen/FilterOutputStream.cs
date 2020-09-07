using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000042 RID: 66
	public class FilterOutputStream : OutputStream
	{
		// Token: 0x060001BA RID: 442 RVA: 0x00008C2E File Offset: 0x00006E2E
		public FilterOutputStream(OutputStream os)
		{
			this.Out = os;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00008C3F File Offset: 0x00006E3F
		public override void Close()
		{
			this.Out.Close();
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00008C4E File Offset: 0x00006E4E
		public override void Flush()
		{
			this.Out.Flush();
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00008C5D File Offset: 0x00006E5D
		public override void Write(byte[] b)
		{
			this.Out.Write(b);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00008C6D File Offset: 0x00006E6D
		public override void Write(int b)
		{
			this.Out.Write(b);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00008C7D File Offset: 0x00006E7D
		public override void Write(byte[] b, int offset, int len)
		{
			this.Out.Write(b, offset, len);
		}

		// Token: 0x0400005C RID: 92
		protected OutputStream Out;
	}
}
