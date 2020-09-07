using System;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A9 RID: 169
	public class SIP_RegistrationBinding : IComparable
	{
		// Token: 0x0600068F RID: 1679 RVA: 0x00027090 File Offset: 0x00026090
		internal SIP_RegistrationBinding(SIP_Registration owner, AbsoluteUri contactUri)
		{
			bool flag = owner == null;
			if (flag)
			{
				throw new ArgumentNullException("owner");
			}
			bool flag2 = contactUri == null;
			if (flag2)
			{
				throw new ArgumentNullException("contactUri");
			}
			this.m_pRegistration = owner;
			this.m_ContactURI = contactUri;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0002711C File Offset: 0x0002611C
		public void Update(SIP_Flow flow, int expires, double qvalue, string callID, int cseqNo)
		{
			bool flag = expires < 0;
			if (flag)
			{
				throw new ArgumentException("Argument 'expires' value must be >= 0.");
			}
			bool flag2 = qvalue < 0.0 || qvalue > 1.0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'qvalue' value must be >= 0.000 and <= 1.000");
			}
			bool flag3 = callID == null;
			if (flag3)
			{
				throw new ArgumentNullException("callID");
			}
			bool flag4 = cseqNo < 0;
			if (flag4)
			{
				throw new ArgumentException("Argument 'cseqNo' value must be >= 0.");
			}
			this.m_pFlow = flow;
			this.m_Expires = expires;
			this.m_QValue = qvalue;
			this.m_CallID = callID;
			this.m_CSeqNo = cseqNo;
			this.m_LastUpdate = DateTime.Now;
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x000271C3 File Offset: 0x000261C3
		public void Remove()
		{
			this.m_pRegistration.RemoveBinding(this);
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x000271D4 File Offset: 0x000261D4
		public string ToContactValue()
		{
			SIP_t_ContactParam sip_t_ContactParam = new SIP_t_ContactParam();
			sip_t_ContactParam.Parse(new StringReader(this.m_ContactURI.ToString()));
			sip_t_ContactParam.Expires = this.m_Expires;
			return sip_t_ContactParam.ToStringValue();
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x00027218 File Offset: 0x00026218
		public int CompareTo(object obj)
		{
			bool flag = obj == null;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = !(obj is SIP_RegistrationBinding);
				if (flag2)
				{
					result = -1;
				}
				else
				{
					SIP_RegistrationBinding sip_RegistrationBinding = (SIP_RegistrationBinding)obj;
					bool flag3 = sip_RegistrationBinding.QValue == this.QValue;
					if (flag3)
					{
						result = 0;
					}
					else
					{
						bool flag4 = sip_RegistrationBinding.QValue > this.QValue;
						if (flag4)
						{
							result = 1;
						}
						else
						{
							bool flag5 = sip_RegistrationBinding.QValue < this.QValue;
							if (flag5)
							{
								result = -1;
							}
							else
							{
								result = -1;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x000272A0 File Offset: 0x000262A0
		public DateTime LastUpdate
		{
			get
			{
				return this.m_LastUpdate;
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x000272B8 File Offset: 0x000262B8
		public bool IsExpired
		{
			get
			{
				return this.TTL <= 0;
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x000272D8 File Offset: 0x000262D8
		public int TTL
		{
			get
			{
				bool flag = DateTime.Now > this.m_LastUpdate.AddSeconds((double)this.m_Expires);
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					result = (int)(this.m_LastUpdate.AddSeconds((double)this.m_Expires) - DateTime.Now).TotalSeconds;
				}
				return result;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x00027338 File Offset: 0x00026338
		public SIP_Flow Flow
		{
			get
			{
				return this.m_pFlow;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x00027350 File Offset: 0x00026350
		public AbsoluteUri ContactURI
		{
			get
			{
				return this.m_ContactURI;
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x00027368 File Offset: 0x00026368
		public double QValue
		{
			get
			{
				return this.m_QValue;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x00027380 File Offset: 0x00026380
		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x00027398 File Offset: 0x00026398
		public int CSeqNo
		{
			get
			{
				return this.m_CSeqNo;
			}
		}

		// Token: 0x040002B8 RID: 696
		private SIP_Registration m_pRegistration = null;

		// Token: 0x040002B9 RID: 697
		private DateTime m_LastUpdate;

		// Token: 0x040002BA RID: 698
		private SIP_Flow m_pFlow = null;

		// Token: 0x040002BB RID: 699
		private AbsoluteUri m_ContactURI = null;

		// Token: 0x040002BC RID: 700
		private int m_Expires = 3600;

		// Token: 0x040002BD RID: 701
		private double m_QValue = 1.0;

		// Token: 0x040002BE RID: 702
		private string m_CallID = "";

		// Token: 0x040002BF RID: 703
		private int m_CSeqNo = 1;
	}
}
