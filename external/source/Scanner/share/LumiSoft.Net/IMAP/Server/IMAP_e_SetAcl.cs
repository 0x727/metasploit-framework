using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000221 RID: 545
	public class IMAP_e_SetAcl : EventArgs
	{
		// Token: 0x06001375 RID: 4981 RVA: 0x000779F0 File Offset: 0x000769F0
		internal IMAP_e_SetAcl(string folder, string identifier, IMAP_Flags_SetType flagsSetType, string rights, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = identifier == null;
			if (flag2)
			{
				throw new ArgumentNullException("identifier");
			}
			bool flag3 = rights == null;
			if (flag3)
			{
				throw new ArgumentNullException("rights");
			}
			bool flag4 = response == null;
			if (flag4)
			{
				throw new ArgumentNullException("response");
			}
			this.m_pResponse = response;
			this.m_Folder = folder;
			this.m_Identifier = identifier;
			this.m_SetType = flagsSetType;
			this.m_Rights = rights;
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x00077AA0 File Offset: 0x00076AA0
		// (set) Token: 0x06001377 RID: 4983 RVA: 0x00077AB8 File Offset: 0x00076AB8
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

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06001378 RID: 4984 RVA: 0x00077AE4 File Offset: 0x00076AE4
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x00077AFC File Offset: 0x00076AFC
		public string Identifier
		{
			get
			{
				return this.m_Identifier;
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x0600137A RID: 4986 RVA: 0x00077B14 File Offset: 0x00076B14
		public IMAP_Flags_SetType FlagsSetType
		{
			get
			{
				return this.m_SetType;
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x00077B2C File Offset: 0x00076B2C
		public string Rights
		{
			get
			{
				return this.m_Rights;
			}
		}

		// Token: 0x040007A1 RID: 1953
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x040007A2 RID: 1954
		private string m_Folder = null;

		// Token: 0x040007A3 RID: 1955
		private string m_Identifier = null;

		// Token: 0x040007A4 RID: 1956
		private IMAP_Flags_SetType m_SetType = IMAP_Flags_SetType.Replace;

		// Token: 0x040007A5 RID: 1957
		private string m_Rights = null;
	}
}
