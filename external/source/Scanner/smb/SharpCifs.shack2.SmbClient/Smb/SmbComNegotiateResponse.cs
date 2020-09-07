using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000090 RID: 144
	internal class SmbComNegotiateResponse : ServerMessageBlock
	{
		// Token: 0x0600042D RID: 1069 RVA: 0x000136DC File Offset: 0x000118DC
		internal SmbComNegotiateResponse(SmbTransport.ServerData server)
		{
			this.Server = server;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x000136F0 File Offset: 0x000118F0
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00013704 File Offset: 0x00011904
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00013718 File Offset: 0x00011918
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			this.DialectIndex = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
			bufferIndex += 2;
			bool flag = this.DialectIndex > 10;
			int result;
			if (flag)
			{
				result = bufferIndex - num;
			}
			else
			{
				this.Server.SecurityMode = (int)(buffer[bufferIndex++] & byte.MaxValue);
				this.Server.Security = (this.Server.SecurityMode & 1);
				this.Server.EncryptedPasswords = ((this.Server.SecurityMode & 2) == 2);
				this.Server.SignaturesEnabled = ((this.Server.SecurityMode & 4) == 4);
				this.Server.SignaturesRequired = ((this.Server.SecurityMode & 8) == 8);
				this.Server.MaxMpxCount = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this.Server.MaxNumberVcs = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this.Server.MaxBufferSize = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				this.Server.MaxRawSize = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				this.Server.SessionKey = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				this.Server.Capabilities = ServerMessageBlock.ReadInt4(buffer, bufferIndex);
				bufferIndex += 4;
				this.Server.ServerTime = ServerMessageBlock.ReadTime(buffer, bufferIndex);
				bufferIndex += 8;
				this.Server.ServerTimeZone = ServerMessageBlock.ReadInt2(buffer, bufferIndex);
				bufferIndex += 2;
				this.Server.EncryptionKeyLength = (int)(buffer[bufferIndex++] & byte.MaxValue);
				result = bufferIndex - num;
			}
			return result;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000138AC File Offset: 0x00011AAC
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			int num = bufferIndex;
			bool flag = (this.Server.Capabilities & SmbConstants.CapExtendedSecurity) == 0;
			if (flag)
			{
				this.Server.EncryptionKey = new byte[this.Server.EncryptionKeyLength];
				Array.Copy(buffer, bufferIndex, this.Server.EncryptionKey, 0, this.Server.EncryptionKeyLength);
				bufferIndex += this.Server.EncryptionKeyLength;
				bool flag2 = this.ByteCount > this.Server.EncryptionKeyLength;
				if (flag2)
				{
					int num2 = 0;
					try
					{
						bool flag3 = (this.Flags2 & SmbConstants.Flags2Unicode) == SmbConstants.Flags2Unicode;
						if (flag3)
						{
							while (buffer[bufferIndex + num2] != 0 || buffer[bufferIndex + num2 + 1] > 0)
							{
								num2 += 2;
								bool flag4 = num2 > 256;
								if (flag4)
								{
									throw new RuntimeException("zero termination not found");
								}
							}
							this.Server.OemDomainName = Runtime.GetStringForBytes(buffer, bufferIndex, num2, SmbConstants.UniEncoding);
						}
						else
						{
							while (buffer[bufferIndex + num2] > 0)
							{
								num2++;
								bool flag5 = num2 > 256;
								if (flag5)
								{
									throw new RuntimeException("zero termination not found");
								}
							}
							this.Server.OemDomainName = Runtime.GetStringForBytes(buffer, bufferIndex, num2, SmbConstants.OemEncoding);
						}
					}
					catch (UnsupportedEncodingException ex)
					{
						bool flag6 = ServerMessageBlock.Log.Level > 1;
						if (flag6)
						{
							Runtime.PrintStackTrace(ex, ServerMessageBlock.Log);
						}
					}
					bufferIndex += num2;
				}
				else
				{
					this.Server.OemDomainName = "";
				}
			}
			else
			{
				this.Server.Guid = new byte[16];
				Array.Copy(buffer, bufferIndex, this.Server.Guid, 0, 16);
				this.Server.OemDomainName = "";
			}
			return bufferIndex - num;
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00013A98 File Offset: 0x00011C98
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComNegotiateResponse[",
				base.ToString(),
				",wordCount=",
				this.WordCount,
				",dialectIndex=",
				this.DialectIndex,
				",securityMode=0x",
				Hexdump.ToHexString(this.Server.SecurityMode, 1),
				",security=",
				(this.Server.Security == SmbConstants.SecurityShare) ? "share" : "user",
				",encryptedPasswords=",
				this.Server.EncryptedPasswords.ToString(),
				",maxMpxCount=",
				this.Server.MaxMpxCount,
				",maxNumberVcs=",
				this.Server.MaxNumberVcs,
				",maxBufferSize=",
				this.Server.MaxBufferSize,
				",maxRawSize=",
				this.Server.MaxRawSize,
				",sessionKey=0x",
				Hexdump.ToHexString(this.Server.SessionKey, 8),
				",capabilities=0x",
				Hexdump.ToHexString(this.Server.Capabilities, 8),
				",serverTime=",
				Extensions.CreateDate(this.Server.ServerTime),
				",serverTimeZone=",
				this.Server.ServerTimeZone,
				",encryptionKeyLength=",
				this.Server.EncryptionKeyLength,
				",byteCount=",
				this.ByteCount,
				",oemDomainName=",
				this.Server.OemDomainName,
				"]"
			});
		}

		// Token: 0x0400019B RID: 411
		internal int DialectIndex;

		// Token: 0x0400019C RID: 412
		internal SmbTransport.ServerData Server;
	}
}
