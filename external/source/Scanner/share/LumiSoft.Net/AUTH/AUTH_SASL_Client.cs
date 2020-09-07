using System;

namespace LumiSoft.Net.AUTH
{
	// Token: 0x02000261 RID: 609
	public abstract class AUTH_SASL_Client
	{
		// Token: 0x060015E4 RID: 5604 RVA: 0x00009954 File Offset: 0x00008954
		public AUTH_SASL_Client()
		{
		}

		// Token: 0x060015E5 RID: 5605
		public abstract byte[] Continue(byte[] serverResponse);

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x060015E6 RID: 5606
		public abstract bool IsCompleted { get; }

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060015E7 RID: 5607
		public abstract string Name { get; }

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060015E8 RID: 5608
		public abstract string UserName { get; }

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x00088EDC File Offset: 0x00087EDC
		public virtual bool SupportsInitialResponse
		{
			get
			{
				return false;
			}
		}
	}
}
