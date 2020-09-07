using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x020000B7 RID: 183
	internal class Trans2FindNext2 : SmbComTransaction
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x00020F3C File Offset: 0x0001F13C
		internal Trans2FindNext2(int sid, int resumeKey, string filename)
		{
			this._sid = sid;
			this._resumeKey = resumeKey;
			this._filename = filename;
			this.Command = 50;
			this.SubCommand = 2;
			this._informationLevel = 260;
			this._flags = 0;
			this.MaxParameterCount = 8;
			this.MaxDataCount = SharpCifs.Smb.Trans2FindFirst2.ListSize;
			this.MaxSetupCount = 0;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00020FA0 File Offset: 0x0001F1A0
		internal override void Reset(int resumeKey, string lastName)
		{
			base.Reset();
			this._resumeKey = resumeKey;
			this._filename = lastName;
			this.Flags2 = 0;
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00020FC0 File Offset: 0x0001F1C0
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			return 2;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00020FEC File Offset: 0x0001F1EC
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this._sid, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)SharpCifs.Smb.Trans2FindFirst2.ListCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this._informationLevel, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this._resumeKey, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt2((long)this._flags, dst, dstIndex);
			dstIndex += 2;
			dstIndex += this.WriteString(this._filename, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00021078 File Offset: 0x0001F278
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0002108C File Offset: 0x0001F28C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x000210A0 File Offset: 0x0001F2A0
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x000210B4 File Offset: 0x0001F2B4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x000210C8 File Offset: 0x0001F2C8
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trans2FindNext2[",
				base.ToString(),
				",sid=",
				this._sid,
				",searchCount=",
				SharpCifs.Smb.Trans2FindFirst2.ListSize,
				",informationLevel=0x",
				Hexdump.ToHexString(this._informationLevel, 3),
				",resumeKey=0x",
				Hexdump.ToHexString(this._resumeKey, 4),
				",flags=0x",
				Hexdump.ToHexString(this._flags, 2),
				",filename=",
				this._filename,
				"]"
			});
		}

		// Token: 0x040003A5 RID: 933
		private int _sid;

		// Token: 0x040003A6 RID: 934
		private int _informationLevel;

		// Token: 0x040003A7 RID: 935
		private int _resumeKey;

		// Token: 0x040003A8 RID: 936
		private int _flags;

		// Token: 0x040003A9 RID: 937
		private string _filename;
	}
}
