using System;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	// Token: 0x02000255 RID: 597
	[Serializable]
	public class DNS_rr_SOA : DNS_rr
	{
		// Token: 0x06001566 RID: 5478 RVA: 0x00084EBC File Offset: 0x00083EBC
		public DNS_rr_SOA(string name, string nameServer, string adminEmail, long serial, long refresh, long retry, long expire, long minimum, int ttl) : base(name, DNS_QType.SOA, ttl)
		{
			this.m_NameServer = nameServer;
			this.m_AdminEmail = adminEmail;
			this.m_Serial = serial;
			this.m_Refresh = refresh;
			this.m_Retry = retry;
			this.m_Expire = expire;
			this.m_Minimum = minimum;
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00084F4C File Offset: 0x00083F4C
		public static DNS_rr_SOA Parse(string name, byte[] reply, ref int offset, int rdLength, int ttl)
		{
			string nameServer = "";
			Dns_Client.GetQName(reply, ref offset, ref nameServer);
			string text = "";
			Dns_Client.GetQName(reply, ref offset, ref text);
			char[] array = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = array[i] == '.';
				if (flag)
				{
					array[i] = '@';
					break;
				}
			}
			text = new string(array);
			int num = offset;
			offset = num + 1;
			byte b = (byte)(reply[num] << 24);
			num = offset;
			offset = num + 1;
			byte b2 = (byte)((int)b | (int)reply[num] << 16);
			num = offset;
			offset = num + 1;
			byte b3 = (byte)((int)b2 | (int)reply[num] << 8);
			num = offset;
			offset = num + 1;
			long serial = (long)(b3 | reply[num]);
			num = offset;
			offset = num + 1;
			byte b4 = (byte)(reply[num] << 24);
			num = offset;
			offset = num + 1;
			byte b5 = (byte)((int)b4 | (int)reply[num] << 16);
			num = offset;
			offset = num + 1;
			byte b6 = (byte)((int)b5 | (int)reply[num] << 8);
			num = offset;
			offset = num + 1;
			long refresh = (long)(b6 | reply[num]);
			num = offset;
			offset = num + 1;
			byte b7 = (byte)(reply[num] << 24);
			num = offset;
			offset = num + 1;
			byte b8 = (byte)((int)b7 | (int)reply[num] << 16);
			num = offset;
			offset = num + 1;
			byte b9 = (byte)((int)b8 | (int)reply[num] << 8);
			num = offset;
			offset = num + 1;
			long retry = (long)(b9 | reply[num]);
			num = offset;
			offset = num + 1;
			byte b10 = (byte)(reply[num] << 24);
			num = offset;
			offset = num + 1;
			byte b11 = (byte)((int)b10 | (int)reply[num] << 16);
			num = offset;
			offset = num + 1;
			byte b12 = (byte)((int)b11 | (int)reply[num] << 8);
			num = offset;
			offset = num + 1;
			long expire = (long)(b12 | reply[num]);
			num = offset;
			offset = num + 1;
			byte b13 = (byte)(reply[num] << 24);
			num = offset;
			offset = num + 1;
			byte b14 = (byte)((int)b13 | (int)reply[num] << 16);
			num = offset;
			offset = num + 1;
			byte b15 = (byte)((int)b14 | (int)reply[num] << 8);
			num = offset;
			offset = num + 1;
			long minimum = (long)(b15 | reply[num]);
			return new DNS_rr_SOA(name, nameServer, text, serial, refresh, retry, expire, minimum, ttl);
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06001568 RID: 5480 RVA: 0x00085130 File Offset: 0x00084130
		public string NameServer
		{
			get
			{
				return this.m_NameServer;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06001569 RID: 5481 RVA: 0x00085148 File Offset: 0x00084148
		public string AdminEmail
		{
			get
			{
				return this.m_AdminEmail;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600156A RID: 5482 RVA: 0x00085160 File Offset: 0x00084160
		public long Serial
		{
			get
			{
				return this.m_Serial;
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x0600156B RID: 5483 RVA: 0x00085178 File Offset: 0x00084178
		public long Refresh
		{
			get
			{
				return this.m_Refresh;
			}
		}

		// Token: 0x170006FA RID: 1786
		// (get) Token: 0x0600156C RID: 5484 RVA: 0x00085190 File Offset: 0x00084190
		public long Retry
		{
			get
			{
				return this.m_Retry;
			}
		}

		// Token: 0x170006FB RID: 1787
		// (get) Token: 0x0600156D RID: 5485 RVA: 0x000851A8 File Offset: 0x000841A8
		public long Expire
		{
			get
			{
				return this.m_Expire;
			}
		}

		// Token: 0x170006FC RID: 1788
		// (get) Token: 0x0600156E RID: 5486 RVA: 0x000851C0 File Offset: 0x000841C0
		public long Minimum
		{
			get
			{
				return this.m_Minimum;
			}
		}

		// Token: 0x04000873 RID: 2163
		private string m_NameServer = "";

		// Token: 0x04000874 RID: 2164
		private string m_AdminEmail = "";

		// Token: 0x04000875 RID: 2165
		private long m_Serial = 0L;

		// Token: 0x04000876 RID: 2166
		private long m_Refresh = 0L;

		// Token: 0x04000877 RID: 2167
		private long m_Retry = 0L;

		// Token: 0x04000878 RID: 2168
		private long m_Expire = 0L;

		// Token: 0x04000879 RID: 2169
		private long m_Minimum = 0L;
	}
}
