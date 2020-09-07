using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000074 RID: 116
	public class SIP_t_Timestamp : SIP_t_Value
	{
		// Token: 0x060003C2 RID: 962 RVA: 0x000134C8 File Offset: 0x000124C8
		public SIP_t_Timestamp(string value)
		{
			this.Parse(new StringReader(value));
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x000134F7 File Offset: 0x000124F7
		public SIP_t_Timestamp(decimal time, decimal delay)
		{
			this.m_Time = time;
			this.m_Delay = delay;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00013528 File Offset: 0x00012528
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00013558 File Offset: 0x00012558
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
				throw new SIP_ParseException("Invalid 'Timestamp' value, time is missing !");
			}
			this.m_Time = Convert.ToDecimal(text);
			text = reader.ReadWord();
			bool flag3 = text != null;
			if (flag3)
			{
				this.m_Delay = Convert.ToDecimal(text);
			}
			else
			{
				this.m_Delay = 0m;
			}
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x000135D0 File Offset: 0x000125D0
		public override string ToStringValue()
		{
			bool flag = this.m_Delay > 0m;
			string result;
			if (flag)
			{
				result = this.m_Time.ToString() + " " + this.m_Delay.ToString();
			}
			else
			{
				result = this.m_Time.ToString();
			}
			return result;
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060003C7 RID: 967 RVA: 0x00013628 File Offset: 0x00012628
		// (set) Token: 0x060003C8 RID: 968 RVA: 0x00013640 File Offset: 0x00012640
		public decimal Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x0001364C File Offset: 0x0001264C
		// (set) Token: 0x060003CA RID: 970 RVA: 0x00013664 File Offset: 0x00012664
		public decimal Delay
		{
			get
			{
				return this.m_Delay;
			}
			set
			{
				this.m_Delay = value;
			}
		}

		// Token: 0x0400014D RID: 333
		private decimal m_Time = 0m;

		// Token: 0x0400014E RID: 334
		private decimal m_Delay = 0m;
	}
}
