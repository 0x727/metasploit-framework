using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200008D RID: 141
	internal class SmbComFindClose2 : ServerMessageBlock
	{
		// Token: 0x0600041B RID: 1051 RVA: 0x0001349B File Offset: 0x0001169B
		internal SmbComFindClose2(int sid)
		{
			this._sid = sid;
			this.Command = 52;
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x000134B4 File Offset: 0x000116B4
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			ServerMessageBlock.WriteInt2((long)this._sid, dst, dstIndex);
			return 2;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000134D8 File Offset: 0x000116D8
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000134EC File Offset: 0x000116EC
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00013500 File Offset: 0x00011700
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00013514 File Offset: 0x00011714
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComFindClose2[",
				base.ToString(),
				",sid=",
				this._sid,
				"]"
			});
		}

		// Token: 0x04000199 RID: 409
		private int _sid;
	}
}
