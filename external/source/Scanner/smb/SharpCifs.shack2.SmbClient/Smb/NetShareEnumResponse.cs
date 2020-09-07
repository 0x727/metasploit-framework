using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200007A RID: 122
	internal class NetShareEnumResponse : SmbComTransactionResponse
	{
		// Token: 0x0600036E RID: 878 RVA: 0x0000EA1C File Offset: 0x0000CC1C
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000EA30 File Offset: 0x0000CC30
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0000EA44 File Offset: 0x0000CC44
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0000EA58 File Offset: 0x0000CC58
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000EA6C File Offset: 0x0000CC6C
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			this.Status = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this._converter = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.NumEntries = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this._totalAvailableEntries = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			return bufferIndex - num;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0000EACC File Offset: 0x0000CCCC
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			this.UseUnicode = false;
			this.Results = new SmbShareInfo[this.NumEntries];
			for (int i = 0; i < this.NumEntries; i++)
			{
				this.Results[i] = new SmbShareInfo();
				SmbShareInfo smbShareInfo = (SmbShareInfo)this.Results[i];
				smbShareInfo.NetName = this.ReadString(buffer, bufferIndex, 13, false);
				bufferIndex += 14;
				smbShareInfo.Type = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				int num2 = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				num2 = (num2 & 65535) - this._converter;
				num2 = num + num2;
				smbShareInfo.Remark = this.ReadString(buffer, num2, 128, false);
				bool flag = ServerMessageBlock.Log.Level >= 4;
				if (flag)
				{
					ServerMessageBlock.Log.WriteLine(smbShareInfo);
				}
			}
			return bufferIndex - num;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0000EBAC File Offset: 0x0000CDAC
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"NetShareEnumResponse[",
				base.ToString(),
				",status=",
				this.Status,
				",converter=",
				this._converter,
				",entriesReturned=",
				this.NumEntries,
				",totalAvailableEntries=",
				this._totalAvailableEntries,
				"]"
			});
		}

		// Token: 0x040000DF RID: 223
		private int _converter;

		// Token: 0x040000E0 RID: 224
		private int _totalAvailableEntries;
	}
}
