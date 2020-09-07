using System;
using LumiSoft.Net.TCP;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000EE RID: 238
	public class POP3_Server : TCP_Server<POP3_Session>
	{
		// Token: 0x06000987 RID: 2439 RVA: 0x00039B7B File Offset: 0x00038B7B
		protected override void OnMaxConnectionsExceeded(POP3_Session session)
		{
			session.TcpStream.WriteLine("-ERR Client host rejected: too many connections, please try again later.");
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00039B8F File Offset: 0x00038B8F
		protected override void OnMaxConnectionsPerIPExceeded(POP3_Session session)
		{
			session.TcpStream.WriteLine("-ERR Client host rejected: too many connections from your IP(" + session.RemoteEndPoint.Address + "), please try again later.");
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000989 RID: 2441 RVA: 0x00039BB8 File Offset: 0x00038BB8
		// (set) Token: 0x0600098A RID: 2442 RVA: 0x00039BEC File Offset: 0x00038BEC
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

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x00039C20 File Offset: 0x00038C20
		// (set) Token: 0x0600098C RID: 2444 RVA: 0x00039C54 File Offset: 0x00038C54
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

		// Token: 0x0400043B RID: 1083
		private string m_GreetingText = "";

		// Token: 0x0400043C RID: 1084
		private int m_MaxBadCommands = 30;
	}
}
