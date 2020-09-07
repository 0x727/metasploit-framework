using System;
using System.IO;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000DC RID: 220
	public abstract class SessionServicePacket
	{
		// Token: 0x06000752 RID: 1874 RVA: 0x000265AA File Offset: 0x000247AA
		internal static void WriteInt2(int val, byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = (byte)(val >> 8 & 255);
			dst[dstIndex] = (byte)(val & 255);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0002862C File Offset: 0x0002682C
		internal static void WriteInt4(int val, byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = (byte)(val >> 24 & 255);
			dst[dstIndex++] = (byte)(val >> 16 & 255);
			dst[dstIndex++] = (byte)(val >> 8 & 255);
			dst[dstIndex] = (byte)(val & 255);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00028680 File Offset: 0x00026880
		internal static int ReadInt2(byte[] src, int srcIndex)
		{
			return ((int)(src[srcIndex] & byte.MaxValue) << 8) + (int)(src[srcIndex + 1] & byte.MaxValue);
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x000286AC File Offset: 0x000268AC
		internal static int ReadInt4(byte[] src, int srcIndex)
		{
			return ((int)(src[srcIndex] & byte.MaxValue) << 24) + ((int)(src[srcIndex + 1] & byte.MaxValue) << 16) + ((int)(src[srcIndex + 2] & byte.MaxValue) << 8) + (int)(src[srcIndex + 3] & byte.MaxValue);
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x000286F4 File Offset: 0x000268F4
		internal static int ReadLength(byte[] src, int srcIndex)
		{
			srcIndex++;
			return ((int)(src[srcIndex++] & 1) << 16) + ((int)(src[srcIndex++] & byte.MaxValue) << 8) + (int)(src[srcIndex++] & byte.MaxValue);
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00028738 File Offset: 0x00026938
		internal static int Readn(InputStream @in, byte[] b, int off, int len)
		{
			int i;
			int num;
			for (i = 0; i < len; i += num)
			{
				num = @in.Read(b, off + i, len - i);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
			}
			return i;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0002877C File Offset: 0x0002697C
		internal static int ReadPacketType(InputStream @in, byte[] buffer, int bufferIndex)
		{
			int num;
			bool flag = (num = SessionServicePacket.Readn(@in, buffer, bufferIndex, 4)) != 4;
			int result;
			if (flag)
			{
				bool flag2 = num == -1;
				if (!flag2)
				{
					throw new IOException("unexpected EOF reading netbios session header");
				}
				result = -1;
			}
			else
			{
				int num2 = (int)(buffer[bufferIndex] & byte.MaxValue);
				result = num2;
			}
			return result;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000287CC File Offset: 0x000269CC
		public virtual int WriteWireFormat(byte[] dst, int dstIndex)
		{
			this.Length = this.WriteTrailerWireFormat(dst, dstIndex + 4);
			this.WriteHeaderWireFormat(dst, dstIndex);
			return 4 + this.Length;
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00028800 File Offset: 0x00026A00
		internal virtual int ReadWireFormat(InputStream @in, byte[] buffer, int bufferIndex)
		{
			this.ReadHeaderWireFormat(@in, buffer, bufferIndex);
			return 4 + this.ReadTrailerWireFormat(@in, buffer, bufferIndex);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00028828 File Offset: 0x00026A28
		internal virtual int WriteHeaderWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex++] = (byte)this.Type;
			bool flag = this.Length > 65535;
			if (flag)
			{
				dst[dstIndex] = 1;
			}
			dstIndex++;
			SessionServicePacket.WriteInt2(this.Length, dst, dstIndex);
			return 4;
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00028874 File Offset: 0x00026A74
		internal virtual int ReadHeaderWireFormat(InputStream @in, byte[] buffer, int bufferIndex)
		{
			this.Type = (int)(buffer[bufferIndex++] & byte.MaxValue);
			this.Length = ((int)(buffer[bufferIndex] & 1) << 16) + SessionServicePacket.ReadInt2(buffer, bufferIndex + 1);
			return 4;
		}

		// Token: 0x0600075D RID: 1885
		internal abstract int WriteTrailerWireFormat(byte[] dst, int dstIndex);

		// Token: 0x0600075E RID: 1886
		internal abstract int ReadTrailerWireFormat(InputStream @in, byte[] buffer, int bufferIndex);

		// Token: 0x040004B1 RID: 1201
		internal const int SessionMessage = 0;

		// Token: 0x040004B2 RID: 1202
		internal const int SessionRequest = 129;

		// Token: 0x040004B3 RID: 1203
		public const int PositiveSessionResponse = 130;

		// Token: 0x040004B4 RID: 1204
		public const int NegativeSessionResponse = 131;

		// Token: 0x040004B5 RID: 1205
		internal const int SessionRetargetResponse = 132;

		// Token: 0x040004B6 RID: 1206
		internal const int SessionKeepAlive = 133;

		// Token: 0x040004B7 RID: 1207
		internal const int MaxMessageSize = 131071;

		// Token: 0x040004B8 RID: 1208
		internal const int HeaderLength = 4;

		// Token: 0x040004B9 RID: 1209
		internal int Type;

		// Token: 0x040004BA RID: 1210
		internal int Length;
	}
}
