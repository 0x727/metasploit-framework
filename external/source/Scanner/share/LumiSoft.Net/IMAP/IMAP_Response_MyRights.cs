using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000208 RID: 520
	[Obsolete("Use class IMAP_r_u_MyRights instead.")]
	public class IMAP_Response_MyRights : IMAP_r_u
	{
		// Token: 0x0600125D RID: 4701 RVA: 0x0006EF24 File Offset: 0x0006DF24
		public IMAP_Response_MyRights(string folder, string rights)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = folder == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'folder' value must be specified.", "folder");
			}
			this.m_FolderName = folder;
			this.m_pRights = rights;
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x0006EF90 File Offset: 0x0006DF90
		public static IMAP_Response_MyRights Parse(string myRightsResponse)
		{
			bool flag = myRightsResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("myRightsResponse");
			}
			StringReader stringReader = new StringReader(myRightsResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string folder = IMAP_Utils.Decode_IMAP_UTF7_String(stringReader.ReadWord(true));
			string rights = stringReader.ReadToEnd().Trim();
			return new IMAP_Response_MyRights(folder, rights);
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x0006EFF0 File Offset: 0x0006DFF0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				"* MYRIGHTS \"",
				this.m_FolderName,
				"\" \"",
				this.m_pRights,
				"\"\r\n"
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001260 RID: 4704 RVA: 0x0006F04C File Offset: 0x0006E04C
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001261 RID: 4705 RVA: 0x0006F064 File Offset: 0x0006E064
		public string Rights
		{
			get
			{
				return this.m_pRights;
			}
		}

		// Token: 0x04000726 RID: 1830
		private string m_FolderName = "";

		// Token: 0x04000727 RID: 1831
		private string m_pRights = null;
	}
}
