using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using LumiSoft.Net.IO;

namespace LumiSoft.Net
{
	// Token: 0x02000012 RID: 18
	public class Net_Utils
	{
		// Token: 0x06000047 RID: 71 RVA: 0x000031D4 File Offset: 0x000021D4
		public static string GetLocalHostName(string hostName)
		{
			bool flag = string.IsNullOrEmpty(hostName);
			string result;
			if (flag)
			{
				result = Dns.GetHostName();
			}
			else
			{
				result = hostName;
			}
			return result;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000031FC File Offset: 0x000021FC
		public static bool CompareArray(Array array1, Array array2)
		{
			return Net_Utils.CompareArray(array1, array2, array2.Length);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000321C File Offset: 0x0000221C
		public static bool CompareArray(Array array1, Array array2, int array2Count)
		{
			bool flag = array1 == null && array2 == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = array1 == null && array2 != null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = array1 != null && array2 == null;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = array1.Length != array2Count;
						if (flag4)
						{
							result = false;
						}
						else
						{
							for (int i = 0; i < array1.Length; i++)
							{
								bool flag5 = !array1.GetValue(i).Equals(array2.GetValue(i));
								if (flag5)
								{
									return false;
								}
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000032C4 File Offset: 0x000022C4
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

		// Token: 0x0600004B RID: 75 RVA: 0x000032F4 File Offset: 0x000022F4
		public static string ArrayToString(string[] values, string delimiter)
		{
			bool flag = values == null;
			string result;
			if (flag)
			{
				result = "";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < values.Length; i++)
				{
					bool flag2 = i > 0;
					if (flag2)
					{
						stringBuilder.Append(delimiter);
					}
					stringBuilder.Append(values[i]);
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003358 File Offset: 0x00002358
		public static long StreamCopy(Stream source, Stream target, int blockSize)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = target == null;
			if (flag2)
			{
				throw new ArgumentNullException("target");
			}
			bool flag3 = blockSize < 1024;
			if (flag3)
			{
				throw new ArgumentException("Argument 'blockSize' value must be >= 1024.");
			}
			byte[] array = new byte[blockSize];
			long num = 0L;
			for (;;)
			{
				int num2 = source.Read(array, 0, array.Length);
				bool flag4 = num2 == 0;
				if (flag4)
				{
					break;
				}
				target.Write(array, 0, num2);
				num += (long)num2;
			}
			return num;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000033F0 File Offset: 0x000023F0
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

		// Token: 0x0600004E RID: 78 RVA: 0x0000347C File Offset: 0x0000247C
		public static bool IsIPAddress(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			IPAddress ipaddress = null;
			return IPAddress.TryParse(value, out ipaddress);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000034AC File Offset: 0x000024AC
		public static bool IsMulticastAddress(IPAddress ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			bool isIPv6Multicast = ip.IsIPv6Multicast;
			bool result;
			if (isIPv6Multicast)
			{
				result = true;
			}
			else
			{
				bool flag2 = ip.AddressFamily == AddressFamily.InterNetwork;
				if (flag2)
				{
					byte[] addressBytes = ip.GetAddressBytes();
					bool flag3 = addressBytes[0] >= 224 && addressBytes[0] <= 239;
					if (flag3)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003520 File Offset: 0x00002520
		public static bool IsPrivateIP(string ip)
		{
			bool flag = ip == null;
			if (flag)
			{
				throw new ArgumentNullException("ip");
			}
			return Net_Utils.IsPrivateIP(IPAddress.Parse(ip));
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003554 File Offset: 0x00002554
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

		// Token: 0x06000052 RID: 82 RVA: 0x00003618 File Offset: 0x00002618
		public static IPEndPoint ParseIPEndPoint(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			IPEndPoint result;
			try
			{
				string[] array = value.Split(new char[]
				{
					':'
				});
				result = new IPEndPoint(IPAddress.Parse(array[0]), Convert.ToInt32(array[1]));
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Invalid IPEndPoint value.", "value", innerException);
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000368C File Offset: 0x0000268C
		public static bool IsInteger(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			long num = 0L;
			return long.TryParse(value, out num);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000036C0 File Offset: 0x000026C0
		public static bool IsAscii(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			foreach (char c in value)
			{
				bool flag2 = c > '\u007f';
				if (flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003718 File Offset: 0x00002718
		public static bool IsSocketAsyncSupported()
		{
			bool result;
			try
			{
				using (new SocketAsyncEventArgs())
				{
					result = true;
				}
			}
			catch (NotSupportedException ex)
			{
				string message = ex.Message;
				result = false;
			}
			return result;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003768 File Offset: 0x00002768
		public static Socket CreateSocket(IPEndPoint localEP, ProtocolType protocolType)
		{
			bool flag = localEP == null;
			if (flag)
			{
				throw new ArgumentNullException("localEP");
			}
			SocketType socketType = SocketType.Stream;
			bool flag2 = protocolType == ProtocolType.Udp;
			if (flag2)
			{
				socketType = SocketType.Dgram;
			}
			bool flag3 = localEP.AddressFamily == AddressFamily.InterNetwork;
			Socket result;
			if (flag3)
			{
				Socket socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
				socket.Bind(localEP);
				result = socket;
			}
			else
			{
				bool flag4 = localEP.AddressFamily == AddressFamily.InterNetworkV6;
				if (!flag4)
				{
					throw new ArgumentException("Invalid IPEndPoint address family.");
				}
				Socket socket2 = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
				socket2.Bind(localEP);
				result = socket2;
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000037FC File Offset: 0x000027FC
		public static string ToHex(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			return BitConverter.ToString(data).ToLower().Replace("-", "");
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000383C File Offset: 0x0000283C
		public static string ToHex(string text)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			return BitConverter.ToString(Encoding.Default.GetBytes(text)).ToLower().Replace("-", "");
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003888 File Offset: 0x00002888
		public static byte[] FromHex(byte[] hexData)
		{
			bool flag = hexData == null;
			if (flag)
			{
				throw new ArgumentNullException("hexData");
			}
			bool flag2 = hexData.Length < 2 || (double)hexData.Length / 2.0 != Math.Floor((double)hexData.Length / 2.0);
			if (flag2)
			{
				throw new Exception("Illegal hex data, hex data must be in two bytes pairs, for example: 0F,FF,A3,... .");
			}
			MemoryStream memoryStream = new MemoryStream(hexData.Length / 2);
			for (int i = 0; i < hexData.Length; i += 2)
			{
				byte[] array = new byte[2];
				for (int j = 0; j < 2; j++)
				{
					bool flag3 = hexData[i + j] == 48;
					if (flag3)
					{
						array[j] = 0;
					}
					else
					{
						bool flag4 = hexData[i + j] == 49;
						if (flag4)
						{
							array[j] = 1;
						}
						else
						{
							bool flag5 = hexData[i + j] == 50;
							if (flag5)
							{
								array[j] = 2;
							}
							else
							{
								bool flag6 = hexData[i + j] == 51;
								if (flag6)
								{
									array[j] = 3;
								}
								else
								{
									bool flag7 = hexData[i + j] == 52;
									if (flag7)
									{
										array[j] = 4;
									}
									else
									{
										bool flag8 = hexData[i + j] == 53;
										if (flag8)
										{
											array[j] = 5;
										}
										else
										{
											bool flag9 = hexData[i + j] == 54;
											if (flag9)
											{
												array[j] = 6;
											}
											else
											{
												bool flag10 = hexData[i + j] == 55;
												if (flag10)
												{
													array[j] = 7;
												}
												else
												{
													bool flag11 = hexData[i + j] == 56;
													if (flag11)
													{
														array[j] = 8;
													}
													else
													{
														bool flag12 = hexData[i + j] == 57;
														if (flag12)
														{
															array[j] = 9;
														}
														else
														{
															bool flag13 = hexData[i + j] == 65 || hexData[i + j] == 97;
															if (flag13)
															{
																array[j] = 10;
															}
															else
															{
																bool flag14 = hexData[i + j] == 66 || hexData[i + j] == 98;
																if (flag14)
																{
																	array[j] = 11;
																}
																else
																{
																	bool flag15 = hexData[i + j] == 67 || hexData[i + j] == 99;
																	if (flag15)
																	{
																		array[j] = 12;
																	}
																	else
																	{
																		bool flag16 = hexData[i + j] == 68 || hexData[i + j] == 100;
																		if (flag16)
																		{
																			array[j] = 13;
																		}
																		else
																		{
																			bool flag17 = hexData[i + j] == 69 || hexData[i + j] == 101;
																			if (flag17)
																			{
																				array[j] = 14;
																			}
																			else
																			{
																				bool flag18 = hexData[i + j] == 70 || hexData[i + j] == 102;
																				if (flag18)
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

		// Token: 0x0600005A RID: 90 RVA: 0x00003B6C File Offset: 0x00002B6C
		public static byte[] FromBase64(string data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Base64 @base = new Base64();
			return @base.Decode(data, true);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003BA0 File Offset: 0x00002BA0
		public static byte[] FromBase64(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			Base64 @base = new Base64();
			return @base.Decode(data, 0, data.Length, true);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003BD8 File Offset: 0x00002BD8
		public static byte[] Base64Encode(byte[] data)
		{
			return Net_Utils.Base64EncodeEx(data, null, true);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003BF4 File Offset: 0x00002BF4
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

		// Token: 0x0600005E RID: 94 RVA: 0x00003E74 File Offset: 0x00002E74
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

		// Token: 0x0600005F RID: 95 RVA: 0x000040A0 File Offset: 0x000030A0
		public static string ComputeMd5(string text, bool hex)
		{
			bool flag = text == null;
			if (flag)
			{
				throw new ArgumentNullException("text");
			}
			string result;
			using (MD5 md = new MD5CryptoServiceProvider())
			{
				byte[] array = md.ComputeHash(Encoding.Default.GetBytes(text));
				if (hex)
				{
					result = Net_Utils.ToHex(array).ToLower();
				}
				else
				{
					result = Encoding.Default.GetString(array);
				}
			}
			return result;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004120 File Offset: 0x00003120
		[Obsolete("Use method 'IsSocketAsyncSupported' instead.")]
		public static bool IsIoCompletionPortsSupported()
		{
			bool result;
			try
			{
				using (new SocketAsyncEventArgs())
				{
					result = true;
				}
			}
			catch (NotSupportedException ex)
			{
				string message = ex.Message;
				result = false;
			}
			return result;
		}
	}
}
