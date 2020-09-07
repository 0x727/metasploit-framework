using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200005A RID: 90
	public class SIP_t_TargetDialog : SIP_t_ValueWithParams
	{
		// Token: 0x060002FD RID: 765 RVA: 0x0001091D File Offset: 0x0000F91D
		public SIP_t_TargetDialog(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0001093C File Offset: 0x0000F93C
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0001096C File Offset: 0x0000F96C
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
				throw new SIP_ParseException("SIP Target-Dialog 'callid' value is missing !");
			}
			this.m_CallID = text;
			base.ParseParameters(reader);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x000109B8 File Offset: 0x0000F9B8
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_CallID);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000301 RID: 769 RVA: 0x000109F0 File Offset: 0x0000F9F0
		// (set) Token: 0x06000302 RID: 770 RVA: 0x00010A08 File Offset: 0x0000FA08
		public string CallID
		{
			get
			{
				return this.m_CallID;
			}
			set
			{
				bool flag = this.m_CallID == null;
				if (flag)
				{
					throw new ArgumentNullException("CallID");
				}
				bool flag2 = this.m_CallID == "";
				if (flag2)
				{
					throw new ArgumentException("Property 'CallID' may not be '' !");
				}
				this.m_CallID = value;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000303 RID: 771 RVA: 0x00010A58 File Offset: 0x0000FA58
		// (set) Token: 0x06000304 RID: 772 RVA: 0x00010A90 File Offset: 0x0000FA90
		public string RemoteTag
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["remote-tag"];
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
					base.Parameters.Remove("remote-tag");
				}
				else
				{
					base.Parameters.Set("remote-tag", value);
				}
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000305 RID: 773 RVA: 0x00010AD4 File Offset: 0x0000FAD4
		// (set) Token: 0x06000306 RID: 774 RVA: 0x00010B0C File Offset: 0x0000FB0C
		public string LocalTag
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["local-tag"];
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
					base.Parameters.Remove("local-tag");
				}
				else
				{
					base.Parameters.Set("local-tag", value);
				}
			}
		}

		// Token: 0x0400011A RID: 282
		private string m_CallID = "";
	}
}
