using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001A2 RID: 418
	public class IMAP_r_u_Recent : IMAP_r_u
	{
		// Token: 0x0600109F RID: 4255 RVA: 0x000676F0 File Offset: 0x000666F0
		public IMAP_r_u_Recent(int messageCount)
		{
			bool flag = messageCount < 0;
			if (flag)
			{
				throw new ArgumentException("Arguments 'messageCount' value must be >= 0.", "messageCount");
			}
			this.m_MessageCount = messageCount;
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0006772C File Offset: 0x0006672C
		public static IMAP_r_u_Recent Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			return new IMAP_r_u_Recent(Convert.ToInt32(response.Split(new char[]
			{
				' '
			})[1]));
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x00067770 File Offset: 0x00066770
		public override string ToString()
		{
			return "* " + this.m_MessageCount.ToString() + " RECENT\r\n";
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x060010A2 RID: 4258 RVA: 0x0006779C File Offset: 0x0006679C
		public int MessageCount
		{
			get
			{
				return this.m_MessageCount;
			}
		}

		// Token: 0x040006AA RID: 1706
		private int m_MessageCount = 0;
	}
}
