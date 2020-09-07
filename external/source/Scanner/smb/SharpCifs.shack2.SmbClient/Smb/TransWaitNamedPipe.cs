using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C8 RID: 200
	internal class TransWaitNamedPipe : SmbComTransaction
	{
		// Token: 0x06000673 RID: 1651 RVA: 0x00022BF0 File Offset: 0x00020DF0
		internal TransWaitNamedPipe(string pipeName)
		{
			this.Name = pipeName;
			this.Command = 37;
			this.SubCommand = 83;
			this.Timeout = -1;
			this.MaxParameterCount = 0;
			this.MaxDataCount = 0;
			this.MaxSetupCount = 0;
			this.SetupCount = 2;
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00022C40 File Offset: 0x00020E40
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			return 4;
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00022C7C File Offset: 0x00020E7C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00022C90 File Offset: 0x00020E90
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00022CA4 File Offset: 0x00020EA4
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00022CB8 File Offset: 0x00020EB8
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00022CCC File Offset: 0x00020ECC
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00022CE0 File Offset: 0x00020EE0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"TransWaitNamedPipe[",
				base.ToString(),
				",pipeName=",
				this.Name,
				"]"
			});
		}
	}
}
