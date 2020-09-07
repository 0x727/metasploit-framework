using System;
using System.Text;
using SharpCifs.Util;
using SharpCifs.Util.Sharpen;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D1 RID: 209
	public class Name
	{
		// Token: 0x060006DB RID: 1755 RVA: 0x00003195 File Offset: 0x00001395
		public Name()
		{
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x00024D84 File Offset: 0x00022F84
		public Name(string name, int hexCode, string scope)
		{
			bool flag = name.Length > 15;
			if (flag)
			{
				name = Runtime.Substring(name, 0, 15);
			}
			this.name = name.ToUpper();
			this.HexCode = hexCode;
			this.Scope = ((!string.IsNullOrEmpty(scope)) ? scope : Name.DefaultScope);
			this.SrcHashCode = 0;
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x00024DE4 File Offset: 0x00022FE4
		internal virtual int WriteWireFormat(byte[] dst, int dstIndex)
		{
			dst[dstIndex] = 32;
			try
			{
				byte[] bytesForString = Runtime.GetBytesForString(this.name, Name.OemEncoding);
				int i;
				for (i = 0; i < bytesForString.Length; i++)
				{
					dst[dstIndex + (2 * i + 1)] = (byte)(((bytesForString[i] & 240) >> 4) + 65);
					dst[dstIndex + (2 * i + 2)] = (byte)((bytesForString[i] & 15) + 65);
				}
				while (i < 15)
				{
					dst[dstIndex + (2 * i + 1)] = 67;
					dst[dstIndex + (2 * i + 2)] = 65;
					i++;
				}
				dst[dstIndex + 31] = (byte)(((this.HexCode & 240) >> 4) + 65);
				dst[dstIndex + 31 + 1] = (byte)((this.HexCode & 15) + 65);
			}
			catch (UnsupportedEncodingException)
			{
			}
			return 33 + this.WriteScopeWireFormat(dst, dstIndex + 33);
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x00024EC8 File Offset: 0x000230C8
		internal virtual int ReadWireFormat(byte[] src, int srcIndex)
		{
			byte[] array = new byte[33];
			int len = 15;
			for (int i = 0; i < 15; i++)
			{
				array[i] = (byte)((src[srcIndex + (2 * i + 1)] & byte.MaxValue) - 65 << 4);
				byte[] array2 = array;
				int num = i;
				array2[num] |= (byte)((src[srcIndex + (2 * i + 2)] & byte.MaxValue) - 65 & 15);
				bool flag = array[i] != 32;
				if (flag)
				{
					len = i + 1;
				}
			}
			try
			{
				this.name = Runtime.GetStringForBytes(array, 0, len, Name.OemEncoding);
			}
			catch (UnsupportedEncodingException)
			{
			}
			this.HexCode = (int)((src[srcIndex + 31] & byte.MaxValue) - 65) << 4;
			this.HexCode |= (int)((src[srcIndex + 31 + 1] & byte.MaxValue) - 65 & 15);
			return 33 + this.ReadScopeWireFormat(src, srcIndex + 33);
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x00024FC0 File Offset: 0x000231C0
		internal int ReadWireFormatDos(byte[] src, int srcIndex)
		{
			int num = 15;
			byte[] array = new byte[num];
			Array.Copy(src, srcIndex, array, 0, num);
			try
			{
				this.name = Runtime.GetStringForBytes(array, 0, num).Trim();
			}
			catch (Exception ex)
			{
			}
			this.HexCode = (int)src[srcIndex + num];
			return num + 1;
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x00025024 File Offset: 0x00023224
		internal virtual int WriteScopeWireFormat(byte[] dst, int dstIndex)
		{
			bool flag = this.Scope == null;
			int result;
			if (flag)
			{
				dst[dstIndex] = 0;
				result = 1;
			}
			else
			{
				dst[dstIndex++] = 46;
				try
				{
					Array.Copy(Runtime.GetBytesForString(this.Scope, Name.OemEncoding), 0, dst, dstIndex, this.Scope.Length);
				}
				catch (UnsupportedEncodingException)
				{
				}
				dstIndex += this.Scope.Length;
				dst[dstIndex++] = 0;
				int num = dstIndex - 2;
				int num2 = num - this.Scope.Length;
				int num3 = 0;
				do
				{
					bool flag2 = dst[num] == 46;
					if (flag2)
					{
						dst[num] = (byte)num3;
						num3 = 0;
					}
					else
					{
						num3++;
					}
				}
				while (num-- > num2);
				result = this.Scope.Length + 2;
			}
			return result;
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x00025100 File Offset: 0x00023300
		internal virtual int ReadScopeWireFormat(byte[] src, int srcIndex)
		{
			int num = srcIndex;
			int num2;
			bool flag = (num2 = (int)(src[srcIndex++] & byte.MaxValue)) == 0;
			int result;
			if (flag)
			{
				this.Scope = null;
				result = 1;
			}
			else
			{
				try
				{
					StringBuilder stringBuilder = new StringBuilder(Runtime.GetStringForBytes(src, srcIndex, num2, Name.OemEncoding));
					srcIndex += num2;
					while ((num2 = (int)(src[srcIndex++] & 255)) != 0)
					{
						stringBuilder.Append('.').Append(Runtime.GetStringForBytes(src, srcIndex, num2, Name.OemEncoding));
						srcIndex += num2;
					}
					this.Scope = stringBuilder.ToString();
				}
				catch (UnsupportedEncodingException)
				{
				}
				result = srcIndex - num;
			}
			return result;
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x000251B8 File Offset: 0x000233B8
		public override int GetHashCode()
		{
			int num = this.name.GetHashCode();
			num += 65599 * this.HexCode;
			num += 65599 * this.SrcHashCode;
			bool flag = this.Scope != null && this.Scope.Length != 0;
			if (flag)
			{
				num += this.Scope.GetHashCode();
			}
			return num;
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x00025224 File Offset: 0x00023424
		public override bool Equals(object obj)
		{
			bool flag = !(obj is Name);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Name name = (Name)obj;
				bool flag2 = this.Scope == null && name.Scope == null;
				if (flag2)
				{
					result = (this.name.Equals(name.name) && this.HexCode == name.HexCode);
				}
				else
				{
					result = (this.name.Equals(name.name) && this.HexCode == name.HexCode && this.Scope.Equals(name.Scope));
				}
			}
			return result;
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x000252C8 File Offset: 0x000234C8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = this.name;
			bool flag = text == null;
			if (flag)
			{
				text = "null";
			}
			else
			{
				bool flag2 = text[0] == '\u0001';
				if (flag2)
				{
					char[] array = text.ToCharArray();
					array[0] = '.';
					array[1] = '.';
					array[14] = '.';
					text = new string(array);
				}
			}
			stringBuilder.Append(text).Append("<").Append(Hexdump.ToHexString(this.HexCode, 2)).Append(">");
			bool flag3 = this.Scope != null;
			if (flag3)
			{
				stringBuilder.Append(".").Append(this.Scope);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000422 RID: 1058
		private const int TypeOffset = 31;

		// Token: 0x04000423 RID: 1059
		private const int ScopeOffset = 33;

		// Token: 0x04000424 RID: 1060
		private static readonly string DefaultScope = Config.GetProperty("jcifs.netbios.scope");

		// Token: 0x04000425 RID: 1061
		internal static readonly string OemEncoding = Config.GetProperty("jcifs.encoding", Runtime.GetProperty("file.encoding"));

		// Token: 0x04000426 RID: 1062
		public string name;

		// Token: 0x04000427 RID: 1063
		public string Scope;

		// Token: 0x04000428 RID: 1064
		public int HexCode;

		// Token: 0x04000429 RID: 1065
		internal int SrcHashCode;
	}
}
