using System;

namespace SharpCifs.Smb
{
	// Token: 0x020000C6 RID: 198
	internal class TransTransactNamedPipe : SmbComTransaction
	{
		// Token: 0x06000663 RID: 1635 RVA: 0x00022910 File Offset: 0x00020B10
		internal TransTransactNamedPipe(int fid, byte[] data, int off, int len)
		{
			this._pipeFid = fid;
			this._pipeData = data;
			this._pipeDataOff = off;
			this._pipeDataLen = len;
			this.Command = 37;
			this.SubCommand = 38;
			this.MaxParameterCount = 0;
			this.MaxDataCount = 65535;
			this.MaxSetupCount = 0;
			this.SetupCount = 2;
			this.Name = "\\PIPE\\";
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x00022980 File Offset: 0x00020B80
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = this.SubCommand;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt2((long)this._pipeFid, dst, dstIndex);
			dstIndex += 2;
			return 4;
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x000229C0 File Offset: 0x00020BC0
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x000229D4 File Offset: 0x00020BD4
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x000229E8 File Offset: 0x00020BE8
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			bool flag = dst.Length - dstIndex < this._pipeDataLen;
			int result;
			if (flag)
			{
				bool flag2 = ServerMessageBlock.Log.Level >= 3;
				if (flag2)
				{
					ServerMessageBlock.Log.WriteLine("TransTransactNamedPipe data too long for buffer");
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

		// Token: 0x06000668 RID: 1640 RVA: 0x00022A58 File Offset: 0x00020C58
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00022A6C File Offset: 0x00020C6C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x00022A80 File Offset: 0x00020C80
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"TransTransactNamedPipe[",
				base.ToString(),
				",pipeFid=",
				this._pipeFid,
				"]"
			});
		}

		// Token: 0x040003D7 RID: 983
		private byte[] _pipeData;

		// Token: 0x040003D8 RID: 984
		private int _pipeFid;

		// Token: 0x040003D9 RID: 985
		private int _pipeDataOff;

		// Token: 0x040003DA RID: 986
		private int _pipeDataLen;
	}
}
