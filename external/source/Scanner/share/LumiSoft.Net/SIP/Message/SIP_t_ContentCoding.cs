using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000067 RID: 103
	public class SIP_t_ContentCoding : SIP_t_Value
	{
		// Token: 0x06000363 RID: 867 RVA: 0x000122EC File Offset: 0x000112EC
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0001231C File Offset: 0x0001131C
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
				throw new SIP_ParseException("Invalid 'content-coding' value, value is missing !");
			}
			this.m_Encoding = text;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00012360 File Offset: 0x00011360
		public override string ToStringValue()
		{
			return this.m_Encoding;
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000366 RID: 870 RVA: 0x00012378 File Offset: 0x00011378
		// (set) Token: 0x06000367 RID: 871 RVA: 0x00012390 File Offset: 0x00011390
		public string Encoding
		{
			get
			{
				return this.m_Encoding;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Encoding value may not be null or empty !");
				}
				bool flag2 = !TextUtils.IsToken(value);
				if (flag2)
				{
					throw new ArgumentException("Encoding value may be 'token' only !");
				}
				this.m_Encoding = value;
			}
		}

		// Token: 0x0400013C RID: 316
		private string m_Encoding = "";
	}
}
