using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000032 RID: 50
	public class UnknownHostException : Exception
	{
		// Token: 0x0600011F RID: 287 RVA: 0x000071F6 File Offset: 0x000053F6
		public UnknownHostException()
		{
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00007200 File Offset: 0x00005400
		public UnknownHostException(string message) : base(message)
		{
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00007252 File Offset: 0x00005452
		public UnknownHostException(Exception ex) : base("Host not found", ex)
		{
		}
	}
}
