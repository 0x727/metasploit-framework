using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x020000A0 RID: 160
	internal class SmbComTreeConnectAndX : AndXServerMessageBlock
	{
		// Token: 0x060004A7 RID: 1191 RVA: 0x000168CC File Offset: 0x00014ACC
		static SmbComTreeConnectAndX()
		{
			string property;
			bool flag = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.CheckDirectory")) != null;
			if (flag)
			{
				SmbComTreeConnectAndX._batchLimits[0] = byte.Parse(property);
			}
			bool flag2 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.CreateDirectory")) != null;
			if (flag2)
			{
				SmbComTreeConnectAndX._batchLimits[2] = byte.Parse(property);
			}
			bool flag3 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.Delete")) != null;
			if (flag3)
			{
				SmbComTreeConnectAndX._batchLimits[3] = byte.Parse(property);
			}
			bool flag4 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.DeleteDirectory")) != null;
			if (flag4)
			{
				SmbComTreeConnectAndX._batchLimits[4] = byte.Parse(property);
			}
			bool flag5 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.OpenAndX")) != null;
			if (flag5)
			{
				SmbComTreeConnectAndX._batchLimits[5] = byte.Parse(property);
			}
			bool flag6 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.Rename")) != null;
			if (flag6)
			{
				SmbComTreeConnectAndX._batchLimits[6] = byte.Parse(property);
			}
			bool flag7 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.Transaction")) != null;
			if (flag7)
			{
				SmbComTreeConnectAndX._batchLimits[7] = byte.Parse(property);
			}
			bool flag8 = (property = Config.GetProperty("jcifs.smb.client.TreeConnectAndX.QueryInformation")) != null;
			if (flag8)
			{
				SmbComTreeConnectAndX._batchLimits[8] = byte.Parse(property);
			}
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00016A1B File Offset: 0x00014C1B
		internal SmbComTreeConnectAndX(SmbSession session, string path, string service, ServerMessageBlock andx) : base(andx)
		{
			this._session = session;
			this.path = path;
			this._service = service;
			this.Command = 117;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00016A4C File Offset: 0x00014C4C
		internal override int GetBatchLimit(byte command)
		{
			int num = (int)(command & byte.MaxValue);
			int num2 = num;
			if (num2 <= 16)
			{
				switch (num2)
				{
				case 0:
					return (int)SmbComTreeConnectAndX._batchLimits[2];
				case 1:
					return (int)SmbComTreeConnectAndX._batchLimits[4];
				case 2:
				case 3:
				case 4:
				case 5:
					break;
				case 6:
					return (int)SmbComTreeConnectAndX._batchLimits[3];
				case 7:
					return (int)SmbComTreeConnectAndX._batchLimits[6];
				case 8:
					return (int)SmbComTreeConnectAndX._batchLimits[8];
				default:
					if (num2 == 16)
					{
						return (int)SmbComTreeConnectAndX._batchLimits[0];
					}
					break;
				}
			}
			else
			{
				if (num2 == 37)
				{
					return (int)SmbComTreeConnectAndX._batchLimits[7];
				}
				if (num2 == 45)
				{
					return (int)SmbComTreeConnectAndX._batchLimits[5];
				}
			}
			return 0;
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00016B08 File Offset: 0x00014D08
		internal override int WriteParameterWordsWireFormat(byte[] dst, int dstIndex)
		{
			bool flag = this._session.transport.Server.Security == SmbConstants.SecurityShare && (this._session.Auth.HashesExternal || this._session.Auth.Password.Length > 0);
			if (flag)
			{
				bool encryptedPasswords = this._session.transport.Server.EncryptedPasswords;
				if (encryptedPasswords)
				{
					this._password = this._session.Auth.GetAnsiHash(this._session.transport.Server.EncryptionKey);
					this._passwordLength = this._password.Length;
				}
				else
				{
					bool disablePlainTextPasswords = SmbComTreeConnectAndX.DisablePlainTextPasswords;
					if (disablePlainTextPasswords)
					{
						throw new RuntimeException("Plain text passwords are disabled");
					}
					this._password = new byte[(this._session.Auth.Password.Length + 1) * 2];
					this._passwordLength = this.WriteString(this._session.Auth.Password, this._password, 0);
				}
			}
			else
			{
				this._passwordLength = 1;
			}
			dst[dstIndex++] = (byte)(this._disconnectTid ? 1 : 0);
			dst[dstIndex++] = 0;
			ServerMessageBlock.WriteInt2((long)this._passwordLength, dst, dstIndex);
			return 4;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00016C5C File Offset: 0x00014E5C
		internal override int WriteBytesWireFormat(byte[] dst, int dstIndex)
		{
			int num = dstIndex;
			bool flag = this._session.transport.Server.Security == SmbConstants.SecurityShare && (this._session.Auth.HashesExternal || this._session.Auth.Password.Length > 0);
			if (flag)
			{
				Array.Copy(this._password, 0, dst, dstIndex, this._passwordLength);
				dstIndex += this._passwordLength;
			}
			else
			{
				dst[dstIndex++] = 0;
			}
			dstIndex += this.WriteString(this.path, dst, dstIndex);
			try
			{
				Array.Copy(Runtime.GetBytesForString(this._service, "UTF-8"), 0, dst, dstIndex, this._service.Length);
			}
			catch (UnsupportedEncodingException)
			{
				return 0;
			}
			dstIndex += this._service.Length;
			dst[dstIndex++] = 0;
			return dstIndex - num;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00016D5C File Offset: 0x00014F5C
		internal override int ReadParameterWordsWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00016D70 File Offset: 0x00014F70
		internal override int ReadBytesWireFormat(byte[] buffer, int bufferIndex)
		{
			return 0;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00016D84 File Offset: 0x00014F84
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"SmbComTreeConnectAndX[",
				base.ToString(),
				",disconnectTid=",
				this._disconnectTid.ToString(),
				",passwordLength=",
				this._passwordLength,
				",password=",
				Hexdump.ToHexString(this._password, this._passwordLength, 0),
				",path=",
				this.path,
				",service=",
				this._service,
				"]"
			});
		}

		// Token: 0x04000253 RID: 595
		private static readonly bool DisablePlainTextPasswords = Config.GetBoolean("jcifs.smb.client.disablePlainTextPasswords", true);

		// Token: 0x04000254 RID: 596
		private SmbSession _session;

		// Token: 0x04000255 RID: 597
		private bool _disconnectTid = false;

		// Token: 0x04000256 RID: 598
		private string _service;

		// Token: 0x04000257 RID: 599
		private byte[] _password;

		// Token: 0x04000258 RID: 600
		private int _passwordLength;

		// Token: 0x04000259 RID: 601
		internal string path;

		// Token: 0x0400025A RID: 602
		private static byte[] _batchLimits = new byte[]
		{
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			0
		};
	}
}
