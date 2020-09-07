using System;
using SharpCifs.Ntlmssp;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x0200007D RID: 125
	public class NtlmContext
	{
		// Token: 0x0600037F RID: 895 RVA: 0x0000EDCC File Offset: 0x0000CFCC
		public NtlmContext(NtlmPasswordAuthentication auth, bool doSigning)
		{
			this.Auth = auth;
			this.NtlmsspFlags = (this.NtlmsspFlags | 4 | 524288 | 536870912);
			if (doSigning)
			{
				this.NtlmsspFlags |= 1073774608;
			}
			this.Workstation = Type1Message.GetDefaultWorkstation();
			this.Log = LogStream.GetInstance();
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000EE40 File Offset: 0x0000D040
		public override string ToString()
		{
			string str = string.Concat(new object[]
			{
				"NtlmContext[auth=",
				this.Auth,
				",ntlmsspFlags=0x",
				Hexdump.ToHexString(this.NtlmsspFlags, 8),
				",workstation=",
				this.Workstation,
				",isEstablished=",
				this.isEstablished.ToString(),
				",state=",
				this.State,
				",serverChallenge="
			});
			bool flag = this.ServerChallenge == null;
			if (flag)
			{
				str += "null";
			}
			else
			{
				str += Hexdump.ToHexString(this.ServerChallenge, 0, this.ServerChallenge.Length * 2);
			}
			str += ",signingKey=";
			bool flag2 = this.SigningKey == null;
			if (flag2)
			{
				str += "null";
			}
			else
			{
				str += Hexdump.ToHexString(this.SigningKey, 0, this.SigningKey.Length * 2);
			}
			return str + "]";
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000EF60 File Offset: 0x0000D160
		public virtual bool IsEstablished()
		{
			return this.isEstablished;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0000EF78 File Offset: 0x0000D178
		public virtual byte[] GetServerChallenge()
		{
			return this.ServerChallenge;
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0000EF90 File Offset: 0x0000D190
		public virtual byte[] GetSigningKey()
		{
			return this.SigningKey;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000EFA8 File Offset: 0x0000D1A8
		public virtual string GetNetbiosName()
		{
			return this.NetbiosName;
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000EFC0 File Offset: 0x0000D1C0
		private string GetNtlmsspListItem(byte[] type2Token, int id0)
		{
			int num = 58;
			for (;;)
			{
				int num2 = (int)Encdec.Dec_uint16le(type2Token, num);
				int num3 = (int)Encdec.Dec_uint16le(type2Token, num + 2);
				num += 4;
				bool flag = num2 == 0 || num + num3 > type2Token.Length;
				if (flag)
				{
					break;
				}
				bool flag2 = num2 == id0;
				if (flag2)
				{
					try
					{
						return Runtime.GetStringForBytes(type2Token, num, num3, SmbConstants.UniEncoding);
					}
					catch (UnsupportedEncodingException)
					{
						break;
					}
				}
				num += num3;
			}
			return null;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000F040 File Offset: 0x0000D240
		public virtual byte[] InitSecContext(byte[] token, int offset, int len)
		{
			int state = this.State;
			if (state != 1)
			{
				if (state == 2)
				{
					try
					{
						Type2Message type2Message = new Type2Message(token);
						bool flag = this.Log.Level >= 4;
						if (flag)
						{
							this.Log.WriteLine(type2Message);
							bool flag2 = this.Log.Level >= 6;
							if (flag2)
							{
								Hexdump.ToHexdump(this.Log, token, 0, token.Length);
							}
						}
						this.ServerChallenge = type2Message.GetChallenge();
						this.NtlmsspFlags &= type2Message.GetFlags();
						Type3Message type3Message = new Type3Message(type2Message, this.Auth.GetPassword(), this.Auth.GetDomain(), this.Auth.GetUsername(), this.Workstation, this.NtlmsspFlags);
						token = type3Message.ToByteArray();
						bool flag3 = this.Log.Level >= 4;
						if (flag3)
						{
							this.Log.WriteLine(type3Message);
							bool flag4 = this.Log.Level >= 6;
							if (flag4)
							{
								Hexdump.ToHexdump(this.Log, token, 0, token.Length);
							}
						}
						bool flag5 = (this.NtlmsspFlags & 16) != 0;
						if (flag5)
						{
							this.SigningKey = type3Message.GetMasterKey();
						}
						this.isEstablished = true;
						this.State++;
						return token;
					}
					catch (Exception ex)
					{
						throw new SmbException(ex.Message, ex);
					}
				}
				throw new SmbException("Invalid state");
			}
			Type1Message type1Message = new Type1Message(this.NtlmsspFlags, this.Auth.GetDomain(), this.Workstation);
			token = type1Message.ToByteArray();
			bool flag6 = this.Log.Level >= 4;
			if (flag6)
			{
				this.Log.WriteLine(type1Message);
				bool flag7 = this.Log.Level >= 6;
				if (flag7)
				{
					Hexdump.ToHexdump(this.Log, token, 0, token.Length);
				}
			}
			this.State++;
			return token;
		}

		// Token: 0x040000E6 RID: 230
		internal NtlmPasswordAuthentication Auth;

		// Token: 0x040000E7 RID: 231
		internal int NtlmsspFlags;

		// Token: 0x040000E8 RID: 232
		internal string Workstation;

		// Token: 0x040000E9 RID: 233
		internal bool isEstablished;

		// Token: 0x040000EA RID: 234
		internal byte[] ServerChallenge;

		// Token: 0x040000EB RID: 235
		internal byte[] SigningKey;

		// Token: 0x040000EC RID: 236
		internal string NetbiosName = null;

		// Token: 0x040000ED RID: 237
		internal int State = 1;

		// Token: 0x040000EE RID: 238
		internal LogStream Log;
	}
}
