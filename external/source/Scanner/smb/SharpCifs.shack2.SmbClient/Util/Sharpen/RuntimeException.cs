using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000030 RID: 48
	public class RuntimeException : Exception
	{
		// Token: 0x0600011A RID: 282 RVA: 0x000071F6 File Offset: 0x000053F6
		public RuntimeException()
		{
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000720B File Offset: 0x0000540B
		public RuntimeException(Exception ex) : base("Runtime Exception", ex)
		{
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00007200 File Offset: 0x00005400
		public RuntimeException(string msg) : base(msg)
		{
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000721B File Offset: 0x0000541B
		public RuntimeException(string msg, Exception ex) : base(msg, ex)
		{
		}
	}
}
