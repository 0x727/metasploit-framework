using System;
using System.Net;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000077 RID: 119
	public class SIP_t_ViaParm : SIP_t_ValueWithParams
	{
		// Token: 0x060003D2 RID: 978 RVA: 0x000138B0 File Offset: 0x000128B0
		public SIP_t_ViaParm()
		{
			this.m_ProtocolName = "SIP";
			this.m_ProtocolVersion = "2.0";
			this.m_ProtocolTransport = "UDP";
			this.m_pSentBy = new HostEndPoint("localhost", -1);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00013920 File Offset: 0x00012920
		public static string CreateBranch()
		{
			return "z9hG4bK-" + Guid.NewGuid().ToString().Replace("-", "");
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00013960 File Offset: 0x00012960
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00013990 File Offset: 0x00012990
		public override void Parse(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			string text = reader.QuotedReadToDelimiter('/');
			bool flag2 = text == null;
			if (flag2)
			{
				throw new SIP_ParseException("Via header field protocol-name is missing !");
			}
			this.ProtocolName = text.Trim();
			text = reader.QuotedReadToDelimiter('/');
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("Via header field protocol-version is missing !");
			}
			this.ProtocolVersion = text.Trim();
			text = reader.ReadWord();
			bool flag4 = text == null;
			if (flag4)
			{
				throw new SIP_ParseException("Via header field transport is missing !");
			}
			this.ProtocolTransport = text.Trim();
			text = reader.QuotedReadToDelimiter(new char[]
			{
				';',
				','
			}, false);
			bool flag5 = text == null;
			if (flag5)
			{
				throw new SIP_ParseException("Via header field sent-by is missing !");
			}
			this.SentBy = HostEndPoint.Parse(text.Trim());
			base.ParseParameters(reader);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00013A78 File Offset: 0x00012A78
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				this.ProtocolName,
				"/",
				this.ProtocolVersion,
				"/",
				this.ProtocolTransport,
				" "
			}));
			stringBuilder.Append(this.SentBy.ToString());
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x00013AFC File Offset: 0x00012AFC
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x00013B14 File Offset: 0x00012B14
		public string ProtocolName
		{
			get
			{
				return this.m_ProtocolName;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property ProtocolName can't be null or empty !");
				}
				this.m_ProtocolName = value;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00013B40 File Offset: 0x00012B40
		// (set) Token: 0x060003DA RID: 986 RVA: 0x00013B58 File Offset: 0x00012B58
		public string ProtocolVersion
		{
			get
			{
				return this.m_ProtocolVersion;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property ProtocolVersion can't be null or empty !");
				}
				this.m_ProtocolVersion = value;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00013B84 File Offset: 0x00012B84
		// (set) Token: 0x060003DC RID: 988 RVA: 0x00013BA4 File Offset: 0x00012BA4
		public string ProtocolTransport
		{
			get
			{
				return this.m_ProtocolTransport.ToUpper();
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property ProtocolTransport can't be null or empty !");
				}
				this.m_ProtocolTransport = value;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060003DD RID: 989 RVA: 0x00013BD0 File Offset: 0x00012BD0
		// (set) Token: 0x060003DE RID: 990 RVA: 0x00013BE8 File Offset: 0x00012BE8
		public HostEndPoint SentBy
		{
			get
			{
				return this.m_pSentBy;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.m_pSentBy = value;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060003DF RID: 991 RVA: 0x00013C14 File Offset: 0x00012C14
		public int SentByPortWithDefault
		{
			get
			{
				bool flag = this.m_pSentBy.Port != -1;
				int result;
				if (flag)
				{
					result = this.m_pSentBy.Port;
				}
				else
				{
					bool flag2 = this.ProtocolTransport == "TLS";
					if (flag2)
					{
						result = 5061;
					}
					else
					{
						result = 5060;
					}
				}
				return result;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x00013C70 File Offset: 0x00012C70
		// (set) Token: 0x060003E1 RID: 993 RVA: 0x00013CA8 File Offset: 0x00012CA8
		public string Branch
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["branch"];
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
					base.Parameters.Remove("branch");
				}
				else
				{
					bool flag2 = !value.StartsWith("z9hG4bK");
					if (flag2)
					{
						throw new ArgumentException("Property Branch value must start with magic cookie 'z9hG4bK' !");
					}
					base.Parameters.Set("branch", value);
				}
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00013D08 File Offset: 0x00012D08
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x00013D40 File Offset: 0x00012D40
		public string Comp
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["comp"];
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
					base.Parameters.Remove("comp");
				}
				else
				{
					base.Parameters.Set("comp", value);
				}
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x00013D84 File Offset: 0x00012D84
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x00013DBC File Offset: 0x00012DBC
		public string Maddr
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["maddr"];
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
					base.Parameters.Remove("maddr");
				}
				else
				{
					base.Parameters.Set("maddr", value);
				}
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x00013E00 File Offset: 0x00012E00
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x00013E3C File Offset: 0x00012E3C
		public IPAddress Received
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["received"];
				bool flag = sip_Parameter != null;
				IPAddress result;
				if (flag)
				{
					result = IPAddress.Parse(sip_Parameter.Value);
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
					base.Parameters.Remove("received");
				}
				else
				{
					base.Parameters.Set("received", value.ToString());
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x00013E80 File Offset: 0x00012E80
		// (set) Token: 0x060003E9 RID: 1001 RVA: 0x00013ED8 File Offset: 0x00012ED8
		public int RPort
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["rport"];
				bool flag = sip_Parameter != null;
				int result;
				if (flag)
				{
					bool flag2 = sip_Parameter.Value == "";
					if (flag2)
					{
						result = 0;
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
				bool flag = value < 0;
				if (flag)
				{
					base.Parameters.Remove("rport");
				}
				else
				{
					bool flag2 = value == 0;
					if (flag2)
					{
						base.Parameters.Set("rport", "");
					}
					else
					{
						base.Parameters.Set("rport", value.ToString());
					}
				}
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x00013F40 File Offset: 0x00012F40
		// (set) Token: 0x060003EB RID: 1003 RVA: 0x00013F7C File Offset: 0x00012F7C
		public int Ttl
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["ttl"];
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
				bool flag = value < 0;
				if (flag)
				{
					base.Parameters.Remove("ttl");
				}
				else
				{
					base.Parameters.Set("ttl", value.ToString());
				}
			}
		}

		// Token: 0x04000150 RID: 336
		private string m_ProtocolName = "";

		// Token: 0x04000151 RID: 337
		private string m_ProtocolVersion = "";

		// Token: 0x04000152 RID: 338
		private string m_ProtocolTransport = "";

		// Token: 0x04000153 RID: 339
		private HostEndPoint m_pSentBy = null;
	}
}
