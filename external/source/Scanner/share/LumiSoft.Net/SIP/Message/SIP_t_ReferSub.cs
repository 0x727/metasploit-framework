using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000053 RID: 83
	public class SIP_t_ReferSub : SIP_t_ValueWithParams
	{
		// Token: 0x060002B7 RID: 695 RVA: 0x0000F955 File Offset: 0x0000E955
		public SIP_t_ReferSub()
		{
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000F966 File Offset: 0x0000E966
		public SIP_t_ReferSub(string value)
		{
			this.Parse(value);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000F980 File Offset: 0x0000E980
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000F9B0 File Offset: 0x0000E9B0
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
				throw new SIP_ParseException("Refer-Sub refer-sub-value value is missing !");
			}
			try
			{
				this.m_Value = Convert.ToBoolean(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid Refer-Sub refer-sub-value value !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000FA24 File Offset: 0x0000EA24
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_Value.ToString());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002BC RID: 700 RVA: 0x0000FA64 File Offset: 0x0000EA64
		// (set) Token: 0x060002BD RID: 701 RVA: 0x0000FA7C File Offset: 0x0000EA7C
		public bool Value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		// Token: 0x04000112 RID: 274
		private bool m_Value = false;
	}
}
