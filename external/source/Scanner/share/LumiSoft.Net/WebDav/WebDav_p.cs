using System;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x0200003A RID: 58
	public abstract class WebDav_p
	{
		// Token: 0x0600020C RID: 524 RVA: 0x00009954 File Offset: 0x00008954
		public WebDav_p()
		{
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600020D RID: 525
		public abstract string Namespace { get; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600020E RID: 526
		public abstract string Name { get; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600020F RID: 527
		public abstract string Value { get; }
	}
}
