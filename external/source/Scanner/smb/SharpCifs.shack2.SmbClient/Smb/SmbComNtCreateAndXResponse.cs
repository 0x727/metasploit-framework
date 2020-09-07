using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000092 RID: 146
	internal class SmbComNtCreateAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x06000439 RID: 1081 RVA: 0x00014038 File Offset: 0x00012238
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0001404C File Offset: 0x0001224C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00014060 File Offset: 0x00012260
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.OplockLevel = buffer[bufferIndex++];
			this.Fid = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.CreateAction = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.CreationTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			this.LastAccessTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			this.LastWriteTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			this.ChangeTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			this.ExtFileAttributes = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.AllocationSize = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			this.EndOfFile = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			this.FileType = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DeviceState = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.Directory = ((buffer[bufferIndex++] & byte.MaxValue) > 0);
			return bufferIndex - num;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00014164 File Offset: 0x00012364
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00014178 File Offset: 0x00012378
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComNTCreateAndXResponse[",
				base.ToString(),
				",oplockLevel=",
				this.OplockLevel,
				",fid=",
				this.Fid,
				",createAction=0x",
				Hexdump.ToHexString(this.CreateAction, 4),
				",creationTime=",
				Extensions.CreateDate(this.CreationTime),
				",lastAccessTime=",
				Extensions.CreateDate(this.LastAccessTime),
				",lastWriteTime=",
				Extensions.CreateDate(this.LastWriteTime),
				",changeTime=",
				Extensions.CreateDate(this.ChangeTime),
				",extFileAttributes=0x",
				Hexdump.ToHexString(this.ExtFileAttributes, 4),
				",allocationSize=",
				this.AllocationSize,
				",endOfFile=",
				this.EndOfFile,
				",fileType=",
				this.FileType,
				",deviceState=",
				this.DeviceState,
				",directory=",
				this.Directory.ToString(),
				"]"
			});
		}

		// Token: 0x040001B4 RID: 436
		internal const int ExclusiveOplockGranted = 1;

		// Token: 0x040001B5 RID: 437
		internal const int BatchOplockGranted = 2;

		// Token: 0x040001B6 RID: 438
		internal const int LevelIiOplockGranted = 3;

		// Token: 0x040001B7 RID: 439
		internal byte OplockLevel;

		// Token: 0x040001B8 RID: 440
		internal int Fid;

		// Token: 0x040001B9 RID: 441
		internal int CreateAction;

		// Token: 0x040001BA RID: 442
		internal int ExtFileAttributes;

		// Token: 0x040001BB RID: 443
		internal int FileType;

		// Token: 0x040001BC RID: 444
		internal int DeviceState;

		// Token: 0x040001BD RID: 445
		internal long CreationTime;

		// Token: 0x040001BE RID: 446
		internal long LastAccessTime;

		// Token: 0x040001BF RID: 447
		internal long LastWriteTime;

		// Token: 0x040001C0 RID: 448
		internal long ChangeTime;

		// Token: 0x040001C1 RID: 449
		internal long AllocationSize;

		// Token: 0x040001C2 RID: 450
		internal long EndOfFile;

		// Token: 0x040001C3 RID: 451
		internal bool Directory;

		// Token: 0x040001C4 RID: 452
		internal bool IsExtended;
	}
}
