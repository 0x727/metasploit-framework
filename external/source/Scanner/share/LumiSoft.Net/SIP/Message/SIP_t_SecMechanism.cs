using System;
using System.Globalization;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000057 RID: 87
	public class SIP_t_SecMechanism : SIP_t_ValueWithParams
	{
		// Token: 0x060002DB RID: 731 RVA: 0x000100BC File Offset: 0x0000F0BC
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002DC RID: 732 RVA: 0x000100EC File Offset: 0x0000F0EC
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
				throw new SIP_ParseException("Invalid 'sec-mechanism', 'mechanism-name' is missing !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00010134 File Offset: 0x0000F134
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Mechanism);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002DE RID: 734 RVA: 0x0001016C File Offset: 0x0000F16C
		// (set) Token: 0x060002DF RID: 735 RVA: 0x00010184 File Offset: 0x0000F184
		public string Mechanism
		{
			get
			{
				return this.m_Mechanism;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Mechanism");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property Mechanism value may not be '' !");
				}
				bool flag3 = !TextUtils.IsToken(value);
				if (flag3)
				{
					throw new ArgumentException("Property Mechanism value must be 'token' !");
				}
				this.m_Mechanism = value;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x000101E4 File Offset: 0x0000F1E4
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x0001023C File Offset: 0x0000F23C
		public double Q
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
				bool flag = value < 0.0 || value > 2.0;
				if (flag)
				{
					throw new ArgumentException("Property QValue value must be between 0.0 and 2.0 !");
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

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x000102B4 File Offset: 0x0000F2B4
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x000102EC File Offset: 0x0000F2EC
		public string D_Alg
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["d-alg"];
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
					base.Parameters.Remove("d-alg");
				}
				else
				{
					base.Parameters.Set("d-alg", value);
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x00010330 File Offset: 0x0000F330
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x00010368 File Offset: 0x0000F368
		public string D_Qop
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["d-qop"];
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
					base.Parameters.Remove("d-qop");
				}
				else
				{
					base.Parameters.Set("d-qop", value);
				}
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000103AC File Offset: 0x0000F3AC
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x000103E4 File Offset: 0x0000F3E4
		public string D_Ver
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["d-ver"];
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
					base.Parameters.Remove("d-ver");
				}
				else
				{
					base.Parameters.Set("d-ver", value);
				}
			}
		}

		// Token: 0x04000117 RID: 279
		private string m_Mechanism = "";
	}
}
