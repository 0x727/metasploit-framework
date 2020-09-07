using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.UA
{
	// Token: 0x0200007D RID: 125
	[Obsolete("Use SIP stack instead.")]
	public class SIP_UA : IDisposable
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x00016DA4 File Offset: 0x00015DA4
		public SIP_UA()
		{
			this.m_pStack = new SIP_Stack();
			this.m_pStack.RequestReceived += this.m_pStack_RequestReceived;
			this.m_pCalls = new List<SIP_UA_Call>();
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00016E18 File Offset: 0x00015E18
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					foreach (SIP_UA_Call sip_UA_Call in this.m_pCalls.ToArray())
					{
						sip_UA_Call.Terminate();
					}
					DateTime now = DateTime.Now;
					while (this.m_pCalls.Count > 0)
					{
						Thread.Sleep(500);
						bool flag2 = (DateTime.Now - now).Seconds > 15;
						if (flag2)
						{
							break;
						}
					}
					this.m_IsDisposed = true;
					this.RequestReceived = null;
					this.IncomingCall = null;
					this.m_pStack.Dispose();
					this.m_pStack = null;
				}
			}
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00016F0C File Offset: 0x00015F0C
		private void m_pStack_RequestReceived(object sender, SIP_RequestReceivedEventArgs e)
		{
			bool flag = e.Request.RequestLine.Method == "CANCEL";
			if (flag)
			{
				SIP_ServerTransaction sip_ServerTransaction = this.m_pStack.TransactionLayer.MatchCancelToTransaction(e.Request);
				bool flag2 = sip_ServerTransaction != null;
				if (flag2)
				{
					sip_ServerTransaction.Cancel();
					e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x200_Ok, e.Request));
				}
				else
				{
					e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x481_Call_Transaction_Does_Not_Exist, e.Request));
				}
			}
			else
			{
				bool flag3 = e.Request.RequestLine.Method == "BYE";
				if (flag3)
				{
					SIP_Dialog sip_Dialog = this.m_pStack.TransactionLayer.MatchDialog(e.Request);
					bool flag4 = sip_Dialog != null;
					if (flag4)
					{
						e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x200_Ok, e.Request));
						sip_Dialog.Terminate();
					}
					else
					{
						e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x481_Call_Transaction_Does_Not_Exist, e.Request));
					}
				}
				else
				{
					bool flag5 = e.Request.RequestLine.Method == "INVITE";
					if (flag5)
					{
						e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x100_Trying, e.Request));
						SIP_UA_Call sip_UA_Call = new SIP_UA_Call(this, e.ServerTransaction);
						sip_UA_Call.StateChanged += this.Call_StateChanged;
						this.m_pCalls.Add(sip_UA_Call);
						this.OnIncomingCall(sip_UA_Call);
					}
					else
					{
						this.OnRequestReceived(e);
					}
				}
			}
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x000170D4 File Offset: 0x000160D4
		private void Call_StateChanged(object sender, EventArgs e)
		{
			SIP_UA_Call sip_UA_Call = (SIP_UA_Call)sender;
			bool flag = sip_UA_Call.State == SIP_UA_CallState.Terminated;
			if (flag)
			{
				this.m_pCalls.Remove(sip_UA_Call);
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x00017108 File Offset: 0x00016108
		public SIP_UA_Call CreateCall(SIP_Request invite)
		{
			bool flag = invite == null;
			if (flag)
			{
				throw new ArgumentNullException("invite");
			}
			bool flag2 = invite.RequestLine.Method != "INVITE";
			if (flag2)
			{
				throw new ArgumentException("Argument 'invite' is not INVITE request.");
			}
			object pLock = this.m_pLock;
			SIP_UA_Call result;
			lock (pLock)
			{
				SIP_UA_Call sip_UA_Call = new SIP_UA_Call(this, invite);
				sip_UA_Call.StateChanged += this.Call_StateChanged;
				this.m_pCalls.Add(sip_UA_Call);
				result = sip_UA_Call;
			}
			return result;
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000496 RID: 1174 RVA: 0x000171B0 File Offset: 0x000161B0
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x000171C8 File Offset: 0x000161C8
		public SIP_Stack Stack
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pStack;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x000171FC File Offset: 0x000161FC
		public SIP_UA_Call[] Calls
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pCalls.ToArray();
			}
		}

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000499 RID: 1177 RVA: 0x00017238 File Offset: 0x00016238
		// (remove) Token: 0x0600049A RID: 1178 RVA: 0x00017270 File Offset: 0x00016270
		
		public event EventHandler<SIP_RequestReceivedEventArgs> RequestReceived = null;

		// Token: 0x0600049B RID: 1179 RVA: 0x000172A8 File Offset: 0x000162A8
		protected void OnRequestReceived(SIP_RequestReceivedEventArgs request)
		{
			bool flag = this.RequestReceived != null;
			if (flag)
			{
				this.RequestReceived(this, request);
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600049C RID: 1180 RVA: 0x000172D4 File Offset: 0x000162D4
		// (remove) Token: 0x0600049D RID: 1181 RVA: 0x0001730C File Offset: 0x0001630C
		
		public event EventHandler<SIP_UA_Call_EventArgs> IncomingCall = null;

		// Token: 0x0600049E RID: 1182 RVA: 0x00017344 File Offset: 0x00016344
		private void OnIncomingCall(SIP_UA_Call call)
		{
			bool flag = this.IncomingCall != null;
			if (flag)
			{
				this.IncomingCall(this, new SIP_UA_Call_EventArgs(call));
			}
		}

		// Token: 0x0400015D RID: 349
		private bool m_IsDisposed = false;

		// Token: 0x0400015E RID: 350
		private SIP_Stack m_pStack = null;

		// Token: 0x0400015F RID: 351
		private List<SIP_UA_Call> m_pCalls = null;

		// Token: 0x04000160 RID: 352
		private object m_pLock = new object();
	}
}
