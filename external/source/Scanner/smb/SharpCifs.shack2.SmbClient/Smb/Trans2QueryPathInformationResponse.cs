using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000BD RID: 189
	internal class Trans2QueryPathInformationResponse : SmbComTransactionResponse
	{
		// Token: 0x0600061D RID: 1565 RVA: 0x000219DE File Offset: 0x0001FBDE
		internal Trans2QueryPathInformationResponse(int informationLevel)
		{
			this._informationLevel = informationLevel;
			this.SubCommand = 5;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x000219F8 File Offset: 0x0001FBF8
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00021A0C File Offset: 0x0001FC0C
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00021A20 File Offset: 0x0001FC20
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00021A34 File Offset: 0x0001FC34
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x00021A48 File Offset: 0x0001FC48
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 2;
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00021A5C File Offset: 0x0001FC5C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int informationLevel = this._informationLevel;
			int result;
			if (informationLevel != 257)
			{
				if (informationLevel != 258)
				{
					result = 0;
				}
				else
				{
					result = this.ReadSmbQueryFileStandardInfoWireFormat(buffer, bufferIndex);
				}
			}
			else
			{
				result = this.ReadSmbQueryFileBasicInfoWireFormat(buffer, bufferIndex);
			}
			return result;
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00021AA4 File Offset: 0x0001FCA4
		internal virtual int ReadSmbQueryFileStandardInfoWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			Trans2QueryPathInformationResponse.SmbQueryFileStandardInfo smbQueryFileStandardInfo = new Trans2QueryPathInformationResponse.SmbQueryFileStandardInfo(this);
			smbQueryFileStandardInfo.AllocationSize = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileStandardInfo.EndOfFile = ServerMessageBlock.ReadInt8(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileStandardInfo.NumberOfLinks = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			smbQueryFileStandardInfo.DeletePending = ((buffer[bufferIndex++] & byte.MaxValue) > 0);
			smbQueryFileStandardInfo.Directory = ((buffer[bufferIndex++] & byte.MaxValue) > 0);
			this.Info = smbQueryFileStandardInfo;
			return bufferIndex - num;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00021B30 File Offset: 0x0001FD30
		internal virtual int ReadSmbQueryFileBasicInfoWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			Trans2QueryPathInformationResponse.SmbQueryFileBasicInfo smbQueryFileBasicInfo = new Trans2QueryPathInformationResponse.SmbQueryFileBasicInfo(this);
			smbQueryFileBasicInfo.CreateTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileBasicInfo.LastAccessTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileBasicInfo.LastWriteTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileBasicInfo.ChangeTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
			bufferIndex += 8;
			smbQueryFileBasicInfo.Attributes = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.Info = smbQueryFileBasicInfo;
			return bufferIndex - num;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00021BB0 File Offset: 0x0001FDB0
		public override string ToString()
		{
			return "Trans2QueryPathInformationResponse[" + base.ToString() + "]";
		}

		// Token: 0x040003B6 RID: 950
		internal const int SMB_QUERY_FILE_BASIC_INFO = 257;

		// Token: 0x040003B7 RID: 951
		internal const int SMB_QUERY_FILE_STANDARD_INFO = 258;

		// Token: 0x040003B8 RID: 952
		private int _informationLevel;

		// Token: 0x040003B9 RID: 953
		internal IInfo Info;

		// Token: 0x02000127 RID: 295
		internal class SmbQueryFileBasicInfo : IInfo
		{
			// Token: 0x06000844 RID: 2116 RVA: 0x0002BBB8 File Offset: 0x00029DB8
			public virtual int GetAttributes()
			{
				return this.Attributes;
			}

			// Token: 0x06000845 RID: 2117 RVA: 0x0002BBD0 File Offset: 0x00029DD0
			public virtual long GetCreateTime()
			{
				return this.CreateTime;
			}

			// Token: 0x06000846 RID: 2118 RVA: 0x0002BBE8 File Offset: 0x00029DE8
			public virtual long GetLastWriteTime()
			{
				return this.LastWriteTime;
			}

			// Token: 0x06000847 RID: 2119 RVA: 0x0002BC00 File Offset: 0x00029E00
			public virtual long GetSize()
			{
				return 0L;
			}

			// Token: 0x06000848 RID: 2120 RVA: 0x0002BC14 File Offset: 0x00029E14
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"SmbQueryFileBasicInfo[createTime=",
					Extensions.CreateDate(this.CreateTime),
					",lastAccessTime=",
					Extensions.CreateDate(this.LastAccessTime),
					",lastWriteTime=",
					Extensions.CreateDate(this.LastWriteTime),
					",changeTime=",
					Extensions.CreateDate(this.ChangeTime),
					",attributes=0x",
					Hexdump.ToHexString(this.Attributes, 4),
					"]"
				});
			}

			// Token: 0x06000849 RID: 2121 RVA: 0x0002BCBF File Offset: 0x00029EBF
			internal SmbQueryFileBasicInfo(Trans2QueryPathInformationResponse enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x040005B2 RID: 1458
			internal long CreateTime;

			// Token: 0x040005B3 RID: 1459
			internal long LastAccessTime;

			// Token: 0x040005B4 RID: 1460
			internal long LastWriteTime;

			// Token: 0x040005B5 RID: 1461
			internal long ChangeTime;

			// Token: 0x040005B6 RID: 1462
			internal int Attributes;

			// Token: 0x040005B7 RID: 1463
			private readonly Trans2QueryPathInformationResponse _enclosing;
		}

		// Token: 0x02000128 RID: 296
		internal class SmbQueryFileStandardInfo : IInfo
		{
			// Token: 0x0600084A RID: 2122 RVA: 0x0002BCD0 File Offset: 0x00029ED0
			public virtual int GetAttributes()
			{
				return 0;
			}

			// Token: 0x0600084B RID: 2123 RVA: 0x0002BCE4 File Offset: 0x00029EE4
			public virtual long GetCreateTime()
			{
				return 0L;
			}

			// Token: 0x0600084C RID: 2124 RVA: 0x0002BCF8 File Offset: 0x00029EF8
			public virtual long GetLastWriteTime()
			{
				return 0L;
			}

			// Token: 0x0600084D RID: 2125 RVA: 0x0002BD0C File Offset: 0x00029F0C
			public virtual long GetSize()
			{
				return this.EndOfFile;
			}

			// Token: 0x0600084E RID: 2126 RVA: 0x0002BD24 File Offset: 0x00029F24
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"SmbQueryInfoStandard[allocationSize=",
					this.AllocationSize,
					",endOfFile=",
					this.EndOfFile,
					",numberOfLinks=",
					this.NumberOfLinks,
					",deletePending=",
					this.DeletePending.ToString(),
					",directory=",
					this.Directory.ToString(),
					"]"
				});
			}

			// Token: 0x0600084F RID: 2127 RVA: 0x0002BDBA File Offset: 0x00029FBA
			internal SmbQueryFileStandardInfo(Trans2QueryPathInformationResponse enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x040005B8 RID: 1464
			internal long AllocationSize;

			// Token: 0x040005B9 RID: 1465
			internal long EndOfFile;

			// Token: 0x040005BA RID: 1466
			internal int NumberOfLinks;

			// Token: 0x040005BB RID: 1467
			internal bool DeletePending;

			// Token: 0x040005BC RID: 1468
			internal bool Directory;

			// Token: 0x040005BD RID: 1469
			private readonly Trans2QueryPathInformationResponse _enclosing;
		}
	}
}
