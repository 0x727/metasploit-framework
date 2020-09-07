using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C5 RID: 197
	internal class TransPeekNamedPipeResponse : SmbComTransactionResponse
	{
		// Token: 0x0600065B RID: 1627 RVA: 0x0002282F File Offset: 0x00020A2F
		internal TransPeekNamedPipeResponse(SmbNamedPipe pipe)
		{
			this._pipe = pipe;
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x00022840 File Offset: 0x00020A40
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x00022854 File Offset: 0x00020A54
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x00022868 File Offset: 0x00020A68
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0002287C File Offset: 0x00020A7C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x00022890 File Offset: 0x00020A90
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			this.Available = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this._head = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.status = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			return 6;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x000228D4 File Offset: 0x00020AD4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x000228E8 File Offset: 0x00020AE8
		public override string ToString()
		{
			return "TransPeekNamedPipeResponse[" + base.ToString() + "]";
		}

		// Token: 0x040003CF RID: 975
		private SmbNamedPipe _pipe;

		// Token: 0x040003D0 RID: 976
		private int _head;

		// Token: 0x040003D1 RID: 977
		internal const int StatusDisconnected = 1;

		// Token: 0x040003D2 RID: 978
		internal const int StatusListening = 2;

		// Token: 0x040003D3 RID: 979
		internal const int StatusConnectionOk = 3;

		// Token: 0x040003D4 RID: 980
		internal const int StatusServerEndClosed = 4;

		// Token: 0x040003D5 RID: 981
		internal int status;

		// Token: 0x040003D6 RID: 982
		internal int Available;
	}
}
