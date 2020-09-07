using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C2 RID: 194
	public class RTCP_Report_Receiver
	{
		// Token: 0x0600076B RID: 1899 RVA: 0x0002D424 File Offset: 0x0002C424
		internal RTCP_Report_Receiver(RTCP_Packet_ReportBlock rr)
		{
			bool flag = rr == null;
			if (flag)
			{
				throw new ArgumentNullException("rr");
			}
			this.m_FractionLost = rr.FractionLost;
			this.m_CumulativePacketsLost = rr.CumulativePacketsLost;
			this.m_ExtHigestSeqNumber = rr.ExtendedHighestSeqNo;
			this.m_Jitter = rr.Jitter;
			this.m_LastSR = rr.LastSR;
			this.m_DelaySinceLastSR = rr.DelaySinceLastSR;
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x0002D4C0 File Offset: 0x0002C4C0
		public uint FractionLost
		{
			get
			{
				return this.m_FractionLost;
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x0002D4D8 File Offset: 0x0002C4D8
		public uint CumulativePacketsLost
		{
			get
			{
				return this.m_CumulativePacketsLost;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x0002D4F0 File Offset: 0x0002C4F0
		public uint ExtendedSequenceNumber
		{
			get
			{
				return this.m_ExtHigestSeqNumber;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x0002D508 File Offset: 0x0002C508
		public uint Jitter
		{
			get
			{
				return this.m_Jitter;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x0002D520 File Offset: 0x0002C520
		public uint LastSR
		{
			get
			{
				return this.m_LastSR;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x0002D538 File Offset: 0x0002C538
		public uint DelaySinceLastSR
		{
			get
			{
				return this.m_DelaySinceLastSR;
			}
		}

		// Token: 0x04000330 RID: 816
		private uint m_FractionLost = 0U;

		// Token: 0x04000331 RID: 817
		private uint m_CumulativePacketsLost = 0U;

		// Token: 0x04000332 RID: 818
		private uint m_ExtHigestSeqNumber = 0U;

		// Token: 0x04000333 RID: 819
		private uint m_Jitter = 0U;

		// Token: 0x04000334 RID: 820
		private uint m_LastSR = 0U;

		// Token: 0x04000335 RID: 821
		private uint m_DelaySinceLastSR = 0U;
	}
}
