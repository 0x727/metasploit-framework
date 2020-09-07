using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000118 RID: 280
	public class MIME_Reader
	{
		// Token: 0x06000AFD RID: 2813 RVA: 0x00042B8C File Offset: 0x00041B8C
		public MIME_Reader(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			this.m_Source = value;
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x00042BD0 File Offset: 0x00041BD0
		public string Atom()
		{
			this.ToFirstChar();
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				int num = this.Peek(false);
				bool flag = num == -1;
				if (flag)
				{
					break;
				}
				char c = (char)num;
				bool flag2 = MIME_Reader.IsAText(c);
				if (!flag2)
				{
					break;
				}
				stringBuilder.Append((char)this.Char(false));
			}
			bool flag3 = stringBuilder.Length > 0;
			string result;
			if (flag3)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x00042C50 File Offset: 0x00041C50
		public string DotAtom()
		{
			this.ToFirstChar();
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				string text = this.Atom();
				bool flag = text == null;
				if (flag)
				{
					break;
				}
				stringBuilder.Append(text);
				bool flag2 = this.Peek(false) == 46;
				if (!flag2)
				{
					break;
				}
				stringBuilder.Append((char)this.Char(false));
			}
			bool flag3 = stringBuilder.Length > 0;
			string result;
			if (flag3)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00042CD8 File Offset: 0x00041CD8
		public string Token()
		{
			this.ToFirstChar();
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				int num = this.Peek(false);
				bool flag = num == -1;
				if (flag)
				{
					break;
				}
				char c = (char)num;
				bool flag2 = MIME_Reader.IsToken(c);
				if (!flag2)
				{
					break;
				}
				stringBuilder.Append((char)this.Char(false));
			}
			bool flag3 = stringBuilder.Length > 0;
			string result;
			if (flag3)
			{
				result = stringBuilder.ToString();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x00042D58 File Offset: 0x00041D58
		public string Comment()
		{
			this.ToFirstChar();
			bool flag = this.Peek(false) != 40;
			if (flag)
			{
				throw new InvalidOperationException("No 'comment' value available.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.Char(false);
			int num = 0;
			for (;;)
			{
				int num2 = this.Char(false);
				bool flag2 = num2 == -1;
				if (flag2)
				{
					break;
				}
				bool flag3 = num2 == 40;
				if (flag3)
				{
					num++;
				}
				else
				{
					bool flag4 = num2 == 41;
					if (flag4)
					{
						bool flag5 = num == 0;
						if (flag5)
						{
							goto Block_5;
						}
						num--;
					}
					else
					{
						stringBuilder.Append((char)num2);
					}
				}
			}
			throw new ArgumentException("Invalid 'comment' value, no closing ')'.");
			Block_5:
			return stringBuilder.ToString();
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00042E10 File Offset: 0x00041E10
		public string Word()
		{
			bool flag = this.Peek(true) == 34;
			string result;
			if (flag)
			{
				result = this.QuotedString();
			}
			else
			{
				result = this.DotAtom();
			}
			return result;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00042E44 File Offset: 0x00041E44
		public string EncodedWord()
		{
			this.ToFirstChar();
			bool flag = this.Peek(false) != 61;
			if (flag)
			{
				throw new InvalidOperationException("No encoded-word available.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				Match match = MIME_Reader.encodedword_regex.Match(this.m_Source, this.m_Offset);
				bool flag2 = match.Success && match.Index == this.m_Offset;
				if (flag2)
				{
					string value = this.m_Source.Substring(this.m_Offset, match.Length);
					this.m_Offset += match.Length;
					try
					{
						bool flag3 = string.Equals(match.Groups["encoding"].Value, "Q", StringComparison.InvariantCultureIgnoreCase);
						if (flag3)
						{
							stringBuilder.Append(MIME_Utils.QDecode(Encoding.GetEncoding(match.Groups["charset"].Value), match.Groups["value"].Value));
						}
						else
						{
							bool flag4 = string.Equals(match.Groups["encoding"].Value, "B", StringComparison.InvariantCultureIgnoreCase);
							if (flag4)
							{
								stringBuilder.Append(Encoding.GetEncoding(match.Groups["charset"].Value).GetString(Net_Utils.FromBase64(Encoding.Default.GetBytes(match.Groups["value"].Value))));
							}
							else
							{
								stringBuilder.Append(value);
							}
						}
					}
					catch
					{
						stringBuilder.Append(value);
					}
				}
				else
				{
					stringBuilder.Append(this.Atom());
				}
				match = MIME_Reader.encodedword_regex.Match(this.m_Source, this.m_Offset);
				bool flag5 = match.Success && match.Index == this.m_Offset;
				if (!flag5)
				{
					break;
				}
				this.ToFirstChar();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00043058 File Offset: 0x00042058
		public string QuotedString()
		{
			this.ToFirstChar();
			bool flag = this.Peek(false) != 34;
			if (flag)
			{
				throw new InvalidOperationException("No quoted-string available.");
			}
			this.Char(false);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag2 = false;
			for (;;)
			{
				int num = this.Char(false);
				bool flag3 = num == -1;
				if (flag3)
				{
					break;
				}
				bool flag4 = flag2;
				if (flag4)
				{
					flag2 = false;
					stringBuilder.Append((char)num);
				}
				else
				{
					bool flag5 = num == 34;
					if (flag5)
					{
						goto Block_4;
					}
					bool flag6 = num == 92;
					if (flag6)
					{
						flag2 = true;
					}
					else
					{
						bool flag7 = num == 13 || num == 10;
						if (!flag7)
						{
							stringBuilder.Append((char)num);
						}
					}
				}
			}
			throw new ArgumentException("Invalid quoted-string, end quote is missing.");
			Block_4:
			return stringBuilder.ToString();
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00043128 File Offset: 0x00042128
		public string Value()
		{
			bool flag = this.Peek(true) == 34;
			string result;
			if (flag)
			{
				result = this.QuotedString();
			}
			else
			{
				result = this.Token();
			}
			return result;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x0004315C File Offset: 0x0004215C
		public string Phrase()
		{
			int num = this.Peek(true);
			bool flag = num == -1;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = num == 34;
				if (flag2)
				{
					result = "\"" + this.QuotedString() + "\"";
				}
				else
				{
					bool flag3 = num == 61;
					if (flag3)
					{
						result = this.EncodedWord();
					}
					else
					{
						string text = this.Atom();
						bool flag4 = text == null;
						if (flag4)
						{
							result = null;
						}
						else
						{
							text = MIME_Reader.encodedword_regex.Replace(text, delegate(Match m)
							{
								string value = m.Value;
								string result2;
								try
								{
									bool flag5 = string.Equals(m.Groups["encoding"].Value, "Q", StringComparison.InvariantCultureIgnoreCase);
									if (flag5)
									{
										result2 = MIME_Utils.QDecode(Encoding.GetEncoding(m.Groups["charset"].Value), m.Groups["value"].Value);
									}
									else
									{
										bool flag6 = string.Equals(m.Groups["encoding"].Value, "B", StringComparison.InvariantCultureIgnoreCase);
										if (flag6)
										{
											result2 = Encoding.GetEncoding(m.Groups["charset"].Value).GetString(Net_Utils.FromBase64(Encoding.Default.GetBytes(m.Groups["value"].Value)));
										}
										else
										{
											result2 = value;
										}
									}
								}
								catch
								{
									result2 = value;
								}
								return result2;
							});
							result = text;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00017E58 File Offset: 0x00016E58
		public string Text()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00043208 File Offset: 0x00042208
		public string ToFirstChar()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (;;)
			{
				bool flag = this.m_Offset > this.m_Source.Length - 1;
				int num;
				if (flag)
				{
					num = -1;
				}
				else
				{
					num = (int)this.m_Source[this.m_Offset];
				}
				bool flag2 = num == -1;
				if (flag2)
				{
					break;
				}
				bool flag3 = num == 32 || num == 9 || num == 13 || num == 10;
				if (!flag3)
				{
					break;
				}
				StringBuilder stringBuilder2 = stringBuilder;
				string source = this.m_Source;
				int offset = this.m_Offset;
				this.m_Offset = offset + 1;
				stringBuilder2.Append(source[offset]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x000432C4 File Offset: 0x000422C4
		public int Char(bool readToFirstChar)
		{
			if (readToFirstChar)
			{
				this.ToFirstChar();
			}
			bool flag = this.m_Offset > this.m_Source.Length - 1;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				string source = this.m_Source;
				int offset = this.m_Offset;
				this.m_Offset = offset + 1;
				result = (int)source[offset];
			}
			return result;
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00043320 File Offset: 0x00042320
		public int Peek(bool readToFirstChar)
		{
			if (readToFirstChar)
			{
				this.ToFirstChar();
			}
			bool flag = this.m_Offset > this.m_Source.Length - 1;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)this.m_Source[this.m_Offset];
			}
			return result;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00043370 File Offset: 0x00042370
		public bool StartsWith(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			return this.m_Source.Substring(this.m_Offset).StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x000433B0 File Offset: 0x000423B0
		public string ToEnd()
		{
			bool flag = this.m_Offset >= this.m_Source.Length;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string text = this.m_Source.Substring(this.m_Offset);
				this.m_Offset = this.m_Source.Length;
				result = text;
			}
			return result;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00043408 File Offset: 0x00042408
		public static bool IsAlpha(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x00043444 File Offset: 0x00042444
		public static bool IsAText(char c)
		{
			bool flag = MIME_Reader.IsAlpha(c) || char.IsDigit(c);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (char c2 in MIME_Reader.atextChars)
				{
					bool flag2 = c == c2;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x000434A0 File Offset: 0x000424A0
		public static bool IsDotAtom(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			foreach (char c in value)
			{
				bool flag2 = c != '.' && !MIME_Reader.IsAText(c);
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00043504 File Offset: 0x00042504
		public static bool IsToken(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			bool flag2 = text == "";
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				foreach (char c in text)
				{
					bool flag3 = !MIME_Reader.IsToken(c);
					if (flag3)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00043578 File Offset: 0x00042578
		public static bool IsToken(char c)
		{
			bool flag = c <= '\u001f' || c == '\u007f';
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = c == ' ';
				if (flag2)
				{
					result = false;
				}
				else
				{
					foreach (char c2 in MIME_Reader.tspecials)
					{
						bool flag3 = c2 == c;
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x000435E4 File Offset: 0x000425E4
		public static bool IsAttributeChar(char c)
		{
			bool flag = c <= '\u001f' || c > '\u007f';
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = c == ' ' || c == '*' || c == '\'' || c == '%';
				if (flag2)
				{
					result = false;
				}
				else
				{
					foreach (char c2 in MIME_Reader.tspecials)
					{
						bool flag3 = c == c2;
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00043660 File Offset: 0x00042660
		public string ReadParenthesized()
		{
			this.ToFirstChar();
			bool flag = this.m_Source[this.m_Offset] == '{';
			char c;
			char c2;
			if (flag)
			{
				c = '{';
				c2 = '}';
			}
			else
			{
				bool flag2 = this.m_Source[this.m_Offset] == '(';
				if (flag2)
				{
					c = '(';
					c2 = ')';
				}
				else
				{
					bool flag3 = this.m_Source[this.m_Offset] == '[';
					if (flag3)
					{
						c = '[';
						c2 = ']';
					}
					else
					{
						bool flag4 = this.m_Source[this.m_Offset] == '<';
						if (!flag4)
						{
							throw new Exception("No parenthesized value '" + this.m_Source.Substring(this.m_Offset) + "' !");
						}
						c = '<';
						c2 = '>';
					}
				}
			}
			this.m_Offset++;
			bool flag5 = false;
			char c3 = '\0';
			int num = 0;
			for (int i = this.m_Offset; i < this.m_Source.Length; i++)
			{
				bool flag6 = c3 != '\\' && this.m_Source[i] == '"';
				if (flag6)
				{
					flag5 = !flag5;
				}
				else
				{
					bool flag7 = !flag5;
					if (flag7)
					{
						bool flag8 = this.m_Source[i] == c;
						if (flag8)
						{
							num++;
						}
						else
						{
							bool flag9 = this.m_Source[i] == c2;
							if (flag9)
							{
								bool flag10 = num == 0;
								if (flag10)
								{
									string result = this.m_Source.Substring(this.m_Offset, i - this.m_Offset);
									this.m_Offset = i + 1;
									return result;
								}
								num--;
							}
						}
					}
				}
				c3 = this.m_Source[i];
			}
			throw new ArgumentException("There is no closing parenthesize for '" + this.m_Source.Substring(this.m_Offset) + "' !");
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00043860 File Offset: 0x00042860
		public string QuotedReadToDelimiter(char[] delimiters)
		{
			bool flag = delimiters == null;
			if (flag)
			{
				throw new ArgumentNullException("delimiters");
			}
			bool flag2 = this.Available == 0;
			string result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				this.ToFirstChar();
				StringBuilder stringBuilder = new StringBuilder();
				bool flag3 = false;
				char c = '\0';
				for (int i = this.m_Offset; i < this.m_Source.Length; i++)
				{
					char c2 = (char)this.Peek(false);
					bool flag4 = c != '\\' && c2 == '"';
					if (flag4)
					{
						flag3 = !flag3;
					}
					bool flag5 = false;
					foreach (char c3 in delimiters)
					{
						bool flag6 = c2 == c3;
						if (flag6)
						{
							flag5 = true;
							break;
						}
					}
					bool flag7 = !flag3 && flag5;
					if (flag7)
					{
						return stringBuilder.ToString();
					}
					stringBuilder.Append(c2);
					this.m_Offset++;
					c = c2;
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000B15 RID: 2837 RVA: 0x00043974 File Offset: 0x00042974
		public int Available
		{
			get
			{
				return this.m_Source.Length - this.m_Offset;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x00043998 File Offset: 0x00042998
		public int Position
		{
			get
			{
				return this.m_Offset;
			}
		}

		// Token: 0x04000488 RID: 1160
		private string m_Source = "";

		// Token: 0x04000489 RID: 1161
		private int m_Offset = 0;

		// Token: 0x0400048A RID: 1162
		private static readonly char[] atextChars = new char[]
		{
			'!',
			'#',
			'$',
			'%',
			'&',
			'\'',
			'*',
			'+',
			'-',
			'/',
			'=',
			'?',
			'^',
			'_',
			'`',
			'{',
			'|',
			'}',
			'~'
		};

		// Token: 0x0400048B RID: 1163
		private static readonly char[] specials = new char[]
		{
			'(',
			')',
			'<',
			'>',
			'[',
			']',
			':',
			';',
			'@',
			'\\',
			',',
			'.',
			'"'
		};

		// Token: 0x0400048C RID: 1164
		private static readonly char[] tspecials = new char[]
		{
			'(',
			')',
			'<',
			'>',
			'@',
			',',
			';',
			':',
			'\\',
			'"',
			'/',
			'[',
			']',
			'?',
			'='
		};

		// Token: 0x0400048D RID: 1165
		private static readonly Regex encodedword_regex = new Regex("=\\?(?<charset>.*?)\\?(?<encoding>[qQbB])\\?(?<value>.*?)\\?=", RegexOptions.IgnoreCase);
	}
}
