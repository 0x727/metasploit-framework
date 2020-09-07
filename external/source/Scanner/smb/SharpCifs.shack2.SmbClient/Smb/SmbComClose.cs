using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000089 RID: 137
	internal class SmbComClose : ServerMessageBlock
	{
		// Token: 0x06000403 RID: 1027 RVA: 0x000130F4 File Offset: 0x000112F4
		internal SmbComClose(int fid, long lastWriteTime)
		{
			this._fid = fid;
			this._lastWriteTime = lastWriteTime;
			this.Command = 4;
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00013114 File Offset: 0x00011314
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteUTime(this._lastWriteTime, dst, dstIndex);
			return 6;
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001314C File Offset: 0x0001134C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00013160 File Offset: 0x00011360
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00013174 File Offset: 0x00011374
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00013188 File Offset: 0x00011388
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComClose[",
				base.ToString(),
				",fid=",
				this._fid,
				",lastWriteTime=",
				this._lastWriteTime,
				"]"
			});
		}

		// Token: 0x04000196 RID: 406
		private int _fid;

		// Token: 0x04000197 RID: 407
		private long _lastWriteTime;
	}
}
