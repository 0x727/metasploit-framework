using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004A RID: 74
	public class SIP_t_EventType : SIP_t_Value
	{
		// Token: 0x0600026F RID: 623 RVA: 0x0000EA78 File Offset: 0x0000DA78
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000EAA8 File Offset: 0x0000DAA8
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
				throw new SIP_ParseException("Invalid 'event-type' value, event-type is missing !");
			}
			this.m_EventType = text;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000EAEC File Offset: 0x0000DAEC
		public override string ToStringValue()
		{
			return this.m_EventType;
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000EB04 File Offset: 0x0000DB04
		// (set) Token: 0x06000273 RID: 627 RVA: 0x0000EB1C File Offset: 0x0000DB1C
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
				this.m_EventType = value;
			}
		}

		// Token: 0x04000108 RID: 264
		private string m_EventType = "";
	}
}
