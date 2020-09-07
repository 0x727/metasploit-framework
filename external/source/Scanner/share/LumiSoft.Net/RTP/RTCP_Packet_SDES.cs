using System;
using System.Collections.Generic;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000DA RID: 218
	public class RTCP_Packet_SDES : RTCP_Packet
	{
		// Token: 0x06000882 RID: 2178 RVA: 0x0003273F File Offset: 0x0003173F
		internal RTCP_Packet_SDES()
		{
			this.m_pChunks = new List<RTCP_Packet_SDES_Chunk>();
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x00032764 File Offset: 0x00031764
		protected override void ParseInternal(byte[] buffer, ref int offset)
		{
			this.m_Version = buffer[offset] >> 6;
			bool flag = Convert.ToBoolean(buffer[offset] >> 5 & 1);
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
			bool flag2 = flag;
			if (flag2)
			{
				base.PaddBytesCount = (int)buffer[offset + num5];
			}
			for (int i = 0; i < num2; i++)
			{
				RTCP_Packet_SDES_Chunk rtcp_Packet_SDES_Chunk = new RTCP_Packet_SDES_Chunk();
				rtcp_Packet_SDES_Chunk.Parse(buffer, ref offset);
				this.m_pChunks.Add(rtcp_Packet_SDES_Chunk);
			}
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0003281C File Offset: 0x0003181C
		public override void ToByte(byte[] buffer, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			buffer[num] = (byte)(128 | (this.m_pChunks.Count & 31));
			num = offset;
			offset = num + 1;
			buffer[num] = 202;
			int num2 = offset;
			num = offset;
			offset = num + 1;
			buffer[num] = 0;
			num = offset;
			offset = num + 1;
			buffer[num] = 0;
			int num3 = offset;
			foreach (RTCP_Packet_SDES_Chunk rtcp_Packet_SDES_Chunk in this.m_pChunks)
			{
				rtcp_Packet_SDES_Chunk.ToByte(buffer, ref offset);
			}
			int num4 = (offset - num3) / 4;
			buffer[num2] = (byte)(num4 >> 8 & 255);
			buffer[num2 + 1] = (byte)(num4 & 255);
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000885 RID: 2181 RVA: 0x000328E8 File Offset: 0x000318E8
		public override int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x00032900 File Offset: 0x00031900
		public override int Type
		{
			get
			{
				return 202;
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000887 RID: 2183 RVA: 0x00032918 File Offset: 0x00031918
		public List<RTCP_Packet_SDES_Chunk> Chunks
		{
			get
			{
				return this.m_pChunks;
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x00032930 File Offset: 0x00031930
		public override int Size
		{
			get
			{
				int num = 4;
				foreach (RTCP_Packet_SDES_Chunk rtcp_Packet_SDES_Chunk in this.m_pChunks)
				{
					num += rtcp_Packet_SDES_Chunk.Size;
				}
				return num;
			}
		}

		// Token: 0x040003BB RID: 955
		private int m_Version = 2;

		// Token: 0x040003BC RID: 956
		private List<RTCP_Packet_SDES_Chunk> m_pChunks = null;
	}
}
