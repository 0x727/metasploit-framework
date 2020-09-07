using System;
using System.Net;

namespace LumiSoft.Net.STUN.Client
{
	// Token: 0x02000029 RID: 41
	public class STUN_Result
	{
		// Token: 0x06000142 RID: 322 RVA: 0x00009274 File Offset: 0x00008274
		public STUN_Result(STUN_NetType netType, IPEndPoint publicEndPoint)
		{
			this.m_NetType = netType;
			this.m_pPublicEndPoint = publicEndPoint;
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000929C File Offset: 0x0000829C
		public STUN_NetType NetType
		{
			get
			{
				return this.m_NetType;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000144 RID: 324 RVA: 0x000092B4 File Offset: 0x000082B4
		public IPEndPoint PublicEndPoint
		{
			get
			{
				return this.m_pPublicEndPoint;
			}
		}

		// Token: 0x0400008C RID: 140
		private STUN_NetType m_NetType = STUN_NetType.OpenInternet;

		// Token: 0x0400008D RID: 141
		private IPEndPoint m_pPublicEndPoint = null;
	}
}
