using System;

namespace LumiSoft.Net.STUN.Message
{
	// Token: 0x02000026 RID: 38
	public class STUN_t_ChangeRequest
	{
		// Token: 0x06000130 RID: 304 RVA: 0x00008B92 File Offset: 0x00007B92
		public STUN_t_ChangeRequest()
		{
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00008BAA File Offset: 0x00007BAA
		public STUN_t_ChangeRequest(bool changeIP, bool changePort)
		{
			this.m_ChangeIP = changeIP;
			this.m_ChangePort = changePort;
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00008BD0 File Offset: 0x00007BD0
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00008BE8 File Offset: 0x00007BE8
		public bool ChangeIP
		{
			get
			{
				return this.m_ChangeIP;
			}
			set
			{
				this.m_ChangeIP = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00008BF4 File Offset: 0x00007BF4
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00008C0C File Offset: 0x00007C0C
		public bool ChangePort
		{
			get
			{
				return this.m_ChangePort;
			}
			set
			{
				this.m_ChangePort = value;
			}
		}

		// Token: 0x04000088 RID: 136
		private bool m_ChangeIP = true;

		// Token: 0x04000089 RID: 137
		private bool m_ChangePort = true;
	}
}
