using System;
using System.Net;

namespace LumiSoft.Net.RTP
{
	// Token: 0x020000C4 RID: 196
	public class RTP_Address
	{
		// Token: 0x06000777 RID: 1911 RVA: 0x0002D628 File Offset: 0x0002C628
		public RTP_Address(IPAddress ip, int dataPort, int controlPort)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool flag2 = dataPort < 0 || dataPort > 65535;
			if (flag2)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Argument 'dataPort' value must be between '",
					0,
					"' and '",
					65535,
					"'."
				}));
			}
			bool flag3 = controlPort < 0 || controlPort > 65535;
			if (flag3)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Argument 'controlPort' value must be between '",
					0,
					"' and '",
					65535,
					"'."
				}));
			}
			bool flag4 = dataPort == controlPort;
			if (flag4)
			{
				throw new ArgumentException("Arguments 'dataPort' and 'controlPort' values must be different.");
			}
			this.m_pIP = ip;
			this.m_DataPort = dataPort;
			this.m_ControlPort = controlPort;
			this.m_pRtpEP = new IPEndPoint(ip, dataPort);
			this.m_pRtcpEP = new IPEndPoint(ip, controlPort);
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x0002D764 File Offset: 0x0002C764
		public RTP_Address(IPAddress ip, int dataPort, int controlPort, int ttl)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool flag2 = !Net_Utils.IsMulticastAddress(ip);
			if (flag2)
			{
				throw new ArgumentException("Argument 'ip' is not multicast ip address.");
			}
			bool flag3 = dataPort < 0 || dataPort > 65535;
			if (flag3)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Argument 'dataPort' value must be between '",
					0,
					"' and '",
					65535,
					"'."
				}));
			}
			bool flag4 = controlPort < 0 || controlPort > 65535;
			if (flag4)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Argument 'controlPort' value must be between '",
					0,
					"' and '",
					65535,
					"'."
				}));
			}
			bool flag5 = dataPort == controlPort;
			if (flag5)
			{
				throw new ArgumentException("Arguments 'dataPort' and 'controlPort' values must be different.");
			}
			bool flag6 = ttl < 0 || ttl > 255;
			if (flag6)
			{
				throw new ArgumentException("Argument 'ttl' value must be between '0' and '255'.");
			}
			this.m_pIP = ip;
			this.m_DataPort = dataPort;
			this.m_ControlPort = controlPort;
			this.m_TTL = ttl;
			this.m_pRtpEP = new IPEndPoint(ip, dataPort);
			this.m_pRtcpEP = new IPEndPoint(ip, controlPort);
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0002D8E8 File Offset: 0x0002C8E8
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = obj is RTP_Address;
				if (flag2)
				{
					RTP_Address rtp_Address = (RTP_Address)obj;
					bool flag3 = rtp_Address.IP.Equals(this.IP) && rtp_Address.ControlPort == this.ControlPort && rtp_Address.DataPort == this.DataPort;
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0002D95C File Offset: 0x0002C95C
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x0002D974 File Offset: 0x0002C974
		public bool IsMulticast
		{
			get
			{
				return Net_Utils.IsMulticastAddress(this.m_pIP);
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x0002D994 File Offset: 0x0002C994
		public IPAddress IP
		{
			get
			{
				return this.m_pIP;
			}
		}

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x0002D9AC File Offset: 0x0002C9AC
		public int DataPort
		{
			get
			{
				return this.m_DataPort;
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0002D9C4 File Offset: 0x0002C9C4
		public int ControlPort
		{
			get
			{
				return this.m_ControlPort;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x0002D9DC File Offset: 0x0002C9DC
		public int TTL
		{
			get
			{
				return this.m_TTL;
			}
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x0002D9F4 File Offset: 0x0002C9F4
		public IPEndPoint RtpEP
		{
			get
			{
				return this.m_pRtpEP;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x0002DA0C File Offset: 0x0002CA0C
		public IPEndPoint RtcpEP
		{
			get
			{
				return this.m_pRtcpEP;
			}
		}

		// Token: 0x0400033A RID: 826
		private IPAddress m_pIP = null;

		// Token: 0x0400033B RID: 827
		private int m_DataPort = 0;

		// Token: 0x0400033C RID: 828
		private int m_ControlPort = 0;

		// Token: 0x0400033D RID: 829
		private int m_TTL = 0;

		// Token: 0x0400033E RID: 830
		private IPEndPoint m_pRtpEP = null;

		// Token: 0x0400033F RID: 831
		private IPEndPoint m_pRtcpEP = null;
	}
}
