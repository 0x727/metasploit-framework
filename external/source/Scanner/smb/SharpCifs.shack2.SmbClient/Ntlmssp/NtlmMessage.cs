using System;

namespace SharpCifs.Ntlmssp
{
	// Token: 0x020000CC RID: 204
	public abstract class NtlmMessage : NtlmFlags
	{
		// Token: 0x06000685 RID: 1669 RVA: 0x00022ECC File Offset: 0x000210CC
		public virtual int GetFlags()
		{
			return this._flags;
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x00022EE4 File Offset: 0x000210E4
		public virtual void SetFlags(int flags)
		{
			this._flags = flags;
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x00022EF0 File Offset: 0x000210F0
		public virtual bool GetFlag(int flag)
		{
			return (this.GetFlags() & flag) != 0;
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00022F0D File Offset: 0x0002110D
		public virtual void SetFlag(int flag, bool value)
		{
			this.SetFlags(value ? (this.GetFlags() | flag) : (this.GetFlags() & (-1 ^ flag)));
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00022F30 File Offset: 0x00021130
		internal static int ReadULong(byte[] src, int index)
		{
			return (int)(src[index] & byte.MaxValue) | (int)(src[index + 1] & byte.MaxValue) << 8 | (int)(src[index + 2] & byte.MaxValue) << 16 | (int)(src[index + 3] & byte.MaxValue) << 24;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00022F78 File Offset: 0x00021178
		internal static int ReadUShort(byte[] src, int index)
		{
			return (int)(src[index] & byte.MaxValue) | (int)(src[index + 1] & byte.MaxValue) << 8;
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00022FA4 File Offset: 0x000211A4
		internal static byte[] ReadSecurityBuffer(byte[] src, int index)
		{
			int num = NtlmMessage.ReadUShort(src, index);
			int sourceIndex = NtlmMessage.ReadULong(src, index + 4);
			byte[] array = new byte[num];
			Array.Copy(src, sourceIndex, array, 0, num);
			return array;
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x00022FDB File Offset: 0x000211DB
		internal static void WriteULong(byte[] dest, int offset, int value)
		{
			dest[offset] = (byte)(value & 255);
			dest[offset + 1] = (byte)(value >> 8 & 255);
			dest[offset + 2] = (byte)(value >> 16 & 255);
			dest[offset + 3] = (byte)(value >> 24 & 255);
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x00023018 File Offset: 0x00021218
		internal static void WriteUShort(byte[] dest, int offset, int value)
		{
			dest[offset] = (byte)(value & 255);
			dest[offset + 1] = (byte)(value >> 8 & 255);
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x00023038 File Offset: 0x00021238
		internal static void WriteSecurityBuffer(byte[] dest, int offset, int bodyOffset, byte[] src)
		{
			int num = (src != null) ? src.Length : 0;
			bool flag = num == 0;
			if (!flag)
			{
				NtlmMessage.WriteUShort(dest, offset, num);
				NtlmMessage.WriteUShort(dest, offset + 2, num);
				NtlmMessage.WriteULong(dest, offset + 4, bodyOffset);
				Array.Copy(src, 0, dest, bodyOffset, num);
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x00023088 File Offset: 0x00021288
		internal static string GetOemEncoding()
		{
			return NtlmMessage.OemEncoding;
		}

		// Token: 0x06000690 RID: 1680
		public abstract byte[] ToByteArray();

		// Token: 0x040003FF RID: 1023
		protected internal static readonly byte[] NtlmsspSignature = new byte[]
		{
			78,
			84,
			76,
			77,
			83,
			83,
			80,
			0
		};

		// Token: 0x04000400 RID: 1024
		private static readonly string OemEncoding = Config.DefaultOemEncoding;

		// Token: 0x04000401 RID: 1025
		protected internal static readonly string UniEncoding = "UTF-16LE";

		// Token: 0x04000402 RID: 1026
		private int _flags;
	}
}
