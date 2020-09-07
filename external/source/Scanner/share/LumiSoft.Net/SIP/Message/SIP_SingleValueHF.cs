using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000072 RID: 114
	public class SIP_SingleValueHF<T> : SIP_HeaderField where T : SIP_t_Value
	{
		// Token: 0x060003B5 RID: 949 RVA: 0x00013281 File Offset: 0x00012281
		public SIP_SingleValueHF(string name, T value) : base(name, "")
		{
			this.m_pValue = value;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000132A4 File Offset: 0x000122A4
		public void Parse(StringReader reader)
		{
			this.m_pValue.Parse(reader);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000132BC File Offset: 0x000122BC
		public string ToStringValue()
		{
			return this.m_pValue.ToStringValue();
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x000132E0 File Offset: 0x000122E0
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x000132F8 File Offset: 0x000122F8
		public override string Value
		{
			get
			{
				return this.ToStringValue();
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Property Value value may not be null !");
				}
				this.Parse(new StringReader(value));
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00013328 File Offset: 0x00012328
		// (set) Token: 0x060003BB RID: 955 RVA: 0x00013340 File Offset: 0x00012340
		public T ValueX
		{
			get
			{
				return this.m_pValue;
			}
			set
			{
				this.m_pValue = value;
			}
		}

		// Token: 0x0400014B RID: 331
		private T m_pValue = default(T);
	}
}
