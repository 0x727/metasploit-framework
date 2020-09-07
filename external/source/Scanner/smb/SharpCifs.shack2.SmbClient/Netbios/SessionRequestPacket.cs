using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000DA RID: 218
	public class SessionRequestPacket : SessionServicePacket
	{
		// Token: 0x0600074B RID: 1867 RVA: 0x000284AB File Offset: 0x000266AB
		public SessionRequestPacket()
		{
			this._calledName = new Name();
			this._callingName = new Name();
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x000284CB File Offset: 0x000266CB
		public SessionRequestPacket(Name calledName, Name callingName)
		{
			this.Type = 129;
			this._calledName = calledName;
			this._callingName = callingName;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000284F0 File Offset: 0x000266F0
		internal override int WriteTrailerWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dstIndex += this._calledName.WriteWireFormat(dst, dstIndex);
			dstIndex += this._callingName.WriteWireFormat(dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0002852C File Offset: 0x0002672C
		internal override int ReadTrailerWireFormat(InputStream @in, byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			bool flag = @in.Read(buffer, bufferIndex, this.Length) != this.Length;
			if (flag)
			{
				throw new IOException("invalid session request wire format");
			}
			bufferIndex += this._calledName.ReadWireFormat(buffer, bufferIndex);
			bufferIndex += this._callingName.ReadWireFormat(buffer, bufferIndex);
			return bufferIndex - num;
		}

		// Token: 0x040004AD RID: 1197
		private Name _calledName;

		// Token: 0x040004AE RID: 1198
		private Name _callingName;
	}
}
