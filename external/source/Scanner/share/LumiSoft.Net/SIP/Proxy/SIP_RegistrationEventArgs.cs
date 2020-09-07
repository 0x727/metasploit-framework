using System;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000AB RID: 171
	public class SIP_RegistrationEventArgs : EventArgs
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x000277DC File Offset: 0x000267DC
		public SIP_RegistrationEventArgs(SIP_Registration registration)
		{
			bool flag = registration == null;
			if (flag)
			{
				throw new ArgumentNullException("registration");
			}
			this.m_pRegistration = registration;
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060006A7 RID: 1703 RVA: 0x00027814 File Offset: 0x00026814
		public SIP_Registration Registration
		{
			get
			{
				return this.m_pRegistration;
			}
		}

		// Token: 0x040002C5 RID: 709
		private SIP_Registration m_pRegistration = null;
	}
}
