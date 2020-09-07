using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F8 RID: 504
	public class IMAP_Quota_Entry
	{
		// Token: 0x060011F6 RID: 4598 RVA: 0x0006CC6C File Offset: 0x0006BC6C
		public IMAP_Quota_Entry(string resourceName, long currentUsage, long maxUsage)
		{
			bool flag = resourceName == null;
			if (flag)
			{
				throw new ArgumentNullException("resourceName");
			}
			bool flag2 = resourceName == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'resourceName' value must be specified.", "resourceName");
			}
			this.m_ResourceName = resourceName;
			this.m_CurrentUsage = currentUsage;
			this.m_MaxUsage = maxUsage;
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x0006CCE8 File Offset: 0x0006BCE8
		public string ResourceName
		{
			get
			{
				return this.m_ResourceName;
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x060011F8 RID: 4600 RVA: 0x0006CD00 File Offset: 0x0006BD00
		public long CurrentUsage
		{
			get
			{
				return this.m_CurrentUsage;
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x060011F9 RID: 4601 RVA: 0x0006CD18 File Offset: 0x0006BD18
		public long MaxUsage
		{
			get
			{
				return this.m_MaxUsage;
			}
		}

		// Token: 0x040006FF RID: 1791
		private string m_ResourceName = "";

		// Token: 0x04000700 RID: 1792
		private long m_CurrentUsage = 0L;

		// Token: 0x04000701 RID: 1793
		private long m_MaxUsage = 0L;
	}
}
