using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200009A RID: 154
	public class SIP_ResponseReceivedEventArgs : EventArgs
	{
		// Token: 0x060005AA RID: 1450 RVA: 0x00020346 File Offset: 0x0001F346
		internal SIP_ResponseReceivedEventArgs(SIP_Stack stack, SIP_ClientTransaction transaction, SIP_Response response)
		{
			this.m_pStack = stack;
			this.m_pResponse = response;
			this.m_pTransaction = transaction;
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x0002037C File Offset: 0x0001F37C
		public SIP_Response Response
		{
			get
			{
				return this.m_pResponse;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x00020394 File Offset: 0x0001F394
		public SIP_ClientTransaction ClientTransaction
		{
			get
			{
				return this.m_pTransaction;
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x000203AC File Offset: 0x0001F3AC
		public SIP_Dialog Dialog
		{
			get
			{
				return this.m_pStack.TransactionLayer.MatchDialog(this.m_pResponse);
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x000203D4 File Offset: 0x0001F3D4
		public SIP_Dialog GetOrCreateDialog
		{
			get
			{
				bool flag = !SIP_Utils.MethodCanEstablishDialog(this.m_pTransaction.Method);
				if (flag)
				{
					throw new InvalidOperationException("Request method '" + this.m_pTransaction.Method + "' can't establish dialog.");
				}
				bool flag2 = this.m_pResponse.To.Tag == null;
				if (flag2)
				{
					throw new InvalidOperationException("Request To-Tag is missing.");
				}
				return this.m_pStack.TransactionLayer.GetOrCreateDialog(this.m_pTransaction, this.m_pResponse);
			}
		}

		// Token: 0x0400021E RID: 542
		private SIP_Stack m_pStack = null;

		// Token: 0x0400021F RID: 543
		private SIP_Response m_pResponse = null;

		// Token: 0x04000220 RID: 544
		private SIP_ClientTransaction m_pTransaction = null;
	}
}
