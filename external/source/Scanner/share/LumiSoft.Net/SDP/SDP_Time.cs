using System;

namespace LumiSoft.Net.SDP
{
	// Token: 0x020000BF RID: 191
	public class SDP_Time
	{
		// Token: 0x06000763 RID: 1891 RVA: 0x0002D26C File Offset: 0x0002C26C
		public SDP_Time(long startTime, long stopTime)
		{
			bool flag = startTime < 0L;
			if (flag)
			{
				throw new ArgumentException("Argument 'startTime' value must be >= 0.");
			}
			bool flag2 = stopTime < 0L;
			if (flag2)
			{
				throw new ArgumentException("Argument 'stopTime' value must be >= 0.");
			}
			this.m_StartTime = startTime;
			this.m_StopTime = stopTime;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0002D2CC File Offset: 0x0002C2CC
		public static SDP_Time Parse(string tValue)
		{
			StringReader stringReader = new StringReader(tValue);
			stringReader.QuotedReadToDelimiter('=');
			string text = stringReader.ReadWord();
			bool flag = text == null;
			if (flag)
			{
				throw new Exception("SDP message \"t\" field <start-time> value is missing !");
			}
			long startTime = Convert.ToInt64(text);
			text = stringReader.ReadWord();
			bool flag2 = text == null;
			if (flag2)
			{
				throw new Exception("SDP message \"t\" field <stop-time> value is missing !");
			}
			long stopTime = Convert.ToInt64(text);
			return new SDP_Time(startTime, stopTime);
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0002D348 File Offset: 0x0002C348
		public string ToValue()
		{
			return string.Concat(new object[]
			{
				"t=",
				this.StartTime,
				" ",
				this.StopTime,
				"\r\n"
			});
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000766 RID: 1894 RVA: 0x0002D39C File Offset: 0x0002C39C
		// (set) Token: 0x06000767 RID: 1895 RVA: 0x0002D3B4 File Offset: 0x0002C3B4
		public long StartTime
		{
			get
			{
				return this.m_StartTime;
			}
			set
			{
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentException("Property StartTime value must be >= 0 !");
				}
				this.m_StopTime = value;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x0002D3E0 File Offset: 0x0002C3E0
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x0002D3F8 File Offset: 0x0002C3F8
		public long StopTime
		{
			get
			{
				return this.m_StopTime;
			}
			set
			{
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentException("Property StopTime value must be >= 0 !");
				}
				this.m_StopTime = value;
			}
		}

		// Token: 0x04000329 RID: 809
		private long m_StartTime = 0L;

		// Token: 0x0400032A RID: 810
		private long m_StopTime = 0L;
	}
}
