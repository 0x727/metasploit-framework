using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000159 RID: 345
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public abstract class Address
	{
		// Token: 0x06000DF4 RID: 3572 RVA: 0x00056C6A File Offset: 0x00055C6A
		public Address(bool groupAddress)
		{
			this.m_GroupAddress = groupAddress;
		}

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00056C8C File Offset: 0x00055C8C
		public bool IsGroupAddress
		{
			get
			{
				return this.m_GroupAddress;
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x00056CA4 File Offset: 0x00055CA4
		// (set) Token: 0x06000DF7 RID: 3575 RVA: 0x00056CBC File Offset: 0x00055CBC
		internal object Owner
		{
			get
			{
				return this.m_pOwner;
			}
			set
			{
				this.m_pOwner = value;
			}
		}

		// Token: 0x040005DA RID: 1498
		private bool m_GroupAddress = false;

		// Token: 0x040005DB RID: 1499
		private object m_pOwner = null;
	}
}
