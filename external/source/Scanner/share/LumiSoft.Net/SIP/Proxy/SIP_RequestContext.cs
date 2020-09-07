using System;
using System.Collections.Generic;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A5 RID: 165
	public class SIP_RequestContext
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x00026570 File Offset: 0x00025570
		internal SIP_RequestContext(SIP_Proxy proxy, SIP_Request request, SIP_Flow flow)
		{
			bool flag = proxy == null;
			if (flag)
			{
				throw new ArgumentNullException("proxy");
			}
			bool flag2 = request == null;
			if (flag2)
			{
				throw new ArgumentNullException("request");
			}
			bool flag3 = flow == null;
			if (flag3)
			{
				throw new ArgumentNullException("flow");
			}
			this.m_pProxy = proxy;
			this.m_pRequest = request;
			this.m_pFlow = flow;
			this.m_pTargets = new List<SIP_ProxyTarget>();
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x00017E58 File Offset: 0x00016E58
		public void ForwardStatelessly()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x00017E58 File Offset: 0x00016E58
		public void ChallengeRequest()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x00026612 File Offset: 0x00025612
		internal void SetUser(string user)
		{
			this.m_User = user;
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x0002661C File Offset: 0x0002561C
		public SIP_Request Request
		{
			get
			{
				return this.m_pRequest;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x00026634 File Offset: 0x00025634
		public SIP_ServerTransaction Transaction
		{
			get
			{
				bool flag = this.Request.RequestLine.Method == "ACK";
				if (flag)
				{
					throw new InvalidOperationException("ACK request is transactionless SIP method.");
				}
				bool flag2 = this.m_pTransaction == null;
				if (flag2)
				{
					this.m_pTransaction = this.m_pProxy.Stack.TransactionLayer.EnsureServerTransaction(this.m_pFlow, this.m_pRequest);
				}
				return this.m_pTransaction;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x000266AC File Offset: 0x000256AC
		public List<SIP_ProxyTarget> Targets
		{
			get
			{
				return this.m_pTargets;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x000266C4 File Offset: 0x000256C4
		public string User
		{
			get
			{
				return this.m_User;
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x000266DC File Offset: 0x000256DC
		public SIP_ProxyContext ProxyContext
		{
			get
			{
				bool flag = this.m_pProxyContext == null;
				if (flag)
				{
					this.m_pProxy.CreateProxyContext(this, this.Transaction, this.Request, true);
				}
				return this.m_pProxyContext;
			}
		}

		// Token: 0x040002A4 RID: 676
		private SIP_Proxy m_pProxy = null;

		// Token: 0x040002A5 RID: 677
		private SIP_Request m_pRequest = null;

		// Token: 0x040002A6 RID: 678
		private SIP_Flow m_pFlow = null;

		// Token: 0x040002A7 RID: 679
		private SIP_ServerTransaction m_pTransaction = null;

		// Token: 0x040002A8 RID: 680
		private List<SIP_ProxyTarget> m_pTargets = null;

		// Token: 0x040002A9 RID: 681
		private string m_User = null;

		// Token: 0x040002AA RID: 682
		private SIP_ProxyContext m_pProxyContext = null;
	}
}
