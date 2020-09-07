using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C5 RID: 197
	public class RTP_Clock
	{
		// Token: 0x06000782 RID: 1922 RVA: 0x0002DA24 File Offset: 0x0002CA24
		public RTP_Clock(int baseValue, int rate)
		{
			bool flag = rate < 1;
			if (flag)
			{
				throw new ArgumentException("Argument 'rate' value must be between 1 and 100 000.", "rate");
			}
			this.m_BaseValue = baseValue;
			this.m_Rate = rate;
			this.m_CreateTime = DateTime.Now;
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0002DA7C File Offset: 0x0002CA7C
		public int MillisecondsToRtpTicks(int milliseconds)
		{
			return this.m_Rate * milliseconds / 1000;
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x0002DA9C File Offset: 0x0002CA9C
		public int BaseValue
		{
			get
			{
				return this.m_BaseValue;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x0002DAB4 File Offset: 0x0002CAB4
		public int Rate
		{
			get
			{
				return this.m_Rate;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x0002DACC File Offset: 0x0002CACC
		public uint RtpTimestamp
		{
			get
			{
				long num = (long)(DateTime.Now - this.m_CreateTime).TotalMilliseconds;
				return (uint)((long)this.m_BaseValue + (long)this.m_Rate * num / 1000L);
			}
		}

		// Token: 0x04000340 RID: 832
		private int m_BaseValue = 0;

		// Token: 0x04000341 RID: 833
		private int m_Rate = 1;

		// Token: 0x04000342 RID: 834
		private DateTime m_CreateTime;
	}
}
