using System;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000D6 RID: 214
	public abstract class RTCP_Packet
	{
		// Token: 0x06000848 RID: 2120 RVA: 0x00031020 File Offset: 0x00030020
		public RTCP_Packet()
		{
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x00031034 File Offset: 0x00030034
		public static RTCP_Packet Parse(byte[] buffer, ref int offset)
		{
			return RTCP_Packet.Parse(buffer, ref offset, false);
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x00031050 File Offset: 0x00030050
		public static RTCP_Packet Parse(byte[] buffer, ref int offset, bool noException)
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
			int num = (int)buffer[offset + 1];
			bool flag3 = num == 200;
			RTCP_Packet result;
			if (flag3)
			{
				RTCP_Packet_SR rtcp_Packet_SR = new RTCP_Packet_SR();
				rtcp_Packet_SR.ParseInternal(buffer, ref offset);
				result = rtcp_Packet_SR;
			}
			else
			{
				bool flag4 = num == 201;
				if (flag4)
				{
					RTCP_Packet_RR rtcp_Packet_RR = new RTCP_Packet_RR();
					rtcp_Packet_RR.ParseInternal(buffer, ref offset);
					result = rtcp_Packet_RR;
				}
				else
				{
					bool flag5 = num == 202;
					if (flag5)
					{
						RTCP_Packet_SDES rtcp_Packet_SDES = new RTCP_Packet_SDES();
						rtcp_Packet_SDES.ParseInternal(buffer, ref offset);
						result = rtcp_Packet_SDES;
					}
					else
					{
						bool flag6 = num == 203;
						if (flag6)
						{
							RTCP_Packet_BYE rtcp_Packet_BYE = new RTCP_Packet_BYE();
							rtcp_Packet_BYE.ParseInternal(buffer, ref offset);
							result = rtcp_Packet_BYE;
						}
						else
						{
							bool flag7 = num == 204;
							if (flag7)
							{
								RTCP_Packet_APP rtcp_Packet_APP = new RTCP_Packet_APP();
								rtcp_Packet_APP.ParseInternal(buffer, ref offset);
								result = rtcp_Packet_APP;
							}
							else
							{
								offset += 2;
								int num2 = offset;
								offset = num2 + 1;
								int num3 = (int)buffer[num2] << 8;
								num2 = offset;
								offset = num2 + 1;
								int num4 = num3 | (int)buffer[num2];
								offset += num4;
								if (!noException)
								{
									throw new ArgumentException("Unknown RTCP packet type '" + num + "'.");
								}
								result = null;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600084B RID: 2123
		public abstract void ToByte(byte[] buffer, ref int offset);

		// Token: 0x0600084C RID: 2124
		protected abstract void ParseInternal(byte[] buffer, ref int offset);

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x0600084D RID: 2125
		public abstract int Version { get; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600084E RID: 2126 RVA: 0x000311B0 File Offset: 0x000301B0
		public bool IsPadded
		{
			get
			{
				return this.m_PaddBytesCount > 0;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x0600084F RID: 2127
		public abstract int Type { get; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000850 RID: 2128 RVA: 0x000311D8 File Offset: 0x000301D8
		// (set) Token: 0x06000851 RID: 2129 RVA: 0x000311F0 File Offset: 0x000301F0
		public int PaddBytesCount
		{
			get
			{
				return this.m_PaddBytesCount;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'PaddBytesCount' value must be >= 0.");
				}
				this.m_PaddBytesCount = value;
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000852 RID: 2130
		public abstract int Size { get; }

		// Token: 0x040003A8 RID: 936
		private int m_PaddBytesCount = 0;
	}
}
