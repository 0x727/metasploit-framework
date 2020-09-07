using System;
using System.Diagnostics;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000272 RID: 626
	public class AUTH_SASL_ServerMechanism_Login : AUTH_SASL_ServerMechanism
	{
		// Token: 0x06001694 RID: 5780 RVA: 0x0008CAA8 File Offset: 0x0008BAA8
		public AUTH_SASL_ServerMechanism_Login(bool requireSSL)
		{
			this.m_RequireSSL = requireSSL;
		}

		// Token: 0x06001695 RID: 5781 RVA: 0x0008CAF5 File Offset: 0x0008BAF5
		public override void Reset()
		{
			this.m_IsCompleted = false;
			this.m_IsAuthenticated = false;
			this.m_UserName = null;
			this.m_Password = null;
			this.m_State = 0;
		}

		// Token: 0x06001696 RID: 5782 RVA: 0x0008CB1C File Offset: 0x0008BB1C
		public override byte[] Continue(byte[] clientResponse)
		{
			bool flag = clientResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("clientResponse");
			}
			bool flag2 = this.m_State == 0 && clientResponse.Length != 0;
			if (flag2)
			{
				this.m_State++;
			}
			bool flag3 = this.m_State == 0;
			byte[] result;
			if (flag3)
			{
				this.m_State++;
				result = Encoding.ASCII.GetBytes("UserName:");
			}
			else
			{
				bool flag4 = this.m_State == 1;
				if (flag4)
				{
					this.m_State++;
					this.m_UserName = Encoding.UTF8.GetString(clientResponse);
					result = Encoding.ASCII.GetBytes("Password:");
				}
				else
				{
					this.m_Password = Encoding.UTF8.GetString(clientResponse);
					AUTH_e_Authenticate auth_e_Authenticate = this.OnAuthenticate("", this.m_UserName, this.m_Password);
					this.m_IsAuthenticated = auth_e_Authenticate.IsAuthenticated;
					this.m_IsCompleted = true;
					result = null;
				}
			}
			return result;
		}

		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06001697 RID: 5783 RVA: 0x0008CC1C File Offset: 0x0008BC1C
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x1700076D RID: 1901
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x0008CC34 File Offset: 0x0008BC34
		public override bool IsAuthenticated
		{
			get
			{
				return this.m_IsAuthenticated;
			}
		}

		// Token: 0x1700076E RID: 1902
		// (get) Token: 0x06001699 RID: 5785 RVA: 0x0008CC4C File Offset: 0x0008BC4C
		public override string Name
		{
			get
			{
				return "LOGIN";
			}
		}

		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x0600169A RID: 5786 RVA: 0x0008CC64 File Offset: 0x0008BC64
		public override bool RequireSSL
		{
			get
			{
				return this.m_RequireSSL;
			}
		}

		// Token: 0x17000770 RID: 1904
		// (get) Token: 0x0600169B RID: 5787 RVA: 0x0008CC7C File Offset: 0x0008BC7C
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x0600169C RID: 5788 RVA: 0x0008CC94 File Offset: 0x0008BC94
		// (remove) Token: 0x0600169D RID: 5789 RVA: 0x0008CCCC File Offset: 0x0008BCCC
		
		public event EventHandler<AUTH_e_Authenticate> Authenticate = null;

		// Token: 0x0600169E RID: 5790 RVA: 0x0008CD04 File Offset: 0x0008BD04
		private AUTH_e_Authenticate OnAuthenticate(string authorizationID, string userName, string password)
		{
			AUTH_e_Authenticate auth_e_Authenticate = new AUTH_e_Authenticate(authorizationID, userName, password);
			bool flag = this.Authenticate != null;
			if (flag)
			{
				this.Authenticate(this, auth_e_Authenticate);
			}
			return auth_e_Authenticate;
		}

		// Token: 0x0400090F RID: 2319
		private bool m_IsCompleted = false;

		// Token: 0x04000910 RID: 2320
		private bool m_IsAuthenticated = false;

		// Token: 0x04000911 RID: 2321
		private bool m_RequireSSL = false;

		// Token: 0x04000912 RID: 2322
		private string m_UserName = null;

		// Token: 0x04000913 RID: 2323
		private string m_Password = null;

		// Token: 0x04000914 RID: 2324
		private int m_State = 0;
	}
}
