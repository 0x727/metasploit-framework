using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200009A RID: 154
	internal class SmbComReadAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x0600046B RID: 1131 RVA: 0x0001504B File Offset: 0x0001324B
		public SmbComReadAndXResponse()
		{
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00015055 File Offset: 0x00013255
		internal SmbComReadAndXResponse(byte[] b, int off)
		{
			this.B = b;
			this.Off = off;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001506D File Offset: 0x0001326D
		internal virtual void SetParam(byte[] b, int off)
		{
			this.B = b;
			this.Off = off;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00015080 File Offset: 0x00013280
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00015094 File Offset: 0x00013294
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x000150A8 File Offset: 0x000132A8
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			bufferIndex += 2;
			this.DataCompactionMode = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 4;
			this.DataLength = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DataOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 12;
			return bufferIndex - num;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x000150FC File Offset: 0x000132FC
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00015110 File Offset: 0x00013310
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComReadAndXResponse[",
				base.ToString(),
				",dataCompactionMode=",
				this.DataCompactionMode,
				",dataLength=",
				this.DataLength,
				",dataOffset=",
				this.DataOffset,
				"]"
			});
		}

		// Token: 0x040001F3 RID: 499
		internal byte[] B;

		// Token: 0x040001F4 RID: 500
		internal int Off;

		// Token: 0x040001F5 RID: 501
		internal int DataCompactionMode;

		// Token: 0x040001F6 RID: 502
		internal int DataLength;

		// Token: 0x040001F7 RID: 503
		internal int DataOffset;
	}
}
