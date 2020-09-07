using System;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000060 RID: 96
	internal class ReentrantLock
	{
		// Token: 0x06000293 RID: 659 RVA: 0x0000B033 File Offset: 0x00009233
		public void Lock()
		{
			Monitor.Enter(this);
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000B040 File Offset: 0x00009240
		public bool TryLock()
		{
			return Monitor.TryEnter(this);
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000B058 File Offset: 0x00009258
		public void Unlock()
		{
			Monitor.Exit(this);
		}
	}
}
