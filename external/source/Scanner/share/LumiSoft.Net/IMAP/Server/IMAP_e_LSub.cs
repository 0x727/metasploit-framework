using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000219 RID: 537
	public class IMAP_e_LSub : EventArgs
	{
		// Token: 0x06001340 RID: 4928 RVA: 0x00077064 File Offset: 0x00076064
		internal IMAP_e_LSub(string referenceName, string folderFilter)
		{
			this.m_FolderReferenceName = referenceName;
			this.m_FolderFilter = folderFilter;
			this.m_pFolders = new List<IMAP_r_u_LSub>();
		}

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06001341 RID: 4929 RVA: 0x0007709C File Offset: 0x0007609C
		public string FolderReferenceName
		{
			get
			{
				return this.m_FolderReferenceName;
			}
		}

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06001342 RID: 4930 RVA: 0x000770B4 File Offset: 0x000760B4
		public string FolderFilter
		{
			get
			{
				return this.m_FolderFilter;
			}
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x06001343 RID: 4931 RVA: 0x000770CC File Offset: 0x000760CC
		public List<IMAP_r_u_LSub> Folders
		{
			get
			{
				return this.m_pFolders;
			}
		}

		// Token: 0x04000786 RID: 1926
		private string m_FolderReferenceName = null;

		// Token: 0x04000787 RID: 1927
		private string m_FolderFilter = null;

		// Token: 0x04000788 RID: 1928
		private List<IMAP_r_u_LSub> m_pFolders = null;
	}
}
