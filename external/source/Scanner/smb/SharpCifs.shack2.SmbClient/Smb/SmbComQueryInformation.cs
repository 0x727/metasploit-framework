using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000097 RID: 151
	internal class SmbComQueryInformation : ServerMessageBlock
	{
		// Token: 0x06000451 RID: 1105 RVA: 0x00014B59 File Offset: 0x00012D59
		internal SmbComQueryInformation(string filename)
		{
			this.Path = filename;
			this.Command = 8;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00014B74 File Offset: 0x00012D74
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00014B88 File Offset: 0x00012D88
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 4;
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00014BBC File Offset: 0x00012DBC
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00014BD0 File Offset: 0x00012DD0
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00014BE4 File Offset: 0x00012DE4
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComQueryInformation[",
				base.ToString(),
				",filename=",
				this.Path,
				"]"
			});
		}
	}
}
