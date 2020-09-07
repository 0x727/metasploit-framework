using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000069 RID: 105
	internal class ThreadFactory
	{
		// Token: 0x060002FD RID: 765 RVA: 0x0000C0A0 File Offset: 0x0000A2A0
		public Thread NewThread(IRunnable r)
		{
			Thread thread = new Thread(r);
			thread.SetDaemon(true);
			thread.Start();
			return thread;
		}
	}
}
