using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006B RID: 107
	public class SIP_t_ErrorUri : SIP_t_ValueWithParams
	{
		// Token: 0x06000380 RID: 896 RVA: 0x0001283C File Offset: 0x0001183C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0001286C File Offset: 0x0001186C
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
				throw new SIP_ParseException("Invalid 'error-uri' value, Uri not between <> !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x000128C0 File Offset: 0x000118C0
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<" + this.m_Uri + ">");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000383 RID: 899 RVA: 0x00012908 File Offset: 0x00011908
		// (set) Token: 0x06000384 RID: 900 RVA: 0x00012920 File Offset: 0x00011920
		public string Uri
		{
			get
			{
				return this.m_Uri;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Uri value can't be null or empty !");
				}
				this.m_Uri = value;
			}
		}

		// Token: 0x04000142 RID: 322
		private string m_Uri = "";
	}
}
