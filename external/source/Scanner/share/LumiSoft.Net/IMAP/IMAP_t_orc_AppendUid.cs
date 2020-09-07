using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018E RID: 398
	public class IMAP_t_orc_AppendUid : IMAP_t_orc
	{
		// Token: 0x06001044 RID: 4164 RVA: 0x000654EC File Offset: 0x000644EC
		public IMAP_t_orc_AppendUid(long mailboxUid, int msgUid)
		{
			this.m_MailboxUid = mailboxUid;
			this.m_MessageUid = msgUid;
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00065514 File Offset: 0x00064514
		public new static IMAP_t_orc_AppendUid Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string[] array = r.ReadParenthesized().Split(new char[]
			{
				' '
			}, 3);
			bool flag2 = !string.Equals("APPENDUID", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid APPENDUID response value.", "r");
			}
			bool flag3 = array.Length != 3;
			if (flag3)
			{
				throw new ArgumentException("Invalid APPENDUID response value.", "r");
			}
			return new IMAP_t_orc_AppendUid(Convert.ToInt64(array[1]), Convert.ToInt32(array[2]));
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000655B0 File Offset: 0x000645B0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"APPENDUID ",
				this.m_MailboxUid,
				" ",
				this.m_MessageUid
			});
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x000655FC File Offset: 0x000645FC
		public long MailboxUid
		{
			get
			{
				return this.m_MailboxUid;
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06001048 RID: 4168 RVA: 0x00065614 File Offset: 0x00064614
		public int MessageUid
		{
			get
			{
				return this.m_MessageUid;
			}
		}

		// Token: 0x04000694 RID: 1684
		private long m_MailboxUid = 0L;

		// Token: 0x04000695 RID: 1685
		private int m_MessageUid = 0;
	}
}
