using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000B8 RID: 184
	internal class Trans2GetDfsReferral : SmbComTransaction
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x00021188 File Offset: 0x0001F388
		internal Trans2GetDfsReferral(string filename)
		{
			this.Path = filename;
			this.Command = 50;
			this.SubCommand = 16;
			this.TotalDataCount = 0;
			this.MaxParameterCount = 0;
			this.MaxDataCount = 4096;
			this.MaxSetupCount = 0;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000211DC File Offset: 0x0001F3DC
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00021208 File Offset: 0x0001F408
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._maxReferralLevel, dst, dstIndex);
			dstIndex += 2;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x00021248 File Offset: 0x0001F448
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0002125C File Offset: 0x0001F45C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00021270 File Offset: 0x0001F470
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00021284 File Offset: 0x0001F484
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00021298 File Offset: 0x0001F498
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trans2GetDfsReferral[",
				base.ToString(),
				",maxReferralLevel=0x",
				this._maxReferralLevel,
				",filename=",
				this.Path,
				"]"
			});
		}

		// Token: 0x040003AA RID: 938
		private int _maxReferralLevel = 3;
	}
}
