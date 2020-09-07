using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000A1 RID: 161
	internal class SmbComTreeConnectAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x060004AF RID: 1199 RVA: 0x00016E2C File Offset: 0x0001502C
		internal SmbComTreeConnectAndXResponse(ServerMessageBlock andx) : base(andx)
		{
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00016E44 File Offset: 0x00015044
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00016E58 File Offset: 0x00015058
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x00016E6C File Offset: 0x0001506C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			this.SupportSearchBits = ((buffer[bufferIndex] & 1) == 1);
			this.ShareIsInDfs = ((buffer[bufferIndex] & 2) == 2);
			return 2;
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00016E9C File Offset: 0x0001509C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			int num2 = this.ReadStringLength(buffer, bufferIndex, 32);
			try
			{
				this.Service = Runtime.GetStringForBytes(buffer, bufferIndex, num2, "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				return 0;
			}
			bufferIndex += num2 + 1;
			return bufferIndex - num;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00016EF4 File Offset: 0x000150F4
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComTreeConnectAndXResponse[",
				base.ToString(),
				",supportSearchBits=",
				this.SupportSearchBits.ToString(),
				",shareIsInDfs=",
				this.ShareIsInDfs.ToString(),
				",service=",
				this.Service,
				",nativeFileSystem=",
				this.NativeFileSystem,
				"]"
			});
		}

		// Token: 0x0400025B RID: 603
		private const int SmbSupportSearchBits = 1;

		// Token: 0x0400025C RID: 604
		private const int SmbShareIsInDfs = 2;

		// Token: 0x0400025D RID: 605
		internal bool SupportSearchBits;

		// Token: 0x0400025E RID: 606
		internal bool ShareIsInDfs;

		// Token: 0x0400025F RID: 607
		internal string Service;

		// Token: 0x04000260 RID: 608
		internal string NativeFileSystem = string.Empty;
	}
}
