using System;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000238 RID: 568
	public class FTP_e_Authenticate : EventArgs
	{
		// Token: 0x060014A1 RID: 5281 RVA: 0x000815C8 File Offset: 0x000805C8
		internal FTP_e_Authenticate(string user, string password)
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

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x060014A2 RID: 5282 RVA: 0x00081630 File Offset: 0x00080630
		// (set) Token: 0x060014A3 RID: 5283 RVA: 0x00081648 File Offset: 0x00080648
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

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x00081654 File Offset: 0x00080654
		public string User
		{
			get
			{
				return this.m_User;
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x060014A5 RID: 5285 RVA: 0x0008166C File Offset: 0x0008066C
		public string Password
		{
			get
			{
				return this.m_Password;
			}
		}

		// Token: 0x04000809 RID: 2057
		private bool m_IsAuthenticated = false;

		// Token: 0x0400080A RID: 2058
		private string m_User = "";

		// Token: 0x0400080B RID: 2059
		private string m_Password = "";
	}
}
