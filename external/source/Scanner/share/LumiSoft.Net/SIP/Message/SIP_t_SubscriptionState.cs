using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000059 RID: 89
	public class SIP_t_SubscriptionState : SIP_t_ValueWithParams
	{
		// Token: 0x060002F1 RID: 753 RVA: 0x0001062B File Offset: 0x0000F62B
		public SIP_t_SubscriptionState(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00010648 File Offset: 0x0000F648
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00010678 File Offset: 0x0000F678
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
				throw new SIP_ParseException("SIP Event 'substate-value' value is missing !");
			}
			this.m_Value = text;
			base.ParseParameters(reader);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x000106C4 File Offset: 0x0000F6C4
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Value);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x000106FC File Offset: 0x0000F6FC
		// (set) Token: 0x060002F6 RID: 758 RVA: 0x00010714 File Offset: 0x0000F714
		public string Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Value");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property 'Value' value may not be '' !");
				}
				bool flag3 = !TextUtils.IsToken(value);
				if (flag3)
				{
					throw new ArgumentException("Property 'Value' value must be 'token' !");
				}
				this.m_Value = value;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x00010774 File Offset: 0x0000F774
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x000107AC File Offset: 0x0000F7AC
		public string Reason
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["reason"];
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
					base.Parameters.Remove("reason");
				}
				else
				{
					base.Parameters.Set("reason", value);
				}
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x000107F0 File Offset: 0x0000F7F0
		// (set) Token: 0x060002FA RID: 762 RVA: 0x0001082C File Offset: 0x0000F82C
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
				bool flag = value == -1;
				if (flag)
				{
					base.Parameters.Remove("expires");
				}
				else
				{
					bool flag2 = value < 0;
					if (flag2)
					{
						throw new ArgumentException("Property 'Expires' value must >= 0 !");
					}
					base.Parameters.Set("expires", value.ToString());
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002FB RID: 763 RVA: 0x00010888 File Offset: 0x0000F888
		// (set) Token: 0x060002FC RID: 764 RVA: 0x000108C4 File Offset: 0x0000F8C4
		public int RetryAfter
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["retry-after"];
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
					base.Parameters.Remove("retry-after");
				}
				else
				{
					bool flag2 = value < 0;
					if (flag2)
					{
						throw new ArgumentException("Property 'RetryAfter' value must >= 0 !");
					}
					base.Parameters.Set("retry-after", value.ToString());
				}
			}
		}

		// Token: 0x04000119 RID: 281
		private string m_Value = "";

		// Token: 0x02000285 RID: 645
		public class SubscriptionState
		{
			// Token: 0x0400097E RID: 2430
			public const string active = "active";

			// Token: 0x0400097F RID: 2431
			public const string pending = "pending";

			// Token: 0x04000980 RID: 2432
			public const string terminated = "terminated";
		}

		// Token: 0x02000286 RID: 646
		public class EventReason
		{
			// Token: 0x04000981 RID: 2433
			public const string deactivated = "deactivated";

			// Token: 0x04000982 RID: 2434
			public const string probation = "probation";

			// Token: 0x04000983 RID: 2435
			public const string rejected = "rejected";

			// Token: 0x04000984 RID: 2436
			public const string timeout = "timeout";

			// Token: 0x04000985 RID: 2437
			public const string giveup = "giveup";

			// Token: 0x04000986 RID: 2438
			public const string noresource = "noresource";
		}
	}
}
