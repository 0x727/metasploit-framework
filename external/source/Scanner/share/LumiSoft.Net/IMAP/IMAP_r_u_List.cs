using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001FF RID: 511
	public class IMAP_r_u_List : IMAP_r_u
	{
		// Token: 0x06001218 RID: 4632 RVA: 0x0006D50C File Offset: 0x0006C50C
		public IMAP_r_u_List(string folder, char delimiter, string[] attributes)
		{
			bool flag = folder == null;
			if (flag)
			{
				throw new ArgumentNullException("folder");
			}
			this.m_FolderName = folder;
			this.m_Delimiter = delimiter;
			bool flag2 = attributes != null;
			if (flag2)
			{
				this.m_pFolderAttributes = attributes;
			}
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0006D573 File Offset: 0x0006C573
		internal IMAP_r_u_List(char delimiter)
		{
			this.m_Delimiter = delimiter;
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0006D5A4 File Offset: 0x0006C5A4
		public static IMAP_r_u_List Parse(string listResponse)
		{
			bool flag = listResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("listResponse");
			}
			StringReader stringReader = new StringReader(listResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string text = stringReader.ReadParenthesized();
			string text2 = stringReader.ReadWord();
			string folder = IMAP_Utils.DecodeMailbox(stringReader.ReadToEnd().Trim());
			return new IMAP_r_u_List(folder, text2[0], (text == string.Empty) ? new string[0] : text.Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0006D638 File Offset: 0x0006C638
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0006D654 File Offset: 0x0006C654
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			bool flag = string.IsNullOrEmpty(this.m_FolderName);
			string result;
			if (flag)
			{
				result = "* LIST (\\Noselect) \"/\" \"\"\r\n";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("* LIST (");
				bool flag2 = this.m_pFolderAttributes != null;
				if (flag2)
				{
					for (int i = 0; i < this.m_pFolderAttributes.Length; i++)
					{
						bool flag3 = i > 0;
						if (flag3)
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
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x0600121D RID: 4637 RVA: 0x0006D740 File Offset: 0x0006C740
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x0006D758 File Offset: 0x0006C758
		public char HierarchyDelimiter
		{
			get
			{
				return this.m_Delimiter;
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x0600121F RID: 4639 RVA: 0x0006D770 File Offset: 0x0006C770
		public string[] FolderAttributes
		{
			get
			{
				return this.m_pFolderAttributes;
			}
		}

		// Token: 0x04000709 RID: 1801
		private string m_FolderName = "";

		// Token: 0x0400070A RID: 1802
		private char m_Delimiter = '/';

		// Token: 0x0400070B RID: 1803
		private string[] m_pFolderAttributes = new string[0];
	}
}
