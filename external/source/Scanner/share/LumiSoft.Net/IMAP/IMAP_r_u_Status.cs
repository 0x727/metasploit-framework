using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000207 RID: 519
	public class IMAP_r_u_Status : IMAP_r_u
	{
		// Token: 0x06001253 RID: 4691 RVA: 0x0006EAB4 File Offset: 0x0006DAB4
		public IMAP_r_u_Status(string folder, int messagesCount, int recentCount, long uidNext, long folderUid, int unseenCount)
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
			this.m_MessageCount = messagesCount;
			this.m_RecentCount = recentCount;
			this.m_UidNext = uidNext;
			this.m_FolderUid = folderUid;
			this.m_UnseenCount = unseenCount;
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x0006EB5C File Offset: 0x0006DB5C
		public static IMAP_r_u_Status Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response);
			stringReader.ReadWord();
			stringReader.ReadWord();
			int messagesCount = 0;
			int recentCount = 0;
			long uidNext = 0L;
			long folderUid = 0L;
			int unseenCount = 0;
			string folder = TextUtils.UnQuoteString(IMAP_Utils.Decode_IMAP_UTF7_String(stringReader.ReadWord()));
			string[] array = stringReader.ReadParenthesized().Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i += 2)
			{
				bool flag2 = array[i].Equals("MESSAGES", StringComparison.InvariantCultureIgnoreCase);
				if (flag2)
				{
					messagesCount = Convert.ToInt32(array[i + 1]);
				}
				else
				{
					bool flag3 = array[i].Equals("RECENT", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						recentCount = Convert.ToInt32(array[i + 1]);
					}
					else
					{
						bool flag4 = array[i].Equals("UIDNEXT", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							uidNext = Convert.ToInt64(array[i + 1]);
						}
						else
						{
							bool flag5 = array[i].Equals("UIDVALIDITY", StringComparison.InvariantCultureIgnoreCase);
							if (flag5)
							{
								folderUid = Convert.ToInt64(array[i + 1]);
							}
							else
							{
								bool flag6 = array[i].Equals("UNSEEN", StringComparison.InvariantCultureIgnoreCase);
								if (flag6)
								{
									unseenCount = Convert.ToInt32(array[i + 1]);
								}
							}
						}
					}
				}
			}
			return new IMAP_r_u_Status(folder, messagesCount, recentCount, uidNext, folderUid, unseenCount);
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0006ECC8 File Offset: 0x0006DCC8
		public override string ToString()
		{
			return this.ToString(IMAP_Mailbox_Encoding.None);
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x0006ECE4 File Offset: 0x0006DCE4
		public override string ToString(IMAP_Mailbox_Encoding encoding)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* STATUS");
			stringBuilder.Append(" " + IMAP_Utils.EncodeMailbox(this.m_FolderName, encoding));
			stringBuilder.Append(" (");
			bool flag = true;
			bool flag2 = this.m_MessageCount >= 0;
			if (flag2)
			{
				stringBuilder.Append("MESSAGES " + this.m_MessageCount);
				flag = false;
			}
			bool flag3 = this.m_RecentCount >= 0;
			if (flag3)
			{
				bool flag4 = !flag;
				if (flag4)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append("RECENT " + this.m_RecentCount);
				flag = false;
			}
			bool flag5 = this.m_UidNext >= 0L;
			if (flag5)
			{
				bool flag6 = !flag;
				if (flag6)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append("UIDNEXT " + this.m_UidNext);
				flag = false;
			}
			bool flag7 = this.m_FolderUid >= 0L;
			if (flag7)
			{
				bool flag8 = !flag;
				if (flag8)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append("UIDVALIDITY " + this.m_FolderUid);
				flag = false;
			}
			bool flag9 = this.m_UnseenCount >= 0;
			if (flag9)
			{
				bool flag10 = !flag;
				if (flag10)
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append("UNSEEN " + this.m_UnseenCount);
			}
			stringBuilder.Append(")\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06001257 RID: 4695 RVA: 0x0006EE94 File Offset: 0x0006DE94
		public string FolderName
		{
			get
			{
				return this.m_FolderName;
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06001258 RID: 4696 RVA: 0x0006EEAC File Offset: 0x0006DEAC
		public int MessagesCount
		{
			get
			{
				return this.m_MessageCount;
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x0006EEC4 File Offset: 0x0006DEC4
		public int RecentCount
		{
			get
			{
				return this.m_RecentCount;
			}
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600125A RID: 4698 RVA: 0x0006EEDC File Offset: 0x0006DEDC
		public long UidNext
		{
			get
			{
				return this.m_UidNext;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x0600125B RID: 4699 RVA: 0x0006EEF4 File Offset: 0x0006DEF4
		public long FolderUid
		{
			get
			{
				return this.m_FolderUid;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x0600125C RID: 4700 RVA: 0x0006EF0C File Offset: 0x0006DF0C
		public int UnseenCount
		{
			get
			{
				return this.m_UnseenCount;
			}
		}

		// Token: 0x04000720 RID: 1824
		private string m_FolderName = "";

		// Token: 0x04000721 RID: 1825
		private int m_MessageCount = 0;

		// Token: 0x04000722 RID: 1826
		private int m_RecentCount = 0;

		// Token: 0x04000723 RID: 1827
		private long m_UidNext = 0L;

		// Token: 0x04000724 RID: 1828
		private long m_FolderUid = 0L;

		// Token: 0x04000725 RID: 1829
		private int m_UnseenCount = 0;
	}
}
