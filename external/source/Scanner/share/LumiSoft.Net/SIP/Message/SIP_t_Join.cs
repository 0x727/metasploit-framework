using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004D RID: 77
	public class SIP_t_Join : SIP_t_ValueWithParams
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0000EEC5 File Offset: 0x0000DEC5
		public SIP_t_Join(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000EEE0 File Offset: 0x0000DEE0
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000EF10 File Offset: 0x0000DF10
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			SIP_t_CallID sip_t_CallID = new SIP_t_CallID();
			sip_t_CallID.Parse(reader);
			this.m_pCallID = sip_t_CallID;
			base.ParseParameters(reader);
			bool flag2 = base.Parameters["to-tag"] == null;
			if (flag2)
			{
				throw new SIP_ParseException("Join value mandatory to-tag value is missing !");
			}
			bool flag3 = base.Parameters["from-tag"] == null;
			if (flag3)
			{
				throw new SIP_ParseException("Join value mandatory from-tag value is missing !");
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000EF98 File Offset: 0x0000DF98
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pCallID.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000EFD8 File Offset: 0x0000DFD8
		// (set) Token: 0x06000289 RID: 649 RVA: 0x0000EFF0 File Offset: 0x0000DFF0
		public SIP_t_CallID CallID
		{
			get
			{
				return this.m_pCallID;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("CallID");
				}
				this.m_pCallID = value;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000F01C File Offset: 0x0000E01C
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000F054 File Offset: 0x0000E054
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
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("ToTag is mandatory and cant be null or empty !");
				}
				base.Parameters.Set("to-tag", value);
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000F08C File Offset: 0x0000E08C
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000F0C4 File Offset: 0x0000E0C4
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
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("FromTag is mandatory and cant be null or empty !");
				}
				base.Parameters.Set("from-tag", value);
			}
		}

		// Token: 0x0400010B RID: 267
		private SIP_t_CallID m_pCallID = null;
	}
}
