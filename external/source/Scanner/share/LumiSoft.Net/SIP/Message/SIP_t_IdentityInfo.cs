using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004C RID: 76
	public class SIP_t_IdentityInfo : SIP_t_ValueWithParams
	{
		// Token: 0x0600027C RID: 636 RVA: 0x0000ECE9 File Offset: 0x0000DCE9
		public SIP_t_IdentityInfo(string value)
		{
			this.Parse(value);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000ED08 File Offset: 0x0000DD08
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000ED38 File Offset: 0x0000DD38
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			try
			{
				string text = reader.ReadParenthesized();
				bool flag2 = text == null;
				if (flag2)
				{
					throw new SIP_ParseException("Invalid Identity-Info 'absoluteURI' value !");
				}
				this.m_Uri = text;
			}
			catch
			{
				throw new SIP_ParseException("Invalid Identity-Info 'absoluteURI' value !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000EDA8 File Offset: 0x0000DDA8
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<" + this.m_Uri + ">");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000EDF0 File Offset: 0x0000DDF0
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0000EE08 File Offset: 0x0000DE08
		public string Uri
		{
			get
			{
				return this.m_Uri;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Uri");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Invalid Identity-Info 'absoluteURI' value !");
				}
				this.m_Uri = value;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0000EE4C File Offset: 0x0000DE4C
		// (set) Token: 0x06000283 RID: 643 RVA: 0x0000EE84 File Offset: 0x0000DE84
		public string Alg
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["alg"];
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
					base.Parameters.Remove("alg");
				}
				else
				{
					base.Parameters.Set("alg", value);
				}
			}
		}

		// Token: 0x0400010A RID: 266
		private string m_Uri = "";
	}
}
