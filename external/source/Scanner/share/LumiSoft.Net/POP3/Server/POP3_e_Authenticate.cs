using System;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000E8 RID: 232
	public class POP3_e_Authenticate : EventArgs
	{
		// Token: 0x0600096F RID: 2415 RVA: 0x00039880 File Offset: 0x00038880
		internal POP3_e_Authenticate(string user, string password)
		{
			bool flag = user == null;
			if (flag)
			{
				throw new ArgumentNullException("user");
			}
			bool flag2 = password == null;
			if (flag2)
			{
				throw new ArgumentNullException("password");
			}
			this.m_User = user;
			this.m_Password = password;
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000970 RID: 2416 RVA: 0x000398E8 File Offset: 0x000388E8
		// (set) Token: 0x06000971 RID: 2417 RVA: 0x00039900 File Offset: 0x00038900
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

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x0003990C File Offset: 0x0003890C
		public string User
		{
			get
			{
				return this.m_User;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000973 RID: 2419 RVA: 0x00039924 File Offset: 0x00038924
		public string Password
		{
			get
			{
				return this.m_Password;
			}
		}

		// Token: 0x0400042F RID: 1071
		private bool m_IsAuthenticated = false;

		// Token: 0x04000430 RID: 1072
		private string m_User = "";

		// Token: 0x04000431 RID: 1073
		private string m_Password = "";
	}
}
