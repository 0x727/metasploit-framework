using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000DB RID: 219
	internal class SessionRetargetResponsePacket : SessionServicePacket
	{
		// Token: 0x0600074F RID: 1871 RVA: 0x0002858E File Offset: 0x0002678E
		public SessionRetargetResponsePacket()
		{
			this.Type = 132;
			this.Length = 6;
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x000285AC File Offset: 0x000267AC
		internal override int WriteTrailerWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000285C0 File Offset: 0x000267C0
		internal override int ReadTrailerWireFormat(InputStream @in, byte[] buffer, int bufferIndex)
		{
			bool flag = @in.Read(buffer, bufferIndex, this.Length) != this.Length;
			if (flag)
			{
				throw new IOException("unexpected EOF reading netbios retarget session response");
			}
			int address = SessionServicePacket.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this._retargetAddress = new NbtAddress(null, address, false, 0);
			this._retargetPort = SessionServicePacket.ReadInt2(buffer, bufferIndex);
			return this.Length;
		}

		// Token: 0x040004AF RID: 1199
		private NbtAddress _retargetAddress;

		// Token: 0x040004B0 RID: 1200
		private int _retargetPort;
	}
}
