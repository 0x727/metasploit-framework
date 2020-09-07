using System;
using System.Text;

namespace SharpCifs.Util
{
	// Token: 0x02000007 RID: 7
	public class Base64
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00003240 File Offset: 0x00001440
		public static string Encode(byte[] bytes)
		{
			int num = bytes.Length;
			bool flag = num == 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder((int)Math.Ceiling((double)num / 3.0) * 4);
				int num2 = num % 3;
				num -= num2;
				int i = 0;
				while (i < num)
				{
					int num3 = (int)(bytes[i++] & byte.MaxValue) << 16 | (int)(bytes[i++] & byte.MaxValue) << 8 | (int)(bytes[i++] & byte.MaxValue);
					stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 18)]);
					stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 12 & 63U)]);
					stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 6 & 63U)]);
					stringBuilder.Append(Base64.Alphabet[num3 & 63]);
				}
				bool flag2 = num2 == 0;
				if (flag2)
				{
					result = stringBuilder.ToString();
				}
				else
				{
					bool flag3 = num2 == 1;
					if (flag3)
					{
						int num3 = (int)(bytes[i] & byte.MaxValue) << 4;
						stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 6)]);
						stringBuilder.Append(Base64.Alphabet[num3 & 63]);
						stringBuilder.Append("==");
						result = stringBuilder.ToString();
					}
					else
					{
						int num3 = ((int)(bytes[i++] & byte.MaxValue) << 8 | (int)(bytes[i] & byte.MaxValue)) << 2;
						stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 12)]);
						stringBuilder.Append(Base64.Alphabet[(int)((uint)num3 >> 6 & 63U)]);
						stringBuilder.Append(Base64.Alphabet[num3 & 63]);
						stringBuilder.Append("=");
						result = stringBuilder.ToString();
					}
				}
			}
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003418 File Offset: 0x00001618
		public static byte[] Decode(string @string)
		{
			int length = @string.Length;
			bool flag = length == 0;
			byte[] result;
			if (flag)
			{
				result = new byte[0];
			}
			else
			{
				int num = (@string[length - 2] == '=') ? 2 : ((@string[length - 1] == '=') ? 1 : 0);
				int num2 = length * 3 / 4 - num;
				byte[] array = new byte[num2];
				int i = 0;
				int num3 = 0;
				while (i < length)
				{
					int num4 = (Base64.Alphabet.IndexOf(@string[i++]) & 255) << 18 | (Base64.Alphabet.IndexOf(@string[i++]) & 255) << 12 | (Base64.Alphabet.IndexOf(@string[i++]) & 255) << 6 | (Base64.Alphabet.IndexOf(@string[i++]) & 255);
					array[num3++] = (byte)((uint)num4 >> 16);
					bool flag2 = num3 < num2;
					if (flag2)
					{
						array[num3++] = (byte)((uint)num4 >> 8 & 255U);
					}
					bool flag3 = num3 < num2;
					if (flag3)
					{
						array[num3++] = (byte)(num4 & 255);
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x04000014 RID: 20
		private static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	}
}
