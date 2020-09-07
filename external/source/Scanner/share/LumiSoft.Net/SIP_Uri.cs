using System;
using System.Text;
using LumiSoft.Net.MIME;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net
{
	// Token: 0x02000020 RID: 32
	public class SIP_Uri : AbsoluteUri
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x000060DC File Offset: 0x000050DC
		public SIP_Uri()
		{
			this.m_pParameters = new SIP_ParameterCollection();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000612C File Offset: 0x0000512C
		public new static SIP_Uri Parse(string value)
		{
			AbsoluteUri absoluteUri = AbsoluteUri.Parse(value);
			bool flag = absoluteUri is SIP_Uri;
			if (flag)
			{
				return (SIP_Uri)absoluteUri;
			}
			throw new ArgumentException("Argument 'value' is not valid SIP or SIPS URI.");
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006168 File Offset: 0x00005168
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !(obj is SIP_Uri);
				if (flag2)
				{
					result = false;
				}
				else
				{
					SIP_Uri sip_Uri = (SIP_Uri)obj;
					bool flag3 = this.IsSecure && !sip_Uri.IsSecure;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.User != sip_Uri.User;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool flag5 = this.Host.ToLower() != sip_Uri.Host.ToLower();
							if (flag5)
							{
								result = false;
							}
							else
							{
								bool flag6 = this.Port != sip_Uri.Port;
								result = !flag6;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006228 File Offset: 0x00005228
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006240 File Offset: 0x00005240
		protected override void ParseInternal(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			value = Uri.UnescapeDataString(value);
			bool flag2 = !value.ToLower().StartsWith("sip:") && !value.ToLower().StartsWith("sips:");
			if (flag2)
			{
				throw new SIP_ParseException("Specified value is invalid SIP-URI !");
			}
			StringReader stringReader = new StringReader(value);
			this.IsSecure = (stringReader.QuotedReadToDelimiter(':').ToLower() == "sips");
			bool flag3 = stringReader.SourceString.IndexOf('@') > -1;
			if (flag3)
			{
				this.User = stringReader.QuotedReadToDelimiter('@');
			}
			string[] array = stringReader.QuotedReadToDelimiter(new char[]
			{
				';',
				'?'
			}, false).Split(new char[]
			{
				':'
			});
			this.Host = array[0];
			bool flag4 = array.Length == 2;
			if (flag4)
			{
				this.Port = Convert.ToInt32(array[1]);
			}
			bool flag5 = stringReader.Available > 0L;
			if (flag5)
			{
				string[] array2 = TextUtils.SplitQuotedString(stringReader.QuotedReadToDelimiter('?'), ';');
				foreach (string text in array2)
				{
					bool flag6 = text.Trim() != "";
					if (flag6)
					{
						string[] array4 = text.Trim().Split(new char[]
						{
							'='
						}, 2);
						bool flag7 = array4.Length == 2;
						if (flag7)
						{
							this.Parameters.Add(array4[0], TextUtils.UnQuoteString(array4[1]));
						}
						else
						{
							this.Parameters.Add(array4[0], null);
						}
					}
				}
				bool flag8 = stringReader.Available > 0L;
				if (flag8)
				{
					this.m_Header = stringReader.ReadToEnd();
				}
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00006418 File Offset: 0x00005418
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool isSecure = this.IsSecure;
			if (isSecure)
			{
				stringBuilder.Append("sips:");
			}
			else
			{
				stringBuilder.Append("sip:");
			}
			bool flag = this.User != null;
			if (flag)
			{
				stringBuilder.Append(this.User + "@");
			}
			stringBuilder.Append(this.Host);
			bool flag2 = this.Port > -1;
			if (flag2)
			{
				stringBuilder.Append(":" + this.Port.ToString());
			}
			foreach (object obj in this.m_pParameters)
			{
				SIP_Parameter sip_Parameter = (SIP_Parameter)obj;
				bool flag3 = sip_Parameter.Value != null;
				if (flag3)
				{
					bool flag4 = MIME_Reader.IsToken(sip_Parameter.Value);
					if (flag4)
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
			bool flag5 = this.Header != null;
			if (flag5)
			{
				stringBuilder.Append("?" + this.Header);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000BC RID: 188 RVA: 0x000065D4 File Offset: 0x000055D4
		public override string Scheme
		{
			get
			{
				bool isSecure = this.IsSecure;
				string result;
				if (isSecure)
				{
					result = "sips";
				}
				else
				{
					result = "sip";
				}
				return result;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00006600 File Offset: 0x00005600
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00006618 File Offset: 0x00005618
		public bool IsSecure
		{
			get
			{
				return this.m_IsSecure;
			}
			set
			{
				this.m_IsSecure = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00006624 File Offset: 0x00005624
		public string Address
		{
			get
			{
				return this.m_User + "@" + this.m_Host;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x0000664C File Offset: 0x0000564C
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00006664 File Offset: 0x00005664
		public string User
		{
			get
			{
				return this.m_User;
			}
			set
			{
				this.m_User = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00006670 File Offset: 0x00005670
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00006688 File Offset: 0x00005688
		public string Host
		{
			get
			{
				return this.m_Host;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Host value can't be null or '' !");
				}
				this.m_Host = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000066B4 File Offset: 0x000056B4
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x000066CC File Offset: 0x000056CC
		public int Port
		{
			get
			{
				return this.m_Port;
			}
			set
			{
				this.m_Port = value;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x000066D8 File Offset: 0x000056D8
		public string HostPort
		{
			get
			{
				bool flag = this.m_Port == -1;
				string result;
				if (flag)
				{
					result = this.m_Host;
				}
				else
				{
					result = this.m_Host + ":" + this.m_Port;
				}
				return result;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00006720 File Offset: 0x00005720
		public SIP_ParameterCollection Parameters
		{
			get
			{
				return this.m_pParameters;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00006738 File Offset: 0x00005738
		// (set) Token: 0x060000C9 RID: 201 RVA: 0x00006774 File Offset: 0x00005774
		public int Param_Cause
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["cause"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					this.Parameters.Remove("cause");
				}
				else
				{
					this.Parameters.Set("cause", value.ToString());
				}
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000CA RID: 202 RVA: 0x000067BC File Offset: 0x000057BC
		// (set) Token: 0x060000CB RID: 203 RVA: 0x000067F4 File Offset: 0x000057F4
		public string Param_Comp
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["comp"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("comp");
				}
				else
				{
					this.Parameters.Set("comp", value);
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00006834 File Offset: 0x00005834
		// (set) Token: 0x060000CD RID: 205 RVA: 0x0000686C File Offset: 0x0000586C
		public string Param_ContentType
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["content-type"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("content-type");
				}
				else
				{
					this.Parameters.Set("content-type", value);
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000CE RID: 206 RVA: 0x000068AC File Offset: 0x000058AC
		// (set) Token: 0x060000CF RID: 207 RVA: 0x000068E8 File Offset: 0x000058E8
		public int Param_Delay
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["delay"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					this.Parameters.Remove("delay");
				}
				else
				{
					this.Parameters.Set("delay", value.ToString());
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00006930 File Offset: 0x00005930
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x0000696C File Offset: 0x0000596C
		public int Param_Duration
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["duration"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					this.Parameters.Remove("duration");
				}
				else
				{
					this.Parameters.Set("duration", value.ToString());
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x000069B4 File Offset: 0x000059B4
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x000069EC File Offset: 0x000059EC
		public string Param_Locale
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["locale"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("locale");
				}
				else
				{
					this.Parameters.Set("locale", value);
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00006A2C File Offset: 0x00005A2C
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00006A60 File Offset: 0x00005A60
		public bool Param_Lr
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["lr"];
				return sip_Parameter != null;
			}
			set
			{
				bool flag = !value;
				if (flag)
				{
					this.Parameters.Remove("lr");
				}
				else
				{
					this.Parameters.Set("lr", null);
				}
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00006AA0 File Offset: 0x00005AA0
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00006AD8 File Offset: 0x00005AD8
		public string Param_Maddr
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["maddr"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("maddr");
				}
				else
				{
					this.Parameters.Set("maddr", value);
				}
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00006B18 File Offset: 0x00005B18
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00006B50 File Offset: 0x00005B50
		public string Param_Method
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["method"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("method");
				}
				else
				{
					this.Parameters.Set("method", value);
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00006B90 File Offset: 0x00005B90
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00006BC8 File Offset: 0x00005BC8
		public string Param_Play
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["play"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("play");
				}
				else
				{
					this.Parameters.Set("play", value);
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00006C08 File Offset: 0x00005C08
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00006C68 File Offset: 0x00005C68
		public int Param_Repeat
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["ttl"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					bool flag2 = sip_Parameter.Value.ToLower() == "forever";
					if (flag2)
					{
						result = int.MaxValue;
					}
					else
					{
						result = Convert.ToInt32(sip_Parameter.Value);
					}
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					this.Parameters.Remove("ttl");
				}
				else
				{
					bool flag2 = value == int.MaxValue;
					if (flag2)
					{
						this.Parameters.Set("ttl", "forever");
					}
					else
					{
						this.Parameters.Set("ttl", value.ToString());
					}
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00006CD4 File Offset: 0x00005CD4
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00006D0C File Offset: 0x00005D0C
		public string Param_Target
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["target"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("target");
				}
				else
				{
					this.Parameters.Set("target", value);
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00006D4C File Offset: 0x00005D4C
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00006D84 File Offset: 0x00005D84
		public string Param_Transport
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["transport"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("transport");
				}
				else
				{
					this.Parameters.Set("transport", value);
				}
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00006DC4 File Offset: 0x00005DC4
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00006E00 File Offset: 0x00005E00
		public int Param_Ttl
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["ttl"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					result = Convert.ToInt32(sip_Parameter.Value);
				}
				else
				{
					result = -1;
				}
				return result;
			}
			set
			{
				bool flag = value == -1;
				if (flag)
				{
					this.Parameters.Remove("ttl");
				}
				else
				{
					this.Parameters.Set("ttl", value.ToString());
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00006E48 File Offset: 0x00005E48
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00006E80 File Offset: 0x00005E80
		public string Param_User
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["user"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("user");
				}
				else
				{
					this.Parameters.Set("user", value);
				}
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00006EC0 File Offset: 0x00005EC0
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00006EF8 File Offset: 0x00005EF8
		public string Param_Voicexml
		{
			get
			{
				SIP_Parameter sip_Parameter = this.Parameters["voicexml"];
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
				bool flag = value == null;
				if (flag)
				{
					this.Parameters.Remove("voicexml");
				}
				else
				{
					this.Parameters.Set("voicexml", value);
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00006F38 File Offset: 0x00005F38
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00006F50 File Offset: 0x00005F50
		public string Header
		{
			get
			{
				return this.m_Header;
			}
			set
			{
				this.m_Header = value;
			}
		}

		// Token: 0x04000067 RID: 103
		private bool m_IsSecure = false;

		// Token: 0x04000068 RID: 104
		private string m_User = null;

		// Token: 0x04000069 RID: 105
		private string m_Host = "";

		// Token: 0x0400006A RID: 106
		private int m_Port = -1;

		// Token: 0x0400006B RID: 107
		private SIP_ParameterCollection m_pParameters = null;

		// Token: 0x0400006C RID: 108
		private string m_Header = null;
	}
}
