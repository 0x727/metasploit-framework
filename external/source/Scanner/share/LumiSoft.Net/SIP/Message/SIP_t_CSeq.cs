using System;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200006A RID: 106
	public class SIP_t_CSeq : SIP_t_Value
	{
		// Token: 0x06000376 RID: 886 RVA: 0x0001265B File Offset: 0x0001165B
		public SIP_t_CSeq(string value)
		{
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00012684 File Offset: 0x00011684
		public SIP_t_CSeq(int sequenceNumber, string requestMethod)
		{
			this.m_SequenceNumber = sequenceNumber;
			this.m_RequestMethod = requestMethod;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000126B0 File Offset: 0x000116B0
		public void Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.Parse(new StringReader(value));
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000126E0 File Offset: 0x000116E0
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
				throw new SIP_ParseException("Invalid 'CSeq' value, sequence number is missing !");
			}
			try
			{
				this.m_SequenceNumber = Convert.ToInt32(text);
			}
			catch
			{
				throw new SIP_ParseException("Invalid CSeq 'sequence number' value !");
			}
			text = reader.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new SIP_ParseException("Invalid 'CSeq' value, request method is missing !");
			}
			this.m_RequestMethod = text;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00012770 File Offset: 0x00011770
		public override string ToStringValue()
		{
			return this.m_SequenceNumber + " " + this.m_RequestMethod;
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600037B RID: 891 RVA: 0x000127A0 File Offset: 0x000117A0
		// (set) Token: 0x0600037C RID: 892 RVA: 0x000127B8 File Offset: 0x000117B8
		public int SequenceNumber
		{
			get
			{
				return this.m_SequenceNumber;
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("Property SequenceNumber value must be >= 1 !");
				}
				this.m_SequenceNumber = value;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600037D RID: 893 RVA: 0x000127E4 File Offset: 0x000117E4
		// (set) Token: 0x0600037E RID: 894 RVA: 0x000127FC File Offset: 0x000117FC
		public string RequestMethod
		{
			get
			{
				return this.m_RequestMethod;
			}
			set
			{
				bool flag = string.IsNullOrEmpty(value);
				if (flag)
				{
					throw new ArgumentException("Property RequestMethod value can't be null or empty !");
				}
				this.m_RequestMethod = value;
			}
		}

		// Token: 0x04000140 RID: 320
		private int m_SequenceNumber = 1;

		// Token: 0x04000141 RID: 321
		private string m_RequestMethod = "";
	}
}
