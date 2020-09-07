using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x02000187 RID: 391
	public class IMAP_r_u_Bye : IMAP_r_u
	{
		// Token: 0x0600101B RID: 4123 RVA: 0x000647A0 File Offset: 0x000637A0
		public IMAP_r_u_Bye(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			this.m_Text = text;
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x000647D8 File Offset: 0x000637D8
		public static IMAP_r_u_Bye Parse(string byeResponse)
		{
			bool flag = byeResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("byeResponse");
			}
			StringReader stringReader = new StringReader(byeResponse);
			stringReader.ReadWord();
			stringReader.ReadWord();
			return new IMAP_r_u_Bye(stringReader.ReadToEnd());
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x00064820 File Offset: 0x00063820
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* BYE " + this.m_Text + "\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x0600101E RID: 4126 RVA: 0x0006485C File Offset: 0x0006385C
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x04000682 RID: 1666
		private string m_Text = null;
	}
}
