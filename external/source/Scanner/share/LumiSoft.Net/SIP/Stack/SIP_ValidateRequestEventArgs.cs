using System;
using System.Net;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000098 RID: 152
	public class SIP_ValidateRequestEventArgs : EventArgs
	{
		// Token: 0x0600059D RID: 1437 RVA: 0x0002017B File Offset: 0x0001F17B
		public SIP_ValidateRequestEventArgs(SIP_Request request, IPEndPoint remoteEndpoint)
		{
			this.m_pRequest = request;
			this.m_pRemoteEndPoint = remoteEndpoint;
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x000201A8 File Offset: 0x0001F1A8
		public SIP_Request Request
		{
			get
			{
				return this.m_pRequest;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x000201C0 File Offset: 0x0001F1C0
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.m_pRemoteEndPoint;
			}
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x060005A0 RID: 1440 RVA: 0x000201D8 File Offset: 0x0001F1D8
		// (set) Token: 0x060005A1 RID: 1441 RVA: 0x000201F0 File Offset: 0x0001F1F0
		public string ResponseCode
		{
			get
			{
				return this.m_ResponseCode;
			}
			set
			{
				this.m_ResponseCode = value;
			}
		}

		// Token: 0x04000216 RID: 534
		private SIP_Request m_pRequest = null;

		// Token: 0x04000217 RID: 535
		private IPEndPoint m_pRemoteEndPoint = null;

		// Token: 0x04000218 RID: 536
		private string m_ResponseCode = null;
	}
}
