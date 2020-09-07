using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000B7 RID: 183
	public class SIP_Proxy : IDisposable
	{
		// Token: 0x060006F5 RID: 1781 RVA: 0x0002979C File Offset: 0x0002879C
		public SIP_Proxy(SIP_Stack stack)
		{
			bool flag = stack == null;
			if (flag)
			{
				throw new ArgumentNullException("stack");
			}
			this.m_pStack = stack;
			this.m_pStack.RequestReceived += this.m_pStack_RequestReceived;
			this.m_pStack.ResponseReceived += this.m_pStack_ResponseReceived;
			this.m_pRegistrar = new SIP_Registrar(this);
			this.m_pB2BUA = new SIP_B2BUA(this);
			this.m_Opaque = Auth_HttpDigest.CreateOpaque();
			this.m_pProxyContexts = new List<SIP_ProxyContext>();
			this.m_pHandlers = new List<SIP_ProxyHandler>();
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00029890 File Offset: 0x00028890
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				bool flag = this.m_pStack != null;
				if (flag)
				{
					this.m_pStack.Dispose();
					this.m_pStack = null;
				}
				this.m_pRegistrar = null;
				this.m_pB2BUA = null;
				this.m_pProxyContexts = null;
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x000298E9 File Offset: 0x000288E9
		private void m_pStack_RequestReceived(object sender, SIP_RequestReceivedEventArgs e)
		{
			this.OnRequestReceived(e);
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x000298F4 File Offset: 0x000288F4
		private void m_pStack_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			this.OnResponseReceived(e);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00029900 File Offset: 0x00028900
		private void OnRequestReceived(SIP_RequestReceivedEventArgs e)
		{
			SIP_Request request = e.Request;
			try
			{
				bool flag = (this.m_ProxyMode & SIP_ProxyMode.Statefull) > (SIP_ProxyMode)0;
				if (flag)
				{
					bool flag2 = e.Request.RequestLine.Method == "CANCEL";
					if (flag2)
					{
						SIP_ServerTransaction sip_ServerTransaction = this.m_pStack.TransactionLayer.MatchCancelToTransaction(e.Request);
						bool flag3 = sip_ServerTransaction != null;
						if (flag3)
						{
							sip_ServerTransaction.Cancel();
							e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x200_Ok, request));
						}
						else
						{
							this.ForwardRequest(false, e, true);
						}
					}
					else
					{
						bool flag4 = e.Request.RequestLine.Method == "ACK";
						if (flag4)
						{
							this.ForwardRequest(false, e, true);
						}
						else
						{
							this.ForwardRequest(true, e, true);
						}
					}
				}
				else
				{
					bool flag5 = (this.m_ProxyMode & SIP_ProxyMode.B2BUA) > (SIP_ProxyMode)0;
					if (flag5)
					{
						this.m_pB2BUA.OnRequestReceived(e);
					}
					else
					{
						bool flag6 = (this.m_ProxyMode & SIP_ProxyMode.Stateless) > (SIP_ProxyMode)0;
						if (flag6)
						{
							this.ForwardRequest(false, e, true);
						}
						else
						{
							e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x501_Not_Implemented, request));
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					this.m_pStack.TransportLayer.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x500_Server_Internal_Error + ": " + ex.Message, e.Request));
				}
				catch
				{
				}
				bool flag7 = !(ex is SIP_TransportException);
				if (flag7)
				{
					this.m_pStack.OnError(ex);
				}
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00029AE0 File Offset: 0x00028AE0
		private void OnResponseReceived(SIP_ResponseReceivedEventArgs e)
		{
			bool flag = (this.m_ProxyMode & SIP_ProxyMode.B2BUA) > (SIP_ProxyMode)0;
			if (flag)
			{
				this.m_pB2BUA.OnResponseReceived(e);
			}
			else
			{
				bool flag2 = (this.m_ProxyMode & SIP_ProxyMode.Statefull) > (SIP_ProxyMode)0;
				if (!flag2)
				{
					e.Response.Via.RemoveTopMostValue();
					bool flag3 = (this.m_ProxyMode & SIP_ProxyMode.Statefull) > (SIP_ProxyMode)0;
					if (flag3)
					{
						this.m_pStack.TransportLayer.SendResponse(e.Response);
					}
					else
					{
						bool flag4 = (this.m_ProxyMode & SIP_ProxyMode.Stateless) > (SIP_ProxyMode)0;
						if (flag4)
						{
							this.m_pStack.TransportLayer.SendResponse(e.Response);
						}
					}
				}
			}
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00029B88 File Offset: 0x00028B88
		internal void ForwardRequest(bool statefull, SIP_RequestReceivedEventArgs e, bool addRecordRoute)
		{
			SIP_RequestContext sip_RequestContext = new SIP_RequestContext(this, e.Request, e.Flow);
			SIP_Request request = e.Request;
			SIP_Uri sip_Uri = null;
			bool flag = request.MaxForwards <= 0;
			if (flag)
			{
				e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x483_Too_Many_Hops, request));
			}
			else
			{
				bool flag2 = !SIP_Utils.IsSipOrSipsUri(request.RequestLine.Uri.ToString()) || !this.OnIsLocalUri(((SIP_Uri)request.RequestLine.Uri).Host);
				if (flag2)
				{
					bool flag3 = false;
					bool isSipOrSipsUri = request.To.Address.IsSipOrSipsUri;
					if (isSipOrSipsUri)
					{
						SIP_Registration registration = this.m_pRegistrar.GetRegistration(((SIP_Uri)request.To.Address.Uri).Address);
						bool flag4 = registration != null;
						if (flag4)
						{
							bool flag5 = registration.GetBinding(request.RequestLine.Uri) != null;
							if (flag5)
							{
								flag3 = true;
							}
						}
					}
					bool flag6 = !flag3;
					if (flag6)
					{
						string user = null;
						bool flag7 = request.RequestLine.Method == "ACK";
						if (!flag7)
						{
							bool flag8 = !this.AuthenticateRequest(e, out user);
							if (flag8)
							{
								return;
							}
						}
						sip_RequestContext.SetUser(user);
					}
				}
				bool flag9 = request.RequestLine.Uri is SIP_Uri && this.IsRecordRoute((SIP_Uri)request.RequestLine.Uri) && request.Route.GetAllValues().Length != 0;
				if (flag9)
				{
					request.RequestLine.Uri = request.Route.GetAllValues()[request.Route.GetAllValues().Length - 1].Address.Uri;
					SIP_t_AddressParam[] allValues = request.Route.GetAllValues();
					sip_Uri = (SIP_Uri)allValues[allValues.Length - 1].Address.Uri;
					request.Route.RemoveLastValue();
				}
				bool flag10 = request.Route.GetAllValues().Length != 0;
				if (flag10)
				{
					sip_Uri = (SIP_Uri)request.Route.GetTopMostValue().Address.Uri;
					bool param_Lr = sip_Uri.Param_Lr;
					if (param_Lr)
					{
						request.Route.RemoveTopMostValue();
					}
					else
					{
						bool flag11 = this.IsLocalRoute(sip_Uri);
						if (flag11)
						{
							request.Route.RemoveTopMostValue();
						}
					}
				}
				bool flag12 = e.Request.RequestLine.Method == "REGISTER";
				if (flag12)
				{
					SIP_Uri sip_Uri2 = (SIP_Uri)e.Request.RequestLine.Uri;
					bool flag13 = this.OnIsLocalUri(sip_Uri2.Host);
					if (flag13)
					{
						bool flag14 = (this.m_ProxyMode & SIP_ProxyMode.Registrar) > (SIP_ProxyMode)0;
						if (flag14)
						{
							this.m_pRegistrar.Register(e);
							return;
						}
						e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x405_Method_Not_Allowed, e.Request));
						return;
					}
				}
				bool flag15 = e.Request.RequestLine.Uri is SIP_Uri;
				if (flag15)
				{
					SIP_Uri sip_Uri3 = (SIP_Uri)e.Request.RequestLine.Uri;
					bool flag16 = !this.OnIsLocalUri(sip_Uri3.Host);
					if (flag16)
					{
						SIP_Flow flow = null;
						string text = (sip_Uri != null && sip_Uri.Parameters["flowInfo"] != null) ? sip_Uri.Parameters["flowInfo"].Value : null;
						bool flag17 = text != null && request.To.Tag != null;
						if (flag17)
						{
							string a = text.Substring(0, text.IndexOf(':'));
							string flowID = text.Substring(text.IndexOf(':') + 1, text.IndexOf('/') - text.IndexOf(':') - 1);
							string flowID2 = text.Substring(text.IndexOf('/') + 1);
							bool flag18 = a == request.To.Tag;
							if (flag18)
							{
								flow = this.m_pStack.TransportLayer.GetFlow(flowID);
							}
							else
							{
								flow = this.m_pStack.TransportLayer.GetFlow(flowID2);
							}
						}
						sip_RequestContext.Targets.Add(new SIP_ProxyTarget(sip_Uri3, flow));
					}
					else
					{
						SIP_Registration registration2 = this.m_pRegistrar.GetRegistration(sip_Uri3.Address);
						bool flag19 = registration2 != null;
						if (flag19)
						{
							foreach (SIP_RegistrationBinding sip_RegistrationBinding in registration2.Bindings)
							{
								bool flag20 = sip_RegistrationBinding.ContactURI is SIP_Uri && sip_RegistrationBinding.TTL > 0;
								if (flag20)
								{
									sip_RequestContext.Targets.Add(new SIP_ProxyTarget((SIP_Uri)sip_RegistrationBinding.ContactURI, sip_RegistrationBinding.Flow));
								}
							}
						}
						else
						{
							bool flag21 = !this.OnAddressExists(sip_Uri3.Address);
							if (flag21)
							{
								e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x404_Not_Found, e.Request));
								return;
							}
						}
						bool flag22 = sip_RequestContext.Targets.Count == 0;
						if (flag22)
						{
							e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x480_Temporarily_Unavailable, e.Request));
							return;
						}
					}
				}
				foreach (SIP_ProxyHandler sip_ProxyHandler in this.Handlers)
				{
					try
					{
						SIP_ProxyHandler sip_ProxyHandler2 = sip_ProxyHandler;
						bool flag23 = !sip_ProxyHandler.IsReusable;
						if (flag23)
						{
							sip_ProxyHandler2 = (SIP_ProxyHandler)Activator.CreateInstance(sip_ProxyHandler.GetType());
						}
						bool flag24 = sip_ProxyHandler2.ProcessRequest(sip_RequestContext);
						if (flag24)
						{
							return;
						}
					}
					catch (Exception x)
					{
						this.m_pStack.OnError(x);
					}
				}
				bool flag25 = sip_RequestContext.Targets.Count == 0 && !SIP_Utils.IsSipOrSipsUri(request.RequestLine.Uri.ToString());
				if (flag25)
				{
					e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x416_Unsupported_URI_Scheme, e.Request));
				}
				else if (statefull)
				{
					SIP_ProxyContext sip_ProxyContext = this.CreateProxyContext(sip_RequestContext, e.ServerTransaction, request, addRecordRoute);
					sip_ProxyContext.Start();
				}
				else
				{
					bool flag26 = false;
					SIP_Hop[] array = null;
					SIP_Request sip_Request = request.Copy();
					sip_Request.RequestLine.Uri = sip_RequestContext.Targets[0].TargetUri;
					SIP_Request sip_Request2 = sip_Request;
					int maxForwards = sip_Request2.MaxForwards;
					sip_Request2.MaxForwards = maxForwards - 1;
					bool flag27 = sip_Request.Route.GetAllValues().Length != 0 && !sip_Request.Route.GetTopMostValue().Parameters.Contains("lr");
					if (flag27)
					{
						sip_Request.Route.Add(sip_Request.RequestLine.Uri.ToString());
						sip_Request.RequestLine.Uri = SIP_Utils.UriToRequestUri(sip_Request.Route.GetTopMostValue().Address.Uri);
						sip_Request.Route.RemoveTopMostValue();
						flag26 = true;
					}
					bool flag28 = flag26;
					SIP_Uri uri;
					if (flag28)
					{
						uri = (SIP_Uri)sip_Request.RequestLine.Uri;
					}
					else
					{
						bool flag29 = sip_Request.Route.GetTopMostValue() != null;
						if (flag29)
						{
							uri = (SIP_Uri)sip_Request.Route.GetTopMostValue().Address.Uri;
						}
						else
						{
							uri = (SIP_Uri)sip_Request.RequestLine.Uri;
						}
					}
					array = this.m_pStack.GetHops(uri, sip_Request.ToByteData().Length, ((SIP_Uri)sip_Request.RequestLine.Uri).IsSecure);
					bool flag30 = array.Length == 0;
					if (flag30)
					{
						bool flag31 = sip_Request.RequestLine.Method != "ACK";
						if (flag31)
						{
							e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x503_Service_Unavailable + ": No hop(s) for target.", sip_Request));
						}
					}
					else
					{
						sip_Request.Via.AddToTop("SIP/2.0/transport-tl-addign sentBy-tl-assign-it;branch=z9hG4bK-" + Net_Utils.ComputeMd5(request.Via.GetTopMostValue().Branch, true));
						sip_Request.Via.GetTopMostValue().Parameters.Add("flowID", request.Flow.ID);
						try
						{
							try
							{
								bool flag32 = sip_RequestContext.Targets[0].Flow != null;
								if (flag32)
								{
									this.m_pStack.TransportLayer.SendRequest(sip_RequestContext.Targets[0].Flow, request);
								}
							}
							catch
							{
								this.m_pStack.TransportLayer.SendRequest(request, null, array[0]);
							}
						}
						catch (SIP_TransportException ex)
						{
							string message = ex.Message;
							bool flag33 = sip_Request.RequestLine.Method != "ACK";
							if (flag33)
							{
								e.ServerTransaction.SendResponse(this.m_pStack.CreateResponse(SIP_ResponseCodes.x503_Service_Unavailable + ": Transport error.", sip_Request));
							}
						}
					}
				}
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002A504 File Offset: 0x00029504
		internal bool AuthenticateRequest(SIP_RequestReceivedEventArgs e)
		{
			string text = null;
			return this.AuthenticateRequest(e, out text);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0002A524 File Offset: 0x00029524
		internal bool AuthenticateRequest(SIP_RequestReceivedEventArgs e, out string userName)
		{
			userName = null;
			SIP_t_Credentials credentials = SIP_Utils.GetCredentials(e.Request, this.m_pStack.Realm);
			bool flag = credentials == null;
			bool result;
			if (flag)
			{
				SIP_Response sip_Response = this.m_pStack.CreateResponse(SIP_ResponseCodes.x407_Proxy_Authentication_Required, e.Request);
				sip_Response.ProxyAuthenticate.Add(new Auth_HttpDigest(this.m_pStack.Realm, this.m_pStack.DigestNonceManager.CreateNonce(), this.m_Opaque).ToChallenge());
				e.ServerTransaction.SendResponse(sip_Response);
				result = false;
			}
			else
			{
				Auth_HttpDigest auth_HttpDigest = new Auth_HttpDigest(credentials.AuthData, e.Request.RequestLine.Method);
				bool flag2 = auth_HttpDigest.Opaque != this.m_Opaque;
				if (flag2)
				{
					SIP_Response sip_Response2 = this.m_pStack.CreateResponse(SIP_ResponseCodes.x407_Proxy_Authentication_Required + ": Opaque value won't match !", e.Request);
					sip_Response2.ProxyAuthenticate.Add(new Auth_HttpDigest(this.m_pStack.Realm, this.m_pStack.DigestNonceManager.CreateNonce(), this.m_Opaque).ToChallenge());
					e.ServerTransaction.SendResponse(sip_Response2);
					result = false;
				}
				else
				{
					bool flag3 = !this.m_pStack.DigestNonceManager.NonceExists(auth_HttpDigest.Nonce);
					if (flag3)
					{
						SIP_Response sip_Response3 = this.m_pStack.CreateResponse(SIP_ResponseCodes.x407_Proxy_Authentication_Required + ": Invalid nonce value !", e.Request);
						sip_Response3.ProxyAuthenticate.Add(new Auth_HttpDigest(this.m_pStack.Realm, this.m_pStack.DigestNonceManager.CreateNonce(), this.m_Opaque).ToChallenge());
						e.ServerTransaction.SendResponse(sip_Response3);
						result = false;
					}
					else
					{
						this.m_pStack.DigestNonceManager.RemoveNonce(auth_HttpDigest.Nonce);
						SIP_AuthenticateEventArgs sip_AuthenticateEventArgs = this.OnAuthenticate(auth_HttpDigest);
						bool flag4 = !sip_AuthenticateEventArgs.Authenticated;
						if (flag4)
						{
							SIP_Response sip_Response4 = this.m_pStack.CreateResponse(SIP_ResponseCodes.x407_Proxy_Authentication_Required + ": Authentication failed.", e.Request);
							sip_Response4.ProxyAuthenticate.Add(new Auth_HttpDigest(this.m_pStack.Realm, this.m_pStack.DigestNonceManager.CreateNonce(), this.m_Opaque).ToChallenge());
							e.ServerTransaction.SendResponse(sip_Response4);
							result = false;
						}
						else
						{
							userName = auth_HttpDigest.UserName;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0002A79C File Offset: 0x0002979C
		internal bool IsLocalRoute(SIP_Uri uri)
		{
			bool flag = uri == null;
			if (flag)
			{
				throw new ArgumentNullException("uri");
			}
			bool flag2 = uri.User != null;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (IPBindInfo ipbindInfo in this.m_pStack.BindInfo)
				{
					bool flag3 = uri.Host.ToLower() == ipbindInfo.HostName.ToLower();
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0002A824 File Offset: 0x00029824
		private bool IsRecordRoute(SIP_Uri route)
		{
			bool flag = route == null;
			if (flag)
			{
				throw new ArgumentNullException("route");
			}
			foreach (IPBindInfo ipbindInfo in this.m_pStack.BindInfo)
			{
				bool flag2 = route.Host.ToLower() == ipbindInfo.HostName.ToLower();
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0002A898 File Offset: 0x00029898
		internal SIP_ProxyContext CreateProxyContext(SIP_RequestContext requestContext, SIP_ServerTransaction transaction, SIP_Request request, bool addRecordRoute)
		{
			SIP_ProxyContext sip_ProxyContext = new SIP_ProxyContext(this, transaction, request, addRecordRoute, this.m_ForkingMode, (this.ProxyMode & SIP_ProxyMode.B2BUA) > (SIP_ProxyMode)0, false, false, requestContext.Targets.ToArray());
			this.m_pProxyContexts.Add(sip_ProxyContext);
			return sip_ProxyContext;
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x0002A8E4 File Offset: 0x000298E4
		public bool IsDisposed
		{
			get
			{
				return this.m_IsDisposed;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000702 RID: 1794 RVA: 0x0002A8FC File Offset: 0x000298FC
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

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x0002A930 File Offset: 0x00029930
		// (set) Token: 0x06000704 RID: 1796 RVA: 0x0002A964 File Offset: 0x00029964
		public SIP_ProxyMode ProxyMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_ProxyMode;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				bool flag = (value & SIP_ProxyMode.Statefull) != (SIP_ProxyMode)0 && (value & SIP_ProxyMode.Stateless) > (SIP_ProxyMode)0;
				if (flag)
				{
					throw new ArgumentException("Proxy can't be at Statefull and Stateless at same time !");
				}
				this.m_ProxyMode = value;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x0002A9B4 File Offset: 0x000299B4
		// (set) Token: 0x06000706 RID: 1798 RVA: 0x0002A9E8 File Offset: 0x000299E8
		public SIP_ForkingMode ForkingMode
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_ForkingMode;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.m_ForkingMode = value;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0002AA1C File Offset: 0x00029A1C
		public SIP_Registrar Registrar
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pRegistrar;
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0002AA50 File Offset: 0x00029A50
		public SIP_B2BUA B2BUA
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pB2BUA;
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0002AA84 File Offset: 0x00029A84
		public List<SIP_ProxyHandler> Handlers
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return this.m_pHandlers;
			}
		}

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x0600070A RID: 1802 RVA: 0x0002AAB8 File Offset: 0x00029AB8
		// (remove) Token: 0x0600070B RID: 1803 RVA: 0x0002AAF0 File Offset: 0x00029AF0
		
		public event SIP_IsLocalUriEventHandler IsLocalUri = null;

		// Token: 0x0600070C RID: 1804 RVA: 0x0002AB28 File Offset: 0x00029B28
		internal bool OnIsLocalUri(string uri)
		{
			bool flag = this.IsLocalUri != null;
			return !flag || this.IsLocalUri(uri);
		}

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x0600070D RID: 1805 RVA: 0x0002AB58 File Offset: 0x00029B58
		// (remove) Token: 0x0600070E RID: 1806 RVA: 0x0002AB90 File Offset: 0x00029B90
		
		public event SIP_AuthenticateEventHandler Authenticate = null;

		// Token: 0x0600070F RID: 1807 RVA: 0x0002ABC8 File Offset: 0x00029BC8
		internal SIP_AuthenticateEventArgs OnAuthenticate(Auth_HttpDigest auth)
		{
			SIP_AuthenticateEventArgs sip_AuthenticateEventArgs = new SIP_AuthenticateEventArgs(auth);
			bool flag = this.Authenticate != null;
			if (flag)
			{
				this.Authenticate(sip_AuthenticateEventArgs);
			}
			return sip_AuthenticateEventArgs;
		}

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06000710 RID: 1808 RVA: 0x0002AC00 File Offset: 0x00029C00
		// (remove) Token: 0x06000711 RID: 1809 RVA: 0x0002AC38 File Offset: 0x00029C38
		
		public event SIP_AddressExistsEventHandler AddressExists = null;

		// Token: 0x06000712 RID: 1810 RVA: 0x0002AC70 File Offset: 0x00029C70
		internal bool OnAddressExists(string address)
		{
			bool flag = this.AddressExists != null;
			return flag && this.AddressExists(address);
		}

		// Token: 0x040002EE RID: 750
		private bool m_IsDisposed = false;

		// Token: 0x040002EF RID: 751
		private SIP_Stack m_pStack = null;

		// Token: 0x040002F0 RID: 752
		private SIP_ProxyMode m_ProxyMode = SIP_ProxyMode.Registrar | SIP_ProxyMode.Statefull;

		// Token: 0x040002F1 RID: 753
		private SIP_ForkingMode m_ForkingMode = SIP_ForkingMode.Parallel;

		// Token: 0x040002F2 RID: 754
		private SIP_Registrar m_pRegistrar = null;

		// Token: 0x040002F3 RID: 755
		private SIP_B2BUA m_pB2BUA = null;

		// Token: 0x040002F4 RID: 756
		private string m_Opaque = "";

		// Token: 0x040002F5 RID: 757
		internal List<SIP_ProxyContext> m_pProxyContexts = null;

		// Token: 0x040002F6 RID: 758
		private List<SIP_ProxyHandler> m_pHandlers = null;
	}
}
