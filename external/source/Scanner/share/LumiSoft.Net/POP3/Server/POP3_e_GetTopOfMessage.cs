using System;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000EC RID: 236
	public class POP3_e_GetTopOfMessage : EventArgs
	{
		// Token: 0x0600097E RID: 2430 RVA: 0x00039A68 File Offset: 0x00038A68
		internal POP3_e_GetTopOfMessage(POP3_ServerMessage message, int lines)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			bool flag2 = lines < 0;
			if (flag2)
			{
				throw new ArgumentException("Argument 'lines' value must be >= 0.", "lines");
			}
			this.m_pMessage = message;
			this.m_LineCount = lines;
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600097F RID: 2431 RVA: 0x00039AD0 File Offset: 0x00038AD0
		public POP3_ServerMessage Message
		{
			get
			{
				return this.m_pMessage;
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000980 RID: 2432 RVA: 0x00039AE8 File Offset: 0x00038AE8
		public int LineCount
		{
			get
			{
				return this.m_LineCount;
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000981 RID: 2433 RVA: 0x00039B00 File Offset: 0x00038B00
		// (set) Token: 0x06000982 RID: 2434 RVA: 0x00039B18 File Offset: 0x00038B18
		public byte[] Data
		{
			get
			{
				return this.m_pData;
			}
			set
			{
				this.m_pData = value;
			}
		}

		// Token: 0x04000437 RID: 1079
		private POP3_ServerMessage m_pMessage = null;

		// Token: 0x04000438 RID: 1080
		private int m_LineCount = 0;

		// Token: 0x04000439 RID: 1081
		private byte[] m_pData = null;
	}
}
