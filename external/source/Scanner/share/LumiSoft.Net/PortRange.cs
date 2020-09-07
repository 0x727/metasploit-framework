using System;

namespace LumiSoft.Net
{
	// Token: 0x02000013 RID: 19
	public class PortRange
	{
		// Token: 0x06000062 RID: 98 RVA: 0x0000417C File Offset: 0x0000317C
		public PortRange(int start, int end)
		{
			bool flag = start < 1 || start > 65535;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("Argument 'start' value must be > 0 and << 65 535.");
			}
			bool flag2 = end < 1 || end > 65535;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("Argument 'end' value must be > 0 and << 65 535.");
			}
			bool flag3 = start > end;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("Argumnet 'start' value must be >= argument 'end' value.");
			}
			this.m_Start = start;
			this.m_End = end;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00004208 File Offset: 0x00003208
		public int Start
		{
			get
			{
				return this.m_Start;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00004220 File Offset: 0x00003220
		public int End
		{
			get
			{
				return this.m_End;
			}
		}

		// Token: 0x04000031 RID: 49
		private int m_Start = 1000;

		// Token: 0x04000032 RID: 50
		private int m_End = 1100;
	}
}
