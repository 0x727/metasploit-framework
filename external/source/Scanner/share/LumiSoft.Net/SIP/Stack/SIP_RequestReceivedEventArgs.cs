using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000099 RID: 153
	public class SIP_RequestReceivedEventArgs : EventArgs
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x000201FA File Offset: 0x0001F1FA
		internal SIP_RequestReceivedEventArgs(SIP_Stack stack, SIP_Flow flow, SIP_Request request) : this(stack, flow, request, null)
		{
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00020208 File Offset: 0x0001F208
		internal SIP_RequestReceivedEventArgs(SIP_Stack stack, SIP_Flow flow, SIP_Request request, SIP_ServerTransaction transaction)
		{
			this.m_pStack = stack;
			this.m_pFlow = flow;
			this.m_pRequest = request;
			this.m_pTransaction = transaction;
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x00020260 File Offset: 0x0001F260
		public SIP_Flow Flow
		{
			get
			{
				return this.m_pFlow;
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00020278 File Offset: 0x0001F278
		public SIP_Request Request
		{
			get
			{
				return this.m_pRequest;
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x00020290 File Offset: 0x0001F290
		public SIP_ServerTransaction ServerTransaction
		{
			get
			{
				bool flag = this.m_pRequest.RequestLine.Method == "ACK";
				SIP_ServerTransaction result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this.m_pTransaction == null;
					if (flag2)
					{
						this.m_pTransaction = this.m_pStack.TransactionLayer.EnsureServerTransaction(this.m_pFlow, this.m_pRequest);
					}
					result = this.m_pTransaction;
				}
				return result;
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x000202FC File Offset: 0x0001F2FC
		public SIP_Dialog Dialog
		{
			get
			{
				return this.m_pStack.TransactionLayer.MatchDialog(this.m_pRequest);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x00020324 File Offset: 0x0001F324
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x0002033C File Offset: 0x0001F33C
		public bool IsHandled
		{
			get
			{
				return this.m_IsHandled;
			}
			set
			{
				this.m_IsHandled = true;
			}
		}

		// Token: 0x04000219 RID: 537
		private SIP_Stack m_pStack = null;

		// Token: 0x0400021A RID: 538
		private SIP_Flow m_pFlow = null;

		// Token: 0x0400021B RID: 539
		private SIP_Request m_pRequest = null;

		// Token: 0x0400021C RID: 540
		private SIP_ServerTransaction m_pTransaction = null;

		// Token: 0x0400021D RID: 541
		private bool m_IsHandled = false;
	}
}
