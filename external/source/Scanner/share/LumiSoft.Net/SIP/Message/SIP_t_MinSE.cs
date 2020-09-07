using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004E RID: 78
	public class SIP_t_MinSE : SIP_t_ValueWithParams
	{
		// Token: 0x0600028E RID: 654 RVA: 0x0000F0FC File Offset: 0x0000E0FC
		public SIP_t_MinSE(string value)
		{
			this.Parse(value);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000F116 File Offset: 0x0000E116
		public SIP_t_MinSE(int minExpires)
		{
			this.m_Time = minExpires;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000F130 File Offset: 0x0000E130
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000F160 File Offset: 0x0000E160
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
				throw new SIP_ParseException("Min-SE delta-seconds value is missing !");
			}
			try
			{
				this.m_Time = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid Min-SE delta-seconds value !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000F1D4 File Offset: 0x0000E1D4
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Time.ToString());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000F214 File Offset: 0x0000E214
		// (set) Token: 0x06000294 RID: 660 RVA: 0x0000F22C File Offset: 0x0000E22C
		public int Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				bool flag = this.m_Time < 1;
				if (flag)
				{
					throw new ArgumentException("Time value must be > 0 !");
				}
				this.m_Time = value;
			}
		}

		// Token: 0x0400010C RID: 268
		private int m_Time = 90;
	}
}
