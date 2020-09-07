using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006D RID: 109
	public class SIP_t_CallID : SIP_t_Value
	{
		// Token: 0x0600038E RID: 910 RVA: 0x00012ADC File Offset: 0x00011ADC
		public static SIP_t_CallID CreateCallID()
		{
			return new SIP_t_CallID
			{
				CallID = Guid.NewGuid().ToString().Replace("-", "")
			};
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00012B20 File Offset: 0x00011B20
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00012B50 File Offset: 0x00011B50
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
				throw new SIP_ParseException("Invalid 'callid' value, callid is missing !");
			}
			this.m_CallID = text;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00012B94 File Offset: 0x00011B94
		public override string ToStringValue()
		{
			return this.m_CallID;
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00012BAC File Offset: 0x00011BAC
		// (set) Token: 0x06000393 RID: 915 RVA: 0x00012BC4 File Offset: 0x00011BC4
		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property CallID value may not be null or empty !");
				}
				this.m_CallID = value;
			}
		}

		// Token: 0x04000144 RID: 324
		private string m_CallID = "";
	}
}
