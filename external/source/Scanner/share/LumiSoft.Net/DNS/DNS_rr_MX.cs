using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000252 RID: 594
	[Serializable]
	public class DNS_rr_MX : DNS_rr, IComparable
	{
		// Token: 0x0600155B RID: 5467 RVA: 0x00084C9C File Offset: 0x00083C9C
		public DNS_rr_MX(string name, int preference, string host, int ttl) : base(name, DNS_QType.MX, ttl)
		{
			this.m_Preference = preference;
			this.m_Host = host;
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00084CCC File Offset: 0x00083CCC
		public static DNS_rr_MX Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			int num = offset;
			offset = num + 1;
			int num2 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int preference = num2 | (int)reply[num];
			string host = "";
			bool qname = Dns_Client.GetQName(reply, ref offset, ref host);
			if (qname)
			{
				return new DNS_rr_MX(name, preference, host, ttl);
			}
			throw new ArgumentException("Invalid MX resource record data !");
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00084D24 File Offset: 0x00083D24
		public int CompareTo(object obj)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = !(obj is DNS_rr_MX);
			if (flag2)
			{
				throw new ArgumentException("Argument obj is not MX_Record !");
			}
			DNS_rr_MX dns_rr_MX = (DNS_rr_MX)obj;
			bool flag3 = this.Preference > dns_rr_MX.Preference;
			int result;
			if (flag3)
			{
				result = 1;
			}
			else
			{
				bool flag4 = this.Preference < dns_rr_MX.Preference;
				if (flag4)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x0600155E RID: 5470 RVA: 0x00084DA4 File Offset: 0x00083DA4
		public int Preference
		{
			get
			{
				return this.m_Preference;
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x0600155F RID: 5471 RVA: 0x00084DBC File Offset: 0x00083DBC
		public string Host
		{
			get
			{
				return this.m_Host;
			}
		}

		// Token: 0x0400086F RID: 2159
		private int m_Preference = 0;

		// Token: 0x04000870 RID: 2160
		private string m_Host = "";
	}
}
