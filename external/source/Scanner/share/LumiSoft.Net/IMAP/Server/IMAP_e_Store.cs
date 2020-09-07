using System;

namespace LumiSoft.Net.IMAP.Server
{
	// Token: 0x02000224 RID: 548
	public class IMAP_e_Store : EventArgs
	{
		// Token: 0x0600138B RID: 5003 RVA: 0x00077DBC File Offset: 0x00076DBC
		internal IMAP_e_Store(string folder, IMAP_MessageInfo msgInfo, IMAP_Flags_SetType flagsSetType, string[] flags, IMAP_r_ServerStatus response)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = msgInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("msgInfo");
			}
			bool flag3 = flags == null;
			if (flag3)
			{
				throw new ArgumentNullException("flags");
			}
			this.m_pResponse = response;
			this.m_Folder = folder;
			this.m_pMsgInfo = msgInfo;
			this.m_SetType = flagsSetType;
			this.m_pFlags = flags;
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x0600138C RID: 5004 RVA: 0x00077E58 File Offset: 0x00076E58
		// (set) Token: 0x0600138D RID: 5005 RVA: 0x00077E70 File Offset: 0x00076E70
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

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x0600138E RID: 5006 RVA: 0x00077E9C File Offset: 0x00076E9C
		public string Folder
		{
			get
			{
				return this.m_Folder;
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x0600138F RID: 5007 RVA: 0x00077EB4 File Offset: 0x00076EB4
		public IMAP_MessageInfo MessageInfo
		{
			get
			{
				return this.m_pMsgInfo;
			}
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06001390 RID: 5008 RVA: 0x00077ECC File Offset: 0x00076ECC
		public IMAP_Flags_SetType FlagsSetType
		{
			get
			{
				return this.m_SetType;
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06001391 RID: 5009 RVA: 0x00077EE4 File Offset: 0x00076EE4
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x040007AE RID: 1966
		private IMAP_r_ServerStatus m_pResponse = null;

		// Token: 0x040007AF RID: 1967
		private string m_Folder = null;

		// Token: 0x040007B0 RID: 1968
		private IMAP_MessageInfo m_pMsgInfo = null;

		// Token: 0x040007B1 RID: 1969
		private IMAP_Flags_SetType m_SetType = IMAP_Flags_SetType.Replace;

		// Token: 0x040007B2 RID: 1970
		private string[] m_pFlags = null;
	}
}
