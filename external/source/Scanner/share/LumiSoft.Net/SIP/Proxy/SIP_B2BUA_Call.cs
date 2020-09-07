using System;
using LumiSoft.Net.SIP.Message;
using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
	// Token: 0x020000A7 RID: 167
	public class SIP_B2BUA_Call
	{
		// Token: 0x0600067D RID: 1661 RVA: 0x00026D7C File Offset: 0x00025D7C
		internal SIP_B2BUA_Call(SIP_B2BUA owner, SIP_Dialog caller, SIP_Dialog callee)
		{
			this.m_pOwner = owner;
			this.m_pCaller = caller;
			this.m_pCallee = callee;
			this.m_StartTime = DateTime.Now;
			this.m_CallID = Guid.NewGuid().ToString().Replace("-", "");
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x000091B8 File Offset: 0x000081B8
		private void m_pCaller_RequestReceived(SIP_RequestReceivedEventArgs e)
		{
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x00026E00 File Offset: 0x00025E00
		private void m_pCaller_Terminated(object sender, EventArgs e)
		{
			this.Terminate();
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00026E0C File Offset: 0x00025E0C
		private void m_pCallee_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			SIP_ServerTransaction sip_ServerTransaction = (SIP_ServerTransaction)e.ClientTransaction.Tag;
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x000091B8 File Offset: 0x000081B8
		private void m_pCallee_RequestReceived(SIP_RequestReceivedEventArgs e)
		{
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00026E00 File Offset: 0x00025E00
		private void m_pCallee_Terminated(object sender, EventArgs e)
		{
			this.Terminate();
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00026E2C File Offset: 0x00025E2C
		private void m_pCaller_ResponseReceived(object sender, SIP_ResponseReceivedEventArgs e)
		{
			SIP_ServerTransaction sip_ServerTransaction = (SIP_ServerTransaction)e.ClientTransaction.Tag;
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x00026E4C File Offset: 0x00025E4C
		public void Terminate()
		{
			bool isTerminated = this.m_IsTerminated;
			if (!isTerminated)
			{
				this.m_IsTerminated = true;
				this.m_pOwner.RemoveCall(this);
				bool flag = this.m_pCaller != null;
				if (flag)
				{
					this.m_pCaller.Dispose();
					this.m_pCaller = null;
				}
				bool flag2 = this.m_pCallee != null;
				if (flag2)
				{
					this.m_pCallee.Dispose();
					this.m_pCallee = null;
				}
				this.m_pOwner.OnCallTerminated(this);
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x00026ECC File Offset: 0x00025ECC
		private void CopyMessage(SIP_Message source, SIP_Message destination, string[] exceptHeaders)
		{
			foreach (object obj in source.Header)
			{
				SIP_HeaderField sip_HeaderField = (SIP_HeaderField)obj;
				bool flag = true;
				foreach (string text in exceptHeaders)
				{
					bool flag2 = text.ToLower() == sip_HeaderField.Name.ToLower();
					if (flag2)
					{
						flag = false;
						break;
					}
				}
				bool flag3 = flag;
				if (flag3)
				{
					destination.Header.Add(sip_HeaderField.Name, sip_HeaderField.Value);
				}
			}
			destination.Data = source.Data;
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x00026F98 File Offset: 0x00025F98
		public DateTime StartTime
		{
			get
			{
				return this.m_StartTime;
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000687 RID: 1671 RVA: 0x00026FB0 File Offset: 0x00025FB0
		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000688 RID: 1672 RVA: 0x00026FC8 File Offset: 0x00025FC8
		public SIP_Dialog CallerDialog
		{
			get
			{
				return this.m_pCaller;
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000689 RID: 1673 RVA: 0x00026FE0 File Offset: 0x00025FE0
		public SIP_Dialog CalleeDialog
		{
			get
			{
				return this.m_pCallee;
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x00026FF8 File Offset: 0x00025FF8
		public bool IsTimedOut
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040002B0 RID: 688
		private SIP_B2BUA m_pOwner = null;

		// Token: 0x040002B1 RID: 689
		private DateTime m_StartTime;

		// Token: 0x040002B2 RID: 690
		private SIP_Dialog m_pCaller = null;

		// Token: 0x040002B3 RID: 691
		private SIP_Dialog m_pCallee = null;

		// Token: 0x040002B4 RID: 692
		private string m_CallID = "";

		// Token: 0x040002B5 RID: 693
		private bool m_IsTerminated = false;
	}
}
