using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000068 RID: 104
	public class SIP_t_LanguageTag : SIP_t_ValueWithParams
	{
		// Token: 0x06000369 RID: 873 RVA: 0x000123EC File Offset: 0x000113EC
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0001241C File Offset: 0x0001141C
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
				throw new SIP_ParseException("Invalid Content-Language value, language-tag value is missing !");
			}
			this.m_LanguageTag = text;
			base.ParseParameters(reader);
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00012468 File Offset: 0x00011468
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_LanguageTag);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600036C RID: 876 RVA: 0x000124A0 File Offset: 0x000114A0
		// (set) Token: 0x0600036D RID: 877 RVA: 0x000124B8 File Offset: 0x000114B8
		public string LanguageTag
		{
			get
			{
				return this.m_LanguageTag;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property LanguageTag value can't be null or empty !");
				}
				this.m_LanguageTag = value;
			}
		}

		// Token: 0x0400013D RID: 317
		private string m_LanguageTag = "";
	}
}
