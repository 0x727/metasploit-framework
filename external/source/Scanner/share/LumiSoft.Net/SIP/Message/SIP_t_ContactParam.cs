using System;
using System.Globalization;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200007C RID: 124
	public class SIP_t_ContactParam : SIP_t_ValueWithParams
	{
		// Token: 0x06000487 RID: 1159 RVA: 0x00016B31 File Offset: 0x00015B31
		public SIP_t_ContactParam()
		{
			this.m_pAddress = new SIP_t_NameAddress();
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00016B50 File Offset: 0x00015B50
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00016B80 File Offset: 0x00015B80
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			SIP_t_NameAddress sip_t_NameAddress = new SIP_t_NameAddress();
			sip_t_NameAddress.Parse(reader);
			this.m_pAddress = sip_t_NameAddress;
			base.ParseParameters(reader);
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00016BC0 File Offset: 0x00015BC0
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00016C00 File Offset: 0x00015C00
		public bool IsStarContact
		{
			get
			{
				return this.m_pAddress.Uri.Value.StartsWith("*");
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x0600048C RID: 1164 RVA: 0x00016C38 File Offset: 0x00015C38
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00016C50 File Offset: 0x00015C50
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x00016CA8 File Offset: 0x00015CA8
		public double QValue
		{
			get
			{
				bool flag = !base.Parameters.Contains("qvalue");
				double result;
				if (flag)
				{
					result = -1.0;
				}
				else
				{
					result = double.Parse(base.Parameters["qvalue"].Value, NumberStyles.Any);
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

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00016D20 File Offset: 0x00015D20
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00016D5C File Offset: 0x00015D5C
		public int Expires
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["expires"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					base.Parameters.Remove("expires");
				}
				else
				{
					base.Parameters.Set("expires", value.ToString());
				}
			}
		}

		// Token: 0x0400015C RID: 348
		private SIP_t_NameAddress m_pAddress = null;
	}
}
