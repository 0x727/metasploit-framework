using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018D RID: 397
	public class IMAP_t_orc_Alert : IMAP_t_orc
	{
		// Token: 0x06001040 RID: 4160 RVA: 0x000653FC File Offset: 0x000643FC
		public IMAP_t_orc_Alert(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			this.m_AlertText = text;
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00065434 File Offset: 0x00064434
		public new static IMAP_t_orc_Alert Parse(StringReader r)
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
			bool flag2 = !string.Equals("ALERT", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid ALERT response value.", "r");
			}
			return new IMAP_t_orc_Alert((array.Length == 2) ? array[1] : "");
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x000654B0 File Offset: 0x000644B0
		public override string ToString()
		{
			return "ALERT " + this.m_AlertText;
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x000654D4 File Offset: 0x000644D4
		public string AlertText
		{
			get
			{
				return this.m_AlertText;
			}
		}

		// Token: 0x04000693 RID: 1683
		private string m_AlertText = null;
	}
}
