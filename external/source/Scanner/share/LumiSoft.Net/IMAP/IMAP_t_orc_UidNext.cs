using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000197 RID: 407
	public class IMAP_t_orc_UidNext : IMAP_t_orc
	{
		// Token: 0x06001068 RID: 4200 RVA: 0x00065D4B File Offset: 0x00064D4B
		public IMAP_t_orc_UidNext(int uidNext)
		{
			this.m_UidNext = uidNext;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x00065D64 File Offset: 0x00064D64
		public new static IMAP_t_orc_UidNext Parse(StringReader r)
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
			bool flag2 = !string.Equals("UIDNEXT", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid UIDNEXT response value.", "r");
			}
			bool flag3 = array.Length != 2;
			if (flag3)
			{
				throw new ArgumentException("Invalid UIDNEXT response value.", "r");
			}
			return new IMAP_t_orc_UidNext(Convert.ToInt32(array[1]));
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x00065DF8 File Offset: 0x00064DF8
		public override string ToString()
		{
			return "UIDNEXT " + this.m_UidNext;
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x00065E20 File Offset: 0x00064E20
		public int UidNext
		{
			get
			{
				return this.m_UidNext;
			}
		}

		// Token: 0x0400069D RID: 1693
		private int m_UidNext = 0;
	}
}
