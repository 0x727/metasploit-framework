using System;
using System.Net;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x02000244 RID: 580
	public class FTP_Server : TCP_Server<FTP_Session>
	{
		// Token: 0x060014D8 RID: 5336 RVA: 0x00081C9A File Offset: 0x00080C9A
		public FTP_Server()
		{
			base.SessionIdleTimeout = 3600;
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00081CD5 File Offset: 0x00080CD5
		protected override void OnMaxConnectionsExceeded(FTP_Session session)
		{
			session.TcpStream.WriteLine("500 Client host rejected: too many connections, please try again later.");
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00081CE9 File Offset: 0x00080CE9
		protected override void OnMaxConnectionsPerIPExceeded(FTP_Session session)
		{
			session.TcpStream.WriteLine("500 Client host rejected: too many connections from your IP(" + session.RemoteEndPoint.Address + "), please try again later.");
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x060014DB RID: 5339 RVA: 0x00081D14 File Offset: 0x00080D14
		// (set) Token: 0x060014DC RID: 5340 RVA: 0x00081D48 File Offset: 0x00080D48
		public string GreetingText
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_GreetingText;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_GreetingText = value;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x060014DD RID: 5341 RVA: 0x00081D7C File Offset: 0x00080D7C
		// (set) Token: 0x060014DE RID: 5342 RVA: 0x00081DB0 File Offset: 0x00080DB0
		public int MaxBadCommands
		{
			get
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_MaxBadCommands;
			}
			set
			{
				bool isDisposed = base.IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = value < 0;
				if (flag)
				{
					throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
				}
				this.m_MaxBadCommands = value;
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x00081DF8 File Offset: 0x00080DF8
		// (set) Token: 0x060014E0 RID: 5344 RVA: 0x00081E10 File Offset: 0x00080E10
		public IPAddress PassivePublicIP
		{
			get
			{
				return this.m_pPassivePublicIP;
			}
			set
			{
				this.m_pPassivePublicIP = value;
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x060014E1 RID: 5345 RVA: 0x00081E1C File Offset: 0x00080E1C
		// (set) Token: 0x060014E2 RID: 5346 RVA: 0x00081E34 File Offset: 0x00080E34
		public int PassiveStartPort
		{
			get
			{
				return this.m_PassiveStartPort;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("Valu must be > 0 !");
				}
				this.m_PassiveStartPort = value;
			}
		}

		// Token: 0x04000825 RID: 2085
		private string m_GreetingText = "";

		// Token: 0x04000826 RID: 2086
		private int m_MaxBadCommands = 30;

		// Token: 0x04000827 RID: 2087
		private IPAddress m_pPassivePublicIP = null;

		// Token: 0x04000828 RID: 2088
		private int m_PassiveStartPort = 20000;
	}
}
