using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000B6 RID: 182
	internal class Trans2FindFirst2Response : SmbComTransactionResponse
	{
		// Token: 0x060005E0 RID: 1504 RVA: 0x00020B98 File Offset: 0x0001ED98
		public Trans2FindFirst2Response()
		{
			this.Command = 50;
			this.SubCommand = 1;
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x00020BB4 File Offset: 0x0001EDB4
		internal virtual string ReadString(byte[] src, int srcIndex, int len)
		{
			string result = null;
			try
			{
				bool useUnicode = this.UseUnicode;
				if (useUnicode)
				{
					result = Runtime.GetStringForBytes(src, srcIndex, len, SmbConstants.UniEncoding);
				}
				else
				{
					bool flag = len > 0 && src[srcIndex + len - 1] == 0;
					if (flag)
					{
						len--;
					}
					result = Runtime.GetStringForBytes(src, srcIndex, len, SmbConstants.OemEncoding);
				}
			}
			catch (UnsupportedEncodingException ex)
			{
				bool flag2 = ServerMessageBlock.Log.Level > 1;
				if (flag2)
				{
					Runtime.PrintStackTrace(ex, ServerMessageBlock.Log);
				}
			}
			return result;
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00020C50 File Offset: 0x0001EE50
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00020C64 File Offset: 0x0001EE64
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00020C78 File Offset: 0x0001EE78
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00020C8C File Offset: 0x0001EE8C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00020CA0 File Offset: 0x0001EEA0
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			bool flag = this.SubCommand == 1;
			if (flag)
			{
				this.Sid = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
			}
			this.NumEntries = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.IsEndOfSearch = ((buffer[bufferIndex] & 1) == 1);
			bufferIndex += 2;
			this.EaErrorOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.LastNameOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			return bufferIndex - num;
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00020D28 File Offset: 0x0001EF28
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			this.LastNameBufferIndex = bufferIndex + this.LastNameOffset;
			this.Results = new Trans2FindFirst2Response.SmbFindFileBothDirectoryInfo[this.NumEntries];
			for (int i = 0; i < this.NumEntries; i++)
			{
				this.Results[i] = new Trans2FindFirst2Response.SmbFindFileBothDirectoryInfo(this);
				Trans2FindFirst2Response.SmbFindFileBothDirectoryInfo smbFindFileBothDirectoryInfo = (Trans2FindFirst2Response.SmbFindFileBothDirectoryInfo)this.Results[i];
				smbFindFileBothDirectoryInfo.NextEntryOffset = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				smbFindFileBothDirectoryInfo.FileIndex = ServerMessageBlock.ReadInt4(buffer, bufferIndex + 4);
				smbFindFileBothDirectoryInfo.CreationTime = ServerMessageBlock.ReadTime(buffer, bufferIndex + 8);
				smbFindFileBothDirectoryInfo.LastWriteTime = ServerMessageBlock.ReadTime(buffer, bufferIndex + 24);
				smbFindFileBothDirectoryInfo.EndOfFile = ServerMessageBlock.ReadInt8(buffer, bufferIndex + 40);
				smbFindFileBothDirectoryInfo.ExtFileAttributes = ServerMessageBlock.ReadInt4(buffer, bufferIndex + 56);
				smbFindFileBothDirectoryInfo.FileNameLength = ServerMessageBlock.ReadInt4(buffer, bufferIndex + 60);
				smbFindFileBothDirectoryInfo.Filename = this.ReadString(buffer, bufferIndex + 94, smbFindFileBothDirectoryInfo.FileNameLength);
				bool flag = this.LastNameBufferIndex >= bufferIndex && (smbFindFileBothDirectoryInfo.NextEntryOffset == 0 || this.LastNameBufferIndex < bufferIndex + smbFindFileBothDirectoryInfo.NextEntryOffset);
				if (flag)
				{
					this.LastName = smbFindFileBothDirectoryInfo.Filename;
					this.ResumeKey = smbFindFileBothDirectoryInfo.FileIndex;
				}
				bufferIndex += smbFindFileBothDirectoryInfo.NextEntryOffset;
			}
			return this.DataCount;
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00020E64 File Offset: 0x0001F064
		public override string ToString()
		{
			bool flag = this.SubCommand == 1;
			string text;
			if (flag)
			{
				text = "Trans2FindFirst2Response[";
			}
			else
			{
				text = "Trans2FindNext2Response[";
			}
			return string.Concat(new object[]
			{
				text,
				base.ToString(),
				",sid=",
				this.Sid,
				",searchCount=",
				this.NumEntries,
				",isEndOfSearch=",
				this.IsEndOfSearch.ToString(),
				",eaErrorOffset=",
				this.EaErrorOffset,
				",lastNameOffset=",
				this.LastNameOffset,
				",lastName=",
				this.LastName,
				"]"
			});
		}

		// Token: 0x04000397 RID: 919
		internal const int SmbInfoStandard = 1;

		// Token: 0x04000398 RID: 920
		internal const int SmbInfoQueryEaSize = 2;

		// Token: 0x04000399 RID: 921
		internal const int SmbInfoQueryEasFromList = 3;

		// Token: 0x0400039A RID: 922
		internal const int SmbFindFileDirectoryInfo = 257;

		// Token: 0x0400039B RID: 923
		internal const int SmbFindFileFullDirectoryInfo = 258;

		// Token: 0x0400039C RID: 924
		internal const int SmbFileNamesInfo = 259;

		// Token: 0x0400039D RID: 925
		internal const int SmbFileBothDirectoryInfo = 260;

		// Token: 0x0400039E RID: 926
		internal int Sid;

		// Token: 0x0400039F RID: 927
		internal bool IsEndOfSearch;

		// Token: 0x040003A0 RID: 928
		internal int EaErrorOffset;

		// Token: 0x040003A1 RID: 929
		internal int LastNameOffset;

		// Token: 0x040003A2 RID: 930
		internal int LastNameBufferIndex;

		// Token: 0x040003A3 RID: 931
		internal string LastName;

		// Token: 0x040003A4 RID: 932
		internal int ResumeKey;

		// Token: 0x02000124 RID: 292
		internal class SmbFindFileBothDirectoryInfo : IFileEntry
		{
			// Token: 0x06000835 RID: 2101 RVA: 0x0002B5C8 File Offset: 0x000297C8
			public virtual string GetName()
			{
				return this.Filename;
			}

			// Token: 0x06000836 RID: 2102 RVA: 0x0002B5E0 File Offset: 0x000297E0
			public new virtual int GetType()
			{
				return 1;
			}

			// Token: 0x06000837 RID: 2103 RVA: 0x0002B5F4 File Offset: 0x000297F4
			public virtual int GetAttributes()
			{
				return this.ExtFileAttributes;
			}

			// Token: 0x06000838 RID: 2104 RVA: 0x0002B60C File Offset: 0x0002980C
			public virtual long CreateTime()
			{
				return this.CreationTime;
			}

			// Token: 0x06000839 RID: 2105 RVA: 0x0002B624 File Offset: 0x00029824
			public virtual long LastModified()
			{
				return this.LastWriteTime;
			}

			// Token: 0x0600083A RID: 2106 RVA: 0x0002B63C File Offset: 0x0002983C
			public virtual long Length()
			{
				return this.EndOfFile;
			}

			// Token: 0x0600083B RID: 2107 RVA: 0x0002B654 File Offset: 0x00029854
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"SmbFindFileBothDirectoryInfo[nextEntryOffset=",
					this.NextEntryOffset,
					",fileIndex=",
					this.FileIndex,
					",creationTime=",
					Extensions.CreateDate(this.CreationTime),
					",lastAccessTime=",
					Extensions.CreateDate(this.LastAccessTime),
					",lastWriteTime=",
					Extensions.CreateDate(this.LastWriteTime),
					",changeTime=",
					Extensions.CreateDate(this.ChangeTime),
					",endOfFile=",
					this.EndOfFile,
					",allocationSize=",
					this.AllocationSize,
					",extFileAttributes=",
					this.ExtFileAttributes,
					",fileNameLength=",
					this.FileNameLength,
					",eaSize=",
					this.EaSize,
					",shortNameLength=",
					this.ShortNameLength,
					",shortName=",
					this.ShortName,
					",filename=",
					this.Filename,
					"]"
				});
			}

			// Token: 0x0600083C RID: 2108 RVA: 0x0002B7CC File Offset: 0x000299CC
			internal SmbFindFileBothDirectoryInfo(Trans2FindFirst2Response enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x04000591 RID: 1425
			internal int NextEntryOffset;

			// Token: 0x04000592 RID: 1426
			internal int FileIndex;

			// Token: 0x04000593 RID: 1427
			internal long CreationTime;

			// Token: 0x04000594 RID: 1428
			internal long LastAccessTime;

			// Token: 0x04000595 RID: 1429
			internal long LastWriteTime;

			// Token: 0x04000596 RID: 1430
			internal long ChangeTime;

			// Token: 0x04000597 RID: 1431
			internal long EndOfFile;

			// Token: 0x04000598 RID: 1432
			internal long AllocationSize;

			// Token: 0x04000599 RID: 1433
			internal int ExtFileAttributes;

			// Token: 0x0400059A RID: 1434
			internal int FileNameLength;

			// Token: 0x0400059B RID: 1435
			internal int EaSize;

			// Token: 0x0400059C RID: 1436
			internal int ShortNameLength;

			// Token: 0x0400059D RID: 1437
			internal string ShortName;

			// Token: 0x0400059E RID: 1438
			internal string Filename;

			// Token: 0x0400059F RID: 1439
			private readonly Trans2FindFirst2Response _enclosing;
		}
	}
}
