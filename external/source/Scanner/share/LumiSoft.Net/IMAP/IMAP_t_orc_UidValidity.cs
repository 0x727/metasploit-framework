using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000198 RID: 408
	public class IMAP_t_orc_UidValidity : IMAP_t_orc
	{
		// Token: 0x0600106C RID: 4204 RVA: 0x00065E38 File Offset: 0x00064E38
		public IMAP_t_orc_UidValidity(long uid)
		{
			this.m_Uid = uid;
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x00065E54 File Offset: 0x00064E54
		public new static IMAP_t_orc_UidValidity Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string[] array = r.ReadParenthesized().Split(new char[]
			{
				' '
			}, 2);
			bool flag2 = !string.Equals("UIDVALIDITY", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid UIDVALIDITY response value.", "r");
			}
			bool flag3 = array.Length != 2;
			if (flag3)
			{
				throw new ArgumentException("Invalid UIDVALIDITY response value.", "r");
			}
			return new IMAP_t_orc_UidValidity(Convert.ToInt64(array[1]));
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x00065EE8 File Offset: 0x00064EE8
		public override string ToString()
		{
			return "UIDVALIDITY " + this.m_Uid;
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x0600106F RID: 4207 RVA: 0x00065F10 File Offset: 0x00064F10
		public long Uid
		{
			get
			{
				return this.m_Uid;
			}
		}

		// Token: 0x0400069E RID: 1694
		private long m_Uid = 0L;
	}
}
