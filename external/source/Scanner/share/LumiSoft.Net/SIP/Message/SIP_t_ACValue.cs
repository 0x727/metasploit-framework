using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000046 RID: 70
	public class SIP_t_ACValue : SIP_t_ValueWithParams
	{
		// Token: 0x0600024F RID: 591 RVA: 0x0000E1BC File Offset: 0x0000D1BC
		public SIP_t_ACValue()
		{
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000E1C6 File Offset: 0x0000D1C6
		public SIP_t_ACValue(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000E1D8 File Offset: 0x0000D1D8
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000E208 File Offset: 0x0000D208
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
				throw new SIP_ParseException("Invalid 'ac-value', '*' is missing !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000E250 File Offset: 0x0000D250
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("*");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000254 RID: 596 RVA: 0x0000E288 File Offset: 0x0000D288
		// (set) Token: 0x06000255 RID: 597 RVA: 0x0000E2BC File Offset: 0x0000D2BC
		public bool Require
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["require"];
				return sip_Parameter != null;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					base.Parameters.Remove("require");
				}
				else
				{
					base.Parameters.Set("require", null);
				}
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000E2FC File Offset: 0x0000D2FC
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000E330 File Offset: 0x0000D330
		public bool Explicit
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["explicit"];
				return sip_Parameter != null;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					base.Parameters.Remove("explicit");
				}
				else
				{
					base.Parameters.Set("explicit", null);
				}
			}
		}
	}
}
