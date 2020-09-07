using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x020000BC RID: 188
	internal class Trans2QueryPathInformation : SmbComTransaction
	{
		// Token: 0x06000615 RID: 1557 RVA: 0x00021850 File Offset: 0x0001FA50
		internal Trans2QueryPathInformation(string filename, int informationLevel)
		{
			this.Path = filename;
			this._informationLevel = informationLevel;
			this.Command = 50;
			this.SubCommand = 5;
			this.TotalDataCount = 0;
			this.MaxParameterCount = 2;
			this.MaxDataCount = 40;
			this.MaxSetupCount = 0;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x000218A0 File Offset: 0x0001FAA0
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x000218CC File Offset: 0x0001FACC
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._informationLevel, dst, dstIndex);
			dstIndex += 2;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00021930 File Offset: 0x0001FB30
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00021944 File Offset: 0x0001FB44
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00021958 File Offset: 0x0001FB58
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002196C File Offset: 0x0001FB6C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00021980 File Offset: 0x0001FB80
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"Trans2QueryPathInformation[",
				base.ToString(),
				",informationLevel=0x",
				Hexdump.ToHexString(this._informationLevel, 3),
				",filename=",
				this.Path,
				"]"
			});
		}

		// Token: 0x040003B5 RID: 949
		private int _informationLevel;
	}
}
