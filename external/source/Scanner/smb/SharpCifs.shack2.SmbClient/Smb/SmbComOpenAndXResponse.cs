using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000096 RID: 150
	internal class SmbComOpenAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x00014958 File Offset: 0x00012B58
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001496C File Offset: 0x00012B6C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00014980 File Offset: 0x00012B80
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.Fid = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.FileAttributes = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.LastWriteTime = ServerMessageBlock.ReadUTime(buffer, bufferIndex);
			bufferIndex += 4;
			this.DataSize = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.GrantedAccess = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.FileType = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DeviceState = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.Action = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.ServerFid = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 6;
			return bufferIndex - num;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00014A3C File Offset: 0x00012C3C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00014A50 File Offset: 0x00012C50
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComOpenAndXResponse[",
				base.ToString(),
				",fid=",
				this.Fid,
				",fileAttributes=",
				this.FileAttributes,
				",lastWriteTime=",
				this.LastWriteTime,
				",dataSize=",
				this.DataSize,
				",grantedAccess=",
				this.GrantedAccess,
				",fileType=",
				this.FileType,
				",deviceState=",
				this.DeviceState,
				",action=",
				this.Action,
				",serverFid=",
				this.ServerFid,
				"]"
			});
		}

		// Token: 0x040001DF RID: 479
		internal int Fid;

		// Token: 0x040001E0 RID: 480
		internal int FileAttributes;

		// Token: 0x040001E1 RID: 481
		internal int DataSize;

		// Token: 0x040001E2 RID: 482
		internal int GrantedAccess;

		// Token: 0x040001E3 RID: 483
		internal int FileType;

		// Token: 0x040001E4 RID: 484
		internal int DeviceState;

		// Token: 0x040001E5 RID: 485
		internal int Action;

		// Token: 0x040001E6 RID: 486
		internal int ServerFid;

		// Token: 0x040001E7 RID: 487
		internal long LastWriteTime;
	}
}
