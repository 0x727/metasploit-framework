using System;
using System.IO;
using SharpCifs.Netbios;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Ntlmssp
{
	// Token: 0x020000CD RID: 205
	public class Type1Message : NtlmMessage
	{
		// Token: 0x06000693 RID: 1683 RVA: 0x000230D4 File Offset: 0x000212D4
		static Type1Message()
		{
			string defaultWorkstation = null;
			try
			{
				defaultWorkstation = NbtAddress.GetLocalHost().GetHostName();
			}
			catch (UnknownHostException)
			{
			}
			Type1Message.DefaultWorkstation = defaultWorkstation;
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0002313C File Offset: 0x0002133C
		public Type1Message() : this(Type1Message.GetDefaultFlags(), Type1Message.GetDefaultDomain(), Type1Message.GetDefaultWorkstation())
		{
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00023158 File Offset: 0x00021358
		public Type1Message(int flags, string suppliedDomain, string suppliedWorkstation)
		{
			this.SetFlags(Type1Message.GetDefaultFlags() | flags);
			this.SetSuppliedDomain(suppliedDomain);
			bool flag = suppliedWorkstation == null;
			if (flag)
			{
				suppliedWorkstation = Type1Message.GetDefaultWorkstation();
			}
			this.SetSuppliedWorkstation(suppliedWorkstation);
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0002319C File Offset: 0x0002139C
		public Type1Message(byte[] material)
		{
			this.Parse(material);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x000231B0 File Offset: 0x000213B0
		public virtual string GetSuppliedDomain()
		{
			return this._suppliedDomain;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x000231C8 File Offset: 0x000213C8
		public virtual void SetSuppliedDomain(string suppliedDomain)
		{
			this._suppliedDomain = suppliedDomain;
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x000231D4 File Offset: 0x000213D4
		public virtual string GetSuppliedWorkstation()
		{
			return this._suppliedWorkstation;
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x000231EC File Offset: 0x000213EC
		public virtual void SetSuppliedWorkstation(string suppliedWorkstation)
		{
			this._suppliedWorkstation = suppliedWorkstation;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x000231F8 File Offset: 0x000213F8
		public override byte[] ToByteArray()
		{
			byte[] result;
			try
			{
				string suppliedDomain = this.GetSuppliedDomain();
				string suppliedWorkstation = this.GetSuppliedWorkstation();
				int num = this.GetFlags();
				bool flag = false;
				byte[] array = new byte[0];
				bool flag2 = !string.IsNullOrEmpty(suppliedDomain);
				if (flag2)
				{
					flag = true;
					num |= 4096;
					array = Runtime.GetBytesForString(suppliedDomain.ToUpper(), NtlmMessage.GetOemEncoding());
				}
				else
				{
					num &= -4097;
				}
				byte[] array2 = new byte[0];
				bool flag3 = !string.IsNullOrEmpty(suppliedWorkstation);
				if (flag3)
				{
					flag = true;
					num |= 8192;
					array2 = Runtime.GetBytesForString(suppliedWorkstation.ToUpper(), NtlmMessage.GetOemEncoding());
				}
				else
				{
					num &= -8193;
				}
				byte[] array3 = new byte[flag ? (32 + array.Length + array2.Length) : 16];
				Array.Copy(NtlmMessage.NtlmsspSignature, 0, array3, 0, 8);
				NtlmMessage.WriteULong(array3, 8, 1);
				NtlmMessage.WriteULong(array3, 12, num);
				bool flag4 = flag;
				if (flag4)
				{
					NtlmMessage.WriteSecurityBuffer(array3, 16, 32, array);
					NtlmMessage.WriteSecurityBuffer(array3, 24, 32 + array.Length, array2);
				}
				result = array3;
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
			return result;
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x00023340 File Offset: 0x00021540
		public override string ToString()
		{
			string suppliedDomain = this.GetSuppliedDomain();
			string suppliedWorkstation = this.GetSuppliedWorkstation();
			return string.Concat(new string[]
			{
				"Type1Message[suppliedDomain=",
				suppliedDomain ?? "null",
				",suppliedWorkstation=",
				suppliedWorkstation ?? "null",
				",flags=0x",
				Hexdump.ToHexString(this.GetFlags(), 8),
				"]"
			});
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x000233B4 File Offset: 0x000215B4
		public static int GetDefaultFlags()
		{
			return Type1Message.DefaultFlags;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x000233CC File Offset: 0x000215CC
		public static string GetDefaultDomain()
		{
			return Type1Message.DefaultDomain;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x000233E4 File Offset: 0x000215E4
		public static string GetDefaultWorkstation()
		{
			return Type1Message.DefaultWorkstation;
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x000233FC File Offset: 0x000215FC
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
			bool flag2 = NtlmMessage.ReadULong(material, 8) != 1;
			if (flag2)
			{
				throw new IOException("Not a Type 1 message.");
			}
			int num = NtlmMessage.ReadULong(material, 12);
			string suppliedDomain = null;
			bool flag3 = (num & 4096) != 0;
			if (flag3)
			{
				byte[] chars = NtlmMessage.ReadSecurityBuffer(material, 16);
				suppliedDomain = Runtime.GetStringForBytes(chars, NtlmMessage.GetOemEncoding());
			}
			string suppliedWorkstation = null;
			bool flag4 = (num & 8192) != 0;
			if (flag4)
			{
				byte[] chars2 = NtlmMessage.ReadSecurityBuffer(material, 24);
				suppliedWorkstation = Runtime.GetStringForBytes(chars2, NtlmMessage.GetOemEncoding());
			}
			this.SetFlags(num);
			this.SetSuppliedDomain(suppliedDomain);
			this.SetSuppliedWorkstation(suppliedWorkstation);
		}

		// Token: 0x04000403 RID: 1027
		private static readonly int DefaultFlags = 512 | (Config.GetBoolean("jcifs.smb.client.useUnicode", true) ? 1 : 2);

		// Token: 0x04000404 RID: 1028
		private static readonly string DefaultDomain = Config.GetProperty("jcifs.smb.client.domain", null);

		// Token: 0x04000405 RID: 1029
		private static readonly string DefaultWorkstation;

		// Token: 0x04000406 RID: 1030
		private string _suppliedDomain;

		// Token: 0x04000407 RID: 1031
		private string _suppliedWorkstation;
	}
}
