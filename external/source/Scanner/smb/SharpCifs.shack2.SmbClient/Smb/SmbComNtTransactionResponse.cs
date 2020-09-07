using System;

namespace SharpCifs.Smb
{
	// Token: 0x02000094 RID: 148
	internal abstract class SmbComNtTransactionResponse : SmbComTransactionResponse
	{
		// Token: 0x06000441 RID: 1089 RVA: 0x000144D0 File Offset: 0x000126D0
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			buffer[bufferIndex++] = 0;
			buffer[bufferIndex++] = 0;
			buffer[bufferIndex++] = 0;
			this.TotalParameterCount = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bool flag = this.BufDataStart == 0;
			if (flag)
			{
				this.BufDataStart = this.TotalParameterCount;
			}
			bufferIndex += 4;
			this.TotalDataCount = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.ParameterCount = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.ParameterOffset = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.ParameterDisplacement = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.DataCount = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.DataOffset = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.DataDisplacement = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			this.SetupCount = (int)(buffer[bufferIndex] & byte.MaxValue);
			bufferIndex += 2;
			bool flag2 = this.SetupCount != 0;
			if (flag2)
			{
				bool flag3 = ServerMessageBlock.Log.Level >= 3;
				if (flag3)
				{
					ServerMessageBlock.Log.WriteLine("setupCount is not zero: " + this.SetupCount);
				}
			}
			return bufferIndex - num;
		}
	}
}
