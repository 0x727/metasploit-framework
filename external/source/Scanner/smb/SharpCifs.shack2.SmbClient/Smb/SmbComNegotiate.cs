using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x0200008F RID: 143
	internal class SmbComNegotiate : ServerMessageBlock
	{
		// Token: 0x06000427 RID: 1063 RVA: 0x000135EB File Offset: 0x000117EB
		public SmbComNegotiate()
		{
			this.Command = 114;
			this.Flags2 = SmbConstants.DefaultFlags2;
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00013608 File Offset: 0x00011808
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0001361C File Offset: 0x0001181C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			byte[] bytesForString;
			try
			{
				bytesForString = Runtime.GetBytesForString("\u0002NT LM 0.12\0", "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				return 0;
			}
			Array.Copy(bytesForString, 0, dst, dstIndex, bytesForString.Length);
			return bytesForString.Length;
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00013668 File Offset: 0x00011868
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0001367C File Offset: 0x0001187C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00013690 File Offset: 0x00011890
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComNegotiate[",
				base.ToString(),
				",wordCount=",
				this.WordCount,
				",dialects=NT LM 0.12]"
			});
		}

		// Token: 0x0400019A RID: 410
		private const string Dialects = "\u0002NT LM 0.12\0";
	}
}
