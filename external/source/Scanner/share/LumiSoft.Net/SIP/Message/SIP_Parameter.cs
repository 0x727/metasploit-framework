using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000044 RID: 68
	public class SIP_Parameter
	{
		// Token: 0x06000241 RID: 577 RVA: 0x0000DF3D File Offset: 0x0000CF3D
		public SIP_Parameter(string name) : this(name, "")
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000DF50 File Offset: 0x0000CF50
		public SIP_Parameter(string name, string value)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == "";
			if (flag2)
			{
				throw new ArgumentException("Parameter 'name' value may no be empty string !");
			}
			this.m_Name = name;
			this.m_Value = value;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000DFB8 File Offset: 0x0000CFB8
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000244 RID: 580 RVA: 0x0000DFD0 File Offset: 0x0000CFD0
		// (set) Token: 0x06000245 RID: 581 RVA: 0x0000DFE8 File Offset: 0x0000CFE8
		public string Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		// Token: 0x04000102 RID: 258
		private string m_Name = "";

		// Token: 0x04000103 RID: 259
		private string m_Value = "";
	}
}
