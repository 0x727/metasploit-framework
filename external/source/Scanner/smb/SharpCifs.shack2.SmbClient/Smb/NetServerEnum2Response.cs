using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x02000078 RID: 120
	internal class NetServerEnum2Response : SmbComTransactionResponse
	{
		// Token: 0x0600035D RID: 861 RVA: 0x0000E634 File Offset: 0x0000C834
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0000E648 File Offset: 0x0000C848
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000E65C File Offset: 0x0000C85C
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0000E670 File Offset: 0x0000C870
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x0000E684 File Offset: 0x0000C884
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			this.Status = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this._converter = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.NumEntries = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this._totalAvailableEntries = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			return bufferIndex - num;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0000E6E4 File Offset: 0x0000C8E4
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			NetServerEnum2Response.ServerInfo1 serverInfo = null;
			this.Results = new NetServerEnum2Response.ServerInfo1[this.NumEntries];
			for (int i = 0; i < this.NumEntries; i++)
			{
				this.Results[i] = new NetServerEnum2Response.ServerInfo1(this);
				serverInfo =  (NetServerEnum2Response.ServerInfo1)this.Results[i];
				serverInfo.Name = this.ReadString(buffer, bufferIndex, 16, false);
				bufferIndex += 16;
				serverInfo.VersionMajor = (int)(buffer[bufferIndex++] & byte.MaxValue);
				serverInfo.VersionMinor = (int)(buffer[bufferIndex++] & byte.MaxValue);
				serverInfo.Type = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				int num2 = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				num2 = (num2 & 65535) - this._converter;
				num2 = num + num2;
				serverInfo.CommentOrMasterBrowser = this.ReadString(buffer, num2, 48, false);
				bool flag = ServerMessageBlock.Log.Level >= 4;
				if (flag)
				{
					ServerMessageBlock.Log.WriteLine(serverInfo);
				}
			}
			this.LastName = ((this.NumEntries == 0) ? null : serverInfo.Name);
			return bufferIndex - num;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0000E7FC File Offset: 0x0000C9FC
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"NetServerEnum2Response[",
				base.ToString(),
				",status=",
				this.Status,
				",converter=",
				this._converter,
				",entriesReturned=",
				this.NumEntries,
				",totalAvailableEntries=",
				this._totalAvailableEntries,
				",lastName=",
				this.LastName,
				"]"
			});
		}

		// Token: 0x040000DB RID: 219
		private int _converter;

		// Token: 0x040000DC RID: 220
		private int _totalAvailableEntries;

		// Token: 0x040000DD RID: 221
		internal string LastName;

		// Token: 0x02000116 RID: 278
		internal class ServerInfo1 : IFileEntry
		{
			// Token: 0x06000813 RID: 2067 RVA: 0x0002B11C File Offset: 0x0002931C
			public virtual string GetName()
			{
				return this.Name;
			}

			// Token: 0x06000814 RID: 2068 RVA: 0x0002B134 File Offset: 0x00029334
			public new virtual int GetType()
			{
				return ((this.Type & int.MinValue) != 0) ? 2 : 4;
			}

			// Token: 0x06000815 RID: 2069 RVA: 0x0002B158 File Offset: 0x00029358
			public virtual int GetAttributes()
			{
				return 17;
			}

			// Token: 0x06000816 RID: 2070 RVA: 0x0002B16C File Offset: 0x0002936C
			public virtual long CreateTime()
			{
				return 0L;
			}

			// Token: 0x06000817 RID: 2071 RVA: 0x0002B180 File Offset: 0x00029380
			public virtual long LastModified()
			{
				return 0L;
			}

			// Token: 0x06000818 RID: 2072 RVA: 0x0002B194 File Offset: 0x00029394
			public virtual long Length()
			{
				return 0L;
			}

			// Token: 0x06000819 RID: 2073 RVA: 0x0002B1A8 File Offset: 0x000293A8
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"ServerInfo1[name=",
					this.Name,
					",versionMajor=",
					this.VersionMajor,
					",versionMinor=",
					this.VersionMinor,
					",type=0x",
					Hexdump.ToHexString(this.Type, 8),
					",commentOrMasterBrowser=",
					this.CommentOrMasterBrowser,
					"]"
				});
			}

			// Token: 0x0600081A RID: 2074 RVA: 0x0002B235 File Offset: 0x00029435
			internal ServerInfo1(NetServerEnum2Response enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x04000559 RID: 1369
			internal string Name;

			// Token: 0x0400055A RID: 1370
			internal int VersionMajor;

			// Token: 0x0400055B RID: 1371
			internal int VersionMinor;

			// Token: 0x0400055C RID: 1372
			internal int Type;

			// Token: 0x0400055D RID: 1373
			internal string CommentOrMasterBrowser;

			// Token: 0x0400055E RID: 1374
			private readonly NetServerEnum2Response _enclosing;
		}
	}
}
