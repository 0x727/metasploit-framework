using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001FC RID: 508
	public class IMAP_r_u_Acl : IMAP_r_u
	{
		// Token: 0x0600120A RID: 4618 RVA: 0x0006D11C File Offset: 0x0006C11C
		public IMAP_r_u_Acl(string folderName, IMAP_Acl_Entry[] entries)
		{
			bool flag = folderName == null;
			if (flag)
			{
				throw new ArgumentNullException("folderName");
			}
			bool flag2 = folderName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'folderName' value must be specified.", "folderName");
			}
			bool flag3 = entries == null;
			if (flag3)
			{
				throw new ArgumentNullException("entries");
			}
			this.m_FolderName = folderName;
			this.m_pEntries = entries;
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x0006D19C File Offset: 0x0006C19C
		public static IMAP_r_u_Acl Parse(string aclResponse)
		{
			bool flag = aclResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("aclResponse");
			}
			StringReader stringReader = new StringReader(aclResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string folderName = TextUtils.UnQuoteString(IMAP_Utils.Decode_IMAP_UTF7_String(stringReader.ReadWord()));
			string[] array = stringReader.ReadToEnd().Split(new char[]
			{
				' '
			});
			List<IMAP_Acl_Entry> list = new List<IMAP_Acl_Entry>();
			for (int i = 0; i < array.Length; i += 2)
			{
				list.Add(new IMAP_Acl_Entry(array[i], array[i + 1]));
			}
			return new IMAP_r_u_Acl(folderName, list.ToArray());
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x0006D248 File Offset: 0x0006C248
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0006D264 File Offset: 0x0006C264
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* ACL ");
			stringBuilder.Append(IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding));
			foreach (IMAP_Acl_Entry imap_Acl_Entry in this.m_pEntries)
			{
				stringBuilder.Append(string.Concat(new string[]
				{
					" \"",
					imap_Acl_Entry.Identifier,
					"\" \"",
					imap_Acl_Entry.Rights,
					"\""
				}));
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x0600120E RID: 4622 RVA: 0x0006D308 File Offset: 0x0006C308
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x0600120F RID: 4623 RVA: 0x0006D320 File Offset: 0x0006C320
		public IMAP_Acl_Entry[] Entires
		{
			get
			{
				return this.m_pEntries;
			}
		}

		// Token: 0x04000705 RID: 1797
		private string m_FolderName = "";

		// Token: 0x04000706 RID: 1798
		private IMAP_Acl_Entry[] m_pEntries = null;
	}
}
