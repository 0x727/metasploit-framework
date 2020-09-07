using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000076 RID: 118
	public abstract class SIP_t_ValueWithParams : SIP_t_Value
	{
		// Token: 0x060003CE RID: 974 RVA: 0x0001366E File Offset: 0x0001266E
		public SIP_t_ValueWithParams()
		{
			this.m_pParameters = new SIP_ParameterCollection();
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001368C File Offset: 0x0001268C
		protected void ParseParameters(StringReader reader)
		{
			this.m_pParameters.Clear();
			while (reader.Available > 0L)
			{
				reader.ReadToFirstChar();
				bool flag = reader.SourceString.StartsWith(";");
				if (flag)
				{
					reader.ReadSpecifiedLength(1);
					string text = reader.QuotedReadToDelimiter(new char[]
					{
						';',
						','
					}, false);
					bool flag2 = text != "";
					if (flag2)
					{
						string[] array = text.Split(new char[]
						{
							'='
						}, 2);
						bool flag3 = array.Length == 2;
						if (flag3)
						{
							this.Parameters.Add(array[0], TextUtils.UnQuoteString(array[1]));
						}
						else
						{
							this.Parameters.Add(array[0], null);
						}
					}
				}
				else
				{
					bool flag4 = reader.SourceString.StartsWith(",");
					if (flag4)
					{
						break;
					}
					throw new SIP_ParseException("Unexpected value '" + reader.SourceString + "' !");
				}
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00013794 File Offset: 0x00012794
		protected string ParametersToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this.m_pParameters)
			{
				SIP_Parameter sip_Parameter = (SIP_Parameter)obj;
				bool flag = !string.IsNullOrEmpty(sip_Parameter.Value);
				if (flag)
				{
					bool flag2 = TextUtils.IsToken(sip_Parameter.Value);
					if (flag2)
					{
						stringBuilder.Append(";" + sip_Parameter.Name + "=" + sip_Parameter.Value);
					}
					else
					{
						stringBuilder.Append(";" + sip_Parameter.Name + "=" + TextUtils.QuoteString(sip_Parameter.Value));
					}
				}
				else
				{
					stringBuilder.Append(";" + sip_Parameter.Name);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00013898 File Offset: 0x00012898
		public SIP_ParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x0400014F RID: 335
		private SIP_ParameterCollection m_pParameters = null;
	}
}
