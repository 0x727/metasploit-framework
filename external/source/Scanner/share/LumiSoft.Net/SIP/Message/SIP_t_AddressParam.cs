using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000073 RID: 115
	public class SIP_t_AddressParam : SIP_t_ValueWithParams
	{
		// Token: 0x060003BC RID: 956 RVA: 0x0001334A File Offset: 0x0001234A
		public SIP_t_AddressParam()
		{
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0001335B File Offset: 0x0001235B
		public SIP_t_AddressParam(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00013374 File Offset: 0x00012374
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000133A4 File Offset: 0x000123A4
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			SIP_t_NameAddress sip_t_NameAddress = new SIP_t_NameAddress();
			sip_t_NameAddress.Parse(reader);
			this.m_pAddress = sip_t_NameAddress;
			base.ParseParameters(reader);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x000133E4 File Offset: 0x000123E4
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_pAddress.ToStringValue());
			foreach (object obj in base.Parameters)
			{
				SIP_Parameter sip_Parameter = (SIP_Parameter)obj;
				bool flag = sip_Parameter.Value != null;
				if (flag)
				{
					stringBuilder.Append(";" + sip_Parameter.Name + "=" + sip_Parameter.Value);
				}
				else
				{
					stringBuilder.Append(";" + sip_Parameter.Name);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x000134B0 File Offset: 0x000124B0
		public SIP_t_NameAddress Address
		{
			get
			{
				return this.m_pAddress;
			}
		}

		// Token: 0x0400014C RID: 332
		private SIP_t_NameAddress m_pAddress = null;
	}
}
