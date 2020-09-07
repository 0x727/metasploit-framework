using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200005F RID: 95
	public class SIP_t_Challenge : SIP_t_Value
	{
		// Token: 0x06000320 RID: 800 RVA: 0x000111C1 File Offset: 0x000101C1
		public SIP_t_Challenge(string value)
		{
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000321 RID: 801 RVA: 0x000111F0 File Offset: 0x000101F0
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00011220 File Offset: 0x00010220
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
				throw new SIP_ParseException("Invalid WWW-Authenticate: value, authentication method is missing !");
			}
			this.m_Method = text;
			text = reader.ReadToEnd();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("Invalid WWW-Authenticate: value, authentication parameters are missing !");
			}
			this.m_AuthData = text.Trim();
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0001128C File Offset: 0x0001028C
		public override string ToStringValue()
		{
			return this.m_Method + " " + this.m_AuthData;
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000324 RID: 804 RVA: 0x000112B4 File Offset: 0x000102B4
		// (set) Token: 0x06000325 RID: 805 RVA: 0x000112CC File Offset: 0x000102CC
		public string Method
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property Method value cant be null or mepty !");
				}
				this.m_Method = value;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000326 RID: 806 RVA: 0x000112F8 File Offset: 0x000102F8
		// (set) Token: 0x06000327 RID: 807 RVA: 0x00011310 File Offset: 0x00010310
		public string AuthData
		{
			get
			{
				return this.m_AuthData;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property AuthData value cant be null or mepty !");
				}
				this.m_AuthData = value;
			}
		}

		// Token: 0x0400012D RID: 301
		private string m_Method = "";

		// Token: 0x0400012E RID: 302
		private string m_AuthData = "";
	}
}
