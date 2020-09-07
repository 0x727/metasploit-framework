using System;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200010E RID: 270
	public class MIME_h_Parameter
	{
		// Token: 0x06000A83 RID: 2691 RVA: 0x0003FFF8 File Offset: 0x0003EFF8
		public MIME_h_Parameter(string name, string value)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			this.m_Name = name;
			this.m_Value = value;
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000A84 RID: 2692 RVA: 0x0004004C File Offset: 0x0003F04C
		public bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000A85 RID: 2693 RVA: 0x00040064 File Offset: 0x0003F064
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000A86 RID: 2694 RVA: 0x0004007C File Offset: 0x0003F07C
		// (set) Token: 0x06000A87 RID: 2695 RVA: 0x00040094 File Offset: 0x0003F094
		public string Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x0400046D RID: 1133
		private bool m_IsModified = false;

		// Token: 0x0400046E RID: 1134
		private string m_Name = "";

		// Token: 0x0400046F RID: 1135
		private string m_Value = "";
	}
}
