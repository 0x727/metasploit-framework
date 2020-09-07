using System;
using System.Diagnostics;

namespace LumiSoft.Net
{
	// Token: 0x02000006 RID: 6
	public class Error_EventArgs
	{
		// Token: 0x0600000E RID: 14 RVA: 0x0000226D File Offset: 0x0000126D
		public Error_EventArgs(Exception x, StackTrace stackTrace)
		{
			this.m_pException = x;
			this.m_pStackTrace = stackTrace;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000022A0 File Offset: 0x000012A0
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000022B8 File Offset: 0x000012B8
		public StackTrace StackTrace
		{
			get
			{
				return this.m_pStackTrace;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000022D0 File Offset: 0x000012D0
		public string Text
		{
			get
			{
				return this.m_Text;
			}
		}

		// Token: 0x0400000F RID: 15
		private Exception m_pException = null;

		// Token: 0x04000010 RID: 16
		private StackTrace m_pStackTrace = null;

		// Token: 0x04000011 RID: 17
		private string m_Text = "";
	}
}
