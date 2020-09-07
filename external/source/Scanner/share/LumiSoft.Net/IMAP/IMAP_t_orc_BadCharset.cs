using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018F RID: 399
	public class IMAP_t_orc_BadCharset : IMAP_t_orc
	{
		// Token: 0x06001049 RID: 4169 RVA: 0x0006562C File Offset: 0x0006462C
		public IMAP_t_orc_BadCharset(string[] charsets)
		{
			bool flag = charsets == null;
			if (flag)
			{
				throw new ArgumentNullException("charsets");
			}
			this.m_pCharsets = charsets;
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00065664 File Offset: 0x00064664
		public new static IMAP_t_orc_BadCharset Parse(StringReader r)
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
			bool flag2 = !string.Equals("BADCHARSET", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid BADCHARSET response value.", "r");
			}
			return new IMAP_t_orc_BadCharset(array[1].Trim().Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000656E8 File Offset: 0x000646E8
		public override string ToString()
		{
			return "BADCHARSET " + Net_Utils.ArrayToString(this.m_pCharsets, " ");
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x0600104C RID: 4172 RVA: 0x00065714 File Offset: 0x00064714
		public string[] Charsets
		{
			get
			{
				return this.m_pCharsets;
			}
		}

		// Token: 0x04000696 RID: 1686
		private string[] m_pCharsets = null;
	}
}
