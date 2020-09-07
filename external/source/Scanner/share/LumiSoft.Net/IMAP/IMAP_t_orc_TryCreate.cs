using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000196 RID: 406
	public class IMAP_t_orc_TryCreate : IMAP_t_orc
	{
		// Token: 0x06001066 RID: 4198 RVA: 0x00065CC8 File Offset: 0x00064CC8
		public new static IMAP_t_orc_TryCreate Parse(StringReader r)
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
			bool flag2 = !string.Equals("TRYCREATE", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid TRYCREATE response value.", "r");
			}
			return new IMAP_t_orc_TryCreate();
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x00065D34 File Offset: 0x00064D34
		public override string ToString()
		{
			return "TRYCREATE";
		}
	}
}
