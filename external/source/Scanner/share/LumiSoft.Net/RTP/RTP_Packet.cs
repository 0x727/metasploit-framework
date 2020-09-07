using System;
using System.Text;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000DC RID: 220
	public class RTP_Packet
	{
		// Token: 0x0600089B RID: 2203 RVA: 0x00032F74 File Offset: 0x00031F74
		public static RTP_Packet Parse(byte[] buffer, int size)
		{
			RTP_Packet rtp_Packet = new RTP_Packet();
			rtp_Packet.ParseInternal(buffer, size);
			return rtp_Packet;
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x000091B8 File Offset: 0x000081B8
		public void Validate()
		{
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x00032F98 File Offset: 0x00031F98
		public void ToByte(byte[] buffer, ref int offset)
		{
			int num = 0;
			bool flag = this.m_CSRC != null;
			if (flag)
			{
				num = this.m_CSRC.Length;
			}
			int num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Version << 6 | 0 | (num & 15));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(Convert.ToInt32(this.m_IsMarker) << 7 | (this.m_PayloadType & 127));
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SequenceNumber >> 8);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_SequenceNumber & 255);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Timestamp >> 24 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Timestamp >> 16 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Timestamp >> 8 & 255U);
			num2 = offset;
			offset = num2 + 1;
			buffer[num2] = (byte)(this.m_Timestamp & 255U);
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
			bool flag2 = this.m_CSRC != null;
			if (flag2)
			{
				foreach (int num3 in this.m_CSRC)
				{
					num2 = offset;
					offset = num2 + 1;
					buffer[num2] = (byte)(num3 >> 24 & 255);
					num2 = offset;
					offset = num2 + 1;
					buffer[num2] = (byte)(num3 >> 16 & 255);
					num2 = offset;
					offset = num2 + 1;
					buffer[num2] = (byte)(num3 >> 8 & 255);
					num2 = offset;
					offset = num2 + 1;
					buffer[num2] = (byte)(num3 & 255);
				}
			}
			Array.Copy(this.m_Data, 0, buffer, offset, this.m_Data.Length);
			offset += this.m_Data.Length;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x000331AC File Offset: 0x000321AC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("----- RTP Packet\r\n");
			stringBuilder.Append("Version: " + this.m_Version.ToString() + "\r\n");
			stringBuilder.Append("IsMaker: " + this.m_IsMarker.ToString() + "\r\n");
			stringBuilder.Append("PayloadType: " + this.m_PayloadType.ToString() + "\r\n");
			stringBuilder.Append("SeqNo: " + this.m_SequenceNumber.ToString() + "\r\n");
			stringBuilder.Append("Timestamp: " + this.m_Timestamp.ToString() + "\r\n");
			stringBuilder.Append("SSRC: " + this.m_SSRC.ToString() + "\r\n");
			stringBuilder.Append("Data: " + this.m_Data.Length + " bytes.\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x000332C0 File Offset: 0x000322C0
		private void ParseInternal(byte[] buffer, int size)
		{
			int num = 0;
			this.m_Version = buffer[num] >> 6;
			bool flag = Convert.ToBoolean(buffer[num] >> 5 & 1);
			bool flag2 = Convert.ToBoolean(buffer[num] >> 4 & 1);
			int num2 = (int)(buffer[num++] & 15);
			this.m_IsMarker = Convert.ToBoolean(buffer[num] >> 7);
			this.m_PayloadType = (int)(buffer[num++] & 127);
			this.m_SequenceNumber = (ushort)((int)buffer[num++] << 8 | (int)buffer[num++]);
			this.m_Timestamp = (uint)((int)buffer[num++] << 24 | (int)buffer[num++] << 16 | (int)buffer[num++] << 8 | (int)buffer[num++]);
			this.m_SSRC = (uint)((int)buffer[num++] << 24 | (int)buffer[num++] << 16 | (int)buffer[num++] << 8 | (int)buffer[num++]);
			this.m_CSRC = new uint[num2];
			for (int i = 0; i < num2; i++)
			{
				this.m_CSRC[i] = (uint)((int)buffer[num++] << 24 | (int)buffer[num++] << 16 | (int)buffer[num++] << 8 | (int)buffer[num++]);
			}
			bool flag3 = flag2;
			if (flag3)
			{
				num++;
				num += (int)buffer[num];
			}
			this.m_Data = new byte[size - num];
			Array.Copy(buffer, num, this.m_Data, 0, this.m_Data.Length);
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x060008A0 RID: 2208 RVA: 0x00033420 File Offset: 0x00032420
		public int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x00033438 File Offset: 0x00032438
		public bool IsPadded
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x060008A2 RID: 2210 RVA: 0x0003344C File Offset: 0x0003244C
		// (set) Token: 0x060008A3 RID: 2211 RVA: 0x00033464 File Offset: 0x00032464
		public bool IsMarker
		{
			get
			{
				return this.m_IsMarker;
			}
			set
			{
				this.m_IsMarker = value;
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060008A4 RID: 2212 RVA: 0x00033470 File Offset: 0x00032470
		// (set) Token: 0x060008A5 RID: 2213 RVA: 0x00033488 File Offset: 0x00032488
		public int PayloadType
		{
			get
			{
				return this.m_PayloadType;
			}
			set
			{
				bool flag = value < 0 || value > 128;
				if (flag)
				{
					throw new ArgumentException("Payload value must be >= 0 and <= 128.");
				}
				this.m_PayloadType = value;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060008A6 RID: 2214 RVA: 0x000334BC File Offset: 0x000324BC
		// (set) Token: 0x060008A7 RID: 2215 RVA: 0x000334D4 File Offset: 0x000324D4
		public ushort SeqNo
		{
			get
			{
				return this.m_SequenceNumber;
			}
			set
			{
				this.m_SequenceNumber = value;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x000334E0 File Offset: 0x000324E0
		// (set) Token: 0x060008A9 RID: 2217 RVA: 0x000334F8 File Offset: 0x000324F8
		public uint Timestamp
		{
			get
			{
				return this.m_Timestamp;
			}
			set
			{
				bool flag = value < 1U;
				if (flag)
				{
					throw new ArgumentException("Timestamp value must be >= 1.");
				}
				this.m_Timestamp = value;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x00033524 File Offset: 0x00032524
		// (set) Token: 0x060008AB RID: 2219 RVA: 0x0003353C File Offset: 0x0003253C
		public uint SSRC
		{
			get
			{
				return this.m_SSRC;
			}
			set
			{
				bool flag = value < 1U;
				if (flag)
				{
					throw new ArgumentException("SSRC value must be >= 1.");
				}
				this.m_SSRC = value;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x00033568 File Offset: 0x00032568
		// (set) Token: 0x060008AD RID: 2221 RVA: 0x00033580 File Offset: 0x00032580
		public uint[] CSRC
		{
			get
			{
				return this.m_CSRC;
			}
			set
			{
				this.m_CSRC = value;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x0003358C File Offset: 0x0003258C
		public uint[] Sources
		{
			get
			{
				uint[] array = new uint[1];
				bool flag = this.m_CSRC != null;
				if (flag)
				{
					array = new uint[1 + this.m_CSRC.Length];
				}
				array[0] = this.m_SSRC;
				Array.Copy(this.m_CSRC, array, this.m_CSRC.Length);
				return array;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060008AF RID: 2223 RVA: 0x000335E4 File Offset: 0x000325E4
		// (set) Token: 0x060008B0 RID: 2224 RVA: 0x000335FC File Offset: 0x000325FC
		public byte[] Data
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Data");
				}
				this.m_Data = value;
			}
		}

		// Token: 0x040003C4 RID: 964
		private int m_Version = 2;

		// Token: 0x040003C5 RID: 965
		private bool m_IsMarker = false;

		// Token: 0x040003C6 RID: 966
		private int m_PayloadType = 0;

		// Token: 0x040003C7 RID: 967
		private ushort m_SequenceNumber = 0;

		// Token: 0x040003C8 RID: 968
		private uint m_Timestamp = 0U;

		// Token: 0x040003C9 RID: 969
		private uint m_SSRC = 0U;

		// Token: 0x040003CA RID: 970
		private uint[] m_CSRC = null;

		// Token: 0x040003CB RID: 971
		private byte[] m_Data = null;
	}
}
