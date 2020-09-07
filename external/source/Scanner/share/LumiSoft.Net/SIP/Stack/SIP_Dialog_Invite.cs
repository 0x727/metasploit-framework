using System;
using System.Diagnostics;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000087 RID: 135
	public class SIP_Dialog_Invite : SIP_Dialog
	{
		// Token: 0x060004ED RID: 1261 RVA: 0x00019253 File Offset: 0x00018253
		internal SIP_Dialog_Invite()
		{
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001927C File Offset: 0x0001827C
		protected internal override void Init(SIP_Stack stack, SIP_Transaction transaction, SIP_Response response)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			bool flag2 = transaction == null;
			if (flag2)
			{
				throw new ArgumentNullException("transaction");
			}
			bool flag3 = response == null;
			if (flag3)
			{
				throw new ArgumentNullException("response");
			}
			base.Init(stack, transaction, response);
			bool flag4 = transaction is SIP_ServerTransaction;
			if (flag4)
			{
				bool flag5 = response.StatusCodeType == SIP_StatusCodeType.Success;
				if (flag5)
				{
					base.SetState(SIP_DialogState.Early, false);
				}
				else
				{
					bool flag6 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
					if (!flag6)
					{
						throw new ArgumentException("Argument 'response' has invalid status code, 1xx - 2xx is only allowed.");
					}
					base.SetState(SIP_DialogState.Early, false);
					this.m_pActiveInvite = transaction;
					this.m_pActiveInvite.StateChanged += delegate(object s, EventArgs a)
					{
						bool flag9 = this.m_pActiveInvite != null && this.m_pActiveInvite.State == SIP_TransactionState.Terminated;
						if (flag9)
						{
							this.m_pActiveInvite = null;
							bool flag10 = base.State == SIP_DialogState.Early;
							if (flag10)
							{
								base.SetState(SIP_DialogState.Confirmed, true);
								this.Terminate("ACK was not received for initial INVITE 2xx response.", true);
							}
							else
							{
								bool flag11 = base.State == SIP_DialogState.Terminating;
								if (flag11)
								{
									base.SetState(SIP_DialogState.Confirmed, false);
									this.Terminate(this.m_TerminateReason, true);
								}
							}
						}
					};
				}
			}
			else
			{
				bool flag7 = response.StatusCodeType == SIP_StatusCodeType.Success;
				if (flag7)
				{
					base.SetState(SIP_DialogState.Confirmed, false);
				}
				else
				{
					bool flag8 = response.StatusCodeType == SIP_StatusCodeType.Provisional;
					if (!flag8)
					{
						throw new ArgumentException("Argument 'response' has invalid status code, 1xx - 2xx is only allowed.");
					}
					base.SetState(SIP_DialogState.Early, false);
					this.m_pActiveInvite = transaction;
					this.m_pActiveInvite.StateChanged += delegate(object s, EventArgs a)
					{
						bool flag9 = this.m_pActiveInvite != null && this.m_pActiveInvite.State == SIP_TransactionState.Terminated;
						if (flag9)
						{
							this.m_pActiveInvite = null;
						}
					};
					((SIP_ClientTransaction)transaction).ResponseReceived += delegate(object s, SIP_ResponseReceivedEventArgs a)
					{
						bool flag9 = a.Response.StatusCodeType == SIP_StatusCodeType.Success;
						if (flag9)
						{
							base.SetState(SIP_DialogState.Confirmed, true);
						}
					};
				}
			}
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x000193C4 File Offset: 0x000183C4
		public override void Dispose()
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_DialogState.Disposed;
				if (!flag2)
				{
					this.m_pActiveInvite = null;
					base.Dispose();
				}
			}
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00019424 File Offset: 0x00018424
		public void Terminate(string reason, bool sendBye)
		{
			object syncRoot = base.SyncRoot;
			lock (syncRoot)
			{
				bool flag2 = base.State == SIP_DialogState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = base.State == SIP_DialogState.Terminating || base.State == SIP_DialogState.Terminated;
				if (!flag3)
				{
					this.m_TerminateReason = reason;
					if (sendBye)
					{
						bool flag4 = (base.State == SIP_DialogState.Early && this.m_pActiveInvite is SIP_ClientTransaction) || base.State == SIP_DialogState.Confirmed;
						if (flag4)
						{
							base.SetState(SIP_DialogState.Terminating, true);
							SIP_Request sip_Request = base.CreateRequest("BYE");
							bool flag5 = !string.IsNullOrEmpty(reason);
							if (flag5)
							{
								SIP_t_ReasonValue sip_t_ReasonValue = new SIP_t_ReasonValue();
								sip_t_ReasonValue.Protocol = "SIP";
								sip_t_ReasonValue.Text = reason;
								sip_Request.Reason.Add(sip_t_ReasonValue.ToStringValue());
							}
							SIP_RequestSender sip_RequestSender = base.CreateRequestSender(sip_Request);
							sip_RequestSender.Completed += delegate(object s, EventArgs a)
							{
								base.SetState(SIP_DialogState.Terminated, true);
							};
							sip_RequestSender.Start();
						}
						else
						{
							bool flag6 = this.m_pActiveInvite != null && this.m_pActiveInvite.FinalResponse == null;
							if (flag6)
							{
								base.Stack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pActiveInvite.Request);
								base.SetState(SIP_DialogState.Terminated, true);
							}
							else
							{
								base.SetState(SIP_DialogState.Terminating, true);
							}
						}
					}
					else
					{
						base.SetState(SIP_DialogState.Terminated, true);
					}
				}
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x000195D0 File Offset: 0x000185D0
		protected internal override bool ProcessRequest(SIP_RequestReceivedEventArgs e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			bool flag2 = base.ProcessRequest(e);
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = e.Request.RequestLine.Method == "ACK";
				if (flag3)
				{
					bool flag4 = base.State == SIP_DialogState.Early;
					if (flag4)
					{
						base.SetState(SIP_DialogState.Confirmed, true);
					}
					else
					{
						bool flag5 = base.State == SIP_DialogState.Terminating;
						if (flag5)
						{
							base.SetState(SIP_DialogState.Confirmed, false);
							this.Terminate(this.m_TerminateReason, true);
						}
					}
				}
				else
				{
					bool flag6 = e.Request.RequestLine.Method == "BYE";
					if (flag6)
					{
						e.ServerTransaction.SendResponse(base.Stack.CreateResponse(SIP_ResponseCodes.x200_Ok, e.Request));
						this.m_IsTerminatedByRemoteParty = true;
						this.OnTerminatedByRemoteParty(e);
						base.SetState(SIP_DialogState.Terminated, true);
						return true;
					}
					bool flag7 = e.Request.RequestLine.Method == "INVITE";
					if (flag7)
					{
						bool hasPendingInvite = this.HasPendingInvite;
						if (hasPendingInvite)
						{
							e.ServerTransaction.SendResponse(base.Stack.CreateResponse(SIP_ResponseCodes.x491_Request_Pending, e.Request));
							return true;
						}
					}
					else
					{
						bool flag8 = SIP_Utils.MethodCanEstablishDialog(e.Request.RequestLine.Method);
						if (flag8)
						{
							e.ServerTransaction.SendResponse(base.Stack.CreateResponse(SIP_ResponseCodes.x603_Decline + " : New dialog usages in dialog not allowed (RFC 5057).", e.Request));
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00019778 File Offset: 0x00018778
		public bool HasPendingInvite
		{
			get
			{
				bool flag = base.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (SIP_Transaction sip_Transaction in base.Transactions)
				{
					bool flag2 = sip_Transaction.State == SIP_TransactionState.Calling || sip_Transaction.State == SIP_TransactionState.Proceeding;
					if (flag2)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x000197EC File Offset: 0x000187EC
		public bool IsTerminatedByRemoteParty
		{
			get
			{
				bool flag = base.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsTerminatedByRemoteParty;
			}
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x060004F4 RID: 1268 RVA: 0x00019824 File Offset: 0x00018824
		// (remove) Token: 0x060004F5 RID: 1269 RVA: 0x0001985C File Offset: 0x0001885C
		
		public event EventHandler<SIP_RequestReceivedEventArgs> TerminatedByRemoteParty = null;

		// Token: 0x060004F6 RID: 1270 RVA: 0x00019894 File Offset: 0x00018894
		private void OnTerminatedByRemoteParty(SIP_RequestReceivedEventArgs bye)
		{
			bool flag = this.TerminatedByRemoteParty != null;
			if (flag)
			{
				this.TerminatedByRemoteParty(this, bye);
			}
		}

		// Token: 0x04000199 RID: 409
		private SIP_Transaction m_pActiveInvite = null;

		// Token: 0x0400019A RID: 410
		private bool m_IsTerminatedByRemoteParty = false;

		// Token: 0x0400019B RID: 411
		private string m_TerminateReason = null;
	}
}
