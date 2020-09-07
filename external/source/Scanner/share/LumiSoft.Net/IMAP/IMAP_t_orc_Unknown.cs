using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000199 RID: 409
	public class IMAP_t_orc_Unknown : IMAP_t_orc
	{
		// Token: 0x06001070 RID: 4208 RVA: 0x00065F28 File Offset: 0x00064F28
		public IMAP_t_orc_Unknown(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Value = value;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x00065F60 File Offset: 0x00064F60
		public new static IMAP_t_orc_Unknown Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			return new IMAP_t_orc_Unknown(r.ReadParenthesized());
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x00065F94 File Offset: 0x00064F94
		public override string ToString()
		{
			return this.m_Value;
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001073 RID: 4211 RVA: 0x00065FAC File Offset: 0x00064FAC
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x0400069F RID: 1695
		private string m_Value = null;
	}
}
