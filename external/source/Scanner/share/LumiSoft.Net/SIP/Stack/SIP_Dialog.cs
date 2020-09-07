using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000086 RID: 134
	public class SIP_Dialog
	{
		// Token: 0x060004C8 RID: 1224 RVA: 0x000181AC File Offset: 0x000171AC
		public SIP_Dialog()
		{
			this.m_CreateTime = DateTime.Now;
			this.m_pRouteSet = new SIP_t_AddressParam[0];
			this.m_pTransactions = new List<SIP_Transaction>();
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00018280 File Offset: 0x00017280
		protected internal virtual void Init(SIP_Stack stack, SIP_Transaction transaction, SIP_Response response)
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
			this.m_pStack = stack;
			bool flag4 = transaction is SIP_ServerTransaction;
			if (flag4)
			{
				this.m_IsSecure = ((SIP_Uri)transaction.Request.RequestLine.Uri).IsSecure;
				this.m_pRouteSet = (SIP_t_AddressParam[])Net_Utils.ReverseArray(transaction.Request.RecordRoute.GetAllValues());
				this.m_pRemoteTarget = (SIP_Uri)transaction.Request.Contact.GetTopMostValue().Address.Uri;
				this.m_RemoteSeqNo = transaction.Request.CSeq.SequenceNumber;
				this.m_LocalSeqNo = 0;
				this.m_CallID = transaction.Request.CallID;
				this.m_LocalTag = response.To.Tag;
				this.m_RemoteTag = transaction.Request.From.Tag;
				this.m_pRemoteUri = transaction.Request.From.Address.Uri;
				this.m_pLocalUri = transaction.Request.To.Address.Uri;
				this.m_pLocalContact = (SIP_Uri)response.Contact.GetTopMostValue().Address.Uri;
				List<string> list = new List<string>();
				foreach (SIP_t_Method sip_t_Method in response.Allow.GetAllValues())
				{
					list.Add(sip_t_Method.Method);
				}
				this.m_pRemoteAllow = list.ToArray();
				List<string> list2 = new List<string>();
				foreach (SIP_t_OptionTag sip_t_OptionTag in response.Supported.GetAllValues())
				{
					list2.Add(sip_t_OptionTag.OptionTag);
				}
				this.m_pRemoteSupported = list2.ToArray();
			}
			else
			{
				this.m_IsSecure = ((SIP_Uri)transaction.Request.RequestLine.Uri).IsSecure;
				this.m_pRouteSet = (SIP_t_AddressParam[])Net_Utils.ReverseArray(response.RecordRoute.GetAllValues());
				this.m_pRemoteTarget = (SIP_Uri)response.Contact.GetTopMostValue().Address.Uri;
				this.m_LocalSeqNo = transaction.Request.CSeq.SequenceNumber;
				this.m_RemoteSeqNo = 0;
				this.m_CallID = transaction.Request.CallID;
				this.m_LocalTag = transaction.Request.From.Tag;
				this.m_RemoteTag = response.To.Tag;
				this.m_pRemoteUri = transaction.Request.To.Address.Uri;
				this.m_pLocalUri = transaction.Request.From.Address.Uri;
				this.m_pLocalContact = (SIP_Uri)transaction.Request.Contact.GetTopMostValue().Address.Uri;
				List<string> list3 = new List<string>();
				foreach (SIP_t_Method sip_t_Method2 in response.Allow.GetAllValues())
				{
					list3.Add(sip_t_Method2.Method);
				}
				this.m_pRemoteAllow = list3.ToArray();
				List<string> list4 = new List<string>();
				foreach (SIP_t_OptionTag sip_t_OptionTag2 in response.Supported.GetAllValues())
				{
					list4.Add(sip_t_OptionTag2.OptionTag);
				}
				this.m_pRemoteSupported = list4.ToArray();
			}
			this.m_pFlow = transaction.Flow;
			this.AddTransaction(transaction);
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00018650 File Offset: 0x00017650
		public virtual void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.State == SIP_DialogState.Disposed;
				if (!flag2)
				{
					this.SetState(SIP_DialogState.Disposed, true);
					this.RequestReceived = null;
					this.m_pStack = null;
					this.m_CallID = null;
					this.m_LocalTag = null;
					this.m_RemoteTag = null;
					this.m_pLocalUri = null;
					this.m_pRemoteUri = null;
					this.m_pLocalContact = null;
					this.m_pRemoteTarget = null;
					this.m_pRouteSet = null;
					this.m_pFlow = null;
				}
			}
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x000186F8 File Offset: 0x000176F8
		public void Terminate()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.State == SIP_DialogState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.SetState(SIP_DialogState.Terminated, true);
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00018760 File Offset: 0x00017760
		public SIP_Request CreateRequest(string method)
		{
			bool flag = this.State == SIP_DialogState.Disposed;
			if (flag)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag2 = method == null;
			if (flag2)
			{
				throw new ArgumentNullException("method");
			}
			bool flag3 = method == string.Empty;
			if (flag3)
			{
				throw new ArgumentException("Argument 'method' value must be specified.");
			}
			object pLock = this.m_pLock;
			SIP_Request result;
			lock (pLock)
			{
				SIP_Request sip_Request = this.m_pStack.CreateRequest(method, new SIP_t_NameAddress("", this.m_pRemoteUri), new SIP_t_NameAddress("", this.m_pLocalUri));
				sip_Request.Route.RemoveAll();
				bool flag5 = this.m_pRouteSet.Length == 0;
				if (flag5)
				{
					sip_Request.RequestLine.Uri = this.m_pRemoteTarget;
				}
				else
				{
					SIP_Uri sip_Uri = (SIP_Uri)this.m_pRouteSet[0].Address.Uri;
					bool param_Lr = sip_Uri.Param_Lr;
					if (param_Lr)
					{
						sip_Request.RequestLine.Uri = this.m_pRemoteTarget;
						for (int i = 0; i < this.m_pRouteSet.Length; i++)
						{
							sip_Request.Route.Add(this.m_pRouteSet[i].ToStringValue());
						}
					}
					else
					{
						sip_Request.RequestLine.Uri = SIP_Utils.UriToRequestUri(sip_Uri);
						for (int j = 1; j < this.m_pRouteSet.Length; j++)
						{
							sip_Request.Route.Add(this.m_pRouteSet[j].ToStringValue());
						}
					}
				}
				sip_Request.To.Tag = this.m_RemoteTag;
				sip_Request.From.Tag = this.m_LocalTag;
				sip_Request.CallID = this.m_CallID;
				bool flag6 = method != "ACK";
				if (flag6)
				{
					SIP_t_CSeq cseq = sip_Request.CSeq;
					int num = this.m_LocalSeqNo + 1;
					this.m_LocalSeqNo = num;
					cseq.SequenceNumber = num;
				}
				sip_Request.Contact.Add(this.m_pLocalContact.ToString());
				result = sip_Request;
			}
			return result;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x000189B0 File Offset: 0x000179B0
		public SIP_RequestSender CreateRequestSender(SIP_Request request)
		{
			object pLock = this.m_pLock;
			SIP_RequestSender result;
			lock (pLock)
			{
				bool flag2 = this.State == SIP_DialogState.Terminated;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = request == null;
				if (flag3)
				{
					throw new ArgumentNullException("request");
				}
				SIP_RequestSender sip_RequestSender = this.m_pStack.CreateRequestSender(request, this.Flow);
				result = sip_RequestSender;
			}
			return result;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00018A3C File Offset: 0x00017A3C
		protected bool IsTargetRefresh(string method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			method = method.ToUpper();
			bool flag2 = method == "INVITE";
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = method == "UPDATE";
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = method == "SUBSCRIBE";
					if (flag4)
					{
						result = true;
					}
					else
					{
						bool flag5 = method == "NOTIFY";
						if (flag5)
						{
							result = true;
						}
						else
						{
							bool flag6 = method == "REFER";
							result = flag6;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00018AD8 File Offset: 0x00017AD8
		protected void SetState(SIP_DialogState state, bool raiseEvent)
		{
			this.m_State = state;
			if (raiseEvent)
			{
				this.OnStateChanged();
			}
			bool flag = this.m_State == SIP_DialogState.Terminated;
			if (flag)
			{
				this.Dispose();
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x00018B14 File Offset: 0x00017B14
		protected internal virtual bool ProcessRequest(SIP_RequestReceivedEventArgs e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			bool flag2 = this.m_RemoteSeqNo == 0;
			if (flag2)
			{
				this.m_RemoteSeqNo = e.Request.CSeq.SequenceNumber;
			}
			else
			{
				bool flag3 = e.Request.CSeq.SequenceNumber < this.m_RemoteSeqNo;
				if (flag3)
				{
					e.ServerTransaction.SendResponse(this.Stack.CreateResponse(SIP_ResponseCodes.x500_Server_Internal_Error + ": The mid-dialog request is out of order(late arriving request).", e.Request));
					return true;
				}
				this.m_RemoteSeqNo = e.Request.CSeq.SequenceNumber;
			}
			bool flag4 = this.IsTargetRefresh(e.Request.RequestLine.Method) && e.Request.Contact.Count != 0;
			if (flag4)
			{
				this.m_pRemoteTarget = (SIP_Uri)e.Request.Contact.GetTopMostValue().Address.Uri;
			}
			this.OnRequestReceived(e);
			return e.IsHandled;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00018C34 File Offset: 0x00017C34
		protected internal virtual bool ProcessResponse(SIP_Response response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			return false;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00018C5C File Offset: 0x00017C5C
		internal void AddTransaction(SIP_Transaction transaction)
		{
			bool flag = transaction == null;
			if (flag)
			{
				throw new ArgumentNullException("transaction");
			}
			this.m_pTransactions.Add(transaction);
			transaction.Disposed += delegate(object s, EventArgs e)
			{
				this.m_pTransactions.Remove(transaction);
			};
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x00018CC4 File Offset: 0x00017CC4
		public object SyncRoot
		{
			get
			{
				return this.m_pLock;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060004D4 RID: 1236 RVA: 0x00018CDC File Offset: 0x00017CDC
		public SIP_DialogState State
		{
			get
			{
				return this.m_State;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x00018CF4 File Offset: 0x00017CF4
		public SIP_Stack Stack
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pStack;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x00018D2C File Offset: 0x00017D2C
		public DateTime CreateTime
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CreateTime;
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x00018D64 File Offset: 0x00017D64
		public string ID
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return string.Concat(new string[]
				{
					this.CallID,
					"-",
					this.LocalTag,
					"-",
					this.RemoteTag
				});
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x00018DCC File Offset: 0x00017DCC
		public string CallID
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_CallID;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00018E04 File Offset: 0x00017E04
		public string LocalTag
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LocalTag;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x00018E3C File Offset: 0x00017E3C
		public string RemoteTag
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RemoteTag;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x00018E74 File Offset: 0x00017E74
		public int LocalSeqNo
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_LocalSeqNo;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x00018EAC File Offset: 0x00017EAC
		public int RemoteSeqNo
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_RemoteSeqNo;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x00018EE4 File Offset: 0x00017EE4
		public AbsoluteUri LocalUri
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLocalUri;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x00018F1C File Offset: 0x00017F1C
		public AbsoluteUri RemoteUri
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRemoteUri;
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060004DF RID: 1247 RVA: 0x00018F54 File Offset: 0x00017F54
		public SIP_Uri LocalContact
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pLocalContact;
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00018F8C File Offset: 0x00017F8C
		public SIP_Uri RemoteTarget
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRemoteTarget;
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060004E1 RID: 1249 RVA: 0x00018FC4 File Offset: 0x00017FC4
		public bool IsSecure
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsSecure;
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x00018FFC File Offset: 0x00017FFC
		public SIP_t_AddressParam[] RouteSet
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRouteSet;
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00019034 File Offset: 0x00018034
		public string[] RemoteAllow
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRemoteAllow;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0001906C File Offset: 0x0001806C
		public string[] RemoteSupported
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRemoteSupported;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x000190A4 File Offset: 0x000180A4
		public SIP_Flow Flow
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pFlow;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x000190DC File Offset: 0x000180DC
		public SIP_Transaction[] Transactions
		{
			get
			{
				bool flag = this.State == SIP_DialogState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pTransactions.ToArray();
			}
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x060004E7 RID: 1255 RVA: 0x00019118 File Offset: 0x00018118
		// (remove) Token: 0x060004E8 RID: 1256 RVA: 0x00019150 File Offset: 0x00018150
		
		public event EventHandler StateChanged = null;

		// Token: 0x060004E9 RID: 1257 RVA: 0x00019188 File Offset: 0x00018188
		private void OnStateChanged()
		{
			bool flag = this.StateChanged != null;
			if (flag)
			{
				this.StateChanged(this, new EventArgs());
			}
		}

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x060004EA RID: 1258 RVA: 0x000191B8 File Offset: 0x000181B8
		// (remove) Token: 0x060004EB RID: 1259 RVA: 0x000191F0 File Offset: 0x000181F0
		
		public event EventHandler<SIP_RequestReceivedEventArgs> RequestReceived = null;

		// Token: 0x060004EC RID: 1260 RVA: 0x00019228 File Offset: 0x00018228
		private void OnRequestReceived(SIP_RequestReceivedEventArgs e)
		{
			bool flag = this.RequestReceived != null;
			if (flag)
			{
				this.RequestReceived(this, e);
			}
		}

		// Token: 0x04000184 RID: 388
		private object m_pLock = new object();

		// Token: 0x04000185 RID: 389
		private SIP_DialogState m_State = SIP_DialogState.Early;

		// Token: 0x04000186 RID: 390
		private SIP_Stack m_pStack = null;

		// Token: 0x04000187 RID: 391
		private DateTime m_CreateTime;

		// Token: 0x04000188 RID: 392
		private string m_CallID = "";

		// Token: 0x04000189 RID: 393
		private string m_LocalTag = "";

		// Token: 0x0400018A RID: 394
		private string m_RemoteTag = "";

		// Token: 0x0400018B RID: 395
		private int m_LocalSeqNo = 0;

		// Token: 0x0400018C RID: 396
		private int m_RemoteSeqNo = 0;

		// Token: 0x0400018D RID: 397
		private AbsoluteUri m_pLocalUri = null;

		// Token: 0x0400018E RID: 398
		private AbsoluteUri m_pRemoteUri = null;

		// Token: 0x0400018F RID: 399
		private SIP_Uri m_pLocalContact = null;

		// Token: 0x04000190 RID: 400
		private SIP_Uri m_pRemoteTarget = null;

		// Token: 0x04000191 RID: 401
		private bool m_IsSecure = false;

		// Token: 0x04000192 RID: 402
		private SIP_t_AddressParam[] m_pRouteSet = null;

		// Token: 0x04000193 RID: 403
		private string[] m_pRemoteAllow = null;

		// Token: 0x04000194 RID: 404
		private string[] m_pRemoteSupported = null;

		// Token: 0x04000195 RID: 405
		private SIP_Flow m_pFlow = null;

		// Token: 0x04000196 RID: 406
		private List<SIP_Transaction> m_pTransactions = null;
	}
}
