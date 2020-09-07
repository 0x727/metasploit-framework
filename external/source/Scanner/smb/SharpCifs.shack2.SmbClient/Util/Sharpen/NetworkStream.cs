using System;
using System.IO;

namespace SharpCifs.Util.Sharpen
{
	// Token: 0x02000056 RID: 86
	public class NetworkStream : Stream
	{
		// Token: 0x0600021F RID: 543 RVA: 0x00009FEC File Offset: 0x000081EC
		public NetworkStream(SocketEx socket)
		{
			this._socket = socket;
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00002380 File Offset: 0x00000580
		public override bool CanRead
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000221 RID: 545 RVA: 0x00002380 File Offset: 0x00000580
		public override bool CanSeek
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00002380 File Offset: 0x00000580
		public override bool CanWrite
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00008663 File Offset: 0x00006863
		public override void Flush()
		{
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00002380 File Offset: 0x00000580
		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00002380 File Offset: 0x00000580
		// (set) Token: 0x06000226 RID: 550 RVA: 0x00002380 File Offset: 0x00000580
		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000A000 File Offset: 0x00008200
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this._socket.Receive(buffer, offset, count);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00002380 File Offset: 0x00000580
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00002380 File Offset: 0x00000580
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000A020 File Offset: 0x00008220
		public override void Write(byte[] buffer, int offset, int count)
		{
			this._socket.Send(buffer, offset, count, null);
		}

		// Token: 0x0400006C RID: 108
		private SocketEx _socket;
	}
}
