using System;
using System.Collections.Generic;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x0200026F RID: 623
	public abstract class AUTH_SASL_ServerMechanism
	{
		// Token: 0x06001672 RID: 5746 RVA: 0x0008C3F5 File Offset: 0x0008B3F5
		public AUTH_SASL_ServerMechanism()
		{
		}

		// Token: 0x06001673 RID: 5747
		public abstract void Reset();

		// Token: 0x06001674 RID: 5748
		public abstract byte[] Continue(byte[] clientResponse);

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06001675 RID: 5749
		public abstract bool IsCompleted { get; }

		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06001676 RID: 5750
		public abstract bool IsAuthenticated { get; }

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06001677 RID: 5751
		public abstract string Name { get; }

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06001678 RID: 5752
		public abstract bool RequireSSL { get; }

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06001679 RID: 5753
		public abstract string UserName { get; }

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x0600167A RID: 5754 RVA: 0x0008C408 File Offset: 0x0008B408
		public Dictionary<string, object> Tags
		{
			get
			{
				return this.m_pTags;
			}
		}

		// Token: 0x040008FF RID: 2303
		private Dictionary<string, object> m_pTags = null;
	}
}
