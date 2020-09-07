using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000190 RID: 400
	public class IMAP_t_orc_Capability : IMAP_t_orc
	{
		// Token: 0x0600104D RID: 4173 RVA: 0x0006572C File Offset: 0x0006472C
		public IMAP_t_orc_Capability(string[] capabilities)
		{
			bool flag = capabilities == null;
			if (flag)
			{
				throw new ArgumentNullException("capabilities");
			}
			this.m_pCapabilities = capabilities;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00065764 File Offset: 0x00064764
		public new static IMAP_t_orc_Capability Parse(StringReader r)
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
			bool flag2 = !string.Equals("CAPABILITY", array[0], StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ArgumentException("Invalid CAPABILITY response value.", "r");
			}
			bool flag3 = array.Length != 2;
			if (flag3)
			{
				throw new ArgumentException("Invalid CAPABILITY response value.", "r");
			}
			return new IMAP_t_orc_Capability(array[1].Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00065800 File Offset: 0x00064800
		public override string ToString()
		{
			return "CAPABILITY (" + Net_Utils.ArrayToString(this.m_pCapabilities, " ") + ")";
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06001050 RID: 4176 RVA: 0x00065834 File Offset: 0x00064834
		public string[] Capabilities
		{
			get
			{
				return this.m_pCapabilities;
			}
		}

		// Token: 0x04000697 RID: 1687
		private string[] m_pCapabilities = null;
	}
}
