using System;
using System.Text;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x0200006D RID: 109
	public class Ace
	{
		// Token: 0x06000322 RID: 802 RVA: 0x0000C884 File Offset: 0x0000AA84
		public virtual bool IsAllow()
		{
			return this.Allow;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000C89C File Offset: 0x0000AA9C
		public virtual bool IsInherited()
		{
			return (this.Flags & 16) != 0;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000C8BC File Offset: 0x0000AABC
		public virtual int GetFlags()
		{
			return this.Flags;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		public virtual string GetApplyToText()
		{
			switch (this.Flags & 11)
			{
			case 0:
				return "This folder only";
			case 1:
				return "This folder and files";
			case 2:
				return "This folder and subfolders";
			case 3:
				return "This folder, subfolders and files";
			case 9:
				return "Files only";
			case 10:
				return "Subfolders only";
			case 11:
				return "Subfolders and files only";
			}
			return "Invalid";
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000C96C File Offset: 0x0000AB6C
		public virtual int GetAccessMask()
		{
			return this.Access;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000C984 File Offset: 0x0000AB84
		public virtual Sid GetSid()
		{
			return this.Sid;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000C99C File Offset: 0x0000AB9C
		internal virtual int Decode(byte[] buf, int bi)
		{
			this.Allow = (buf[bi++] == 0);
			this.Flags = (int)(buf[bi++] & byte.MaxValue);
			int result = ServerMessageBlock.ReadInt2(buf, bi);
			bi += 2;
			this.Access = ServerMessageBlock.ReadInt4(buf, bi);
			bi += 4;
			this.Sid = new Sid(buf, bi);
			return result;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000CA00 File Offset: 0x0000AC00
		internal virtual void AppendCol(StringBuilder sb, string str, int width)
		{
			sb.Append(str);
			int num = width - str.Length;
			for (int i = 0; i < num; i++)
			{
				sb.Append(' ');
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000CA3C File Offset: 0x0000AC3C
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.IsAllow() ? "Allow " : "Deny  ");
			this.AppendCol(stringBuilder, this.Sid.ToDisplayString(), 25);
			stringBuilder.Append(" 0x").Append(Hexdump.ToHexString(this.Access, 8)).Append(' ');
			stringBuilder.Append(this.IsInherited() ? "Inherited " : "Direct    ");
			this.AppendCol(stringBuilder, this.GetApplyToText(), 34);
			return stringBuilder.ToString();
		}

		// Token: 0x0400009B RID: 155
		public const int FileReadData = 1;

		// Token: 0x0400009C RID: 156
		public const int FileWriteData = 2;

		// Token: 0x0400009D RID: 157
		public const int FileAppendData = 4;

		// Token: 0x0400009E RID: 158
		public const int FileReadEa = 8;

		// Token: 0x0400009F RID: 159
		public const int FileWriteEa = 16;

		// Token: 0x040000A0 RID: 160
		public const int FileExecute = 32;

		// Token: 0x040000A1 RID: 161
		public const int FileDelete = 64;

		// Token: 0x040000A2 RID: 162
		public const int FileReadAttributes = 128;

		// Token: 0x040000A3 RID: 163
		public const int FileWriteAttributes = 256;

		// Token: 0x040000A4 RID: 164
		public const int Delete = 65536;

		// Token: 0x040000A5 RID: 165
		public const int ReadControl = 131072;

		// Token: 0x040000A6 RID: 166
		public const int WriteDac = 262144;

		// Token: 0x040000A7 RID: 167
		public const int WriteOwner = 524288;

		// Token: 0x040000A8 RID: 168
		public const int Synchronize = 1048576;

		// Token: 0x040000A9 RID: 169
		public const int GenericAll = 268435456;

		// Token: 0x040000AA RID: 170
		public const int GenericExecute = 536870912;

		// Token: 0x040000AB RID: 171
		public const int GenericWrite = 1073741824;

		// Token: 0x040000AC RID: 172
		public const int GenericRead = -2147483648;

		// Token: 0x040000AD RID: 173
		public const int FlagsObjectInherit = 1;

		// Token: 0x040000AE RID: 174
		public const int FlagsContainerInherit = 2;

		// Token: 0x040000AF RID: 175
		public const int FlagsNoPropagate = 4;

		// Token: 0x040000B0 RID: 176
		public const int FlagsInheritOnly = 8;

		// Token: 0x040000B1 RID: 177
		public const int FlagsInherited = 16;

		// Token: 0x040000B2 RID: 178
		internal bool Allow;

		// Token: 0x040000B3 RID: 179
		internal int Flags;

		// Token: 0x040000B4 RID: 180
		internal int Access;

		// Token: 0x040000B5 RID: 181
		internal Sid Sid;
	}
}
