using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000BE RID: 190
	internal class Trans2SetFileInformation : SmbComTransaction
	{
		// Token: 0x06000627 RID: 1575 RVA: 0x00021BD8 File Offset: 0x0001FDD8
		internal Trans2SetFileInformation(int fid, int attributes, long createTime, long lastWriteTime)
		{
			this._fid = fid;
			this._attributes = attributes;
			this._createTime = createTime;
			this._lastWriteTime = lastWriteTime;
			this.Command = 50;
			this.SubCommand = 8;
			this.MaxParameterCount = 6;
			this.MaxDataCount = 0;
			this.MaxSetupCount = 0;
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00021C30 File Offset: 0x0001FE30
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00021C5C File Offset: 0x0001FE5C
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._fid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2(257L, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2(0L, dst, dstIndex);
			dstIndex += 2;
			return dstIndex - num;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00021CAC File Offset: 0x0001FEAC
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteTime(this._createTime, dst, dstIndex);
			dstIndex += 8;
			ServerMessageBlock.WriteInt8(0L, dst, dstIndex);
			dstIndex += 8;
			ServerMessageBlock.WriteTime(this._lastWriteTime, dst, dstIndex);
			dstIndex += 8;
			ServerMessageBlock.WriteInt8(0L, dst, dstIndex);
			dstIndex += 8;
			ServerMessageBlock.WriteInt2((long)(128 | this._attributes), dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt8(0L, dst, dstIndex);
			dstIndex += 6;
			return dstIndex - num;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00021D30 File Offset: 0x0001FF30
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00021D44 File Offset: 0x0001FF44
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00021D58 File Offset: 0x0001FF58
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00021D6C File Offset: 0x0001FF6C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trans2SetFileInformation[",
				base.ToString(),
				",fid=",
				this._fid,
				"]"
			});
		}

		// Token: 0x040003BA RID: 954
		internal const int SmbFileBasicInfo = 257;

		// Token: 0x040003BB RID: 955
		private int _fid;

		// Token: 0x040003BC RID: 956
		private int _attributes;

		// Token: 0x040003BD RID: 957
		private long _createTime;

		// Token: 0x040003BE RID: 958
		private long _lastWriteTime;
	}
}
