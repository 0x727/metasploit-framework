using System;

namespace LumiSoft.Net.SIP.UA
{
	// Token: 0x02000080 RID: 128
	[Obsolete("Use SIP stack instead.")]
	public class SIP_UA_Call_EventArgs : EventArgs
	{
		// Token: 0x060004BD RID: 1213 RVA: 0x0001803C File Offset: 0x0001703C
		public SIP_UA_Call_EventArgs(SIP_UA_Call call)
		{
			bool flag = call == null;
			if (flag)
			{
				throw new ArgumentNullException("call");
			}
			this.m_pCall = call;
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x00018074 File Offset: 0x00017074
		public SIP_UA_Call Call
		{
			get
			{
				return this.m_pCall;
			}
		}

		// Token: 0x0400017D RID: 381
		private SIP_UA_Call m_pCall = null;
	}
}
