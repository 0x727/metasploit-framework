using System;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026B RID: 619
	public class AUTH_e_Authenticate : EventArgs
	{
		// Token: 0x06001634 RID: 5684 RVA: 0x0008AD2C File Offset: 0x00089D2C
		public AUTH_e_Authenticate(string authorizationID, string userName, string password)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = userName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'userName' value must be specified.", "userName");
			}
			this.m_AuthorizationID = authorizationID;
			this.m_UserName = userName;
			this.m_Password = password;
		}

		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06001635 RID: 5685 RVA: 0x0008ADB4 File Offset: 0x00089DB4
		// (set) Token: 0x06001636 RID: 5686 RVA: 0x0008ADCC File Offset: 0x00089DCC
		public bool IsAuthenticated
		{
			get
			{
				return this.m_IsAuthenticated;
			}
			set
			{
				this.m_IsAuthenticated = value;
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06001637 RID: 5687 RVA: 0x0008ADD8 File Offset: 0x00089DD8
		public string AuthorizationID
		{
			get
			{
				return this.m_AuthorizationID;
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06001638 RID: 5688 RVA: 0x0008ADF0 File Offset: 0x00089DF0
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06001639 RID: 5689 RVA: 0x0008AE08 File Offset: 0x00089E08
		public string Password
		{
			get
			{
				return this.m_Password;
			}
		}

		// Token: 0x040008E8 RID: 2280
		private bool m_IsAuthenticated = false;

		// Token: 0x040008E9 RID: 2281
		private string m_AuthorizationID = "";

		// Token: 0x040008EA RID: 2282
		private string m_UserName = "";

		// Token: 0x040008EB RID: 2283
		private string m_Password = "";
	}
}
