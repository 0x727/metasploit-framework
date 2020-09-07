using System;
using LumiSoft.Net.AUTH;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000AC RID: 172
	public class SIP_AuthenticateEventArgs
	{
		// Token: 0x060006A8 RID: 1704 RVA: 0x0002782C File Offset: 0x0002682C
		public SIP_AuthenticateEventArgs(Auth_HttpDigest auth)
		{
			this.m_pAuth = auth;
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x0002784C File Offset: 0x0002684C
		public Auth_HttpDigest AuthContext
		{
			get
			{
				return this.m_pAuth;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x00027864 File Offset: 0x00026864
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x0002787C File Offset: 0x0002687C
		public bool Authenticated
		{
			get
			{
				return this.m_Authenticated;
			}
			set
			{
				this.m_Authenticated = value;
			}
		}

		// Token: 0x040002C6 RID: 710
		private Auth_HttpDigest m_pAuth = null;

		// Token: 0x040002C7 RID: 711
		private bool m_Authenticated = false;
	}
}
