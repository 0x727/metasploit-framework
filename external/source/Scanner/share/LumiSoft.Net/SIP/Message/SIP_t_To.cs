using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006F RID: 111
	public class SIP_t_To : SIP_t_ValueWithParams
	{
		// Token: 0x0600039A RID: 922 RVA: 0x00012CD3 File Offset: 0x00011CD3
		public SIP_t_To(string value)
		{
			this.m_pAddress = new SIP_t_NameAddress();
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00012CFC File Offset: 0x00011CFC
		public SIP_t_To(SIP_t_NameAddress address)
		{
			this.m_pAddress = address;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00012D14 File Offset: 0x00011D14
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00012D44 File Offset: 0x00011D44
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

		// Token: 0x0600039E RID: 926 RVA: 0x00012D7C File Offset: 0x00011D7C
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600039F RID: 927 RVA: 0x00012DBC File Offset: 0x00011DBC
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00012DD4 File Offset: 0x00011DD4
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00012E0C File Offset: 0x00011E0C
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

		// Token: 0x04000146 RID: 326
		private SIP_t_NameAddress m_pAddress = null;
	}
}
