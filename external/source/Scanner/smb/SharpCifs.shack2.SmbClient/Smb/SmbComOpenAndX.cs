using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000095 RID: 149
	internal class SmbComOpenAndX : AndXServerMessageBlock
	{
		// Token: 0x06000443 RID: 1091 RVA: 0x00014608 File Offset: 0x00012808
		internal SmbComOpenAndX(string fileName, int access, int flags, ServerMessageBlock andx) : base(andx)
		{
			this.Path = fileName;
			this.Command = 45;
			this.DesiredAccess = (access & 3);
			bool flag = this.DesiredAccess == 3;
			if (flag)
			{
				this.DesiredAccess = 2;
			}
			this.DesiredAccess |= 64;
			this.DesiredAccess &= -2;
			this.SearchAttributes = (SmbConstants.AttrDirectory | SmbConstants.AttrHidden | SmbConstants.AttrSystem);
			this.FileAttributes = 0;
			bool flag2 = (flags & 64) == 64;
			if (flag2)
			{
				bool flag3 = (flags & 16) == 16;
				if (flag3)
				{
					this.OpenFunction = 18;
				}
				else
				{
					this.OpenFunction = 2;
				}
			}
			else
			{
				bool flag4 = (flags & 16) == 16;
				if (flag4)
				{
					bool flag5 = (flags & 32) == 32;
					if (flag5)
					{
						this.OpenFunction = 16;
					}
					else
					{
						this.OpenFunction = 17;
					}
				}
				else
				{
					this.OpenFunction = 1;
				}
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x000146FC File Offset: 0x000128FC
		internal override int GetBatchLimit(byte command)
		{
			return (command == 46) ? SmbComOpenAndX.BatchLimit : 0;
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x0001471C File Offset: 0x0001291C
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this.flags, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.DesiredAccess, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.SearchAttributes, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.FileAttributes, dst, dstIndex);
			dstIndex += 2;
			this.CreationTime = 0;
			ServerMessageBlock.WriteInt4((long)this.CreationTime, dst, dstIndex);
			dstIndex += 4;
			ServerMessageBlock.WriteInt2((long)this.OpenFunction, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this.AllocationSize, dst, dstIndex);
			dstIndex += 4;
			for (int i = 0; i < 8; i++)
			{
				dst[dstIndex++] = 0;
			}
			return dstIndex - num;
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x000147E4 File Offset: 0x000129E4
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			bool useUnicode = this.UseUnicode;
			if (useUnicode)
			{
				dst[dstIndex++] = 0;
			}
			dstIndex += this.WriteString(this.Path, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00014824 File Offset: 0x00012A24
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00014838 File Offset: 0x00012A38
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0001484C File Offset: 0x00012A4C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComOpenAndX[",
				base.ToString(),
				",flags=0x",
				Hexdump.ToHexString(this.flags, 2),
				",desiredAccess=0x",
				Hexdump.ToHexString(this.DesiredAccess, 4),
				",searchAttributes=0x",
				Hexdump.ToHexString(this.SearchAttributes, 4),
				",fileAttributes=0x",
				Hexdump.ToHexString(this.FileAttributes, 4),
				",creationTime=",
				Extensions.CreateDate((long)this.CreationTime),
				",openFunction=0x",
				Hexdump.ToHexString(this.OpenFunction, 2),
				",allocationSize=",
				this.AllocationSize,
				",fileName=",
				this.Path,
				"]"
			});
		}

		// Token: 0x040001C9 RID: 457
		private const int FlagsReturnAdditionalInfo = 1;

		// Token: 0x040001CA RID: 458
		private const int FlagsRequestOplock = 2;

		// Token: 0x040001CB RID: 459
		private const int FlagsRequestBatchOplock = 4;

		// Token: 0x040001CC RID: 460
		private const int SharingCompatibility = 0;

		// Token: 0x040001CD RID: 461
		private const int SharingDenyReadWriteExecute = 16;

		// Token: 0x040001CE RID: 462
		private const int SharingDenyWrite = 32;

		// Token: 0x040001CF RID: 463
		private const int SharingDenyReadExecute = 48;

		// Token: 0x040001D0 RID: 464
		private const int SharingDenyNone = 64;

		// Token: 0x040001D1 RID: 465
		private const int DoNotCache = 4096;

		// Token: 0x040001D2 RID: 466
		private const int WriteThrough = 16384;

		// Token: 0x040001D3 RID: 467
		private const int OpenFnCreate = 16;

		// Token: 0x040001D4 RID: 468
		private const int OpenFnFailIfExists = 0;

		// Token: 0x040001D5 RID: 469
		private const int OpenFnOpen = 1;

		// Token: 0x040001D6 RID: 470
		private const int OpenFnTrunc = 2;

		// Token: 0x040001D7 RID: 471
		private static readonly int BatchLimit = Config.GetInt("jcifs.smb.client.OpenAndX.ReadAndX", 1);

		// Token: 0x040001D8 RID: 472
		internal int flags;

		// Token: 0x040001D9 RID: 473
		internal int DesiredAccess;

		// Token: 0x040001DA RID: 474
		internal int SearchAttributes;

		// Token: 0x040001DB RID: 475
		internal int FileAttributes;

		// Token: 0x040001DC RID: 476
		internal int CreationTime;

		// Token: 0x040001DD RID: 477
		internal int OpenFunction;

		// Token: 0x040001DE RID: 478
		internal int AllocationSize;
	}
}
