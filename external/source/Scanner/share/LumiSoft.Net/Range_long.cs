using System;

namespace LumiSoft.Net
{
	// Token: 0x0200000D RID: 13
	public class Range_long
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00002E91 File Offset: 0x00001E91
		public Range_long(long value)
		{
			this.m_Start = value;
			this.m_End = value;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002EB9 File Offset: 0x00001EB9
		public Range_long(long start, long end)
		{
			this.m_Start = start;
			this.m_End = end;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002EE4 File Offset: 0x00001EE4
		public bool Contains(long value)
		{
			return value >= this.m_Start && value <= this.m_End;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002F18 File Offset: 0x00001F18
		public long Start
		{
			get
			{
				return this.m_Start;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002F30 File Offset: 0x00001F30
		public long End
		{
			get
			{
				return this.m_End;
			}
		}

		// Token: 0x04000028 RID: 40
		private long m_Start = 0L;

		// Token: 0x04000029 RID: 41
		private long m_End = 0L;
	}
}
