using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C3 RID: 195
	public class RTCP_Report_Sender
	{
		// Token: 0x06000772 RID: 1906 RVA: 0x0002D550 File Offset: 0x0002C550
		internal RTCP_Report_Sender(RTCP_Packet_SR sr)
		{
			bool flag = sr == null;
			if (flag)
			{
				throw new ArgumentNullException("sr");
			}
			this.m_NtpTimestamp = sr.NtpTimestamp;
			this.m_RtpTimestamp = sr.RtpTimestamp;
			this.m_SenderPacketCount = sr.SenderPacketCount;
			this.m_SenderOctetCount = sr.SenderOctetCount;
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x0002D5C8 File Offset: 0x0002C5C8
		public ulong NtpTimestamp
		{
			get
			{
				return this.m_NtpTimestamp;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x0002D5E0 File Offset: 0x0002C5E0
		public uint RtpTimestamp
		{
			get
			{
				return this.m_RtpTimestamp;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x0002D5F8 File Offset: 0x0002C5F8
		public uint SenderPacketCount
		{
			get
			{
				return this.m_SenderPacketCount;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x0002D610 File Offset: 0x0002C610
		public uint SenderOctetCount
		{
			get
			{
				return this.m_SenderOctetCount;
			}
		}

		// Token: 0x04000336 RID: 822
		private ulong m_NtpTimestamp = 0UL;

		// Token: 0x04000337 RID: 823
		private uint m_RtpTimestamp = 0U;

		// Token: 0x04000338 RID: 824
		private uint m_SenderPacketCount = 0U;

		// Token: 0x04000339 RID: 825
		private uint m_SenderOctetCount = 0U;
	}
}
