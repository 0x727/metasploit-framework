using System;
using System.IO;
using SharpCifs.Netbios;
using SharpCifs.Smb;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Ntlmssp
{
	// Token: 0x020000CF RID: 207
	public class Type3Message : NtlmMessage
	{
		// Token: 0x060006B6 RID: 1718 RVA: 0x00023C6C File Offset: 0x00021E6C
		static Type3Message()
		{
			string defaultWorkstation = null;
			try
			{
				defaultWorkstation = NbtAddress.GetLocalHost().GetHostName();
			}
			catch (UnknownHostException)
			{
			}
			Type3Message.DefaultWorkstation = defaultWorkstation;
			Type3Message.LmCompatibility = Config.GetInt("jcifs.smb.lmCompatibility", 3);
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00023D04 File Offset: 0x00021F04
		public Type3Message()
		{
			this.SetFlags(Type3Message.GetDefaultFlags());
			this.SetDomain(Type3Message.GetDefaultDomain());
			this.SetUser(Type3Message.GetDefaultUser());
			this.SetWorkstation(Type3Message.GetDefaultWorkstation());
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00023D40 File Offset: 0x00021F40
		public Type3Message(Type2Message type2)
		{
			this.SetFlags(Type3Message.GetDefaultFlags(type2));
			this.SetWorkstation(Type3Message.GetDefaultWorkstation());
			string defaultDomain = Type3Message.GetDefaultDomain();
			this.SetDomain(defaultDomain);
			string defaultUser = Type3Message.GetDefaultUser();
			this.SetUser(defaultUser);
			string defaultPassword = Type3Message.GetDefaultPassword();
			switch (Type3Message.LmCompatibility)
			{
			case 0:
			case 1:
				this.SetLmResponse(Type3Message.GetLMResponse(type2, defaultPassword));
				this.SetNtResponse(Type3Message.GetNTResponse(type2, defaultPassword));
				break;
			case 2:
			{
				byte[] ntresponse = Type3Message.GetNTResponse(type2, defaultPassword);
				this.SetLmResponse(ntresponse);
				this.SetNtResponse(ntresponse);
				break;
			}
			case 3:
			case 4:
			case 5:
			{
				byte[] clientChallenge = new byte[8];
				this.SetLmResponse(Type3Message.GetLMv2Response(type2, defaultDomain, defaultUser, defaultPassword, clientChallenge));
				break;
			}
			default:
				this.SetLmResponse(Type3Message.GetLMResponse(type2, defaultPassword));
				this.SetNtResponse(Type3Message.GetNTResponse(type2, defaultPassword));
				break;
			}
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00023E30 File Offset: 0x00022030
		public Type3Message(Type2Message type2, string password, string domain, string user, string workstation, int flags)
		{
			this.SetFlags(flags | Type3Message.GetDefaultFlags(type2));
			bool flag = workstation == null;
			if (flag)
			{
				workstation = Type3Message.GetDefaultWorkstation();
			}
			this.SetWorkstation(workstation);
			this.SetDomain(domain);
			this.SetUser(user);
			switch (Type3Message.LmCompatibility)
			{
			case 0:
			case 1:
			{
				bool flag2 = (this.GetFlags() & 524288) == 0;
				if (flag2)
				{
					this.SetLmResponse(Type3Message.GetLMResponse(type2, password));
					this.SetNtResponse(Type3Message.GetNTResponse(type2, password));
				}
				else
				{
					byte[] array = new byte[24];
					Arrays.Fill<byte>(array, 8, 24, 0);
					byte[] array2 = NtlmPasswordAuthentication.NtowFv1(password);
					byte[] ntlm2Response = NtlmPasswordAuthentication.GetNtlm2Response(array2, type2.GetChallenge(), array);
					this.SetLmResponse(array);
					this.SetNtResponse(ntlm2Response);
					bool flag3 = (this.GetFlags() & 16) == 16;
					if (flag3)
					{
						byte[] array3 = new byte[16];
						Array.Copy(type2.GetChallenge(), 0, array3, 0, 8);
						Array.Copy(array, 0, array3, 8, 8);
						Md4 md = new Md4();
						md.Update(array2);
						byte[] key = md.Digest();
						Hmact64 hmact = new Hmact64(key);
						hmact.Update(array3);
						byte[] array4 = hmact.Digest();
						bool flag4 = (this.GetFlags() & 1073741824) != 0;
						if (flag4)
						{
							this._masterKey = new byte[16];
							byte[] array5 = new byte[16];
							Rc4 rc = new Rc4(array4);
							rc.Update(this._masterKey, 0, 16, array5, 0);
							this.SetSessionKey(array5);
						}
						else
						{
							this._masterKey = array4;
							this.SetSessionKey(this._masterKey);
						}
					}
				}
				break;
			}
			case 2:
			{
				byte[] ntresponse = Type3Message.GetNTResponse(type2, password);
				this.SetLmResponse(ntresponse);
				this.SetNtResponse(ntresponse);
				break;
			}
			case 3:
			case 4:
			case 5:
			{
				byte[] array6 = NtlmPasswordAuthentication.NtowFv2(domain, user, password);
				byte[] clientChallenge = new byte[8];
				this.SetLmResponse(Type3Message.GetLMv2Response(type2, domain, user, password, clientChallenge));
				byte[] clientChallenge2 = new byte[8];
				this.SetNtResponse(Type3Message.GetNtlMv2Response(type2, array6, clientChallenge2));
				bool flag5 = (this.GetFlags() & 16) == 16;
				if (flag5)
				{
					Hmact64 hmact2 = new Hmact64(array6);
					hmact2.Update(this._ntResponse, 0, 16);
					byte[] array7 = hmact2.Digest();
					bool flag6 = (this.GetFlags() & 1073741824) != 0;
					if (flag6)
					{
						this._masterKey = new byte[16];
						byte[] array8 = new byte[16];
						Rc4 rc2 = new Rc4(array7);
						rc2.Update(this._masterKey, 0, 16, array8, 0);
						this.SetSessionKey(array8);
					}
					else
					{
						this._masterKey = array7;
						this.SetSessionKey(this._masterKey);
					}
				}
				break;
			}
			default:
				this.SetLmResponse(Type3Message.GetLMResponse(type2, password));
				this.SetNtResponse(Type3Message.GetNTResponse(type2, password));
				break;
			}
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0002412D File Offset: 0x0002232D
		public Type3Message(int flags, byte[] lmResponse, byte[] ntResponse, string domain, string user, string workstation)
		{
			this.SetFlags(flags);
			this.SetLmResponse(lmResponse);
			this.SetNtResponse(ntResponse);
			this.SetDomain(domain);
			this.SetUser(user);
			this.SetWorkstation(workstation);
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0002416A File Offset: 0x0002236A
		public Type3Message(byte[] material)
		{
			this.Parse(material);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x0002417C File Offset: 0x0002237C
		public virtual byte[] GetLMResponse()
		{
			return this._lmResponse;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00024194 File Offset: 0x00022394
		public virtual void SetLmResponse(byte[] lmResponse)
		{
			this._lmResponse = lmResponse;
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x000241A0 File Offset: 0x000223A0
		public virtual byte[] GetNTResponse()
		{
			return this._ntResponse;
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x000241B8 File Offset: 0x000223B8
		public virtual void SetNtResponse(byte[] ntResponse)
		{
			this._ntResponse = ntResponse;
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x000241C4 File Offset: 0x000223C4
		public virtual string GetDomain()
		{
			return this._domain;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x000241DC File Offset: 0x000223DC
		public virtual void SetDomain(string domain)
		{
			this._domain = domain;
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x000241E8 File Offset: 0x000223E8
		public virtual string GetUser()
		{
			return this._user;
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x00024200 File Offset: 0x00022400
		public virtual void SetUser(string user)
		{
			this._user = user;
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0002420C File Offset: 0x0002240C
		public virtual string GetWorkstation()
		{
			return this._workstation;
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00024224 File Offset: 0x00022424
		public virtual void SetWorkstation(string workstation)
		{
			this._workstation = workstation;
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x00024230 File Offset: 0x00022430
		public virtual byte[] GetMasterKey()
		{
			return this._masterKey;
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x00024248 File Offset: 0x00022448
		public virtual byte[] GetSessionKey()
		{
			return this._sessionKey;
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00024260 File Offset: 0x00022460
		public virtual void SetSessionKey(byte[] sessionKey)
		{
			this._sessionKey = sessionKey;
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0002426C File Offset: 0x0002246C
		public override byte[] ToByteArray()
		{
			byte[] result;
			try
			{
				int flags = this.GetFlags();
				bool flag = (flags & 1) != 0;
				string encoding = flag ? null : NtlmMessage.GetOemEncoding();
				string domain = this.GetDomain();
				byte[] array = null;
				bool flag2 = !string.IsNullOrEmpty(domain);
				if (flag2)
				{
					array = (flag ? Runtime.GetBytesForString(domain, NtlmMessage.UniEncoding) : Runtime.GetBytesForString(domain, encoding));
				}
				int num = (array != null) ? array.Length : 0;
				string user = this.GetUser();
				byte[] array2 = null;
				bool flag3 = !string.IsNullOrEmpty(user);
				if (flag3)
				{
					array2 = (flag ? Runtime.GetBytesForString(user, NtlmMessage.UniEncoding) : Runtime.GetBytesForString(user.ToUpper(), encoding));
				}
				int num2 = (array2 != null) ? array2.Length : 0;
				string workstation = this.GetWorkstation();
				byte[] array3 = null;
				bool flag4 = !string.IsNullOrEmpty(workstation);
				if (flag4)
				{
					array3 = (flag ? Runtime.GetBytesForString(workstation, NtlmMessage.UniEncoding) : Runtime.GetBytesForString(workstation.ToUpper(), encoding));
				}
				int num3 = (array3 != null) ? array3.Length : 0;
				byte[] lmresponse = this.GetLMResponse();
				int num4 = (lmresponse != null) ? lmresponse.Length : 0;
				byte[] ntresponse = this.GetNTResponse();
				int num5 = (ntresponse != null) ? ntresponse.Length : 0;
				byte[] sessionKey = this.GetSessionKey();
				int num6 = (sessionKey != null) ? sessionKey.Length : 0;
				byte[] array4 = new byte[64 + num + num2 + num3 + num4 + num5 + num6];
				Array.Copy(NtlmMessage.NtlmsspSignature, 0, array4, 0, 8);
				NtlmMessage.WriteULong(array4, 8, 3);
				int num7 = 64;
				NtlmMessage.WriteSecurityBuffer(array4, 12, num7, lmresponse);
				num7 += num4;
				NtlmMessage.WriteSecurityBuffer(array4, 20, num7, ntresponse);
				num7 += num5;
				NtlmMessage.WriteSecurityBuffer(array4, 28, num7, array);
				num7 += num;
				NtlmMessage.WriteSecurityBuffer(array4, 36, num7, array2);
				num7 += num2;
				NtlmMessage.WriteSecurityBuffer(array4, 44, num7, array3);
				num7 += num3;
				NtlmMessage.WriteSecurityBuffer(array4, 52, num7, sessionKey);
				NtlmMessage.WriteULong(array4, 60, flags);
				result = array4;
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
			return result;
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x00024498 File Offset: 0x00022698
		public override string ToString()
		{
			string user = this.GetUser();
			string domain = this.GetDomain();
			string workstation = this.GetWorkstation();
			byte[] lmresponse = this.GetLMResponse();
			byte[] ntresponse = this.GetNTResponse();
			byte[] sessionKey = this.GetSessionKey();
			return string.Concat(new string[]
			{
				"Type3Message[domain=",
				domain,
				",user=",
				user,
				",workstation=",
				workstation,
				",lmResponse=",
				(lmresponse == null) ? "null" : ("<" + lmresponse.Length + " bytes>"),
				",ntResponse=",
				(ntresponse == null) ? "null" : ("<" + ntresponse.Length + " bytes>"),
				",sessionKey=",
				(sessionKey == null) ? "null" : ("<" + sessionKey.Length + " bytes>"),
				",flags=0x",
				Hexdump.ToHexString(this.GetFlags(), 8),
				"]"
			});
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x000245B8 File Offset: 0x000227B8
		public static int GetDefaultFlags()
		{
			return Type3Message.DefaultFlags;
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x000245D0 File Offset: 0x000227D0
		public static int GetDefaultFlags(Type2Message type2)
		{
			bool flag = type2 == null;
			int result;
			if (flag)
			{
				result = Type3Message.DefaultFlags;
			}
			else
			{
				int num = 512;
				num |= (((type2.GetFlags() & 1) != 0) ? 1 : 2);
				result = num;
			}
			return result;
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0002460C File Offset: 0x0002280C
		public static byte[] GetLMResponse(Type2Message type2, string password)
		{
			bool flag = type2 == null || password == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = NtlmPasswordAuthentication.GetPreNtlmResponse(password, type2.GetChallenge());
			}
			return result;
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x00024640 File Offset: 0x00022840
		public static byte[] GetLMv2Response(Type2Message type2, string domain, string user, string password, byte[] clientChallenge)
		{
			bool flag = type2 == null || domain == null || user == null || password == null || clientChallenge == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = NtlmPasswordAuthentication.GetLMv2Response(domain, user, password, type2.GetChallenge(), clientChallenge);
			}
			return result;
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x00024680 File Offset: 0x00022880
		public static byte[] GetNtlMv2Response(Type2Message type2, byte[] responseKeyNt, byte[] clientChallenge)
		{
			bool flag = type2 == null || responseKeyNt == null || clientChallenge == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				long nanos = (Runtime.CurrentTimeMillis() + 11644473600000L) * 10000L;
				result = NtlmPasswordAuthentication.GetNtlMv2Response(responseKeyNt, type2.GetChallenge(), clientChallenge, nanos, type2.GetTargetInformation());
			}
			return result;
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x000246D4 File Offset: 0x000228D4
		public static byte[] GetNTResponse(Type2Message type2, string password)
		{
			bool flag = type2 == null || password == null;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = NtlmPasswordAuthentication.GetNtlmResponse(password, type2.GetChallenge());
			}
			return result;
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00024708 File Offset: 0x00022908
		public static string GetDefaultDomain()
		{
			return Type3Message.DefaultDomain;
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00024720 File Offset: 0x00022920
		public static string GetDefaultUser()
		{
			return Type3Message.DefaultUser;
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00024738 File Offset: 0x00022938
		public static string GetDefaultPassword()
		{
			return Type3Message.DefaultPassword;
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00024750 File Offset: 0x00022950
		public static string GetDefaultWorkstation()
		{
			return Type3Message.DefaultWorkstation;
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00024768 File Offset: 0x00022968
		private void Parse(byte[] material)
		{
			for (int i = 0; i < 8; i++)
			{
				bool flag = material[i] != NtlmMessage.NtlmsspSignature[i];
				if (flag)
				{
					throw new IOException("Not an NTLMSSP message.");
				}
			}
			bool flag2 = NtlmMessage.ReadULong(material, 8) != 3;
			if (flag2)
			{
				throw new IOException("Not a Type 3 message.");
			}
			byte[] lmResponse = NtlmMessage.ReadSecurityBuffer(material, 12);
			int num = NtlmMessage.ReadULong(material, 16);
			byte[] ntResponse = NtlmMessage.ReadSecurityBuffer(material, 20);
			int num2 = NtlmMessage.ReadULong(material, 24);
			byte[] chars = NtlmMessage.ReadSecurityBuffer(material, 28);
			int num3 = NtlmMessage.ReadULong(material, 32);
			byte[] chars2 = NtlmMessage.ReadSecurityBuffer(material, 36);
			int num4 = NtlmMessage.ReadULong(material, 40);
			byte[] chars3 = NtlmMessage.ReadSecurityBuffer(material, 44);
			int num5 = NtlmMessage.ReadULong(material, 48);
			byte[] sessionKey = null;
			bool flag3 = num == 52 || num2 == 52 || num3 == 52 || num4 == 52 || num5 == 52;
			int num6;
			string encoding;
			if (flag3)
			{
				num6 = 514;
				encoding = NtlmMessage.GetOemEncoding();
			}
			else
			{
				sessionKey = NtlmMessage.ReadSecurityBuffer(material, 52);
				num6 = NtlmMessage.ReadULong(material, 60);
				encoding = (((num6 & 1) != 0) ? NtlmMessage.UniEncoding : NtlmMessage.GetOemEncoding());
			}
			this.SetSessionKey(sessionKey);
			this.SetFlags(num6);
			this.SetLmResponse(lmResponse);
			this.SetNtResponse(ntResponse);
			this.SetDomain(Runtime.GetStringForBytes(chars, encoding));
			this.SetUser(Runtime.GetStringForBytes(chars2, encoding));
			this.SetWorkstation(Runtime.GetStringForBytes(chars3, encoding));
		}

		// Token: 0x0400040F RID: 1039
		internal const long MillisecondsBetween1970And1601 = 11644473600000L;

		// Token: 0x04000410 RID: 1040
		private static readonly int DefaultFlags = 512 | (Config.GetBoolean("jcifs.smb.client.useUnicode", true) ? 1 : 2);

		// Token: 0x04000411 RID: 1041
		private static readonly string DefaultDomain = Config.GetProperty("jcifs.smb.client.domain", null);

		// Token: 0x04000412 RID: 1042
		private static readonly string DefaultUser = Config.GetProperty("jcifs.smb.client.username", null);

		// Token: 0x04000413 RID: 1043
		private static readonly string DefaultPassword = Config.GetProperty("jcifs.smb.client.password", null);

		// Token: 0x04000414 RID: 1044
		private static readonly string DefaultWorkstation;

		// Token: 0x04000415 RID: 1045
		private static readonly int LmCompatibility;

		// Token: 0x04000416 RID: 1046
		private byte[] _lmResponse;

		// Token: 0x04000417 RID: 1047
		private byte[] _ntResponse;

		// Token: 0x04000418 RID: 1048
		private string _domain;

		// Token: 0x04000419 RID: 1049
		private string _user;

		// Token: 0x0400041A RID: 1050
		private string _workstation;

		// Token: 0x0400041B RID: 1051
		private byte[] _masterKey;

		// Token: 0x0400041C RID: 1052
		private byte[] _sessionKey;
	}
}
