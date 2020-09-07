using System;
using System.IO;

namespace LumiSoft.Net.IO
{
	// Token: 0x0200011F RID: 287
	public class ReadWriteControlledStream : Stream
	{
		// Token: 0x06000B4F RID: 2895 RVA: 0x000459C0 File Offset: 0x000449C0
		public ReadWriteControlledStream(Stream stream, FileAccess access)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.m_pStream = stream;
			this.m_AccessMode = access;
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x00045A05 File Offset: 0x00044A05
		public override void Flush()
		{
			this.m_pStream.Flush();
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x00045A14 File Offset: 0x00044A14
		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.m_pStream.Seek(offset, origin);
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x00045A33 File Offset: 0x00044A33
		public override void SetLength(long value)
		{
			this.m_pStream.SetLength(value);
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x00045A44 File Offset: 0x00044A44
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'offset' value.");
			}
			bool flag3 = offset + count > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'count' value.");
			}
			bool flag4 = (this.m_AccessMode & FileAccess.Read) == (FileAccess)0;
			if (flag4)
			{
				throw new NotSupportedException();
			}
			return this.m_pStream.Read(buffer, offset, count);
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x00045AC8 File Offset: 0x00044AC8
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0 || offset > buffer.Length;
			if (flag2)
			{
				throw new ArgumentException("Invalid argument 'offset' value.");
			}
			bool flag3 = offset + count > buffer.Length;
			if (flag3)
			{
				throw new ArgumentException("Invalid argument 'count' value.");
			}
			bool flag4 = (this.m_AccessMode & FileAccess.Write) == (FileAccess)0;
			if (flag4)
			{
				throw new NotSupportedException();
			}
			this.m_pStream.Write(buffer, offset, count);
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06000B55 RID: 2901 RVA: 0x00045B44 File Offset: 0x00044B44
		public override bool CanRead
		{
			get
			{
				return (this.m_AccessMode & FileAccess.Read) > (FileAccess)0;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00045B64 File Offset: 0x00044B64
		public override bool CanSeek
		{
			get
			{
				return this.m_pStream.CanSeek;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06000B57 RID: 2903 RVA: 0x00045B84 File Offset: 0x00044B84
		public override bool CanWrite
		{
			get
			{
				return (this.m_AccessMode & FileAccess.Write) > (FileAccess)0;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06000B58 RID: 2904 RVA: 0x00045BA4 File Offset: 0x00044BA4
		public override long Length
		{
			get
			{
				return this.m_pStream.Length;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000B59 RID: 2905 RVA: 0x00045BC4 File Offset: 0x00044BC4
		// (set) Token: 0x06000B5A RID: 2906 RVA: 0x00045BE1 File Offset: 0x00044BE1
		public override long Position
		{
			get
			{
				return this.m_pStream.Position;
			}
			set
			{
				this.m_pStream.Position = value;
			}
		}

		// Token: 0x040004A4 RID: 1188
		private Stream m_pStream = null;

		// Token: 0x040004A5 RID: 1189
		private FileAccess m_AccessMode = FileAccess.ReadWrite;
	}
}
