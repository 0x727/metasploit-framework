using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000194 RID: 404
	public class IMAP_t_orc_ReadOnly : IMAP_t_orc
	{
		// Token: 0x06001060 RID: 4192 RVA: 0x00065BC0 File Offset: 0x00064BC0
		public new static IMAP_t_orc_ReadOnly Parse(StringReader r)
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
			bool flag2 = !string.Equals("READ-ONLY", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid READ-ONLY response value.", "r");
			}
			return new IMAP_t_orc_ReadOnly();
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00065C2C File Offset: 0x00064C2C
		public override string ToString()
		{
			return "READ-ONLY";
		}
	}
}
