using System;

namespace LumiSoft.Net.WebDav
{
	// Token: 0x0200003D RID: 61
	public class WebDav_p_Default : WebDav_p
	{
		// Token: 0x06000219 RID: 537 RVA: 0x0000D0F0 File Offset: 0x0000C0F0
		public WebDav_p_Default(string nameSpace, string name, string value)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'name' value must be specified.");
			}
			this.m_Namespace = nameSpace;
			this.m_Name = name;
			this.m_Value = value;
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000D164 File Offset: 0x0000C164
		public override string Namespace
		{
			get
			{
				return this.m_Namespace;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000D17C File Offset: 0x0000C17C
		public override string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000D194 File Offset: 0x0000C194
		public override string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040000E7 RID: 231
		private string m_Namespace = "";

		// Token: 0x040000E8 RID: 232
		private string m_Name = null;

		// Token: 0x040000E9 RID: 233
		private string m_Value = null;
	}
}
