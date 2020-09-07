using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004B RID: 75
	public class SIP_t_HiEntry : SIP_t_ValueWithParams
	{
		// Token: 0x06000275 RID: 629 RVA: 0x0000EB58 File Offset: 0x0000DB58
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000EB88 File Offset: 0x0000DB88
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.m_pAddress = new SIP_t_NameAddress();
			this.m_pAddress.Parse(reader);
			base.ParseParameters(reader);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000EBCC File Offset: 0x0000DBCC
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000EC0C File Offset: 0x0000DC0C
		// (set) Token: 0x06000279 RID: 633 RVA: 0x0000EC24 File Offset: 0x0000DC24
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
			set
			{
				bool flag = this.m_pAddress == null;
				if (flag)
				{
					throw new ArgumentNullException("m_pAddress");
				}
				this.m_pAddress = value;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600027A RID: 634 RVA: 0x0000EC54 File Offset: 0x0000DC54
		// (set) Token: 0x0600027B RID: 635 RVA: 0x0000EC9C File Offset: 0x0000DC9C
		public double Index
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["index"];
				bool flag = sip_Parameter != null;
				double result;
				if (flag)
				{
					result = (double)Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1.0;
				}
				return result;
			}
			set
			{
				bool flag = value == -1.0;
				if (flag)
				{
					base.Parameters.Remove("index");
				}
				else
				{
					base.Parameters.Set("index", value.ToString());
				}
			}
		}

		// Token: 0x04000109 RID: 265
		private SIP_t_NameAddress m_pAddress = null;
	}
}
