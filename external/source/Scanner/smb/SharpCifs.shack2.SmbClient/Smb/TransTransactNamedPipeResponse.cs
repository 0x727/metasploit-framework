using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000C7 RID: 199
	internal class TransTransactNamedPipeResponse : SmbComTransactionResponse
	{
		// Token: 0x0600066B RID: 1643 RVA: 0x00022ACC File Offset: 0x00020CCC
		internal TransTransactNamedPipeResponse(SmbNamedPipe pipe)
		{
			this._pipe = pipe;
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x00022AE0 File Offset: 0x00020CE0
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00022AF4 File Offset: 0x00020CF4
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x00022B08 File Offset: 0x00020D08
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00022B1C File Offset: 0x00020D1C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00022B30 File Offset: 0x00020D30
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00022B44 File Offset: 0x00020D44
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			bool flag = this._pipe.PipeIn != null;
			if (flag)
			{
				TransactNamedPipeInputStream transactNamedPipeInputStream = (TransactNamedPipeInputStream)this._pipe.PipeIn;
				object @lock = transactNamedPipeInputStream.Lock;
				lock (@lock)
				{
					transactNamedPipeInputStream.Receive(buffer, bufferIndex, len);
					Runtime.Notify(transactNamedPipeInputStream.Lock);
				}
			}
			return len;
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00022BC8 File Offset: 0x00020DC8
		public override string ToString()
		{
			return "TransTransactNamedPipeResponse[" + base.ToString() + "]";
		}

		// Token: 0x040003DB RID: 987
		private SmbNamedPipe _pipe;
	}
}
