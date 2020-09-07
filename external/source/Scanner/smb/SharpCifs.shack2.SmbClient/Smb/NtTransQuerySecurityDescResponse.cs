using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000081 RID: 129
	internal class NtTransQuerySecurityDescResponse : SmbComNtTransactionResponse
	{
		// Token: 0x060003AC RID: 940 RVA: 0x00010554 File Offset: 0x0000E754
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00010568 File Offset: 0x0000E768
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0001057C File Offset: 0x0000E77C
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00010590 File Offset: 0x0000E790
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000105A4 File Offset: 0x0000E7A4
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			this.Length = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			return 4;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000105C4 File Offset: 0x0000E7C4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			bool flag = this.ErrorCode != 0;
			int result;
			if (flag)
			{
				result = 4;
			}
			else
			{
				try
				{
					this.SecurityDescriptor = new SecurityDescriptor();
					bufferIndex += this.SecurityDescriptor.Decode(buffer, bufferIndex, len);
				}
				catch (IOException ex)
				{
					throw new RuntimeException(ex.Message);
				}
				result = bufferIndex - num;
			}
			return result;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00010630 File Offset: 0x0000E830
		public override string ToString()
		{
			return "NtTransQuerySecurityResponse[" + base.ToString() + "]";
		}

		// Token: 0x04000145 RID: 325
		internal SecurityDescriptor SecurityDescriptor;
	}
}
