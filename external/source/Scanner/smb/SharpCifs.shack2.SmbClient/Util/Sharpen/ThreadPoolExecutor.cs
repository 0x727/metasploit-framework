using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x0200006A RID: 106
	internal class ThreadPoolExecutor
	{
		// Token: 0x060002FF RID: 767 RVA: 0x0000C0C9 File Offset: 0x0000A2C9
		public ThreadPoolExecutor(int corePoolSize, ThreadFactory factory)
		{
			this._corePoolSize = corePoolSize;
			this._maxPoolSize = corePoolSize;
			this._tf = factory;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000C0FE File Offset: 0x0000A2FE
		public void SetMaximumPoolSize(int size)
		{
			this._maxPoolSize = size;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000C108 File Offset: 0x0000A308
		public bool IsShutdown()
		{
			return this._shutdown;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000C120 File Offset: 0x0000A320
		public virtual bool IsTerminated()
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			bool result;
			lock (pendingTasks)
			{
				result = (this._shutdown && this._pendingTasks.Count == 0);
			}
			return result;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000C17C File Offset: 0x0000A37C
		public virtual bool IsTerminating()
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			bool result;
			lock (pendingTasks)
			{
				result = (this._shutdown && !this.IsTerminated());
			}
			return result;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000C1D0 File Offset: 0x0000A3D0
		public int GetCorePoolSize()
		{
			return this._corePoolSize;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000C1E8 File Offset: 0x0000A3E8
		public void PrestartAllCoreThreads()
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			lock (pendingTasks)
			{
				while (this._runningThreads < this._corePoolSize)
				{
					this.StartPoolThread();
				}
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000C244 File Offset: 0x0000A444
		public void SetThreadFactory(ThreadFactory f)
		{
			this._tf = f;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000C24E File Offset: 0x0000A44E
		public void Execute(IRunnable r)
		{
			this.InternalExecute(r, true);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000C25C File Offset: 0x0000A45C
		internal void InternalExecute(IRunnable r, bool checkShutdown)
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			lock (pendingTasks)
			{
				bool flag2 = this._shutdown && checkShutdown;
				if (flag2)
				{
					throw new InvalidOperationException();
				}
				bool flag3 = this._runningThreads < this._corePoolSize;
				if (flag3)
				{
					this.StartPoolThread();
				}
				else
				{
					bool flag4 = this._freeThreads > 0;
					if (flag4)
					{
						this._freeThreads--;
					}
					else
					{
						bool flag5 = this._runningThreads < this._maxPoolSize;
						if (flag5)
						{
							this.StartPoolThread();
						}
					}
				}
				this._pendingTasks.Enqueue(r);
				Monitor.PulseAll(this._pendingTasks);
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0000C324 File Offset: 0x0000A524
		private void StartPoolThread()
		{
			this._runningThreads++;
			this._pool.Add(this._tf.NewThread(new RunnableAction(new Action(this.RunPoolThread))));
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0000C360 File Offset: 0x0000A560
		public void RunPoolThread()
		{
			while (!this.IsTerminated())
			{
				try
				{
					IRunnable runnable = null;
					Queue<IRunnable> pendingTasks = this._pendingTasks;
					lock (pendingTasks)
					{
						this._freeThreads++;
						while (!this.IsTerminated() && this._pendingTasks.Count == 0)
						{
							Monitor.Wait(this._pendingTasks);
						}
						bool flag2 = this.IsTerminated();
						if (flag2)
						{
							break;
						}
						runnable = this._pendingTasks.Dequeue();
					}
					bool flag3 = runnable != null;
					if (flag3)
					{
						runnable.Run();
					}
				}
				catch (ThreadAbortException)
				{
					break;
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000C444 File Offset: 0x0000A644
		public virtual void Shutdown()
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			lock (pendingTasks)
			{
				this._shutdown = true;
				Monitor.PulseAll(this._pendingTasks);
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000C498 File Offset: 0x0000A698
		public virtual List<IRunnable> ShutdownNow()
		{
			Queue<IRunnable> pendingTasks = this._pendingTasks;
			List<IRunnable> result;
			lock (pendingTasks)
			{
				this._shutdown = true;
				foreach (Thread thread in this._pool)
				{
					try
					{
						thread.Abort();
					}
					catch
					{
					}
				}
				this._pool.Clear();
				this._freeThreads = 0;
				this._runningThreads = 0;
				List<IRunnable> list = new List<IRunnable>(this._pendingTasks);
				this._pendingTasks.Clear();
				result = list;
			}
			return result;
		}

		// Token: 0x0400008E RID: 142
		private ThreadFactory _tf;

		// Token: 0x0400008F RID: 143
		private int _corePoolSize;

		// Token: 0x04000090 RID: 144
		private int _maxPoolSize;

		// Token: 0x04000091 RID: 145
		private List<Thread> _pool = new List<Thread>();

		// Token: 0x04000092 RID: 146
		private int _runningThreads;

		// Token: 0x04000093 RID: 147
		private int _freeThreads;

		// Token: 0x04000094 RID: 148
		private bool _shutdown;

		// Token: 0x04000095 RID: 149
		private Queue<IRunnable> _pendingTasks = new Queue<IRunnable>();
	}
}
