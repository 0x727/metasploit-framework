using System;
using System.Diagnostics;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000271 RID: 625
	public class AUTH_SASL_ServerMechanism_DigestMd5 : AUTH_SASL_ServerMechanism
	{
		// Token: 0x06001687 RID: 5767 RVA: 0x0008C718 File Offset: 0x0008B718
		public AUTH_SASL_ServerMechanism_DigestMd5(bool requireSSL)
		{
			this.m_RequireSSL = requireSSL;
			this.m_Nonce = Auth_HttpDigest.CreateNonce();
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x0008C783 File Offset: 0x0008B783
		public override void Reset()
		{
			this.m_IsCompleted = false;
			this.m_IsAuthenticated = false;
			this.m_UserName = "";
			this.m_State = 0;
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x0008C7A8 File Offset: 0x0008B7A8
		public override byte[] Continue(byte[] clientResponse)
		{
			bool flag = clientResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("clientResponse");
			}
			bool flag2 = this.m_State == 0;
			byte[] result;
			if (flag2)
			{
				this.m_State++;
				AUTH_SASL_DigestMD5_Challenge auth_SASL_DigestMD5_Challenge = new AUTH_SASL_DigestMD5_Challenge(new string[]
				{
					this.m_Realm
				}, this.m_Nonce, new string[]
				{
					"auth"
				}, false);
				result = Encoding.UTF8.GetBytes(auth_SASL_DigestMD5_Challenge.ToChallenge());
			}
			else
			{
				bool flag3 = this.m_State == 1;
				if (flag3)
				{
					this.m_State++;
					try
					{
						AUTH_SASL_DigestMD5_Response auth_SASL_DigestMD5_Response = AUTH_SASL_DigestMD5_Response.Parse(Encoding.UTF8.GetString(clientResponse));
						bool flag4 = this.m_Realm != auth_SASL_DigestMD5_Response.Realm || this.m_Nonce != auth_SASL_DigestMD5_Response.Nonce;
						if (flag4)
						{
							return Encoding.UTF8.GetBytes("rspauth=\"\"");
						}
						this.m_UserName = auth_SASL_DigestMD5_Response.UserName;
						AUTH_e_UserInfo auth_e_UserInfo = this.OnGetUserInfo(auth_SASL_DigestMD5_Response.UserName);
						bool userExists = auth_e_UserInfo.UserExists;
						if (userExists)
						{
							bool flag5 = auth_SASL_DigestMD5_Response.Authenticate(auth_e_UserInfo.UserName, auth_e_UserInfo.Password);
							if (flag5)
							{
								this.m_IsAuthenticated = true;
								return Encoding.UTF8.GetBytes(auth_SASL_DigestMD5_Response.ToRspauthResponse(auth_e_UserInfo.UserName, auth_e_UserInfo.Password));
							}
						}
					}
					catch
					{
					}
					result = Encoding.UTF8.GetBytes("rspauth=\"\"");
				}
				else
				{
					this.m_IsCompleted = true;
					result = null;
				}
			}
			return result;
		}

		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x0600168A RID: 5770 RVA: 0x0008C948 File Offset: 0x0008B948
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x0600168B RID: 5771 RVA: 0x0008C960 File Offset: 0x0008B960
		public override bool IsAuthenticated
		{
			get
			{
				return this.m_IsAuthenticated;
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x0600168C RID: 5772 RVA: 0x0008C978 File Offset: 0x0008B978
		public override string Name
		{
			get
			{
				return "DIGEST-MD5";
			}
		}

		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x0600168D RID: 5773 RVA: 0x0008C990 File Offset: 0x0008B990
		public override bool RequireSSL
		{
			get
			{
				return this.m_RequireSSL;
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x0600168E RID: 5774 RVA: 0x0008C9A8 File Offset: 0x0008B9A8
		// (set) Token: 0x0600168F RID: 5775 RVA: 0x0008C9C0 File Offset: 0x0008B9C0
		public string Realm
		{
			get
			{
				return this.m_Realm;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = "";
				}
				this.m_Realm = value;
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06001690 RID: 5776 RVA: 0x0008C9E8 File Offset: 0x0008B9E8
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1400009B RID: 155
		// (add) Token: 0x06001691 RID: 5777 RVA: 0x0008CA00 File Offset: 0x0008BA00
		// (remove) Token: 0x06001692 RID: 5778 RVA: 0x0008CA38 File Offset: 0x0008BA38
		
		public event EventHandler<AUTH_e_UserInfo> GetUserInfo = null;

		// Token: 0x06001693 RID: 5779 RVA: 0x0008CA70 File Offset: 0x0008BA70
		private AUTH_e_UserInfo OnGetUserInfo(string userName)
		{
			AUTH_e_UserInfo auth_e_UserInfo = new AUTH_e_UserInfo(userName);
			bool flag = this.GetUserInfo != null;
			if (flag)
			{
				this.GetUserInfo(this, auth_e_UserInfo);
			}
			return auth_e_UserInfo;
		}

		// Token: 0x04000907 RID: 2311
		private bool m_IsCompleted = false;

		// Token: 0x04000908 RID: 2312
		private bool m_IsAuthenticated = false;

		// Token: 0x04000909 RID: 2313
		private bool m_RequireSSL = false;

		// Token: 0x0400090A RID: 2314
		private string m_Realm = "";

		// Token: 0x0400090B RID: 2315
		private string m_Nonce = "";

		// Token: 0x0400090C RID: 2316
		private string m_UserName = "";

		// Token: 0x0400090D RID: 2317
		private int m_State = 0;
	}
}
