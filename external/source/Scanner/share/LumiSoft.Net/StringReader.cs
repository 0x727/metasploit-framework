using System;
using System.Text;

namespace LumiSoft.Net
{
	// Token: 0x02000021 RID: 33
	public class StringReader
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00006F5C File Offset: 0x00005F5C
		public StringReader(string source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			this.m_OriginalString = source;
			this.m_SourceString = source;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006FA9 File Offset: 0x00005FA9
		public void AppendString(string value)
		{
			this.m_SourceString += value;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006FC0 File Offset: 0x00005FC0
		public string ReadToFirstChar()
		{
			int num = 0;
			for (int i = 0; i < this.m_SourceString.Length; i++)
			{
				bool flag = char.IsWhiteSpace(this.m_SourceString[i]);
				if (!flag)
				{
					break;
				}
				num++;
			}
			string result = this.m_SourceString.Substring(0, num);
			this.m_SourceString = this.m_SourceString.Substring(num);
			return result;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007038 File Offset: 0x00006038
		public string ReadSpecifiedLength(int length)
		{
			bool flag = this.m_SourceString.Length >= length;
			if (flag)
			{
				string result = this.m_SourceString.Substring(0, length);
				this.m_SourceString = this.m_SourceString.Substring(length);
				return result;
			}
			throw new Exception("Read length can't be bigger than source string !");
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007090 File Offset: 0x00006090
		public string QuotedReadToDelimiter(char delimiter)
		{
			return this.QuotedReadToDelimiter(new char[]
			{
				delimiter
			});
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000070B4 File Offset: 0x000060B4
		public string QuotedReadToDelimiter(char[] delimiters)
		{
			return this.QuotedReadToDelimiter(delimiters, true);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000070D0 File Offset: 0x000060D0
		public string QuotedReadToDelimiter(char[] delimiters, bool removeDelimiter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < this.m_SourceString.Length; i++)
			{
				char c = this.m_SourceString[i];
				bool flag3 = flag2;
				if (flag3)
				{
					stringBuilder.Append(c);
					flag2 = false;
				}
				else
				{
					bool flag4 = c == '\\';
					if (flag4)
					{
						stringBuilder.Append(c);
						flag2 = true;
					}
					else
					{
						bool flag5 = c == '"';
						if (flag5)
						{
							flag = !flag;
						}
						bool flag6 = false;
						foreach (char c2 in delimiters)
						{
							bool flag7 = c == c2;
							if (flag7)
							{
								flag6 = true;
								break;
							}
						}
						bool flag8 = !flag && flag6;
						if (flag8)
						{
							string result = stringBuilder.ToString();
							if (removeDelimiter)
							{
								this.m_SourceString = this.m_SourceString.Substring(i + 1);
							}
							else
							{
								this.m_SourceString = this.m_SourceString.Substring(i);
							}
							return result;
						}
						stringBuilder.Append(c);
					}
				}
			}
			this.m_SourceString = "";
			return stringBuilder.ToString();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000720C File Offset: 0x0000620C
		public string ReadWord()
		{
			return this.ReadWord(true);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007228 File Offset: 0x00006228
		public string ReadWord(bool unQuote)
		{
			return this.ReadWord(unQuote, new char[]
			{
				' ',
				',',
				';',
				'{',
				'}',
				'(',
				')',
				'[',
				']',
				'<',
				'>',
				'\r',
				'\n'
			}, false);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007254 File Offset: 0x00006254
		public string ReadWord(bool unQuote, char[] wordTerminatorChars, bool removeWordTerminator)
		{
			this.ReadToFirstChar();
			bool flag = this.Available == 0L;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.m_SourceString.StartsWith("\"");
				if (flag2)
				{
					if (unQuote)
					{
						result = TextUtils.UnQuoteString(this.QuotedReadToDelimiter(wordTerminatorChars, removeWordTerminator));
					}
					else
					{
						result = this.QuotedReadToDelimiter(wordTerminatorChars, removeWordTerminator);
					}
				}
				else
				{
					int num = 0;
					for (int i = 0; i < this.m_SourceString.Length; i++)
					{
						char c = this.m_SourceString[i];
						bool flag3 = false;
						foreach (char c2 in wordTerminatorChars)
						{
							bool flag4 = c == c2;
							if (flag4)
							{
								flag3 = true;
								break;
							}
						}
						bool flag5 = flag3;
						if (flag5)
						{
							break;
						}
						num++;
					}
					string text = this.m_SourceString.Substring(0, num);
					if (removeWordTerminator)
					{
						bool flag6 = this.m_SourceString.Length >= num + 1;
						if (flag6)
						{
							this.m_SourceString = this.m_SourceString.Substring(num + 1);
						}
					}
					else
					{
						this.m_SourceString = this.m_SourceString.Substring(num);
					}
					result = text;
				}
			}
			return result;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000073A0 File Offset: 0x000063A0
		public string ReadParenthesized()
		{
			this.ReadToFirstChar();
			bool flag = this.m_SourceString.StartsWith("{");
			char c;
			char c2;
			if (flag)
			{
				c = '{';
				c2 = '}';
			}
			else
			{
				bool flag2 = this.m_SourceString.StartsWith("(");
				if (flag2)
				{
					c = '(';
					c2 = ')';
				}
				else
				{
					bool flag3 = this.m_SourceString.StartsWith("[");
					if (flag3)
					{
						c = '[';
						c2 = ']';
					}
					else
					{
						bool flag4 = this.m_SourceString.StartsWith("<");
						if (!flag4)
						{
							throw new Exception("No parenthesized value '" + this.m_SourceString + "' !");
						}
						c = '<';
						c2 = '>';
					}
				}
			}
			bool flag5 = false;
			bool flag6 = false;
			int num = -1;
			int num2 = 0;
			for (int i = 1; i < this.m_SourceString.Length; i++)
			{
				bool flag7 = flag6;
				if (flag7)
				{
					flag6 = false;
				}
				else
				{
					bool flag8 = this.m_SourceString[i] == '\\';
					if (flag8)
					{
						flag6 = true;
					}
					else
					{
						bool flag9 = this.m_SourceString[i] == '"';
						if (flag9)
						{
							flag5 = !flag5;
						}
						else
						{
							bool flag10 = !flag5;
							if (flag10)
							{
								bool flag11 = this.m_SourceString[i] == c;
								if (flag11)
								{
									num2++;
								}
								else
								{
									bool flag12 = this.m_SourceString[i] == c2;
									if (flag12)
									{
										bool flag13 = num2 == 0;
										if (flag13)
										{
											num = i;
											break;
										}
										num2--;
									}
								}
							}
						}
					}
				}
			}
			bool flag14 = num == -1;
			if (flag14)
			{
				throw new Exception("There is no closing parenthesize for '" + this.m_SourceString + "' !");
			}
			string result = this.m_SourceString.Substring(1, num - 1);
			this.m_SourceString = this.m_SourceString.Substring(num + 1);
			return result;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000758C File Offset: 0x0000658C
		public string ReadToEnd()
		{
			bool flag = this.Available == 0L;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string sourceString = this.m_SourceString;
				this.m_SourceString = "";
				result = sourceString;
			}
			return result;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000075C4 File Offset: 0x000065C4
		public void RemoveFromEnd(int count)
		{
			bool flag = count < 0;
			if (flag)
			{
				throw new ArgumentException("Argument 'count' value must be >= 0.", "count");
			}
			this.m_SourceString = this.m_SourceString.Substring(0, this.m_SourceString.Length - count);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000760C File Offset: 0x0000660C
		public bool StartsWith(string value)
		{
			return this.m_SourceString.StartsWith(value);
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000762C File Offset: 0x0000662C
		public bool StartsWith(string value, bool case_sensitive)
		{
			bool result;
			if (case_sensitive)
			{
				result = this.m_SourceString.StartsWith(value, StringComparison.InvariantCulture);
			}
			else
			{
				result = this.m_SourceString.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
			}
			return result;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007664 File Offset: 0x00006664
		public bool EndsWith(string value)
		{
			return this.m_SourceString.EndsWith(value);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00007684 File Offset: 0x00006684
		public bool EndsWith(string value, bool case_sensitive)
		{
			bool result;
			if (case_sensitive)
			{
				result = this.m_SourceString.EndsWith(value, StringComparison.InvariantCulture);
			}
			else
			{
				result = this.m_SourceString.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
			}
			return result;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000076BC File Offset: 0x000066BC
		public bool StartsWithWord()
		{
			bool flag = this.m_SourceString.Length == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = char.IsWhiteSpace(this.m_SourceString[0]);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = char.IsSeparator(this.m_SourceString[0]);
					if (flag3)
					{
						result = false;
					}
					else
					{
						char[] array = new char[]
						{
							' ',
							',',
							';',
							'{',
							'}',
							'(',
							')',
							'[',
							']',
							'<',
							'>',
							'\r',
							'\n'
						};
						foreach (char c in array)
						{
							bool flag4 = c == this.m_SourceString[0];
							if (flag4)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00007770 File Offset: 0x00006770
		public long Available
		{
			get
			{
				return (long)this.m_SourceString.Length;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00007790 File Offset: 0x00006790
		public string OriginalString
		{
			get
			{
				return this.m_OriginalString;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000FE RID: 254 RVA: 0x000077A8 File Offset: 0x000067A8
		public string SourceString
		{
			get
			{
				return this.m_SourceString;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000FF RID: 255 RVA: 0x000077C0 File Offset: 0x000067C0
		public int Position
		{
			get
			{
				return this.m_OriginalString.Length - this.m_SourceString.Length;
			}
		}

		// Token: 0x0400006D RID: 109
		private string m_OriginalString = "";

		// Token: 0x0400006E RID: 110
		private string m_SourceString = "";
	}
}
