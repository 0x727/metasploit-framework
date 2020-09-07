using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000B9 RID: 185
	internal class Trans2GetDfsReferralResponse : SmbComTransactionResponse
	{
		// Token: 0x060005FA RID: 1530 RVA: 0x000212F5 File Offset: 0x0001F4F5
		public Trans2GetDfsReferralResponse()
		{
			this.SubCommand = 16;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00021308 File Offset: 0x0001F508
		internal override int WriteSetupWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0002131C File Offset: 0x0001F51C
		internal override int WriteParametersWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00021330 File Offset: 0x0001F530
		internal override int WriteDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00021344 File Offset: 0x0001F544
		internal override int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00021358 File Offset: 0x0001F558
		internal override int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			return 0;
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0002136C File Offset: 0x0001F56C
		internal override int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			this.PathConsumed = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			bool flag = (this.Flags2 & SmbConstants.Flags2Unicode) != 0;
			if (flag)
			{
				this.PathConsumed /= 2;
			}
			this.NumReferrals = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.flags = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 4;
			this.Referrals = new Trans2GetDfsReferralResponse.Referral[this.NumReferrals];
			for (int i = 0; i < this.NumReferrals; i++)
			{
				this.Referrals[i] = new Trans2GetDfsReferralResponse.Referral(this);
				bufferIndex += this.Referrals[i].ReadWireFormat(buffer, bufferIndex, len);
			}
			return bufferIndex - num;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00021428 File Offset: 0x0001F628
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Trans2GetDfsReferralResponse[",
				base.ToString(),
				",pathConsumed=",
				this.PathConsumed,
				",numReferrals=",
				this.NumReferrals,
				",flags=",
				this.flags,
				"]"
			});
		}

		// Token: 0x040003AB RID: 939
		internal int PathConsumed;

		// Token: 0x040003AC RID: 940
		internal int NumReferrals;

		// Token: 0x040003AD RID: 941
		internal int flags;

		// Token: 0x040003AE RID: 942
		internal Trans2GetDfsReferralResponse.Referral[] Referrals;

		// Token: 0x02000125 RID: 293
		internal class Referral
		{
			// Token: 0x0600083D RID: 2109 RVA: 0x0002B7E0 File Offset: 0x000299E0
			internal virtual int ReadWireFormat(byte[] buffer, int bufferIndex, int len)
			{
				int num = bufferIndex;
				this._version = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bool flag = this._version != 3 && this._version != 1;
				if (flag)
				{
					throw new RuntimeException("Version " + this._version + " referral not supported. Please report this to jcifs at samba dot org.");
				}
				bufferIndex += 2;
				this._size = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this._serverType = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this._flags = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				bool flag2 = this._version == 3;
				if (flag2)
				{
					this._proximity = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					this.Ttl = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					this._pathOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					this._altPathOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					this._nodeOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					this.Path = this._enclosing.ReadString(buffer, num + this._pathOffset, len, (this._enclosing.Flags2 & SmbConstants.Flags2Unicode) != 0);
					bool flag3 = this._nodeOffset > 0;
					if (flag3)
					{
						this.Node = this._enclosing.ReadString(buffer, num + this._nodeOffset, len, (this._enclosing.Flags2 & SmbConstants.Flags2Unicode) != 0);
					}
				}
				else
				{
					bool flag4 = this._version == 1;
					if (flag4)
					{
						this.Node = this._enclosing.ReadString(buffer, bufferIndex, len, (this._enclosing.Flags2 & SmbConstants.Flags2Unicode) != 0);
					}
				}
				return this._size;
			}

			// Token: 0x0600083E RID: 2110 RVA: 0x0002B998 File Offset: 0x00029B98
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"Referral[version=",
					this._version,
					",size=",
					this._size,
					",serverType=",
					this._serverType,
					",flags=",
					this._flags,
					",proximity=",
					this._proximity,
					",ttl=",
					this.Ttl,
					",pathOffset=",
					this._pathOffset,
					",altPathOffset=",
					this._altPathOffset,
					",nodeOffset=",
					this._nodeOffset,
					",path=",
					this.Path,
					",altPath=",
					this._altPath,
					",node=",
					this.Node,
					"]"
				});
			}

			// Token: 0x0600083F RID: 2111 RVA: 0x0002BAC7 File Offset: 0x00029CC7
			internal Referral(Trans2GetDfsReferralResponse enclosing)
			{
				this._enclosing = enclosing;
			}

			// Token: 0x040005A0 RID: 1440
			private int _version;

			// Token: 0x040005A1 RID: 1441
			private int _size;

			// Token: 0x040005A2 RID: 1442
			private int _serverType;

			// Token: 0x040005A3 RID: 1443
			private int _flags;

			// Token: 0x040005A4 RID: 1444
			private int _proximity;

			// Token: 0x040005A5 RID: 1445
			private int _pathOffset;

			// Token: 0x040005A6 RID: 1446
			private int _altPathOffset;

			// Token: 0x040005A7 RID: 1447
			private int _nodeOffset;

			// Token: 0x040005A8 RID: 1448
			private string _altPath;

			// Token: 0x040005A9 RID: 1449
			internal int Ttl;

			// Token: 0x040005AA RID: 1450
			internal string Path;

			// Token: 0x040005AB RID: 1451
			internal string Node;

			// Token: 0x040005AC RID: 1452
			private readonly Trans2GetDfsReferralResponse _enclosing;
		}
	}
}
