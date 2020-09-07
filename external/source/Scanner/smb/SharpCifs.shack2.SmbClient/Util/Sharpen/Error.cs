using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000026 RID: 38
	public class Error : Exception
	{
		// Token: 0x0600010C RID: 268 RVA: 0x000071F6 File Offset: 0x000053F6
		public Error()
		{
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000720B File Offset: 0x0000540B
		public Error(Exception ex) : base("Runtime Exception", ex)
		{
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00007200 File Offset: 0x00005400
		public Error(string msg) : base(msg)
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000721B File Offset: 0x0000541B
		public Error(string msg, Exception ex) : base(msg, ex)
		{
		}
	}
}
