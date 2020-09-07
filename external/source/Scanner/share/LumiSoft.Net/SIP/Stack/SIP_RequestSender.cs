using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000092 RID: 146
	public class SIP_RequestSender : IDisposable
	{
		// Token: 0x0600055E RID: 1374 RVA: 0x0001E4A8 File Offset: 0x0001D4A8
		internal SIP_RequestSender(SIP_Stack stack, SIP_Request request, SIP_Flow flow)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			this.m_pStack = stack;
			this.m_pRequest = request;
			this.m_pFlow = flow;
			this.m_pCredentials = new List<NetworkCredential>();
			this.m_pHops = new Queue<SIP_Hop>();
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x0001E570 File Offset: 0x0001D570
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (!flag2)
				{
					this.m_State = SIP_RequestSender.SIP_RequestSenderState.Disposed;
					this.OnDisposed();
					this.ResponseReceived = null;
					this.Completed = null;
					this.Disposed = null;
					this.m_pStack = null;
					this.m_pRequest = null;
					this.m_pCredentials = null;
					this.m_pHops = null;
					this.m_pTransaction = null;
					this.m_pLock = null;
				}
			}
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x0001E60C File Offset: 0x0001D60C
		private void ClientTransaction_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.m_pFlow = e.ClientTransaction.Request.Flow;
				bool flag2 = e.Response.StatusCode == 401 || e.Response.StatusCode == 407;
				if (flag2)
				{
					bool flag3 = false;
					foreach (SIP_t_Challenge sip_t_Challenge in e.Response.WWWAuthenticate.GetAllValues())
					{
						foreach (SIP_t_Credentials sip_t_Credentials in this.m_pTransaction.Request.Authorization.GetAllValues())
						{
							bool flag4 = new Auth_HttpDigest(sip_t_Challenge.AuthData, "").Realm == new Auth_HttpDigest(sip_t_Credentials.AuthData, "").Realm;
							if (flag4)
							{
								flag3 = true;
								break;
							}
						}
					}
					foreach (SIP_t_Challenge sip_t_Challenge2 in e.Response.ProxyAuthenticate.GetAllValues())
					{
						foreach (SIP_t_Credentials sip_t_Credentials2 in this.m_pTransaction.Request.ProxyAuthorization.GetAllValues())
						{
							bool flag5 = new Auth_HttpDigest(sip_t_Challenge2.AuthData, "").Realm == new Auth_HttpDigest(sip_t_Credentials2.AuthData, "").Realm;
							if (flag5)
							{
								flag3 = true;
								break;
							}
						}
					}
					bool flag6 = flag3;
					if (flag6)
					{
						this.OnResponseReceived(e.Response);
					}
					else
					{
						SIP_Request sip_Request = this.m_pRequest.Copy();
						sip_Request.CSeq = new SIP_t_CSeq(this.m_pStack.ConsumeCSeq(), sip_Request.CSeq.RequestMethod);
						bool flag7 = this.Authorize(sip_Request, e.Response, this.Credentials.ToArray());
						if (flag7)
						{
							SIP_Flow flow = this.m_pTransaction.Flow;
							this.CleanUpActiveTransaction();
							this.SendToFlow(flow, sip_Request);
						}
						else
						{
							this.OnResponseReceived(e.Response);
						}
					}
				}
				else
				{
					this.OnResponseReceived(e.Response);
					bool flag8 = e.Response.StatusCodeType > SIP_StatusCodeType.Provisional;
					if (flag8)
					{
						this.OnCompleted();
					}
				}
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001E8B0 File Offset: 0x0001D8B0
		private void ClientTransaction_TimedOut(object sender, EventArgs e)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_pHops.Count > 0;
				if (flag2)
				{
					this.CleanUpActiveTransaction();
					this.SendToNextHop();
				}
				else
				{
					this.OnResponseReceived(this.m_pStack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pRequest));
					this.OnCompleted();
				}
			}
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x0001E93C File Offset: 0x0001D93C
		private void ClientTransaction_TransportError(object sender, EventArgs e)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_pHops.Count > 0;
				if (flag2)
				{
					this.CleanUpActiveTransaction();
					this.SendToNextHop();
				}
				else
				{
					this.OnResponseReceived(this.m_pStack.CreateResponse(SIP_ResponseCodes.x503_Service_Unavailable + ": Transport error.", this.m_pRequest));
					this.OnCompleted();
				}
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x0001E9D0 File Offset: 0x0001D9D0
		public void Start()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool isStarted = this.m_IsStarted;
				if (isStarted)
				{
					throw new InvalidOperationException("Start method has been already called.");
				}
				this.m_IsStarted = true;
				this.m_State = SIP_RequestSender.SIP_RequestSenderState.Starting;
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					object pLock2 = this.m_pLock;
					lock (pLock2)
					{
						bool flag4 = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
						if (!flag4)
						{
							bool flag5 = false;
							SIP_Uri sip_Uri = null;
							bool flag6 = flag5;
							if (flag6)
							{
								sip_Uri = (SIP_Uri)this.m_pRequest.RequestLine.Uri;
							}
							else
							{
								bool flag7 = this.m_pRequest.Route.GetTopMostValue() != null;
								if (flag7)
								{
									sip_Uri = (SIP_Uri)this.m_pRequest.Route.GetTopMostValue().Address.Uri;
								}
								else
								{
									sip_Uri = (SIP_Uri)this.m_pRequest.RequestLine.Uri;
								}
							}
							try
							{
								foreach (SIP_Hop item in this.m_pStack.GetHops(sip_Uri, this.m_pRequest.ToByteData().Length, ((SIP_Uri)this.m_pRequest.RequestLine.Uri).IsSecure))
								{
									this.m_pHops.Enqueue(item);
								}
							}
							catch (Exception ex)
							{
								this.OnTransportError(new SIP_TransportException("SIP hops resolving failed '" + ex.Message + "'."));
								this.OnCompleted();
								return;
							}
							bool flag8 = this.m_pHops.Count == 0;
							if (flag8)
							{
								this.OnTransportError(new SIP_TransportException("No target hops resolved for '" + sip_Uri + "'."));
								this.OnCompleted();
							}
							else
							{
								this.m_State = SIP_RequestSender.SIP_RequestSenderState.Started;
								try
								{
									bool flag9 = this.m_pFlow != null;
									if (flag9)
									{
										this.SendToFlow(this.m_pFlow, this.m_pRequest.Copy());
										return;
									}
								}
								catch
								{
								}
								this.SendToNextHop();
							}
						}
					}
				});
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x0001EA64 File Offset: 0x0001DA64
		public void Cancel()
		{
			while (this.m_State == SIP_RequestSender.SIP_RequestSenderState.Starting)
			{
				Thread.Sleep(5);
			}
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag2 = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag2)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag3 = !this.m_IsStarted;
				if (flag3)
				{
					throw new InvalidOperationException("Request sending has not started, nothing to cancel.");
				}
				bool flag4 = this.m_State != SIP_RequestSender.SIP_RequestSenderState.Started;
				if (flag4)
				{
					return;
				}
				this.m_pHops.Clear();
			}
			this.m_pTransaction.Cancel();
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x0001EB24 File Offset: 0x0001DB24
		private bool Authorize(SIP_Request request, SIP_Response response, NetworkCredential[] credentials)
		{
			bool flag = request == null;
			if (flag)
			{
				throw new ArgumentNullException("request");
			}
			bool flag2 = response == null;
			if (flag2)
			{
				throw new ArgumentNullException("response");
			}
			bool flag3 = credentials == null;
			if (flag3)
			{
				throw new ArgumentNullException("credentials");
			}
			bool result = true;
			foreach (SIP_t_Challenge sip_t_Challenge in response.WWWAuthenticate.GetAllValues())
			{
				Auth_HttpDigest auth_HttpDigest = new Auth_HttpDigest(sip_t_Challenge.AuthData, request.RequestLine.Method);
				NetworkCredential networkCredential = null;
				foreach (NetworkCredential networkCredential2 in credentials)
				{
					bool flag4 = string.Equals(networkCredential2.Domain, auth_HttpDigest.Realm, StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						networkCredential = networkCredential2;
						break;
					}
				}
				bool flag5 = networkCredential == null;
				if (flag5)
				{
					result = false;
				}
				else
				{
					auth_HttpDigest.UserName = networkCredential.UserName;
					auth_HttpDigest.Password = networkCredential.Password;
					auth_HttpDigest.CNonce = Auth_HttpDigest.CreateNonce();
					auth_HttpDigest.Uri = request.RequestLine.Uri.ToString();
					request.Authorization.Add(auth_HttpDigest.ToAuthorization());
				}
			}
			foreach (SIP_t_Challenge sip_t_Challenge2 in response.ProxyAuthenticate.GetAllValues())
			{
				Auth_HttpDigest auth_HttpDigest2 = new Auth_HttpDigest(sip_t_Challenge2.AuthData, request.RequestLine.Method);
				NetworkCredential networkCredential3 = null;
				foreach (NetworkCredential networkCredential4 in credentials)
				{
					bool flag6 = string.Equals(networkCredential4.Domain, auth_HttpDigest2.Realm, StringComparison.InvariantCultureIgnoreCase);
					if (flag6)
					{
						networkCredential3 = networkCredential4;
						break;
					}
				}
				bool flag7 = networkCredential3 == null;
				if (flag7)
				{
					result = false;
				}
				else
				{
					auth_HttpDigest2.UserName = networkCredential3.UserName;
					auth_HttpDigest2.Password = networkCredential3.Password;
					auth_HttpDigest2.CNonce = Auth_HttpDigest.CreateNonce();
					auth_HttpDigest2.Uri = request.RequestLine.Uri.ToString();
					request.ProxyAuthorization.Add(auth_HttpDigest2.ToAuthorization());
				}
			}
			return result;
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0001ED64 File Offset: 0x0001DD64
		private void SendToNextHop()
		{
			bool flag = this.m_pHops.Count == 0;
			if (flag)
			{
				throw new InvalidOperationException("No more hop(s).");
			}
			try
			{
				SIP_Hop sip_Hop = this.m_pHops.Dequeue();
				this.SendToFlow(this.m_pStack.TransportLayer.GetOrCreateFlow(sip_Hop.Transport, null, sip_Hop.EndPoint), this.m_pRequest.Copy());
			}
			catch (ObjectDisposedException ex)
			{
				bool flag2 = this.m_pStack.State != SIP_StackState.Disposed;
				if (flag2)
				{
					throw ex;
				}
			}
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0001EE00 File Offset: 0x0001DE00
		private void SendToFlow(SIP_Flow flow, SIP_Request request)
		{
			bool flag = flow == null;
			if (flag)
			{
				throw new ArgumentNullException("flow");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			SIP_t_ContactParam topMostValue = request.Contact.GetTopMostValue();
			bool flag3 = SIP_Utils.MethodCanEstablishDialog(request.RequestLine.Method) && topMostValue == null;
			if (flag3)
			{
				SIP_Uri sip_Uri = (SIP_Uri)request.From.Address.Uri;
				request.Contact.Add((flow.IsSecure ? "sips:" : "sip:") + sip_Uri.User + "@" + flow.LocalPublicEP.ToString());
			}
			else
			{
				bool flag4 = topMostValue != null && topMostValue.Address.Uri is SIP_Uri && ((SIP_Uri)topMostValue.Address.Uri).Host == "auto-allocate";
				if (flag4)
				{
					((SIP_Uri)topMostValue.Address.Uri).Host = flow.LocalPublicEP.ToString();
				}
			}
			this.m_pTransaction = this.m_pStack.TransactionLayer.CreateClientTransaction(flow, request, true);
			this.m_pTransaction.ResponseReceived += this.ClientTransaction_ResponseReceived;
			this.m_pTransaction.TimedOut += this.ClientTransaction_TimedOut;
			this.m_pTransaction.TransportError += new EventHandler<ExceptionEventArgs>(this.ClientTransaction_TransportError);
			this.m_pTransaction.Start();
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0001EF84 File Offset: 0x0001DF84
		private void CleanUpActiveTransaction()
		{
			bool flag = this.m_pTransaction != null;
			if (flag)
			{
				this.m_pTransaction.ResponseReceived -= this.ClientTransaction_ResponseReceived;
				this.m_pTransaction.TimedOut -= this.ClientTransaction_TimedOut;
				this.m_pTransaction.TransportError -= new EventHandler<ExceptionEventArgs>(this.ClientTransaction_TransportError);
				this.m_pTransaction = null;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x0001EFF0 File Offset: 0x0001DFF0
		public bool IsDisposed
		{
			get
			{
				return this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x0001F00C File Offset: 0x0001E00C
		public bool IsStarted
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_IsStarted;
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x0001F044 File Offset: 0x0001E044
		public bool IsCompleted
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_State == SIP_RequestSender.SIP_RequestSenderState.Completed;
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600056C RID: 1388 RVA: 0x0001F080 File Offset: 0x0001E080
		public SIP_Stack Stack
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pStack;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600056D RID: 1389 RVA: 0x0001F0B8 File Offset: 0x0001E0B8
		public SIP_Request Request
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRequest;
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x0001F0F0 File Offset: 0x0001E0F0
		public SIP_Flow Flow
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pFlow;
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x0001F128 File Offset: 0x0001E128
		public List<NetworkCredential> Credentials
		{
			get
			{
				bool flag = this.m_State == SIP_RequestSender.SIP_RequestSenderState.Disposed;
				if (flag)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pCredentials;
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x0001F160 File Offset: 0x0001E160
		// (set) Token: 0x06000571 RID: 1393 RVA: 0x0001F178 File Offset: 0x0001E178
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

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06000572 RID: 1394 RVA: 0x0001F184 File Offset: 0x0001E184
		// (remove) Token: 0x06000573 RID: 1395 RVA: 0x0001F1BC File Offset: 0x0001E1BC
		
		public event EventHandler<SIP_ResponseReceivedEventArgs> ResponseReceived = null;

		// Token: 0x06000574 RID: 1396 RVA: 0x0001F1F4 File Offset: 0x0001E1F4
		private void OnResponseReceived(SIP_Response response)
		{
			bool flag = this.ResponseReceived != null;
			if (flag)
			{
				this.ResponseReceived(this, new SIP_ResponseReceivedEventArgs(this.m_pStack, this.m_pTransaction, response));
			}
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x000091B8 File Offset: 0x000081B8
		private void OnTransportError(Exception exception)
		{
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06000576 RID: 1398 RVA: 0x0001F230 File Offset: 0x0001E230
		// (remove) Token: 0x06000577 RID: 1399 RVA: 0x0001F268 File Offset: 0x0001E268
		
		public event EventHandler Completed = null;

		// Token: 0x06000578 RID: 1400 RVA: 0x0001F2A0 File Offset: 0x0001E2A0
		private void OnCompleted()
		{
			this.m_State = SIP_RequestSender.SIP_RequestSenderState.Completed;
			bool flag = this.Completed != null;
			if (flag)
			{
				this.Completed(this, new EventArgs());
			}
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06000579 RID: 1401 RVA: 0x0001F2D8 File Offset: 0x0001E2D8
		// (remove) Token: 0x0600057A RID: 1402 RVA: 0x0001F310 File Offset: 0x0001E310
		
		public event EventHandler Disposed = null;

		// Token: 0x0600057B RID: 1403 RVA: 0x0001F348 File Offset: 0x0001E348
		private void OnDisposed()
		{
			bool flag = this.Disposed != null;
			if (flag)
			{
				this.Disposed(this, new EventArgs());
			}
		}

		// Token: 0x040001D8 RID: 472
		private object m_pLock = new object();

		// Token: 0x040001D9 RID: 473
		private SIP_RequestSender.SIP_RequestSenderState m_State = SIP_RequestSender.SIP_RequestSenderState.Initial;

		// Token: 0x040001DA RID: 474
		private bool m_IsStarted = false;

		// Token: 0x040001DB RID: 475
		private SIP_Stack m_pStack = null;

		// Token: 0x040001DC RID: 476
		private SIP_Request m_pRequest = null;

		// Token: 0x040001DD RID: 477
		private List<NetworkCredential> m_pCredentials = null;

		// Token: 0x040001DE RID: 478
		private Queue<SIP_Hop> m_pHops = null;

		// Token: 0x040001DF RID: 479
		private SIP_ClientTransaction m_pTransaction = null;

		// Token: 0x040001E0 RID: 480
		private SIP_Flow m_pFlow = null;

		// Token: 0x040001E1 RID: 481
		private object m_pTag = null;

		// Token: 0x0200028A RID: 650
		private enum SIP_RequestSenderState
		{
			// Token: 0x0400098F RID: 2447
			Initial,
			// Token: 0x04000990 RID: 2448
			Starting,
			// Token: 0x04000991 RID: 2449
			Started,
			// Token: 0x04000992 RID: 2450
			Completed,
			// Token: 0x04000993 RID: 2451
			Disposed
		}
	}
}
