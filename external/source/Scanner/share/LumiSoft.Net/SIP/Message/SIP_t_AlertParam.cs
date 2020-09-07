using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000063 RID: 99
	public class SIP_t_AlertParam : SIP_t_ValueWithParams
	{
		// Token: 0x06000343 RID: 835 RVA: 0x00011B88 File Offset: 0x00010B88
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00011BB8 File Offset: 0x00010BB8
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			reader.ReadToFirstChar();
			bool flag2 = !reader.StartsWith("<");
			if (flag2)
			{
				throw new SIP_ParseException("Invalid Alert-Info value, Uri not between <> !");
			}
			this.m_Uri = reader.ReadParenthesized();
			base.ParseParameters(reader);
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00011C14 File Offset: 0x00010C14
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<" + this.m_Uri + ">");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00011C5C File Offset: 0x00010C5C
		// (set) Token: 0x06000347 RID: 839 RVA: 0x00011C74 File Offset: 0x00010C74
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

		// Token: 0x04000134 RID: 308
		private string m_Uri = "";
	}
}
