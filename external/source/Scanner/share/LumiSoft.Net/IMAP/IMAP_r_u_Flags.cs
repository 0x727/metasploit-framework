using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001A1 RID: 417
	public class IMAP_r_u_Flags : IMAP_r_u
	{
		// Token: 0x0600109B RID: 4251 RVA: 0x000675CC File Offset: 0x000665CC
		public IMAP_r_u_Flags(string[] flags)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			this.m_pFlags = flags;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x00067604 File Offset: 0x00066604
		public static IMAP_r_u_Flags Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			StringReader stringReader = new StringReader(response.Split(new char[]
			{
				' '
			}, 3)[2]);
			return new IMAP_r_u_Flags(stringReader.ReadParenthesized().Split(new char[]
			{
				' '
			}));
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x00067660 File Offset: 0x00066660
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* FLAGS (");
			for (int i = 0; i < this.m_pFlags.Length; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(this.m_pFlags[i]);
			}
			stringBuilder.Append(")\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x0600109E RID: 4254 RVA: 0x000676D8 File Offset: 0x000666D8
		public string[] Flags
		{
			get
			{
				return this.m_pFlags;
			}
		}

		// Token: 0x040006A9 RID: 1705
		private string[] m_pFlags = null;
	}
}
