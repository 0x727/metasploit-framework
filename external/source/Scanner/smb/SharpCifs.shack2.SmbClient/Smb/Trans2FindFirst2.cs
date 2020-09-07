using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x020000B5 RID: 181
	internal class Trans2FindFirst2 : SmbComTransaction
	{
		// Token: 0x060005D7 RID: 1495 RVA: 0x00020900 File Offset: 0x0001EB00
		internal Trans2FindFirst2(string filename, string wildcard, int searchAttributes)
		{
			bool flag = filename.Equals("\\");
			if (flag)
			{
				this.Path = filename;
			}
			else
			{
				this.Path = filename + "\\";
			}
			this._wildcard = wildcard;
			this._searchAttributes = (searchAttributes & 55);
			this.Command = 50;
			this.SubCommand = 1;
			this._flags = 0;
			this._informationLevel = 260;
			this.TotalDataCount = 0;
			this.MaxParameterCount = 10;
			this.MaxDataCount = SharpCifs.Smb.Trans2FindFirst2.ListSize;
			this.MaxSetupCount = 0;
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0002099C File Offset: 0x0001EB9C
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x000209C8 File Offset: 0x0001EBC8
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._searchAttributes, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)SharpCifs.Smb.Trans2FindFirst2.ListCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._flags, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._informationLevel, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this._searchStorageType, dst, dstIndex);
			dstIndex += 4;
			dstIndex += this.WriteString(this.Path + this._wildcard, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00020A60 File Offset: 0x0001EC60
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x00020A74 File Offset: 0x0001EC74
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00020A88 File Offset: 0x0001EC88
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00020A9C File Offset: 0x0001EC9C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00020AB0 File Offset: 0x0001ECB0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trans2FindFirst2[",
				base.ToString(),
				",searchAttributes=0x",
				Hexdump.ToHexString(this._searchAttributes, 2),
				",searchCount=",
				SharpCifs.Smb.Trans2FindFirst2.ListCount,
				",flags=0x",
				Hexdump.ToHexString(this._flags, 2),
				",informationLevel=0x",
				Hexdump.ToHexString(this._informationLevel, 3),
				",searchStorageType=",
				this._searchStorageType,
				",filename=",
				this.Path,
				"]"
			});
		}

		// Token: 0x04000382 RID: 898
		private const int FlagsCloseAfterThisRequest = 1;

		// Token: 0x04000383 RID: 899
		private const int FlagsCloseIfEndReached = 2;

		// Token: 0x04000384 RID: 900
		private const int FlagsReturnResumeKeys = 4;

		// Token: 0x04000385 RID: 901
		private const int FlagsResumeFromPreviousEnd = 8;

		// Token: 0x04000386 RID: 902
		private const int FlagsFindWithBackupIntent = 16;

		// Token: 0x04000387 RID: 903
		private const int DefaultListSize = 65535;

		// Token: 0x04000388 RID: 904
		private const int DefaultListCount = 200;

		// Token: 0x04000389 RID: 905
		private int _searchAttributes;

		// Token: 0x0400038A RID: 906
		private int _flags;

		// Token: 0x0400038B RID: 907
		private int _informationLevel;

		// Token: 0x0400038C RID: 908
		private int _searchStorageType = 0;

		// Token: 0x0400038D RID: 909
		private string _wildcard;

		// Token: 0x0400038E RID: 910
		internal const int SmbInfoStandard = 1;

		// Token: 0x0400038F RID: 911
		internal const int SmbInfoQueryEaSize = 2;

		// Token: 0x04000390 RID: 912
		internal const int SmbInfoQueryEasFromList = 3;

		// Token: 0x04000391 RID: 913
		internal const int SmbFindFileDirectoryInfo = 257;

		// Token: 0x04000392 RID: 914
		internal const int SmbFindFileFullDirectoryInfo = 258;

		// Token: 0x04000393 RID: 915
		internal const int SmbFileNamesInfo = 259;

		// Token: 0x04000394 RID: 916
		internal const int SmbFileBothDirectoryInfo = 260;

		// Token: 0x04000395 RID: 917
		internal static readonly int ListSize = Config.GetInt("jcifs.smb.client.listSize", 65535);

		// Token: 0x04000396 RID: 918
		internal static readonly int ListCount = Config.GetInt("jcifs.smb.client.listCount", 200);
	}
}
