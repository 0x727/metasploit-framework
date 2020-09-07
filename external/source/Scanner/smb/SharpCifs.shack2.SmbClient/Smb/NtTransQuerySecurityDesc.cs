using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x02000080 RID: 128
	internal class NtTransQuerySecurityDesc : SmbComNtTransaction
	{
		// Token: 0x060003A4 RID: 932 RVA: 0x000103DC File Offset: 0x0000E5DC
		internal NtTransQuerySecurityDesc(int fid, int securityInformation)
		{
			this.Fid = fid;
			this.SecurityInformation = securityInformation;
			this.Command = 160;
			this.Function = 6;
			this.SetupCount = 0;
			this.TotalDataCount = 0;
			this.MaxParameterCount = 4;
			this.MaxDataCount = 32768;
			this.MaxSetupCount = 0;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00010438 File Offset: 0x0000E638
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0001044C File Offset: 0x0000E64C
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this.Fid, dst, dstIndex);
			dstIndex += 2;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt4((long)this.SecurityInformation, dst, dstIndex);
			dstIndex += 4;
			return dstIndex - num;
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000104A0 File Offset: 0x0000E6A0
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x000104B4 File Offset: 0x0000E6B4
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x000104C8 File Offset: 0x0000E6C8
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x000104DC File Offset: 0x0000E6DC
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x000104F0 File Offset: 0x0000E6F0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"NtTransQuerySecurityDesc[",
				base.ToString(),
				",fid=0x",
				Hexdump.ToHexString(this.Fid, 4),
				",securityInformation=0x",
				Hexdump.ToHexString(this.SecurityInformation, 8),
				"]"
			});
		}

		// Token: 0x04000143 RID: 323
		internal int Fid;

		// Token: 0x04000144 RID: 324
		internal int SecurityInformation;
	}
}
