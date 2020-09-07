using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000270 RID: 624
	public class AUTH_SASL_ServerMechanism_CramMd5 : AUTH_SASL_ServerMechanism
	{
		// Token: 0x0600167B RID: 5755 RVA: 0x0008C420 File Offset: 0x0008B420
		public AUTH_SASL_ServerMechanism_CramMd5(bool requireSSL)
		{
			this.m_RequireSSL = requireSSL;
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x0008C475 File Offset: 0x0008B475
		public override void Reset()
		{
			this.m_IsCompleted = false;
			this.m_IsAuthenticated = false;
			this.m_UserName = "";
			this.m_State = 0;
			this.m_Key = "";
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x0008C4A4 File Offset: 0x0008B4A4
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
				this.m_Key = "<" + Guid.NewGuid().ToString() + "@host>";
				result = Encoding.UTF8.GetBytes(this.m_Key);
			}
			else
			{
				string[] array = Encoding.UTF8.GetString(clientResponse).Split(new char[]
				{
					' '
				});
				bool flag3 = array.Length == 2 && !string.IsNullOrEmpty(array[0]);
				if (flag3)
				{
					this.m_UserName = array[0];
					AUTH_e_UserInfo auth_e_UserInfo = this.OnGetUserInfo(array[0]);
					bool userExists = auth_e_UserInfo.UserExists;
					if (userExists)
					{
						string a = Net_Utils.ToHex(this.HmacMd5(this.m_Key, auth_e_UserInfo.Password));
						bool flag4 = a == array[1];
						if (flag4)
						{
							this.m_IsAuthenticated = true;
						}
					}
				}
				this.m_IsCompleted = true;
				result = null;
			}
			return result;
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x0008C5C4 File Offset: 0x0008B5C4
		private byte[] HmacMd5(string hashKey, string text)
		{
			HMACMD5 hmacmd = new HMACMD5(Encoding.Default.GetBytes(text));
			return hmacmd.ComputeHash(Encoding.ASCII.GetBytes(hashKey));
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x0600167F RID: 5759 RVA: 0x0008C5F8 File Offset: 0x0008B5F8
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06001680 RID: 5760 RVA: 0x0008C610 File Offset: 0x0008B610
		public override bool IsAuthenticated
		{
			get
			{
				return this.m_IsAuthenticated;
			}
		}

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06001681 RID: 5761 RVA: 0x0008C628 File Offset: 0x0008B628
		public override string Name
		{
			get
			{
				return "CRAM-MD5";
			}
		}

		// Token: 0x17000764 RID: 1892
		// (get) Token: 0x06001682 RID: 5762 RVA: 0x0008C640 File Offset: 0x0008B640
		public override bool RequireSSL
		{
			get
			{
				return this.m_RequireSSL;
			}
		}

		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x0008C658 File Offset: 0x0008B658
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1400009A RID: 154
		// (add) Token: 0x06001684 RID: 5764 RVA: 0x0008C670 File Offset: 0x0008B670
		// (remove) Token: 0x06001685 RID: 5765 RVA: 0x0008C6A8 File Offset: 0x0008B6A8
		
		public event EventHandler<AUTH_e_UserInfo> GetUserInfo = null;

		// Token: 0x06001686 RID: 5766 RVA: 0x0008C6E0 File Offset: 0x0008B6E0
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

		// Token: 0x04000900 RID: 2304
		private bool m_IsCompleted = false;

		// Token: 0x04000901 RID: 2305
		private bool m_IsAuthenticated = false;

		// Token: 0x04000902 RID: 2306
		private bool m_RequireSSL = false;

		// Token: 0x04000903 RID: 2307
		private string m_UserName = "";

		// Token: 0x04000904 RID: 2308
		private int m_State = 0;

		// Token: 0x04000905 RID: 2309
		private string m_Key = "";
	}
}
