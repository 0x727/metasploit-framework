using System;

namespace LumiSoft.Net
{
	// Token: 0x0200000F RID: 15
	public class EventArgs<T> : EventArgs
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002F48 File Offset: 0x00001F48
		public EventArgs(T value)
		{
			this.m_pValue = value;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002F5C File Offset: 0x00001F5C
		public T Value
		{
			get
			{
				return this.m_pValue;
			}
		}

		// Token: 0x0400002D RID: 45
		private T m_pValue;
	}
}
