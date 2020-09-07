using System;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BA RID: 186
	public class SDP_Origin
	{
		// Token: 0x0600071F RID: 1823 RVA: 0x0002BC70 File Offset: 0x0002AC70
		public SDP_Origin(string userName, long sessionID, long sessionVersion, string netType, string addressType, string unicastAddress)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = userName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'userName' value must be specified.");
			}
			bool flag3 = netType == null;
			if (flag3)
			{
				throw new ArgumentNullException("netType");
			}
			bool flag4 = netType == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'netType' value must be specified.");
			}
			bool flag5 = addressType == null;
			if (flag5)
			{
				throw new ArgumentNullException("addressType");
			}
			bool flag6 = addressType == string.Empty;
			if (flag6)
			{
				throw new ArgumentException("Argument 'addressType' value must be specified.");
			}
			bool flag7 = unicastAddress == null;
			if (flag7)
			{
				throw new ArgumentNullException("unicastAddress");
			}
			bool flag8 = unicastAddress == string.Empty;
			if (flag8)
			{
				throw new ArgumentException("Argument 'unicastAddress' value must be specified.");
			}
			this.m_UserName = userName;
			this.m_SessionID = sessionID;
			this.m_SessionVersion = sessionVersion;
			this.m_NetType = netType;
			this.m_AddressType = addressType;
			this.m_UnicastAddress = unicastAddress;
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x0002BDA8 File Offset: 0x0002ADA8
		public static SDP_Origin Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value = value.Trim();
			bool flag2 = !value.ToLower().StartsWith("o=");
			if (flag2)
			{
				throw new ParseException("Invalid SDP Origin('o=') value '" + value + "'.");
			}
			value = value.Substring(2);
			string[] array = value.Split(new char[]
			{
				' '
			});
			bool flag3 = array.Length != 6;
			if (flag3)
			{
				throw new ParseException("Invalid SDP Origin('o=') value '" + value + "'.");
			}
			return new SDP_Origin(array[0], Convert.ToInt64(array[1]), Convert.ToInt64(array[2]), array[3], array[4], array[5]);
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x0002BE68 File Offset: 0x0002AE68
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"o=",
				this.m_UserName,
				" ",
				this.m_SessionID,
				" ",
				this.m_SessionVersion,
				" ",
				this.m_NetType,
				" ",
				this.m_AddressType,
				" ",
				this.m_UnicastAddress,
				"\r\n"
			});
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000722 RID: 1826 RVA: 0x0002BF04 File Offset: 0x0002AF04
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0002BF1C File Offset: 0x0002AF1C
		public long SessionID
		{
			get
			{
				return this.m_SessionID;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000724 RID: 1828 RVA: 0x0002BF34 File Offset: 0x0002AF34
		// (set) Token: 0x06000725 RID: 1829 RVA: 0x0002BF4C File Offset: 0x0002AF4C
		public long SessionVersion
		{
			get
			{
				return this.m_SessionVersion;
			}
			set
			{
				this.m_SessionVersion = value;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000726 RID: 1830 RVA: 0x0002BF58 File Offset: 0x0002AF58
		public string NetType
		{
			get
			{
				return this.m_NetType;
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000727 RID: 1831 RVA: 0x0002BF70 File Offset: 0x0002AF70
		public string AddressType
		{
			get
			{
				return this.m_AddressType;
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000728 RID: 1832 RVA: 0x0002BF88 File Offset: 0x0002AF88
		public string UnicastAddress
		{
			get
			{
				return this.m_UnicastAddress;
			}
		}

		// Token: 0x0400030A RID: 778
		private string m_UserName = null;

		// Token: 0x0400030B RID: 779
		private long m_SessionID = 0L;

		// Token: 0x0400030C RID: 780
		private long m_SessionVersion = 0L;

		// Token: 0x0400030D RID: 781
		private string m_NetType = null;

		// Token: 0x0400030E RID: 782
		private string m_AddressType = null;

		// Token: 0x0400030F RID: 783
		private string m_UnicastAddress = null;
	}
}
