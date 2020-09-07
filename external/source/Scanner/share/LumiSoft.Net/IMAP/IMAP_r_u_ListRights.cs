using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000200 RID: 512
	public class IMAP_r_u_ListRights : IMAP_r_u
	{
		// Token: 0x06001220 RID: 4640 RVA: 0x0006D788 File Offset: 0x0006C788
		public IMAP_r_u_ListRights(string folder, string identifier, string requiredRights, string optionalRights)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			bool flag2 = folder == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'folder' name must be specified.", "folder");
			}
			bool flag3 = identifier == null;
			if (flag3)
			{
				throw new ArgumentNullException("identifier");
			}
			bool flag4 = identifier == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'identifier' name must be specified.", "identifier");
			}
			this.m_FolderName = folder;
			this.m_Identifier = identifier;
			this.m_RequiredRights = ((requiredRights == string.Empty) ? null : requiredRights);
			this.m_OptionalRights = ((optionalRights == string.Empty) ? null : optionalRights);
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0006D868 File Offset: 0x0006C868
		public static IMAP_r_u_ListRights Parse(string listRightsResponse)
		{
			bool flag = listRightsResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("listRightsResponse");
			}
			StringReader stringReader = new StringReader(listRightsResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string folder = IMAP_Utils.Decode_IMAP_UTF7_String(stringReader.ReadWord(true));
			string identifier = stringReader.ReadWord(true);
			string requiredRights = stringReader.ReadWord(true);
			string optionalRights = stringReader.ReadWord(true);
			return new IMAP_r_u_ListRights(folder, identifier, requiredRights, optionalRights);
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0006D8D8 File Offset: 0x0006C8D8
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0006D8F4 File Offset: 0x0006C8F4
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				"* LISTRIGHTS ",
				IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding),
				" \"",
				this.m_RequiredRights,
				"\" ",
				this.m_OptionalRights,
				"\r\n"
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x0006D968 File Offset: 0x0006C968
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x0006D980 File Offset: 0x0006C980
		public string Identifier
		{
			get
			{
				return this.m_Identifier;
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001226 RID: 4646 RVA: 0x0006D998 File Offset: 0x0006C998
		public string RequiredRights
		{
			get
			{
				return this.m_RequiredRights;
			}
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x0006D9B0 File Offset: 0x0006C9B0
		public string OptionalRights
		{
			get
			{
				return this.m_OptionalRights;
			}
		}

		// Token: 0x0400070C RID: 1804
		private string m_FolderName = "";

		// Token: 0x0400070D RID: 1805
		private string m_Identifier = "";

		// Token: 0x0400070E RID: 1806
		private string m_RequiredRights = null;

		// Token: 0x0400070F RID: 1807
		private string m_OptionalRights = null;
	}
}
