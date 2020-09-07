using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000070 RID: 112
	public class SIP_t_WarningValue : SIP_t_Value
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x00012E74 File Offset: 0x00011E74
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00012EA4 File Offset: 0x00011EA4
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
				throw new SIP_ParseException("Invalid 'warning-value' value, warn-code is missing !");
			}
			try
			{
				this.Code = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid 'warning-value' warn-code value, warn-code is missing !");
			}
			text = reader.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("Invalid 'warning-value' value, warn-agent is missing !");
			}
			this.Agent = text;
			text = reader.ReadToEnd();
			bool flag4 = text == null;
			if (flag4)
			{
				throw new SIP_ParseException("Invalid 'warning-value' value, warn-text is missing !");
			}
			this.Text = TextUtils.UnQuoteString(text);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00012F60 File Offset: 0x00011F60
		public override string ToStringValue()
		{
			return string.Concat(new object[]
			{
				this.m_Code,
				" ",
				this.m_Agent,
				" ",
				TextUtils.QuoteString(this.m_Text)
			});
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00012FB4 File Offset: 0x00011FB4
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00012FCC File Offset: 0x00011FCC
		public int Code
		{
			get
			{
				return this.m_Code;
			}
			set
			{
				bool flag = value < 100 || value > 999;
				if (flag)
				{
					throw new ArgumentException("Property Code value must be 3 digit !");
				}
				this.m_Code = value;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00013004 File Offset: 0x00012004
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0001301C File Offset: 0x0001201C
		public string Agent
		{
			get
			{
				return this.m_Agent;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Agent value may not be null or empty !");
				}
				this.m_Agent = value;
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00013048 File Offset: 0x00012048
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00013060 File Offset: 0x00012060
		public string Text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Text value may not be null or empty !");
				}
				this.m_Text = value;
			}
		}

		// Token: 0x04000147 RID: 327
		private int m_Code = 0;

		// Token: 0x04000148 RID: 328
		private string m_Agent = "";

		// Token: 0x04000149 RID: 329
		private string m_Text = "";
	}
}
