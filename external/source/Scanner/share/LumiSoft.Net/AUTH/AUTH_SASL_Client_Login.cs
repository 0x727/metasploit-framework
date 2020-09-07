using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000264 RID: 612
	public class AUTH_SASL_Client_Login : AUTH_SASL_Client
	{
		// Token: 0x060015F4 RID: 5620 RVA: 0x00089338 File Offset: 0x00088338
		public AUTH_SASL_Client_Login(string userName, string password)
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

		// Token: 0x060015F5 RID: 5621 RVA: 0x000893C0 File Offset: 0x000883C0
		public override byte[] Continue(byte[] serverResponse)
		{
			bool flag = serverResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("serverResponse");
			}
			bool isCompleted = this.m_IsCompleted;
			if (isCompleted)
			{
				throw new InvalidOperationException("Authentication is completed.");
			}
			bool flag2 = this.m_State == 0;
			byte[] bytes;
			if (flag2)
			{
				this.m_State++;
				bytes = Encoding.UTF8.GetBytes(this.m_UserName);
			}
			else
			{
				bool flag3 = this.m_State == 1;
				if (!flag3)
				{
					throw new InvalidOperationException("Authentication is completed.");
				}
				this.m_State++;
				this.m_IsCompleted = true;
				bytes = Encoding.UTF8.GetBytes(this.m_Password);
			}
			return bytes;
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x060015F6 RID: 5622 RVA: 0x0008946C File Offset: 0x0008846C
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x060015F7 RID: 5623 RVA: 0x00089484 File Offset: 0x00088484
		public override string Name
		{
			get
			{
				return "LOGIN";
			}
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x060015F8 RID: 5624 RVA: 0x0008949C File Offset: 0x0008849C
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x040008BE RID: 2238
		private bool m_IsCompleted = false;

		// Token: 0x040008BF RID: 2239
		private int m_State = 0;

		// Token: 0x040008C0 RID: 2240
		private string m_UserName = null;

		// Token: 0x040008C1 RID: 2241
		private string m_Password = null;
	}
}
