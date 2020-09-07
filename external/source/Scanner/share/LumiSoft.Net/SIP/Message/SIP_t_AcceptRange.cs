using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000062 RID: 98
	public class SIP_t_AcceptRange : SIP_t_Value
	{
		// Token: 0x06000338 RID: 824 RVA: 0x000116E8 File Offset: 0x000106E8
		public SIP_t_AcceptRange()
		{
			this.m_pMediaParameters = new SIP_ParameterCollection();
			this.m_pParameters = new SIP_ParameterCollection();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00011724 File Offset: 0x00010724
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00011754 File Offset: 0x00010754
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
				throw new SIP_ParseException("Invalid 'accept-range' value, m-type is missing !");
			}
			this.MediaType = text;
			bool flag3 = true;
			while (reader.Available > 0L)
			{
				reader.ReadToFirstChar();
				bool flag4 = reader.SourceString.StartsWith(",");
				if (flag4)
				{
					break;
				}
				bool flag5 = reader.SourceString.StartsWith(";");
				if (!flag5)
				{
					throw new SIP_ParseException("SIP_t_AcceptRange unexpected prarameter value !");
				}
				reader.ReadSpecifiedLength(1);
				string text2 = reader.QuotedReadToDelimiter(new char[]
				{
					';',
					','
				}, false);
				bool flag6 = text2 != "";
				if (flag6)
				{
					string[] array = text2.Split(new char[]
					{
						'='
					}, 2);
					string text3 = array[0].Trim();
					string value = "";
					bool flag7 = array.Length == 2;
					if (flag7)
					{
						value = array[1];
					}
					bool flag8 = text3.ToLower() == "q";
					if (flag8)
					{
						flag3 = false;
					}
					bool flag9 = flag3;
					if (flag9)
					{
						this.MediaParameters.Add(text3, value);
					}
					else
					{
						this.Parameters.Add(text3, value);
					}
				}
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000118C0 File Offset: 0x000108C0
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_MediaType);
			foreach (object obj in this.m_pMediaParameters)
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
			foreach (object obj2 in this.m_pParameters)
			{
				SIP_Parameter sip_Parameter2 = (SIP_Parameter)obj2;
				bool flag2 = sip_Parameter2.Value != null;
				if (flag2)
				{
					stringBuilder.Append(";" + sip_Parameter2.Name + "=" + sip_Parameter2.Value);
				}
				else
				{
					stringBuilder.Append(";" + sip_Parameter2.Name);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00011A24 File Offset: 0x00010A24
		// (set) Token: 0x0600033D RID: 829 RVA: 0x00011A3C File Offset: 0x00010A3C
		public string MediaType
		{
			get
			{
				return this.m_MediaType;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property MediaType value can't be null or empty !");
				}
				bool flag2 = value.IndexOf('/') == -1;
				if (flag2)
				{
					throw new ArgumentException("Invalid roperty MediaType value, syntax: mediaType / mediaSubType !");
				}
				this.m_MediaType = value;
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00011A84 File Offset: 0x00010A84
		public SIP_ParameterCollection MediaParameters
		{
			get
			{
				return this.m_pMediaParameters;
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600033F RID: 831 RVA: 0x00011A9C File Offset: 0x00010A9C
		public SIP_ParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000340 RID: 832 RVA: 0x00011AB4 File Offset: 0x00010AB4
		// (set) Token: 0x06000341 RID: 833 RVA: 0x00011AF8 File Offset: 0x00010AF8
		public double QValue
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["qvalue"];
				bool flag = sip_Parameter != null;
				double result;
				if (flag)
				{
					result = Convert.ToDouble(sip_Parameter.Value);
				}
				else
				{
					result = -1.0;
				}
				return result;
			}
			set
			{
				bool flag = value < 0.0 || value > 1.0;
				if (flag)
				{
					throw new ArgumentException("Property QValue value must be between 0.0 and 1.0 !");
				}
				bool flag2 = value < 0.0;
				if (flag2)
				{
					this.Parameters.Remove("qvalue");
				}
				else
				{
					this.Parameters.Set("qvalue", value.ToString());
				}
			}
		}

		// Token: 0x04000131 RID: 305
		private string m_MediaType = "";

		// Token: 0x04000132 RID: 306
		private SIP_ParameterCollection m_pMediaParameters = null;

		// Token: 0x04000133 RID: 307
		private SIP_ParameterCollection m_pParameters = null;
	}
}
