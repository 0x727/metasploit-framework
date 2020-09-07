using System;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026C RID: 620
	public class AUTH_e_UserInfo : EventArgs
	{
		// Token: 0x0600163A RID: 5690 RVA: 0x0008AE20 File Offset: 0x00089E20
		public AUTH_e_UserInfo(string userName)
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
			this.m_UserName = userName;
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x0600163B RID: 5691 RVA: 0x0008AE90 File Offset: 0x00089E90
		// (set) Token: 0x0600163C RID: 5692 RVA: 0x0008AEA8 File Offset: 0x00089EA8
		public bool UserExists
		{
			get
			{
				return this.m_UserExists;
			}
			set
			{
				this.m_UserExists = value;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x0600163D RID: 5693 RVA: 0x0008AEB4 File Offset: 0x00089EB4
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x0600163E RID: 5694 RVA: 0x0008AECC File Offset: 0x00089ECC
		// (set) Token: 0x0600163F RID: 5695 RVA: 0x0008AEE4 File Offset: 0x00089EE4
		public string Password
		{
			get
			{
				return this.m_Password;
			}
			set
			{
				this.m_Password = value;
			}
		}

		// Token: 0x040008EC RID: 2284
		private bool m_UserExists = false;

		// Token: 0x040008ED RID: 2285
		private string m_UserName = "";

		// Token: 0x040008EE RID: 2286
		private string m_Password = "";
	}
}
