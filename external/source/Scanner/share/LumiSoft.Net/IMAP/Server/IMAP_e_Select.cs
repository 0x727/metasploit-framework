using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000220 RID: 544
	public class IMAP_e_Select : EventArgs
	{
		// Token: 0x0600136A RID: 4970 RVA: 0x0007781C File Offset: 0x0007681C
		internal IMAP_e_Select(string cmdTag, string folder)
		{
			bool flag = cmdTag == null;
			if (flag)
			{
				throw new ArgumentNullException("cmdTag");
			}
			bool flag2 = folder == null;
			if (flag2)
			{
				throw new ArgumentNullException("folder");
			}
			this.m_CmdTag = cmdTag;
			this.m_Folder = folder;
			this.m_pFlags = new List<string>();
			this.m_pPermanentFlags = new List<string>();
			this.m_pFlags.AddRange(new string[]
			{
				"\\Answered",
				"\\Flagged",
				"\\Deleted",
				"\\Seen",
				"\\Draft"
			});
			this.m_pPermanentFlags.AddRange(new string[]
			{
				"\\Answered",
				"\\Flagged",
				"\\Deleted",
				"\\Seen",
				"\\Draft"
			});
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x0600136B RID: 4971 RVA: 0x00077924 File Offset: 0x00076924
		public string CmdTag
		{
			get
			{
				return this.m_CmdTag;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x0007793C File Offset: 0x0007693C
		// (set) Token: 0x0600136D RID: 4973 RVA: 0x00077954 File Offset: 0x00076954
		public IMAP_r_ServerStatus ErrorResponse
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

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x00077960 File Offset: 0x00076960
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x00077978 File Offset: 0x00076978
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x00077990 File Offset: 0x00076990
		public bool IsReadOnly
		{
			get
			{
				return this.m_IsReadOnly;
			}
			set
			{
				this.m_IsReadOnly = value;
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x0007799C File Offset: 0x0007699C
		// (set) Token: 0x06001372 RID: 4978 RVA: 0x000779B4 File Offset: 0x000769B4
		public int FolderUID
		{
			get
			{
				return this.m_FolderUID;
			}
			set
			{
				this.m_FolderUID = value;
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x000779C0 File Offset: 0x000769C0
		public List<string> Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001374 RID: 4980 RVA: 0x000779D8 File Offset: 0x000769D8
		public List<string> PermanentFlags
		{
			get
			{
				return this.m_pPermanentFlags;
			}
		}

		// Token: 0x0400079A RID: 1946
		private string m_CmdTag = null;

		// Token: 0x0400079B RID: 1947
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x0400079C RID: 1948
		private string m_Folder = null;

		// Token: 0x0400079D RID: 1949
		private bool m_IsReadOnly = false;

		// Token: 0x0400079E RID: 1950
		private int m_FolderUID = 0;

		// Token: 0x0400079F RID: 1951
		private List<string> m_pFlags = null;

		// Token: 0x040007A0 RID: 1952
		private List<string> m_pPermanentFlags = null;
	}
}
