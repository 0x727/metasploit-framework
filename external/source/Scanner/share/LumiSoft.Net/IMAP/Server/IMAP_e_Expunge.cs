using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000211 RID: 529
	public class IMAP_e_Expunge : EventArgs
	{
		// Token: 0x06001311 RID: 4881 RVA: 0x00076868 File Offset: 0x00075868
		internal IMAP_e_Expunge(string folder, IMAP_MessageInfo msgInfo, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = msgInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("msgInfo");
			}
			bool flag3 = response == null;
			if (flag3)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
			this.m_Folder = folder;
			this.m_pMsgInfo = msgInfo;
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x000768E4 File Offset: 0x000758E4
		// (set) Token: 0x06001313 RID: 4883 RVA: 0x000768FC File Offset: 0x000758FC
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pResponse = value;
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x00076928 File Offset: 0x00075928
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06001315 RID: 4885 RVA: 0x00076940 File Offset: 0x00075940
		public IMAP_MessageInfo MessageInfo
		{
			get
			{
				return this.m_pMsgInfo;
			}
		}

		// Token: 0x0400076B RID: 1899
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x0400076C RID: 1900
		private string m_Folder = null;

		// Token: 0x0400076D RID: 1901
		private IMAP_MessageInfo m_pMsgInfo = null;
	}
}
