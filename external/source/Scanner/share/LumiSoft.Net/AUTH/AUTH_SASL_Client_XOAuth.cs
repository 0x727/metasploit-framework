using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000267 RID: 615
	public class AUTH_SASL_Client_XOAuth : AUTH_SASL_Client
	{
		// Token: 0x06001605 RID: 5637 RVA: 0x000897CC File Offset: 0x000887CC
		public AUTH_SASL_Client_XOAuth(string userName, string authString)
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
			bool flag3 = authString == null;
			if (flag3)
			{
				throw new ArgumentNullException("authString");
			}
			bool flag4 = authString == "";
			if (flag4)
			{
				throw new ArgumentException("Argument 'authString' value must be specified.", "authString");
			}
			this.m_UserName = userName;
			this.m_AuthString = authString;
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00089874 File Offset: 0x00088874
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
				result = Encoding.UTF8.GetBytes(this.m_AuthString);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x06001607 RID: 5639 RVA: 0x000898C8 File Offset: 0x000888C8
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x06001608 RID: 5640 RVA: 0x000898E0 File Offset: 0x000888E0
		public override string Name
		{
			get
			{
				return "XOAUTH";
			}
		}

		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x06001609 RID: 5641 RVA: 0x000898F8 File Offset: 0x000888F8
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x0600160A RID: 5642 RVA: 0x00089910 File Offset: 0x00088910
		public override bool SupportsInitialResponse
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040008CB RID: 2251
		private bool m_IsCompleted = false;

		// Token: 0x040008CC RID: 2252
		private int m_State = 0;

		// Token: 0x040008CD RID: 2253
		private string m_UserName = null;

		// Token: 0x040008CE RID: 2254
		private string m_AuthString = null;
	}
}
