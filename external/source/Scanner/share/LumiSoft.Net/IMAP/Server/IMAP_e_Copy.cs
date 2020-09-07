using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000210 RID: 528
	public class IMAP_e_Copy : EventArgs
	{
		// Token: 0x0600130B RID: 4875 RVA: 0x0007673C File Offset: 0x0007573C
		internal IMAP_e_Copy(string sourceFolder, string targetFolder, IMAP_MessageInfo[] messagesInfo, IMAP_r_ServerStatus response)
		{
			bool flag = sourceFolder == null;
			if (flag)
			{
				throw new ArgumentNullException("sourceFolder");
			}
			bool flag2 = targetFolder == null;
			if (flag2)
			{
				throw new ArgumentNullException("targetFolder");
			}
			bool flag3 = messagesInfo == null;
			if (flag3)
			{
				throw new ArgumentNullException("messagesInfo");
			}
			bool flag4 = response == null;
			if (flag4)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
			this.m_SourceFolder = sourceFolder;
			this.m_TargetFolder = targetFolder;
			this.m_pMessagesInfo = messagesInfo;
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x0600130C RID: 4876 RVA: 0x000767DC File Offset: 0x000757DC
		// (set) Token: 0x0600130D RID: 4877 RVA: 0x000767F4 File Offset: 0x000757F4
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

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x0600130E RID: 4878 RVA: 0x00076820 File Offset: 0x00075820
		public string SourceFolder
		{
			get
			{
				return this.m_SourceFolder;
			}
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x0600130F RID: 4879 RVA: 0x00076838 File Offset: 0x00075838
		public string TargetFolder
		{
			get
			{
				return this.m_TargetFolder;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06001310 RID: 4880 RVA: 0x00076850 File Offset: 0x00075850
		public IMAP_MessageInfo[] MessagesInfo
		{
			get
			{
				return this.m_pMessagesInfo;
			}
		}

		// Token: 0x04000767 RID: 1895
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000768 RID: 1896
		private string m_SourceFolder = null;

		// Token: 0x04000769 RID: 1897
		private string m_TargetFolder = null;

		// Token: 0x0400076A RID: 1898
		private IMAP_MessageInfo[] m_pMessagesInfo = null;
	}
}
