using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000263 RID: 611
	public class AUTH_SASL_Client_DigestMd5 : AUTH_SASL_Client
	{
		// Token: 0x060015EF RID: 5615 RVA: 0x00089074 File Offset: 0x00088074
		public AUTH_SASL_Client_DigestMd5(string protocol, string server, string userName, string password)
		{
			bool flag = protocol == null;
			if (flag)
			{
				throw new ArgumentNullException("protocol");
			}
			bool flag2 = protocol == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'protocol' value must be specified.", "userName");
			}
			bool flag3 = server == null;
			if (flag3)
			{
				throw new ArgumentNullException("protocol");
			}
			bool flag4 = server == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'server' value must be specified.", "userName");
			}
			bool flag5 = userName == null;
			if (flag5)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag6 = userName == string.Empty;
			if (flag6)
			{
				throw new ArgumentException("Argument 'username' value must be specified.", "userName");
			}
			bool flag7 = password == null;
			if (flag7)
			{
				throw new ArgumentNullException("password");
			}
			this.m_Protocol = protocol;
			this.m_ServerName = server;
			this.m_UserName = userName;
			this.m_Password = password;
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00089190 File Offset: 0x00088190
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
			byte[] result;
			if (flag2)
			{
				this.m_State++;
				AUTH_SASL_DigestMD5_Challenge auth_SASL_DigestMD5_Challenge = AUTH_SASL_DigestMD5_Challenge.Parse(Encoding.UTF8.GetString(serverResponse));
				this.m_pResponse = new AUTH_SASL_DigestMD5_Response(auth_SASL_DigestMD5_Challenge, auth_SASL_DigestMD5_Challenge.Realm[0], this.m_UserName, this.m_Password, Guid.NewGuid().ToString().Replace("-", ""), 1, auth_SASL_DigestMD5_Challenge.QopOptions[0], this.m_Protocol + "/" + this.m_ServerName);
				result = Encoding.UTF8.GetBytes(this.m_pResponse.ToResponse());
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
				bool flag4 = !string.Equals(Encoding.UTF8.GetString(serverResponse), this.m_pResponse.ToRspauthResponse(this.m_UserName, this.m_Password), StringComparison.InvariantCultureIgnoreCase);
				if (flag4)
				{
					throw new Exception("Server server 'rspauth' value mismatch with local 'rspauth' value.");
				}
				result = new byte[0];
			}
			return result;
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060015F1 RID: 5617 RVA: 0x000892F0 File Offset: 0x000882F0
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060015F2 RID: 5618 RVA: 0x00089308 File Offset: 0x00088308
		public override string Name
		{
			get
			{
				return "DIGEST-MD5";
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x060015F3 RID: 5619 RVA: 0x00089320 File Offset: 0x00088320
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x040008B7 RID: 2231
		private bool m_IsCompleted = false;

		// Token: 0x040008B8 RID: 2232
		private int m_State = 0;

		// Token: 0x040008B9 RID: 2233
		private string m_Protocol = null;

		// Token: 0x040008BA RID: 2234
		private string m_ServerName = null;

		// Token: 0x040008BB RID: 2235
		private string m_UserName = null;

		// Token: 0x040008BC RID: 2236
		private string m_Password = null;

		// Token: 0x040008BD RID: 2237
		private AUTH_SASL_DigestMD5_Response m_pResponse = null;
	}
}
