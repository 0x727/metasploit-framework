using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000066 RID: 102
	public class SIP_t_Info : SIP_t_ValueWithParams
	{
		// Token: 0x0600035D RID: 861 RVA: 0x00012190 File Offset: 0x00011190
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000121C0 File Offset: 0x000111C0
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			reader.QuotedReadToDelimiter('<');
			bool flag2 = !reader.StartsWith("<");
			if (flag2)
			{
				throw new SIP_ParseException("Invalid Alert-Info value, Uri not between <> !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00012214 File Offset: 0x00011214
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<" + this.m_Uri + ">");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000360 RID: 864 RVA: 0x0001225C File Offset: 0x0001125C
		// (set) Token: 0x06000361 RID: 865 RVA: 0x00012294 File Offset: 0x00011294
		public string Purpose
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["purpose"];
				bool flag = sip_Parameter != null;
				string result;
				if (flag)
				{
					result = sip_Parameter.Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					base.Parameters.Remove("purpose");
				}
				else
				{
					base.Parameters.Set("purpose", value);
				}
			}
		}

		// Token: 0x0400013B RID: 315
		private string m_Uri = "";
	}
}
