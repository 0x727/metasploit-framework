using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net
{
	// Token: 0x02000022 RID: 34
	public class TextUtils
	{
		// Token: 0x06000100 RID: 256 RVA: 0x000077EC File Offset: 0x000067EC
		public static string QuoteString(string text)
		{
			bool flag = text != null && text.StartsWith("\"") && text.EndsWith("\"");
			string result;
			if (flag)
			{
				result = text;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (char c in text)
				{
					bool flag2 = c == '\\';
					if (flag2)
					{
						stringBuilder.Append("\\\\");
					}
					else
					{
						bool flag3 = c == '"';
						if (flag3)
						{
							stringBuilder.Append("\\\"");
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
				}
				result = "\"" + stringBuilder.ToString() + "\"";
			}
			return result;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000078A8 File Offset: 0x000068A8
		public static string UnQuoteString(string text)
		{
			int num = 0;
			int num2 = text.Length;
			for (int i = 0; i < num2; i++)
			{
				char c = text[i];
				bool flag = c == ' ' || c == '\t';
				if (!flag)
				{
					break;
				}
				num++;
			}
			for (int j = num2 - 1; j > 0; j--)
			{
				char c2 = text[j];
				bool flag2 = c2 == ' ' || c2 == '\t';
				if (!flag2)
				{
					break;
				}
				num2--;
			}
			bool flag3 = num2 - num <= 0;
			string result;
			if (flag3)
			{
				result = "";
			}
			else
			{
				bool flag4 = text[num] == '"';
				if (flag4)
				{
					num++;
				}
				bool flag5 = text[num2 - 1] == '"';
				if (flag5)
				{
					num2--;
				}
				bool flag6 = num2 == num - 1;
				if (flag6)
				{
					result = "";
				}
				else
				{
					char[] array = new char[num2 - num];
					int num3 = 0;
					bool flag7 = false;
					for (int k = num; k < num2; k++)
					{
						char c3 = text[k];
						bool flag8 = !flag7 && c3 == '\\';
						if (flag8)
						{
							flag7 = true;
						}
						else
						{
							bool flag9 = flag7;
							if (flag9)
							{
								array[num3] = c3;
								num3++;
								flag7 = false;
							}
							else
							{
								array[num3] = c3;
								num3++;
								flag7 = false;
							}
						}
					}
					result = new string(array, 0, num3);
				}
			}
			return result;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00007A28 File Offset: 0x00006A28
		public static string EscapeString(string text, char[] charsToEscape)
		{
			char[] array = new char[text.Length * 2];
			int num = 0;
			foreach (char c in text)
			{
				foreach (char c2 in charsToEscape)
				{
					bool flag = c == c2;
					if (flag)
					{
						array[num] = '\\';
						num++;
						break;
					}
				}
				array[num] = c;
				num++;
			}
			return new string(array, 0, num);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00007AB4 File Offset: 0x00006AB4
		public static string UnEscapeString(string text)
		{
			char[] array = new char[text.Length];
			int num = 0;
			bool flag = false;
			foreach (char c in text)
			{
				bool flag2 = !flag && c == '\\';
				if (flag2)
				{
					flag = true;
				}
				else
				{
					array[num] = c;
					num++;
					flag = false;
				}
			}
			return new string(array, 0, num);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007B2C File Offset: 0x00006B2C
		public static string[] SplitQuotedString(string text, char splitChar)
		{
			return TextUtils.SplitQuotedString(text, splitChar, false);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007B48 File Offset: 0x00006B48
		public static string[] SplitQuotedString(string text, char splitChar, bool unquote)
		{
			return TextUtils.SplitQuotedString(text, splitChar, unquote, int.MaxValue);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007B68 File Offset: 0x00006B68
		public static string[] SplitQuotedString(string text, char splitChar, bool unquote, int count)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			List<string> list = new List<string>();
			int num = 0;
			bool flag2 = false;
			char c = '0';
			for (int i = 0; i < text.Length; i++)
			{
				char c2 = text[i];
				bool flag3 = list.Count + 1 >= count;
				if (flag3)
				{
					break;
				}
				bool flag4 = c != '\\' && c2 == '"';
				if (flag4)
				{
					flag2 = !flag2;
				}
				bool flag5 = !flag2;
				if (flag5)
				{
					bool flag6 = c2 == splitChar;
					if (flag6)
					{
						if (unquote)
						{
							list.Add(TextUtils.UnQuoteString(text.Substring(num, i - num)));
						}
						else
						{
							list.Add(text.Substring(num, i - num));
						}
						num = i + 1;
					}
				}
				c = c2;
			}
			if (unquote)
			{
				list.Add(TextUtils.UnQuoteString(text.Substring(num, text.Length - num)));
			}
			else
			{
				list.Add(text.Substring(num, text.Length - num));
			}
			return list.ToArray();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007C9C File Offset: 0x00006C9C
		public static int QuotedIndexOf(string text, char indexChar)
		{
			int result = -1;
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				bool flag2 = c == '"';
				if (flag2)
				{
					flag = !flag;
				}
				bool flag3 = !flag && c == indexChar;
				if (flag3)
				{
					return i;
				}
			}
			return result;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007D00 File Offset: 0x00006D00
		public static string[] SplitString(string text, char splitChar)
		{
			ArrayList arrayList = new ArrayList();
			int num = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				bool flag = text[i] == splitChar;
				if (flag)
				{
					arrayList.Add(text.Substring(num, i - num));
					num = i + 1;
				}
			}
			bool flag2 = num <= length;
			if (flag2)
			{
				arrayList.Add(text.Substring(num));
			}
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00007D98 File Offset: 0x00006D98
		public static bool IsToken(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException(value);
			}
			char[] array = new char[]
			{
				'-',
				'.',
				'!',
				'%',
				'*',
				'_',
				'+',
				'`',
				'\'',
				'~'
			};
			foreach (char c in value)
			{
				bool flag2 = (c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && (c < '0' || c > '9');
				if (flag2)
				{
					bool flag3 = false;
					foreach (char c2 in array)
					{
						bool flag4 = c == c2;
						if (flag4)
						{
							flag3 = true;
							break;
						}
					}
					bool flag5 = !flag3;
					if (flag5)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
