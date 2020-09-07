using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000058 RID: 88
	public class SIP_t_SessionExpires : SIP_t_ValueWithParams
	{
		// Token: 0x060002E8 RID: 744 RVA: 0x00010425 File Offset: 0x0000F425
		public SIP_t_SessionExpires(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00010440 File Offset: 0x0000F440
		public SIP_t_SessionExpires(int expires, string refresher)
		{
			bool flag = this.m_Expires < 90;
			if (flag)
			{
				throw new ArgumentException("Argument 'expires' value must be >= 90 !");
			}
			this.m_Expires = expires;
			this.Refresher = refresher;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00010488 File Offset: 0x0000F488
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002EB RID: 747 RVA: 0x000104B8 File Offset: 0x0000F4B8
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
				throw new SIP_ParseException("Session-Expires delta-seconds value is missing !");
			}
			try
			{
				this.m_Expires = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid Session-Expires delta-seconds value !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0001052C File Offset: 0x0000F52C
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Expires.ToString());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0001056C File Offset: 0x0000F56C
		// (set) Token: 0x060002EE RID: 750 RVA: 0x00010584 File Offset: 0x0000F584
		public int Expires
		{
			get
			{
				return this.m_Expires;
			}
			set
			{
				bool flag = this.m_Expires < 90;
				if (flag)
				{
					throw new ArgumentException("Property Expires value must be >= 90 !");
				}
				this.m_Expires = value;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002EF RID: 751 RVA: 0x000105B4 File Offset: 0x0000F5B4
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x000105EC File Offset: 0x0000F5EC
		public string Refresher
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["refresher"];
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
					base.Parameters.Remove("refresher");
				}
				else
				{
					base.Parameters.Set("refresher", value);
				}
			}
		}

		// Token: 0x04000118 RID: 280
		private int m_Expires = 90;
	}
}
