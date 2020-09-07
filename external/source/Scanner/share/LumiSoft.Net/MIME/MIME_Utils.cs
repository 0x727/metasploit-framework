using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LumiSoft.Net.MIME
{
	// Token: 0x0200011A RID: 282
	public class MIME_Utils
	{
		// Token: 0x06000B1A RID: 2842 RVA: 0x00043A48 File Offset: 0x00042A48
		public static string DateTimeToRfc2822(DateTime dateTime)
		{
			return dateTime.ToString("ddd, dd MMM yyyy HH':'mm':'ss ", DateTimeFormatInfo.InvariantInfo) + dateTime.ToString("zzz").Replace(":", "");
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x00043A8C File Offset: 0x00042A8C
		public static DateTime ParseRfc2822DateTime(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException(value);
			}
			DateTime result;
			try
			{
				MIME_Reader mime_Reader = new MIME_Reader(value);
				string text = mime_Reader.Atom();
				bool flag2 = text.Length == 3;
				if (flag2)
				{
					mime_Reader.Char(true);
					text = mime_Reader.Atom();
				}
				int day = Convert.ToInt32(text);
				text = mime_Reader.Atom().ToLower();
				bool flag3 = text == "jan";
				int month;
				if (flag3)
				{
					month = 1;
				}
				else
				{
					bool flag4 = text == "feb";
					if (flag4)
					{
						month = 2;
					}
					else
					{
						bool flag5 = text == "mar";
						if (flag5)
						{
							month = 3;
						}
						else
						{
							bool flag6 = text == "apr";
							if (flag6)
							{
								month = 4;
							}
							else
							{
								bool flag7 = text == "may";
								if (flag7)
								{
									month = 5;
								}
								else
								{
									bool flag8 = text == "jun";
									if (flag8)
									{
										month = 6;
									}
									else
									{
										bool flag9 = text == "jul";
										if (flag9)
										{
											month = 7;
										}
										else
										{
											bool flag10 = text == "aug";
											if (flag10)
											{
												month = 8;
											}
											else
											{
												bool flag11 = text == "sep";
												if (flag11)
												{
													month = 9;
												}
												else
												{
													bool flag12 = text == "oct";
													if (flag12)
													{
														month = 10;
													}
													else
													{
														bool flag13 = text == "nov";
														if (flag13)
														{
															month = 11;
														}
														else
														{
															bool flag14 = text == "dec";
															if (!flag14)
															{
																throw new ArgumentException("Invalid month-name value '" + value + "'.");
															}
															month = 12;
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
				int year = Convert.ToInt32(mime_Reader.Atom());
				int hour = Convert.ToInt32(mime_Reader.Atom());
				mime_Reader.Char(true);
				int minute = Convert.ToInt32(mime_Reader.Atom());
				int second = 0;
				bool flag15 = mime_Reader.Peek(true) == 58;
				if (flag15)
				{
					mime_Reader.Char(true);
					second = Convert.ToInt32(mime_Reader.Atom());
				}
				int num = 0;
				text = mime_Reader.Atom();
				bool flag16 = text == null;
				if (!flag16)
				{
					bool flag17 = text[0] == '+' || text[0] == '-';
					if (flag17)
					{
						bool flag18 = text[0] == '+';
						if (flag18)
						{
							num = Convert.ToInt32(text.Substring(1, 2)) * 60 + Convert.ToInt32(text.Substring(3, 2));
						}
						else
						{
							num = -(Convert.ToInt32(text.Substring(1, 2)) * 60 + Convert.ToInt32(text.Substring(3, 2)));
						}
					}
					else
					{
						text = text.ToUpper();
						bool flag19 = text == "A";
						if (flag19)
						{
							num = 60;
						}
						else
						{
							bool flag20 = text == "ACDT";
							if (flag20)
							{
								num = 630;
							}
							else
							{
								bool flag21 = text == "ACST";
								if (flag21)
								{
									num = 570;
								}
								else
								{
									bool flag22 = text == "ADT";
									if (flag22)
									{
										num = -180;
									}
									else
									{
										bool flag23 = text == "AEDT";
										if (flag23)
										{
											num = 660;
										}
										else
										{
											bool flag24 = text == "AEST";
											if (flag24)
											{
												num = 600;
											}
											else
											{
												bool flag25 = text == "AKDT";
												if (flag25)
												{
													num = -480;
												}
												else
												{
													bool flag26 = text == "AKST";
													if (flag26)
													{
														num = -540;
													}
													else
													{
														bool flag27 = text == "AST";
														if (flag27)
														{
															num = -240;
														}
														else
														{
															bool flag28 = text == "AWDT";
															if (flag28)
															{
																num = 540;
															}
															else
															{
																bool flag29 = text == "AWST";
																if (flag29)
																{
																	num = 480;
																}
																else
																{
																	bool flag30 = text == "B";
																	if (flag30)
																	{
																		num = 120;
																	}
																	else
																	{
																		bool flag31 = text == "BST";
																		if (flag31)
																		{
																			num = 60;
																		}
																		else
																		{
																			bool flag32 = text == "C";
																			if (flag32)
																			{
																				num = 180;
																			}
																			else
																			{
																				bool flag33 = text == "CDT";
																				if (flag33)
																				{
																					num = -300;
																				}
																				else
																				{
																					bool flag34 = text == "CEDT";
																					if (flag34)
																					{
																						num = 120;
																					}
																					else
																					{
																						bool flag35 = text == "CEST";
																						if (flag35)
																						{
																							num = 120;
																						}
																						else
																						{
																							bool flag36 = text == "CET";
																							if (flag36)
																							{
																								num = 60;
																							}
																							else
																							{
																								bool flag37 = text == "CST";
																								if (flag37)
																								{
																									num = -360;
																								}
																								else
																								{
																									bool flag38 = text == "CXT";
																									if (flag38)
																									{
																										num = 60;
																									}
																									else
																									{
																										bool flag39 = text == "D";
																										if (flag39)
																										{
																											num = 240;
																										}
																										else
																										{
																											bool flag40 = text == "E";
																											if (flag40)
																											{
																												num = 300;
																											}
																											else
																											{
																												bool flag41 = text == "EDT";
																												if (flag41)
																												{
																													num = -240;
																												}
																												else
																												{
																													bool flag42 = text == "EEDT";
																													if (flag42)
																													{
																														num = 180;
																													}
																													else
																													{
																														bool flag43 = text == "EEST";
																														if (flag43)
																														{
																															num = 180;
																														}
																														else
																														{
																															bool flag44 = text == "EET";
																															if (flag44)
																															{
																																num = 120;
																															}
																															else
																															{
																																bool flag45 = text == "EST";
																																if (flag45)
																																{
																																	num = -300;
																																}
																																else
																																{
																																	bool flag46 = text == "F";
																																	if (flag46)
																																	{
																																		num = 360;
																																	}
																																	else
																																	{
																																		bool flag47 = text == "G";
																																		if (flag47)
																																		{
																																			num = 420;
																																		}
																																		else
																																		{
																																			bool flag48 = text == "GMT";
																																			if (flag48)
																																			{
																																				num = 0;
																																			}
																																			else
																																			{
																																				bool flag49 = text == "H";
																																				if (flag49)
																																				{
																																					num = 480;
																																				}
																																				else
																																				{
																																					bool flag50 = text == "I";
																																					if (flag50)
																																					{
																																						num = 540;
																																					}
																																					else
																																					{
																																						bool flag51 = text == "IST";
																																						if (flag51)
																																						{
																																							num = 60;
																																						}
																																						else
																																						{
																																							bool flag52 = text == "K";
																																							if (flag52)
																																							{
																																								num = 600;
																																							}
																																							else
																																							{
																																								bool flag53 = text == "L";
																																								if (flag53)
																																								{
																																									num = 660;
																																								}
																																								else
																																								{
																																									bool flag54 = text == "M";
																																									if (flag54)
																																									{
																																										num = 720;
																																									}
																																									else
																																									{
																																										bool flag55 = text == "MDT";
																																										if (flag55)
																																										{
																																											num = -360;
																																										}
																																										else
																																										{
																																											bool flag56 = text == "MST";
																																											if (flag56)
																																											{
																																												num = -420;
																																											}
																																											else
																																											{
																																												bool flag57 = text == "N";
																																												if (flag57)
																																												{
																																													num = -60;
																																												}
																																												else
																																												{
																																													bool flag58 = text == "NDT";
																																													if (flag58)
																																													{
																																														num = -150;
																																													}
																																													else
																																													{
																																														bool flag59 = text == "NFT";
																																														if (flag59)
																																														{
																																															num = 690;
																																														}
																																														else
																																														{
																																															bool flag60 = text == "NST";
																																															if (flag60)
																																															{
																																																num = -210;
																																															}
																																															else
																																															{
																																																bool flag61 = text == "O";
																																																if (flag61)
																																																{
																																																	num = -120;
																																																}
																																																else
																																																{
																																																	bool flag62 = text == "P";
																																																	if (flag62)
																																																	{
																																																		num = -180;
																																																	}
																																																	else
																																																	{
																																																		bool flag63 = text == "PDT";
																																																		if (flag63)
																																																		{
																																																			num = -420;
																																																		}
																																																		else
																																																		{
																																																			bool flag64 = text == "PST";
																																																			if (flag64)
																																																			{
																																																				num = -480;
																																																			}
																																																			else
																																																			{
																																																				bool flag65 = text == "Q";
																																																				if (flag65)
																																																				{
																																																					num = -240;
																																																				}
																																																				else
																																																				{
																																																					bool flag66 = text == "R";
																																																					if (flag66)
																																																					{
																																																						num = -300;
																																																					}
																																																					else
																																																					{
																																																						bool flag67 = text == "S";
																																																						if (flag67)
																																																						{
																																																							num = -360;
																																																						}
																																																						else
																																																						{
																																																							bool flag68 = text == "T";
																																																							if (flag68)
																																																							{
																																																								num = -420;
																																																							}
																																																							else
																																																							{
																																																								bool flag69 = text == "";
																																																								if (flag69)
																																																								{
																																																									num = -480;
																																																								}
																																																								else
																																																								{
																																																									bool flag70 = text == "UTC";
																																																									if (flag70)
																																																									{
																																																										num = 0;
																																																									}
																																																									else
																																																									{
																																																										bool flag71 = text == "V";
																																																										if (flag71)
																																																										{
																																																											num = -540;
																																																										}
																																																										else
																																																										{
																																																											bool flag72 = text == "W";
																																																											if (flag72)
																																																											{
																																																												num = -600;
																																																											}
																																																											else
																																																											{
																																																												bool flag73 = text == "WEDT";
																																																												if (flag73)
																																																												{
																																																													num = 60;
																																																												}
																																																												else
																																																												{
																																																													bool flag74 = text == "WEST";
																																																													if (flag74)
																																																													{
																																																														num = 60;
																																																													}
																																																													else
																																																													{
																																																														bool flag75 = text == "WET";
																																																														if (flag75)
																																																														{
																																																															num = 0;
																																																														}
																																																														else
																																																														{
																																																															bool flag76 = text == "WST";
																																																															if (flag76)
																																																															{
																																																																num = 480;
																																																															}
																																																															else
																																																															{
																																																																bool flag77 = text == "X";
																																																																if (flag77)
																																																																{
																																																																	num = -660;
																																																																}
																																																																else
																																																																{
																																																																	bool flag78 = text == "Y";
																																																																	if (flag78)
																																																																	{
																																																																		num = -720;
																																																																	}
																																																																	else
																																																																	{
																																																																		bool flag79 = text == "Z";
																																																																		if (flag79)
																																																																		{
																																																																			num = 0;
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
				DateTime dateTime = new DateTime(year, month, day, hour, minute, second).AddMinutes((double)(-(double)num));
				result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc).ToLocalTime();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				throw new ArgumentException("Argumnet 'value' value '" + value + "' is not valid RFC 822/2822 date-time string.");
			}
			return result;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x000444F4 File Offset: 0x000434F4
		public static string UnfoldHeader(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			return value.Replace("\r\n", "");
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x0004452C File Offset: 0x0004352C
		public static string CreateMessageID()
		{
			return string.Concat(new string[]
			{
				"<",
				Guid.NewGuid().ToString().Replace("-", "").Substring(16),
				"@",
				Guid.NewGuid().ToString().Replace("-", "").Substring(16),
				">"
			});
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000445BC File Offset: 0x000435BC
		internal static string ParseHeaders(Stream entryStrm)
		{
			byte[] array = new byte[]
			{
				13,
				10
			};
			MemoryStream memoryStream = new MemoryStream();
			StreamLineReader streamLineReader = new StreamLineReader(entryStrm);
			for (byte[] array2 = streamLineReader.ReadLine(); array2 != null; array2 = streamLineReader.ReadLine())
			{
				bool flag = array2.Length == 0;
				if (flag)
				{
					break;
				}
				memoryStream.Write(array2, 0, array2.Length);
				memoryStream.Write(array, 0, array.Length);
			}
			return Encoding.Default.GetString(memoryStream.ToArray());
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00044640 File Offset: 0x00043640
		public static string ParseHeaderField(string fieldName, Stream entryStrm)
		{
			return MIME_Utils.ParseHeaderField(fieldName, MIME_Utils.ParseHeaders(entryStrm));
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x00044660 File Offset: 0x00043660
		public static string ParseHeaderField(string fieldName, string headers)
		{
			using (TextReader textReader = new StreamReader(new MemoryStream(Encoding.Default.GetBytes(headers))))
			{
				for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
				{
					bool flag = text.ToUpper().StartsWith(fieldName.ToUpper());
					if (flag)
					{
						string text2 = text.Substring(fieldName.Length).Trim();
						text = textReader.ReadLine();
						while (text != null && (text.StartsWith("\t") || text.StartsWith(" ")))
						{
							text2 += text;
							text = textReader.ReadLine();
						}
						return text2;
					}
				}
			}
			return "";
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0004473C File Offset: 0x0004373C
		public static string QDecode(Encoding encoding, string data)
		{
			bool flag = encoding == null;
			if (flag)
			{
				throw new ArgumentNullException("encoding");
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			return encoding.GetString(MIME_Utils.QuotedPrintableDecode(Encoding.ASCII.GetBytes(data.Replace("_", " "))));
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0004479C File Offset: 0x0004379C
		public static byte[] QuotedPrintableDecode(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			MemoryStream memoryStream = new MemoryStream();
			MemoryStream memoryStream2 = new MemoryStream(data);
			for (int i = memoryStream2.ReadByte(); i > -1; i = memoryStream2.ReadByte())
			{
				bool flag2 = i == 61;
				if (flag2)
				{
					byte[] array = new byte[2];
					int num = memoryStream2.Read(array, 0, 2);
					bool flag3 = num == 2;
					if (flag3)
					{
						bool flag4 = array[0] == 13 && array[1] == 10;
						if (!flag4)
						{
							try
							{
								memoryStream.Write(Net_Utils.FromHex(array), 0, 1);
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
	}
}
