using System;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A4 RID: 164
	public class SIP_ProxyHandler
	{
		// Token: 0x0600065F RID: 1631 RVA: 0x00026450 File Offset: 0x00025450
		public virtual bool ProcessRequest(SIP_RequestContext requestContext)
		{
			bool flag = requestContext.Request.RequestLine.Uri.Scheme.ToLower() != "tel" && !(requestContext.Request.RequestLine.Uri is SIP_Uri);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SIP_Uri sip_Uri = (SIP_Uri)requestContext.Request.RequestLine.Uri;
				long num = 0L;
				bool flag2 = sip_Uri.User.StartsWith("+") || long.TryParse(sip_Uri.User, out num);
				if (flag2)
				{
					bool flag3 = requestContext.User == null;
					if (flag3)
					{
						requestContext.ChallengeRequest();
						result = true;
					}
					else
					{
						SIP_ProxyContext proxyContext = requestContext.ProxyContext;
						proxyContext.Start();
						result = true;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00026524 File Offset: 0x00025524
		public bool IsLocalUri()
		{
			return false;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x000091B8 File Offset: 0x000081B8
		public void GetRegistrarContacts()
		{
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x00026538 File Offset: 0x00025538
		public virtual bool IsReusable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x0002654C File Offset: 0x0002554C
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00026564 File Offset: 0x00025564
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

		// Token: 0x040002A3 RID: 675
		private object m_pTag = null;
	}
}
