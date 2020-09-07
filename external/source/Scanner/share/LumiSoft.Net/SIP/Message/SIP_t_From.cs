using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006C RID: 108
	public class SIP_t_From : SIP_t_ValueWithParams
	{
		// Token: 0x06000385 RID: 901 RVA: 0x0001294B File Offset: 0x0001194B
		public SIP_t_From(string value)
		{
			this.m_pAddress = new SIP_t_NameAddress();
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00012974 File Offset: 0x00011974
		public SIP_t_From(SIP_t_NameAddress address)
		{
			this.m_pAddress = address;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0001298C File Offset: 0x0001198C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000388 RID: 904 RVA: 0x000129BC File Offset: 0x000119BC
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.m_pAddress.Parse(reader);
			base.ParseParameters(reader);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x000129F4 File Offset: 0x000119F4
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00012A34 File Offset: 0x00011A34
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600038B RID: 907 RVA: 0x00012A4C File Offset: 0x00011A4C
		// (set) Token: 0x0600038C RID: 908 RVA: 0x00012A84 File Offset: 0x00011A84
		public string Tag
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["tag"];
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
					base.Parameters.Remove("tag");
				}
				else
				{
					base.Parameters.Set("tag", value);
				}
			}
		}

		// Token: 0x04000143 RID: 323
		private SIP_t_NameAddress m_pAddress = null;
	}
}
