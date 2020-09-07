using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000168 RID: 360
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MimeUtils
	{
		// Token: 0x06000EAB RID: 3755 RVA: 0x0005AE18 File Offset: 0x00059E18
		public static DateTime ParseDate(string date)
		{
			date = date.ToLower();
			date = date.Replace("ut", "-0000");
			date = date.Replace("gmt", "-0000");
			date = date.Replace("edt", "-0400");
			date = date.Replace("est", "-0500");
			date = date.Replace("cdt", "-0500");
			date = date.Replace("cst", "-0600");
			date = date.Replace("mdt", "-0600");
			date = date.Replace("mst", "-0700");
			date = date.Replace("pdt", "-0700");
			date = date.Replace("pst", "-0800");
			date = date.Replace("bst", "+0100");
			date = date.Replace("jan", "01");
			date = date.Replace("feb", "02");
			date = date.Replace("mar", "03");
			date = date.Replace("apr", "04");
			date = date.Replace("may", "05");
			date = date.Replace("jun", "06");
			date = date.Replace("jul", "07");
			date = date.Replace("aug", "08");
			date = date.Replace("sep", "09");
			date = date.Replace("oct", "10");
			date = date.Replace("nov", "11");
			date = date.Replace("dec", "12");
			bool flag = date.IndexOf(',') > -1;
			if (flag)
			{
				date = date.Substring(date.IndexOf(',') + 1);
			}
			bool flag2 = date.IndexOf(" (") > -1;
			if (flag2)
			{
				date = date.Substring(0, date.IndexOf(" ("));
			}
			int year = 1900;
			int month = 1;
			int day = 1;
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			StringReader stringReader = new StringReader(date);
			try
			{
				day = Convert.ToInt32(stringReader.ReadWord(true, new char[]
				{
					'.',
					'-',
					' '
				}, true));
			}
			catch
			{
				throw new Exception("Invalid date value '" + date + "', invalid day value !");
			}
			try
			{
				month = Convert.ToInt32(stringReader.ReadWord(true, new char[]
				{
					'.',
					'-',
					' '
				}, true));
			}
			catch
			{
				throw new Exception("Invalid date value '" + date + "', invalid month value !");
			}
			try
			{
				year = Convert.ToInt32(stringReader.ReadWord(true, new char[]
				{
					'.',
					'-',
					' '
				}, true));
			}
			catch
			{
				throw new Exception("Invalid date value '" + date + "', invalid year value !");
			}
			bool flag3 = stringReader.Available > 0L;
			if (flag3)
			{
				try
				{
					num = Convert.ToInt32(stringReader.ReadWord(true, new char[]
					{
						':'
					}, true));
				}
				catch
				{
					throw new Exception("Invalid date value '" + date + "', invalid hour value !");
				}
				try
				{
					num2 = Convert.ToInt32(stringReader.ReadWord(true, new char[]
					{
						':'
					}, false));
				}
				catch
				{
					throw new Exception("Invalid date value '" + date + "', invalid minute value !");
				}
				stringReader.ReadToFirstChar();
				bool flag4 = stringReader.StartsWith(":");
				if (flag4)
				{
					stringReader.ReadSpecifiedLength(1);
					try
					{
						string text = stringReader.ReadWord(true, new char[]
						{
							' '
						}, true);
						bool flag5 = text.IndexOf('.') > -1;
						if (flag5)
						{
							text = text.Substring(0, text.IndexOf('.'));
						}
						num3 = Convert.ToInt32(text);
					}
					catch
					{
						throw new Exception("Invalid date value '" + date + "', invalid second value !");
					}
				}
				stringReader.ReadToFirstChar();
				bool flag6 = stringReader.Available > 3L;
				if (flag6)
				{
					string text2 = stringReader.SourceString.Replace(":", "");
					bool flag7 = text2.StartsWith("+") || text2.StartsWith("-");
					if (flag7)
					{
						bool flag8 = text2.StartsWith("+");
						text2 = text2.Substring(1);
						while (text2.Length < 4)
						{
							text2 = "0" + text2;
						}
						try
						{
							int num5 = Convert.ToInt32(text2.Substring(0, 2));
							int num6 = Convert.ToInt32(text2.Substring(2));
							bool flag9 = flag8;
							if (flag9)
							{
								num4 = 0 - (num5 * 60 + num6);
							}
							else
							{
								num4 = num5 * 60 + num6;
							}
						}
						catch
						{
						}
					}
				}
			}
			bool flag10 = num != -1 && num2 != -1 && num3 != -1;
			DateTime result;
			if (flag10)
			{
				DateTime dateTime = new DateTime(year, month, day, num, num2, num3).AddMinutes((double)num4);
				result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, DateTimeKind.Utc).ToLocalTime();
			}
			else
			{
				result = new DateTime(year, month, day);
			}
			return result;
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x0005B3B8 File Offset: 0x0005A3B8
		public static string DateTimeToRfc2822(DateTime dateTime)
		{
			return dateTime.ToUniversalTime().ToString("r", DateTimeFormatInfo.InvariantInfo);
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0005B3E4 File Offset: 0x0005A3E4
		public static string ParseHeaders(Stream entryStrm)
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

		// Token: 0x06000EAE RID: 3758 RVA: 0x0005B468 File Offset: 0x0005A468
		public static string ParseHeaderField(string fieldName, Stream entryStrm)
		{
			return MimeUtils.ParseHeaderField(fieldName, MimeUtils.ParseHeaders(entryStrm));
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0005B488 File Offset: 0x0005A488
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

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0005B564 File Offset: 0x0005A564
		public static string ParseHeaderFiledParameter(string fieldName, string parameterName, string headers)
		{
			string text = MimeUtils.ParseHeaderField(fieldName, headers);
			bool flag = text.Length > 0;
			if (flag)
			{
				int num = text.ToUpper().IndexOf(parameterName.ToUpper());
				bool flag2 = num > -1;
				if (flag2)
				{
					text = text.Substring(num + parameterName.Length + 1);
					bool flag3 = text.StartsWith("\"");
					if (flag3)
					{
						return text.Substring(1, text.IndexOf("\"", 1) - 1);
					}
					int length = text.Length;
					bool flag4 = text.IndexOf(" ") > -1;
					if (flag4)
					{
						length = text.IndexOf(" ");
					}
					return text.Substring(0, length);
				}
			}
			return "";
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0005B628 File Offset: 0x0005A628
		public static MediaType_enum ParseMediaType(string headerFieldValue)
		{
			bool flag = headerFieldValue == null;
			MediaType_enum result;
			if (flag)
			{
				result = MediaType_enum.NotSpecified;
			}
			else
			{
				string text = TextUtils.SplitString(headerFieldValue, ';')[0].ToLower();
				bool flag2 = text.IndexOf("text/plain") > -1;
				if (flag2)
				{
					result = MediaType_enum.Text_plain;
				}
				else
				{
					bool flag3 = text.IndexOf("text/html") > -1;
					if (flag3)
					{
						result = MediaType_enum.Text_html;
					}
					else
					{
						bool flag4 = text.IndexOf("text/xml") > -1;
						if (flag4)
						{
							result = MediaType_enum.Text_xml;
						}
						else
						{
							bool flag5 = text.IndexOf("text/rtf") > -1;
							if (flag5)
							{
								result = MediaType_enum.Text_rtf;
							}
							else
							{
								bool flag6 = text.IndexOf("text") > -1;
								if (flag6)
								{
									result = MediaType_enum.Text;
								}
								else
								{
									bool flag7 = text.IndexOf("image/gif") > -1;
									if (flag7)
									{
										result = MediaType_enum.Image_gif;
									}
									else
									{
										bool flag8 = text.IndexOf("image/tiff") > -1;
										if (flag8)
										{
											result = MediaType_enum.Image_tiff;
										}
										else
										{
											bool flag9 = text.IndexOf("image/jpeg") > -1;
											if (flag9)
											{
												result = MediaType_enum.Image_jpeg;
											}
											else
											{
												bool flag10 = text.IndexOf("image") > -1;
												if (flag10)
												{
													result = MediaType_enum.Image;
												}
												else
												{
													bool flag11 = text.IndexOf("audio") > -1;
													if (flag11)
													{
														result = MediaType_enum.Audio;
													}
													else
													{
														bool flag12 = text.IndexOf("video") > -1;
														if (flag12)
														{
															result = MediaType_enum.Video;
														}
														else
														{
															bool flag13 = text.IndexOf("application/octet-stream") > -1;
															if (flag13)
															{
																result = MediaType_enum.Application_octet_stream;
															}
															else
															{
																bool flag14 = text.IndexOf("application") > -1;
																if (flag14)
																{
																	result = MediaType_enum.Application;
																}
																else
																{
																	bool flag15 = text.IndexOf("multipart/mixed") > -1;
																	if (flag15)
																	{
																		result = MediaType_enum.Multipart_mixed;
																	}
																	else
																	{
																		bool flag16 = text.IndexOf("multipart/alternative") > -1;
																		if (flag16)
																		{
																			result = MediaType_enum.Multipart_alternative;
																		}
																		else
																		{
																			bool flag17 = text.IndexOf("multipart/parallel") > -1;
																			if (flag17)
																			{
																				result = MediaType_enum.Multipart_parallel;
																			}
																			else
																			{
																				bool flag18 = text.IndexOf("multipart/related") > -1;
																				if (flag18)
																				{
																					result = MediaType_enum.Multipart_related;
																				}
																				else
																				{
																					bool flag19 = text.IndexOf("multipart/signed") > -1;
																					if (flag19)
																					{
																						result = MediaType_enum.Multipart_signed;
																					}
																					else
																					{
																						bool flag20 = text.IndexOf("multipart") > -1;
																						if (flag20)
																						{
																							result = MediaType_enum.Multipart;
																						}
																						else
																						{
																							bool flag21 = text.IndexOf("message/rfc822") > -1;
																							if (flag21)
																							{
																								result = MediaType_enum.Message_rfc822;
																							}
																							else
																							{
																								bool flag22 = text.IndexOf("message") > -1;
																								if (flag22)
																								{
																									result = MediaType_enum.Message;
																								}
																								else
																								{
																									result = MediaType_enum.Unknown;
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
			return result;
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0005B8DC File Offset: 0x0005A8DC
		public static string MediaTypeToString(MediaType_enum mediaType)
		{
			bool flag = mediaType == MediaType_enum.Text_plain;
			string result;
			if (flag)
			{
				result = "text/plain";
			}
			else
			{
				bool flag2 = mediaType == MediaType_enum.Text_html;
				if (flag2)
				{
					result = "text/html";
				}
				else
				{
					bool flag3 = mediaType == MediaType_enum.Text_xml;
					if (flag3)
					{
						result = "text/xml";
					}
					else
					{
						bool flag4 = mediaType == MediaType_enum.Text_rtf;
						if (flag4)
						{
							result = "text/rtf";
						}
						else
						{
							bool flag5 = mediaType == MediaType_enum.Text;
							if (flag5)
							{
								result = "text";
							}
							else
							{
								bool flag6 = mediaType == MediaType_enum.Image_gif;
								if (flag6)
								{
									result = "image/gif";
								}
								else
								{
									bool flag7 = mediaType == MediaType_enum.Image_tiff;
									if (flag7)
									{
										result = "image/tiff";
									}
									else
									{
										bool flag8 = mediaType == MediaType_enum.Image_jpeg;
										if (flag8)
										{
											result = "image/jpeg";
										}
										else
										{
											bool flag9 = mediaType == MediaType_enum.Image;
											if (flag9)
											{
												result = "image";
											}
											else
											{
												bool flag10 = mediaType == MediaType_enum.Audio;
												if (flag10)
												{
													result = "audio";
												}
												else
												{
													bool flag11 = mediaType == MediaType_enum.Video;
													if (flag11)
													{
														result = "video";
													}
													else
													{
														bool flag12 = mediaType == MediaType_enum.Application_octet_stream;
														if (flag12)
														{
															result = "application/octet-stream";
														}
														else
														{
															bool flag13 = mediaType == MediaType_enum.Application;
															if (flag13)
															{
																result = "application";
															}
															else
															{
																bool flag14 = mediaType == MediaType_enum.Multipart_mixed;
																if (flag14)
																{
																	result = "multipart/mixed";
																}
																else
																{
																	bool flag15 = mediaType == MediaType_enum.Multipart_alternative;
																	if (flag15)
																	{
																		result = "multipart/alternative";
																	}
																	else
																	{
																		bool flag16 = mediaType == MediaType_enum.Multipart_parallel;
																		if (flag16)
																		{
																			result = "multipart/parallel";
																		}
																		else
																		{
																			bool flag17 = mediaType == MediaType_enum.Multipart_related;
																			if (flag17)
																			{
																				result = "multipart/related";
																			}
																			else
																			{
																				bool flag18 = mediaType == MediaType_enum.Multipart_signed;
																				if (flag18)
																				{
																					result = "multipart/signed";
																				}
																				else
																				{
																					bool flag19 = mediaType == MediaType_enum.Multipart;
																					if (flag19)
																					{
																						result = "multipart";
																					}
																					else
																					{
																						bool flag20 = mediaType == MediaType_enum.Message_rfc822;
																						if (flag20)
																						{
																							result = "message/rfc822";
																						}
																						else
																						{
																							bool flag21 = mediaType == MediaType_enum.Message;
																							if (flag21)
																							{
																								result = "message";
																							}
																							else
																							{
																								bool flag22 = mediaType == MediaType_enum.Unknown;
																								if (flag22)
																								{
																									result = "unknown";
																								}
																								else
																								{
																									result = null;
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
			return result;
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0005BAFC File Offset: 0x0005AAFC
		public static ContentTransferEncoding_enum ParseContentTransferEncoding(string headerFieldValue)
		{
			bool flag = headerFieldValue == null;
			ContentTransferEncoding_enum result;
			if (flag)
			{
				result = ContentTransferEncoding_enum.NotSpecified;
			}
			else
			{
				string a = headerFieldValue.ToLower();
				bool flag2 = a == "7bit";
				if (flag2)
				{
					result = ContentTransferEncoding_enum._7bit;
				}
				else
				{
					bool flag3 = a == "quoted-printable";
					if (flag3)
					{
						result = ContentTransferEncoding_enum.QuotedPrintable;
					}
					else
					{
						bool flag4 = a == "base64";
						if (flag4)
						{
							result = ContentTransferEncoding_enum.Base64;
						}
						else
						{
							bool flag5 = a == "8bit";
							if (flag5)
							{
								result = ContentTransferEncoding_enum._8bit;
							}
							else
							{
								bool flag6 = a == "binary";
								if (flag6)
								{
									result = ContentTransferEncoding_enum.Binary;
								}
								else
								{
									result = ContentTransferEncoding_enum.Unknown;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x0005BB94 File Offset: 0x0005AB94
		public static string ContentTransferEncodingToString(ContentTransferEncoding_enum encoding)
		{
			bool flag = encoding == ContentTransferEncoding_enum._7bit;
			string result;
			if (flag)
			{
				result = "7bit";
			}
			else
			{
				bool flag2 = encoding == ContentTransferEncoding_enum.QuotedPrintable;
				if (flag2)
				{
					result = "quoted-printable";
				}
				else
				{
					bool flag3 = encoding == ContentTransferEncoding_enum.Base64;
					if (flag3)
					{
						result = "base64";
					}
					else
					{
						bool flag4 = encoding == ContentTransferEncoding_enum._8bit;
						if (flag4)
						{
							result = "8bit";
						}
						else
						{
							bool flag5 = encoding == ContentTransferEncoding_enum.Binary;
							if (flag5)
							{
								result = "binary";
							}
							else
							{
								bool flag6 = encoding == ContentTransferEncoding_enum.Unknown;
								if (flag6)
								{
									result = "unknown";
								}
								else
								{
									result = null;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x0005BC18 File Offset: 0x0005AC18
		public static ContentDisposition_enum ParseContentDisposition(string headerFieldValue)
		{
			bool flag = headerFieldValue == null;
			ContentDisposition_enum result;
			if (flag)
			{
				result = ContentDisposition_enum.NotSpecified;
			}
			else
			{
				string text = headerFieldValue.ToLower();
				bool flag2 = text.IndexOf("attachment") > -1;
				if (flag2)
				{
					result = ContentDisposition_enum.Attachment;
				}
				else
				{
					bool flag3 = text.IndexOf("inline") > -1;
					if (flag3)
					{
						result = ContentDisposition_enum.Inline;
					}
					else
					{
						result = ContentDisposition_enum.Unknown;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x0005BC74 File Offset: 0x0005AC74
		public static string ContentDispositionToString(ContentDisposition_enum disposition)
		{
			bool flag = disposition == ContentDisposition_enum.Attachment;
			string result;
			if (flag)
			{
				result = "attachment";
			}
			else
			{
				bool flag2 = disposition == ContentDisposition_enum.Inline;
				if (flag2)
				{
					result = "inline";
				}
				else
				{
					bool flag3 = disposition == ContentDisposition_enum.Unknown;
					if (flag3)
					{
						result = "unknown";
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0005BCBC File Offset: 0x0005ACBC
		public static string EncodeWord(string text)
		{
			bool flag = text == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = Core.IsAscii(text);
				if (flag2)
				{
					result = text;
				}
				else
				{
					result = Core.CanonicalEncode(text, "utf-8");
				}
			}
			return result;
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0005BCF8 File Offset: 0x0005ACF8
		public static string DecodeWords(string text)
		{
			bool flag = text == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				StringReader stringReader = new StringReader(text);
				StringBuilder stringBuilder = new StringBuilder();
				bool flag2 = false;
				while (stringReader.Available > 0L)
				{
					string text2 = stringReader.ReadToFirstChar();
					bool flag3 = stringReader.StartsWith("=?") && stringReader.SourceString.IndexOf("?=") > -1;
					if (flag3)
					{
						StringBuilder stringBuilder2 = new StringBuilder();
						string text3 = null;
						try
						{
							stringBuilder2.Append(stringReader.ReadSpecifiedLength(2));
							string text4 = stringReader.QuotedReadToDelimiter('?');
							stringBuilder2.Append(text4 + "?");
							string text5 = stringReader.QuotedReadToDelimiter('?');
							stringBuilder2.Append(text5 + "?");
							string text6 = stringReader.QuotedReadToDelimiter('?');
							stringBuilder2.Append(text6 + "?");
							bool flag4 = stringReader.StartsWith("=");
							if (flag4)
							{
								stringBuilder2.Append(stringReader.ReadSpecifiedLength(1));
								Encoding encoding = Encoding.GetEncoding(text4);
								bool flag5 = text5.ToLower() == "q";
								if (flag5)
								{
									text3 = Core.QDecode(encoding, text6);
								}
								else
								{
									bool flag6 = text5.ToLower() == "b";
									if (flag6)
									{
										text3 = encoding.GetString(Core.Base64Decode(Encoding.Default.GetBytes(text6)));
									}
								}
							}
						}
						catch
						{
						}
						bool flag7 = !flag2;
						if (flag7)
						{
							stringBuilder.Append(text2);
						}
						bool flag8 = text3 == null;
						if (flag8)
						{
							stringBuilder.Append(stringBuilder2.ToString());
						}
						else
						{
							stringBuilder.Append(text3);
						}
						flag2 = true;
					}
					else
					{
						bool flag9 = stringReader.StartsWithWord();
						if (flag9)
						{
							stringBuilder.Append(text2 + stringReader.ReadWord(false));
							flag2 = false;
						}
						else
						{
							stringBuilder.Append(text2 + stringReader.ReadSpecifiedLength(1));
						}
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0005BF14 File Offset: 0x0005AF14
		public static string EncodeHeaderField(string text)
		{
			bool flag = Core.IsAscii(text);
			string result;
			if (flag)
			{
				result = text;
			}
			else
			{
				bool flag2 = text.IndexOf("\"") > -1;
				if (flag2)
				{
					string text2 = text;
					int i = 0;
					while (i < text2.Length - 1)
					{
						int num = text2.IndexOf("\"", i);
						bool flag3 = num == -1;
						if (flag3)
						{
							break;
						}
						int num2 = text2.IndexOf("\"", num + 1);
						bool flag4 = num2 == -1;
						if (flag4)
						{
							break;
						}
						string text3 = text2.Substring(0, num);
						string text4 = text2.Substring(num2 + 1);
						string text5 = text2.Substring(num + 1, num2 - num - 1);
						bool flag5 = !Core.IsAscii(text5);
						if (flag5)
						{
							string text6 = Core.CanonicalEncode(text5, "utf-8");
							text2 = string.Concat(new string[]
							{
								text3,
								"\"",
								text6,
								"\"",
								text4
							});
							i += num2 + 1 + text6.Length - text5.Length;
						}
						else
						{
							i += num2 + 1;
						}
					}
					bool flag6 = Core.IsAscii(text2);
					if (flag6)
					{
						result = text2;
					}
					else
					{
						result = Core.CanonicalEncode(text, "utf-8");
					}
				}
				else
				{
					result = Core.CanonicalEncode(text, "utf-8");
				}
			}
			return result;
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0005C078 File Offset: 0x0005B078
		public static string CreateMessageID()
		{
			return string.Concat(new string[]
			{
				"<",
				Guid.NewGuid().ToString().Replace("-", ""),
				"@",
				Guid.NewGuid().ToString().Replace("-", ""),
				">"
			});
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x0005C0F8 File Offset: 0x0005B0F8
		public static string FoldData(string data)
		{
			bool flag = data.Length > 76;
			string result;
			if (flag)
			{
				int num = 0;
				int num2 = -1;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < data.Length; i++)
				{
					char c = data[i];
					bool flag2 = c == ' ' || c == '\t';
					if (flag2)
					{
						num2 = i;
					}
					bool flag3 = i == data.Length - 1;
					if (flag3)
					{
						stringBuilder.Append(data.Substring(num));
					}
					else
					{
						bool flag4 = i - num >= 76;
						if (flag4)
						{
							bool flag5 = num2 == -1;
							if (flag5)
							{
								num2 = i;
							}
							stringBuilder.Append(data.Substring(num, num2 - num) + "\r\n\t");
							i = num2;
							num2 = -1;
							num = i;
						}
					}
				}
				result = stringBuilder.ToString();
			}
			else
			{
				result = data;
			}
			return result;
		}
	}
}
