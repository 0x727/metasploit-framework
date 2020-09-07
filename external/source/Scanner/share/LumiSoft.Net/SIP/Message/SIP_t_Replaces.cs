using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000054 RID: 84
	public class SIP_t_Replaces : SIP_t_ValueWithParams
	{
		// Token: 0x060002BF RID: 703 RVA: 0x0000FA9C File Offset: 0x0000EA9C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000FACC File Offset: 0x0000EACC
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
				throw new SIP_ParseException("Replaces 'callid' value is missing !");
			}
			this.m_CallID = text;
			base.ParseParameters(reader);
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000FB18 File Offset: 0x0000EB18
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_CallID);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x0000FB50 File Offset: 0x0000EB50
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x0000FB68 File Offset: 0x0000EB68
		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("CallID");
				}
				this.m_CallID = value;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000FB94 File Offset: 0x0000EB94
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x0000FBCC File Offset: 0x0000EBCC
		public string ToTag
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["to-tag"];
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
					base.Parameters.Remove("to-tag");
				}
				else
				{
					base.Parameters.Set("to-tag", value);
				}
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000FC0C File Offset: 0x0000EC0C
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x0000FC44 File Offset: 0x0000EC44
		public string FromTag
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["from-tag"];
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
					base.Parameters.Remove("from-tag");
				}
				else
				{
					base.Parameters.Set("from-tag", value);
				}
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000FC84 File Offset: 0x0000EC84
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000FCB4 File Offset: 0x0000ECB4
		public bool EarlyFlag
		{
			get
			{
				return base.Parameters.Contains("early-only");
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					base.Parameters.Remove("early-only");
				}
				else
				{
					base.Parameters.Set("early-only", null);
				}
			}
		}

		// Token: 0x04000113 RID: 275
		private string m_CallID = "";
	}
}
