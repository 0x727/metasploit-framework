using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000A4 RID: 164
	internal class SmbComWriteAndX : AndXServerMessageBlock
	{
		// Token: 0x060004C3 RID: 1219 RVA: 0x0001720D File Offset: 0x0001540D
		public SmbComWriteAndX() : base(null)
		{
			this.Command = 47;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00017220 File Offset: 0x00015420
		internal SmbComWriteAndX(int fid, long offset, int remaining, byte[] b, int off, int len, ServerMessageBlock andx) : base(andx)
		{
			this._fid = fid;
			this._offset = offset;
			this._remaining = remaining;
			this._b = b;
			this._off = off;
			this._dataLength = len;
			this.Command = 47;
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001726C File Offset: 0x0001546C
		internal virtual void SetParam(int fid, long offset, int remaining, byte[] b, int off, int len)
		{
			this._fid = fid;
			this._offset = offset;
			this._remaining = remaining;
			this._b = b;
			this._off = off;
			this._dataLength = len;
			this.Digest = null;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000172A4 File Offset: 0x000154A4
		internal override int GetBatchLimit(byte command)
		{
			bool flag = command == 46;
			int result;
			if (flag)
			{
				result = SmbComWriteAndX.ReadAndxBatchLimit;
			}
			else
			{
				bool flag2 = command == 4;
				if (flag2)
				{
					result = SmbComWriteAndX.CloseBatchLimit;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x000172DC File Offset: 0x000154DC
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			this._dataOffset = dstIndex - this.HeaderStart + 26;
			this._pad = (this._dataOffset - this.HeaderStart) % 4;
			this._pad = ((this._pad == 0) ? 0 : (4 - this._pad));
			this._dataOffset += this._pad;
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4(this._offset, dst, dstIndex);
			dstIndex += 4;
			for (int i = 0; i < 4; i++)
			{
				dst[dstIndex++] = byte.MaxValue;
			}
			ServerMessageBlock.WriteInt2((long)this.WriteMode, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._remaining, dst, dstIndex);
			dstIndex += 2;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt2((long)this._dataLength, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._dataOffset, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4(this._offset >> 32, dst, dstIndex);
			dstIndex += 4;
			return dstIndex - num;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00017404 File Offset: 0x00015604
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			for (;;)
			{
				int pad = this._pad;
				this._pad = pad - 1;
				if (pad <= 0)
				{
					break;
				}
				dst[dstIndex++] = 238;
			}
			Array.Copy(this._b, this._off, dst, dstIndex, this._dataLength);
			dstIndex += this._dataLength;
			return dstIndex - num;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00017468 File Offset: 0x00015668
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001747C File Offset: 0x0001567C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00017490 File Offset: 0x00015690
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComWriteAndX[",
				base.ToString(),
				",fid=",
				this._fid,
				",offset=",
				this._offset,
				",writeMode=",
				this.WriteMode,
				",remaining=",
				this._remaining,
				",dataLength=",
				this._dataLength,
				",dataOffset=",
				this._dataOffset,
				"]"
			});
		}

		// Token: 0x04000267 RID: 615
		private static readonly int ReadAndxBatchLimit = Config.GetInt("jcifs.smb.client.WriteAndX.ReadAndX", 1);

		// Token: 0x04000268 RID: 616
		private static readonly int CloseBatchLimit = Config.GetInt("jcifs.smb.client.WriteAndX.Close", 1);

		// Token: 0x04000269 RID: 617
		private int _fid;

		// Token: 0x0400026A RID: 618
		private int _remaining;

		// Token: 0x0400026B RID: 619
		private int _dataLength;

		// Token: 0x0400026C RID: 620
		private int _dataOffset;

		// Token: 0x0400026D RID: 621
		private int _off;

		// Token: 0x0400026E RID: 622
		private byte[] _b;

		// Token: 0x0400026F RID: 623
		private long _offset;

		// Token: 0x04000270 RID: 624
		private int _pad;

		// Token: 0x04000271 RID: 625
		internal int WriteMode;
	}
}
