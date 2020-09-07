using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200008C RID: 140
	internal class SmbComDeleteDirectory : ServerMessageBlock
	{
		// Token: 0x06000415 RID: 1045 RVA: 0x000133CA File Offset: 0x000115CA
		internal SmbComDeleteDirectory(string directoryName)
		{
			this.Path = directoryName;
			this.Command = 1;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x000133E4 File Offset: 0x000115E4
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000133F8 File Offset: 0x000115F8
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 4;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x0001342C File Offset: 0x0001162C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00013440 File Offset: 0x00011640
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00013454 File Offset: 0x00011654
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComDeleteDirectory[",
				base.ToString(),
				",directoryName=",
				this.Path,
				"]"
			});
		}
	}
}
