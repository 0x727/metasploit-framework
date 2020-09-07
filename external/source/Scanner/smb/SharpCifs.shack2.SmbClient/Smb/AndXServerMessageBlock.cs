using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x0200006F RID: 111
	internal abstract class AndXServerMessageBlock : ServerMessageBlock
	{
		// Token: 0x0600032E RID: 814 RVA: 0x0000CADB File Offset: 0x0000ACDB
		public AndXServerMessageBlock()
		{
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000CAF0 File Offset: 0x0000ACF0
		internal AndXServerMessageBlock(ServerMessageBlock andx)
		{
			bool flag = andx != null;
			if (flag)
			{
				this.Andx = andx;
				this._andxCommand = andx.Command;
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000CB30 File Offset: 0x0000AD30
		internal virtual int GetBatchLimit(byte command)
		{
			return 0;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000CB44 File Offset: 0x0000AD44
		internal override int Encode(byte[] dst, int dstIndex)
		{
			int num = this.HeaderStart = dstIndex;
			dstIndex += this.WriteHeaderWireFormat(dst, dstIndex);
			dstIndex += this.WriteAndXWireFormat(dst, dstIndex);
			this.Length = dstIndex - num;
			bool flag = this.Digest != null;
			if (flag)
			{
				this.Digest.Sign(dst, this.HeaderStart, this.Length, this, this.Response);
			}
			return this.Length;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000CBB8 File Offset: 0x0000ADB8
		internal override int Decode(byte[] buffer, int bufferIndex)
		{
			int num = this.HeaderStart = bufferIndex;
			bufferIndex += this.ReadHeaderWireFormat(buffer, bufferIndex);
			bufferIndex += this.ReadAndXWireFormat(buffer, bufferIndex);
			this.Length = bufferIndex - num;
			return this.Length;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000CBFC File Offset: 0x0000ADFC
		internal virtual int WriteAndXWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			this.WordCount = this.WriteParameterWordsWireFormat(dst, num + 3 + 2);
			this.WordCount += 4;
			dstIndex += this.WordCount + 1;
			this.WordCount /= 2;
			dst[num] = (byte)(this.WordCount & 255);
			this.ByteCount = this.WriteBytesWireFormat(dst, dstIndex + 2);
			dst[dstIndex++] = (byte)(this.ByteCount & 255);
			dst[dstIndex++] = (byte)(this.ByteCount >> 8 & 255);
			dstIndex += this.ByteCount;
			bool flag = this.Andx == null || !SmbConstants.UseBatching || this.BatchLevel >= this.GetBatchLimit(this.Andx.Command);
			int result;
			if (flag)
			{
				this._andxCommand = byte.MaxValue;
				this.Andx = null;
				dst[num + 1] = byte.MaxValue;
				dst[num + 2] = 0;
				dst[num + 3] = 222;
				dst[num + 3 + 1] = 222;
				result = dstIndex - num;
			}
			else
			{
				this.Andx.BatchLevel = this.BatchLevel + 1;
				dst[num + 1] = this._andxCommand;
				dst[num + 2] = 0;
				this._andxOffset = dstIndex - this.HeaderStart;
				ServerMessageBlock.WriteInt2((long)this._andxOffset, dst, num + 3);
				this.Andx.UseUnicode = this.UseUnicode;
				bool flag2 = this.Andx is AndXServerMessageBlock;
				if (flag2)
				{
					this.Andx.Uid = this.Uid;
					dstIndex += ((AndXServerMessageBlock)this.Andx).WriteAndXWireFormat(dst, dstIndex);
				}
				else
				{
					int num2 = dstIndex;
					this.Andx.WordCount = this.Andx.WriteParameterWordsWireFormat(dst, dstIndex);
					dstIndex += this.Andx.WordCount + 1;
					this.Andx.WordCount /= 2;
					dst[num2] = (byte)(this.Andx.WordCount & 255);
					this.Andx.ByteCount = this.Andx.WriteBytesWireFormat(dst, dstIndex + 2);
					dst[dstIndex++] = (byte)(this.Andx.ByteCount & 255);
					dst[dstIndex++] = (byte)(this.Andx.ByteCount >> 8 & 255);
					dstIndex += this.Andx.ByteCount;
				}
				result = dstIndex - num;
			}
			return result;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000CE64 File Offset: 0x0000B064
		internal virtual int ReadAndXWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.WordCount = (int)buffer[bufferIndex++];
			bool flag = this.WordCount != 0;
			if (flag)
			{
				this._andxCommand = buffer[bufferIndex];
				this._andxOffset = ServerMessageBlock.ReadInt2(buffer, bufferIndex + 2);
				bool flag2 = this._andxOffset == 0;
				if (flag2)
				{
					this._andxCommand = byte.MaxValue;
				}
				bool flag3 = this.WordCount > 2;
				if (flag3)
				{
					this.ReadParameterWordsWireFormat(buffer, bufferIndex + 4);
					bool flag4 = this.Command == 162 && ((SmbComNtCreateAndXResponse)this).IsExtended;
					if (flag4)
					{
						this.WordCount += 8;
					}
				}
				bufferIndex = num + 1 + this.WordCount * 2;
			}
			this.ByteCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			bool flag5 = this.ByteCount != 0;
			if (flag5)
			{
				int num2 = this.ReadBytesWireFormat(buffer, bufferIndex);
				bufferIndex += this.ByteCount;
			}
			bool flag6 = this.ErrorCode != 0 || this._andxCommand == byte.MaxValue;
			if (flag6)
			{
				this._andxCommand = byte.MaxValue;
				this.Andx = null;
			}
			else
			{
				bool flag7 = this.Andx == null;
				if (flag7)
				{
					this._andxCommand = byte.MaxValue;
					throw new RuntimeException("no andx command supplied with response");
				}
				bufferIndex = this.HeaderStart + this._andxOffset;
				this.Andx.HeaderStart = this.HeaderStart;
				this.Andx.Command = this._andxCommand;
				this.Andx.ErrorCode = this.ErrorCode;
				this.Andx.Flags = this.Flags;
				this.Andx.Flags2 = this.Flags2;
				this.Andx.Tid = this.Tid;
				this.Andx.Pid = this.Pid;
				this.Andx.Uid = this.Uid;
				this.Andx.Mid = this.Mid;
				this.Andx.UseUnicode = this.UseUnicode;
				bool flag8 = this.Andx is AndXServerMessageBlock;
				if (flag8)
				{
					bufferIndex += ((AndXServerMessageBlock)this.Andx).ReadAndXWireFormat(buffer, bufferIndex);
				}
				else
				{
					buffer[bufferIndex++] = (byte)(this.Andx.WordCount & 255);
					bool flag9 = this.Andx.WordCount != 0;
					if (flag9)
					{
						bool flag10 = this.Andx.WordCount > 2;
						if (flag10)
						{
							bufferIndex += this.Andx.ReadParameterWordsWireFormat(buffer, bufferIndex);
						}
					}
					this.Andx.ByteCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
					bufferIndex += 2;
					bool flag11 = this.Andx.ByteCount != 0;
					if (flag11)
					{
						this.Andx.ReadBytesWireFormat(buffer, bufferIndex);
						bufferIndex += this.Andx.ByteCount;
					}
				}
				this.Andx.Received = true;
			}
			return bufferIndex - num;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000D158 File Offset: 0x0000B358
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				",andxCommand=0x",
				Hexdump.ToHexString((int)this._andxCommand, 2),
				",andxOffset=",
				this._andxOffset
			});
		}

		// Token: 0x040000B6 RID: 182
		private const int AndxCommandOffset = 1;

		// Token: 0x040000B7 RID: 183
		private const int AndxReservedOffset = 2;

		// Token: 0x040000B8 RID: 184
		private const int AndxOffsetOffset = 3;

		// Token: 0x040000B9 RID: 185
		private byte _andxCommand = byte.MaxValue;

		// Token: 0x040000BA RID: 186
		private int _andxOffset;

		// Token: 0x040000BB RID: 187
		internal ServerMessageBlock Andx;
	}
}
