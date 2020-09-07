using System;
using System.Collections.Generic;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x02000121 RID: 289
	public class MultiStream : Stream
	{
		// Token: 0x06000B6E RID: 2926 RVA: 0x00046595 File Offset: 0x00045595
		public MultiStream()
		{
			this.m_pStreams = new Queue<Stream>();
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x000465B8 File Offset: 0x000455B8
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				this.m_pStreams = null;
				base.Dispose();
			}
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x000465E8 File Offset: 0x000455E8
		public void AppendStream(Stream stream)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStreams.Enqueue(stream);
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x00046634 File Offset: 0x00045634
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x00046658 File Offset: 0x00045658
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x00046684 File Offset: 0x00045684
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x000466B0 File Offset: 0x000456B0
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			int num;
			for (;;)
			{
				bool flag = this.m_pStreams.Count == 0;
				if (flag)
				{
					break;
				}
				num = this.m_pStreams.Peek().Read(buffer, offset, count);
				bool flag2 = num == 0;
				if (!flag2)
				{
					goto IL_60;
				}
				this.m_pStreams.Dequeue();
			}
			return 0;
			IL_60:
			return num;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0004672C File Offset: 0x0004572C
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x00046758 File Offset: 0x00045758
		public override bool CanRead
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return true;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x00046784 File Offset: 0x00045784
		public override bool CanSeek
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x000467B0 File Offset: 0x000457B0
		public override bool CanWrite
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return false;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000B79 RID: 2937 RVA: 0x000467DC File Offset: 0x000457DC
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				long num = 0L;
				foreach (Stream stream in this.m_pStreams.ToArray())
				{
					num += stream.Length;
				}
				return num;
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x00046838 File Offset: 0x00045838
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x00046864 File Offset: 0x00045864
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				throw new NotSupportedException();
			}
		}

		// Token: 0x040004B7 RID: 1207
		private bool m_IsDisposed = false;

		// Token: 0x040004B8 RID: 1208
		private Queue<Stream> m_pStreams = null;
	}
}
