using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000069 RID: 105
	public class SIP_t_Credentials : SIP_t_Value
	{
		// Token: 0x0600036E RID: 878 RVA: 0x000124E3 File Offset: 0x000114E3
		public SIP_t_Credentials(string value)
		{
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00012510 File Offset: 0x00011510
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00012540 File Offset: 0x00011540
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			string text = reader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new SIP_ParseException("Invalid 'credentials' value, authentication method is missing !");
			}
			this.m_Method = text;
			text = reader.ReadToEnd();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("Invalid 'credentials' value, authentication parameters are missing !");
			}
			this.m_AuthData = text.Trim();
		}

		// Token: 0x06000371 RID: 881 RVA: 0x000125AC File Offset: 0x000115AC
		public override string ToStringValue()
		{
			return this.m_Method + " " + this.m_AuthData;
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000125D4 File Offset: 0x000115D4
		// (set) Token: 0x06000373 RID: 883 RVA: 0x000125EC File Offset: 0x000115EC
		public string Method
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Method value cant be null or mepty !");
				}
				this.m_Method = value;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000374 RID: 884 RVA: 0x00012618 File Offset: 0x00011618
		// (set) Token: 0x06000375 RID: 885 RVA: 0x00012630 File Offset: 0x00011630
		public string AuthData
		{
			get
			{
				return this.m_AuthData;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property AuthData value cant be null or mepty !");
				}
				this.m_AuthData = value;
			}
		}

		// Token: 0x0400013E RID: 318
		private string m_Method = "";

		// Token: 0x0400013F RID: 319
		private string m_AuthData = "";
	}
}
