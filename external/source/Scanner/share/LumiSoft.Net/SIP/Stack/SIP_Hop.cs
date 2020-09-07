using System;
using System.Net;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008A RID: 138
	public class SIP_Hop
	{
		// Token: 0x0600051E RID: 1310 RVA: 0x0001A970 File Offset: 0x00019970
		public SIP_Hop(IPEndPoint ep, string transport)
		{
			bool flag = ep == null;
			if (flag)
			{
				throw new ArgumentNullException("ep");
			}
			bool flag2 = transport == null;
			if (flag2)
			{
				throw new ArgumentNullException("transport");
			}
			bool flag3 = transport == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'transport' value must be specified.");
			}
			this.m_pEndPoint = ep;
			this.m_Transport = transport;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001A9E8 File Offset: 0x000199E8
		public SIP_Hop(IPAddress ip, int port, string transport)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool flag2 = port < 1;
			if (flag2)
			{
				throw new ArgumentException("Argument 'port' value must be >= 1.");
			}
			bool flag3 = transport == null;
			if (flag3)
			{
				throw new ArgumentNullException("transport");
			}
			bool flag4 = transport == "";
			if (flag4)
			{
				throw new ArgumentException("Argument 'transport' value must be specified.");
			}
			this.m_pEndPoint = new IPEndPoint(ip, port);
			this.m_Transport = transport;
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0001AA7C File Offset: 0x00019A7C
		public IPEndPoint EndPoint
		{
			get
			{
				return this.m_pEndPoint;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x0001AA94 File Offset: 0x00019A94
		public IPAddress IP
		{
			get
			{
				return this.m_pEndPoint.Address;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000522 RID: 1314 RVA: 0x0001AAB4 File Offset: 0x00019AB4
		public int Port
		{
			get
			{
				return this.m_pEndPoint.Port;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000523 RID: 1315 RVA: 0x0001AAD4 File Offset: 0x00019AD4
		public string Transport
		{
			get
			{
				return this.m_Transport;
			}
		}

		// Token: 0x040001AF RID: 431
		private IPEndPoint m_pEndPoint = null;

		// Token: 0x040001B0 RID: 432
		private string m_Transport = "";
	}
}
