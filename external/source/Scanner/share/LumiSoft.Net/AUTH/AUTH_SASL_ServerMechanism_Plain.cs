using System;
using System.Diagnostics;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000273 RID: 627
	public class AUTH_SASL_ServerMechanism_Plain : AUTH_SASL_ServerMechanism
	{
		// Token: 0x0600169F RID: 5791 RVA: 0x0008CD3D File Offset: 0x0008BD3D
		public AUTH_SASL_ServerMechanism_Plain(bool requireSSL)
		{
			this.m_RequireSSL = requireSSL;
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0008CD75 File Offset: 0x0008BD75
		public override void Reset()
		{
			this.m_IsCompleted = false;
			this.m_IsAuthenticated = false;
			this.m_UserName = "";
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0008CD94 File Offset: 0x0008BD94
		public override byte[] Continue(byte[] clientResponse)
		{
			bool flag = clientResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("clientResponse");
			}
			bool flag2 = clientResponse.Length == 0;
			byte[] result;
			if (flag2)
			{
				result = new byte[0];
			}
			else
			{
				string[] array = Encoding.UTF8.GetString(clientResponse).Split(new char[1]);
				bool flag3 = array.Length == 3 && !string.IsNullOrEmpty(array[1]);
				if (flag3)
				{
					this.m_UserName = array[1];
					AUTH_e_Authenticate auth_e_Authenticate = this.OnAuthenticate(array[0], array[1], array[2]);
					this.m_IsAuthenticated = auth_e_Authenticate.IsAuthenticated;
				}
				this.m_IsCompleted = true;
				result = null;
			}
			return result;
		}

		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x060016A2 RID: 5794 RVA: 0x0008CE34 File Offset: 0x0008BE34
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x060016A3 RID: 5795 RVA: 0x0008CE4C File Offset: 0x0008BE4C
		public override bool IsAuthenticated
		{
			get
			{
				return this.m_IsAuthenticated;
			}
		}

		// Token: 0x17000773 RID: 1907
		// (get) Token: 0x060016A4 RID: 5796 RVA: 0x0008CE64 File Offset: 0x0008BE64
		public override string Name
		{
			get
			{
				return "PLAIN";
			}
		}

		// Token: 0x17000774 RID: 1908
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x0008CE7C File Offset: 0x0008BE7C
		public override bool RequireSSL
		{
			get
			{
				return this.m_RequireSSL;
			}
		}

		// Token: 0x17000775 RID: 1909
		// (get) Token: 0x060016A6 RID: 5798 RVA: 0x0008CE94 File Offset: 0x0008BE94
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1400009D RID: 157
		// (add) Token: 0x060016A7 RID: 5799 RVA: 0x0008CEAC File Offset: 0x0008BEAC
		// (remove) Token: 0x060016A8 RID: 5800 RVA: 0x0008CEE4 File Offset: 0x0008BEE4
		
		public event EventHandler<AUTH_e_Authenticate> Authenticate = null;

		// Token: 0x060016A9 RID: 5801 RVA: 0x0008CF1C File Offset: 0x0008BF1C
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

		// Token: 0x04000916 RID: 2326
		private bool m_IsCompleted = false;

		// Token: 0x04000917 RID: 2327
		private bool m_IsAuthenticated = false;

		// Token: 0x04000918 RID: 2328
		private bool m_RequireSSL = false;

		// Token: 0x04000919 RID: 2329
		private string m_UserName = "";
	}
}
