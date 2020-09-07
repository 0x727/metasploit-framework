using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C4 RID: 196
	internal class TransPeekNamedPipe : SmbComTransaction
	{
		// Token: 0x06000653 RID: 1619 RVA: 0x000226F0 File Offset: 0x000208F0
		internal TransPeekNamedPipe(string pipeName, int fid)
		{
			this.Name = pipeName;
			this._fid = fid;
			this.Command = 37;
			this.SubCommand = 35;
			this.Timeout = -1;
			this.MaxParameterCount = 6;
			this.MaxDataCount = 1;
			this.MaxSetupCount = 0;
			this.SetupCount = 2;
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00022748 File Offset: 0x00020948
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			return 4;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00022784 File Offset: 0x00020984
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00022798 File Offset: 0x00020998
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x000227AC File Offset: 0x000209AC
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x000227C0 File Offset: 0x000209C0
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x000227D4 File Offset: 0x000209D4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x000227E8 File Offset: 0x000209E8
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"TransPeekNamedPipe[",
				base.ToString(),
				",pipeName=",
				this.Name,
				"]"
			});
		}

		// Token: 0x040003CE RID: 974
		private int _fid;
	}
}
