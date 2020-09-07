using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using LumiSoft.Net.DNS;
using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net
{
	// Token: 0x02000019 RID: 25
	[Obsolete("")]
	public class Core
	{
		// Token: 0x06000070 RID: 112 RVA: 0x000044C0 File Offset: 0x000034C0
		public static string GetHostName(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			string result = ip.ToString();
			try
			{
				Dns_Client dns_Client = new Dns_Client();
				DnsServerResponse dnsServerResponse = dns_Client.Query(ip.ToString(), DNS_QType.PTR);
				bool flag2 = dnsServerResponse.ResponseCode == DNS_RCode.NO_ERROR;
				if (flag2)
				{
					DNS_rr_PTR[] ptrrecords = dnsServerResponse.GetPTRRecords();
					bool flag3 = ptrrecords.Length != 0;
					if (flag3)
					{
						result = ptrrecords[0].DomainName;
					}
				}
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004550 File Offset: 0x00003550
		public static string GetArgsText(string input, string cmdTxtToRemove)
		{
			string text = input.Trim();
			bool flag = text.Length >= cmdTxtToRemove.Length;
			if (flag)
			{
				text = text.Substring(cmdTxtToRemove.Length);
			}
			return text.Trim();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004598 File Offset: 0x00003598
		[Obsolete("Use Net_Utils.IsInteger instead of it")]
		public static bool IsNumber(string str)
		{
			bool result;
			try
			{
				Convert.ToInt64(str);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000045CC File Offset: 0x000035CC
		[Obsolete("Use Net_Utils.ReverseArray instead of it")]
		public static Array ReverseArray(Array array)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			Array.Reverse(array);
			return array;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000045FC File Offset: 0x000035FC
		public static byte[] Base64Encode(byte[] data)
		{
			return Core.Base64EncodeEx(data, null, true);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004618 File Offset: 0x00003618
		public static byte[] Base64EncodeEx(byte[] data, char[] base64Chars, bool padd)
		{
			bool flag = base64Chars != null && base64Chars.Length != 64;
			if (flag)
			{
				throw new Exception("There must be 64 chars in base64Chars char array !");
			}
			bool flag2 = base64Chars == null;
			if (flag2)
			{
				base64Chars = new char[]
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
					'/'
				};
			}
			byte[] array = new byte[64];
			for (int i = 0; i < 64; i++)
			{
				array[i] = (byte)base64Chars[i];
			}
			int num = (int)Math.Ceiling((double)(data.Length * 8) / 6.0);
			bool flag3 = padd && (double)num / 4.0 != Math.Ceiling((double)num / 4.0);
			if (flag3)
			{
				num += (int)(Math.Ceiling((double)num / 4.0) * 4.0) - num;
			}
			int num2 = 0;
			bool flag4 = num > 76;
			if (flag4)
			{
				num2 = (int)Math.Ceiling((double)num / 76.0) - 1;
			}
			byte[] array2 = new byte[num + num2 * 2];
			int num3 = 0;
			int num4 = 0;
			for (int j = 0; j < data.Length; j += 3)
			{
				bool flag5 = num3 >= 76;
				if (flag5)
				{
					array2[num4] = 13;
					array2[num4 + 1] = 10;
					num4 += 2;
					num3 = 0;
				}
				bool flag6 = data.Length - j >= 3;
				if (flag6)
				{
					array2[num4] = array[data[j] >> 2];
					array2[num4 + 1] = array[(int)(data[j] & 3) << 4 | data[j + 1] >> 4];
					array2[num4 + 2] = array[(int)(data[j + 1] & 15) << 2 | data[j + 2] >> 6];
					array2[num4 + 3] = array[(int)(data[j + 2] & 63)];
					num4 += 4;
					num3 += 4;
				}
				else
				{
					bool flag7 = data.Length - j == 2;
					if (flag7)
					{
						array2[num4] = array[data[j] >> 2];
						array2[num4 + 1] = array[(int)(data[j] & 3) << 4 | data[j + 1] >> 4];
						array2[num4 + 2] = array[(int)(data[j + 1] & 15) << 2];
						if (padd)
						{
							array2[num4 + 3] = 61;
						}
					}
					else
					{
						bool flag8 = data.Length - j == 1;
						if (flag8)
						{
							array2[num4] = array[data[j] >> 2];
							array2[num4 + 1] = array[(int)(data[j] & 3) << 4];
							if (padd)
							{
								array2[num4 + 2] = 61;
								array2[num4 + 3] = 61;
							}
						}
					}
				}
			}
			return array2;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004898 File Offset: 0x00003898
		[Obsolete("Use Net_Utils.FromBase64 instead of it")]
		public static byte[] Base64Decode(byte[] base64Data)
		{
			return Core.Base64DecodeEx(base64Data, null);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000048B4 File Offset: 0x000038B4
		public static byte[] Base64DecodeEx(byte[] base64Data, char[] base64Chars)
		{
			bool flag = base64Chars != null && base64Chars.Length != 64;
			if (flag)
			{
				throw new Exception("There must be 64 chars in base64Chars char array !");
			}
			bool flag2 = base64Chars == null;
			if (flag2)
			{
				base64Chars = new char[]
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
					'/'
				};
			}
			byte[] array = new byte[128];
			for (int i = 0; i < 128; i++)
			{
				int num = -1;
				for (int j = 0; j < base64Chars.Length; j++)
				{
					bool flag3 = i == (int)base64Chars[j];
					if (flag3)
					{
						num = j;
						break;
					}
				}
				bool flag4 = num > -1;
				if (flag4)
				{
					array[i] = (byte)num;
				}
				else
				{
					array[i] = byte.MaxValue;
				}
			}
			byte[] array2 = new byte[base64Data.Length * 6 / 8 + 4];
			int num2 = 0;
			int num3 = 0;
			byte[] array3 = new byte[3];
			byte[] array4 = new byte[4];
			for (int k = 0; k < base64Data.Length; k++)
			{
				byte b = base64Data[k];
				bool flag5 = b == 61;
				if (flag5)
				{
					array4[num3] = byte.MaxValue;
				}
				else
				{
					byte b2 = array[(int)(b & 127)];
					bool flag6 = b2 != byte.MaxValue;
					if (flag6)
					{
						array4[num3] = b2;
						num3++;
					}
				}
				int num4 = -1;
				bool flag7 = num3 == 4;
				if (flag7)
				{
					num4 = 3;
				}
				else
				{
					bool flag8 = k == base64Data.Length - 1;
					if (flag8)
					{
						bool flag9 = num3 == 1;
						if (flag9)
						{
							num4 = 0;
						}
						else
						{
							bool flag10 = num3 == 2;
							if (flag10)
							{
								num4 = 1;
							}
							else
							{
								bool flag11 = num3 == 3;
								if (flag11)
								{
									num4 = 2;
								}
							}
						}
					}
				}
				bool flag12 = num4 > -1;
				if (flag12)
				{
					array2[num2] = (byte)((int)array4[0] << 2 | array4[1] >> 4);
					array2[num2 + 1] = (byte)((int)(array4[1] & 15) << 4 | array4[2] >> 2);
					array2[num2 + 2] = (byte)((int)(array4[2] & 3) << 6 | (int)array4[3]);
					num2 += num4;
					num3 = 0;
				}
			}
			bool flag13 = num2 > -1;
			byte[] result;
			if (flag13)
			{
				byte[] array5 = new byte[num2];
				Array.Copy(array2, 0, array5, 0, num2);
				result = array5;
			}
			else
			{
				result = new byte[0];
			}
			return result;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004AE0 File Offset: 0x00003AE0
		public static byte[] QuotedPrintableEncode(byte[] data)
		{
			int num = 0;
			MemoryStream memoryStream = new MemoryStream();
			foreach (byte b in data)
			{
				bool flag = num > 75;
				if (flag)
				{
					memoryStream.Write(new byte[]
					{
						61,
						13,
						10
					}, 0, 3);
					num = 0;
				}
				bool flag2 = b <= 33 || b >= 126 || b == 61;
				if (flag2)
				{
					memoryStream.Write(new byte[]
					{
						61
					}, 0, 1);
					memoryStream.Write(Core.ToHex(b), 0, 2);
					num += 3;
				}
				else
				{
					memoryStream.WriteByte(b);
					num++;
				}
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004BA0 File Offset: 0x00003BA0
		[Obsolete("Use MIME_Utils.QuotedPrintableDecode instead of it")]
		public static byte[] QuotedPrintableDecode(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream();
			MemoryStream memoryStream2 = new MemoryStream(data);
			for (int i = memoryStream2.ReadByte(); i > -1; i = memoryStream2.ReadByte())
			{
				bool flag = i == 61;
				if (flag)
				{
					byte[] array = new byte[2];
					int num = memoryStream2.Read(array, 0, 2);
					bool flag2 = num == 2;
					if (flag2)
					{
						bool flag3 = array[0] == 13 && array[1] == 10;
						if (!flag3)
						{
							try
							{
								memoryStream.Write(Core.FromHex(array), 0, 1);
							}
							catch
							{
								memoryStream.WriteByte(61);
								memoryStream.Write(array, 0, 2);
							}
						}
					}
					else
					{
						memoryStream.Write(array, 0, num);
					}
				}
				else
				{
					memoryStream.WriteByte((byte)i);
				}
			}
			return memoryStream.ToArray();
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004C94 File Offset: 0x00003C94
		[Obsolete("Use MIME_Utils.QDecode instead of it")]
		public static string QDecode(Encoding encoding, string data)
		{
			return encoding.GetString(Core.QuotedPrintableDecode(Encoding.ASCII.GetBytes(data.Replace("_", " "))));
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004CCC File Offset: 0x00003CCC
		[Obsolete("Use MimeUtils.DecodeWords method instead.")]
		public static string CanonicalDecode(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < text.Length)
			{
				int num = text.IndexOf("=?", i);
				int num2 = -1;
				bool flag = num > -1;
				if (flag)
				{
					num2 = text.IndexOf("?=", num + 2);
				}
				bool flag2 = num > -1 && num2 > -1;
				if (flag2)
				{
					bool flag3 = num - i > 0;
					if (flag3)
					{
						stringBuilder.Append(text.Substring(i, num - i));
					}
					for (;;)
					{
						string[] array = text.Substring(num + 2, num2 - num - 2).Split(new char[]
						{
							'?'
						});
						bool flag4 = array.Length == 3;
						if (flag4)
						{
							//goto Block_5;
							try
							{
								//string[] array;
								Encoding encoding = Encoding.GetEncoding(array[0]);
								bool flag7 = array[1].ToLower() == "q";
								if (flag7)
								{
									stringBuilder.Append(Core.QDecode(encoding, array[2]));
								}
								else
								{
									stringBuilder.Append(encoding.GetString(Core.Base64Decode(Encoding.Default.GetBytes(array[2]))));
								}
							}
							catch
							{
								stringBuilder.Append(text.Substring(num, num2 - num + 2));
							}
							i = num2 + 2;
							break;
						}
						bool flag5 = array.Length < 3;
						if (!flag5)
						{
							//goto IL_160;
							stringBuilder.Append("=?");
							i = num + 2;
							break;
						}
						num2 = text.IndexOf("?=", num2 + 2);
						bool flag6 = num2 == -1;
						if (flag6)
						{
							//goto Block_7;
							stringBuilder.Append("=?");
							i = num + 2;
							break;
						}
					}
					continue;
					//Block_5:
					
					//continue;
					//Block_7:
					
					//continue;
					//IL_160:
					
				}
				else
				{
					bool flag8 = text.Length > i;
					if (flag8)
					{
						stringBuilder.Append(text.Substring(i));
						i = text.Length;
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004EB0 File Offset: 0x00003EB0
		public static string CanonicalEncode(string str, string charSet)
		{
			bool flag = !Core.IsAscii(str);
			string result;
			if (flag)
			{
				string text = "=?" + charSet + "?B?";
				text += Convert.ToBase64String(Encoding.GetEncoding(charSet).GetBytes(str));
				text += "?=";
				result = text;
			}
			else
			{
				result = str;
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004F0C File Offset: 0x00003F0C
		[Obsolete("Use IMAP_Utils.Encode_IMAP_UTF7_String instead of it")]
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
						byte[] array = Core.Base64EncodeEx(memoryStream2.ToArray(), base64Chars, false);
						memoryStream.WriteByte(38);
						memoryStream.Write(array, 0, array.Length);
						memoryStream.WriteByte(45);
					}
				}
			}
			return Encoding.Default.GetString(memoryStream.ToArray());
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005084 File Offset: 0x00004084
		[Obsolete("Use IMAP_Utils.Decode_IMAP_UTF7_String instead of it")]
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
							byte[] array = Core.Base64DecodeEx(bytes, base64Chars);
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

		// Token: 0x0600007F RID: 127 RVA: 0x000051FC File Offset: 0x000041FC
		[Obsolete("Use Net_Utils.IsAscii instead of it")]
		public static bool IsAscii(string data)
		{
			foreach (char c in data)
			{
				bool flag = c > '\u007f';
				if (flag)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005240 File Offset: 0x00004240
		public static string GetFileNameFromPath(string filePath)
		{
			return Path.GetFileName(filePath);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005258 File Offset: 0x00004258
		public static bool IsIP(string value)
		{
			bool result;
			try
			{
				IPAddress ipaddress = null;
				result = IPAddress.TryParse(value, out ipaddress);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000528C File Offset: 0x0000428C
		public static int CompareIP(IPAddress source, IPAddress destination)
		{
			byte[] addressBytes = source.GetAddressBytes();
			byte[] addressBytes2 = destination.GetAddressBytes();
			bool flag = addressBytes.Length < addressBytes2.Length;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = addressBytes.Length > addressBytes2.Length;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					for (int i = 0; i < addressBytes.Length; i++)
					{
						bool flag3 = addressBytes[i] < addressBytes2[i];
						if (flag3)
						{
							return 1;
						}
						bool flag4 = addressBytes[i] > addressBytes2[i];
						if (flag4)
						{
							return -1;
						}
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005318 File Offset: 0x00004318
		[Obsolete("Use Net_Utils.IsPrivateIP instead of it")]
		public static bool IsPrivateIP(string ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			return Core.IsPrivateIP(IPAddress.Parse(ip));
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000534C File Offset: 0x0000434C
		[Obsolete("Use Net_Utils.IsPrivateIP instead of it")]
		public static bool IsPrivateIP(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool flag2 = ip.AddressFamily == AddressFamily.InterNetwork;
			if (flag2)
			{
				byte[] addressBytes = ip.GetAddressBytes();
				bool flag3 = addressBytes[0] == 192 && addressBytes[1] == 168;
				if (flag3)
				{
					return true;
				}
				bool flag4 = addressBytes[0] == 172 && addressBytes[1] >= 16 && addressBytes[1] <= 31;
				if (flag4)
				{
					return true;
				}
				bool flag5 = addressBytes[0] == 10;
				if (flag5)
				{
					return true;
				}
				bool flag6 = addressBytes[0] == 169 && addressBytes[1] == 254;
				if (flag6)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005410 File Offset: 0x00004410
		[Obsolete("Use Net_Utils.CreateSocket instead of it")]
		public static Socket CreateSocket(IPEndPoint localEP, ProtocolType protocolType)
		{
			SocketType socketType = SocketType.Stream;
			bool flag = protocolType == ProtocolType.Udp;
			if (flag)
			{
				socketType = SocketType.Dgram;
			}
			bool flag2 = localEP.AddressFamily == AddressFamily.InterNetwork;
			Socket result;
			if (flag2)
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
				socket.Bind(localEP);
				result = socket;
			}
			else
			{
				bool flag3 = localEP.AddressFamily == AddressFamily.InterNetworkV6;
				if (!flag3)
				{
					throw new ArgumentException("Invalid IPEndPoint address family.");
				}
				Socket socket2 = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
				socket2.Bind(localEP);
				result = socket2;
			}
			return result;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000548C File Offset: 0x0000448C
		public static string ToHexString(string data)
		{
			return Encoding.Default.GetString(Core.ToHex(Encoding.Default.GetBytes(data)));
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000054B8 File Offset: 0x000044B8
		public static string ToHexString(byte[] data)
		{
			return Encoding.Default.GetString(Core.ToHex(data));
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000054DC File Offset: 0x000044DC
		public static byte[] ToHex(byte byteValue)
		{
			return Core.ToHex(new byte[]
			{
				byteValue
			});
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005500 File Offset: 0x00004500
		public static byte[] ToHex(byte[] data)
		{
			char[] array = new char[]
			{
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
				'A',
				'B',
				'C',
				'D',
				'E',
				'F'
			};
			MemoryStream memoryStream = new MemoryStream(data.Length * 2);
			foreach (byte b in data)
			{
				memoryStream.Write(new byte[]
				{
					(byte)array[(b & 240) >> 4],
					(byte)array[(int)(b & 15)]
				}, 0, 2);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005580 File Offset: 0x00004580
		[Obsolete("Use Net_Utils.FromHex instead of it")]
		public static byte[] FromHex(byte[] hexData)
		{
			bool flag = hexData.Length < 2 || (double)hexData.Length / 2.0 != Math.Floor((double)hexData.Length / 2.0);
			if (flag)
			{
				throw new Exception("Illegal hex data, hex data must be in two bytes pairs, for example: 0F,FF,A3,... .");
			}
			MemoryStream memoryStream = new MemoryStream(hexData.Length / 2);
			for (int i = 0; i < hexData.Length; i += 2)
			{
				byte[] array = new byte[2];
				for (int j = 0; j < 2; j++)
				{
					bool flag2 = hexData[i + j] == 48;
					if (flag2)
					{
						array[j] = 0;
					}
					else
					{
						bool flag3 = hexData[i + j] == 49;
						if (flag3)
						{
							array[j] = 1;
						}
						else
						{
							bool flag4 = hexData[i + j] == 50;
							if (flag4)
							{
								array[j] = 2;
							}
							else
							{
								bool flag5 = hexData[i + j] == 51;
								if (flag5)
								{
									array[j] = 3;
								}
								else
								{
									bool flag6 = hexData[i + j] == 52;
									if (flag6)
									{
										array[j] = 4;
									}
									else
									{
										bool flag7 = hexData[i + j] == 53;
										if (flag7)
										{
											array[j] = 5;
										}
										else
										{
											bool flag8 = hexData[i + j] == 54;
											if (flag8)
											{
												array[j] = 6;
											}
											else
											{
												bool flag9 = hexData[i + j] == 55;
												if (flag9)
												{
													array[j] = 7;
												}
												else
												{
													bool flag10 = hexData[i + j] == 56;
													if (flag10)
													{
														array[j] = 8;
													}
													else
													{
														bool flag11 = hexData[i + j] == 57;
														if (flag11)
														{
															array[j] = 9;
														}
														else
														{
															bool flag12 = hexData[i + j] == 65 || hexData[i + j] == 97;
															if (flag12)
															{
																array[j] = 10;
															}
															else
															{
																bool flag13 = hexData[i + j] == 66 || hexData[i + j] == 98;
																if (flag13)
																{
																	array[j] = 11;
																}
																else
																{
																	bool flag14 = hexData[i + j] == 67 || hexData[i + j] == 99;
																	if (flag14)
																	{
																		array[j] = 12;
																	}
																	else
																	{
																		bool flag15 = hexData[i + j] == 68 || hexData[i + j] == 100;
																		if (flag15)
																		{
																			array[j] = 13;
																		}
																		else
																		{
																			bool flag16 = hexData[i + j] == 69 || hexData[i + j] == 101;
																			if (flag16)
																			{
																				array[j] = 14;
																			}
																			else
																			{
																				bool flag17 = hexData[i + j] == 70 || hexData[i + j] == 102;
																				if (flag17)
																				{
																					array[j] = 15;
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				memoryStream.WriteByte((byte)((int)array[0] << 4 | (int)array[1]));
			}
			return memoryStream.ToArray();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005840 File Offset: 0x00004840
		[Obsolete("Use Net_Utils.ComputeMd5 instead of it")]
		public static string ComputeMd5(string text, bool hex)
		{
			string result;
			using (MD5 md = new MD5CryptoServiceProvider())
			{
				byte[] bytes = md.ComputeHash(Encoding.Default.GetBytes(text));
				if (hex)
				{
					result = Core.ToHexString(Encoding.Default.GetString(bytes)).ToLower();
				}
				else
				{
					result = Encoding.Default.GetString(bytes);
				}
			}
			return result;
		}
	}
}
