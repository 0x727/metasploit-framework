using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000A6 RID: 166
	internal class SmbComWriteResponse : ServerMessageBlock
	{
		// Token: 0x060004D3 RID: 1235 RVA: 0x00017624 File Offset: 0x00015824
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x00017638 File Offset: 0x00015838
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001764C File Offset: 0x0001584C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			this.Count = ((long)ServerMessageBlock.ReadInt2(buffer, bufferIndex) & 65535L);
			return 8;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00017674 File Offset: 0x00015874
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00017688 File Offset: 0x00015888
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComWriteResponse[",
				base.ToString(),
				",count=",
				this.Count,
				"]"
			});
		}

		// Token: 0x04000273 RID: 627
		internal long Count;
	}
}
