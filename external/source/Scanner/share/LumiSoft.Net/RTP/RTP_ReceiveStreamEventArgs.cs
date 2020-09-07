using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CD RID: 205
	public class RTP_ReceiveStreamEventArgs : EventArgs
	{
		// Token: 0x060007ED RID: 2029 RVA: 0x0002F8C4 File Offset: 0x0002E8C4
		public RTP_ReceiveStreamEventArgs(RTP_ReceiveStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060007EE RID: 2030 RVA: 0x0002F8FC File Offset: 0x0002E8FC
		public RTP_ReceiveStream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x0400037A RID: 890
		private RTP_ReceiveStream m_pStream = null;
	}
}
