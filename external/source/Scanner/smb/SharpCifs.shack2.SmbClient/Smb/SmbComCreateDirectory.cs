using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200008A RID: 138
	internal class SmbComCreateDirectory : ServerMessageBlock
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x000131EA File Offset: 0x000113EA
		internal SmbComCreateDirectory(string directoryName)
		{
			this.Path = directoryName;
			this.Command = 0;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00013204 File Offset: 0x00011404
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00013218 File Offset: 0x00011418
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 4;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0001324C File Offset: 0x0001144C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00013260 File Offset: 0x00011460
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00013274 File Offset: 0x00011474
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComCreateDirectory[",
				base.ToString(),
				",directoryName=",
				this.Path,
				"]"
			});
		}
	}
}
