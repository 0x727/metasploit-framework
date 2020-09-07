using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200008E RID: 142
	internal class SmbComLogoffAndX : AndXServerMessageBlock
	{
		// Token: 0x06000421 RID: 1057 RVA: 0x00013560 File Offset: 0x00011760
		internal SmbComLogoffAndX(ServerMessageBlock andx) : base(andx)
		{
			this.Command = 116;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00013574 File Offset: 0x00011774
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00013588 File Offset: 0x00011788
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001359C File Offset: 0x0001179C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000135B0 File Offset: 0x000117B0
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000135C4 File Offset: 0x000117C4
		public override string ToString()
		{
			return "SmbComLogoffAndX[" + base.ToString() + "]";
		}
	}
}
