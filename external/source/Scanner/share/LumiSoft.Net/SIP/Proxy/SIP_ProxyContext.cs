using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000B0 RID: 176
	public class SIP_ProxyContext : IDisposable
	{
		// Token: 0x060006C9 RID: 1737 RVA: 0x000282A8 File Offset: 0x000272A8
		internal SIP_ProxyContext(SIP_Proxy proxy, SIP_ServerTransaction transaction, SIP_Request request, bool addRecordRoute, SIP_ForkingMode forkingMode, bool isB2BUA, bool noCancel, bool noRecurse, SIP_ProxyTarget[] targets)
		{
			bool flag = proxy == null;
			if (flag)
			{
				throw new ArgumentNullException("proxy");
			}
			bool flag2 = transaction == null;
			if (flag2)
			{
				throw new ArgumentNullException("transaction");
			}
			bool flag3 = request == null;
			if (flag3)
			{
				throw new ArgumentNullException("request");
			}
			bool flag4 = targets == null;
			if (flag4)
			{
				throw new ArgumentNullException("targets");
			}
			bool flag5 = targets.Length == 0;
			if (flag5)
			{
				throw new ArgumentException("Argumnet 'targets' must contain at least 1 value.");
			}
			this.m_pProxy = proxy;
			this.m_pServerTransaction = transaction;
			this.m_pServerTransaction.Canceled += this.m_pServerTransaction_Canceled;
			this.m_pServerTransaction.Disposed += this.m_pServerTransaction_Disposed;
			this.m_pRequest = request;
			this.m_AddRecordRoute = addRecordRoute;
			this.m_ForkingMode = forkingMode;
			this.m_IsB2BUA = isB2BUA;
			this.m_NoCancel = noCancel;
			this.m_NoRecurse = noRecurse;
			this.m_pTargetsHandlers = new List<SIP_ProxyContext.TargetHandler>();
			this.m_pResponses = new List<SIP_Response>();
			this.m_ID = Guid.NewGuid().ToString();
			this.m_CreateTime = DateTime.Now;
			this.m_pTargets = new Queue<SIP_ProxyContext.TargetHandler>();
			foreach (SIP_ProxyTarget sip_ProxyTarget in targets)
			{
				this.m_pTargets.Enqueue(new SIP_ProxyContext.TargetHandler(this, sip_ProxyTarget.Flow, sip_ProxyTarget.TargetUri, this.m_AddRecordRoute, false));
			}
			this.m_pCredentials = new List<NetworkCredential>();
			foreach (SIP_t_Directive sip_t_Directive in request.RequestDisposition.GetAllValues())
			{
				bool flag6 = sip_t_Directive.Directive == SIP_t_Directive.DirectiveType.NoFork;
				if (flag6)
				{
					this.m_ForkingMode = SIP_ForkingMode.None;
				}
				else
				{
					bool flag7 = sip_t_Directive.Directive == SIP_t_Directive.DirectiveType.Parallel;
					if (flag7)
					{
						this.m_ForkingMode = SIP_ForkingMode.Parallel;
					}
					else
					{
						bool flag8 = sip_t_Directive.Directive == SIP_t_Directive.DirectiveType.Sequential;
						if (flag8)
						{
							this.m_ForkingMode = SIP_ForkingMode.Sequential;
						}
						else
						{
							bool flag9 = sip_t_Directive.Directive == SIP_t_Directive.DirectiveType.NoCancel;
							if (flag9)
							{
								this.m_NoCancel = true;
							}
							else
							{
								bool flag10 = sip_t_Directive.Directive == SIP_t_Directive.DirectiveType.NoRecurse;
								if (flag10)
								{
									this.m_NoRecurse = true;
								}
							}
						}
					}
				}
			}
			this.m_pProxy.Stack.Logger.AddText("ProxyContext(id='" + this.m_ID + "') created.");
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0002858C File Offset: 0x0002758C
		public void Dispose()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (!isDisposed)
				{
					this.m_IsDisposed = true;
					this.m_pProxy.Stack.Logger.AddText("ProxyContext(id='" + this.m_ID + "') disposed.");
					this.m_pProxy.m_pProxyContexts.Remove(this);
					this.m_pProxy = null;
					this.m_pServerTransaction = null;
					this.m_pTargetsHandlers = null;
					this.m_pResponses = null;
					this.m_pTargets = null;
				}
			}
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00028640 File Offset: 0x00027640
		private void m_pServerTransaction_Canceled(object sender, EventArgs e)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.CancelAllTargets();
			}
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00028688 File Offset: 0x00027688
		private void m_pServerTransaction_Disposed(object sender, EventArgs e)
		{
			this.Dispose();
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x00028694 File Offset: 0x00027694
		private void TargetHandler_Disposed(SIP_ProxyContext.TargetHandler handler)
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.m_pTargetsHandlers.Remove(handler);
				bool flag2 = this.m_pTargets.Count == 0 && this.m_pTargetsHandlers.Count == 0;
				if (flag2)
				{
					this.Dispose();
				}
			}
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0002870C File Offset: 0x0002770C
		public void Start()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool isStarted = this.m_IsStarted;
				if (isStarted)
				{
					throw new InvalidOperationException("Start has already called.");
				}
				this.m_IsStarted = true;
				bool flag2 = this.m_ForkingMode == SIP_ForkingMode.None;
				if (flag2)
				{
					SIP_ProxyContext.TargetHandler targetHandler = this.m_pTargets.Dequeue();
					this.m_pTargetsHandlers.Add(targetHandler);
					targetHandler.Start();
				}
				else
				{
					bool flag3 = this.m_ForkingMode == SIP_ForkingMode.Parallel;
					if (flag3)
					{
						while (!this.m_IsDisposed && this.m_pTargets.Count > 0)
						{
							SIP_ProxyContext.TargetHandler targetHandler2 = this.m_pTargets.Dequeue();
							this.m_pTargetsHandlers.Add(targetHandler2);
							targetHandler2.Start();
						}
					}
					else
					{
						bool flag4 = this.m_ForkingMode == SIP_ForkingMode.Sequential;
						if (flag4)
						{
							SIP_ProxyContext.TargetHandler targetHandler3 = this.m_pTargets.Dequeue();
							this.m_pTargetsHandlers.Add(targetHandler3);
							targetHandler3.Start();
						}
					}
				}
			}
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00028858 File Offset: 0x00027858
		public void Cancel()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag2 = !this.m_IsStarted;
				if (flag2)
				{
					throw new InvalidOperationException("Start method is not called, nothing to cancel.");
				}
				this.m_pServerTransaction.Cancel();
			}
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x000288D8 File Offset: 0x000278D8
		private void ProcessResponse(SIP_ProxyContext.TargetHandler handler, SIP_ClientTransaction transaction, SIP_Response response)
		{
			bool flag = handler == null;
			if (flag)
			{
				throw new ArgumentNullException("handler");
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
			bool flag4 = false;
			object pLock = this.m_pLock;
			lock (pLock)
			{
				bool flag6 = !this.m_IsB2BUA;
				if (flag6)
				{
					response.Via.RemoveTopMostValue();
					bool flag7 = response.Via.GetAllValues().Length == 0;
					if (flag7)
					{
						return;
					}
				}
				bool flag8 = response.StatusCodeType == SIP_StatusCodeType.Redirection && !this.m_NoRecurse && !handler.IsRecursed;
				if (flag8)
				{
					SIP_t_ContactParam[] allValues = response.Contact.GetAllValues();
					response.Contact.RemoveAll();
					foreach (SIP_t_ContactParam sip_t_ContactParam in allValues)
					{
						bool isSipOrSipsUri = sip_t_ContactParam.Address.IsSipOrSipsUri;
						if (isSipOrSipsUri)
						{
							this.m_pTargets.Enqueue(new SIP_ProxyContext.TargetHandler(this, null, (SIP_Uri)sip_t_ContactParam.Address.Uri, this.m_AddRecordRoute, true));
						}
						else
						{
							response.Contact.Add(sip_t_ContactParam.ToStringValue());
						}
					}
					bool flag9 = response.Contact.GetAllValues().Length != 0;
					if (flag9)
					{
						this.m_pResponses.Add(response);
					}
					bool flag10 = this.m_pTargets.Count > 0;
					if (flag10)
					{
						bool flag11 = this.m_ForkingMode == SIP_ForkingMode.Parallel;
						if (flag11)
						{
							while (this.m_pTargets.Count > 0)
							{
								SIP_ProxyContext.TargetHandler targetHandler = this.m_pTargets.Dequeue();
								this.m_pTargetsHandlers.Add(handler);
								targetHandler.Start();
							}
						}
						else
						{
							SIP_ProxyContext.TargetHandler targetHandler2 = this.m_pTargets.Dequeue();
							this.m_pTargetsHandlers.Add(handler);
							targetHandler2.Start();
						}
						return;
					}
				}
				else
				{
					this.m_pResponses.Add(response);
				}
				bool flag12 = !this.m_IsFinalResponseSent;
				if (flag12)
				{
					bool flag13 = response.StatusCodeType == SIP_StatusCodeType.Provisional && response.StatusCode != 100;
					if (flag13)
					{
						flag4 = true;
					}
					else
					{
						bool flag14 = response.StatusCodeType == SIP_StatusCodeType.Success;
						if (flag14)
						{
							flag4 = true;
						}
						else
						{
							bool flag15 = response.StatusCodeType == SIP_StatusCodeType.GlobalFailure;
							if (flag15)
							{
								this.CancelAllTargets();
							}
						}
					}
				}
				else
				{
					bool flag16 = response.StatusCodeType == SIP_StatusCodeType.Success && this.m_pServerTransaction.Request.RequestLine.Method == "INVITE";
					if (flag16)
					{
						flag4 = true;
					}
				}
				bool flag17 = this.m_ForkingMode == SIP_ForkingMode.Sequential && response.StatusCodeType > SIP_StatusCodeType.Provisional;
				if (flag17)
				{
					bool flag18 = response.StatusCodeType == SIP_StatusCodeType.Success;
					if (!flag18)
					{
						bool flag19 = response.StatusCodeType == SIP_StatusCodeType.GlobalFailure;
						if (!flag19)
						{
							bool flag20 = this.m_pTargets.Count > 0;
							if (flag20)
							{
								SIP_ProxyContext.TargetHandler targetHandler3 = this.m_pTargets.Dequeue();
								this.m_pTargetsHandlers.Add(handler);
								targetHandler3.Start();
								return;
							}
						}
					}
				}
				bool flag21 = !this.m_IsFinalResponseSent && !flag4 && this.m_pTargets.Count == 0;
				if (flag21)
				{
					bool flag22 = true;
					foreach (SIP_ProxyContext.TargetHandler targetHandler4 in this.m_pTargetsHandlers)
					{
						bool flag23 = !targetHandler4.IsCompleted;
						if (flag23)
						{
							flag22 = false;
							break;
						}
					}
					bool flag24 = flag22;
					if (flag24)
					{
						response = this.GetBestFinalResponse();
						bool flag25 = response == null;
						if (flag25)
						{
							response = this.Proxy.Stack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pServerTransaction.Request);
						}
						flag4 = true;
					}
				}
				bool flag26 = flag4;
				if (flag26)
				{
					bool flag27 = response.StatusCode == 401 || response.StatusCode == 407;
					if (flag27)
					{
						foreach (SIP_Response sip_Response in this.m_pResponses.ToArray())
						{
							bool flag28 = response != sip_Response && (sip_Response.StatusCode == 401 || sip_Response.StatusCode == 407);
							if (flag28)
							{
								foreach (SIP_SingleValueHF<SIP_t_Challenge> sip_HeaderField in sip_Response.WWWAuthenticate.HeaderFields)
								{
									sip_Response.WWWAuthenticate.Add(sip_HeaderField.Value);
								}
								foreach (SIP_SingleValueHF<SIP_t_Challenge> sip_HeaderField2 in sip_Response.ProxyAuthenticate.HeaderFields)
								{
									sip_Response.ProxyAuthenticate.Add(sip_HeaderField2.Value);
								}
							}
						}
					}
					this.SendResponse(transaction, response);
					bool flag29 = response.StatusCodeType > SIP_StatusCodeType.Provisional;
					if (flag29)
					{
						this.m_IsFinalResponseSent = true;
					}
					bool flag30 = response.StatusCodeType > SIP_StatusCodeType.Provisional;
					if (flag30)
					{
						this.CancelAllTargets();
					}
				}
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00028E4C File Offset: 0x00027E4C
		private void SendResponse(SIP_ClientTransaction transaction, SIP_Response response)
		{
			bool isB2BUA = this.m_IsB2BUA;
			if (isB2BUA)
			{
				SIP_Request request = this.m_pServerTransaction.Request;
				SIP_Response sip_Response = response.Copy();
				sip_Response.Via.RemoveAll();
				sip_Response.Via.AddToTop(request.Via.GetTopMostValue().ToStringValue());
				sip_Response.CallID = request.CallID;
				sip_Response.CSeq = request.CSeq;
				sip_Response.Contact.RemoveAll();
				sip_Response.RecordRoute.RemoveAll();
				sip_Response.Allow.RemoveAll();
				sip_Response.Supported.RemoveAll();
				bool flag = request.RequestLine.Method != "ACK" && request.RequestLine.Method != "BYE";
				if (flag)
				{
					sip_Response.Allow.Add("INVITE,ACK,OPTIONS,CANCEL,BYE,PRACK");
				}
				bool flag2 = request.RequestLine.Method != "ACK";
				if (flag2)
				{
					sip_Response.Supported.Add("100rel,timer");
				}
				sip_Response.Require.RemoveAll();
				this.m_pServerTransaction.SendResponse(sip_Response);
				bool flag3 = response.CSeq.RequestMethod.ToUpper() == "INVITE" && response.StatusCodeType == SIP_StatusCodeType.Success;
				if (flag3)
				{
					this.m_pProxy.B2BUA.AddCall(this.m_pServerTransaction.Dialog, transaction.Dialog);
				}
			}
			else
			{
				this.m_pServerTransaction.SendResponse(response);
			}
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00028FE0 File Offset: 0x00027FE0
		private void CancelAllTargets()
		{
			bool flag = !this.m_NoCancel;
			if (flag)
			{
				this.m_pTargets.Clear();
				foreach (SIP_ProxyContext.TargetHandler targetHandler in this.m_pTargetsHandlers.ToArray())
				{
					targetHandler.Cancel();
				}
			}
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00029034 File Offset: 0x00028034
		private SIP_Response GetBestFinalResponse()
		{
			foreach (SIP_Response sip_Response in this.m_pResponses.ToArray())
			{
				bool flag = sip_Response.StatusCodeType == SIP_StatusCodeType.GlobalFailure;
				if (flag)
				{
					return sip_Response;
				}
			}
			foreach (SIP_Response sip_Response2 in this.m_pResponses.ToArray())
			{
				bool flag2 = sip_Response2.StatusCodeType == SIP_StatusCodeType.Success;
				if (flag2)
				{
					return sip_Response2;
				}
			}
			foreach (SIP_Response sip_Response3 in this.m_pResponses.ToArray())
			{
				bool flag3 = sip_Response3.StatusCodeType == SIP_StatusCodeType.Redirection;
				if (flag3)
				{
					return sip_Response3;
				}
			}
			foreach (SIP_Response sip_Response4 in this.m_pResponses.ToArray())
			{
				bool flag4 = sip_Response4.StatusCodeType == SIP_StatusCodeType.RequestFailure;
				if (flag4)
				{
					return sip_Response4;
				}
			}
			foreach (SIP_Response sip_Response5 in this.m_pResponses.ToArray())
			{
				bool flag5 = sip_Response5.StatusCodeType == SIP_StatusCodeType.ServerFailure;
				if (flag5)
				{
					return sip_Response5;
				}
			}
			return null;
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x0002918C File Offset: 0x0002818C
		private NetworkCredential GetCredential(string realm)
		{
			bool flag = realm == null;
			if (flag)
			{
				throw new ArgumentNullException("realm");
			}
			foreach (NetworkCredential networkCredential in this.m_pCredentials)
			{
				bool flag2 = networkCredential.Domain.ToLower() == realm.ToLower();
				if (flag2)
				{
					return networkCredential;
				}
			}
			return null;
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x00029218 File Offset: 0x00028218
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x00029230 File Offset: 0x00028230
		public SIP_Proxy Proxy
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pProxy;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060006D7 RID: 1751 RVA: 0x00029264 File Offset: 0x00028264
		public string ID
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_ID;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00029294 File Offset: 0x00028294
		public DateTime CreateTime
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_CreateTime;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x000292C4 File Offset: 0x000282C4
		public SIP_ForkingMode ForkingMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_ForkingMode;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x000292F4 File Offset: 0x000282F4
		public bool NoCancel
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_NoCancel;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x00029324 File Offset: 0x00028324
		public bool Recurse
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return !this.m_NoRecurse;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00029358 File Offset: 0x00028358
		public SIP_ServerTransaction ServerTransaction
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_pServerTransaction;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00029388 File Offset: 0x00028388
		public SIP_Request Request
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SIP_ProxyContext");
				}
				return this.m_pRequest;
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x000293B8 File Offset: 0x000283B8
		public SIP_Response[] Responses
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pResponses.ToArray();
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x000293F4 File Offset: 0x000283F4
		public List<NetworkCredential> Credentials
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pCredentials;
			}
		}

		// Token: 0x040002D1 RID: 721
		private bool m_IsDisposed = false;

		// Token: 0x040002D2 RID: 722
		private bool m_IsStarted = false;

		// Token: 0x040002D3 RID: 723
		private SIP_Proxy m_pProxy = null;

		// Token: 0x040002D4 RID: 724
		private SIP_ServerTransaction m_pServerTransaction = null;

		// Token: 0x040002D5 RID: 725
		private SIP_Request m_pRequest = null;

		// Token: 0x040002D6 RID: 726
		private bool m_AddRecordRoute = false;

		// Token: 0x040002D7 RID: 727
		private SIP_ForkingMode m_ForkingMode = SIP_ForkingMode.Parallel;

		// Token: 0x040002D8 RID: 728
		private bool m_IsB2BUA = true;

		// Token: 0x040002D9 RID: 729
		private bool m_NoCancel = false;

		// Token: 0x040002DA RID: 730
		private bool m_NoRecurse = true;

		// Token: 0x040002DB RID: 731
		private string m_ID = "";

		// Token: 0x040002DC RID: 732
		private DateTime m_CreateTime;

		// Token: 0x040002DD RID: 733
		private List<SIP_ProxyContext.TargetHandler> m_pTargetsHandlers = null;

		// Token: 0x040002DE RID: 734
		private List<SIP_Response> m_pResponses = null;

		// Token: 0x040002DF RID: 735
		private Queue<SIP_ProxyContext.TargetHandler> m_pTargets = null;

		// Token: 0x040002E0 RID: 736
		private List<NetworkCredential> m_pCredentials = null;

		// Token: 0x040002E1 RID: 737
		private bool m_IsFinalResponseSent = false;

		// Token: 0x040002E2 RID: 738
		private object m_pLock = new object();

		// Token: 0x02000290 RID: 656
		private class TargetHandler : IDisposable
		{
			// Token: 0x06001716 RID: 5910 RVA: 0x0008EF8C File Offset: 0x0008DF8C
			public TargetHandler(SIP_ProxyContext owner, SIP_Flow flow, SIP_Uri targetUri, bool addRecordRoute, bool isRecursed)
			{
				bool flag = owner == null;
				if (flag)
				{
					throw new ArgumentNullException("owner");
				}
				bool flag2 = targetUri == null;
				if (flag2)
				{
					throw new ArgumentNullException("targetUri");
				}
				this.m_pOwner = owner;
				this.m_pFlow = flow;
				this.m_pTargetUri = targetUri;
				this.m_AddRecordRoute = addRecordRoute;
				this.m_IsRecursed = isRecursed;
				this.m_pHops = new Queue<SIP_Hop>();
			}

			// Token: 0x06001717 RID: 5911 RVA: 0x0008F060 File Offset: 0x0008E060
			public void Dispose()
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (!isDisposed)
					{
						this.m_IsDisposed = true;
						this.m_pOwner.TargetHandler_Disposed(this);
						this.m_pOwner = null;
						this.m_pRequest = null;
						this.m_pTargetUri = null;
						this.m_pHops = null;
						bool flag2 = this.m_pTransaction != null;
						if (flag2)
						{
							this.m_pTransaction.Dispose();
							this.m_pTransaction = null;
						}
						bool flag3 = this.m_pTimerC != null;
						if (flag3)
						{
							this.m_pTimerC.Dispose();
							this.m_pTimerC = null;
						}
					}
				}
			}

			// Token: 0x06001718 RID: 5912 RVA: 0x0008F128 File Offset: 0x0008E128
			private void Init()
			{
				bool flag = false;
				this.m_pRequest = this.m_pOwner.Request.Copy();
				this.m_pRequest.RequestLine.Uri = this.m_pTargetUri;
				SIP_Request pRequest = this.m_pRequest;
				int maxForwards = pRequest.MaxForwards;
				pRequest.MaxForwards = maxForwards - 1;
				bool flag2 = this.m_pRequest.Route.GetAllValues().Length != 0 && !this.m_pRequest.Route.GetTopMostValue().Parameters.Contains("lr");
				if (flag2)
				{
					this.m_pRequest.Route.Add(this.m_pRequest.RequestLine.Uri.ToString());
					this.m_pRequest.RequestLine.Uri = SIP_Utils.UriToRequestUri(this.m_pRequest.Route.GetTopMostValue().Address.Uri);
					this.m_pRequest.Route.RemoveTopMostValue();
					flag = true;
				}
				bool flag3 = flag;
				SIP_Uri uri;
				if (flag3)
				{
					uri = (SIP_Uri)this.m_pRequest.RequestLine.Uri;
				}
				else
				{
					bool flag4 = this.m_pRequest.Route.GetTopMostValue() != null;
					if (flag4)
					{
						uri = (SIP_Uri)this.m_pRequest.Route.GetTopMostValue().Address.Uri;
					}
					else
					{
						uri = (SIP_Uri)this.m_pRequest.RequestLine.Uri;
					}
				}
				foreach (SIP_Hop item in this.m_pOwner.Proxy.Stack.GetHops(uri, this.m_pRequest.ToByteData().Length, ((SIP_Uri)this.m_pRequest.RequestLine.Uri).IsSecure))
				{
					this.m_pHops.Enqueue(item);
				}
				bool flag5 = this.m_pHops.Count > 0 && this.m_AddRecordRoute && this.m_pRequest.RequestLine.Method != "ACK";
				if (flag5)
				{
					string recordRoute = this.m_pOwner.Proxy.Stack.TransportLayer.GetRecordRoute(this.m_pHops.Peek().Transport);
					bool flag6 = recordRoute != null;
					if (flag6)
					{
						this.m_pRequest.RecordRoute.Add(recordRoute);
					}
				}
			}

			// Token: 0x06001719 RID: 5913 RVA: 0x0008F38C File Offset: 0x0008E38C
			private void ClientTransaction_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					this.m_HasReceivedResponse = true;
					bool flag2 = this.m_pTimerC != null && e.Response.StatusCode >= 101 && e.Response.StatusCode <= 199;
					if (flag2)
					{
						this.m_pTimerC.Interval = 180000.0;
					}
					bool flag3 = e.Response.StatusCodeType > SIP_StatusCodeType.Provisional;
					if (flag3)
					{
						this.m_IsCompleted = true;
					}
					this.m_pOwner.ProcessResponse(this, this.m_pTransaction, e.Response);
				}
			}

			// Token: 0x0600171A RID: 5914 RVA: 0x0008F450 File Offset: 0x0008E450
			private void ClientTransaction_TimedOut(object sender, EventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool flag2 = this.m_pHops.Count > 0;
					if (flag2)
					{
						this.CleanUpActiveHop();
						this.SendToNextHop();
					}
					else
					{
						this.m_IsCompleted = true;
						this.m_pOwner.ProcessResponse(this, this.m_pTransaction, this.m_pOwner.Proxy.Stack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pTransaction.Request));
						this.Dispose();
					}
				}
			}

			// Token: 0x0600171B RID: 5915 RVA: 0x0008F4FC File Offset: 0x0008E4FC
			private void ClientTransaction_TransportError(object sender, ExceptionEventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool flag2 = this.m_pHops.Count > 0;
					if (flag2)
					{
						this.CleanUpActiveHop();
						this.SendToNextHop();
					}
					else
					{
						this.m_IsCompleted = true;
						this.m_pOwner.ProcessResponse(this, this.m_pTransaction, this.m_pOwner.Proxy.Stack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pTransaction.Request));
						this.Dispose();
					}
				}
			}

			// Token: 0x0600171C RID: 5916 RVA: 0x0008F5A8 File Offset: 0x0008E5A8
			private void m_pTransaction_Disposed(object sender, EventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (!isDisposed)
					{
						bool hasReceivedResponse = this.HasReceivedResponse;
						if (hasReceivedResponse)
						{
							this.Dispose();
						}
					}
				}
			}

			// Token: 0x0600171D RID: 5917 RVA: 0x0008F608 File Offset: 0x0008E608
			private void m_pTimerC_Elapsed(object sender, ElapsedEventArgs e)
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool hasProvisionalResponse = this.m_pTransaction.HasProvisionalResponse;
					if (hasProvisionalResponse)
					{
						this.m_pTransaction.Cancel();
					}
					else
					{
						this.m_pOwner.ProcessResponse(this, this.m_pTransaction, this.m_pOwner.Proxy.Stack.CreateResponse(SIP_ResponseCodes.x408_Request_Timeout, this.m_pTransaction.Request));
						this.Dispose();
					}
				}
			}

			// Token: 0x0600171E RID: 5918 RVA: 0x0008F6A8 File Offset: 0x0008E6A8
			public void Start()
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool isStarted = this.m_IsStarted;
					if (isStarted)
					{
						throw new InvalidOperationException("Start has already called.");
					}
					this.m_IsStarted = true;
					this.Init();
					bool flag2 = this.m_pHops.Count == 0;
					if (flag2)
					{
						this.m_pOwner.ProcessResponse(this, this.m_pTransaction, this.m_pOwner.Proxy.Stack.CreateResponse(SIP_ResponseCodes.x503_Service_Unavailable + ": No hop(s) for target.", this.m_pTransaction.Request));
						this.Dispose();
					}
					else
					{
						bool flag3 = this.m_pFlow != null;
						if (flag3)
						{
							this.SendToFlow(this.m_pFlow, this.m_pRequest.Copy());
						}
						else
						{
							this.SendToNextHop();
						}
					}
				}
			}

			// Token: 0x0600171F RID: 5919 RVA: 0x0008F7C0 File Offset: 0x0008E7C0
			public void Cancel()
			{
				object pLock = this.m_pLock;
				lock (pLock)
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					bool isStarted = this.m_IsStarted;
					if (isStarted)
					{
						this.m_pTransaction.Cancel();
					}
					else
					{
						this.Dispose();
					}
				}
			}

			// Token: 0x06001720 RID: 5920 RVA: 0x0008F840 File Offset: 0x0008E840
			private void SendToNextHop()
			{
				bool flag = this.m_pHops.Count == 0;
				if (flag)
				{
					throw new InvalidOperationException("No more hop(s).");
				}
				SIP_Hop sip_Hop = this.m_pHops.Dequeue();
				this.SendToFlow(this.m_pOwner.Proxy.Stack.TransportLayer.GetOrCreateFlow(sip_Hop.Transport, null, sip_Hop.EndPoint), this.m_pRequest.Copy());
			}

			// Token: 0x06001721 RID: 5921 RVA: 0x0008F8B4 File Offset: 0x0008E8B4
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
				bool flag3 = this.m_AddRecordRoute && request.From.Tag != null && request.RecordRoute.GetAllValues().Length != 0;
				if (flag3)
				{
					string value = string.Concat(new string[]
					{
						request.From.Tag,
						":",
						this.m_pOwner.ServerTransaction.Flow.ID,
						"/",
						flow.ID
					});
					((SIP_Uri)request.RecordRoute.GetTopMostValue().Address.Uri).Parameters.Add("flowInfo", value);
				}
				this.m_pTransaction = this.m_pOwner.Proxy.Stack.TransactionLayer.CreateClientTransaction(flow, request, true);
				this.m_pTransaction.ResponseReceived += this.ClientTransaction_ResponseReceived;
				this.m_pTransaction.TimedOut += this.ClientTransaction_TimedOut;
				this.m_pTransaction.TransportError += this.ClientTransaction_TransportError;
				this.m_pTransaction.Disposed += this.m_pTransaction_Disposed;
				this.m_pTransaction.Start();
				bool flag4 = request.RequestLine.Method == "INVITE";
				if (flag4)
				{
					this.m_pTimerC = new TimerEx();
					this.m_pTimerC.AutoReset = false;
					this.m_pTimerC.Interval = 180000.0;
					this.m_pTimerC.Elapsed += this.m_pTimerC_Elapsed;
				}
			}

			// Token: 0x06001722 RID: 5922 RVA: 0x0008FA80 File Offset: 0x0008EA80
			private void CleanUpActiveHop()
			{
				bool flag = this.m_pTimerC != null;
				if (flag)
				{
					this.m_pTimerC.Dispose();
					this.m_pTimerC = null;
				}
				bool flag2 = this.m_pTransaction != null;
				if (flag2)
				{
					this.m_pTransaction.Dispose();
					this.m_pTransaction = null;
				}
			}

			// Token: 0x17000786 RID: 1926
			// (get) Token: 0x06001723 RID: 5923 RVA: 0x0008FAD4 File Offset: 0x0008EAD4
			public bool IsDisposed
			{
				get
				{
					return this.m_IsDisposed;
				}
			}

			// Token: 0x17000787 RID: 1927
			// (get) Token: 0x06001724 RID: 5924 RVA: 0x0008FAEC File Offset: 0x0008EAEC
			public bool IsStarted
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_IsStarted;
				}
			}

			// Token: 0x17000788 RID: 1928
			// (get) Token: 0x06001725 RID: 5925 RVA: 0x0008FB20 File Offset: 0x0008EB20
			public bool IsCompleted
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_IsCompleted;
				}
			}

			// Token: 0x17000789 RID: 1929
			// (get) Token: 0x06001726 RID: 5926 RVA: 0x0008FB54 File Offset: 0x0008EB54
			public SIP_Request Request
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_pRequest;
				}
			}

			// Token: 0x1700078A RID: 1930
			// (get) Token: 0x06001727 RID: 5927 RVA: 0x0008FB88 File Offset: 0x0008EB88
			public SIP_Uri TargetUri
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_pTargetUri;
				}
			}

			// Token: 0x1700078B RID: 1931
			// (get) Token: 0x06001728 RID: 5928 RVA: 0x0008FBBC File Offset: 0x0008EBBC
			public bool IsRecordingRoute
			{
				get
				{
					return this.m_AddRecordRoute;
				}
			}

			// Token: 0x1700078C RID: 1932
			// (get) Token: 0x06001729 RID: 5929 RVA: 0x0008FBD4 File Offset: 0x0008EBD4
			public bool IsRecursed
			{
				get
				{
					return this.m_IsRecursed;
				}
			}

			// Token: 0x1700078D RID: 1933
			// (get) Token: 0x0600172A RID: 5930 RVA: 0x0008FBEC File Offset: 0x0008EBEC
			public bool HasReceivedResponse
			{
				get
				{
					bool isDisposed = this.m_IsDisposed;
					if (isDisposed)
					{
						throw new ObjectDisposedException(base.GetType().Name);
					}
					return this.m_HasReceivedResponse;
				}
			}

			// Token: 0x040009A2 RID: 2466
			private object m_pLock = new object();

			// Token: 0x040009A3 RID: 2467
			private bool m_IsDisposed = false;

			// Token: 0x040009A4 RID: 2468
			private bool m_IsStarted = false;

			// Token: 0x040009A5 RID: 2469
			private SIP_ProxyContext m_pOwner = null;

			// Token: 0x040009A6 RID: 2470
			private SIP_Request m_pRequest = null;

			// Token: 0x040009A7 RID: 2471
			private SIP_Flow m_pFlow = null;

			// Token: 0x040009A8 RID: 2472
			private SIP_Uri m_pTargetUri = null;

			// Token: 0x040009A9 RID: 2473
			private bool m_AddRecordRoute = true;

			// Token: 0x040009AA RID: 2474
			private bool m_IsRecursed = false;

			// Token: 0x040009AB RID: 2475
			private Queue<SIP_Hop> m_pHops = null;

			// Token: 0x040009AC RID: 2476
			private SIP_ClientTransaction m_pTransaction = null;

			// Token: 0x040009AD RID: 2477
			private TimerEx m_pTimerC = null;

			// Token: 0x040009AE RID: 2478
			private bool m_HasReceivedResponse = false;

			// Token: 0x040009AF RID: 2479
			private bool m_IsCompleted = false;
		}
	}
}
