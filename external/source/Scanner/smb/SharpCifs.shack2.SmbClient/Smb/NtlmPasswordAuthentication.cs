using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x0200007E RID: 126
	public sealed class NtlmPasswordAuthentication : Principal
	{
		// Token: 0x06000387 RID: 903 RVA: 0x0000F274 File Offset: 0x0000D474
		private static void E(byte[] key, byte[] data, byte[] e)
		{
			byte[] array = new byte[7];
			byte[] array2 = new byte[8];
			for (int i = 0; i < key.Length / 7; i++)
			{
				Array.Copy(key, i * 7, array, 0, 7);
				DES des = new DES(array);
				des.Encrypt(data, array2);
				Array.Copy(array2, 0, e, i * 8, 8);
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0000F2D4 File Offset: 0x0000D4D4
		internal static void InitDefaults()
		{
			bool flag = NtlmPasswordAuthentication.DefaultDomain != null;
			if (!flag)
			{
				NtlmPasswordAuthentication.DefaultDomain = Config.GetProperty("jcifs.smb.client.domain", "?");
				NtlmPasswordAuthentication.DefaultUsername = Config.GetProperty("jcifs.smb.client.username", "GUEST");
				NtlmPasswordAuthentication.DefaultPassword = Config.GetProperty("jcifs.smb.client.password", NtlmPasswordAuthentication.Blank);
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000F330 File Offset: 0x0000D530
		public static byte[] GetPreNtlmResponse(string password, byte[] challenge)
		{
			byte[] array = new byte[14];
			byte[] array2 = new byte[21];
			byte[] array3 = new byte[24];
			byte[] bytesForString;
			try
			{
				bytesForString = Runtime.GetBytesForString(password.ToUpper(), SmbConstants.OemEncoding);
			}
			catch (UnsupportedEncodingException ex)
			{
				throw new RuntimeException("Try setting jcifs.encoding=US-ASCII", ex);
			}
			int num = bytesForString.Length;
			bool flag = num > 14;
			if (flag)
			{
				num = 14;
			}
			Array.Copy(bytesForString, 0, array, 0, num);
			NtlmPasswordAuthentication.E(array, NtlmPasswordAuthentication.S8, array2);
			NtlmPasswordAuthentication.E(array2, challenge, array3);
			return array3;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0000F3CC File Offset: 0x0000D5CC
		public static byte[] GetNtlmResponse(string password, byte[] challenge)
		{
			byte[] b = null;
			byte[] array = new byte[21];
			byte[] array2 = new byte[24];
			try
			{
				b = Runtime.GetBytesForString(password, SmbConstants.UniEncoding);
			}
			catch (UnsupportedEncodingException ex)
			{
				bool flag = NtlmPasswordAuthentication._log.Level > 0;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, NtlmPasswordAuthentication._log);
				}
			}
			Md4 md = new Md4();
			md.Update(b);
			try
			{
				md.Digest(array, 0, 16);
			}
			catch (Exception ex2)
			{
				bool flag2 = NtlmPasswordAuthentication._log.Level > 0;
				if (flag2)
				{
					Runtime.PrintStackTrace(ex2, NtlmPasswordAuthentication._log);
				}
			}
			NtlmPasswordAuthentication.E(array, challenge, array2);
			return array2;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000F498 File Offset: 0x0000D698
		public static byte[] GetLMv2Response(string domain, string user, string password, byte[] challenge, byte[] clientChallenge)
		{
			byte[] result;
			try
			{
				byte[] array = new byte[16];
				byte[] array2 = new byte[24];
				Md4 md = new Md4();
				md.Update(Runtime.GetBytesForString(password, SmbConstants.UniEncoding));
				Hmact64 hmact = new Hmact64(md.Digest());
				hmact.Update(Runtime.GetBytesForString(user.ToUpper(), SmbConstants.UniEncoding));
				hmact.Update(Runtime.GetBytesForString(domain.ToUpper(), SmbConstants.UniEncoding));
				hmact = new Hmact64(hmact.Digest());
				hmact.Update(challenge);
				hmact.Update(clientChallenge);
				hmact.Digest(array2, 0, 16);
				Array.Copy(clientChallenge, 0, array2, 16, 8);
				result = array2;
			}
			catch (Exception ex)
			{
				bool flag = NtlmPasswordAuthentication._log.Level > 0;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, NtlmPasswordAuthentication._log);
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000F580 File Offset: 0x0000D780
		public static byte[] GetNtlm2Response(byte[] nTowFv1, byte[] serverChallenge, byte[] clientChallenge)
		{
			byte[] array = new byte[8];
			try
			{
				MessageDigest instance = MessageDigest.GetInstance("MD5");
				instance.Update(serverChallenge);
				instance.Update(clientChallenge, 0, 8);
				Array.Copy(instance.Digest(), 0, array, 0, 8);
			}
			catch (Exception ex)
			{
				bool flag = NtlmPasswordAuthentication._log.Level > 0;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, NtlmPasswordAuthentication._log);
				}
				throw new RuntimeException("MD5", ex);
			}
			byte[] array2 = new byte[21];
			Array.Copy(nTowFv1, 0, array2, 0, 16);
			byte[] array3 = new byte[24];
			NtlmPasswordAuthentication.E(array2, array, array3);
			return array3;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000F634 File Offset: 0x0000D834
		public static byte[] NtowFv1(string password)
		{
			bool flag = password == null;
			if (flag)
			{
				throw new RuntimeException("Password parameter is required");
			}
			byte[] result;
			try
			{
				Md4 md = new Md4();
				md.Update(Runtime.GetBytesForString(password, SmbConstants.UniEncoding));
				result = md.Digest();
			}
			catch (UnsupportedEncodingException ex)
			{
				throw new RuntimeException(ex.Message);
			}
			return result;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0000F698 File Offset: 0x0000D898
		public static byte[] NtowFv2(string domain, string username, string password)
		{
			byte[] result;
			try
			{
				Md4 md = new Md4();
				md.Update(Runtime.GetBytesForString(password, SmbConstants.UniEncoding));
				Hmact64 hmact = new Hmact64(md.Digest());
				hmact.Update(Runtime.GetBytesForString(username.ToUpper(), SmbConstants.UniEncoding));
				hmact.Update(Runtime.GetBytesForString(domain, SmbConstants.UniEncoding));
				result = hmact.Digest();
			}
			catch (UnsupportedEncodingException ex)
			{
				throw new RuntimeException(ex.Message);
			}
			return result;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0000F71C File Offset: 0x0000D91C
		internal static byte[] ComputeResponse(byte[] responseKey, byte[] serverChallenge, byte[] clientData, int offset, int length)
		{
			Hmact64 hmact = new Hmact64(responseKey);
			hmact.Update(serverChallenge);
			hmact.Update(clientData, offset, length);
			byte[] array = hmact.Digest();
			byte[] array2 = new byte[array.Length + clientData.Length];
			Array.Copy(array, 0, array2, 0, array.Length);
			Array.Copy(clientData, 0, array2, array.Length, clientData.Length);
			return array2;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0000F77C File Offset: 0x0000D97C
		public static byte[] GetLMv2Response(byte[] responseKeyLm, byte[] serverChallenge, byte[] clientChallenge)
		{
			return NtlmPasswordAuthentication.ComputeResponse(responseKeyLm, serverChallenge, clientChallenge, 0, clientChallenge.Length);
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000F79C File Offset: 0x0000D99C
		public static byte[] GetNtlMv2Response(byte[] responseKeyNt, byte[] serverChallenge, byte[] clientChallenge, long nanos1601, byte[] targetInfo)
		{
			int num = (targetInfo != null) ? targetInfo.Length : 0;
			byte[] array = new byte[28 + num + 4];
			Encdec.Enc_uint32le(257, array, 0);
			Encdec.Enc_uint32le(0, array, 4);
			Encdec.Enc_uint64le(nanos1601, array, 8);
			Array.Copy(clientChallenge, 0, array, 16, 8);
			Encdec.Enc_uint32le(0, array, 24);
			bool flag = targetInfo != null;
			if (flag)
			{
				Array.Copy(targetInfo, 0, array, 28, num);
			}
			Encdec.Enc_uint32le(0, array, 28 + num);
			return NtlmPasswordAuthentication.ComputeResponse(responseKeyNt, serverChallenge, array, 0, array.Length);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0000F82C File Offset: 0x0000DA2C
		public NtlmPasswordAuthentication(string userInfo)
		{
			this.Domain = (this.Username = (this.Password = null));
			bool flag = userInfo != null;
			if (flag)
			{
				try
				{
					userInfo = NtlmPasswordAuthentication.Unescape(userInfo);
				}
				catch (UnsupportedEncodingException)
				{
				}
				int length = userInfo.Length;
				int i = 0;
				int index = 0;
				while (i < length)
				{
					char c = userInfo[i];
					bool flag2 = c == ';';
					if (flag2)
					{
						this.Domain = Runtime.Substring(userInfo, 0, i);
						index = i + 1;
					}
					else
					{
						bool flag3 = c == ':';
						if (flag3)
						{
							this.Password = Runtime.Substring(userInfo, i + 1);
							break;
						}
					}
					i++;
				}
				this.Username = Runtime.Substring(userInfo, index, i);
			}
			NtlmPasswordAuthentication.InitDefaults();
			bool flag4 = this.Domain == null;
			if (flag4)
			{
				this.Domain = NtlmPasswordAuthentication.DefaultDomain;
			}
			bool flag5 = this.Username == null;
			if (flag5)
			{
				this.Username = NtlmPasswordAuthentication.DefaultUsername;
			}
			bool flag6 = this.Password == null;
			if (flag6)
			{
				this.Password = NtlmPasswordAuthentication.DefaultPassword;
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0000F95C File Offset: 0x0000DB5C
		public NtlmPasswordAuthentication(string domain, string username, string password)
		{
			bool flag = username != null;
			if (flag)
			{
				int num = username.IndexOf('@');
				bool flag2 = num > 0;
				if (flag2)
				{
					domain = Runtime.Substring(username, num + 1);
					username = Runtime.Substring(username, 0, num);
				}
				else
				{
					num = username.IndexOf('\\');
					bool flag3 = num > 0;
					if (flag3)
					{
						domain = Runtime.Substring(username, 0, num);
						username = Runtime.Substring(username, num + 1);
					}
				}
			}
			this.Domain = domain;
			this.Username = username;
			this.Password = password;
			NtlmPasswordAuthentication.InitDefaults();
			bool flag4 = domain == null;
			if (flag4)
			{
				this.Domain = NtlmPasswordAuthentication.DefaultDomain;
			}
			bool flag5 = username == null;
			if (flag5)
			{
				this.Username = NtlmPasswordAuthentication.DefaultUsername;
			}
			bool flag6 = password == null;
			if (flag6)
			{
				this.Password = NtlmPasswordAuthentication.DefaultPassword;
			}
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0000FA30 File Offset: 0x0000DC30
		public NtlmPasswordAuthentication(string domain, string username, byte[] challenge, byte[] ansiHash, byte[] unicodeHash)
		{
			bool flag = domain == null || username == null || ansiHash == null || unicodeHash == null;
			if (flag)
			{
				throw new ArgumentException("External credentials cannot be null");
			}
			this.Domain = domain;
			this.Username = username;
			this.Password = null;
			this.Challenge = challenge;
			this.AnsiHash = ansiHash;
			this.UnicodeHash = unicodeHash;
			this.HashesExternal = true;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0000FA9C File Offset: 0x0000DC9C
		public string GetDomain()
		{
			return this.Domain;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0000FAB4 File Offset: 0x0000DCB4
		public string GetUsername()
		{
			return this.Username;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0000FACC File Offset: 0x0000DCCC
		public string GetPassword()
		{
			return this.Password;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0000FAE4 File Offset: 0x0000DCE4
		public new string GetName()
		{
			return (this.Domain.Length > 0 && !this.Domain.Equals("?")) ? (this.Domain + "\\" + this.Username) : this.Username;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0000FB3C File Offset: 0x0000DD3C
		public byte[] GetAnsiHash(byte[] challenge)
		{
			bool hashesExternal = this.HashesExternal;
			byte[] result;
			if (hashesExternal)
			{
				result = this.AnsiHash;
			}
			else
			{
				switch (NtlmPasswordAuthentication.LmCompatibility)
				{
				case 0:
				case 1:
					result = NtlmPasswordAuthentication.GetPreNtlmResponse(this.Password, challenge);
					break;
				case 2:
					result = NtlmPasswordAuthentication.GetNtlmResponse(this.Password, challenge);
					break;
				case 3:
				case 4:
				case 5:
				{
					bool flag = this.ClientChallenge == null;
					if (flag)
					{
						this.ClientChallenge = new byte[8];
						NtlmPasswordAuthentication.Random.NextBytes(this.ClientChallenge);
					}
					result = NtlmPasswordAuthentication.GetLMv2Response(this.Domain, this.Username, this.Password, challenge, this.ClientChallenge);
					break;
				}
				default:
					result = NtlmPasswordAuthentication.GetPreNtlmResponse(this.Password, challenge);
					break;
				}
			}
			return result;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0000FC08 File Offset: 0x0000DE08
		public byte[] GetUnicodeHash(byte[] challenge)
		{
			bool hashesExternal = this.HashesExternal;
			byte[] result;
			if (hashesExternal)
			{
				result = this.UnicodeHash;
			}
			else
			{
				switch (NtlmPasswordAuthentication.LmCompatibility)
				{
				case 0:
				case 1:
				case 2:
					result = NtlmPasswordAuthentication.GetNtlmResponse(this.Password, challenge);
					break;
				case 3:
				case 4:
				case 5:
					result = new byte[0];
					break;
				default:
					result = NtlmPasswordAuthentication.GetNtlmResponse(this.Password, challenge);
					break;
				}
			}
			return result;
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public byte[] GetSigningKey(byte[] challenge)
		{
			byte[] result;
			switch (NtlmPasswordAuthentication.LmCompatibility)
			{
			case 0:
			case 1:
			case 2:
			{
				byte[] array = new byte[40];
				this.GetUserSessionKey(challenge, array, 0);
				Array.Copy(this.GetUnicodeHash(challenge), 0, array, 16, 24);
				result = array;
				break;
			}
			case 3:
			case 4:
			case 5:
				throw new SmbException("NTLMv2 requires extended security (jcifs.smb.client.useExtendedSecurity must be true if jcifs.smb.lmCompatibility >= 3)");
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0000FCEC File Offset: 0x0000DEEC
		public byte[] GetUserSessionKey(byte[] challenge)
		{
			bool hashesExternal = this.HashesExternal;
			byte[] result;
			if (hashesExternal)
			{
				result = null;
			}
			else
			{
				byte[] array = new byte[16];
				try
				{
					this.GetUserSessionKey(challenge, array, 0);
				}
				catch (Exception ex)
				{
					bool flag = NtlmPasswordAuthentication._log.Level > 0;
					if (flag)
					{
						Runtime.PrintStackTrace(ex, NtlmPasswordAuthentication._log);
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x0000FD5C File Offset: 0x0000DF5C
		internal void GetUserSessionKey(byte[] challenge, byte[] dest, int offset)
		{
			bool hashesExternal = this.HashesExternal;
			if (!hashesExternal)
			{
				try
				{
					Md4 md = new Md4();
					md.Update(Runtime.GetBytesForString(this.Password, SmbConstants.UniEncoding));
					switch (NtlmPasswordAuthentication.LmCompatibility)
					{
					case 0:
					case 1:
					case 2:
						md.Update(md.Digest());
						md.Digest(dest, offset, 16);
						break;
					case 3:
					case 4:
					case 5:
					{
						bool flag = this.ClientChallenge == null;
						if (flag)
						{
							this.ClientChallenge = new byte[8];
							NtlmPasswordAuthentication.Random.NextBytes(this.ClientChallenge);
						}
						Hmact64 hmact = new Hmact64(md.Digest());
						hmact.Update(Runtime.GetBytesForString(this.Username.ToUpper(), SmbConstants.UniEncoding));
						hmact.Update(Runtime.GetBytesForString(this.Domain.ToUpper(), SmbConstants.UniEncoding));
						byte[] key = hmact.Digest();
						hmact = new Hmact64(key);
						hmact.Update(challenge);
						hmact.Update(this.ClientChallenge);
						Hmact64 hmact2 = new Hmact64(key);
						hmact2.Update(hmact.Digest());
						hmact2.Digest(dest, offset, 16);
						break;
					}
					default:
						md.Update(md.Digest());
						md.Digest(dest, offset, 16);
						break;
					}
				}
				catch (Exception rootCause)
				{
					throw new SmbException(string.Empty, rootCause);
				}
			}
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
		public override bool Equals(object obj)
		{
			bool flag = obj is NtlmPasswordAuthentication;
			if (flag)
			{
				NtlmPasswordAuthentication ntlmPasswordAuthentication = (NtlmPasswordAuthentication)obj;
				bool flag2 = ntlmPasswordAuthentication.Domain.ToUpper().Equals(this.Domain.ToUpper()) && ntlmPasswordAuthentication.Username.ToUpper().Equals(this.Username.ToUpper());
				if (flag2)
				{
					bool flag3 = this.HashesExternal && ntlmPasswordAuthentication.HashesExternal;
					if (flag3)
					{
						return Arrays.Equals<byte>(this.AnsiHash, ntlmPasswordAuthentication.AnsiHash) && Arrays.Equals<byte>(this.UnicodeHash, ntlmPasswordAuthentication.UnicodeHash);
					}
					bool flag4 = !this.HashesExternal && this.Password.Equals(ntlmPasswordAuthentication.Password);
					if (flag4)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000FFC0 File Offset: 0x0000E1C0
		public override int GetHashCode()
		{
			return this.GetName().ToUpper().GetHashCode();
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
		public override string ToString()
		{
			return this.GetName();
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000FFFC File Offset: 0x0000E1FC
		internal static string Unescape(string str)
		{
			byte[] array = new byte[1];
			bool flag = str == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int length = str.Length;
				char[] array2 = new char[length];
				int num = 0;
				int i;
				int length2 = i = 0;
				while (i < length)
				{
					int num2 = num;
					if (num2 != 0)
					{
						if (num2 == 1)
						{
							array[0] = (byte)(Convert.ToInt32(Runtime.Substring(str, i, i + 2), 16) & 255);
							array2[length2++] = Runtime.GetStringForBytes(array, 0, 1, "ASCII")[0];
							i++;
							num = 0;
						}
					}
					else
					{
						char c = str[i];
						bool flag2 = c == '%';
						if (flag2)
						{
							num = 1;
						}
						else
						{
							array2[length2++] = c;
						}
					}
					i++;
				}
				result = new string(array2, 0, length2);
			}
			return result;
		}

		// Token: 0x040000EF RID: 239
		private static readonly int LmCompatibility = Config.GetInt("jcifs.smb.lmCompatibility", 3);

		// Token: 0x040000F0 RID: 240
		private static readonly Random Random = new Random();

		// Token: 0x040000F1 RID: 241
		private static LogStream _log = LogStream.GetInstance();

		// Token: 0x040000F2 RID: 242
		private static readonly byte[] S8 = new byte[]
		{
			75,
			71,
			83,
			33,
			64,
			35,
			36,
			37
		};

		// Token: 0x040000F3 RID: 243
		internal static string DefaultDomain;

		// Token: 0x040000F4 RID: 244
		internal static string DefaultUsername;

		// Token: 0x040000F5 RID: 245
		internal static string DefaultPassword;

		// Token: 0x040000F6 RID: 246
		internal static readonly string Blank = string.Empty;

		// Token: 0x040000F7 RID: 247
		public static readonly NtlmPasswordAuthentication Anonymous = new NtlmPasswordAuthentication(string.Empty, string.Empty, string.Empty);

		// Token: 0x040000F8 RID: 248
		internal static readonly NtlmPasswordAuthentication Null = new NtlmPasswordAuthentication(string.Empty, string.Empty, string.Empty);

		// Token: 0x040000F9 RID: 249
		internal static readonly NtlmPasswordAuthentication Guest = new NtlmPasswordAuthentication("?", "GUEST", string.Empty);

		// Token: 0x040000FA RID: 250
		internal static readonly NtlmPasswordAuthentication Default = new NtlmPasswordAuthentication(null);

		// Token: 0x040000FB RID: 251
		internal string Domain;

		// Token: 0x040000FC RID: 252
		internal string Username;

		// Token: 0x040000FD RID: 253
		internal string Password;

		// Token: 0x040000FE RID: 254
		internal byte[] AnsiHash;

		// Token: 0x040000FF RID: 255
		internal byte[] UnicodeHash;

		// Token: 0x04000100 RID: 256
		internal bool HashesExternal;

		// Token: 0x04000101 RID: 257
		internal byte[] ClientChallenge;

		// Token: 0x04000102 RID: 258
		internal byte[] Challenge;
	}
}
