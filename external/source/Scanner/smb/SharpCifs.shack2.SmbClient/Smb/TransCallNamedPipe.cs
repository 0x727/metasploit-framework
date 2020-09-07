using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C2 RID: 194
	internal class TransCallNamedPipe : SmbComTransaction
	{
		// Token: 0x06000643 RID: 1603 RVA: 0x00022420 File Offset: 0x00020620
		internal TransCallNamedPipe(string pipeName, byte[] data, int off, int len)
		{
			this.Name = pipeName;
			this._pipeData = data;
			this._pipeDataOff = off;
			this._pipeDataLen = len;
			this.Command = 37;
			this.SubCommand = 84;
			this.Timeout = -1;
			this.MaxParameterCount = 0;
			this.MaxDataCount = 65535;
			this.MaxSetupCount = 0;
			this.SetupCount = 2;
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0002248C File Offset: 0x0002068C
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			return 4;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x000224C8 File Offset: 0x000206C8
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000224DC File Offset: 0x000206DC
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000224F0 File Offset: 0x000206F0
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			bool flag = dst.Length - dstIndex < this._pipeDataLen;
			int result;
			if (flag)
			{
				bool flag2 = ServerMessageBlock.Log.Level >= 3;
				if (flag2)
				{
					ServerMessageBlock.Log.WriteLine("TransCallNamedPipe data too long for buffer");
				}
				result = 0;
			}
			else
			{
				Array.Copy(this._pipeData, this._pipeDataOff, dst, dstIndex, this._pipeDataLen);
				result = this._pipeDataLen;
			}
			return result;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00022560 File Offset: 0x00020760
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00022574 File Offset: 0x00020774
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00022588 File Offset: 0x00020788
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"TransCallNamedPipe[",
				base.ToString(),
				",pipeName=",
				this.Name,
				"]"
			});
		}

		// Token: 0x040003CA RID: 970
		private byte[] _pipeData;

		// Token: 0x040003CB RID: 971
		private int _pipeDataOff;

		// Token: 0x040003CC RID: 972
		private int _pipeDataLen;
	}
}
