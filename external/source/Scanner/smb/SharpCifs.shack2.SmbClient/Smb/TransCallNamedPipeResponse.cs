using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000C3 RID: 195
	internal class TransCallNamedPipeResponse : SmbComTransactionResponse
	{
		// Token: 0x0600064B RID: 1611 RVA: 0x000225CF File Offset: 0x000207CF
		internal TransCallNamedPipeResponse(SmbNamedPipe pipe)
		{
			this._pipe = pipe;
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000225E0 File Offset: 0x000207E0
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000225F4 File Offset: 0x000207F4
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00022608 File Offset: 0x00020808
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0002261C File Offset: 0x0002081C
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00022630 File Offset: 0x00020830
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00022644 File Offset: 0x00020844
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

		// Token: 0x06000652 RID: 1618 RVA: 0x000226C8 File Offset: 0x000208C8
		public override string ToString()
		{
			return "TransCallNamedPipeResponse[" + base.ToString() + "]";
		}

		// Token: 0x040003CD RID: 973
		private SmbNamedPipe _pipe;
	}
}
