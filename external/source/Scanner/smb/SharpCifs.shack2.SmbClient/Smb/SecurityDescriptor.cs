using System;
using System.IO;

namespace SharpCifs.Smb
{
	// Token: 0x02000083 RID: 131
	public class SecurityDescriptor
	{
		// Token: 0x060003B6 RID: 950 RVA: 0x00003195 File Offset: 0x00001395
		public SecurityDescriptor()
		{
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x0001068C File Offset: 0x0000E88C
		public SecurityDescriptor(byte[] buffer, int bufferIndex, int len)
		{
			this.Decode(buffer, bufferIndex, len);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000106A0 File Offset: 0x0000E8A0
		public virtual int Decode(byte[] buffer, int bufferIndex, int len)
		{
			int num = bufferIndex;
			bufferIndex++;
			bufferIndex++;
			this.Type = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			int num2 = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex = num + num2;
			bufferIndex++;
			bufferIndex++;
			int num3 = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			int num4 = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
			bufferIndex += 4;
			bool flag = num4 > 4096;
			if (flag)
			{
				throw new IOException("Invalid SecurityDescriptor");
			}
			bool flag2 = num2 != 0;
			if (flag2)
			{
				this.Aces = new Ace[num4];
				for (int i = 0; i < num4; i++)
				{
					this.Aces[i] = new Ace();
					bufferIndex += this.Aces[i].Decode(buffer, bufferIndex);
				}
			}
			else
			{
				this.Aces = null;
			}
			return bufferIndex - num;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000107A4 File Offset: 0x0000E9A4
		public override string ToString()
		{
			string text = "SecurityDescriptor:\n";
			bool flag = this.Aces != null;
			if (flag)
			{
				for (int i = 0; i < this.Aces.Length; i++)
				{
					text = text + this.Aces[i] + "\n";
				}
			}
			else
			{
				text += "NULL";
			}
			return text;
		}

		// Token: 0x04000147 RID: 327
		public int Type;

		// Token: 0x04000148 RID: 328
		public Ace[] Aces;
	}
}
