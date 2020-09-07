using System;

namespace LumiSoft.Net
{
	// Token: 0x02000010 RID: 16
	public class ExceptionEventArgs : EventArgs
	{
		// Token: 0x0600003D RID: 61 RVA: 0x00002F74 File Offset: 0x00001F74
		public ExceptionEventArgs(Exception exception)
		{
			bool flag = exception == null;
			if (flag)
			{
				throw new ArgumentNullException("exception");
			}
			this.m_pException = exception;
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002FAC File Offset: 0x00001FAC
		public Exception Exception
		{
			get
			{
				return this.m_pException;
			}
		}

		// Token: 0x0400002E RID: 46
		private Exception m_pException = null;
	}
}
