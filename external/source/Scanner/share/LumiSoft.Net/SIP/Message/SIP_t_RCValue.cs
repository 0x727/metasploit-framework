using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000050 RID: 80
	public class SIP_t_RCValue : SIP_t_ValueWithParams
	{
		// Token: 0x060002A1 RID: 673 RVA: 0x0000F4E4 File Offset: 0x0000E4E4
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000F514 File Offset: 0x0000E514
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
				throw new SIP_ParseException("Invalid 'rc-value', '*' is missing !");
			}
			base.ParseParameters(reader);
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000F55C File Offset: 0x0000E55C
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("*");
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}
	}
}
