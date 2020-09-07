using System;
using System.Net;
using System.Security.Principal;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.TCP
{
	// Token: 0x0200002F RID: 47
	public abstract class TCP_Session : IDisposable
	{
		// Token: 0x06000154 RID: 340 RVA: 0x00009954 File Offset: 0x00008954
		public TCP_Session()
		{
		}

		// Token: 0x06000155 RID: 341
		public abstract void Dispose();

		// Token: 0x06000156 RID: 342
		public abstract void Disconnect();

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000157 RID: 343
		public abstract bool IsConnected { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000158 RID: 344
		public abstract string ID { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000159 RID: 345
		public abstract DateTime ConnectTime { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600015A RID: 346
		public abstract DateTime LastActivity { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600015B RID: 347
		public abstract IPEndPoint LocalEndPoint { get; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600015C RID: 348
		public abstract IPEndPoint RemoteEndPoint { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00009960 File Offset: 0x00008960
		public virtual bool IsSecureConnection
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600015E RID: 350 RVA: 0x00009974 File Offset: 0x00008974
		public bool IsAuthenticated
		{
			get
			{
				return this.AuthenticatedUserIdentity != null;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00009990 File Offset: 0x00008990
		public virtual GenericIdentity AuthenticatedUserIdentity
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000160 RID: 352
		public abstract SmartStream TcpStream { get; }
	}
}
