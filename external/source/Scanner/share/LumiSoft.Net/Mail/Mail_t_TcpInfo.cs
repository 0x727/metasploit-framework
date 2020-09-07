using System;
using System.Net;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000186 RID: 390
	public class Mail_t_TcpInfo
	{
		// Token: 0x06001017 RID: 4119 RVA: 0x000646C8 File Offset: 0x000636C8
		public Mail_t_TcpInfo(IPAddress ip, string hostName)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			this.m_pIP = ip;
			this.m_HostName = hostName;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00064710 File Offset: 0x00063710
		public override string ToString()
		{
			bool flag = string.IsNullOrEmpty(this.m_HostName);
			string result;
			if (flag)
			{
				result = "[" + this.m_pIP.ToString() + "]";
			}
			else
			{
				result = this.m_HostName + " [" + this.m_pIP.ToString() + "]";
			}
			return result;
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x00064770 File Offset: 0x00063770
		public IPAddress IP
		{
			get
			{
				return this.m_pIP;
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600101A RID: 4122 RVA: 0x00064788 File Offset: 0x00063788
		public string HostName
		{
			get
			{
				return this.m_HostName;
			}
		}

		// Token: 0x04000680 RID: 1664
		private IPAddress m_pIP = null;

		// Token: 0x04000681 RID: 1665
		private string m_HostName = null;
	}
}
