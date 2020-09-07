using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000052 RID: 82
	public class SIP_t_ReferredBy : SIP_t_ValueWithParams
	{
		// Token: 0x060002AF RID: 687 RVA: 0x0000F7CB File Offset: 0x0000E7CB
		public SIP_t_ReferredBy(string value)
		{
			this.m_pAddress = new SIP_t_NameAddress();
			this.Parse(value);
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000F7F0 File Offset: 0x0000E7F0
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000F820 File Offset: 0x0000E820
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.m_pAddress.Parse(reader);
			base.ParseParameters(reader);
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000F858 File Offset: 0x0000E858
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000F898 File Offset: 0x0000E898
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x0000F8B0 File Offset: 0x0000E8B0
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Address");
				}
				this.m_pAddress = value;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000F8DC File Offset: 0x0000E8DC
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x0000F914 File Offset: 0x0000E914
		public string CID
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["cid"];
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
					base.Parameters.Remove("cid");
				}
				else
				{
					base.Parameters.Set("cid", value);
				}
			}
		}

		// Token: 0x04000111 RID: 273
		private SIP_t_NameAddress m_pAddress = null;
	}
}
