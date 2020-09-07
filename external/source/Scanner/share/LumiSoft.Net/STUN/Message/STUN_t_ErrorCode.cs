using System;

namespace LumiSoft.Net.STUN.Message
{
	// Token: 0x02000027 RID: 39
	public class STUN_t_ErrorCode
	{
		// Token: 0x06000136 RID: 310 RVA: 0x00008C16 File Offset: 0x00007C16
		public STUN_t_ErrorCode(int code, string reasonText)
		{
			this.m_Code = code;
			this.m_ReasonText = reasonText;
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00008C40 File Offset: 0x00007C40
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00008C58 File Offset: 0x00007C58
		public int Code
		{
			get
			{
				return this.m_Code;
			}
			set
			{
				this.m_Code = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00008C64 File Offset: 0x00007C64
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00008C7C File Offset: 0x00007C7C
		public string ReasonText
		{
			get
			{
				return this.m_ReasonText;
			}
			set
			{
				this.m_ReasonText = value;
			}
		}

		// Token: 0x0400008A RID: 138
		private int m_Code = 0;

		// Token: 0x0400008B RID: 139
		private string m_ReasonText = "";
	}
}
