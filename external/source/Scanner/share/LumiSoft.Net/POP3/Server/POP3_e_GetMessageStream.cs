using System;
using System.IO;

namespace LumiSoft.Net.POP3.Server
{
	// Token: 0x020000EB RID: 235
	public class POP3_e_GetMessageStream : EventArgs
	{
		// Token: 0x06000978 RID: 2424 RVA: 0x000399C0 File Offset: 0x000389C0
		internal POP3_e_GetMessageStream(POP3_ServerMessage message)
		{
			bool flag = message == null;
			if (flag)
			{
				throw new ArgumentNullException("message");
			}
			this.m_pMessage = message;
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000979 RID: 2425 RVA: 0x00039A08 File Offset: 0x00038A08
		public POP3_ServerMessage Message
		{
			get
			{
				return this.m_pMessage;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600097A RID: 2426 RVA: 0x00039A20 File Offset: 0x00038A20
		// (set) Token: 0x0600097B RID: 2427 RVA: 0x00039A38 File Offset: 0x00038A38
		public bool CloseMessageStream
		{
			get
			{
				return this.m_CloseStream;
			}
			set
			{
				this.m_CloseStream = value;
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600097C RID: 2428 RVA: 0x00039A44 File Offset: 0x00038A44
		// (set) Token: 0x0600097D RID: 2429 RVA: 0x00039A5C File Offset: 0x00038A5C
		public Stream MessageStream
		{
			get
			{
				return this.m_pStream;
			}
			set
			{
				this.m_pStream = value;
			}
		}

		// Token: 0x04000434 RID: 1076
		private POP3_ServerMessage m_pMessage = null;

		// Token: 0x04000435 RID: 1077
		private bool m_CloseStream = true;

		// Token: 0x04000436 RID: 1078
		private Stream m_pStream = null;
	}
}
