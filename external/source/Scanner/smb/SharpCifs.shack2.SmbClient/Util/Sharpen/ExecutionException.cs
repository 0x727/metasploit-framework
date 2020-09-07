using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000027 RID: 39
	public class ExecutionException : Exception
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00007227 File Offset: 0x00005427
		public ExecutionException(Exception inner) : base("Execution failed", inner)
		{
		}
	}
}
