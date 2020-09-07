using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000060 RID: 96
	public class SIP_t_Encoding : SIP_t_ValueWithParams
	{
		// Token: 0x06000329 RID: 809 RVA: 0x00011350 File Offset: 0x00010350
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00011380 File Offset: 0x00010380
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
				throw new SIP_ParseException("Invalid 'encoding' value is missing !");
			}
			this.m_ContentEncoding = text;
			base.ParseParameters(reader);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x000113CC File Offset: 0x000103CC
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_ContentEncoding);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00011404 File Offset: 0x00010404
		// (set) Token: 0x0600032D RID: 813 RVA: 0x0001141C File Offset: 0x0001041C
		public string ContentEncoding
		{
			get
			{
				return this.m_ContentEncoding;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property ContentEncoding value can't be null or empty !");
				}
				bool flag2 = !TextUtils.IsToken(value);
				if (flag2)
				{
					throw new ArgumentException("Property ContentEncoding value may be 'token' only !");
				}
				this.m_ContentEncoding = value;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00011460 File Offset: 0x00010460
		// (set) Token: 0x0600032F RID: 815 RVA: 0x000114A4 File Offset: 0x000104A4
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

		// Token: 0x0400012F RID: 303
		private string m_ContentEncoding = "";
	}
}
