using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006E RID: 110
	public class SIP_t_OptionTag : SIP_t_Value
	{
		// Token: 0x06000395 RID: 917 RVA: 0x00012C04 File Offset: 0x00011C04
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00012C34 File Offset: 0x00011C34
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
				throw new ArgumentException("Invalid 'option-tag' value, value is missing !");
			}
			this.m_OptionTag = text;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00012C78 File Offset: 0x00011C78
		public override string ToStringValue()
		{
			return this.m_OptionTag;
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00012C90 File Offset: 0x00011C90
		// (set) Token: 0x06000399 RID: 921 RVA: 0x00012CA8 File Offset: 0x00011CA8
		public string OptionTag
		{
			get
			{
				return this.m_OptionTag;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("property OptionTag value cant be null or empty !");
				}
				this.m_OptionTag = value;
			}
		}

		// Token: 0x04000145 RID: 325
		private string m_OptionTag = "";
	}
}
