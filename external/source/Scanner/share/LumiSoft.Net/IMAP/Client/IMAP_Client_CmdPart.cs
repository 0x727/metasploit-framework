using System;

namespace LumiSoft.Net.IMAP.Client
{
	// Token: 0x02000228 RID: 552
	internal class IMAP_Client_CmdPart
	{
		// Token: 0x060013A4 RID: 5028 RVA: 0x000782D8 File Offset: 0x000772D8
		public IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type type, string data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			this.m_Type = type;
			this.m_Value = data;
		}

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x060013A5 RID: 5029 RVA: 0x00078320 File Offset: 0x00077320
		public IMAP_Client_CmdPart_Type Type
		{
			get
			{
				return this.m_Type;
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x060013A6 RID: 5030 RVA: 0x00078338 File Offset: 0x00077338
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040007BF RID: 1983
		private IMAP_Client_CmdPart_Type m_Type = IMAP_Client_CmdPart_Type.Constant;

		// Token: 0x040007C0 RID: 1984
		private string m_Value = null;
	}
}
