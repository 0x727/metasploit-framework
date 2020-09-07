using System;

namespace LumiSoft.Net.SIP.Stack
{
	// Token: 0x02000088 RID: 136
	public class SIP_Dialog_Subscribe
	{
		// Token: 0x060004FB RID: 1275 RVA: 0x00009954 File Offset: 0x00008954
		internal SIP_Dialog_Subscribe()
		{
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x000199B0 File Offset: 0x000189B0
		public void Notify(SIP_Request notify)
		{
			bool flag = notify == null;
			if (flag)
			{
				throw new ArgumentNullException("notify");
			}
		}
	}
}
