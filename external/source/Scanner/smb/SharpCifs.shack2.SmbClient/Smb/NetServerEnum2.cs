using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000077 RID: 119
	internal class NetServerEnum2 : SmbComTransaction
	{
		// Token: 0x06000353 RID: 851 RVA: 0x0000E3D8 File Offset: 0x0000C5D8
		internal NetServerEnum2(string domain, int serverTypes)
		{
			this.Domain = domain;
			this.ServerTypes = serverTypes;
			this.Command = 37;
			this.SubCommand = 104;
			this.Name = "\\PIPE\\LANMAN";
			this.MaxParameterCount = 8;
			this.MaxDataCount = 16384;
			this.MaxSetupCount = 0;
			this.SetupCount = 0;
			this.Timeout = 5000;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000E441 File Offset: 0x0000C641
		internal override void Reset(int key, string lastName)
		{
			base.Reset();
			this.LastName = lastName;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000E454 File Offset: 0x0000C654
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000E468 File Offset: 0x0000C668
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			int num2 = (this.SubCommand == 104) ? 0 : 1;
			byte[] bytesForString;
			try
			{
				bytesForString = Runtime.GetBytesForString(SharpCifs.Smb.NetServerEnum2.Descr[num2], "UTF-8");
			}
			catch (UnsupportedEncodingException)
			{
				return 0;
			}
			ServerMessageBlock.WriteInt2((long)(this.SubCommand & byte.MaxValue), dst, dstIndex);
			dstIndex += 2;
			Array.Copy(bytesForString, 0, dst, dstIndex, bytesForString.Length);
			dstIndex += bytesForString.Length;
			ServerMessageBlock.WriteInt2(1L, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.MaxDataCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this.ServerTypes, dst, dstIndex);
			dstIndex += 4;
			dstIndex += this.WriteString(this.Domain.ToUpper(), dst, dstIndex, false);
			bool flag = num2 == 1;
			if (flag)
			{
				dstIndex += this.WriteString(this.LastName.ToUpper(), dst, dstIndex, false);
			}
			return dstIndex - num;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000E560 File Offset: 0x0000C760
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000E574 File Offset: 0x0000C774
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0000E588 File Offset: 0x0000C788
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0000E59C File Offset: 0x0000C79C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0000E5B0 File Offset: 0x0000C7B0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"NetServerEnum2[",
				base.ToString(),
				",name=",
				this.Name,
				",serverTypes=",
				(this.ServerTypes == -1) ? "SV_TYPE_ALL" : "SV_TYPE_DOMAIN_ENUM",
				"]"
			});
		}

		// Token: 0x040000D5 RID: 213
		internal const int SvTypeAll = -1;

		// Token: 0x040000D6 RID: 214
		internal const int SvTypeDomainEnum = -2147483648;

		// Token: 0x040000D7 RID: 215
		internal static readonly string[] Descr = new string[]
		{
			"WrLehDO\0B16BBDz\0",
			"WrLehDz\0B16BBDz\0"
		};

		// Token: 0x040000D8 RID: 216
		internal string Domain;

		// Token: 0x040000D9 RID: 217
		internal string LastName;

		// Token: 0x040000DA RID: 218
		internal int ServerTypes;
	}
}
