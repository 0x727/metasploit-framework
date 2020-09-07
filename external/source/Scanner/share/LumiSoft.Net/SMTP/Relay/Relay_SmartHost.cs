using System;

namespace LumiSoft.Net.SMTP.Relay
{
	// Token: 0x02000149 RID: 329
	public class Relay_SmartHost
	{
		// Token: 0x06000D16 RID: 3350 RVA: 0x00051744 File Offset: 0x00050744
		public Relay_SmartHost(string host, int port) : this(host, port, SslMode.None, null, null)
		{
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x00051754 File Offset: 0x00050754
		public Relay_SmartHost(string host, int port, SslMode sslMode, string userName, string password)
		{
			bool flag = host == null;
			if (flag)
			{
				throw new ArgumentNullException("host");
			}
			bool flag2 = host == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'host' value must be specified.");
			}
			bool flag3 = port < 1;
			if (flag3)
			{
				throw new ArgumentException("Argument 'port' value is invalid.");
			}
			this.m_Host = host;
			this.m_Port = port;
			this.m_SslMode = sslMode;
			this.m_UserName = userName;
			this.m_Password = password;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x000517FC File Offset: 0x000507FC
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !(obj is Relay_SmartHost);
				if (flag2)
				{
					result = false;
				}
				else
				{
					Relay_SmartHost relay_SmartHost = (Relay_SmartHost)obj;
					bool flag3 = this.m_Host != relay_SmartHost.Host;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.m_Port != relay_SmartHost.Port;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = this.m_SslMode != relay_SmartHost.SslMode;
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = this.m_UserName != relay_SmartHost.UserName;
								if (flag6)
								{
									result = false;
								}
								else
								{
									bool flag7 = this.m_Password != relay_SmartHost.Password;
									result = !flag7;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x000518CC File Offset: 0x000508CC
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06000D1A RID: 3354 RVA: 0x000518E4 File Offset: 0x000508E4
		public string Host
		{
			get
			{
				return this.m_Host;
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06000D1B RID: 3355 RVA: 0x000518FC File Offset: 0x000508FC
		public int Port
		{
			get
			{
				return this.m_Port;
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x00051914 File Offset: 0x00050914
		public SslMode SslMode
		{
			get
			{
				return this.m_SslMode;
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06000D1D RID: 3357 RVA: 0x0005192C File Offset: 0x0005092C
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x00051944 File Offset: 0x00050944
		public string Password
		{
			get
			{
				return this.m_Password;
			}
		}

		// Token: 0x0400058E RID: 1422
		private string m_Host = "";

		// Token: 0x0400058F RID: 1423
		private int m_Port = 25;

		// Token: 0x04000590 RID: 1424
		private SslMode m_SslMode = SslMode.None;

		// Token: 0x04000591 RID: 1425
		private string m_UserName = null;

		// Token: 0x04000592 RID: 1426
		private string m_Password = null;
	}
}
