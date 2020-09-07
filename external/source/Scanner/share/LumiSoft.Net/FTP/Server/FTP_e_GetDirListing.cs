using System;
using System.Collections.Generic;

namespace LumiSoft.Net.FTP.Server
{
	// Token: 0x0200023C RID: 572
	public class FTP_e_GetDirListing : EventArgs
	{
		// Token: 0x060014B1 RID: 5297 RVA: 0x000817B4 File Offset: 0x000807B4
		public FTP_e_GetDirListing(string path)
		{
			bool flag = path == null;
			if (flag)
			{
				throw new ArgumentNullException("path");
			}
			this.m_Path = path;
			this.m_pItems = new List<FTP_ListItem>();
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x00081804 File Offset: 0x00080804
		// (set) Token: 0x060014B3 RID: 5299 RVA: 0x0008181C File Offset: 0x0008081C
		public FTP_t_ReplyLine[] Error
		{
			get
			{
				return this.m_pReplyLines;
			}
			set
			{
				this.m_pReplyLines = value;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x00081828 File Offset: 0x00080828
		public string Path
		{
			get
			{
				return this.m_Path;
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x060014B5 RID: 5301 RVA: 0x00081840 File Offset: 0x00080840
		public List<FTP_ListItem> Items
		{
			get
			{
				return this.m_pItems;
			}
		}

		// Token: 0x04000811 RID: 2065
		private string m_Path = null;

		// Token: 0x04000812 RID: 2066
		private List<FTP_ListItem> m_pItems = null;

		// Token: 0x04000813 RID: 2067
		private FTP_t_ReplyLine[] m_pReplyLines = null;
	}
}
