using System;

namespace LumiSoft.Net.FTP
{
	// Token: 0x02000232 RID: 562
	public class FTP_ListItem
	{
		// Token: 0x0600146F RID: 5231 RVA: 0x0007F7B8 File Offset: 0x0007E7B8
		public FTP_ListItem(string name, long size, DateTime modified, bool isDir)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == "";
			if (flag2)
			{
				throw new ArgumentException("Argument 'name' value must be specified.");
			}
			this.m_Name = name;
			this.m_Size = size;
			this.m_Modified = modified;
			this.m_IsDir = isDir;
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06001470 RID: 5232 RVA: 0x0007F834 File Offset: 0x0007E834
		public bool IsDir
		{
			get
			{
				return this.m_IsDir;
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06001471 RID: 5233 RVA: 0x0007F84C File Offset: 0x0007E84C
		public bool IsFile
		{
			get
			{
				return !this.m_IsDir;
			}
		}

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06001472 RID: 5234 RVA: 0x0007F868 File Offset: 0x0007E868
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06001473 RID: 5235 RVA: 0x0007F880 File Offset: 0x0007E880
		public long Size
		{
			get
			{
				return this.m_Size;
			}
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06001474 RID: 5236 RVA: 0x0007F898 File Offset: 0x0007E898
		public DateTime Modified
		{
			get
			{
				return this.m_Modified;
			}
		}

		// Token: 0x040007F3 RID: 2035
		private string m_Name = "";

		// Token: 0x040007F4 RID: 2036
		private long m_Size = 0L;

		// Token: 0x040007F5 RID: 2037
		private DateTime m_Modified;

		// Token: 0x040007F6 RID: 2038
		private bool m_IsDir = false;
	}
}
