using System;
using System.Collections.Generic;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000068 RID: 104
	public class ThreadGroup
	{
		// Token: 0x060002F8 RID: 760 RVA: 0x0000BF8A File Offset: 0x0000A18A
		public ThreadGroup()
		{
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000BF8A File Offset: 0x0000A18A
		public ThreadGroup(string name)
		{
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000BFA0 File Offset: 0x0000A1A0
		internal void Add(Thread t)
		{
			List<Thread> threads = this._threads;
			lock (threads)
			{
				this._threads.Add(t);
			}
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000BFEC File Offset: 0x0000A1EC
		internal void Remove(Thread t)
		{
			List<Thread> threads = this._threads;
			lock (threads)
			{
				this._threads.Remove(t);
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000C038 File Offset: 0x0000A238
		public int Enumerate(Thread[] array)
		{
			List<Thread> threads = this._threads;
			int result;
			lock (threads)
			{
				int num = Math.Min(array.Length, this._threads.Count);
				this._threads.CopyTo(0, array, 0, num);
				result = num;
			}
			return result;
		}

		// Token: 0x0400008D RID: 141
		private List<Thread> _threads = new List<Thread>();
	}
}
