using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000A3 RID: 163
	internal class SmbComWrite : ServerMessageBlock
	{
		// Token: 0x060004BB RID: 1211 RVA: 0x00017007 File Offset: 0x00015207
		public SmbComWrite()
		{
			this.Command = 11;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00017019 File Offset: 0x00015219
		internal SmbComWrite(int fid, int offset, int remaining, byte[] b, int off, int len)
		{
			this._fid = fid;
			this._count = len;
			this._offset = offset;
			this._remaining = remaining;
			this._b = b;
			this._off = off;
			this.Command = 11;
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x00017058 File Offset: 0x00015258
		internal virtual void SetParam(int fid, long offset, int remaining, byte[] b, int off, int len)
		{
			this._fid = fid;
			this._offset = (int)(offset & (long)(-1));
			this._remaining = remaining;
			this._b = b;
			this._off = off;
			this._count = len;
			this.Digest = null;
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x00017094 File Offset: 0x00015294
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._count, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this._offset, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt2((long)this._remaining, dst, dstIndex);
			dstIndex += 2;
			return dstIndex - num;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x000170FC File Offset: 0x000152FC
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 1;
			ServerMessageBlock.WriteInt2((long)this._count, dst, dstIndex);
			dstIndex += 2;
			Array.Copy(this._b, this._off, dst, dstIndex, this._count);
			dstIndex += this._count;
			return dstIndex - num;
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x00017154 File Offset: 0x00015354
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x00017168 File Offset: 0x00015368
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001717C File Offset: 0x0001537C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComWrite[",
				base.ToString(),
				",fid=",
				this._fid,
				",count=",
				this._count,
				",offset=",
				this._offset,
				",remaining=",
				this._remaining,
				"]"
			});
		}

		// Token: 0x04000261 RID: 609
		private int _fid;

		// Token: 0x04000262 RID: 610
		private int _count;

		// Token: 0x04000263 RID: 611
		private int _offset;

		// Token: 0x04000264 RID: 612
		private int _remaining;

		// Token: 0x04000265 RID: 613
		private int _off;

		// Token: 0x04000266 RID: 614
		private byte[] _b;
	}
}
