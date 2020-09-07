using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021E RID: 542
	public class IMAP_e_Rename : EventArgs
	{
		// Token: 0x0600135C RID: 4956 RVA: 0x000775D8 File Offset: 0x000765D8
		internal IMAP_e_Rename(string cmdTag, string currentFolder, string newFolder)
		{
			bool flag = cmdTag == null;
			if (flag)
			{
				throw new ArgumentNullException("cmdTag");
			}
			bool flag2 = currentFolder == null;
			if (flag2)
			{
				throw new ArgumentNullException("currentFolder");
			}
			bool flag3 = newFolder == null;
			if (flag3)
			{
				throw new ArgumentNullException("newFolder");
			}
			this.m_CmdTag = cmdTag;
			this.m_CurrentFolder = currentFolder;
			this.m_NewFolder = newFolder;
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x0600135D RID: 4957 RVA: 0x0007765C File Offset: 0x0007665C
		// (set) Token: 0x0600135E RID: 4958 RVA: 0x00077674 File Offset: 0x00076674
		public IMAP_r_ServerStatus Response
		{
			get
			{
				return this.m_pResponse;
			}
			set
			{
				this.m_pResponse = value;
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x0600135F RID: 4959 RVA: 0x00077680 File Offset: 0x00076680
		public string CmdTag
		{
			get
			{
				return this.m_CmdTag;
			}
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06001360 RID: 4960 RVA: 0x00077698 File Offset: 0x00076698
		public string CurrentFolder
		{
			get
			{
				return this.m_CurrentFolder;
			}
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06001361 RID: 4961 RVA: 0x000776B0 File Offset: 0x000766B0
		public string NewFolder
		{
			get
			{
				return this.m_NewFolder;
			}
		}

		// Token: 0x04000793 RID: 1939
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000794 RID: 1940
		private string m_CmdTag = null;

		// Token: 0x04000795 RID: 1941
		private string m_CurrentFolder = null;

		// Token: 0x04000796 RID: 1942
		private string m_NewFolder = null;
	}
}
