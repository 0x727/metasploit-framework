using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000DB RID: 219
	public class RTCP_Packet_ReportBlock
	{
		// Token: 0x06000889 RID: 2185 RVA: 0x00032994 File Offset: 0x00031994
		internal RTCP_Packet_ReportBlock(uint ssrc)
		{
			this.m_SSRC = ssrc;
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x000329E1 File Offset: 0x000319E1
		internal RTCP_Packet_ReportBlock()
		{
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00032A1C File Offset: 0x00031A1C
		public void Parse(byte[] buffer, int offset)
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
			this.m_SSRC = (uint)((int)buffer[offset++] << 24 | (int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
			this.m_FractionLost = (uint)buffer[offset++];
			this.m_CumulativePacketsLost = (uint)((int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
			this.m_ExtHighestSeqNumber = (uint)((int)buffer[offset++] << 24 | (int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
			this.m_Jitter = (uint)((int)buffer[offset++] << 24 | (int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
			this.m_LastSR = (uint)((int)buffer[offset++] << 24 | (int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
			this.m_DelaySinceLastSR = (uint)((int)buffer[offset++] << 24 | (int)buffer[offset++] << 16 | (int)buffer[offset++] << 8 | (int)buffer[offset++]);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x00032B7C File Offset: 0x00031B7C
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
				throw new ArgumentException("Argument 'offset' must be >= 0.");
			}
			bool flag3 = offset + 24 > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Argument 'buffer' has not enough room to store report block.");
			}
			int num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_SSRC >> 24 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_SSRC >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_SSRC >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_SSRC & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)this.m_FractionLost;
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_CumulativePacketsLost >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_CumulativePacketsLost >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_CumulativePacketsLost & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_ExtHighestSeqNumber >> 24 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_ExtHighestSeqNumber >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_ExtHighestSeqNumber >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_ExtHighestSeqNumber & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_Jitter >> 24 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_Jitter >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_Jitter >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_Jitter & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_LastSR >> 24 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_LastSR >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_LastSR >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_LastSR & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_DelaySinceLastSR >> 24 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_DelaySinceLastSR >> 16 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_DelaySinceLastSR >> 8 & 255U);
			num = offset;
			offset = num + 1;
			buffer[num] = (byte)(this.m_DelaySinceLastSR & 255U);
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600088D RID: 2189 RVA: 0x00032E34 File Offset: 0x00031E34
		public uint SSRC
		{
			get
			{
				return this.m_SSRC;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x00032E4C File Offset: 0x00031E4C
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x00032E64 File Offset: 0x00031E64
		public uint FractionLost
		{
			get
			{
				return this.m_FractionLost;
			}
			set
			{
				this.m_FractionLost = value;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000890 RID: 2192 RVA: 0x00032E70 File Offset: 0x00031E70
		// (set) Token: 0x06000891 RID: 2193 RVA: 0x00032E88 File Offset: 0x00031E88
		public uint CumulativePacketsLost
		{
			get
			{
				return this.m_CumulativePacketsLost;
			}
			set
			{
				this.m_CumulativePacketsLost = value;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000892 RID: 2194 RVA: 0x00032E94 File Offset: 0x00031E94
		// (set) Token: 0x06000893 RID: 2195 RVA: 0x00032EAC File Offset: 0x00031EAC
		public uint ExtendedHighestSeqNo
		{
			get
			{
				return this.m_ExtHighestSeqNumber;
			}
			set
			{
				this.m_ExtHighestSeqNumber = value;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000894 RID: 2196 RVA: 0x00032EB8 File Offset: 0x00031EB8
		// (set) Token: 0x06000895 RID: 2197 RVA: 0x00032ED0 File Offset: 0x00031ED0
		public uint Jitter
		{
			get
			{
				return this.m_Jitter;
			}
			set
			{
				this.m_Jitter = value;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000896 RID: 2198 RVA: 0x00032EDC File Offset: 0x00031EDC
		// (set) Token: 0x06000897 RID: 2199 RVA: 0x00032EF4 File Offset: 0x00031EF4
		public uint LastSR
		{
			get
			{
				return this.m_LastSR;
			}
			set
			{
				this.m_LastSR = value;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x00032F00 File Offset: 0x00031F00
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x00032F18 File Offset: 0x00031F18
		public uint DelaySinceLastSR
		{
			get
			{
				return this.m_DelaySinceLastSR;
			}
			set
			{
				this.m_DelaySinceLastSR = value;
			}
		}

		// Token: 0x040003BD RID: 957
		private uint m_SSRC = 0U;

		// Token: 0x040003BE RID: 958
		private uint m_FractionLost = 0U;

		// Token: 0x040003BF RID: 959
		private uint m_CumulativePacketsLost = 0U;

		// Token: 0x040003C0 RID: 960
		private uint m_ExtHighestSeqNumber = 0U;

		// Token: 0x040003C1 RID: 961
		private uint m_Jitter = 0U;

		// Token: 0x040003C2 RID: 962
		private uint m_LastSR = 0U;

		// Token: 0x040003C3 RID: 963
		private uint m_DelaySinceLastSR = 0U;
	}
}
