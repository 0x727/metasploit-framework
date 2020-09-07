using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000202 RID: 514
	public class IMAP_r_u_MyRights : IMAP_r_u
	{
		// Token: 0x0600122F RID: 4655 RVA: 0x0006DC14 File Offset: 0x0006CC14
		public IMAP_r_u_MyRights(string folder, string rights)
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

		// Token: 0x06001230 RID: 4656 RVA: 0x0006DC80 File Offset: 0x0006CC80
		public static IMAP_r_u_MyRights Parse(string myRightsResponse)
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
			return new IMAP_r_u_MyRights(folder, rights);
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x0006DCE0 File Offset: 0x0006CCE0
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0006DCFC File Offset: 0x0006CCFC
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				"* MYRIGHTS ",
				IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding),
				" \"",
				this.m_pRights,
				"\"\r\n"
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06001233 RID: 4659 RVA: 0x0006DD5C File Offset: 0x0006CD5C
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06001234 RID: 4660 RVA: 0x0006DD74 File Offset: 0x0006CD74
		public string Rights
		{
			get
			{
				return this.m_pRights;
			}
		}

		// Token: 0x04000713 RID: 1811
		private string m_FolderName = "";

		// Token: 0x04000714 RID: 1812
		private string m_pRights = null;
	}
}
