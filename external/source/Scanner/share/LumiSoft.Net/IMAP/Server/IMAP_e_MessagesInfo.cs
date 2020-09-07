using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x0200021A RID: 538
	public class IMAP_e_MessagesInfo : EventArgs
	{
		// Token: 0x06001344 RID: 4932 RVA: 0x000770E4 File Offset: 0x000760E4
		internal IMAP_e_MessagesInfo(string folder)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			this.m_Folder = folder;
			this.m_pMessages = new List<IMAP_MessageInfo>();
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x00077130 File Offset: 0x00076130
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x06001346 RID: 4934 RVA: 0x00077148 File Offset: 0x00076148
		public List<IMAP_MessageInfo> MessagesInfo
		{
			get
			{
				return this.m_pMessages;
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x00077160 File Offset: 0x00076160
		internal int Exists
		{
			get
			{
				return this.m_pMessages.Count;
			}
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x06001348 RID: 4936 RVA: 0x00077180 File Offset: 0x00076180
		internal int Recent
		{
			get
			{
				int num = 0;
				foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pMessages)
				{
					foreach (string a in imap_MessageInfo.Flags)
					{
						bool flag = string.Equals(a, "Recent", StringComparison.InvariantCultureIgnoreCase);
						if (flag)
						{
							num++;
							break;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x00077218 File Offset: 0x00076218
		internal int FirstUnseen
		{
			get
			{
				for (int i = 0; i < this.m_pMessages.Count; i++)
				{
					bool flag = !this.m_pMessages[i].ContainsFlag("Seen");
					if (flag)
					{
						return i + 1;
					}
				}
				return -1;
			}
		}

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x0007726C File Offset: 0x0007626C
		internal int Unseen
		{
			get
			{
				int num = this.m_pMessages.Count;
				foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pMessages)
				{
					foreach (string a in imap_MessageInfo.Flags)
					{
						bool flag = string.Equals(a, "Seen", StringComparison.InvariantCultureIgnoreCase);
						if (flag)
						{
							num--;
							break;
						}
					}
				}
				return num;
			}
		}

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x0007730C File Offset: 0x0007630C
		internal long UidNext
		{
			get
			{
				long num = 0L;
				foreach (IMAP_MessageInfo imap_MessageInfo in this.m_pMessages)
				{
					bool flag = imap_MessageInfo.UID > num;
					if (flag)
					{
						num = imap_MessageInfo.UID;
					}
				}
				num += 1L;
				return num;
			}
		}

		// Token: 0x04000789 RID: 1929
		private string m_Folder = null;

		// Token: 0x0400078A RID: 1930
		private List<IMAP_MessageInfo> m_pMessages = null;
	}
}
