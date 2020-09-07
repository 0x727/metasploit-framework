using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000CF RID: 207
	public class RTP_SendStreamEventArgs : EventArgs
	{
		// Token: 0x06000807 RID: 2055 RVA: 0x0002FEE8 File Offset: 0x0002EEE8
		public RTP_SendStreamEventArgs(RTP_SendStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x0002FF20 File Offset: 0x0002EF20
		public RTP_SendStream Stream
		{
			get
			{
				return this.m_pStream;
			}
		}

		// Token: 0x04000387 RID: 903
		private RTP_SendStream m_pStream = null;
	}
}
