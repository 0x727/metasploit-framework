using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000201 RID: 513
	public class IMAP_r_u_LSub : IMAP_r_u
	{
		// Token: 0x06001228 RID: 4648 RVA: 0x0006D9C8 File Offset: 0x0006C9C8
		public IMAP_r_u_LSub(string folder, char delimiter, string[] attributes)
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
			this.m_Delimiter = delimiter;
			bool flag3 = attributes != null;
			if (flag3)
			{
				this.m_pFolderAttributes = attributes;
			}
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x0006DA50 File Offset: 0x0006CA50
		public static IMAP_r_u_LSub Parse(string lSubResponse)
		{
			bool flag = lSubResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("lSubResponse");
			}
			StringReader stringReader = new StringReader(lSubResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string text = stringReader.ReadParenthesized();
			string text2 = stringReader.ReadWord();
			string folder = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(stringReader.ReadToEnd().Trim()));
			return new IMAP_r_u_LSub(folder, text2[0], (text == string.Empty) ? new string[0] : text.Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0006DAE8 File Offset: 0x0006CAE8
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x0006DB04 File Offset: 0x0006CB04
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* LSUB (");
			bool flag = this.m_pFolderAttributes != null;
			if (flag)
			{
				for (int i = 0; i < this.m_pFolderAttributes.Length; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(this.m_pFolderAttributes[i]);
				}
			}
			stringBuilder.Append(") ");
			stringBuilder.Append("\"" + this.m_Delimiter.ToString() + "\" ");
			stringBuilder.Append(IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding));
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x0006DBCC File Offset: 0x0006CBCC
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x0006DBE4 File Offset: 0x0006CBE4
		public char HierarchyDelimiter
		{
			get
			{
				return this.m_Delimiter;
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x0006DBFC File Offset: 0x0006CBFC
		public string[] FolderAttributes
		{
			get
			{
				return this.m_pFolderAttributes;
			}
		}

		// Token: 0x04000710 RID: 1808
		private string m_FolderName = "";

		// Token: 0x04000711 RID: 1809
		private char m_Delimiter = '/';

		// Token: 0x04000712 RID: 1810
		private string[] m_pFolderAttributes = new string[0];
	}
}
