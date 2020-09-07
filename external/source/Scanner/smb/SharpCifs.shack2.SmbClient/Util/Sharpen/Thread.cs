using System;
//using System.Threading;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000067 RID: 103
	public class Thread : IRunnable
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x0000BC9D File Offset: 0x00009E9D
		public Thread() : this(null, null, null)
		{
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000BCAA File Offset: 0x00009EAA
		public Thread(string name) : this(null, null, name)
		{
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000BCB7 File Offset: 0x00009EB7
		public Thread(ThreadGroup grp, string name) : this(null, grp, name)
		{
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		public Thread(IRunnable runnable) : this(runnable, null, null)
		{
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000BCD4 File Offset: 0x00009ED4
		private Thread(IRunnable runnable, ThreadGroup grp, string name)
		{
			this._thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.InternalRun));
			this._runnable = (runnable ?? this);
			this._tgroup = (grp ?? Thread._defaultGroup);
			this._tgroup.Add(this);
			bool flag = name != null;
			if (flag)
			{
				this._thread.Name = name;
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000BD3E File Offset: 0x00009F3E
		private Thread(System.Threading.Thread t)
		{
			this._thread = t;
			this._tgroup = Thread._defaultGroup;
			this._tgroup.Add(this);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000BD68 File Offset: 0x00009F68
		public static Thread CurrentThread()
		{
			bool flag = (Thread._wrapperThread == null);
			if (flag)
			{
				Thread._wrapperThread = new Thread(System.Threading.Thread.CurrentThread);
			}
			return Thread._wrapperThread;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000BD9C File Offset: 0x00009F9C
		public string GetName()
		{
			return this._thread.Name;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000BDBC File Offset: 0x00009FBC
		public ThreadGroup GetThreadGroup()
		{
			return this._tgroup;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000BDD4 File Offset: 0x00009FD4
		private void InternalRun()
		{
			Thread._wrapperThread = this;
			try
			{
				this._runnable.Run();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
				this._tgroup.Remove(this);
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00008663 File Offset: 0x00006863
		public static void Yield()
		{
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000BE34 File Offset: 0x0000A034
		public void Interrupt()
		{
			System.Threading.Thread thread = this._thread;
			lock (thread)
			{
				this._interrupted = true;
				this._thread.Abort();
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000BE88 File Offset: 0x0000A088
		public static bool Interrupted()
		{
			bool flag = Thread._wrapperThread == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Thread wrapperThread = Thread._wrapperThread;
				Thread obj = wrapperThread;
				lock (obj)
				{
					bool interrupted = Thread._wrapperThread._interrupted;
					Thread._wrapperThread._interrupted = false;
					result = interrupted;
				}
			}
			return result;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000BEF8 File Offset: 0x0000A0F8
		public bool IsAlive()
		{
			return this._thread.IsAlive;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000BF15 File Offset: 0x0000A115
		public void Join()
		{
			this._thread.Join();
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000BF24 File Offset: 0x0000A124
		public void Join(long timeout)
		{
			this._thread.Join((int)timeout);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00008663 File Offset: 0x00006863
		public virtual void Run()
		{
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000BF35 File Offset: 0x0000A135
		public void SetDaemon(bool daemon)
		{
			this._thread.IsBackground = daemon;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000BF45 File Offset: 0x0000A145
		public void SetName(string name)
		{
			this._thread.Name = name;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000BF55 File Offset: 0x0000A155
		public static void Sleep(long milis)
		{
			Thread.Sleep((int)milis);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000BF60 File Offset: 0x0000A160
		public void Start()
		{
			this._thread.Start();
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000BF6F File Offset: 0x0000A16F
		public void Abort()
		{
			this._thread.Abort();
		}

		// Token: 0x04000087 RID: 135
		private static ThreadGroup _defaultGroup = new ThreadGroup();

		// Token: 0x04000088 RID: 136
		private bool _interrupted;

		// Token: 0x04000089 RID: 137
		private IRunnable _runnable;

		// Token: 0x0400008A RID: 138
		private ThreadGroup _tgroup;

		// Token: 0x0400008B RID: 139
		private System.Threading.Thread _thread;

		// Token: 0x0400008C RID: 140
		[ThreadStatic]
		private static Thread _wrapperThread;
	}
}
