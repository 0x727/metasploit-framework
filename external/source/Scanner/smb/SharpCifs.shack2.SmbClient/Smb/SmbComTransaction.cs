using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x0200009E RID: 158
	internal abstract class SmbComTransaction : ServerMessageBlock
	{
		// Token: 0x06000487 RID: 1159 RVA: 0x00015B88 File Offset: 0x00013D88
		public SmbComTransaction()
		{
			this.MaxParameterCount = 1024;
			this.primarySetupOffset = 61;
			this.secondaryParameterOffset = 51;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00015BF4 File Offset: 0x00013DF4
		internal override void Reset()
		{
			base.Reset();
			this._isPrimary = (this._hasMore = true);
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x00015C19 File Offset: 0x00013E19
		internal virtual void Reset(int key, string lastName)
		{
			this.Reset();
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00015C24 File Offset: 0x00013E24
		public virtual bool MoveNext()
		{
			return this._hasMore;
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00015C3C File Offset: 0x00013E3C
		public virtual object Current()
		{
			bool isPrimary = this._isPrimary;
			if (isPrimary)
			{
				this._isPrimary = false;
				this.ParameterOffset = this.primarySetupOffset + this.SetupCount * 2 + 2;
				bool flag = this.Command != 160;
				if (flag)
				{
					bool flag2 = this.Command == 37 && !this.IsResponse();
					if (flag2)
					{
						this.ParameterOffset += this.StringWireLength(this.Name, this.ParameterOffset);
					}
				}
				else
				{
					bool flag3 = this.Command == 160;
					if (flag3)
					{
						this.ParameterOffset += 2;
					}
				}
				this._pad = this.ParameterOffset % 2;
				this._pad = ((this._pad == 0) ? 0 : (2 - this._pad));
				this.ParameterOffset += this._pad;
				this.TotalParameterCount = this.WriteParametersWireFormat(this.TxnBuf, this._bufParameterOffset);
				this._bufDataOffset = this.TotalParameterCount;
				int num = this.MaxBufferSize - this.ParameterOffset;
				this.ParameterCount = Math.Min(this.TotalParameterCount, num);
				num -= this.ParameterCount;
				this.DataOffset = this.ParameterOffset + this.ParameterCount;
				this._pad1 = this.DataOffset % 2;
				this._pad1 = ((this._pad1 == 0) ? 0 : (2 - this._pad1));
				this.DataOffset += this._pad1;
				this.TotalDataCount = this.WriteDataWireFormat(this.TxnBuf, this._bufDataOffset);
				this.DataCount = Math.Min(this.TotalDataCount, num);
			}
			else
			{
				bool flag4 = this.Command != 160;
				if (flag4)
				{
					this.Command = 38;
				}
				else
				{
					this.Command = 161;
				}
				this.ParameterOffset = 51;
				bool flag5 = this.TotalParameterCount - this.ParameterDisplacement > 0;
				if (flag5)
				{
					this._pad = this.ParameterOffset % 2;
					this._pad = ((this._pad == 0) ? 0 : (2 - this._pad));
					this.ParameterOffset += this._pad;
				}
				this.ParameterDisplacement += this.ParameterCount;
				int num2 = this.MaxBufferSize - this.ParameterOffset - this._pad;
				this.ParameterCount = Math.Min(this.TotalParameterCount - this.ParameterDisplacement, num2);
				num2 -= this.ParameterCount;
				this.DataOffset = this.ParameterOffset + this.ParameterCount;
				this._pad1 = this.DataOffset % 2;
				this._pad1 = ((this._pad1 == 0) ? 0 : (2 - this._pad1));
				this.DataOffset += this._pad1;
				this.DataDisplacement += this.DataCount;
				num2 -= this._pad1;
				this.DataCount = Math.Min(this.TotalDataCount - this.DataDisplacement, num2);
			}
			bool flag6 = this.ParameterDisplacement + this.ParameterCount >= this.TotalParameterCount && this.DataDisplacement + this.DataCount >= this.TotalDataCount;
			if (flag6)
			{
				this._hasMore = false;
			}
			return this;
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00015F98 File Offset: 0x00014198
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this.TotalParameterCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.TotalDataCount, dst, dstIndex);
			dstIndex += 2;
			bool flag = this.Command != 38;
			if (flag)
			{
				ServerMessageBlock.WriteInt2((long)this.MaxParameterCount, dst, dstIndex);
				dstIndex += 2;
				ServerMessageBlock.WriteInt2((long)this.MaxDataCount, dst, dstIndex);
				dstIndex += 2;
				dst[dstIndex++] = this.MaxSetupCount;
				dst[dstIndex++] = 0;
				ServerMessageBlock.WriteInt2((long)this._flags, dst, dstIndex);
				dstIndex += 2;
				ServerMessageBlock.WriteInt4((long)this.Timeout, dst, dstIndex);
				dstIndex += 4;
				dst[dstIndex++] = 0;
				dst[dstIndex++] = 0;
			}
			ServerMessageBlock.WriteInt2((long)this.ParameterCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.ParameterOffset, dst, dstIndex);
			dstIndex += 2;
			bool flag2 = this.Command == 38;
			if (flag2)
			{
				ServerMessageBlock.WriteInt2((long)this.ParameterDisplacement, dst, dstIndex);
				dstIndex += 2;
			}
			ServerMessageBlock.WriteInt2((long)this.DataCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)((this.DataCount == 0) ? 0 : this.DataOffset), dst, dstIndex);
			dstIndex += 2;
			bool flag3 = this.Command == 38;
			if (flag3)
			{
				ServerMessageBlock.WriteInt2((long)this.DataDisplacement, dst, dstIndex);
				dstIndex += 2;
			}
			else
			{
				dst[dstIndex++] = (byte)this.SetupCount;
				dst[dstIndex++] = 0;
				dstIndex += this.WriteSetupWireFormat(dst, dstIndex);
			}
			return dstIndex - num;
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x00016130 File Offset: 0x00014330
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			int num2 = this._pad;
			bool flag = this.Command == 37 && !this.IsResponse();
			if (flag)
			{
				dstIndex += this.WriteString(this.Name, dst, dstIndex);
			}
			bool flag2 = this.ParameterCount > 0;
			if (flag2)
			{
				while (num2-- > 0)
				{
					dst[dstIndex++] = 0;
				}
				Array.Copy(this.TxnBuf, this._bufParameterOffset, dst, dstIndex, this.ParameterCount);
				dstIndex += this.ParameterCount;
			}
			bool flag3 = this.DataCount > 0;
			if (flag3)
			{
				num2 = this._pad1;
				while (num2-- > 0)
				{
					dst[dstIndex++] = 0;
				}
				Array.Copy(this.TxnBuf, this._bufDataOffset, dst, dstIndex, this.DataCount);
				this._bufDataOffset += this.DataCount;
				dstIndex += this.DataCount;
			}
			return dstIndex - num;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00016238 File Offset: 0x00014438
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0001624C File Offset: 0x0001444C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000490 RID: 1168
		internal abstract int WriteSetupWireFormat(byte[] dst, int dstIndex);

		// Token: 0x06000491 RID: 1169
		internal abstract int WriteParametersWireFormat(byte[] dst, int dstIndex);

		// Token: 0x06000492 RID: 1170
		internal abstract int WriteDataWireFormat(byte[] dst, int dstIndex);

		// Token: 0x06000493 RID: 1171
		internal abstract int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x06000494 RID: 1172
		internal abstract int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x06000495 RID: 1173
		internal abstract int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x06000496 RID: 1174 RVA: 0x00016260 File Offset: 0x00014460
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				",totalParameterCount=",
				this.TotalParameterCount,
				",totalDataCount=",
				this.TotalDataCount,
				",maxParameterCount=",
				this.MaxParameterCount,
				",maxDataCount=",
				this.MaxDataCount,
				",maxSetupCount=",
				(int)this.MaxSetupCount,
				",flags=0x",
				Hexdump.ToHexString(this._flags, 2),
				",timeout=",
				this.Timeout,
				",parameterCount=",
				this.ParameterCount,
				",parameterOffset=",
				this.ParameterOffset,
				",parameterDisplacement=",
				this.ParameterDisplacement,
				",dataCount=",
				this.DataCount,
				",dataOffset=",
				this.DataOffset,
				",dataDisplacement=",
				this.DataDisplacement,
				",setupCount=",
				this.SetupCount,
				",pad=",
				this._pad,
				",pad1=",
				this._pad1
			});
		}

		// Token: 0x0400020B RID: 523
		private static readonly int DefaultMaxDataCount = Config.GetInt("jcifs.smb.client.transaction_buf_size", 65535) - 512;

		// Token: 0x0400020C RID: 524
		private const int PrimarySetupOffset = 61;

		// Token: 0x0400020D RID: 525
		private const int SecondaryParameterOffset = 51;

		// Token: 0x0400020E RID: 526
		private const int DisconnectTid = 1;

		// Token: 0x0400020F RID: 527
		private const int OneWayTransaction = 2;

		// Token: 0x04000210 RID: 528
		private const int PaddingSize = 2;

		// Token: 0x04000211 RID: 529
		private int _flags = 0;

		// Token: 0x04000212 RID: 530
		private int _fid;

		// Token: 0x04000213 RID: 531
		private int _pad;

		// Token: 0x04000214 RID: 532
		private int _pad1;

		// Token: 0x04000215 RID: 533
		private bool _hasMore = true;

		// Token: 0x04000216 RID: 534
		private bool _isPrimary = true;

		// Token: 0x04000217 RID: 535
		private int _bufParameterOffset;

		// Token: 0x04000218 RID: 536
		private int _bufDataOffset;

		// Token: 0x04000219 RID: 537
		internal const int TransactionBufSize = 65535;

		// Token: 0x0400021A RID: 538
		internal const byte Trans2FindFirst2 = 1;

		// Token: 0x0400021B RID: 539
		internal const byte Trans2FindNext2 = 2;

		// Token: 0x0400021C RID: 540
		internal const byte Trans2QueryFsInformation = 3;

		// Token: 0x0400021D RID: 541
		internal const byte Trans2QueryPathInformation = 5;

		// Token: 0x0400021E RID: 542
		internal const byte Trans2GetDfsReferral = 16;

		// Token: 0x0400021F RID: 543
		internal const byte Trans2SetFileInformation = 8;

		// Token: 0x04000220 RID: 544
		internal const int NetShareEnum = 0;

		// Token: 0x04000221 RID: 545
		internal const int NetServerEnum2 = 104;

		// Token: 0x04000222 RID: 546
		internal const int NetServerEnum3 = 215;

		// Token: 0x04000223 RID: 547
		internal const byte TransPeekNamedPipe = 35;

		// Token: 0x04000224 RID: 548
		internal const byte TransWaitNamedPipe = 83;

		// Token: 0x04000225 RID: 549
		internal const byte TransCallNamedPipe = 84;

		// Token: 0x04000226 RID: 550
		internal const byte TransTransactNamedPipe = 38;

		// Token: 0x04000227 RID: 551
		protected internal int primarySetupOffset;

		// Token: 0x04000228 RID: 552
		protected internal int secondaryParameterOffset;

		// Token: 0x04000229 RID: 553
		protected internal int ParameterCount;

		// Token: 0x0400022A RID: 554
		protected internal int ParameterOffset;

		// Token: 0x0400022B RID: 555
		protected internal int ParameterDisplacement;

		// Token: 0x0400022C RID: 556
		protected internal int DataCount;

		// Token: 0x0400022D RID: 557
		protected internal int DataOffset;

		// Token: 0x0400022E RID: 558
		protected internal int DataDisplacement;

		// Token: 0x0400022F RID: 559
		internal int TotalParameterCount;

		// Token: 0x04000230 RID: 560
		internal int TotalDataCount;

		// Token: 0x04000231 RID: 561
		internal int MaxParameterCount;

		// Token: 0x04000232 RID: 562
		internal int MaxDataCount = SharpCifs.Smb.SmbComTransaction.DefaultMaxDataCount;

		// Token: 0x04000233 RID: 563
		internal byte MaxSetupCount;

		// Token: 0x04000234 RID: 564
		internal int Timeout = 0;

		// Token: 0x04000235 RID: 565
		internal int SetupCount = 1;

		// Token: 0x04000236 RID: 566
		internal byte SubCommand;

		// Token: 0x04000237 RID: 567
		internal string Name = string.Empty;

		// Token: 0x04000238 RID: 568
		internal int MaxBufferSize;

		// Token: 0x04000239 RID: 569
		internal byte[] TxnBuf;
	}
}
