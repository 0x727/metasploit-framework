using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200019A RID: 410
	public class IMAP_t_orc_Unseen : IMAP_t_orc
	{
		// Token: 0x06001074 RID: 4212 RVA: 0x00065FC4 File Offset: 0x00064FC4
		public IMAP_t_orc_Unseen(int firstUnseen)
		{
			this.m_FirstUnseen = firstUnseen;
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x00065FDC File Offset: 0x00064FDC
		public new static IMAP_t_orc_Unseen Parse(StringReader r)
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
			bool flag2 = !string.Equals("UNSEEN", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid UNSEEN response value.", "r");
			}
			bool flag3 = array.Length != 2;
			if (flag3)
			{
				throw new ArgumentException("Invalid UNSEEN response value.", "r");
			}
			return new IMAP_t_orc_Unseen(Convert.ToInt32(array[1]));
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x00066070 File Offset: 0x00065070
		public override string ToString()
		{
			return "UNSEEN " + this.m_FirstUnseen;
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x00066098 File Offset: 0x00065098
		public int SeqNo
		{
			get
			{
				return this.m_FirstUnseen;
			}
		}

		// Token: 0x040006A0 RID: 1696
		private int m_FirstUnseen = 0;
	}
}
