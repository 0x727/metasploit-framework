using System;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000160 RID: 352
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class HeaderFieldParameter
	{
		// Token: 0x06000E28 RID: 3624 RVA: 0x00057955 File Offset: 0x00056955
		public HeaderFieldParameter(string parameterName, string parameterValue)
		{
			this.m_Name = parameterName;
			this.m_Value = parameterValue;
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x00057984 File Offset: 0x00056984
		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0005799C File Offset: 0x0005699C
		public string Value
		{
			get
			{
				return this.m_Value;
			}
		}

		// Token: 0x040005F0 RID: 1520
		private string m_Name = "";

		// Token: 0x040005F1 RID: 1521
		private string m_Value = "";
	}
}
