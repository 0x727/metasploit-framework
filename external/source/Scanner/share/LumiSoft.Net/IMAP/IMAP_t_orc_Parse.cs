using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000192 RID: 402
	public class IMAP_t_orc_Parse : IMAP_t_orc
	{
		// Token: 0x06001057 RID: 4183 RVA: 0x000659B8 File Offset: 0x000649B8
		public IMAP_t_orc_Parse(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			this.m_ErrorText = text;
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x000659F0 File Offset: 0x000649F0
		public new static IMAP_t_orc_Parse Parse(StringReader r)
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
			bool flag2 = !string.Equals("PARSE", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid PARSE response value.", "r");
			}
			return new IMAP_t_orc_Parse((array.Length == 2) ? array[1] : "");
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x00065A6C File Offset: 0x00064A6C
		public override string ToString()
		{
			return "PARSE " + this.m_ErrorText;
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x0600105A RID: 4186 RVA: 0x00065A90 File Offset: 0x00064A90
		public string ErrorText
		{
			get
			{
				return this.m_ErrorText;
			}
		}

		// Token: 0x0400069B RID: 1691
		private string m_ErrorText = null;
	}
}
