using System;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Smb
{
	// Token: 0x02000086 RID: 134
	public class SigningDigest
	{
		// Token: 0x060003F4 RID: 1012 RVA: 0x000129C8 File Offset: 0x00010BC8
		public SigningDigest(byte[] macSigningKey, bool bypass)
		{
			try
			{
				this._digest = MessageDigest.GetInstance("MD5");
			}
			catch (NoSuchAlgorithmException ex)
			{
				bool flag = SigningDigest.Log.Level > 0;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, SigningDigest.Log);
				}
				throw new SmbException("MD5", ex);
			}
			this._macSigningKey = macSigningKey;
			this._bypass = bypass;
			this._updates = 0;
			this._signSequence = 0;
			bool flag2 = SigningDigest.Log.Level >= 5;
			if (flag2)
			{
				SigningDigest.Log.WriteLine("macSigningKey:");
				Hexdump.ToHexdump(SigningDigest.Log, macSigningKey, 0, macSigningKey.Length);
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00012A80 File Offset: 0x00010C80
		public SigningDigest(SmbTransport transport, NtlmPasswordAuthentication auth)
		{
			try
			{
				this._digest = MessageDigest.GetInstance("MD5");
			}
			catch (NoSuchAlgorithmException ex)
			{
				bool flag = SigningDigest.Log.Level > 0;
				if (flag)
				{
					Runtime.PrintStackTrace(ex, SigningDigest.Log);
				}
				throw new SmbException("MD5", ex);
			}
			try
			{
				switch (SmbConstants.LmCompatibility)
				{
				case 0:
				case 1:
				case 2:
					this._macSigningKey = new byte[40];
					auth.GetUserSessionKey(transport.Server.EncryptionKey, this._macSigningKey, 0);
					Array.Copy(auth.GetUnicodeHash(transport.Server.EncryptionKey), 0, this._macSigningKey, 16, 24);
					break;
				case 3:
				case 4:
				case 5:
					this._macSigningKey = new byte[16];
					auth.GetUserSessionKey(transport.Server.EncryptionKey, this._macSigningKey, 0);
					break;
				default:
					this._macSigningKey = new byte[40];
					auth.GetUserSessionKey(transport.Server.EncryptionKey, this._macSigningKey, 0);
					Array.Copy(auth.GetUnicodeHash(transport.Server.EncryptionKey), 0, this._macSigningKey, 16, 24);
					break;
				}
			}
			catch (Exception rootCause)
			{
				throw new SmbException(string.Empty, rootCause);
			}
			bool flag2 = SigningDigest.Log.Level >= 5;
			if (flag2)
			{
				SigningDigest.Log.WriteLine("LM_COMPATIBILITY=" + SmbConstants.LmCompatibility);
				Hexdump.ToHexdump(SigningDigest.Log, this._macSigningKey, 0, this._macSigningKey.Length);
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00012C3C File Offset: 0x00010E3C
		public virtual void Update(byte[] input, int offset, int len)
		{
			bool flag = SigningDigest.Log.Level >= 5;
			if (flag)
			{
				SigningDigest.Log.WriteLine(string.Concat(new object[]
				{
					"update: ",
					this._updates,
					" ",
					offset,
					":",
					len
				}));
				Hexdump.ToHexdump(SigningDigest.Log, input, offset, Math.Min(len, 256));
				SigningDigest.Log.Flush();
			}
			bool flag2 = len == 0;
			if (!flag2)
			{
				this._digest.Update(input, offset, len);
				this._updates++;
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00012CFC File Offset: 0x00010EFC
		public virtual byte[] Digest()
		{
			byte[] array = this._digest.Digest();
			bool flag = SigningDigest.Log.Level >= 5;
			if (flag)
			{
				SigningDigest.Log.WriteLine("digest: ");
				Hexdump.ToHexdump(SigningDigest.Log, array, 0, array.Length);
				SigningDigest.Log.Flush();
			}
			this._updates = 0;
			return array;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00012D64 File Offset: 0x00010F64
		internal virtual void Sign(byte[] data, int offset, int length, ServerMessageBlock request, ServerMessageBlock response)
		{
			request.SignSeq = this._signSequence;
			bool flag = response != null;
			if (flag)
			{
				response.SignSeq = this._signSequence + 1;
				response.VerifyFailed = false;
			}
			try
			{
				this.Update(this._macSigningKey, 0, this._macSigningKey.Length);
				int num = offset + SmbConstants.SignatureOffset;
				for (int i = 0; i < 8; i++)
				{
					data[num + i] = 0;
				}
				ServerMessageBlock.WriteInt4((long)this._signSequence, data, num);
				this.Update(data, offset, length);
				Array.Copy(this.Digest(), 0, data, num, 8);
				bool bypass = this._bypass;
				if (bypass)
				{
					this._bypass = false;
					Array.Copy(Runtime.GetBytesForString("BSRSPYL "), 0, data, num, 8);
				}
			}
			catch (Exception ex)
			{
				bool flag2 = SigningDigest.Log.Level > 0;
				if (flag2)
				{
					Runtime.PrintStackTrace(ex, SigningDigest.Log);
				}
			}
			finally
			{
				this._signSequence += 2;
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00012E88 File Offset: 0x00011088
		internal virtual bool Verify(byte[] data, int offset, ServerMessageBlock response)
		{
			this.Update(this._macSigningKey, 0, this._macSigningKey.Length);
			this.Update(data, offset, SmbConstants.SignatureOffset);
			int num = offset + SmbConstants.SignatureOffset;
			byte[] array = new byte[8];
			ServerMessageBlock.WriteInt4((long)response.SignSeq, array, 0);
			this.Update(array, 0, array.Length);
			num += 8;
			bool flag = response.Command == 46;
			if (flag)
			{
				SmbComReadAndXResponse smbComReadAndXResponse = (SmbComReadAndXResponse)response;
				int num2 = response.Length - smbComReadAndXResponse.DataLength;
				this.Update(data, num, num2 - SmbConstants.SignatureOffset - 8);
				this.Update(smbComReadAndXResponse.B, smbComReadAndXResponse.Off, smbComReadAndXResponse.DataLength);
			}
			else
			{
				this.Update(data, num, response.Length - SmbConstants.SignatureOffset - 8);
			}
			byte[] array2 = this.Digest();
			for (int i = 0; i < 8; i++)
			{
				bool flag2 = array2[i] != data[offset + SmbConstants.SignatureOffset + i];
				if (flag2)
				{
					bool flag3 = SigningDigest.Log.Level >= 2;
					if (flag3)
					{
						SigningDigest.Log.WriteLine("signature verification failure");
						Hexdump.ToHexdump(SigningDigest.Log, array2, 0, 8);
						Hexdump.ToHexdump(SigningDigest.Log, data, offset + SmbConstants.SignatureOffset, 8);
					}
					return response.VerifyFailed = true;
				}
			}
			return response.VerifyFailed = false;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00013008 File Offset: 0x00011208
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"LM_COMPATIBILITY=",
				SmbConstants.LmCompatibility,
				" MacSigningKey=",
				Hexdump.ToHexString(this._macSigningKey, 0, this._macSigningKey.Length)
			});
		}

		// Token: 0x04000190 RID: 400
		internal static LogStream Log = LogStream.GetInstance();

		// Token: 0x04000191 RID: 401
		private MessageDigest _digest;

		// Token: 0x04000192 RID: 402
		private byte[] _macSigningKey;

		// Token: 0x04000193 RID: 403
		private bool _bypass;

		// Token: 0x04000194 RID: 404
		private int _updates;

		// Token: 0x04000195 RID: 405
		private int _signSequence;
	}
}
