using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000075 RID: 117
	public abstract class SIP_t_Value
	{
		// Token: 0x060003CB RID: 971 RVA: 0x00009954 File Offset: 0x00008954
		public SIP_t_Value()
		{
		}

		// Token: 0x060003CC RID: 972
		public abstract void Parse(StringReader reader);

		// Token: 0x060003CD RID: 973
		public abstract string ToStringValue();
	}
}
