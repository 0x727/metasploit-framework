using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000093 RID: 147
	internal abstract class SmbComNtTransaction : SmbComTransaction
	{
		// Token: 0x0600043F RID: 1087 RVA: 0x00014300 File Offset: 0x00012500
		public SmbComNtTransaction()
		{
			this.primarySetupOffset = 69;
			this.secondaryParameterOffset = 51;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x0001431C File Offset: 0x0001251C
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			bool flag = this.Command != 161;
			if (flag)
			{
				dst[dstIndex++] = this.MaxSetupCount;
			}
			else
			{
				dst[dstIndex++] = 0;
			}
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt4((long)this.TotalParameterCount, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this.TotalDataCount, dst, dstIndex);
			dstIndex += 4;
			bool flag2 = this.Command != 161;
			if (flag2)
			{
				ServerMessageBlock.WriteInt4((long)this.MaxParameterCount, dst, dstIndex);
				dstIndex += 4;
				ServerMessageBlock.WriteInt4((long)this.MaxDataCount, dst, dstIndex);
				dstIndex += 4;
			}
			ServerMessageBlock.WriteInt4((long)this.ParameterCount, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)((this.ParameterCount == 0) ? 0 : this.ParameterOffset), dst, dstIndex);
			dstIndex += 4;
			bool flag3 = this.Command == 161;
			if (flag3)
			{
				ServerMessageBlock.WriteInt4((long)this.ParameterDisplacement, dst, dstIndex);
				dstIndex += 4;
			}
			ServerMessageBlock.WriteInt4((long)this.DataCount, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)((this.DataCount == 0) ? 0 : this.DataOffset), dst, dstIndex);
			dstIndex += 4;
			bool flag4 = this.Command == 161;
			if (flag4)
			{
				ServerMessageBlock.WriteInt4((long)this.DataDisplacement, dst, dstIndex);
				dstIndex += 4;
				dst[dstIndex++] = 0;
			}
			else
			{
				dst[dstIndex++] = (byte)this.SetupCount;
				ServerMessageBlock.WriteInt2((long)this.Function, dst, dstIndex);
				dstIndex += 2;
				dstIndex += this.WriteSetupWireFormat(dst, dstIndex);
			}
			return dstIndex - num;
		}

		// Token: 0x040001C5 RID: 453
		private const int NttPrimarySetupOffset = 69;

		// Token: 0x040001C6 RID: 454
		private const int NttSecondaryParameterOffset = 51;

		// Token: 0x040001C7 RID: 455
		internal const int NtTransactQuerySecurityDesc = 6;

		// Token: 0x040001C8 RID: 456
		internal int Function;
	}
}
