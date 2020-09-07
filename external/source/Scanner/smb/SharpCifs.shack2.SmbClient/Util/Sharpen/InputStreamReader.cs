using System;
using System.IO;
using System.Text;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200004B RID: 75
	public class InputStreamReader : StreamReader
	{
		// Token: 0x060001DF RID: 479 RVA: 0x00009120 File Offset: 0x00007320
		protected InputStreamReader(string file) : base(file)
		{
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000912B File Offset: 0x0000732B
		public InputStreamReader(InputStream s) : base(s.GetWrappedStream())
		{
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000913B File Offset: 0x0000733B
		public InputStreamReader(InputStream s, string encoding) : base(s.GetWrappedStream(), Encoding.GetEncoding(encoding))
		{
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00009151 File Offset: 0x00007351
		public InputStreamReader(InputStream s, Encoding e) : base(s.GetWrappedStream(), e)
		{
		}
	}
}
