using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000BB RID: 187
	internal class Trans2QueryFsInformationResponse : SmbComTransactionResponse
	{
		// Token: 0x0600060A RID: 1546 RVA: 0x000215ED File Offset: 0x0001F7ED
		internal Trans2QueryFsInformationResponse(int informationLevel)
		{
			this._informationLevel = informationLevel;
			this.Command = 50;
			this.SubCommand = 3;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00021610 File Offset: 0x0001F810
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00021624 File Offset: 0x0001F824
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00021638 File Offset: 0x0001F838
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0002164C File Offset: 0x0001F84C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00021660 File Offset: 0x0001F860
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00021674 File Offset: 0x0001F874
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int informationLevel = this._informationLevel;
			int result;
			if (informationLevel != 1)
			{
				if (informationLevel != 259)
				{
					if (informationLevel != 1007)
					{
						result = 0;
					}
					else
					{
						result = this.ReadFsFullSizeInformationWireFormat(buffer, bufferIndex);
					}
				}
				else
				{
					result = this.ReadSmbQueryFsSizeInfoWireFormat(buffer, bufferIndex);
				}
			}
			else
			{
				result = this.ReadSmbInfoAllocationWireFormat(buffer, bufferIndex);
			}
			return result;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000216D0 File Offset: 0x0001F8D0
		internal virtual int ReadSmbInfoAllocationWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			Trans2QueryFsInformationResponse.SmbInfoAllocation smbInfoAllocation = new Trans2QueryFsInformationResponse.SmbInfoAllocation(this);
			bufferIndex += 4;
			smbInfoAllocation.SectPerAlloc = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbInfoAllocation.Alloc = (long)ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbInfoAllocation.Free = (long)ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbInfoAllocation.BytesPerSect = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 4;
			this.Info = smbInfoAllocation;
			return bufferIndex - num;
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00021744 File Offset: 0x0001F944
		internal virtual int ReadSmbQueryFsSizeInfoWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			Trans2QueryFsInformationResponse.SmbInfoAllocation smbInfoAllocation = new Trans2QueryFsInformationResponse.SmbInfoAllocation(this);
			smbInfoAllocation.Alloc = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			smbInfoAllocation.Free = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			smbInfoAllocation.SectPerAlloc = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbInfoAllocation.BytesPerSect = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.Info = smbInfoAllocation;
			return bufferIndex - num;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x000217B4 File Offset: 0x0001F9B4
		internal virtual int ReadFsFullSizeInformationWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			Trans2QueryFsInformationResponse.SmbInfoAllocation smbInfoAllocation = new Trans2QueryFsInformationResponse.SmbInfoAllocation(this);
			smbInfoAllocation.Alloc = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			smbInfoAllocation.Free = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			bufferIndex += 8;
			smbInfoAllocation.SectPerAlloc = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbInfoAllocation.BytesPerSect = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.Info = smbInfoAllocation;
			return bufferIndex - num;
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00021828 File Offset: 0x0001FA28
		public override string ToString()
		{
			return "Trans2QueryFSInformationResponse[" + base.ToString() + "]";
		}

		// Token: 0x040003B0 RID: 944
		internal const int SMB_INFO_ALLOCATION = 1;

		// Token: 0x040003B1 RID: 945
		internal const int SmbQueryFsSizeInfo = 259;

		// Token: 0x040003B2 RID: 946
		internal const int SmbFsFullSizeInformation = 1007;

		// Token: 0x040003B3 RID: 947
		private int _informationLevel;

		// Token: 0x040003B4 RID: 948
		internal IAllocInfo Info;

		// Token: 0x02000126 RID: 294
		internal class SmbInfoAllocation : IAllocInfo
		{
			// Token: 0x06000840 RID: 2112 RVA: 0x0002BAD8 File Offset: 0x00029CD8
			public virtual long GetCapacity()
			{
				return this.Alloc * (long)this.SectPerAlloc * (long)this.BytesPerSect;
			}

			// Token: 0x06000841 RID: 2113 RVA: 0x0002BB00 File Offset: 0x00029D00
			public virtual long GetFree()
			{
				return this.Free * (long)this.SectPerAlloc * (long)this.BytesPerSect;
			}

			// Token: 0x06000842 RID: 2114 RVA: 0x0002BB28 File Offset: 0x00029D28
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"SmbInfoAllocation[alloc=",
					this.Alloc,
					",free=",
					this.Free,
					",sectPerAlloc=",
					this.SectPerAlloc,
					",bytesPerSect=",
					this.BytesPerSect,
					"]"
				});
			}

			// Token: 0x06000843 RID: 2115 RVA: 0x0002BBA6 File Offset: 0x00029DA6
			internal SmbInfoAllocation(Trans2QueryFsInformationResponse enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x040005AD RID: 1453
			internal long Alloc;

			// Token: 0x040005AE RID: 1454
			internal long Free;

			// Token: 0x040005AF RID: 1455
			internal int SectPerAlloc;

			// Token: 0x040005B0 RID: 1456
			internal int BytesPerSect;

			// Token: 0x040005B1 RID: 1457
			private readonly Trans2QueryFsInformationResponse _enclosing;
		}
	}
}
