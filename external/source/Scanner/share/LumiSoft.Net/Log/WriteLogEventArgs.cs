using System;

namespace LumiSoft.Net.Log
{
	// Token: 0x02000035 RID: 53
	public class WriteLogEventArgs : EventArgs
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000C794 File Offset: 0x0000B794
		public WriteLogEventArgs(LogEntry logEntry)
		{
			bool flag = logEntry == null;
			if (flag)
			{
				throw new ArgumentNullException("logEntry");
			}
			this.m_pLogEntry = logEntry;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000C7CC File Offset: 0x0000B7CC
		public LogEntry LogEntry
		{
			get
			{
				return this.m_pLogEntry;
			}
		}

		// Token: 0x040000D1 RID: 209
		private LogEntry m_pLogEntry = null;
	}
}
