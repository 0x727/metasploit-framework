using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000216 RID: 534
	public class IMAP_e_List : EventArgs
	{
		// Token: 0x06001330 RID: 4912 RVA: 0x00076E0C File Offset: 0x00075E0C
		internal IMAP_e_List(string referenceName, string folderFilter)
		{
			this.m_FolderReferenceName = referenceName;
			this.m_FolderFilter = folderFilter;
			this.m_pFolders = new List<IMAP_r_u_List>();
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x06001331 RID: 4913 RVA: 0x00076E44 File Offset: 0x00075E44
		public string FolderReferenceName
		{
			get
			{
				return this.m_FolderReferenceName;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x06001332 RID: 4914 RVA: 0x00076E5C File Offset: 0x00075E5C
		public string FolderFilter
		{
			get
			{
				return this.m_FolderFilter;
			}
		}

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06001333 RID: 4915 RVA: 0x00076E74 File Offset: 0x00075E74
		public List<IMAP_r_u_List> Folders
		{
			get
			{
				return this.m_pFolders;
			}
		}

		// Token: 0x0400077C RID: 1916
		private string m_FolderReferenceName = null;

		// Token: 0x0400077D RID: 1917
		private string m_FolderFilter = null;

		// Token: 0x0400077E RID: 1918
		private List<IMAP_r_u_List> m_pFolders = null;
	}
}
