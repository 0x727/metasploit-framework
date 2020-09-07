using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000099 RID: 153
	internal class SmbComReadAndX : AndXServerMessageBlock
	{
		// Token: 0x06000461 RID: 1121 RVA: 0x00014DD3 File Offset: 0x00012FD3
		public SmbComReadAndX() : base(null)
		{
			this.Command = 46;
			this._openTimeout = -1;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00014DF0 File Offset: 0x00012FF0
		internal SmbComReadAndX(int fid, long offset, int maxCount, ServerMessageBlock andx) : base(andx)
		{
			this._fid = fid;
			this._offset = offset;
			this.MinCount = maxCount;
			this.MaxCount = maxCount;
			this.Command = 46;
			this._openTimeout = -1;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00014E34 File Offset: 0x00013034
		internal virtual void SetParam(int fid, long offset, int maxCount)
		{
			this._fid = fid;
			this._offset = offset;
			this.MinCount = maxCount;
			this.MaxCount = maxCount;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00014E60 File Offset: 0x00013060
		internal override int GetBatchLimit(byte command)
		{
			return (command == 4) ? SmbComReadAndX.BatchLimit : 0;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00014E80 File Offset: 0x00013080
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4(this._offset, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt2((long)this.MaxCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.MinCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this._openTimeout, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt2((long)this.Remaining, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4(this._offset >> 32, dst, dstIndex);
			dstIndex += 4;
			return dstIndex - num;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00014F24 File Offset: 0x00013124
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00014F38 File Offset: 0x00013138
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00014F4C File Offset: 0x0001314C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00014F60 File Offset: 0x00013160
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComReadAndX[",
				base.ToString(),
				",fid=",
				this._fid,
				",offset=",
				this._offset,
				",maxCount=",
				this.MaxCount,
				",minCount=",
				this.MinCount,
				",openTimeout=",
				this._openTimeout,
				",remaining=",
				this.Remaining,
				",offset=",
				this._offset,
				"]"
			});
		}

		// Token: 0x040001EC RID: 492
		private static readonly int BatchLimit = Config.GetInt("jcifs.smb.client.ReadAndX.Close", 1);

		// Token: 0x040001ED RID: 493
		private long _offset;

		// Token: 0x040001EE RID: 494
		private int _fid;

		// Token: 0x040001EF RID: 495
		private int _openTimeout;

		// Token: 0x040001F0 RID: 496
		internal int MaxCount;

		// Token: 0x040001F1 RID: 497
		internal int MinCount;

		// Token: 0x040001F2 RID: 498
		internal int Remaining;
	}
}
