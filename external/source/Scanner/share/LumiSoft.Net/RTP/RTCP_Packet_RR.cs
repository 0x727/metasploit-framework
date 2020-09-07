using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D7 RID: 215
	public class RTCP_Packet_RR : RTCP_Packet
	{
		// Token: 0x06000853 RID: 2131 RVA: 0x00031219 File Offset: 0x00030219
		internal RTCP_Packet_RR()
		{
			this.m_pReportBlocks = new List<RTCP_Packet_ReportBlock>();
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00031219 File Offset: 0x00030219
		internal RTCP_Packet_RR(uint ssrc)
		{
			this.m_pReportBlocks = new List<RTCP_Packet_ReportBlock>();
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00031244 File Offset: 0x00030244
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
			for (int i = 0; i < num2; i++)
			{
				RTCP_Packet_ReportBlock rtcp_Packet_ReportBlock = new RTCP_Packet_ReportBlock();
				rtcp_Packet_ReportBlock.Parse(buffer, offset);
				this.m_pReportBlocks.Add(rtcp_Packet_ReportBlock);
				offset += 24;
			}
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x00031378 File Offset: 0x00030378
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
			int num = (4 + this.m_pReportBlocks.Count * 24) / 4;
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(128 | (this.m_pReportBlocks.Count & 31));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = 201;
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
			foreach (RTCP_Packet_ReportBlock rtcp_Packet_ReportBlock in this.m_pReportBlocks)
			{
				rtcp_Packet_ReportBlock.ToByte(buffer, ref offset);
			}
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x000314D4 File Offset: 0x000304D4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Type: RR");
			stringBuilder.AppendLine("Version: " + this.m_Version);
			stringBuilder.AppendLine("SSRC: " + this.m_SSRC);
			stringBuilder.AppendLine("Report blocks: " + this.m_pReportBlocks.Count.ToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000858 RID: 2136 RVA: 0x0003155C File Offset: 0x0003055C
		public override int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x00031574 File Offset: 0x00030574
		public override int Type
		{
			get
			{
				return 201;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x0600085A RID: 2138 RVA: 0x0003158C File Offset: 0x0003058C
		// (set) Token: 0x0600085B RID: 2139 RVA: 0x000315A4 File Offset: 0x000305A4
		public uint SSRC
		{
			get
			{
				return this.m_SSRC;
			}
			set
			{
				this.m_SSRC = value;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x000315B0 File Offset: 0x000305B0
		public List<RTCP_Packet_ReportBlock> ReportBlocks
		{
			get
			{
				return this.m_pReportBlocks;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x000315C8 File Offset: 0x000305C8
		public override int Size
		{
			get
			{
				return 8 + 24 * this.m_pReportBlocks.Count;
			}
		}

		// Token: 0x040003A9 RID: 937
		private int m_Version = 2;

		// Token: 0x040003AA RID: 938
		private uint m_SSRC = 0U;

		// Token: 0x040003AB RID: 939
		private List<RTCP_Packet_ReportBlock> m_pReportBlocks = null;
	}
}
