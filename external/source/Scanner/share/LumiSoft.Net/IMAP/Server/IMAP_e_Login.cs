using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000218 RID: 536
	public class IMAP_e_Login : EventArgs
	{
		// Token: 0x0600133B RID: 4923 RVA: 0x00076FA8 File Offset: 0x00075FA8
		internal IMAP_e_Login(string user, string password)
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

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x0600133C RID: 4924 RVA: 0x00077010 File Offset: 0x00076010
		// (set) Token: 0x0600133D RID: 4925 RVA: 0x00077028 File Offset: 0x00076028
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

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x0600133E RID: 4926 RVA: 0x00077034 File Offset: 0x00076034
		public string UserName
		{
			get
			{
				return this.m_User;
			}
		}

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x0600133F RID: 4927 RVA: 0x0007704C File Offset: 0x0007604C
		public string Password
		{
			get
			{
				return this.m_Password;
			}
		}

		// Token: 0x04000783 RID: 1923
		private bool m_IsAuthenticated = false;

		// Token: 0x04000784 RID: 1924
		private string m_User = "";

		// Token: 0x04000785 RID: 1925
		private string m_Password = "";
	}
}
