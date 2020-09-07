using System;
using System.Collections.Generic;
using System.Diagnostics;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A6 RID: 166
	public class SIP_B2BUA : IDisposable
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x0002671D File Offset: 0x0002571D
		internal SIP_B2BUA(SIP_Proxy owner)
		{
			this.m_pProxy = owner;
			this.m_pCalls = new List<SIP_B2BUA_Call>();
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0002675C File Offset: 0x0002575C
		public void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				foreach (SIP_B2BUA_Call sip_B2BUA_Call in this.m_pCalls)
				{
					sip_B2BUA_Call.Terminate();
				}
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x000267C8 File Offset: 0x000257C8
		internal void OnRequestReceived(SIP_RequestReceivedEventArgs e)
		{
			SIP_Request request = e.Request;
			bool flag = request.RequestLine.Method == "CANCEL";
			if (flag)
			{
				SIP_ServerTransaction sip_ServerTransaction = this.m_pProxy.Stack.TransactionLayer.MatchCancelToTransaction(e.Request);
				bool flag2 = sip_ServerTransaction != null;
				if (flag2)
				{
					sip_ServerTransaction.Cancel();
				}
			}
			else
			{
				bool flag3 = request.RequestLine.Method == "BYE";
				if (!flag3)
				{
					bool flag4 = request.RequestLine.Method == "ACK";
					if (!flag4)
					{
						bool flag5 = request.RequestLine.Method == "OPTIONS";
						if (!flag5)
						{
							bool flag6 = request.RequestLine.Method == "PRACK";
							if (!flag6)
							{
								bool flag7 = request.RequestLine.Method == "UPDATE";
								if (!flag7)
								{
									SIP_Request sip_Request = e.Request.Copy();
									sip_Request.Via.RemoveAll();
									sip_Request.MaxForwards = 70;
									sip_Request.CallID = SIP_t_CallID.CreateCallID().CallID;
									sip_Request.CSeq.SequenceNumber = 1;
									sip_Request.Contact.RemoveAll();
									bool flag8 = sip_Request.Route.Count > 0 && this.m_pProxy.IsLocalRoute(SIP_Uri.Parse(sip_Request.Route.GetTopMostValue().Address.Uri.ToString()));
									if (flag8)
									{
										sip_Request.Route.RemoveTopMostValue();
									}
									sip_Request.RecordRoute.RemoveAll();
									foreach (SIP_SingleValueHF<SIP_t_Credentials> sip_SingleValueHF in sip_Request.ProxyAuthorization.HeaderFields)
									{
										try
										{
											Auth_HttpDigest auth_HttpDigest = new Auth_HttpDigest(sip_SingleValueHF.ValueX.AuthData, sip_Request.RequestLine.Method);
											bool flag9 = this.m_pProxy.Stack.Realm == auth_HttpDigest.Realm;
											if (flag9)
											{
												sip_Request.ProxyAuthorization.Remove(sip_SingleValueHF);
											}
										}
										catch
										{
										}
									}
									sip_Request.Allow.RemoveAll();
									sip_Request.Supported.RemoveAll();
									bool flag10 = request.RequestLine.Method != "ACK" && request.RequestLine.Method != "BYE";
									if (flag10)
									{
										sip_Request.Allow.Add("INVITE,ACK,OPTIONS,CANCEL,BYE,PRACK");
									}
									bool flag11 = request.RequestLine.Method != "ACK";
									if (flag11)
									{
										sip_Request.Supported.Add("100rel,timer");
									}
									sip_Request.Require.RemoveAll();
									bool flag12 = request.RequestLine.Method == "INVITE" || request.RequestLine.Method == "UPDATE";
									if (flag12)
									{
										sip_Request.SessionExpires = new SIP_t_SessionExpires(this.m_pProxy.Stack.SessionExpries, "uac");
										sip_Request.MinSE = new SIP_t_MinSE(this.m_pProxy.Stack.MinimumSessionExpries);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x000091B8 File Offset: 0x000081B8
		internal void OnResponseReceived(SIP_ResponseReceivedEventArgs e)
		{
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00026B38 File Offset: 0x00025B38
		internal void AddCall(SIP_Dialog caller, SIP_Dialog calee)
		{
			List<SIP_B2BUA_Call> pCalls = this.m_pCalls;
			lock (pCalls)
			{
				SIP_B2BUA_Call sip_B2BUA_Call = new SIP_B2BUA_Call(this, caller, calee);
				this.m_pCalls.Add(sip_B2BUA_Call);
				this.OnCallCreated(sip_B2BUA_Call);
			}
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x00026B98 File Offset: 0x00025B98
		internal void RemoveCall(SIP_B2BUA_Call call)
		{
			this.m_pCalls.Remove(call);
			this.OnCallTerminated(call);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00026BB0 File Offset: 0x00025BB0
		public SIP_B2BUA_Call GetCallByID(string callID)
		{
			foreach (SIP_B2BUA_Call sip_B2BUA_Call in this.m_pCalls.ToArray())
			{
				bool flag = sip_B2BUA_Call.CallID == callID;
				if (flag)
				{
					return sip_B2BUA_Call;
				}
			}
			return null;
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000675 RID: 1653 RVA: 0x00026BFC File Offset: 0x00025BFC
		public SIP_Stack Stack
		{
			get
			{
				return this.m_pProxy.Stack;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x00026C1C File Offset: 0x00025C1C
		public SIP_B2BUA_Call[] Calls
		{
			get
			{
				return this.m_pCalls.ToArray();
			}
		}

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06000677 RID: 1655 RVA: 0x00026C3C File Offset: 0x00025C3C
		// (remove) Token: 0x06000678 RID: 1656 RVA: 0x00026C74 File Offset: 0x00025C74
		
		public event EventHandler CallCreated = null;

		// Token: 0x06000679 RID: 1657 RVA: 0x00026CAC File Offset: 0x00025CAC
		protected void OnCallCreated(SIP_B2BUA_Call call)
		{
			bool flag = this.CallCreated != null;
			if (flag)
			{
				this.CallCreated(call, new EventArgs());
			}
		}

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x0600067A RID: 1658 RVA: 0x00026CDC File Offset: 0x00025CDC
		// (remove) Token: 0x0600067B RID: 1659 RVA: 0x00026D14 File Offset: 0x00025D14
		
		public event EventHandler CallTerminated = null;

		// Token: 0x0600067C RID: 1660 RVA: 0x00026D4C File Offset: 0x00025D4C
		protected internal void OnCallTerminated(SIP_B2BUA_Call call)
		{
			bool flag = this.CallTerminated != null;
			if (flag)
			{
				this.CallTerminated(call, new EventArgs());
			}
		}

		// Token: 0x040002AB RID: 683
		private SIP_Proxy m_pProxy = null;

		// Token: 0x040002AC RID: 684
		private List<SIP_B2BUA_Call> m_pCalls = null;

		// Token: 0x040002AD RID: 685
		private bool m_IsDisposed = false;
	}
}
