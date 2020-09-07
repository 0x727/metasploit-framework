using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000A2 RID: 162
	internal class SmbComTreeDisconnect : ServerMessageBlock
	{
		// Token: 0x060004B5 RID: 1205 RVA: 0x00016F7D File Offset: 0x0001517D
		public SmbComTreeDisconnect()
		{
			this.Command = 113;
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00016F90 File Offset: 0x00015190
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00016FA4 File Offset: 0x000151A4
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00016FB8 File Offset: 0x000151B8
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x00016FCC File Offset: 0x000151CC
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00016FE0 File Offset: 0x000151E0
		public override string ToString()
		{
			return "SmbComTreeDisconnect[" + base.ToString() + "]";
		}
	}
}
