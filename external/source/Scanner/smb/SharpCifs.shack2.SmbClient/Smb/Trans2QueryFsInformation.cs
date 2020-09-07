using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x020000BA RID: 186
	internal class Trans2QueryFsInformation : SmbComTransaction
	{
		// Token: 0x06000602 RID: 1538 RVA: 0x000214A4 File Offset: 0x0001F6A4
		internal Trans2QueryFsInformation(int informationLevel)
		{
			this.Command = 50;
			this.SubCommand = 3;
			this._informationLevel = informationLevel;
			this.TotalParameterCount = 2;
			this.TotalDataCount = 0;
			this.MaxParameterCount = 0;
			this.MaxDataCount = 800;
			this.MaxSetupCount = 0;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x000214F8 File Offset: 0x0001F6F8
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00021524 File Offset: 0x0001F724
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._informationLevel, dst, dstIndex);
			dstIndex += 2;
			return dstIndex - num;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00021550 File Offset: 0x0001F750
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00021564 File Offset: 0x0001F764
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00021578 File Offset: 0x0001F778
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0002158C File Offset: 0x0001F78C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x000215A0 File Offset: 0x0001F7A0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Trans2QueryFSInformation[",
				base.ToString(),
				",informationLevel=0x",
				Hexdump.ToHexString(this._informationLevel, 3),
				"]"
			});
		}

		// Token: 0x040003AF RID: 943
		private int _informationLevel;
	}
}
