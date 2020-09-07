using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x0200024D RID: 589
	[Serializable]
	public class DNS_rr_SRV : DNS_rr
	{
		// Token: 0x06001547 RID: 5447 RVA: 0x00084960 File Offset: 0x00083960
		public DNS_rr_SRV(string name, int priority, int weight, int port, string target, int ttl) : base(name, DNS_QType.SRV, ttl)
		{
			this.m_Priority = priority;
			this.m_Weight = weight;
			this.m_Port = port;
			this.m_Target = target;
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000849B8 File Offset: 0x000839B8
		public static DNS_rr_SRV Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			int num = offset;
			offset = num + 1;
			int num2 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int priority = num2 | (int)reply[num];
			num = offset;
			offset = num + 1;
			int num3 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int weight = num3 | (int)reply[num];
			num = offset;
			offset = num + 1;
			int num4 = (int)reply[num] << 8;
			num = offset;
			offset = num + 1;
			int port = num4 | (int)reply[num];
			string target = "";
			Dns_Client.GetQName(reply, ref offset, ref target);
			return new DNS_rr_SRV(name, priority, weight, port, target, ttl);
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06001549 RID: 5449 RVA: 0x00084A48 File Offset: 0x00083A48
		public int Priority
		{
			get
			{
				return this.m_Priority;
			}
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x00084A60 File Offset: 0x00083A60
		public int Weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x0600154B RID: 5451 RVA: 0x00084A78 File Offset: 0x00083A78
		public int Port
		{
			get
			{
				return this.m_Port;
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x00084A90 File Offset: 0x00083A90
		public string Target
		{
			get
			{
				return this.m_Target;
			}
		}

		// Token: 0x04000864 RID: 2148
		private int m_Priority = 1;

		// Token: 0x04000865 RID: 2149
		private int m_Weight = 1;

		// Token: 0x04000866 RID: 2150
		private int m_Port = 0;

		// Token: 0x04000867 RID: 2151
		private string m_Target = "";
	}
}
