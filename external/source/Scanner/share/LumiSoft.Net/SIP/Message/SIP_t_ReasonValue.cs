using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000051 RID: 81
	public class SIP_t_ReasonValue : SIP_t_ValueWithParams
	{
		// Token: 0x060002A4 RID: 676 RVA: 0x0000F593 File Offset: 0x0000E593
		public SIP_t_ReasonValue()
		{
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000F5A8 File Offset: 0x0000E5A8
		public SIP_t_ReasonValue(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000F5C8 File Offset: 0x0000E5C8
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000F5F8 File Offset: 0x0000E5F8
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
				throw new SIP_ParseException("SIP reason-value 'protocol' value is missing !");
			}
			this.m_Protocol = text;
			base.ParseParameters(reader);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000F644 File Offset: 0x0000E644
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Protocol);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000F67C File Offset: 0x0000E67C
		// (set) Token: 0x060002AA RID: 682 RVA: 0x0000F694 File Offset: 0x0000E694
		public string Protocol
		{
			get
			{
				return this.m_Protocol;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Protocol");
				}
				this.m_Protocol = value;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000F6C0 File Offset: 0x0000E6C0
		// (set) Token: 0x060002AC RID: 684 RVA: 0x0000F70C File Offset: 0x0000E70C
		public int Cause
		{
			get
			{
				bool flag = base.Parameters["cause"] == null;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					result = Convert.ToInt32(base.Parameters["cause"].Value);
				}
				return result;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					base.Parameters.Remove("cause");
				}
				else
				{
					base.Parameters.Set("cause", value.ToString());
				}
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000F754 File Offset: 0x0000E754
		// (set) Token: 0x060002AE RID: 686 RVA: 0x0000F78C File Offset: 0x0000E78C
		public string Text
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["text"];
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
				bool flag = value == null;
				if (flag)
				{
					base.Parameters.Remove("text");
				}
				else
				{
					base.Parameters.Set("text", value);
				}
			}
		}

		// Token: 0x04000110 RID: 272
		private string m_Protocol = "";
	}
}
