using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000A5 RID: 165
	internal class SmbComWriteAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x060004CD RID: 1229 RVA: 0x00017574 File Offset: 0x00015774
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00017588 File Offset: 0x00015788
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001759C File Offset: 0x0001579C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			this.Count = ((long)ServerMessageBlock.ReadInt2(buffer, bufferIndex) & 65535L);
			return 8;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x000175C4 File Offset: 0x000157C4
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x000175D8 File Offset: 0x000157D8
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComWriteAndXResponse[",
				base.ToString(),
				",count=",
				this.Count,
				"]"
			});
		}

		// Token: 0x04000272 RID: 626
		internal long Count;
	}
}
