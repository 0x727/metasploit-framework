using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000055 RID: 85
	public class SIP_t_RetryAfter : SIP_t_ValueWithParams
	{
		// Token: 0x060002CA RID: 714 RVA: 0x0000FCF3 File Offset: 0x0000ECF3
		public SIP_t_RetryAfter(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000FD0C File Offset: 0x0000ED0C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000FD3C File Offset: 0x0000ED3C
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
				throw new SIP_ParseException("SIP Retry-After 'delta-seconds' value is missing !");
			}
			try
			{
				this.m_Time = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid SIP Retry-After 'delta-seconds' value !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000FDB0 File Offset: 0x0000EDB0
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Time);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000FDE8 File Offset: 0x0000EDE8
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000FE00 File Offset: 0x0000EE00
		public int Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("Property Time value must be >= 1 !");
				}
				this.m_Time = value;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000FE2C File Offset: 0x0000EE2C
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0000FE68 File Offset: 0x0000EE68
		public int Duration
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["duration"];
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
				bool flag = value == -1;
				if (flag)
				{
					base.Parameters.Remove("duration");
				}
				else
				{
					bool flag2 = value < 1;
					if (flag2)
					{
						throw new ArgumentException("Property Duration value must be >= 1 !");
					}
					base.Parameters.Set("duration", value.ToString());
				}
			}
		}

		// Token: 0x04000114 RID: 276
		private int m_Time = 0;
	}
}
