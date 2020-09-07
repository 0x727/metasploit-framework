using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x0200008B RID: 139
	internal class SmbComDelete : ServerMessageBlock
	{
		// Token: 0x0600040F RID: 1039 RVA: 0x000132BB File Offset: 0x000114BB
		internal SmbComDelete(string fileName)
		{
			this.Path = fileName;
			this.Command = 6;
			this._searchAttributes = (SmbConstants.AttrHidden | SmbConstants.AttrHidden | SmbConstants.AttrSystem);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x000132EC File Offset: 0x000114EC
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			ServerMessageBlock.WriteInt2((long)this._searchAttributes, dst, dstIndex);
			return 2;
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00013310 File Offset: 0x00011510
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 4;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00013344 File Offset: 0x00011544
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00013358 File Offset: 0x00011558
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001336C File Offset: 0x0001156C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComDelete[",
				base.ToString(),
				",searchAttributes=0x",
				Hexdump.ToHexString(this._searchAttributes, 4),
				",fileName=",
				this.Path,
				"]"
			});
		}

		// Token: 0x04000198 RID: 408
		private int _searchAttributes;
	}
}
