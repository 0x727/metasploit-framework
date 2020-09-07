using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000079 RID: 121
	internal class NetShareEnum : SmbComTransaction
	{
		// Token: 0x06000365 RID: 869 RVA: 0x0000E8AC File Offset: 0x0000CAAC
		public NetShareEnum()
		{
			this.Command = 37;
			this.SubCommand = 0;
			this.Name = "\\PIPE\\LANMAN";
			this.MaxParameterCount = 8;
			this.MaxSetupCount = 0;
			this.SetupCount = 0;
			this.Timeout = 5000;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000E8FC File Offset: 0x0000CAFC
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000E910 File Offset: 0x0000CB10
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			byte[] bytesForString;
			try
			{
				bytesForString = Runtime.GetBytesForString(SharpCifs.Smb.NetShareEnum.Descr, "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				return 0;
			}
			ServerMessageBlock.WriteInt2(0L, dst, dstIndex);
			dstIndex += 2;
			Array.Copy(bytesForString, 0, dst, dstIndex, bytesForString.Length);
			dstIndex += bytesForString.Length;
			ServerMessageBlock.WriteInt2(1L, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.MaxDataCount, dst, dstIndex);
			dstIndex += 2;
			return dstIndex - num;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0000E998 File Offset: 0x0000CB98
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0000E9AC File Offset: 0x0000CBAC
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0000E9C0 File Offset: 0x0000CBC0
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0000E9D4 File Offset: 0x0000CBD4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000E9E8 File Offset: 0x0000CBE8
		public override string ToString()
		{
			return "NetShareEnum[" + base.ToString() + "]";
		}

		// Token: 0x040000DE RID: 222
		private static readonly string Descr = "WrLeh\0B13BWz\0";
	}
}
