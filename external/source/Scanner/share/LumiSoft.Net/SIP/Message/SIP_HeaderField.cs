using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200007A RID: 122
	public class SIP_HeaderField
	{
		// Token: 0x0600046C RID: 1132 RVA: 0x00015A71 File Offset: 0x00014A71
		internal SIP_HeaderField(string name, string value)
		{
			this.m_Name = name;
			this.m_Value = value;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00015AA6 File Offset: 0x00014AA6
		internal void SetMultiValue(bool value)
		{
			this.m_IsMultiValue = value;
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00015AB0 File Offset: 0x00014AB0
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00015AC8 File Offset: 0x00014AC8
		// (set) Token: 0x06000470 RID: 1136 RVA: 0x00015AE0 File Offset: 0x00014AE0
		public virtual string Value
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

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x00015AEC File Offset: 0x00014AEC
		public bool IsMultiValue
		{
			get
			{
				return this.m_IsMultiValue;
			}
		}

		// Token: 0x04000158 RID: 344
		private string m_Name = "";

		// Token: 0x04000159 RID: 345
		private string m_Value = "";

		// Token: 0x0400015A RID: 346
		private bool m_IsMultiValue = false;
	}
}
