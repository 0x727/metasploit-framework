using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200008D RID: 141
	public class SIP_ResponseSentEventArgs : EventArgs
	{
		// Token: 0x06000540 RID: 1344 RVA: 0x0001CB64 File Offset: 0x0001BB64
		public SIP_ResponseSentEventArgs(SIP_ServerTransaction transaction, SIP_Response response)
		{
			bool flag = transaction == null;
			if (flag)
			{
				throw new ArgumentNullException("transaction");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pTransaction = transaction;
			this.m_pResponse = response;
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x0001CBC0 File Offset: 0x0001BBC0
		public SIP_ServerTransaction ServerTransaction
		{
			get
			{
				return this.m_pTransaction;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x0001CBD8 File Offset: 0x0001BBD8
		public SIP_Response Response
		{
			get
			{
				return this.m_pResponse;
			}
		}

		// Token: 0x040001BE RID: 446
		private SIP_ServerTransaction m_pTransaction = null;

		// Token: 0x040001BF RID: 447
		private SIP_Response m_pResponse = null;
	}
}
