using System;
using System.Net;

namespace LumiSoft.Net
{
	// Token: 0x02000007 RID: 7
	public class HostEntry
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000022E8 File Offset: 0x000012E8
		public HostEntry(string hostName, IPAddress[] ipAddresses, string[] aliases)
		{
			bool flag = hostName == null;
			if (flag)
			{
				throw new ArgumentNullException("hostName");
			}
			bool flag2 = hostName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'hostName' value must be specified.", "hostName");
			}
			bool flag3 = ipAddresses == null;
			if (flag3)
			{
				throw new ArgumentNullException("ipAddresses");
			}
			this.m_HostName = hostName;
			this.m_pAddresses = ipAddresses;
			this.m_pAliases = ((aliases == null) ? new string[0] : aliases);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000237C File Offset: 0x0000137C
		public string HostName
		{
			get
			{
				return this.m_HostName;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002394 File Offset: 0x00001394
		public IPAddress[] Addresses
		{
			get
			{
				return this.m_pAddresses;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000023AC File Offset: 0x000013AC
		public string[] Aliases
		{
			get
			{
				return this.m_pAliases;
			}
		}

		// Token: 0x04000012 RID: 18
		private string m_HostName = null;

		// Token: 0x04000013 RID: 19
		private IPAddress[] m_pAddresses = null;

		// Token: 0x04000014 RID: 20
		private string[] m_pAliases = null;
	}
}
