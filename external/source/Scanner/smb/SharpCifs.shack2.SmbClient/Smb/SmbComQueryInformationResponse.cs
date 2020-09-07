using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000098 RID: 152
	internal class SmbComQueryInformationResponse : ServerMessageBlock, IInfo
	{
		// Token: 0x06000457 RID: 1111 RVA: 0x00014C2B File Offset: 0x00012E2B
		internal SmbComQueryInformationResponse(long serverTimeZoneOffset)
		{
			this._serverTimeZoneOffset = serverTimeZoneOffset;
			this.Command = 8;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00014C4C File Offset: 0x00012E4C
		public virtual int GetAttributes()
		{
			return this._fileAttributes;
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00014C64 File Offset: 0x00012E64
		public virtual long GetCreateTime()
		{
			return this._lastWriteTime + this._serverTimeZoneOffset;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00014C84 File Offset: 0x00012E84
		public virtual long GetLastWriteTime()
		{
			return this._lastWriteTime + this._serverTimeZoneOffset;
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00014CA4 File Offset: 0x00012EA4
		public virtual long GetSize()
		{
			return (long)this._fileSize;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00014CC0 File Offset: 0x00012EC0
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00014CD4 File Offset: 0x00012ED4
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00014CE8 File Offset: 0x00012EE8
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			bool flag = this.WordCount == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				this._fileAttributes = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this._lastWriteTime = ServerMessageBlock.ReadUTime(buffer, bufferIndex);
				bufferIndex += 4;
				this._fileSize = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				result = 20;
			}
			return result;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00014D40 File Offset: 0x00012F40
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00014D54 File Offset: 0x00012F54
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComQueryInformationResponse[",
				base.ToString(),
				",fileAttributes=0x",
				Hexdump.ToHexString(this._fileAttributes, 4),
				",lastWriteTime=",
				Extensions.CreateDate(this._lastWriteTime),
				",fileSize=",
				this._fileSize,
				"]"
			});
		}

		// Token: 0x040001E8 RID: 488
		private int _fileAttributes = 0;

		// Token: 0x040001E9 RID: 489
		private long _lastWriteTime;

		// Token: 0x040001EA RID: 490
		private long _serverTimeZoneOffset;

		// Token: 0x040001EB RID: 491
		private int _fileSize;
	}
}
