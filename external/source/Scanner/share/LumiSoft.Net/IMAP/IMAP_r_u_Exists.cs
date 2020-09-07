using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200019F RID: 415
	public class IMAP_r_u_Exists : IMAP_r_u
	{
		// Token: 0x06001080 RID: 4224 RVA: 0x000661FC File Offset: 0x000651FC
		public IMAP_r_u_Exists(int messageCount)
		{
			bool flag = messageCount < 0;
			if (flag)
			{
				throw new ArgumentException("Arguments 'messageCount' value must be >= 0.", "messageCount");
			}
			this.m_MessageCount = messageCount;
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x00066238 File Offset: 0x00065238
		public static IMAP_r_u_Exists Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			return new IMAP_r_u_Exists(Convert.ToInt32(response.Split(new char[]
			{
				' '
			})[1]));
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0006627C File Offset: 0x0006527C
		public override string ToString()
		{
			return "* " + this.m_MessageCount.ToString() + " EXISTS\r\n";
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06001083 RID: 4227 RVA: 0x000662A8 File Offset: 0x000652A8
		public int MessageCount
		{
			get
			{
				return this.m_MessageCount;
			}
		}

		// Token: 0x040006A6 RID: 1702
		private int m_MessageCount = 0;
	}
}
