using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LumiSoft.Net.MIME
{
	// Token: 0x02000108 RID: 264
	public class MIME_Encoding_EncodedWord
	{
		// Token: 0x06000A2D RID: 2605 RVA: 0x0003E758 File Offset: 0x0003D758
		public MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding encoding, Encoding charset)
		{
			bool flag = charset == null;
			if (flag)
			{
				throw new ArgumentNullException("charset");
			}
			this.m_Encoding = encoding;
			this.m_pCharset = charset;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0003E7A0 File Offset: 0x0003D7A0
		public string Encode(string text)
		{
			bool flag = MIME_Encoding_EncodedWord.MustEncode(text);
			string result;
			if (flag)
			{
				result = MIME_Encoding_EncodedWord.EncodeS(this.m_Encoding, this.m_pCharset, this.m_Split, text);
			}
			else
			{
				result = text;
			}
			return result;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0003E7DC File Offset: 0x0003D7DC
		public string Decode(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			return MIME_Encoding_EncodedWord.DecodeS(text);
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x0003E808 File Offset: 0x0003D808
		public static bool MustEncode(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			foreach (char c in text)
			{
				bool flag2 = c > '\u007f';
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0003E860 File Offset: 0x0003D860
		public static string EncodeS(MIME_EncodedWordEncoding encoding, Encoding charset, bool split, string text)
		{
			bool flag = charset == null;
			if (flag)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag2 = text == null;
			if (flag2)
			{
				throw new ArgumentNullException("text");
			}
			bool flag3 = MIME_Encoding_EncodedWord.MustEncode(text);
			string result;
			if (flag3)
			{
				List<string> list = new List<string>();
				if (split)
				{
					int num;
					for (int i = 0; i < text.Length; i += num)
					{
						num = Math.Min(30, text.Length - i);
						list.Add(text.Substring(i, num));
					}
				}
				else
				{
					list.Add(text);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < list.Count; j++)
				{
					string s = list[j];
					byte[] bytes = charset.GetBytes(s);
					bool flag4 = encoding == MIME_EncodedWordEncoding.B;
					if (flag4)
					{
						stringBuilder.Append(string.Concat(new string[]
						{
							"=?",
							charset.WebName,
							"?B?",
							Convert.ToBase64String(bytes),
							"?="
						}));
					}
					else
					{
						stringBuilder.Append("=?" + charset.WebName + "?Q?");
						int num2 = 0;
						foreach (byte b in bytes)
						{
							bool flag5 = b > 127 || b == 61 || b == 63 || b == 95 || b == 32;
							string text2;
							if (flag5)
							{
								text2 = "=" + b.ToString("X2");
							}
							else
							{
								char c = (char)b;
								text2 = c.ToString();
							}
							stringBuilder.Append(text2);
							num2 += text2.Length;
						}
						stringBuilder.Append("?=");
					}
					bool flag6 = j < list.Count - 1;
					if (flag6)
					{
						stringBuilder.Append("\r\n ");
					}
				}
				result = stringBuilder.ToString();
			}
			else
			{
				result = text;
			}
			return result;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x0003EA80 File Offset: 0x0003DA80
		public static string DecodeS(string word)
		{
			bool flag = word == null;
			if (flag)
			{
				throw new ArgumentNullException("word");
			}
			return MIME_Encoding_EncodedWord.DecodeTextS(word);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0003EAAC File Offset: 0x0003DAAC
		public static string DecodeTextS(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("word");
			}
			string retVal = text;
			retVal = MIME_Encoding_EncodedWord.encodedword_regex.Replace(retVal, delegate(Match m)
			{
				string text2 = m.Value;
				string result;
				try
				{
					bool flag2 = string.Equals(m.Groups["encoding"].Value, "Q", StringComparison.InvariantCultureIgnoreCase);
					if (flag2)
					{
						text2 = MIME_Utils.QDecode(Encoding.GetEncoding(m.Groups["charset"].Value), m.Groups["value"].Value);
					}
					else
					{
						bool flag3 = string.Equals(m.Groups["encoding"].Value, "B", StringComparison.InvariantCultureIgnoreCase);
						if (flag3)
						{
							text2 = Encoding.GetEncoding(m.Groups["charset"].Value).GetString(Net_Utils.FromBase64(Encoding.Default.GetBytes(m.Groups["value"].Value)));
						}
					}
					Match match = MIME_Encoding_EncodedWord.encodedword_regex.Match(retVal, m.Index + m.Length);
					bool flag4 = !match.Success || match.Index != m.Index + m.Length;
					if (flag4)
					{
						text2 += m.Groups["whitespaces"].Value;
					}
					result = text2;
				}
				catch
				{
					result = text2;
				}
				return result;
			});
			return retVal;
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000A34 RID: 2612 RVA: 0x0003EB08 File Offset: 0x0003DB08
		// (set) Token: 0x06000A35 RID: 2613 RVA: 0x0003EB20 File Offset: 0x0003DB20
		public bool Split
		{
			get
			{
				return this.m_Split;
			}
			set
			{
				this.m_Split = value;
			}
		}

		// Token: 0x0400045B RID: 1115
		private MIME_EncodedWordEncoding m_Encoding;

		// Token: 0x0400045C RID: 1116
		private Encoding m_pCharset = null;

		// Token: 0x0400045D RID: 1117
		private bool m_Split = true;

		// Token: 0x0400045E RID: 1118
		private static readonly Regex encodedword_regex = new Regex("\\=\\?(?<charset>\\S+?)\\?(?<encoding>[qQbB])\\?(?<value>.+?)\\?\\=(?<whitespaces>\\s*)", RegexOptions.IgnoreCase);
	}
}
