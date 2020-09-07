using System;
using System.Security.Cryptography;
using System.Text;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000262 RID: 610
	public class AUTH_SASL_Client_CramMd5 : AUTH_SASL_Client
	{
		// Token: 0x060015EA RID: 5610 RVA: 0x00088EF0 File Offset: 0x00087EF0
		public AUTH_SASL_Client_CramMd5(string userName, string password)
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

		// Token: 0x060015EB RID: 5611 RVA: 0x00088F78 File Offset: 0x00087F78
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
			if (flag2)
			{
				this.m_State++;
				this.m_IsCompleted = true;
				HMACMD5 hmacmd = new HMACMD5(Encoding.UTF8.GetBytes(this.m_Password));
				string str = Net_Utils.ToHex(hmacmd.ComputeHash(serverResponse)).ToLower();
				return Encoding.UTF8.GetBytes(this.m_UserName + " " + str);
			}
			throw new InvalidOperationException("Authentication is completed.");
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060015EC RID: 5612 RVA: 0x0008902C File Offset: 0x0008802C
		public override bool IsCompleted
		{
			get
			{
				return this.m_IsCompleted;
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060015ED RID: 5613 RVA: 0x00089044 File Offset: 0x00088044
		public override string Name
		{
			get
			{
				return "CRAM-MD5";
			}
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060015EE RID: 5614 RVA: 0x0008905C File Offset: 0x0008805C
		public override string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x040008B3 RID: 2227
		private bool m_IsCompleted = false;

		// Token: 0x040008B4 RID: 2228
		private int m_State = 0;

		// Token: 0x040008B5 RID: 2229
		private string m_UserName = null;

		// Token: 0x040008B6 RID: 2230
		private string m_Password = null;
	}
}
