using System;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x0200011D RID: 285
	public class MemoryStreamEx : Stream
	{
		// Token: 0x06000B31 RID: 2865 RVA: 0x00044F13 File Offset: 0x00043F13
		public MemoryStreamEx() : this(MemoryStreamEx.m_DefaultMemorySize)
		{
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00044F22 File Offset: 0x00043F22
		public MemoryStreamEx(int memSize)
		{
			this.m_MaxMemSize = memSize;
			this.m_pStream = new MemoryStream();
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00044F58 File Offset: 0x00043F58
		~MemoryStreamEx()
		{
			this.Dispose();
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00044F88 File Offset: 0x00043F88
		public new void Dispose()
		{
			bool isDisposed = this.m_IsDisposed;
			if (!isDisposed)
			{
				this.m_IsDisposed = true;
				bool flag = this.m_pStream != null;
				if (flag)
				{
					this.m_pStream.Close();
				}
				this.m_pStream = null;
				base.Dispose();
			}
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x00044FD4 File Offset: 0x00043FD4
		public override void Flush()
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			this.m_pStream.Flush();
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x00045004 File Offset: 0x00044004
		public override long Seek(long offset, SeekOrigin origin)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			return this.m_pStream.Seek(offset, origin);
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0004503C File Offset: 0x0004403C
		public override void SetLength(long value)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			this.m_pStream.SetLength(value);
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x00045070 File Offset: 0x00044070
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			return this.m_pStream.Read(buffer, offset, count);
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x000450BC File Offset: 0x000440BC
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool isDisposed = this.m_IsDisposed;
			if (isDisposed)
			{
				throw new ObjectDisposedException("SmartStream");
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = this.m_pStream is MemoryStream && this.m_pStream.Position + (long)count > (long)this.m_MaxMemSize;
			if (flag2)
			{
				FileStream fileStream = new FileStream(Path.GetTempPath() + "ls-" + Guid.NewGuid().ToString().Replace("-", "") + ".tmp", FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 32000, FileOptions.DeleteOnClose);
				this.m_pStream.Position = 0L;
				Net_Utils.StreamCopy(this.m_pStream, fileStream, 8000);
				this.m_pStream.Close();
				this.m_pStream = fileStream;
			}
			this.m_pStream.Write(buffer, offset, count);
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000B3A RID: 2874 RVA: 0x000451B0 File Offset: 0x000441B0
		// (set) Token: 0x06000B3B RID: 2875 RVA: 0x000451C8 File Offset: 0x000441C8
		public static int DefaultMemorySize
		{
			get
			{
				return MemoryStreamEx.m_DefaultMemorySize;
			}
			set
			{
				bool flag = value < 32000;
				if (flag)
				{
					throw new ArgumentException("Property 'DefaultMemorySize' value must be >= 32k.", "value");
				}
				MemoryStreamEx.m_DefaultMemorySize = value;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000B3C RID: 2876 RVA: 0x000451FC File Offset: 0x000441FC
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

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x00045228 File Offset: 0x00044228
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

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06000B3E RID: 2878 RVA: 0x00045254 File Offset: 0x00044254
		public override bool CanWrite
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

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x00045280 File Offset: 0x00044280
		public override long Length
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.Length;
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06000B40 RID: 2880 RVA: 0x000452B4 File Offset: 0x000442B4
		// (set) Token: 0x06000B41 RID: 2881 RVA: 0x000452E8 File Offset: 0x000442E8
		public override long Position
		{
			get
			{
				bool isDisposed = this.m_IsDisposed;
				if (isDisposed)
				{
					throw new ObjectDisposedException("SmartStream");
				}
				return this.m_pStream.Position;
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
				this.m_pStream.Position = value;
			}
		}

		// Token: 0x04000499 RID: 1177
		private static int m_DefaultMemorySize = 64000;

		// Token: 0x0400049A RID: 1178
		private bool m_IsDisposed = false;

		// Token: 0x0400049B RID: 1179
		private Stream m_pStream = null;

		// Token: 0x0400049C RID: 1180
		private int m_MaxMemSize = 64000;
	}
}
