using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200009F RID: 159
	internal abstract class SmbComTransactionResponse : ServerMessageBlock
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x0001641C File Offset: 0x0001461C
		public SmbComTransactionResponse()
		{
			this.TxnBuf = null;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001643C File Offset: 0x0001463C
		internal override void Reset()
		{
			base.Reset();
			this.BufDataStart = 0;
			this.IsPrimary = (this.HasMore = true);
			this._parametersDone = (this._dataDone = false);
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00016478 File Offset: 0x00014678
		public virtual bool MoveNext()
		{
			return this.ErrorCode == 0 && this.HasMore;
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001649C File Offset: 0x0001469C
		public virtual object Current()
		{
			bool isPrimary = this.IsPrimary;
			if (isPrimary)
			{
				this.IsPrimary = false;
			}
			return this;
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x000164C4 File Offset: 0x000146C4
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x000164D8 File Offset: 0x000146D8
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x000164EC File Offset: 0x000146EC
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.TotalParameterCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bool flag = this.BufDataStart == 0;
			if (flag)
			{
				this.BufDataStart = this.TotalParameterCount;
			}
			bufferIndex += 2;
			this.TotalDataCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 4;
			this.ParameterCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.ParameterOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.ParameterDisplacement = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DataCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DataOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.DataDisplacement = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			this.SetupCount = (int)(buffer[bufferIndex] & byte.MaxValue);
			bufferIndex += 2;
			bool flag2 = this.SetupCount != 0;
			if (flag2)
			{
				bool flag3 = ServerMessageBlock.Log.Level > 2;
				if (flag3)
				{
					ServerMessageBlock.Log.WriteLine("setupCount is not zero: " + this.SetupCount);
				}
			}
			return bufferIndex - num;
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00016608 File Offset: 0x00014808
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			this._pad = (this._pad1 = 0);
			bool flag = this.ParameterCount > 0;
			if (flag)
			{
				bufferIndex += (this._pad = this.ParameterOffset - (bufferIndex - this.HeaderStart));
				Array.Copy(buffer, bufferIndex, this.TxnBuf, this.BufParameterStart + this.ParameterDisplacement, this.ParameterCount);
				bufferIndex += this.ParameterCount;
			}
			bool flag2 = this.DataCount > 0;
			if (flag2)
			{
				bufferIndex += (this._pad1 = this.DataOffset - (bufferIndex - this.HeaderStart));
				Array.Copy(buffer, bufferIndex, this.TxnBuf, this.BufDataStart + this.DataDisplacement, this.DataCount);
				bufferIndex += this.DataCount;
			}
			bool flag3 = !this._parametersDone && this.ParameterDisplacement + this.ParameterCount == this.TotalParameterCount;
			if (flag3)
			{
				this._parametersDone = true;
			}
			bool flag4 = !this._dataDone && this.DataDisplacement + this.DataCount == this.TotalDataCount;
			if (flag4)
			{
				this._dataDone = true;
			}
			bool flag5 = this._parametersDone && this._dataDone;
			if (flag5)
			{
				this.HasMore = false;
				this.ReadParametersWireFormat(this.TxnBuf, this.BufParameterStart, this.TotalParameterCount);
				this.ReadDataWireFormat(this.TxnBuf, this.BufDataStart, this.TotalDataCount);
			}
			return this._pad + this.ParameterCount + this._pad1 + this.DataCount;
		}

		// Token: 0x060004A0 RID: 1184
		internal abstract int WriteSetupWireFormat(byte[] dst, int dstIndex);

		// Token: 0x060004A1 RID: 1185
		internal abstract int WriteParametersWireFormat(byte[] dst, int dstIndex);

		// Token: 0x060004A2 RID: 1186
		internal abstract int WriteDataWireFormat(byte[] dst, int dstIndex);

		// Token: 0x060004A3 RID: 1187
		internal abstract int ReadSetupWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x060004A4 RID: 1188
		internal abstract int ReadParametersWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x060004A5 RID: 1189
		internal abstract int ReadDataWireFormat(byte[] buffer, int bufferIndex, int len);

		// Token: 0x060004A6 RID: 1190 RVA: 0x000167A4 File Offset: 0x000149A4
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				",totalParameterCount=",
				this.TotalParameterCount,
				",totalDataCount=",
				this.TotalDataCount,
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

		// Token: 0x0400023A RID: 570
		private const int SetupOffset = 61;

		// Token: 0x0400023B RID: 571
		private const int DisconnectTid = 1;

		// Token: 0x0400023C RID: 572
		private const int OneWayTransaction = 2;

		// Token: 0x0400023D RID: 573
		private int _pad;

		// Token: 0x0400023E RID: 574
		private int _pad1;

		// Token: 0x0400023F RID: 575
		private bool _parametersDone;

		// Token: 0x04000240 RID: 576
		private bool _dataDone;

		// Token: 0x04000241 RID: 577
		protected internal int TotalParameterCount;

		// Token: 0x04000242 RID: 578
		protected internal int TotalDataCount;

		// Token: 0x04000243 RID: 579
		protected internal int ParameterCount;

		// Token: 0x04000244 RID: 580
		protected internal int ParameterOffset;

		// Token: 0x04000245 RID: 581
		protected internal int ParameterDisplacement;

		// Token: 0x04000246 RID: 582
		protected internal int DataOffset;

		// Token: 0x04000247 RID: 583
		protected internal int DataDisplacement;

		// Token: 0x04000248 RID: 584
		protected internal int SetupCount;

		// Token: 0x04000249 RID: 585
		protected internal int BufParameterStart;

		// Token: 0x0400024A RID: 586
		protected internal int BufDataStart;

		// Token: 0x0400024B RID: 587
		internal int DataCount;

		// Token: 0x0400024C RID: 588
		internal byte SubCommand;

		// Token: 0x0400024D RID: 589
		internal bool HasMore = true;

		// Token: 0x0400024E RID: 590
		internal bool IsPrimary = true;

		// Token: 0x0400024F RID: 591
		internal byte[] TxnBuf;

		// Token: 0x04000250 RID: 592
		internal int Status;

		// Token: 0x04000251 RID: 593
		internal int NumEntries;

		// Token: 0x04000252 RID: 594
		internal IFileEntry[] Results;
	}
}
