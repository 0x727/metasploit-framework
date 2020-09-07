using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000088 RID: 136
	internal class SmbComBlankResponse : ServerMessageBlock
	{
		// Token: 0x060003FD RID: 1021 RVA: 0x00013074 File Offset: 0x00011274
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00013088 File Offset: 0x00011288
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0001309C File Offset: 0x0001129C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000130B0 File Offset: 0x000112B0
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x000130C4 File Offset: 0x000112C4
		public override string ToString()
		{
			return "SmbComBlankResponse[" + base.ToString() + "]";
		}
	}
}
