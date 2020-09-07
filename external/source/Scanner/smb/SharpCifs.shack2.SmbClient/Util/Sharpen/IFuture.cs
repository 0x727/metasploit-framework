using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000049 RID: 73
	internal interface IFuture<T>
	{
		// Token: 0x060001CA RID: 458
		bool Cancel(bool mayInterruptIfRunning);

		// Token: 0x060001CB RID: 459
		T Get();
	}
}
