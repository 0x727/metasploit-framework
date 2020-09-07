using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200020D RID: 525
	public class IMAP_Utils
	{
		// Token: 0x06001269 RID: 4713 RVA: 0x0006F2CC File Offset: 0x0006E2CC
		public static string[] MessageFlagsAdd(string[] flags, string[] flagsToAdd)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			bool flag2 = flagsToAdd == null;
			if (flag2)
			{
				throw new ArgumentNullException("flagsToAdd");
			}
			List<string> list = new List<string>();
			list.AddRange(flags);
			foreach (string text in flagsToAdd)
			{
				bool flag3 = false;
				foreach (string a in flags)
				{
					bool flag4 = string.Equals(a, text, StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						flag3 = true;
						break;
					}
				}
				bool flag5 = !flag3;
				if (flag5)
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x0006F388 File Offset: 0x0006E388
		public static string[] MessageFlagsRemove(string[] flags, string[] flagsToRemove)
		{
			bool flag = flags == null;
			if (flag)
			{
				throw new ArgumentNullException("flags");
			}
			bool flag2 = flagsToRemove == null;
			if (flag2)
			{
				throw new ArgumentNullException("flagsToRemove");
			}
			List<string> list = new List<string>();
			foreach (string text in flags)
			{
				bool flag3 = false;
				foreach (string b in flagsToRemove)
				{
					bool flag4 = string.Equals(text, b, StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						flag3 = true;
						break;
					}
				}
				bool flag5 = !flag3;
				if (flag5)
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x0006F43C File Offset: 0x0006E43C
		public static string ACL_to_String(IMAP_ACL_Flags flags)
		{
			string text = "";
			bool flag = (flags & IMAP_ACL_Flags.l) > IMAP_ACL_Flags.None;
			if (flag)
			{
				text += "l";
			}
			bool flag2 = (flags & IMAP_ACL_Flags.r) > IMAP_ACL_Flags.None;
			if (flag2)
			{
				text += "r";
			}
			bool flag3 = (flags & IMAP_ACL_Flags.s) > IMAP_ACL_Flags.None;
			if (flag3)
			{
				text += "s";
			}
			bool flag4 = (flags & IMAP_ACL_Flags.w) > IMAP_ACL_Flags.None;
			if (flag4)
			{
				text += "w";
			}
			bool flag5 = (flags & IMAP_ACL_Flags.i) > IMAP_ACL_Flags.None;
			if (flag5)
			{
				text += "i";
			}
			bool flag6 = (flags & IMAP_ACL_Flags.p) > IMAP_ACL_Flags.None;
			if (flag6)
			{
				text += "p";
			}
			bool flag7 = (flags & IMAP_ACL_Flags.c) > IMAP_ACL_Flags.None;
			if (flag7)
			{
				text += "c";
			}
			bool flag8 = (flags & IMAP_ACL_Flags.d) > IMAP_ACL_Flags.None;
			if (flag8)
			{
				text += "d";
			}
			bool flag9 = (flags & IMAP_ACL_Flags.a) > IMAP_ACL_Flags.None;
			if (flag9)
			{
				text += "a";
			}
			return text;
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0006F548 File Offset: 0x0006E548
		public static IMAP_ACL_Flags ACL_From_String(string aclString)
		{
			IMAP_ACL_Flags imap_ACL_Flags = IMAP_ACL_Flags.None;
			aclString = aclString.ToLower();
			bool flag = aclString.IndexOf('l') > -1;
			if (flag)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.l;
			}
			bool flag2 = aclString.IndexOf('r') > -1;
			if (flag2)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.r;
			}
			bool flag3 = aclString.IndexOf('s') > -1;
			if (flag3)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.s;
			}
			bool flag4 = aclString.IndexOf('w') > -1;
			if (flag4)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.w;
			}
			bool flag5 = aclString.IndexOf('i') > -1;
			if (flag5)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.i;
			}
			bool flag6 = aclString.IndexOf('p') > -1;
			if (flag6)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.p;
			}
			bool flag7 = aclString.IndexOf('c') > -1;
			if (flag7)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.c;
			}
			bool flag8 = aclString.IndexOf('d') > -1;
			if (flag8)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.d;
			}
			bool flag9 = aclString.IndexOf('a') > -1;
			if (flag9)
			{
				imap_ACL_Flags |= IMAP_ACL_Flags.a;
			}
			return imap_ACL_Flags;
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0006F63C File Offset: 0x0006E63C
		public static DateTime ParseDate(string date)
		{
			bool flag = date == null;
			if (flag)
			{
				throw new ArgumentNullException("date");
			}
			bool flag2 = date.IndexOf('-') > -1;
			if (flag2)
			{
				try
				{
					return DateTime.ParseExact(date.Trim(), new string[]
					{
						"d-MMM-yyyy",
						"d-MMM-yyyy HH:mm:ss zzz"
					}, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None);
				}
				catch
				{
					throw new ArgumentException("Argument 'date' value '" + date + "' is not valid IMAP date.");
				}
			}
			return MIME_Utils.ParseRfc2822DateTime(date);
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0006F6CC File Offset: 0x0006E6CC
		public static string DateTimeToString(DateTime date)
		{
			string str = "";
			str += date.ToString("dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			return str + " " + date.ToString("zzz", CultureInfo.InvariantCulture).Replace(":", "");
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0006F72C File Offset: 0x0006E72C
		public static string Encode_IMAP_UTF7_String(string text)
		{
			char[] base64Chars = new char[]
			{
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'I',
				'J',
				'K',
				'L',
				'M',
				'N',
				'O',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				'a',
				'b',
				'c',
				'd',
				'e',
				'f',
				'g',
				'h',
				'i',
				'j',
				'k',
				'l',
				'm',
				'n',
				'o',
				'p',
				'q',
				'r',
				's',
				't',
				'u',
				'v',
				'w',
				'x',
				'y',
				'z',
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'+',
				','
			};
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				bool flag = c == '&';
				if (flag)
				{
					memoryStream.Write(new byte[]
					{
						38,
						45
					}, 0, 2);
				}
				else
				{
					bool flag2 = (c >= ' ' && c <= '%') || (c >= '\'' && c <= '~');
					if (flag2)
					{
						memoryStream.WriteByte((byte)c);
					}
					else
					{
						MemoryStream memoryStream2 = new MemoryStream();
						for (int j = i; j < text.Length; j++)
						{
							char c2 = text[j];
							bool flag3 = (c2 >= ' ' && c2 <= '%') || (c2 >= '\'' && c2 <= '~');
							if (flag3)
							{
								break;
							}
							memoryStream2.WriteByte((byte)((c2 & '＀') >> 8));
							memoryStream2.WriteByte((byte)(c2 & 'ÿ'));
							i = j;
						}
						byte[] array = Net_Utils.Base64EncodeEx(memoryStream2.ToArray(), base64Chars, false);
						memoryStream.WriteByte(38);
						memoryStream.Write(array, 0, array.Length);
						memoryStream.WriteByte(45);
					}
				}
			}
			return Encoding.Default.GetString(memoryStream.ToArray());
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x0006F8A4 File Offset: 0x0006E8A4
		public static string Decode_IMAP_UTF7_String(string text)
		{
			char[] base64Chars = new char[]
			{
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'I',
				'J',
				'K',
				'L',
				'M',
				'N',
				'O',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'U',
				'V',
				'W',
				'X',
				'Y',
				'Z',
				'a',
				'b',
				'c',
				'd',
				'e',
				'f',
				'g',
				'h',
				'i',
				'j',
				'k',
				'l',
				'm',
				'n',
				'o',
				'p',
				'q',
				'r',
				's',
				't',
				'u',
				'v',
				'w',
				'x',
				'y',
				'z',
				'0',
				'1',
				'2',
				'3',
				'4',
				'5',
				'6',
				'7',
				'8',
				'9',
				'+',
				','
			};
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				bool flag = c == '&';
				if (flag)
				{
					int num = -1;
					for (int j = i + 1; j < text.Length; j++)
					{
						bool flag2 = text[j] == '-';
						if (flag2)
						{
							num = j;
							break;
						}
						bool flag3 = text[j] == '&';
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = num == -1;
					if (flag4)
					{
						stringBuilder.Append(c);
					}
					else
					{
						bool flag5 = num - i == 1;
						if (flag5)
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							byte[] bytes = Encoding.Default.GetBytes(text.Substring(i + 1, num - i - 1));
							byte[] array = Net_Utils.Base64DecodeEx(bytes, base64Chars);
							char[] array2 = new char[array.Length / 2];
							for (int k = 0; k < array2.Length; k++)
							{
								array2[k] = (char)((int)array[k * 2] << 8 | (int)array[k * 2 + 1]);
							}
							stringBuilder.Append(array2);
							i += bytes.Length + 1;
						}
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x0006FA1C File Offset: 0x0006EA1C
		public static string EncodeMailbox(string mailbox, IMAP_Mailbox_Encoding encoding)
		{
			bool flag = mailbox == null;
			if (flag)
			{
				throw new ArgumentNullException("mailbox");
			}
			bool flag2 = encoding == IMAP_Mailbox_Encoding.ImapUtf7;
			string result;
			if (flag2)
			{
				result = "\"" + IMAP_Utils.Encode_IMAP_UTF7_String(mailbox) + "\"";
			}
			else
			{
				bool flag3 = encoding == IMAP_Mailbox_Encoding.ImapUtf8;
				if (flag3)
				{
					result = "\"" + mailbox + "\"";
				}
				else
				{
					result = "\"" + mailbox + "\"";
				}
			}
			return result;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x0006FA90 File Offset: 0x0006EA90
		public static string DecodeMailbox(string mailbox)
		{
			bool flag = mailbox == null;
			if (flag)
			{
				throw new ArgumentNullException("mailbox");
			}
			bool flag2 = mailbox.StartsWith("*\"");
			string result;
			if (flag2)
			{
				result = mailbox.Substring(2, mailbox.Length - 3);
			}
			else
			{
				result = IMAP_Utils.Decode_IMAP_UTF7_String(TextUtils.UnQuoteString(mailbox));
			}
			return result;
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0006FAE4 File Offset: 0x0006EAE4
		public static string NormalizeFolder(string folder)
		{
			folder = folder.Replace("\\", "/");
			bool flag = folder.StartsWith("/");
			if (flag)
			{
				folder = folder.Substring(1);
			}
			bool flag2 = folder.EndsWith("/");
			if (flag2)
			{
				folder = folder.Substring(0, folder.Length - 1);
			}
			return folder;
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0006FB48 File Offset: 0x0006EB48
		public static bool IsValidFolderName(string folder)
		{
			return true;
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0006FB5C File Offset: 0x0006EB5C
		public static bool MustUseLiteralString(string value, bool utf8StringSupported)
		{
			bool flag = value != null;
			if (flag)
			{
				foreach (char c in value)
				{
					bool flag2 = !utf8StringSupported && c > '~';
					if (flag2)
					{
						return true;
					}
					bool flag3 = char.IsControl(c);
					if (flag3)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0006FBC4 File Offset: 0x0006EBC4
		public static byte[] ImapStringToByte(Encoding charset, bool utf8StringSupported, string value)
		{
			bool flag = charset == null;
			if (flag)
			{
				throw new ArgumentNullException("charset");
			}
			bool flag2 = value == null;
			byte[] result;
			if (flag2)
			{
				result = Encoding.ASCII.GetBytes("NIL");
			}
			else
			{
				bool flag3 = value == "";
				if (flag3)
				{
					result = Encoding.ASCII.GetBytes("\"\"");
				}
				else
				{
					bool flag4 = false;
					bool flag5 = false;
					foreach (char c in value)
					{
						bool flag6 = c > '\u007f';
						if (flag6)
						{
							flag4 = true;
						}
						else
						{
							bool flag7 = char.IsControl(c);
							if (flag7)
							{
								flag5 = true;
							}
						}
					}
					bool flag8 = flag5 || (!utf8StringSupported && flag4);
					if (flag8)
					{
						byte[] bytes = charset.GetBytes(value);
						byte[] bytes2 = Encoding.ASCII.GetBytes("{" + bytes.Length + "}\r\n");
						byte[] array = new byte[bytes2.Length + bytes.Length];
						Array.Copy(bytes2, array, bytes2.Length);
						Array.Copy(bytes, 0, array, bytes2.Length, bytes.Length);
						result = array;
					}
					else if (utf8StringSupported)
					{
						result = Encoding.UTF8.GetBytes("*" + TextUtils.QuoteString(value));
					}
					else
					{
						result = charset.GetBytes(TextUtils.QuoteString(value));
					}
				}
			}
			return result;
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0006FD2C File Offset: 0x0006ED2C
		public static string ReadString(StringReader reader)
		{
			bool flag = reader == null;
			if (flag)
			{
				throw new ArgumentNullException("reader");
			}
			reader.ReadToFirstChar();
			bool flag2 = reader.SourceString.StartsWith("{");
			string result;
			if (flag2)
			{
				int length = Convert.ToInt32(reader.ReadParenthesized());
				reader.ReadSpecifiedLength(2);
				result = reader.ReadSpecifiedLength(length);
			}
			else
			{
				bool flag3 = reader.StartsWith("*\"");
				if (flag3)
				{
					reader.ReadSpecifiedLength(1);
					result = reader.ReadWord();
				}
				else
				{
					string text = reader.ReadWord(true, new char[]
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
						'\r',
						'\n'
					}, false);
					bool flag4 = string.Equals(text, "NIL", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						result = null;
					}
					else
					{
						result = text;
					}
				}
			}
			return result;
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0006FDE8 File Offset: 0x0006EDE8
		[Obsolete("Use class IMAP_t_MsgFlags instead.")]
		public static IMAP_MessageFlags ParseMessageFlags(string flagsString)
		{
			IMAP_MessageFlags imap_MessageFlags = IMAP_MessageFlags.None;
			flagsString = flagsString.ToUpper();
			bool flag = flagsString.IndexOf("ANSWERED") > -1;
			if (flag)
			{
				imap_MessageFlags |= IMAP_MessageFlags.Answered;
			}
			bool flag2 = flagsString.IndexOf("FLAGGED") > -1;
			if (flag2)
			{
				imap_MessageFlags |= IMAP_MessageFlags.Flagged;
			}
			bool flag3 = flagsString.IndexOf("DELETED") > -1;
			if (flag3)
			{
				imap_MessageFlags |= IMAP_MessageFlags.Deleted;
			}
			bool flag4 = flagsString.IndexOf("SEEN") > -1;
			if (flag4)
			{
				imap_MessageFlags |= IMAP_MessageFlags.Seen;
			}
			bool flag5 = flagsString.IndexOf("DRAFT") > -1;
			if (flag5)
			{
				imap_MessageFlags |= IMAP_MessageFlags.Draft;
			}
			return imap_MessageFlags;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0006FE88 File Offset: 0x0006EE88
		[Obsolete("Use class IMAP_t_MsgFlags instead.")]
		public static string[] MessageFlagsToStringArray(IMAP_MessageFlags msgFlags)
		{
			List<string> list = new List<string>();
			bool flag = (IMAP_MessageFlags.Answered & msgFlags) > IMAP_MessageFlags.None;
			if (flag)
			{
				list.Add("\\ANSWERED");
			}
			bool flag2 = (IMAP_MessageFlags.Flagged & msgFlags) > IMAP_MessageFlags.None;
			if (flag2)
			{
				list.Add("\\FLAGGED");
			}
			bool flag3 = (IMAP_MessageFlags.Deleted & msgFlags) > IMAP_MessageFlags.None;
			if (flag3)
			{
				list.Add("\\DELETED");
			}
			bool flag4 = (IMAP_MessageFlags.Seen & msgFlags) > IMAP_MessageFlags.None;
			if (flag4)
			{
				list.Add("\\SEEN");
			}
			bool flag5 = (IMAP_MessageFlags.Draft & msgFlags) > IMAP_MessageFlags.None;
			if (flag5)
			{
				list.Add("\\DRAFT");
			}
			return list.ToArray();
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0006FF28 File Offset: 0x0006EF28
		[Obsolete("Use method 'MessageFlagsToStringArray' instead.")]
		public static string MessageFlagsToString(IMAP_MessageFlags msgFlags)
		{
			string text = "";
			bool flag = (IMAP_MessageFlags.Answered & msgFlags) > IMAP_MessageFlags.None;
			if (flag)
			{
				text += " \\ANSWERED";
			}
			bool flag2 = (IMAP_MessageFlags.Flagged & msgFlags) > IMAP_MessageFlags.None;
			if (flag2)
			{
				text += " \\FLAGGED";
			}
			bool flag3 = (IMAP_MessageFlags.Deleted & msgFlags) > IMAP_MessageFlags.None;
			if (flag3)
			{
				text += " \\DELETED";
			}
			bool flag4 = (IMAP_MessageFlags.Seen & msgFlags) > IMAP_MessageFlags.None;
			if (flag4)
			{
				text += " \\SEEN";
			}
			bool flag5 = (IMAP_MessageFlags.Draft & msgFlags) > IMAP_MessageFlags.None;
			if (flag5)
			{
				text += " \\DRAFT";
			}
			return text.Trim();
		}
	}
}
