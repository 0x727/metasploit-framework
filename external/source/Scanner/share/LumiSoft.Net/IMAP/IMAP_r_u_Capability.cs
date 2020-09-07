using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001FD RID: 509
	public class IMAP_r_u_Capability : IMAP_r_u
	{
		// Token: 0x06001210 RID: 4624 RVA: 0x0006D338 File Offset: 0x0006C338
		public IMAP_r_u_Capability(string[] capabilities)
		{
			bool flag = capabilities == null;
			if (flag)
			{
				throw new ArgumentNullException("capabilities");
			}
			this.m_pCapabilities = capabilities;
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0006D370 File Offset: 0x0006C370
		public static IMAP_r_u_Capability Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response);
			stringReader.ReadWord();
			stringReader.ReadWord();
			string[] capabilities = stringReader.ReadToEnd().Split(new char[]
			{
				' '
			});
			return new IMAP_r_u_Capability(capabilities);
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0006D3C8 File Offset: 0x0006C3C8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* CAPABILITY");
			foreach (string str in this.m_pCapabilities)
			{
				stringBuilder.Append(" " + str);
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x170005FF RID: 1535
		// (get) Token: 0x06001213 RID: 4627 RVA: 0x0006D430 File Offset: 0x0006C430
		public string[] Capabilities
		{
			get
			{
				return this.m_pCapabilities;
			}
		}

		// Token: 0x04000707 RID: 1799
		private string[] m_pCapabilities = null;
	}
}
