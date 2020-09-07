using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x0200009C RID: 156
	public abstract class SIP_Transaction : IDisposable
	{
		// Token: 0x060005B0 RID: 1456 RVA: 0x00020460 File Offset: 0x0001F460
		public SIP_Transaction(SIP_Stack stack, SIP_Flow flow, SIP_Request request)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			bool flag2 = flow == null;
			if (flag2)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag3 = request == null;
			if (flag3)
			{
				throw new ArgumentNullException("request");
			}
			this.m_pStack = stack;
			this.m_pFlow = flow;
			this.m_pRequest = request;
			this.m_Method = request.RequestLine.Method;
			this.m_CreateTime = DateTime.Now;
			this.m_pResponses = new List<SIP_Response>();
			SIP_t_ViaParm topMostValue = request.Via.GetTopMostValue();
			bool flag4 = topMostValue == null;
			if (flag4)
			{
				throw new ArgumentException("Via: header is missing !");
			}
			bool flag5 = topMostValue.Branch == null;
			if (flag5)
			{
				throw new ArgumentException("Via: header 'branch' parameter is missing !");
			}
			this.m_ID = topMostValue.Branch;
			bool flag6 = this is SIP_ServerTransaction;
			if (flag6)
			{
				string text = request.Via.GetTopMostValue().Branch + "-" + request.Via.GetTopMostValue().SentBy;
				bool flag7 = request.RequestLine.Method == "CANCEL";
				if (flag7)
				{
					text += "-CANCEL";
				}
				this.m_Key = text;
			}
			else
			{
				this.m_Key = this.m_ID + "-" + request.RequestLine.Method;
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00020640 File Offset: 0x0001F640
		public virtual void Dispose()
		{
			this.SetState(SIP_TransactionState.Disposed);
			this.OnDisposed();
			this.m_pStack = null;
			this.m_pFlow = null;
			this.m_pRequest = null;
			this.StateChanged = null;
			this.Disposed = null;
			this.TimedOut = null;
			this.TransportError = null;
		}

		// Token: 0x060005B2 RID: 1458
		public abstract void Cancel();

		// Token: 0x060005B3 RID: 1459 RVA: 0x00020690 File Offset: 0x0001F690
		protected void SetState(SIP_TransactionState state)
		{
			bool flag = this.Stack.Logger != null;
			if (flag)
			{
				this.Stack.Logger.AddText(this.ID, string.Concat(new string[]
				{
					"Transaction [branch='",
					this.ID,
					"';method='",
					this.Method,
					"';IsServer=",
					(this is SIP_ServerTransaction).ToString(),
					"] switched to '",
					state.ToString(),
					"' state."
				}));
			}
			this.m_State = state;
			this.OnStateChanged();
			bool flag2 = this.m_State == SIP_TransactionState.Terminated;
			if (flag2)
			{
				this.Dispose();
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0002075C File Offset: 0x0001F75C
		protected void AddResponse(SIP_Response response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			bool flag2 = this.m_pResponses.Count < 15 || response.StatusCode >= 200;
			if (flag2)
			{
				this.m_pResponses.Add(response);
			}
		}

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060005B5 RID: 1461 RVA: 0x000207B4 File Offset: 0x0001F7B4
		public object SyncRoot
		{
			get
			{
				return this.m_pLock;
			}
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x000207CC File Offset: 0x0001F7CC
		public bool IsDisposed
		{
			get
			{
				return this.m_State == SIP_TransactionState.Disposed;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060005B7 RID: 1463 RVA: 0x000207E8 File Offset: 0x0001F7E8
		public SIP_TransactionState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00020800 File Offset: 0x0001F800
		public SIP_Stack Stack
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pStack;
			}
		}

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x00020838 File Offset: 0x0001F838
		public SIP_Flow Flow
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pFlow;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00020870 File Offset: 0x0001F870
		public SIP_Request Request
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRequest;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x000208A8 File Offset: 0x0001F8A8
		public string Method
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_Method;
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x000208E0 File Offset: 0x0001F8E0
		public string ID
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_ID;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x00020918 File Offset: 0x0001F918
		public DateTime CreateTime
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CreateTime;
			}
		}

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x00020950 File Offset: 0x0001F950
		public SIP_Response[] Responses
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pResponses.ToArray();
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0002098C File Offset: 0x0001F98C
		public SIP_Response LastProvisionalResponse
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				for (int i = this.Responses.Length - 1; i > -1; i--)
				{
					bool flag2 = this.Responses[i].StatusCodeType == SIP_StatusCodeType.Provisional;
					if (flag2)
					{
						return this.Responses[i];
					}
				}
				return null;
			}
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x000209FC File Offset: 0x0001F9FC
		public SIP_Response FinalResponse
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (SIP_Response sip_Response in this.Responses)
				{
					bool flag2 = sip_Response.StatusCodeType > SIP_StatusCodeType.Provisional;
					if (flag2)
					{
						return sip_Response;
					}
				}
				return null;
			}
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x00020A64 File Offset: 0x0001FA64
		public bool HasProvisionalResponse
		{
			get
			{
				bool flag = this.State == SIP_TransactionState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				foreach (SIP_Response sip_Response in this.m_pResponses)
				{
					bool flag2 = sip_Response.StatusCodeType == SIP_StatusCodeType.Provisional;
					if (flag2)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060005C2 RID: 1474 RVA: 0x00020AF0 File Offset: 0x0001FAF0
		public SIP_Dialog Dialog
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x00020B04 File Offset: 0x0001FB04
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x00020B1C File Offset: 0x0001FB1C
		public object Tag
		{
			get
			{
				return this.m_pTag;
			}
			set
			{
				this.m_pTag = value;
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x00020B28 File Offset: 0x0001FB28
		internal string Key
		{
			get
			{
				return this.m_Key;
			}
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060005C6 RID: 1478 RVA: 0x00020B40 File Offset: 0x0001FB40
		// (remove) Token: 0x060005C7 RID: 1479 RVA: 0x00020B78 File Offset: 0x0001FB78
		
		public event EventHandler StateChanged = null;

		// Token: 0x060005C8 RID: 1480 RVA: 0x00020BB0 File Offset: 0x0001FBB0
		private void OnStateChanged()
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs());
			}
		}

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x060005C9 RID: 1481 RVA: 0x00020BE0 File Offset: 0x0001FBE0
		// (remove) Token: 0x060005CA RID: 1482 RVA: 0x00020C18 File Offset: 0x0001FC18
		
		public event EventHandler Disposed = null;

		// Token: 0x060005CB RID: 1483 RVA: 0x00020C50 File Offset: 0x0001FC50
		protected void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x060005CC RID: 1484 RVA: 0x00020C80 File Offset: 0x0001FC80
		// (remove) Token: 0x060005CD RID: 1485 RVA: 0x00020CB8 File Offset: 0x0001FCB8
		
		public event EventHandler TimedOut = null;

		// Token: 0x060005CE RID: 1486 RVA: 0x00020CF0 File Offset: 0x0001FCF0
		protected void OnTimedOut()
		{
			bool flag = this.TimedOut != null;
			if (flag)
			{
				this.TimedOut(this, new EventArgs());
			}
		}

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x060005CF RID: 1487 RVA: 0x00020D20 File Offset: 0x0001FD20
		// (remove) Token: 0x060005D0 RID: 1488 RVA: 0x00020D58 File Offset: 0x0001FD58
		
		public event EventHandler<ExceptionEventArgs> TransportError = null;

		// Token: 0x060005D1 RID: 1489 RVA: 0x00020D90 File Offset: 0x0001FD90
		protected void OnTransportError(Exception exception)
		{
			bool flag = exception == null;
			if (flag)
			{
				throw new ArgumentNullException("exception");
			}
			bool flag2 = this.TransportError != null;
			if (flag2)
			{
				this.TransportError(this, new ExceptionEventArgs(exception));
			}
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060005D2 RID: 1490 RVA: 0x00020DD4 File Offset: 0x0001FDD4
		// (remove) Token: 0x060005D3 RID: 1491 RVA: 0x00020E0C File Offset: 0x0001FE0C
		
		public event EventHandler TransactionError = null;

		// Token: 0x060005D4 RID: 1492 RVA: 0x00020E44 File Offset: 0x0001FE44
		protected void OnTransactionError(string errorText)
		{
			bool flag = this.TransactionError != null;
			if (flag)
			{
				this.TransactionError(this, new EventArgs());
			}
		}

		// Token: 0x04000221 RID: 545
		private SIP_TransactionState m_State;

		// Token: 0x04000222 RID: 546
		private SIP_Stack m_pStack = null;

		// Token: 0x04000223 RID: 547
		private SIP_Flow m_pFlow = null;

		// Token: 0x04000224 RID: 548
		private SIP_Request m_pRequest = null;

		// Token: 0x04000225 RID: 549
		private string m_Method = "";

		// Token: 0x04000226 RID: 550
		private string m_ID = "";

		// Token: 0x04000227 RID: 551
		private string m_Key = "";

		// Token: 0x04000228 RID: 552
		private DateTime m_CreateTime;

		// Token: 0x04000229 RID: 553
		private List<SIP_Response> m_pResponses = null;

		// Token: 0x0400022A RID: 554
		private object m_pTag = null;

		// Token: 0x0400022B RID: 555
		private object m_pLock = new object();
	}
}
