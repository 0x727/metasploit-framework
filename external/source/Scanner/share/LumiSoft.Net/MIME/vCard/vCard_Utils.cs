using System;
using System.Text;

namespace LumiSoft.Net.Mime.vCard
{
	// Token: 0x0200016A RID: 362
	internal class vCard_Utils
	{
		// Token: 0x06000EC4 RID: 3780 RVA: 0x0005C3F8 File Offset: 0x0005B3F8
		public static string Encode(string version, string value)
		{
			return vCard_Utils.Encode(version, Encoding.UTF8, value);
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0005C418 File Offset: 0x0005B418
		public static string Encode(string version, Encoding charset, string value)
		{
			bool flag = charset == null;
			if (flag)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag2 = version.StartsWith("3");
			if (flag2)
			{
				value = value.Replace("\r", "").Replace("\n", "\\n").Replace(",", "\\,");
			}
			else
			{
				value = vCard_Utils.QPEncode(charset.GetBytes(value));
			}
			return value;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0005C494 File Offset: 0x0005B494
		public static string QPEncode(byte[] data)
		{
			bool flag = data == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (byte b in data)
				{
					bool flag2 = b > 127 || b == 61 || b == 63 || b == 95 || char.IsControl((char)b);
					string value;
					if (flag2)
					{
						value = "=" + b.ToString("X2");
					}
					else
					{
						char c = (char)b;
						value = c.ToString();
					}
					stringBuilder.Append(value);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
