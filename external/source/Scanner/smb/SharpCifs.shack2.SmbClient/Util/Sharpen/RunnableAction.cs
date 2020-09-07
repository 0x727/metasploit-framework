using System;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200006B RID: 107
	internal class RunnableAction : IRunnable
	{
		// Token: 0x0600030D RID: 781 RVA: 0x0000C574 File Offset: 0x0000A774
		public RunnableAction(Action a)
		{
			this._action = a;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000C585 File Offset: 0x0000A785
		public void Run()
		{
			this._action();
		}

		// Token: 0x04000096 RID: 150
		private Action _action;
	}
}
