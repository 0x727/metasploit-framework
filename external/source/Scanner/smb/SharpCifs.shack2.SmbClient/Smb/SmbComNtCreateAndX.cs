using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x02000091 RID: 145
	internal class SmbComNtCreateAndX : AndXServerMessageBlock
	{
		// Token: 0x06000433 RID: 1075 RVA: 0x00013C9C File Offset: 0x00011E9C
		internal SmbComNtCreateAndX(string name, int flags, int access, int shareAccess, int extFileAttributes, int createOptions, ServerMessageBlock andx) : base(andx)
		{
			this.Path = name;
			this.Command = 162;
			this.DesiredAccess = access;
			this.DesiredAccess |= (SmbConstants.FileReadData | SmbConstants.FileReadEa | SmbConstants.FileReadAttributes);
			this._extFileAttributes = extFileAttributes;
			this._shareAccess = shareAccess;
			bool flag = (flags & 64) == 64;
			if (flag)
			{
				bool flag2 = (flags & 16) == 16;
				if (flag2)
				{
					this._createDisposition = 5;
				}
				else
				{
					this._createDisposition = 4;
				}
			}
			else
			{
				bool flag3 = (flags & 16) == 16;
				if (flag3)
				{
					bool flag4 = (flags & 32) == 32;
					if (flag4)
					{
						this._createDisposition = 2;
					}
					else
					{
						this._createDisposition = 3;
					}
				}
				else
				{
					this._createDisposition = 1;
				}
			}
			bool flag5 = (createOptions & 1) == 0;
			if (flag5)
			{
				this._createOptions = (createOptions | 64);
			}
			else
			{
				this._createOptions = createOptions;
			}
			this._impersonationLevel = 2;
			this._securityFlags = 3;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00013D9C File Offset: 0x00011F9C
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 0;
			this._namelenIndex = dstIndex;
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this.Flags0, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this._rootDirectoryFid, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this.DesiredAccess, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt8(this._allocationSize, dst, dstIndex);
			dstIndex += 8;
			ServerMessageBlock.WriteInt4((long)this._extFileAttributes, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this._shareAccess, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this._createDisposition, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this._createOptions, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt4((long)this._impersonationLevel, dst, dstIndex);
			dstIndex += 4;
			dst[dstIndex++] = this._securityFlags;
			return dstIndex - num;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00013E8C File Offset: 0x0001208C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = this.WriteString(this.Path, dst, dstIndex);
			ServerMessageBlock.WriteInt2((long)(this.UseUnicode ? (this.Path.Length * 2) : num), dst, this._namelenIndex);
			return num;
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00013ED4 File Offset: 0x000120D4
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00013EE8 File Offset: 0x000120E8
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00013EFC File Offset: 0x000120FC
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComNTCreateAndX[",
				base.ToString(),
				",flags=0x",
				Hexdump.ToHexString(this.Flags0, 2),
				",rootDirectoryFid=",
				this._rootDirectoryFid,
				",desiredAccess=0x",
				Hexdump.ToHexString(this.DesiredAccess, 4),
				",allocationSize=",
				this._allocationSize,
				",extFileAttributes=0x",
				Hexdump.ToHexString(this._extFileAttributes, 4),
				",shareAccess=0x",
				Hexdump.ToHexString(this._shareAccess, 4),
				",createDisposition=0x",
				Hexdump.ToHexString(this._createDisposition, 4),
				",createOptions=0x",
				Hexdump.ToHexString(this._createOptions, 8),
				",impersonationLevel=0x",
				Hexdump.ToHexString(this._impersonationLevel, 4),
				",securityFlags=0x",
				Hexdump.ToHexString((int)this._securityFlags, 2),
				",name=",
				this.Path,
				"]"
			});
		}

		// Token: 0x0400019D RID: 413
		internal const int FileSupersede = 0;

		// Token: 0x0400019E RID: 414
		internal const int FileOpen = 1;

		// Token: 0x0400019F RID: 415
		internal const int FileCreate = 2;

		// Token: 0x040001A0 RID: 416
		internal const int FileOpenIf = 3;

		// Token: 0x040001A1 RID: 417
		internal const int FileOverwrite = 4;

		// Token: 0x040001A2 RID: 418
		internal const int FileOverwriteIf = 5;

		// Token: 0x040001A3 RID: 419
		internal const int FileWriteThrough = 2;

		// Token: 0x040001A4 RID: 420
		internal const int FileSequentialOnly = 4;

		// Token: 0x040001A5 RID: 421
		internal const int FileSynchronousIoAlert = 16;

		// Token: 0x040001A6 RID: 422
		internal const int FileSynchronousIoNonalert = 32;

		// Token: 0x040001A7 RID: 423
		internal const int SecurityContextTracking = 1;

		// Token: 0x040001A8 RID: 424
		internal const int SecurityEffectiveOnly = 2;

		// Token: 0x040001A9 RID: 425
		private int _rootDirectoryFid;

		// Token: 0x040001AA RID: 426
		private int _extFileAttributes;

		// Token: 0x040001AB RID: 427
		private int _shareAccess;

		// Token: 0x040001AC RID: 428
		private int _createDisposition;

		// Token: 0x040001AD RID: 429
		private int _createOptions;

		// Token: 0x040001AE RID: 430
		private int _impersonationLevel;

		// Token: 0x040001AF RID: 431
		private long _allocationSize;

		// Token: 0x040001B0 RID: 432
		private byte _securityFlags;

		// Token: 0x040001B1 RID: 433
		private int _namelenIndex;

		// Token: 0x040001B2 RID: 434
		internal int Flags0;

		// Token: 0x040001B3 RID: 435
		internal int DesiredAccess;
	}
}
