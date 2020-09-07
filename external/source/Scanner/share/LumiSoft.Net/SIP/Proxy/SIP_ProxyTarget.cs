using System;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A8 RID: 168
	public class SIP_ProxyTarget
	{
		// Token: 0x0600068B RID: 1675 RVA: 0x0002700B File Offset: 0x0002600B
		public SIP_ProxyTarget(SIP_Uri targetUri) : this(targetUri, null)
		{
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x00027018 File Offset: 0x00026018
		public SIP_ProxyTarget(SIP_Uri targetUri, SIP_Flow flow)
		{
			bool flag = targetUri == null;
			if (flag)
			{
				throw new ArgumentNullException("targetUri");
			}
			this.m_pTargetUri = targetUri;
			this.m_pFlow = flow;
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600068D RID: 1677 RVA: 0x00027060 File Offset: 0x00026060
		public SIP_Uri TargetUri
		{
			get
			{
				return this.m_pTargetUri;
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600068E RID: 1678 RVA: 0x00027078 File Offset: 0x00026078
		public SIP_Flow Flow
		{
			get
			{
				return this.m_pFlow;
			}
		}

		// Token: 0x040002B6 RID: 694
		private SIP_Uri m_pTargetUri = null;

		// Token: 0x040002B7 RID: 695
		private SIP_Flow m_pFlow = null;
	}
}
