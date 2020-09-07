using System;
using System.IO;
using SharpCifs.Netbios;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Ntlmssp
{
	// Token: 0x020000CE RID: 206
	public class Type2Message : NtlmMessage
	{
		// Token: 0x060006A1 RID: 1697 RVA: 0x000234D8 File Offset: 0x000216D8
		static Type2Message()
		{
			byte[] array = new byte[0];
			bool flag = Type2Message.DefaultDomain != null;
			if (flag)
			{
				try
				{
					array = Runtime.GetBytesForString(Type2Message.DefaultDomain, NtlmMessage.UniEncoding);
				}
				catch (IOException)
				{
				}
			}
			int num = array.Length;
			byte[] array2 = new byte[0];
			try
			{
				string hostName = NbtAddress.GetLocalHost().GetHostName();
				bool flag2 = hostName != null;
				if (flag2)
				{
					try
					{
						array2 = Runtime.GetBytesForString(hostName, NtlmMessage.UniEncoding);
					}
					catch (IOException)
					{
					}
				}
			}
			catch (UnknownHostException)
			{
			}
			int num2 = array2.Length;
			byte[] array3 = new byte[((num > 0) ? (num + 4) : 0) + ((num2 > 0) ? (num2 + 4) : 0) + 4];
			int num3 = 0;
			bool flag3 = num > 0;
			if (flag3)
			{
				NtlmMessage.WriteUShort(array3, num3, 2);
				num3 += 2;
				NtlmMessage.WriteUShort(array3, num3, num);
				num3 += 2;
				Array.Copy(array, 0, array3, num3, num);
				num3 += num;
			}
			bool flag4 = num2 > 0;
			if (flag4)
			{
				NtlmMessage.WriteUShort(array3, num3, 1);
				num3 += 2;
				NtlmMessage.WriteUShort(array3, num3, num2);
				num3 += 2;
				Array.Copy(array2, 0, array3, num3, num2);
			}
			Type2Message.DefaultTargetInformation = array3;
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x00023658 File Offset: 0x00021858
		public Type2Message() : this(Type2Message.GetDefaultFlags(), null, null)
		{
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x00023669 File Offset: 0x00021869
		public Type2Message(Type1Message type1) : this(type1, null, null)
		{
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00023676 File Offset: 0x00021876
		public Type2Message(Type1Message type1, byte[] challenge, string target) : this(Type2Message.GetDefaultFlags(type1), challenge, (type1 != null && target == null && type1.GetFlag(4)) ? Type2Message.GetDefaultDomain() : target)
		{
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x000236A0 File Offset: 0x000218A0
		public Type2Message(int flags, byte[] challenge, string target)
		{
			this.SetFlags(flags);
			this.SetChallenge(challenge);
			this.SetTarget(target);
			bool flag = target != null;
			if (flag)
			{
				this.SetTargetInformation(Type2Message.GetDefaultTargetInformation());
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x000236E3 File Offset: 0x000218E3
		public Type2Message(byte[] material)
		{
			this.Parse(material);
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x000236F8 File Offset: 0x000218F8
		public virtual byte[] GetChallenge()
		{
			return this._challenge;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x00023710 File Offset: 0x00021910
		public virtual void SetChallenge(byte[] challenge)
		{
			this._challenge = challenge;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0002371C File Offset: 0x0002191C
		public virtual string GetTarget()
		{
			return this._target;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00023734 File Offset: 0x00021934
		public virtual void SetTarget(string target)
		{
			this._target = target;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x00023740 File Offset: 0x00021940
		public virtual byte[] GetTargetInformation()
		{
			return this._targetInformation;
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x00023758 File Offset: 0x00021958
		public virtual void SetTargetInformation(byte[] targetInformation)
		{
			this._targetInformation = targetInformation;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00023764 File Offset: 0x00021964
		public virtual byte[] GetContext()
		{
			return this._context;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0002377C File Offset: 0x0002197C
		public virtual void SetContext(byte[] context)
		{
			this._context = context;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x00023788 File Offset: 0x00021988
		public override byte[] ToByteArray()
		{
			byte[] result;
			try
			{
				string target = this.GetTarget();
				byte[] challenge = this.GetChallenge();
				byte[] array = this.GetContext();
				byte[] targetInformation = this.GetTargetInformation();
				int num = this.GetFlags();
				byte[] array2 = new byte[0];
				bool flag = (num & 4) != 0;
				if (flag)
				{
					bool flag2 = !string.IsNullOrEmpty(target);
					if (flag2)
					{
						array2 = (((num & 1) != 0) ? Runtime.GetBytesForString(target, NtlmMessage.UniEncoding) : Runtime.GetBytesForString(target.ToUpper(), NtlmMessage.GetOemEncoding()));
					}
					else
					{
						num &= -5;
					}
				}
				bool flag3 = targetInformation != null;
				if (flag3)
				{
					num |= 8388608;
					bool flag4 = array == null;
					if (flag4)
					{
						array = new byte[8];
					}
				}
				int num2 = 32;
				bool flag5 = array != null;
				if (flag5)
				{
					num2 += 8;
				}
				bool flag6 = targetInformation != null;
				if (flag6)
				{
					num2 += 8;
				}
				byte[] array3 = new byte[num2 + array2.Length + ((targetInformation != null) ? targetInformation.Length : 0)];
				Array.Copy(NtlmMessage.NtlmsspSignature, 0, array3, 0, 8);
				NtlmMessage.WriteULong(array3, 8, 2);
				NtlmMessage.WriteSecurityBuffer(array3, 12, num2, array2);
				NtlmMessage.WriteULong(array3, 20, num);
				Array.Copy(challenge ?? new byte[8], 0, array3, 24, 8);
				bool flag7 = array != null;
				if (flag7)
				{
					Array.Copy(array, 0, array3, 32, 8);
				}
				bool flag8 = targetInformation != null;
				if (flag8)
				{
					NtlmMessage.WriteSecurityBuffer(array3, 40, num2 + array2.Length, targetInformation);
				}
				result = array3;
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException(ex.Message);
			}
			return result;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00023934 File Offset: 0x00021B34
		public override string ToString()
		{
			string target = this.GetTarget();
			byte[] challenge = this.GetChallenge();
			byte[] context = this.GetContext();
			byte[] targetInformation = this.GetTargetInformation();
			return string.Concat(new string[]
			{
				"Type2Message[target=",
				target,
				",challenge=",
				(challenge == null) ? "null" : ("<" + challenge.Length + " bytes>"),
				",context=",
				(context == null) ? "null" : ("<" + context.Length + " bytes>"),
				",targetInformation=",
				(targetInformation == null) ? "null" : ("<" + targetInformation.Length + " bytes>"),
				",flags=0x",
				Hexdump.ToHexString(this.GetFlags(), 8),
				"]"
			});
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x00023A24 File Offset: 0x00021C24
		public static int GetDefaultFlags()
		{
			return Type2Message.DefaultFlags;
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00023A3C File Offset: 0x00021C3C
		public static int GetDefaultFlags(Type1Message type1)
		{
			bool flag = type1 == null;
			int result;
			if (flag)
			{
				result = Type2Message.DefaultFlags;
			}
			else
			{
				int num = 512;
				int flags = type1.GetFlags();
				num |= (((flags & 1) != 0) ? 1 : 2);
				bool flag2 = (flags & 4) != 0;
				if (flag2)
				{
					string defaultDomain = Type2Message.GetDefaultDomain();
					bool flag3 = defaultDomain != null;
					if (flag3)
					{
						num |= 65540;
					}
				}
				result = num;
			}
			return result;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00023AA4 File Offset: 0x00021CA4
		public static string GetDefaultDomain()
		{
			return Type2Message.DefaultDomain;
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00023ABC File Offset: 0x00021CBC
		public static byte[] GetDefaultTargetInformation()
		{
			return Type2Message.DefaultTargetInformation;
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00023AD4 File Offset: 0x00021CD4
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
			bool flag2 = NtlmMessage.ReadULong(material, 8) != 2;
			if (flag2)
			{
				throw new IOException("Not a Type 2 message.");
			}
			int num = NtlmMessage.ReadULong(material, 20);
			this.SetFlags(num);
			string target = null;
			byte[] array = NtlmMessage.ReadSecurityBuffer(material, 12);
			bool flag3 = array.Length != 0;
			if (flag3)
			{
				target = Runtime.GetStringForBytes(array, ((num & 1) != 0) ? NtlmMessage.UniEncoding : NtlmMessage.GetOemEncoding());
			}
			this.SetTarget(target);
			for (int j = 24; j < 32; j++)
			{
				bool flag4 = material[j] > 0;
				if (flag4)
				{
					byte[] array2 = new byte[8];
					Array.Copy(material, 24, array2, 0, 8);
					this.SetChallenge(array2);
					break;
				}
			}
			int num2 = NtlmMessage.ReadULong(material, 16);
			bool flag5 = num2 == 32 || material.Length == 32;
			if (!flag5)
			{
				for (int k = 32; k < 40; k++)
				{
					bool flag6 = material[k] > 0;
					if (flag6)
					{
						byte[] array3 = new byte[8];
						Array.Copy(material, 32, array3, 0, 8);
						this.SetContext(array3);
						break;
					}
				}
				bool flag7 = num2 == 40 || material.Length == 40;
				if (!flag7)
				{
					array = NtlmMessage.ReadSecurityBuffer(material, 40);
					bool flag8 = array.Length != 0;
					if (flag8)
					{
						this.SetTargetInformation(array);
					}
				}
			}
		}

		// Token: 0x04000408 RID: 1032
		private static readonly int DefaultFlags = 512 | (Config.GetBoolean("jcifs.smb.client.useUnicode", true) ? 1 : 2);

		// Token: 0x04000409 RID: 1033
		private static readonly string DefaultDomain = Config.GetProperty("jcifs.smb.client.domain", null);

		// Token: 0x0400040A RID: 1034
		private static readonly byte[] DefaultTargetInformation;

		// Token: 0x0400040B RID: 1035
		private byte[] _challenge;

		// Token: 0x0400040C RID: 1036
		private string _target;

		// Token: 0x0400040D RID: 1037
		private byte[] _context;

		// Token: 0x0400040E RID: 1038
		private byte[] _targetInformation;
	}
}
