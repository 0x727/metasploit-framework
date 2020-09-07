using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C9 RID: 201
	internal class TransWaitNamedPipeResponse : SmbComTransactionResponse
	{
		// Token: 0x0600067B RID: 1659 RVA: 0x00022D28 File Offset: 0x00020F28
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00022D3C File Offset: 0x00020F3C
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00022D50 File Offset: 0x00020F50
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00022D64 File Offset: 0x00020F64
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x00022D78 File Offset: 0x00020F78
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00022D8C File Offset: 0x00020F8C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00022DA0 File Offset: 0x00020FA0
		public override string ToString()
		{
			return "TransWaitNamedPipeResponse[" + base.ToString() + "]";
		}
	}
}
