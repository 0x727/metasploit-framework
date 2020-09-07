using System;
using System.Collections.Generic;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D2 RID: 210
	public class RTCP_CompoundPacket
	{
		// Token: 0x0600082B RID: 2091 RVA: 0x00030527 File Offset: 0x0002F527
		internal RTCP_CompoundPacket()
		{
			this.m_pPackets = new List<RTCP_Packet>();
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00030544 File Offset: 0x0002F544
		public static RTCP_CompoundPacket Parse(byte[] buffer, int count)
		{
			int i = 0;
			RTCP_CompoundPacket rtcp_CompoundPacket = new RTCP_CompoundPacket();
			while (i < count)
			{
				RTCP_Packet rtcp_Packet = RTCP_Packet.Parse(buffer, ref i, true);
				bool flag = rtcp_Packet != null;
				if (flag)
				{
					rtcp_CompoundPacket.m_pPackets.Add(rtcp_Packet);
				}
			}
			return rtcp_CompoundPacket;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00030590 File Offset: 0x0002F590
		public byte[] ToByte()
		{
			byte[] array = new byte[this.TotalSize];
			int num = 0;
			this.ToByte(array, ref num);
			return array;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x000305BC File Offset: 0x0002F5BC
		public void ToByte(byte[] buffer, ref int offset)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'offset' value must be >= 0.");
			}
			foreach (RTCP_Packet rtcp_Packet in this.m_pPackets)
			{
				rtcp_Packet.ToByte(buffer, ref offset);
			}
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x00030640 File Offset: 0x0002F640
		public void Validate()
		{
			bool flag = this.m_pPackets.Count == 0;
			if (flag)
			{
				throw new ArgumentException("No RTCP packets.");
			}
			for (int i = 0; i < this.m_pPackets.Count; i++)
			{
				RTCP_Packet rtcp_Packet = this.m_pPackets[i];
				bool flag2 = rtcp_Packet.Version != 2;
				if (flag2)
				{
					throw new ArgumentException("RTP version field must equal 2.");
				}
				bool flag3 = i < this.m_pPackets.Count - 1 && rtcp_Packet.IsPadded;
				if (flag3)
				{
					throw new ArgumentException("Only the last packet in RTCP compound packet may be padded.");
				}
			}
			bool flag4 = this.m_pPackets[0].Type != 200 || this.m_pPackets[0].Type != 201;
			if (flag4)
			{
				throw new ArgumentException("The first RTCP packet in a compound packet must be equal to SR or RR.");
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x00030728 File Offset: 0x0002F728
		public List<RTCP_Packet> Packets
		{
			get
			{
				return this.m_pPackets;
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000831 RID: 2097 RVA: 0x00030740 File Offset: 0x0002F740
		internal int TotalSize
		{
			get
			{
				int num = 0;
				foreach (RTCP_Packet rtcp_Packet in this.m_pPackets)
				{
					num += rtcp_Packet.Size;
				}
				return num;
			}
		}

		// Token: 0x0400039A RID: 922
		private List<RTCP_Packet> m_pPackets = null;
	}
}
