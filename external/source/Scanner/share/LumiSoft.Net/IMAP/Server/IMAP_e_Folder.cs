using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000213 RID: 531
	public class IMAP_e_Folder : EventArgs
	{
		// Token: 0x06001320 RID: 4896 RVA: 0x00076B2C File Offset: 0x00075B2C
		internal IMAP_e_Folder(string cmdTag, string folder, IMAP_r_ServerStatus response)
		{
			bool flag = cmdTag == null;
			if (flag)
			{
				throw new ArgumentNullException("cmdTag");
			}
			bool flag2 = cmdTag == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'cmdTag' value must be specified.", "cmdTag");
			}
			bool flag3 = folder == null;
			if (flag3)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag4 = response == null;
			if (flag4)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
			this.m_CmdTag = cmdTag;
			this.m_Folder = folder;
		}

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06001321 RID: 4897 RVA: 0x00076BCC File Offset: 0x00075BCC
		// (set) Token: 0x06001322 RID: 4898 RVA: 0x00076BE4 File Offset: 0x00075BE4
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

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06001323 RID: 4899 RVA: 0x00076BF0 File Offset: 0x00075BF0
		public string CmdTag
		{
			get
			{
				return this.m_CmdTag;
			}
		}

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06001324 RID: 4900 RVA: 0x00076C08 File Offset: 0x00075C08
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x04000772 RID: 1906
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x04000773 RID: 1907
		private string m_CmdTag = null;

		// Token: 0x04000774 RID: 1908
		private string m_Folder = "";
	}
}
