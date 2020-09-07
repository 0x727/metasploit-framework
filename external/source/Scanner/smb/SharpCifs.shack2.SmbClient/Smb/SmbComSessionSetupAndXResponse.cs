using System;

namespace SharpCifs.Smb
{
	// Token: 0x0200009D RID: 157
	internal class SmbComSessionSetupAndXResponse : AndXServerMessageBlock
	{
		// Token: 0x06000481 RID: 1153 RVA: 0x00015984 File Offset: 0x00013B84
		internal SmbComSessionSetupAndXResponse(ServerMessageBlock andx) : base(andx)
		{
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x000159B0 File Offset: 0x00013BB0
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x000159C4 File Offset: 0x00013BC4
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x000159D8 File Offset: 0x00013BD8
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.IsLoggedInAsGuest = ((buffer[bufferIndex] & 1) == 1);
			bufferIndex += 2;
			bool extendedSecurity = this.ExtendedSecurity;
			if (extendedSecurity)
			{
				int num2 = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this.Blob = new byte[num2];
			}
			return bufferIndex - num;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00015A2C File Offset: 0x00013C2C
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			bool extendedSecurity = this.ExtendedSecurity;
			if (extendedSecurity)
			{
				Array.Copy(buffer, bufferIndex, this.Blob, 0, this.Blob.Length);
				bufferIndex += this.Blob.Length;
			}
			this._nativeOs = this.ReadString(buffer, bufferIndex);
			bufferIndex += this.StringWireLength(this._nativeOs, bufferIndex);
			this._nativeLanMan = this.ReadString(buffer, bufferIndex, num + this.ByteCount, 255, this.UseUnicode);
			bufferIndex += this.StringWireLength(this._nativeLanMan, bufferIndex);
			bool flag = !this.ExtendedSecurity;
			if (flag)
			{
				this._primaryDomain = this.ReadString(buffer, bufferIndex, num + this.ByteCount, 255, this.UseUnicode);
				bufferIndex += this.StringWireLength(this._primaryDomain, bufferIndex);
			}
			return bufferIndex - num;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x00015B04 File Offset: 0x00013D04
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"SmbComSessionSetupAndXResponse[",
				base.ToString(),
				",isLoggedInAsGuest=",
				this.IsLoggedInAsGuest.ToString(),
				",nativeOs=",
				this._nativeOs,
				",nativeLanMan=",
				this._nativeLanMan,
				",primaryDomain=",
				this._primaryDomain,
				"]"
			});
		}

		// Token: 0x04000206 RID: 518
		private string _nativeOs = string.Empty;

		// Token: 0x04000207 RID: 519
		private string _nativeLanMan = string.Empty;

		// Token: 0x04000208 RID: 520
		private string _primaryDomain = string.Empty;

		// Token: 0x04000209 RID: 521
		internal bool IsLoggedInAsGuest;

		// Token: 0x0400020A RID: 522
		internal byte[] Blob;
	}
}
