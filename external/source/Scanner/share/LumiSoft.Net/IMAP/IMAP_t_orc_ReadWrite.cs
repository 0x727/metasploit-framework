using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000195 RID: 405
	public class IMAP_t_orc_ReadWrite : IMAP_t_orc
	{
		// Token: 0x06001063 RID: 4195 RVA: 0x00065C44 File Offset: 0x00064C44
		public new static IMAP_t_orc_ReadWrite Parse(StringReader r)
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
			bool flag2 = !string.Equals("READ-WRITE", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid READ-WRITE response value.", "r");
			}
			return new IMAP_t_orc_ReadWrite();
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x00065CB0 File Offset: 0x00064CB0
		public override string ToString()
		{
			return "READ-WRITE";
		}
	}
}
