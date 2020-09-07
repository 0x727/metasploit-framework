using System;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x0200009C RID: 156
	internal class SmbComSessionSetupAndX : AndXServerMessageBlock
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x000152E0 File Offset: 0x000134E0
		internal SmbComSessionSetupAndX(SmbSession session, ServerMessageBlock andx, object cred) : base(andx)
		{
			this.Command = 115;
			this.Session = session;
			this.Cred = cred;
			this._sessionKey = session.transport.SessionKey;
			this._capabilities = session.transport.Capabilities;
			bool flag = session.transport.Server.Security == SmbConstants.SecurityUser;
			if (flag)
			{
				bool flag2 = cred is NtlmPasswordAuthentication;
				if (flag2)
				{
					NtlmPasswordAuthentication ntlmPasswordAuthentication = (NtlmPasswordAuthentication)cred;
					bool flag3 = ntlmPasswordAuthentication == NtlmPasswordAuthentication.Anonymous;
					if (flag3)
					{
						this._lmHash = new byte[0];
						this._ntHash = new byte[0];
						this._capabilities &= ~SmbConstants.CapExtendedSecurity;
					}
					else
					{
						bool encryptedPasswords = session.transport.Server.EncryptedPasswords;
						if (encryptedPasswords)
						{
							this._lmHash = ntlmPasswordAuthentication.GetAnsiHash(session.transport.Server.EncryptionKey);
							this._ntHash = ntlmPasswordAuthentication.GetUnicodeHash(session.transport.Server.EncryptionKey);
							bool flag4 = this._lmHash.Length == 0 && this._ntHash.Length == 0;
							if (flag4)
							{
								throw new RuntimeException("Null setup prohibited.");
							}
						}
						else
						{
							bool disablePlainTextPasswords = SmbComSessionSetupAndX.DisablePlainTextPasswords;
							if (disablePlainTextPasswords)
							{
								throw new RuntimeException("Plain text passwords are disabled");
							}
							bool useUnicode = this.UseUnicode;
							if (useUnicode)
							{
								string password = ntlmPasswordAuthentication.GetPassword();
								this._lmHash = new byte[0];
								this._ntHash = new byte[(password.Length + 1) * 2];
								this.WriteString(password, this._ntHash, 0);
							}
							else
							{
								string password2 = ntlmPasswordAuthentication.GetPassword();
								this._lmHash = new byte[(password2.Length + 1) * 2];
								this._ntHash = new byte[0];
								this.WriteString(password2, this._lmHash, 0);
							}
						}
					}
					this._accountName = ntlmPasswordAuthentication.Username;
					bool useUnicode2 = this.UseUnicode;
					if (useUnicode2)
					{
						this._accountName = this._accountName.ToUpper();
					}
					this._primaryDomain = ntlmPasswordAuthentication.Domain.ToUpper();
				}
				else
				{
					bool flag5 = cred is byte[];
					if (!flag5)
					{
						throw new SmbException("Unsupported credential type");
					}
					this._blob = (byte[])cred;
				}
			}
			else
			{
				bool flag6 = session.transport.Server.Security == SmbConstants.SecurityShare;
				if (!flag6)
				{
					throw new SmbException("Unsupported");
				}
				bool flag7 = cred is NtlmPasswordAuthentication;
				if (!flag7)
				{
					throw new SmbException("Unsupported credential type");
				}
				NtlmPasswordAuthentication ntlmPasswordAuthentication2 = (NtlmPasswordAuthentication)cred;
				this._lmHash = new byte[0];
				this._ntHash = new byte[0];
				this._accountName = ntlmPasswordAuthentication2.Username;
				bool useUnicode3 = this.UseUnicode;
				if (useUnicode3)
				{
					this._accountName = this._accountName.ToUpper();
				}
				this._primaryDomain = ntlmPasswordAuthentication2.Domain.ToUpper();
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x000155E4 File Offset: 0x000137E4
		internal override int GetBatchLimit(byte command)
		{
			return (command == 117) ? SmbComSessionSetupAndX.BatchLimit : 0;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00015604 File Offset: 0x00013804
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			ServerMessageBlock.WriteInt2((long)this.Session.transport.SndBufSize, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)this.Session.transport.MaxMpxCount, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt2((long)SmbConstants.VcNumber, dst, dstIndex);
			dstIndex += 2;
			ServerMessageBlock.WriteInt4((long)this._sessionKey, dst, dstIndex);
			dstIndex += 4;
			bool flag = this._blob != null;
			if (flag)
			{
				ServerMessageBlock.WriteInt2((long)this._blob.Length, dst, dstIndex);
				dstIndex += 2;
			}
			else
			{
				ServerMessageBlock.WriteInt2((long)this._lmHash.Length, dst, dstIndex);
				dstIndex += 2;
				ServerMessageBlock.WriteInt2((long)this._ntHash.Length, dst, dstIndex);
				dstIndex += 2;
			}
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt4((long)this._capabilities, dst, dstIndex);
			dstIndex += 4;
			return dstIndex - num;
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001570C File Offset: 0x0001390C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			bool flag = this._blob != null;
			if (flag)
			{
				Array.Copy(this._blob, 0, dst, dstIndex, this._blob.Length);
				dstIndex += this._blob.Length;
			}
			else
			{
				Array.Copy(this._lmHash, 0, dst, dstIndex, this._lmHash.Length);
				dstIndex += this._lmHash.Length;
				Array.Copy(this._ntHash, 0, dst, dstIndex, this._ntHash.Length);
				dstIndex += this._ntHash.Length;
				dstIndex += this.WriteString(this._accountName, dst, dstIndex);
				dstIndex += this.WriteString(this._primaryDomain, dst, dstIndex);
			}
			dstIndex += this.WriteString(SmbConstants.NativeOs, dst, dstIndex);
			dstIndex += this.WriteString(SmbConstants.NativeLanman, dst, dstIndex);
			return dstIndex - num;
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x000157E8 File Offset: 0x000139E8
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x000157FC File Offset: 0x000139FC
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00015810 File Offset: 0x00013A10
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComSessionSetupAndX[",
				base.ToString(),
				",snd_buf_size=",
				this.Session.transport.SndBufSize,
				",maxMpxCount=",
				this.Session.transport.MaxMpxCount,
				",VC_NUMBER=",
				SmbConstants.VcNumber,
				",sessionKey=",
				this._sessionKey,
				",lmHash.length=",
				(this._lmHash == null) ? 0 : this._lmHash.Length,
				",ntHash.length=",
				(this._ntHash == null) ? 0 : this._ntHash.Length,
				",capabilities=",
				this._capabilities,
				",accountName=",
				this._accountName,
				",primaryDomain=",
				this._primaryDomain,
				",NATIVE_OS=",
				SmbConstants.NativeOs,
				",NATIVE_LANMAN=",
				SmbConstants.NativeLanman,
				"]"
			});
		}

		// Token: 0x040001FB RID: 507
		private static readonly int BatchLimit = Config.GetInt("jcifs.smb.client.SessionSetupAndX.TreeConnectAndX", 1);

		// Token: 0x040001FC RID: 508
		private static readonly bool DisablePlainTextPasswords = Config.GetBoolean("jcifs.smb.client.disablePlainTextPasswords", true);

		// Token: 0x040001FD RID: 509
		private byte[] _lmHash;

		// Token: 0x040001FE RID: 510
		private byte[] _ntHash;

		// Token: 0x040001FF RID: 511
		private byte[] _blob;

		// Token: 0x04000200 RID: 512
		private int _sessionKey;

		// Token: 0x04000201 RID: 513
		private int _capabilities;

		// Token: 0x04000202 RID: 514
		private string _accountName;

		// Token: 0x04000203 RID: 515
		private string _primaryDomain;

		// Token: 0x04000204 RID: 516
		internal SmbSession Session;

		// Token: 0x04000205 RID: 517
		internal object Cred;
	}
}
