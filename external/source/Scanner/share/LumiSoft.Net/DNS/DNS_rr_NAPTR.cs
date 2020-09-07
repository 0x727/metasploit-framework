using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x0200024C RID: 588
	[Serializable]
	public class DNS_rr_NAPTR : DNS_rr
	{
		// Token: 0x0600153F RID: 5439 RVA: 0x000847BC File Offset: 0x000837BC
		public DNS_rr_NAPTR(string name, int order, int preference, string flags, string services, string regexp, string replacement, int ttl) : base(name, DNS_QType.NAPTR, ttl)
		{
			this.m_Order = order;
			this.m_Preference = preference;
			this.m_Flags = flags;
			this.m_Services = services;
			this.m_Regexp = regexp;
			this.m_Replacement = replacement;
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00084840 File Offset: 0x00083840
		public static DNS_rr_NAPTR Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			int num = offset;
			offset = num + 1;
			int num2 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int order = num2 | (int)reply[num];
			num = offset;
			offset = num + 1;
			int num3 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int preference = num3 | (int)reply[num];
			string flags = Dns_Client.ReadCharacterString(reply, ref offset);
			string services = Dns_Client.ReadCharacterString(reply, ref offset);
			string regexp = Dns_Client.ReadCharacterString(reply, ref offset);
			string replacement = "";
			Dns_Client.GetQName(reply, ref offset, ref replacement);
			return new DNS_rr_NAPTR(name, order, preference, flags, services, regexp, replacement, ttl);
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x000848D0 File Offset: 0x000838D0
		public int Order
		{
			get
			{
				return this.m_Order;
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x000848E8 File Offset: 0x000838E8
		public int Preference
		{
			get
			{
				return this.m_Preference;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x00084900 File Offset: 0x00083900
		public string Flags
		{
			get
			{
				return this.m_Flags;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x00084918 File Offset: 0x00083918
		public string Services
		{
			get
			{
				return this.m_Services;
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x00084930 File Offset: 0x00083930
		public string Regexp
		{
			get
			{
				return this.m_Regexp;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06001546 RID: 5446 RVA: 0x00084948 File Offset: 0x00083948
		public string Replacement
		{
			get
			{
				return this.m_Replacement;
			}
		}

		// Token: 0x0400085E RID: 2142
		private int m_Order = 0;

		// Token: 0x0400085F RID: 2143
		private int m_Preference = 0;

		// Token: 0x04000860 RID: 2144
		private string m_Flags = "";

		// Token: 0x04000861 RID: 2145
		private string m_Services = "";

		// Token: 0x04000862 RID: 2146
		private string m_Regexp = "";

		// Token: 0x04000863 RID: 2147
		private string m_Replacement = "";
	}
}
