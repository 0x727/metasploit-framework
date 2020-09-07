using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000061 RID: 97
	public class SIP_t_Language : SIP_t_ValueWithParams
	{
		// Token: 0x06000331 RID: 817 RVA: 0x00011534 File Offset: 0x00010534
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00011564 File Offset: 0x00010564
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
				throw new SIP_ParseException("Invalid Accept-Language value, language-range value is missing !");
			}
			this.m_LanguageRange = text;
			base.ParseParameters(reader);
		}

		// Token: 0x06000333 RID: 819 RVA: 0x000115B0 File Offset: 0x000105B0
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_LanguageRange);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000334 RID: 820 RVA: 0x000115E8 File Offset: 0x000105E8
		// (set) Token: 0x06000335 RID: 821 RVA: 0x00011600 File Offset: 0x00010600
		public string LanguageRange
		{
			get
			{
				return this.m_LanguageRange;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property LanguageRange value can't be null or empty !");
				}
				this.m_LanguageRange = value;
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000336 RID: 822 RVA: 0x0001162C File Offset: 0x0001062C
		// (set) Token: 0x06000337 RID: 823 RVA: 0x00011670 File Offset: 0x00010670
		public double QValue
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["qvalue"];
				bool flag = sip_Parameter != null;
				double result;
				if (flag)
				{
					result = Convert.ToDouble(sip_Parameter.Value);
				}
				else
				{
					result = -1.0;
				}
				return result;
			}
			set
			{
				bool flag = value < 0.0 || value > 1.0;
				if (flag)
				{
					throw new ArgumentException("Property QValue value must be between 0.0 and 1.0 !");
				}
				bool flag2 = value < 0.0;
				if (flag2)
				{
					base.Parameters.Remove("qvalue");
				}
				else
				{
					base.Parameters.Set("qvalue", value.ToString());
				}
			}
		}

		// Token: 0x04000130 RID: 304
		private string m_LanguageRange = "";
	}
}
