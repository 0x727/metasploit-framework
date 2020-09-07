using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000193 RID: 403
	public class IMAP_t_orc_PermanentFlags : IMAP_t_orc
	{
		// Token: 0x0600105B RID: 4187 RVA: 0x00065AA8 File Offset: 0x00064AA8
		public IMAP_t_orc_PermanentFlags(string[] flags)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			this.m_pFlags = flags;
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00065AE0 File Offset: 0x00064AE0
		public new static IMAP_t_orc_PermanentFlags Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			bool flag2 = !r.StartsWith("[PERMANENTFLAGS", false);
			if (flag2)
			{
				throw new ArgumentException("Invalid PERMANENTFLAGS response value.", "r");
			}
			r.ReadSpecifiedLength(1);
			r.ReadWord();
			r.ReadToFirstChar();
			string[] flags = r.ReadParenthesized().Split(new char[]
			{
				' '
			});
			r.ReadSpecifiedLength(1);
			return new IMAP_t_orc_PermanentFlags(flags);
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00065B68 File Offset: 0x00064B68
		public override string ToString()
		{
			return "PERMANENTFLAGS (" + Net_Utils.ArrayToString(this.m_pFlags, " ") + ")";
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x0600105E RID: 4190 RVA: 0x00065B9C File Offset: 0x00064B9C
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x0400069C RID: 1692
		private string[] m_pFlags = null;
	}
}
