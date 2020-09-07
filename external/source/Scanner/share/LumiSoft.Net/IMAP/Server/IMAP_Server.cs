using System;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000227 RID: 551
	public class IMAP_Server : TCP_Server<IMAP_Session>
	{
		// Token: 0x0600139E RID: 5022 RVA: 0x000781B7 File Offset: 0x000771B7
		protected override void OnMaxConnectionsExceeded(IMAP_Session session)
		{
			session.TcpStream.WriteLine("* NO Client host rejected: too many connections, please try again later.");
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x000781CB File Offset: 0x000771CB
		protected override void OnMaxConnectionsPerIPExceeded(IMAP_Session session)
		{
			session.TcpStream.WriteLine("* NO Client host rejected: too many connections from your IP(" + session.RemoteEndPoint.Address + "), please try again later.");
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x060013A0 RID: 5024 RVA: 0x000781F4 File Offset: 0x000771F4
		// (set) Token: 0x060013A1 RID: 5025 RVA: 0x00078228 File Offset: 0x00077228
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

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x060013A2 RID: 5026 RVA: 0x0007825C File Offset: 0x0007725C
		// (set) Token: 0x060013A3 RID: 5027 RVA: 0x00078290 File Offset: 0x00077290
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

		// Token: 0x040007BD RID: 1981
		private string m_GreetingText = "";

		// Token: 0x040007BE RID: 1982
		private int m_MaxBadCommands = 30;
	}
}
