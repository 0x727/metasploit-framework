using System;
using System.Collections.Generic;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D9 RID: 217
	public class RTCP_Packet_SR : RTCP_Packet
	{
		// Token: 0x06000871 RID: 2161 RVA: 0x00031F6C File Offset: 0x00030F6C
		internal RTCP_Packet_SR(uint ssrc)
		{
			this.m_SSRC = ssrc;
			this.m_pReportBlocks = new List<RTCP_Packet_ReportBlock>();
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x00031FC8 File Offset: 0x00030FC8
		internal RTCP_Packet_SR()
		{
			this.m_pReportBlocks = new List<RTCP_Packet_ReportBlock>();
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0003201C File Offset: 0x0003101C
		protected override void ParseInternal(byte[] buffer, ref int offset)
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
			this.m_Version = buffer[offset] >> 6;
			bool flag3 = Convert.ToBoolean(buffer[offset] >> 5 & 1);
			int num = offset;
			offset = num + 1;
			int num2 = (int)(buffer[num] & 31);
			num = offset;
			offset = num + 1;
			int num3 = (int)buffer[num];
			num = offset;
			offset = num + 1;
			int num4 = (int)buffer[num] << 8;
			num = offset;
			offset = num + 1;
			int num5 = num4 | (int)buffer[num];
			bool flag4 = flag3;
			if (flag4)
			{
				base.PaddBytesCount = (int)buffer[offset + num5];
			}
			num = offset;
			offset = num + 1;
			uint num6 = (uint)((uint)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			uint num7 = num6 | (uint)((uint)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			uint num8 = num7 | (uint)((uint)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_SSRC = (num8 | (uint)buffer[num]);
			num = offset;
			offset = num + 1;
			byte b = (byte)(buffer[num] << 24);
			num = offset;
			offset = num + 1;
			byte b2 = (byte)((int)b | (int)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			byte b3 = (byte)((int)b2 | (int)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			byte b4 = b3 | buffer[num];
			num = offset;
			offset = num + 1;
			byte b5 = (byte)((int)b4 | (int)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			byte b6 = (byte)((int)b5 | (int)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			byte b7 = (byte)((int)b6 | (int)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_NtpTimestamp = (ulong)((long)(b7 | buffer[num]));
			num = offset;
			offset = num + 1;
			uint num9 = (uint)((uint)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			uint num10 = num9 | (uint)((uint)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			uint num11 = num10 | (uint)((uint)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_RtpTimestamp = (num11 | (uint)buffer[num]);
			num = offset;
			offset = num + 1;
			uint num12 = (uint)((uint)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			uint num13 = num12 | (uint)((uint)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			uint num14 = num13 | (uint)((uint)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_SenderPacketCount = (num14 | (uint)buffer[num]);
			num = offset;
			offset = num + 1;
			uint num15 = (uint)((uint)buffer[num] << 24);
			num = offset;
			offset = num + 1;
			uint num16 = num15 | (uint)((uint)buffer[num] << 16);
			num = offset;
			offset = num + 1;
			uint num17 = num16 | (uint)((uint)buffer[num] << 8);
			num = offset;
			offset = num + 1;
			this.m_SenderOctetCount = (num17 | (uint)buffer[num]);
			for (int i = 0; i < num2; i++)
			{
				RTCP_Packet_ReportBlock rtcp_Packet_ReportBlock = new RTCP_Packet_ReportBlock();
				rtcp_Packet_ReportBlock.Parse(buffer, offset);
				this.m_pReportBlocks.Add(rtcp_Packet_ReportBlock);
				offset += 24;
			}
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x000322BC File Offset: 0x000312BC
		public override void ToByte(byte[] buffer, ref int offset)
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
			int num = (24 + this.m_pReportBlocks.Count * 24) / 4;
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(128 | (this.m_pReportBlocks.Count & 31));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = 200;
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num >> 8 & 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(num & 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SSRC >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SSRC >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SSRC >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SSRC & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 56 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 48 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 40 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 32 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 24 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 16 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp >> 8 & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_NtpTimestamp & 255UL);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_RtpTimestamp >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_RtpTimestamp >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_RtpTimestamp >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_RtpTimestamp & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderPacketCount >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderPacketCount >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderPacketCount >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderPacketCount & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderOctetCount >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderOctetCount >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderOctetCount >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SenderOctetCount & 255U);
			foreach (RTCP_Packet_ReportBlock rtcp_Packet_ReportBlock in this.m_pReportBlocks)
			{
				rtcp_Packet_ReportBlock.ToByte(buffer, ref offset);
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x0003262C File Offset: 0x0003162C
		public override int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000876 RID: 2166 RVA: 0x00032644 File Offset: 0x00031644
		public override int Type
		{
			get
			{
				return 200;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000877 RID: 2167 RVA: 0x0003265C File Offset: 0x0003165C
		public uint SSRC
		{
			get
			{
				return this.m_SSRC;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000878 RID: 2168 RVA: 0x00032674 File Offset: 0x00031674
		// (set) Token: 0x06000879 RID: 2169 RVA: 0x0003268C File Offset: 0x0003168C
		public ulong NtpTimestamp
		{
			get
			{
				return this.m_NtpTimestamp;
			}
			set
			{
				this.m_NtpTimestamp = value;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x00032698 File Offset: 0x00031698
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x000326B0 File Offset: 0x000316B0
		public uint RtpTimestamp
		{
			get
			{
				return this.m_RtpTimestamp;
			}
			set
			{
				this.m_RtpTimestamp = value;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x000326BC File Offset: 0x000316BC
		// (set) Token: 0x0600087D RID: 2173 RVA: 0x000326D4 File Offset: 0x000316D4
		public uint SenderPacketCount
		{
			get
			{
				return this.m_SenderPacketCount;
			}
			set
			{
				this.m_SenderPacketCount = value;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x000326E0 File Offset: 0x000316E0
		// (set) Token: 0x0600087F RID: 2175 RVA: 0x000326F8 File Offset: 0x000316F8
		public uint SenderOctetCount
		{
			get
			{
				return this.m_SenderOctetCount;
			}
			set
			{
				this.m_SenderOctetCount = value;
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x00032704 File Offset: 0x00031704
		public List<RTCP_Packet_ReportBlock> ReportBlocks
		{
			get
			{
				return this.m_pReportBlocks;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000881 RID: 2177 RVA: 0x0003271C File Offset: 0x0003171C
		public override int Size
		{
			get
			{
				return 28 + 24 * this.m_pReportBlocks.Count;
			}
		}

		// Token: 0x040003B4 RID: 948
		private int m_Version = 2;

		// Token: 0x040003B5 RID: 949
		private uint m_SSRC = 0U;

		// Token: 0x040003B6 RID: 950
		private ulong m_NtpTimestamp = 0UL;

		// Token: 0x040003B7 RID: 951
		private uint m_RtpTimestamp = 0U;

		// Token: 0x040003B8 RID: 952
		private uint m_SenderPacketCount = 0U;

		// Token: 0x040003B9 RID: 953
		private uint m_SenderOctetCount = 0U;

		// Token: 0x040003BA RID: 954
		private List<RTCP_Packet_ReportBlock> m_pReportBlocks = null;
	}
}
