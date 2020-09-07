using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000206 RID: 518
	public class IMAP_r_u_QuotaRoot : IMAP_r_u
	{
		// Token: 0x0600124D RID: 4685 RVA: 0x0006E8D4 File Offset: 0x0006D8D4
		public IMAP_r_u_QuotaRoot(string folder, string[] quotaRoots)
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
			bool flag3 = quotaRoots == null;
			if (flag3)
			{
				throw new ArgumentNullException("quotaRoots");
			}
			this.m_FolderName = folder;
			this.m_QuotaRoots = quotaRoots;
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x0006E954 File Offset: 0x0006D954
		public static IMAP_r_u_QuotaRoot Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string folder = TextUtils.UnQuoteString(IMAP_Utils.Decode_IMAP_UTF7_String(stringReader.ReadWord()));
			List<string> list = new List<string>();
			while (stringReader.Available > 0L)
			{
				string text = stringReader.ReadWord();
				bool flag2 = text != null;
				if (!flag2)
				{
					break;
				}
				list.Add(text);
			}
			return new IMAP_r_u_QuotaRoot(folder, list.ToArray());
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x0006E9EC File Offset: 0x0006D9EC
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x0006EA08 File Offset: 0x0006DA08
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* QUOTAROOT " + IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding));
			foreach (string str in this.m_QuotaRoots)
			{
				stringBuilder.Append(" \"" + str + "\"");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x1700061A RID: 1562
		// (get) Token: 0x06001251 RID: 4689 RVA: 0x0006EA84 File Offset: 0x0006DA84
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x1700061B RID: 1563
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x0006EA9C File Offset: 0x0006DA9C
		public string[] QuotaRoots
		{
			get
			{
				return this.m_QuotaRoots;
			}
		}

		// Token: 0x0400071E RID: 1822
		private string m_FolderName = "";

		// Token: 0x0400071F RID: 1823
		private string[] m_QuotaRoots = null;
	}
}
