using System;
using SharpCifs.Util;

namespace SharpCifs.Smb
{
	// Token: 0x0200009B RID: 155
	internal class SmbComRename : ServerMessageBlock
	{
		// Token: 0x06000473 RID: 1139 RVA: 0x00015189 File Offset: 0x00013389
		internal SmbComRename(string oldFileName, string newFileName)
		{
			this.Command = 7;
			this._oldFileName = oldFileName;
			this._newFileName = newFileName;
			this._searchAttributes = (SmbConstants.AttrHidden | SmbConstants.AttrSystem | SmbConstants.AttrDirectory);
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x000151C0 File Offset: 0x000133C0
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			ServerMessageBlock.WriteInt2((long)this._searchAttributes, dst, dstIndex);
			return 2;
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000151E4 File Offset: 0x000133E4
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			dst[dstIndex++] = 4;
			dstIndex += this.WriteString(this._oldFileName, dst, dstIndex);
			dst[dstIndex++] = 4;
			bool useUnicode = this.UseUnicode;
			if (useUnicode)
			{
				dst[dstIndex++] = 0;
			}
			dstIndex += this.WriteString(this._newFileName, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00015248 File Offset: 0x00013448
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001525C File Offset: 0x0001345C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00015270 File Offset: 0x00013470
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComRename[",
				base.ToString(),
				",searchAttributes=0x",
				Hexdump.ToHexString(this._searchAttributes, 4),
				",oldFileName=",
				this._oldFileName,
				",newFileName=",
				this._newFileName,
				"]"
			});
		}

		// Token: 0x040001F8 RID: 504
		private int _searchAttributes;

		// Token: 0x040001F9 RID: 505
		private string _oldFileName;

		// Token: 0x040001FA RID: 506
		private string _newFileName;
	}
}
