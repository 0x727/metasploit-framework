using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000BF RID: 191
	internal class Trans2SetFileInformationResponse : SmbComTransactionResponse
	{
		// Token: 0x0600062F RID: 1583 RVA: 0x00021DB8 File Offset: 0x0001FFB8
		public Trans2SetFileInformationResponse()
		{
			this.SubCommand = 8;
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00021DCC File Offset: 0x0001FFCC
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00021DE0 File Offset: 0x0001FFE0
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00021DF4 File Offset: 0x0001FFF4
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00021E08 File Offset: 0x00020008
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00021E1C File Offset: 0x0002001C
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00021E30 File Offset: 0x00020030
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00021E44 File Offset: 0x00020044
		public override string ToString()
		{
			return "Trans2SetFileInformationResponse[" + base.ToString() + "]";
		}
	}
}
