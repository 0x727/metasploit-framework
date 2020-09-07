using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000268 RID: 616
	public class AUTH_SASL_Client_XOAuth2 : AUTH_SASL_Client
	{
		// Token: 0x0600160B RID: 5643 RVA: 0x00089924 File Offset: 0x00088924
		public AUTH_SASL_Client_XOAuth2(string userName, string accessToken)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = userName == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'userName' value must be specified.", "userName");
			}
			bool flag3 = accessToken == null;
			if (flag3)
			{
				throw new ArgumentNullException("accessToken");
			}
			bool flag4 = accessToken == "";
			if (flag4)
			{
				throw new ArgumentException("Argument 'accessToken' value must be specified.", "accessToken");
			}
			this.m_UserName = userName;
			this.m_AccessToken = accessToken;
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000899CC File Offset: 0x000889CC
		public override byte[] Continue(byte[] serverResponse)
		{
			bool isCompleted = this.m_IsCompleted;
			if (isCompleted)
			{
				throw new InvalidOperationException("Authentication is completed.");
			}
			bool flag = this.m_State == 0;
			byte[] result;
			if (flag)
			{
				this.m_IsCompleted = true;
				string s = string.Concat(new string[]
				{
					"user=",
					this.m_UserName,
					"\u0001auth=Bearer ",
					this.m_AccessToken,
					"\u0001\u0001"
				});
				result = Encoding.UTF8.GetBytes(s);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x0600160D RID: 5645 RVA: 0x00089A50 File Offset: 0x00088A50
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x0600160E RID: 5646 RVA: 0x00089A68 File Offset: 0x00088A68
		public override string Name
		{
			get
			{
				return "XOAUTH2";
			}
		}

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x0600160F RID: 5647 RVA: 0x00089A80 File Offset: 0x00088A80
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x00089A98 File Offset: 0x00088A98
		public override bool SupportsInitialResponse
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040008CF RID: 2255
		private bool m_IsCompleted = false;

		// Token: 0x040008D0 RID: 2256
		private int m_State = 0;

		// Token: 0x040008D1 RID: 2257
		private string m_UserName = null;

		// Token: 0x040008D2 RID: 2258
		private string m_AccessToken = null;
	}
}
