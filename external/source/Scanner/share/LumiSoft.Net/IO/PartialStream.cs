using System;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x02000122 RID: 290
	public class PartialStream : Stream
	{
		// Token: 0x06000B7C RID: 2940 RVA: 0x00046890 File Offset: 0x00045890
		public PartialStream(Stream stream, long start, long length)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = !stream.CanSeek;
			if (flag2)
			{
				throw new ArgumentException("Argument 'stream' does not support seeking.");
			}
			bool flag3 = start < 0L;
			if (flag3)
			{
				throw new ArgumentException("Argument 'start' value must be >= 0.");
			}
			bool flag4 = start + length > stream.Length;
			if (flag4)
			{
				throw new ArgumentException("Argument 'length' value will exceed source stream length.");
			}
			this.m_pStream = stream;
			this.m_Start = start;
			this.m_Length = length;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00046940 File Offset: 0x00045940
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				base.Dispose();
			}
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x0004696C File Offset: 0x0004596C
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x00046990 File Offset: 0x00045990
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = origin == SeekOrigin.Begin;
			if (flag)
			{
				this.m_Position = 0L;
			}
			else
			{
				bool flag2 = origin == SeekOrigin.Current;
				if (!flag2)
				{
					bool flag3 = origin == SeekOrigin.End;
					if (flag3)
					{
						this.m_Position = this.m_Length;
					}
				}
			}
			return this.m_Position;
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x000469F8 File Offset: 0x000459F8
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x00046A24 File Offset: 0x00045A24
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = this.m_pStream.Position != this.m_Start + this.m_Position;
			if (flag)
			{
				this.m_pStream.Position = this.m_Start + this.m_Position;
			}
			int num = this.m_pStream.Read(buffer, offset, Math.Min(count, (int)(this.Length - this.m_Position)));
			this.m_Position += (long)num;
			return num;
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x00046ABC File Offset: 0x00045ABC
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			throw new NotSupportedException();
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000B83 RID: 2947 RVA: 0x00046AE8 File Offset: 0x00045AE8
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

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000B84 RID: 2948 RVA: 0x00046B14 File Offset: 0x00045B14
		public override bool CanSeek
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

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x00046B40 File Offset: 0x00045B40
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

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000B86 RID: 2950 RVA: 0x00046B6C File Offset: 0x00045B6C
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_Length;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000B87 RID: 2951 RVA: 0x00046B9C File Offset: 0x00045B9C
		// (set) Token: 0x06000B88 RID: 2952 RVA: 0x00046BCC File Offset: 0x00045BCC
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_Position;
			}
			set
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				bool flag = value < 0L || value > this.Length;
				if (flag)
				{
					throw new ArgumentException("Property 'Position' value must be >= 0 and <= this.Length.");
				}
				this.m_Position = value;
			}
		}

		// Token: 0x040004B9 RID: 1209
		private bool m_IsDisposed = false;

		// Token: 0x040004BA RID: 1210
		private Stream m_pStream = null;

		// Token: 0x040004BB RID: 1211
		private long m_Start = 0L;

		// Token: 0x040004BC RID: 1212
		private long m_Length = 0L;

		// Token: 0x040004BD RID: 1213
		private long m_Position = 0L;
	}
}
