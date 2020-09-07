using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000064 RID: 100
	public class SIP_t_Method : SIP_t_Value
	{
		// Token: 0x06000349 RID: 841 RVA: 0x00011CB4 File Offset: 0x00010CB4
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00011CE4 File Offset: 0x00010CE4
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
				throw new SIP_ParseException("Invalid 'Method' value, value is missing !");
			}
			this.m_Method = text;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00011D28 File Offset: 0x00010D28
		public override string ToStringValue()
		{
			return this.m_Method;
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600034C RID: 844 RVA: 0x00011D40 File Offset: 0x00010D40
		// (set) Token: 0x0600034D RID: 845 RVA: 0x00011D58 File Offset: 0x00010D58
		public string Method
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Method value can't be null or empty !");
				}
				bool flag2 = TextUtils.IsToken(value);
				if (flag2)
				{
					throw new ArgumentException("Property Method value must be 'token' !");
				}
				this.m_Method = value;
			}
		}

		// Token: 0x04000135 RID: 309
		private string m_Method = "";
	}
}
