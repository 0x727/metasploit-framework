using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x02000049 RID: 73
	public class SIP_t_Event : SIP_t_ValueWithParams
	{
		// Token: 0x06000266 RID: 614 RVA: 0x0000E8BA File Offset: 0x0000D8BA
		public SIP_t_Event(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000E8D8 File Offset: 0x0000D8D8
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000E908 File Offset: 0x0000D908
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
				throw new SIP_ParseException("SIP Event 'event-type' value is missing !");
			}
			this.m_EventType = text;
			base.ParseParameters(reader);
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000E954 File Offset: 0x0000D954
		public override string ToStringValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_EventType);
			stringBuilder.Append(base.ParametersToString());
			return stringBuilder.ToString();
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000E98C File Offset: 0x0000D98C
		// (set) Token: 0x0600026B RID: 619 RVA: 0x0000E9A4 File Offset: 0x0000D9A4
		public string EventType
		{
			get
			{
				return this.m_EventType;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("EventType");
				}
				bool flag2 = value == "";
				if (flag2)
				{
					throw new ArgumentException("Property EventType value can't be '' !");
				}
				this.m_EventType = value;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000E9E8 File Offset: 0x0000D9E8
		// (set) Token: 0x0600026D RID: 621 RVA: 0x0000EA20 File Offset: 0x0000DA20
		public string ID
		{
			get
			{
				SIP_Parameter sip_Parameter = base.Parameters["id"];
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
					base.Parameters.Remove("id");
				}
				else
				{
					base.Parameters.Set("id", value);
				}
			}
		}

		// Token: 0x04000107 RID: 263
		private string m_EventType = "";
	}
}
