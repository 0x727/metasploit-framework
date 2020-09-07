using System;

namespace LumiSoft.Net
{
	// Token: 0x02000008 RID: 8
	public interface IAsyncOP
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000016 RID: 22
		AsyncOP_State State { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000017 RID: 23
		Exception Error { get; }
	}
}
