using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C8 RID: 200
	public class RTP_PacketEventArgs : EventArgs
	{
		// Token: 0x060007AB RID: 1963 RVA: 0x0002E558 File Offset: 0x0002D558
		public RTP_PacketEventArgs(RTP_Packet packet)
		{
			bool flag = packet == null;
			if (flag)
			{
				throw new ArgumentNullException("packet");
			}
			this.m_pPacket = packet;
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x0002E590 File Offset: 0x0002D590
		public RTP_Packet Packet
		{
			get
			{
				return this.m_pPacket;
			}
		}

		// Token: 0x04000351 RID: 849
		private RTP_Packet m_pPacket = null;
	}
}
