using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000266 RID: 614
	public class AUTH_SASL_Client_Plain : AUTH_SASL_Client
	{
		// Token: 0x060015FF RID: 5631 RVA: 0x0008966C File Offset: 0x0008866C
		public AUTH_SASL_Client_Plain(string userName, string password)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = userName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'username' value must be specified.", "userName");
			}
			bool flag3 = password == null;
			if (flag3)
			{
				throw new ArgumentNullException("password");
			}
			this.m_UserName = userName;
			this.m_Password = password;
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x000896F4 File Offset: 0x000886F4
		public override byte[] Continue(byte[] serverResponse)
		{
			bool isCompleted = this.m_IsCompleted;
			if (isCompleted)
			{
				throw new InvalidOperationException("Authentication is completed.");
			}
			bool flag = this.m_State == 0;
			if (flag)
			{
				this.m_State++;
				this.m_IsCompleted = true;
				return Encoding.UTF8.GetBytes("\0" + this.m_UserName + "\0" + this.m_Password);
			}
			throw new InvalidOperationException("Authentication is completed.");
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06001601 RID: 5633 RVA: 0x00089770 File Offset: 0x00088770
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06001602 RID: 5634 RVA: 0x00089788 File Offset: 0x00088788
		public override string Name
		{
			get
			{
				return "PLAIN";
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06001603 RID: 5635 RVA: 0x000897A0 File Offset: 0x000887A0
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06001604 RID: 5636 RVA: 0x000897B8 File Offset: 0x000887B8
		public override bool SupportsInitialResponse
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040008C7 RID: 2247
		private bool m_IsCompleted = false;

		// Token: 0x040008C8 RID: 2248
		private int m_State = 0;

		// Token: 0x040008C9 RID: 2249
		private string m_UserName = null;

		// Token: 0x040008CA RID: 2250
		private string m_Password = null;
	}
}
