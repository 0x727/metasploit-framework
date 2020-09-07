using System;
using System.IO;
using System.Text;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200005A RID: 90
	internal class OutputStreamWriter : StreamWriter
	{
		// Token: 0x0600023B RID: 571 RVA: 0x0000A249 File Offset: 0x00008449
		public OutputStreamWriter(OutputStream stream) : base(stream.GetWrappedStream())
		{
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000A259 File Offset: 0x00008459
		public OutputStreamWriter(OutputStream stream, string encoding) : base(stream.GetWrappedStream(), Extensions.GetEncoding(encoding))
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000A26F File Offset: 0x0000846F
		public OutputStreamWriter(OutputStream stream, Encoding encoding) : base(stream.GetWrappedStream(), encoding)
		{
		}
	}
}
