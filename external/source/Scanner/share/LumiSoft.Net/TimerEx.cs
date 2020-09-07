using System;
using System.Timers;

namespace LumiSoft.Net
{
	// Token: 0x02000015 RID: 21
	public class TimerEx : Timer
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00004238 File Offset: 0x00003238
		public TimerEx()
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004242 File Offset: 0x00003242
		public TimerEx(double interval) : base(interval)
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000424D File Offset: 0x0000324D
		public TimerEx(double interval, bool autoReset) : base(interval)
		{
			base.AutoReset = autoReset;
		}
	}
}
