using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200004F RID: 79
	public class SIP_t_RAck : SIP_t_Value
	{
		// Token: 0x06000295 RID: 661 RVA: 0x0000F25A File Offset: 0x0000E25A
		public SIP_t_RAck(string value)
		{
			this.Parse(value);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000F285 File Offset: 0x0000E285
		public SIP_t_RAck(int responseNo, int cseqNo, string method)
		{
			this.ResponseNumber = responseNo;
			this.CSeqNumber = cseqNo;
			this.Method = method;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000F2C0 File Offset: 0x0000E2C0
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000F2F0 File Offset: 0x0000E2F0
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
				throw new SIP_ParseException("RAck response-num value is missing !");
			}
			try
			{
				this.m_ResponseNumber = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid RAck response-num value !");
			}
			text = reader.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("RAck CSeq-num value is missing !");
			}
			try
			{
				this.m_CSeqNumber = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid RAck CSeq-num value !");
			}
			text = reader.ReadWord();
			bool flag4 = text == null;
			if (flag4)
			{
				throw new SIP_ParseException("RAck Method value is missing !");
			}
			this.m_Method = text;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000F3C4 File Offset: 0x0000E3C4
		public override string ToStringValue()
		{
			return string.Concat(new object[]
			{
				this.m_ResponseNumber,
				" ",
				this.m_CSeqNumber,
				" ",
				this.m_Method
			});
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600029A RID: 666 RVA: 0x0000F418 File Offset: 0x0000E418
		// (set) Token: 0x0600029B RID: 667 RVA: 0x0000F430 File Offset: 0x0000E430
		public int ResponseNumber
		{
			get
			{
				return this.m_ResponseNumber;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("ResponseNumber value must be >= 1 !");
				}
				this.m_ResponseNumber = value;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600029C RID: 668 RVA: 0x0000F45C File Offset: 0x0000E45C
		// (set) Token: 0x0600029D RID: 669 RVA: 0x0000F474 File Offset: 0x0000E474
		public int CSeqNumber
		{
			get
			{
				return this.m_CSeqNumber;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("CSeqNumber value must be >= 1 !");
				}
				this.m_CSeqNumber = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0000F4A0 File Offset: 0x0000E4A0
		// (set) Token: 0x0600029F RID: 671 RVA: 0x0000F4B8 File Offset: 0x0000E4B8
		public string Method
		{
			get
			{
				return this.m_Method;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("Method");
				}
				this.m_Method = value;
			}
		}

		// Token: 0x0400010D RID: 269
		private int m_ResponseNumber = 1;

		// Token: 0x0400010E RID: 270
		private int m_CSeqNumber = 1;

		// Token: 0x0400010F RID: 271
		private string m_Method = "";
	}
}
