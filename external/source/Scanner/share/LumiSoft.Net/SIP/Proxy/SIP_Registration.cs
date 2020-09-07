using System;
using System.Collections.Generic;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000AA RID: 170
	public class SIP_Registration
	{
		// Token: 0x0600069C RID: 1692 RVA: 0x000273B0 File Offset: 0x000263B0
		public SIP_Registration(string userName, string aor)
		{
			bool flag = userName == null;
			if (flag)
			{
				throw new ArgumentNullException("userName");
			}
			bool flag2 = aor == null;
			if (flag2)
			{
				throw new ArgumentNullException("aor");
			}
			bool flag3 = aor == "";
			if (flag3)
			{
				throw new ArgumentException("Argument 'aor' value must be specified.");
			}
			this.m_UserName = userName;
			this.m_AOR = aor;
			this.m_CreateTime = DateTime.Now;
			this.m_pBindings = new List<SIP_RegistrationBinding>();
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x00027454 File Offset: 0x00026454
		public SIP_RegistrationBinding GetBinding(AbsoluteUri contactUri)
		{
			bool flag = contactUri == null;
			if (flag)
			{
				throw new ArgumentNullException("contactUri");
			}
			object pLock = this.m_pLock;
			SIP_RegistrationBinding result;
			lock (pLock)
			{
				foreach (SIP_RegistrationBinding sip_RegistrationBinding in this.m_pBindings)
				{
					bool flag3 = contactUri.Equals(sip_RegistrationBinding.ContactURI);
					if (flag3)
					{
						return sip_RegistrationBinding;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00027508 File Offset: 0x00026508
		public void AddOrUpdateBindings(SIP_Flow flow, string callID, int cseqNo, SIP_t_ContactParam[] contacts)
		{
			bool flag = callID == null;
			if (flag)
			{
				throw new ArgumentNullException("callID");
			}
			bool flag2 = cseqNo < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'cseqNo' value must be >= 0.");
			}
			bool flag3 = contacts == null;
			if (flag3)
			{
				throw new ArgumentNullException("contacts");
			}
			object pLock = this.m_pLock;
			lock (pLock)
			{
				foreach (SIP_t_ContactParam sip_t_ContactParam in contacts)
				{
					SIP_RegistrationBinding sip_RegistrationBinding = this.GetBinding(sip_t_ContactParam.Address.Uri);
					bool flag5 = sip_RegistrationBinding == null;
					if (flag5)
					{
						sip_RegistrationBinding = new SIP_RegistrationBinding(this, sip_t_ContactParam.Address.Uri);
						this.m_pBindings.Add(sip_RegistrationBinding);
					}
					sip_RegistrationBinding.Update(flow, (sip_t_ContactParam.Expires == -1) ? 3600 : sip_t_ContactParam.Expires, (sip_t_ContactParam.QValue == -1.0) ? 1.0 : sip_t_ContactParam.QValue, callID, cseqNo);
				}
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00027638 File Offset: 0x00026638
		public void RemoveBinding(SIP_RegistrationBinding binding)
		{
			bool flag = binding == null;
			if (flag)
			{
				throw new ArgumentNullException("binding");
			}
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.m_pBindings.Remove(binding);
			}
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00027698 File Offset: 0x00026698
		public void RemoveAllBindings()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				this.m_pBindings.Clear();
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x000276E4 File Offset: 0x000266E4
		public void RemoveExpiredBindings()
		{
			object pLock = this.m_pLock;
			lock (pLock)
			{
				for (int i = 0; i < this.m_pBindings.Count; i++)
				{
					bool isExpired = this.m_pBindings[i].IsExpired;
					if (isExpired)
					{
						this.m_pBindings.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0002776C File Offset: 0x0002676C
		public DateTime CreateTime
		{
			get
			{
				return this.m_CreateTime;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060006A3 RID: 1699 RVA: 0x00027784 File Offset: 0x00026784
		public string UserName
		{
			get
			{
				return this.m_UserName;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060006A4 RID: 1700 RVA: 0x0002779C File Offset: 0x0002679C
		public string AOR
		{
			get
			{
				return this.m_AOR;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060006A5 RID: 1701 RVA: 0x000277B4 File Offset: 0x000267B4
		public SIP_RegistrationBinding[] Bindings
		{
			get
			{
				SIP_RegistrationBinding[] array = this.m_pBindings.ToArray();
				Array.Sort<SIP_RegistrationBinding>(array);
				return array;
			}
		}

		// Token: 0x040002C0 RID: 704
		private DateTime m_CreateTime;

		// Token: 0x040002C1 RID: 705
		private string m_UserName = "";

		// Token: 0x040002C2 RID: 706
		private string m_AOR = "";

		// Token: 0x040002C3 RID: 707
		private List<SIP_RegistrationBinding> m_pBindings = null;

		// Token: 0x040002C4 RID: 708
		private object m_pLock = new object();
	}
}
