using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200019E RID: 414
	public class IMAP_r_u_Enable : IMAP_r_u
	{
		// Token: 0x0600107C RID: 4220 RVA: 0x000660EC File Offset: 0x000650EC
		public IMAP_r_u_Enable(string[] capabilities)
		{
			bool flag = capabilities == null;
			if (flag)
			{
				throw new ArgumentNullException("capabilities");
			}
			this.m_Capabilities = capabilities;
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x00066124 File Offset: 0x00065124
		public static IMAP_r_u_Enable Parse(string enableResponse)
		{
			bool flag = enableResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("enableResponse");
			}
			StringReader stringReader = new StringReader(enableResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			return new IMAP_r_u_Enable(stringReader.ReadToEnd().Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x0006617C File Offset: 0x0006517C
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* ENABLED");
			foreach (string str in this.m_Capabilities)
			{
				stringBuilder.Append(" " + str);
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x0600107F RID: 4223 RVA: 0x000661E4 File Offset: 0x000651E4
		public string[] Capabilities
		{
			get
			{
				return this.m_Capabilities;
			}
		}

		// Token: 0x040006A5 RID: 1701
		private string[] m_Capabilities = null;
	}
}
