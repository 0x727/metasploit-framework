using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LumiSoft.Net.IO
{
	// Token: 0x02000128 RID: 296
	public class JunkingStream : Stream
	{
		// Token: 0x06000BC2 RID: 3010 RVA: 0x000091B8 File Offset: 0x000081B8
		public override void Flush()
		{
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0004540D File Offset: 0x0004440D
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0004540D File Offset: 0x0004440D
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0004540D File Offset: 0x0004440D
		public override int Read([In] [Out] byte[] buffer, int offset, int size)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x000091B8 File Offset: 0x000081B8
		public override void Write(byte[] buffer, int offset, int size)
		{
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000BC7 RID: 3015 RVA: 0x00048038 File Offset: 0x00047038
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x0004804C File Offset: 0x0004704C
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000BC9 RID: 3017 RVA: 0x00048060 File Offset: 0x00047060
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000BCA RID: 3018 RVA: 0x0004540D File Offset: 0x0004440D
		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000BCB RID: 3019 RVA: 0x0004540D File Offset: 0x0004440D
		// (set) Token: 0x06000BCC RID: 3020 RVA: 0x0004540D File Offset: 0x0004440D
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
