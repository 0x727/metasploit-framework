using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000191 RID: 401
	public class IMAP_t_orc_CopyUid : IMAP_t_orc
	{
		// Token: 0x06001051 RID: 4177 RVA: 0x0006584C File Offset: 0x0006484C
		public IMAP_t_orc_CopyUid(long targetMailboxUid, IMAP_t_SeqSet sourceSeqSet, IMAP_t_SeqSet targetSeqSet)
		{
			bool flag = sourceSeqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("sourceSeqSet");
			}
			bool flag2 = targetSeqSet == null;
			if (flag2)
			{
				throw new ArgumentNullException("targetSeqSet");
			}
			this.m_TargetMailboxUid = targetMailboxUid;
			this.m_pSourceSeqSet = sourceSeqSet;
			this.m_pTargetSeqSet = targetSeqSet;
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x000658B4 File Offset: 0x000648B4
		public new static IMAP_t_orc_CopyUid Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string[] array = r.ReadParenthesized().Split(new char[]
			{
				' '
			}, 4);
			bool flag2 = !string.Equals("COPYUID", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid COPYUID response value.", "r");
			}
			bool flag3 = array.Length != 4;
			if (flag3)
			{
				throw new ArgumentException("Invalid COPYUID response value.", "r");
			}
			return new IMAP_t_orc_CopyUid(Convert.ToInt64(array[1]), IMAP_t_SeqSet.Parse(array[2]), IMAP_t_SeqSet.Parse(array[3]));
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00065958 File Offset: 0x00064958
		public override string ToString()
		{
			return "COPYUID m_MailboxUid m_MessageUid";
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06001054 RID: 4180 RVA: 0x00065970 File Offset: 0x00064970
		public long TargetMailboxUid
		{
			get
			{
				return this.m_TargetMailboxUid;
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x00065988 File Offset: 0x00064988
		public IMAP_t_SeqSet SourceSeqSet
		{
			get
			{
				return this.m_pSourceSeqSet;
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06001056 RID: 4182 RVA: 0x000659A0 File Offset: 0x000649A0
		public IMAP_t_SeqSet TargetSeqSet
		{
			get
			{
				return this.m_pTargetSeqSet;
			}
		}

		// Token: 0x04000698 RID: 1688
		private long m_TargetMailboxUid = 0L;

		// Token: 0x04000699 RID: 1689
		private IMAP_t_SeqSet m_pSourceSeqSet = null;

		// Token: 0x0400069A RID: 1690
		private IMAP_t_SeqSet m_pTargetSeqSet = null;
	}
}
