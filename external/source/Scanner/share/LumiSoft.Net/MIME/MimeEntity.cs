using System;
using System.Collections;
using System.IO;
using System.Text;
using LumiSoft.Net.IO;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mime
{
	// Token: 0x02000166 RID: 358
	[Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MimeEntity
	{
		// Token: 0x06000E5F RID: 3679 RVA: 0x000589A4 File Offset: 0x000579A4
		public MimeEntity()
		{
			this.m_pHeader = new HeaderFieldCollection();
			this.m_pChildEntities = new MimeEntityCollection(this);
			this.m_pHeaderFieldCache = new Hashtable();
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x00058A00 File Offset: 0x00057A00
		internal bool Parse(SmartStream stream, string toBoundary)
		{
			this.m_pHeader.Clear();
			this.m_pHeaderFieldCache.Clear();
			this.m_pHeader.Parse(stream);
			bool flag = (this.ContentType & MediaType_enum.Multipart) > (MediaType_enum)0;
			if (flag)
			{
				string contentType_Boundary = this.ContentType_Boundary;
				bool flag2 = contentType_Boundary == null;
				if (!flag2)
				{
					SmartStream.ReadLineAsyncOP readLineAsyncOP = new SmartStream.ReadLineAsyncOP(new byte[8000], SizeExceededAction.JunkAndThrowException);
					stream.ReadLine(readLineAsyncOP, false);
					bool flag3 = readLineAsyncOP.Error != null;
					if (flag3)
					{
						throw readLineAsyncOP.Error;
					}
					string lineUtf;
					for (lineUtf = readLineAsyncOP.LineUtf8; lineUtf != null; lineUtf = readLineAsyncOP.LineUtf8)
					{
						bool flag4 = lineUtf.StartsWith("--" + contentType_Boundary);
						if (flag4)
						{
							break;
						}
						stream.ReadLine(readLineAsyncOP, false);
						bool flag5 = readLineAsyncOP.Error != null;
						if (flag5)
						{
							throw readLineAsyncOP.Error;
						}
					}
					bool flag6 = string.IsNullOrEmpty(lineUtf);
					if (flag6)
					{
						return false;
					}
					bool flag7;
					do
					{
						MimeEntity mimeEntity = new MimeEntity();
						this.ChildEntities.Add(mimeEntity);
						flag7 = !mimeEntity.Parse(stream, contentType_Boundary);
					}
					while (!flag7);
					bool flag8 = !string.IsNullOrEmpty(toBoundary);
					if (flag8)
					{
						stream.ReadLine(readLineAsyncOP, false);
						bool flag9 = readLineAsyncOP.Error != null;
						if (flag9)
						{
							throw readLineAsyncOP.Error;
						}
						for (lineUtf = readLineAsyncOP.LineUtf8; lineUtf != null; lineUtf = readLineAsyncOP.LineUtf8)
						{
							bool flag10 = lineUtf.StartsWith("--" + toBoundary);
							if (flag10)
							{
								break;
							}
							stream.ReadLine(readLineAsyncOP, false);
							bool flag11 = readLineAsyncOP.Error != null;
							if (flag11)
							{
								throw readLineAsyncOP.Error;
							}
						}
						bool flag12 = string.IsNullOrEmpty(lineUtf);
						if (flag12)
						{
							return false;
						}
						bool flag13 = lineUtf.EndsWith(toBoundary + "--");
						return !flag13;
					}
				}
			}
			else
			{
				bool flag14 = !string.IsNullOrEmpty(toBoundary);
				if (flag14)
				{
					MemoryStream memoryStream = new MemoryStream();
					SmartStream.ReadLineAsyncOP readLineAsyncOP2 = new SmartStream.ReadLineAsyncOP(new byte[32000], SizeExceededAction.JunkAndThrowException);
					for (;;)
					{
						stream.ReadLine(readLineAsyncOP2, false);
						bool flag15 = readLineAsyncOP2.Error != null;
						if (flag15)
						{
							break;
						}
						bool flag16 = readLineAsyncOP2.BytesInBuffer == 0;
						if (flag16)
						{
							goto Block_16;
						}
						bool flag17 = readLineAsyncOP2.LineBytesInBuffer >= 2 && readLineAsyncOP2.Buffer[0] == 45 && readLineAsyncOP2.Buffer[1] == 45;
						if (flag17)
						{
							string lineUtf2 = readLineAsyncOP2.LineUtf8;
							bool flag18 = lineUtf2 == "--" + toBoundary + "--";
							if (flag18)
							{
								goto Block_20;
							}
							bool flag19 = lineUtf2 == "--" + toBoundary;
							if (flag19)
							{
								goto Block_21;
							}
						}
						memoryStream.Write(readLineAsyncOP2.Buffer, 0, readLineAsyncOP2.BytesInBuffer);
					}
					throw readLineAsyncOP2.Error;
					Block_16:
					this.m_EncodedData = memoryStream.ToArray();
					return false;
					Block_20:
					this.m_EncodedData = memoryStream.ToArray();
					return false;
					Block_21:
					this.m_EncodedData = memoryStream.ToArray();
					return true;
				}
				MemoryStream memoryStream2 = new MemoryStream();
				stream.ReadAll(memoryStream2);
				this.m_EncodedData = memoryStream2.ToArray();
			}
			return false;
		}

		// Token: 0x06000E61 RID: 3681 RVA: 0x00058D64 File Offset: 0x00057D64
		public void ToStream(Stream storeStream)
		{
			byte[] bytes = Encoding.Default.GetBytes(this.FoldHeader(this.HeaderString));
			storeStream.Write(bytes, 0, bytes.Length);
			bool flag = (this.ContentType & MediaType_enum.Multipart) > (MediaType_enum)0;
			if (flag)
			{
				string contentType_Boundary = this.ContentType_Boundary;
				foreach (object obj in this.ChildEntities)
				{
					MimeEntity mimeEntity = (MimeEntity)obj;
					bytes = Encoding.Default.GetBytes("\r\n--" + contentType_Boundary + "\r\n");
					storeStream.Write(bytes, 0, bytes.Length);
					mimeEntity.ToStream(storeStream);
				}
				bytes = Encoding.Default.GetBytes("\r\n--" + contentType_Boundary + "--\r\n");
				storeStream.Write(bytes, 0, bytes.Length);
			}
			else
			{
				storeStream.Write(new byte[]
				{
					13,
					10
				}, 0, 2);
				bool flag2 = this.DataEncoded != null;
				if (flag2)
				{
					storeStream.Write(this.DataEncoded, 0, this.DataEncoded.Length);
				}
			}
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x00058E9C File Offset: 0x00057E9C
		public void DataToFile(string fileName)
		{
			using (FileStream fileStream = File.Create(fileName))
			{
				this.DataToStream(fileStream);
			}
		}

		// Token: 0x06000E63 RID: 3683 RVA: 0x00058ED8 File Offset: 0x00057ED8
		public void DataToStream(Stream stream)
		{
			byte[] data = this.Data;
			stream.Write(data, 0, data.Length);
		}

		// Token: 0x06000E64 RID: 3684 RVA: 0x00058EFC File Offset: 0x00057EFC
		public void DataFromFile(string fileName)
		{
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				this.DataFromStream(fileStream);
			}
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x00058F38 File Offset: 0x00057F38
		public void DataFromStream(Stream stream)
		{
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, (int)stream.Length);
			this.Data = array;
		}

		// Token: 0x06000E66 RID: 3686 RVA: 0x00058F6C File Offset: 0x00057F6C
		private byte[] EncodeData(byte[] data, ContentTransferEncoding_enum encoding)
		{
			bool flag = encoding == ContentTransferEncoding_enum.NotSpecified;
			if (flag)
			{
				throw new Exception("Please specify Content-Transfer-Encoding first !");
			}
			bool flag2 = encoding == ContentTransferEncoding_enum.Unknown;
			if (flag2)
			{
				throw new Exception("Not supported Content-Transfer-Encoding. If it's your custom encoding, encode data yourself and set it with DataEncoded property !");
			}
			bool flag3 = encoding == ContentTransferEncoding_enum.Base64;
			byte[] result;
			if (flag3)
			{
				result = Core.Base64Encode(data);
			}
			else
			{
				bool flag4 = encoding == ContentTransferEncoding_enum.QuotedPrintable;
				if (flag4)
				{
					result = Core.QuotedPrintableEncode(data);
				}
				else
				{
					result = data;
				}
			}
			return result;
		}

		// Token: 0x06000E67 RID: 3687 RVA: 0x00058FD0 File Offset: 0x00057FD0
		private string FoldHeader(string header)
		{
			StringBuilder stringBuilder = new StringBuilder();
			header = header.Replace("\r\n", "\n");
			string[] array = header.Split(new char[]
			{
				'\n'
			});
			foreach (string text in array)
			{
				bool flag = text.IndexOf('\t') > -1;
				if (flag)
				{
					stringBuilder.Append(text.Replace("\t", "\r\n\t") + "\r\n");
				}
				else
				{
					stringBuilder.Append(text + "\r\n");
				}
			}
			bool flag2 = stringBuilder.Length > 1;
			string result;
			if (flag2)
			{
				result = stringBuilder.ToString(0, stringBuilder.Length - 2);
			}
			else
			{
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06000E68 RID: 3688 RVA: 0x000590A0 File Offset: 0x000580A0
		public HeaderFieldCollection Header
		{
			get
			{
				return this.m_pHeader;
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06000E69 RID: 3689 RVA: 0x000590B8 File Offset: 0x000580B8
		public string HeaderString
		{
			get
			{
				return this.m_pHeader.ToHeaderString("utf-8");
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06000E6A RID: 3690 RVA: 0x000590DC File Offset: 0x000580DC
		public MimeEntity ParentEntity
		{
			get
			{
				return this.m_pParentEntity;
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06000E6B RID: 3691 RVA: 0x000590F4 File Offset: 0x000580F4
		public MimeEntityCollection ChildEntities
		{
			get
			{
				return this.m_pChildEntities;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06000E6C RID: 3692 RVA: 0x0005910C File Offset: 0x0005810C
		// (set) Token: 0x06000E6D RID: 3693 RVA: 0x00059150 File Offset: 0x00058150
		public string MimeVersion
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Mime-Version:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Mime-Version:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Mime-Version:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Mime-Version:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Mime-Version:", value);
				}
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06000E6E RID: 3694 RVA: 0x000591A4 File Offset: 0x000581A4
		// (set) Token: 0x06000E6F RID: 3695 RVA: 0x000591E8 File Offset: 0x000581E8
		public string ContentClass
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Class:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Content-Class:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Content-Class:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Content-Class:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Content-Class:", value);
				}
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06000E70 RID: 3696 RVA: 0x0005923C File Offset: 0x0005823C
		// (set) Token: 0x06000E71 RID: 3697 RVA: 0x00059290 File Offset: 0x00058290
		public MediaType_enum ContentType
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Type:");
				MediaType_enum result;
				if (flag)
				{
					string value = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:")).Value;
					result = MimeUtils.ParseMediaType(value);
				}
				else
				{
					result = MediaType_enum.NotSpecified;
				}
				return result;
			}
			set
			{
				bool flag = this.DataEncoded != null;
				if (flag)
				{
					throw new Exception("ContentType can't be changed while there is data specified, set data to null before !");
				}
				bool flag2 = value == MediaType_enum.Unknown;
				if (flag2)
				{
					throw new Exception("MediaType_enum.Unkown isn't allowed to set !");
				}
				bool flag3 = value == MediaType_enum.NotSpecified;
				if (flag3)
				{
					throw new Exception("MediaType_enum.NotSpecified isn't allowed to set !");
				}
				bool flag4 = value == MediaType_enum.Text_plain;
				string value2;
				if (flag4)
				{
					value2 = "text/plain; charset=\"utf-8\"";
				}
				else
				{
					bool flag5 = value == MediaType_enum.Text_html;
					if (flag5)
					{
						value2 = "text/html; charset=\"utf-8\"";
					}
					else
					{
						bool flag6 = value == MediaType_enum.Text_xml;
						if (flag6)
						{
							value2 = "text/xml; charset=\"utf-8\"";
						}
						else
						{
							bool flag7 = value == MediaType_enum.Text_rtf;
							if (flag7)
							{
								value2 = "text/rtf; charset=\"utf-8\"";
							}
							else
							{
								bool flag8 = value == MediaType_enum.Text;
								if (flag8)
								{
									value2 = "text; charset=\"utf-8\"";
								}
								else
								{
									bool flag9 = value == MediaType_enum.Image_gif;
									if (flag9)
									{
										value2 = "image/gif";
									}
									else
									{
										bool flag10 = value == MediaType_enum.Image_tiff;
										if (flag10)
										{
											value2 = "image/tiff";
										}
										else
										{
											bool flag11 = value == MediaType_enum.Image_jpeg;
											if (flag11)
											{
												value2 = "image/jpeg";
											}
											else
											{
												bool flag12 = value == MediaType_enum.Image;
												if (flag12)
												{
													value2 = "image";
												}
												else
												{
													bool flag13 = value == MediaType_enum.Audio;
													if (flag13)
													{
														value2 = "audio";
													}
													else
													{
														bool flag14 = value == MediaType_enum.Video;
														if (flag14)
														{
															value2 = "video";
														}
														else
														{
															bool flag15 = value == MediaType_enum.Application_octet_stream;
															if (flag15)
															{
																value2 = "application/octet-stream";
															}
															else
															{
																bool flag16 = value == MediaType_enum.Application;
																if (flag16)
																{
																	value2 = "application";
																}
																else
																{
																	bool flag17 = value == MediaType_enum.Multipart_mixed;
																	if (flag17)
																	{
																		value2 = "multipart/mixed;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																	}
																	else
																	{
																		bool flag18 = value == MediaType_enum.Multipart_alternative;
																		if (flag18)
																		{
																			value2 = "multipart/alternative;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																		}
																		else
																		{
																			bool flag19 = value == MediaType_enum.Multipart_parallel;
																			if (flag19)
																			{
																				value2 = "multipart/parallel;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																			}
																			else
																			{
																				bool flag20 = value == MediaType_enum.Multipart_related;
																				if (flag20)
																				{
																					value2 = "multipart/related;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																				}
																				else
																				{
																					bool flag21 = value == MediaType_enum.Multipart_signed;
																					if (flag21)
																					{
																						value2 = "multipart/signed;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																					}
																					else
																					{
																						bool flag22 = value == MediaType_enum.Multipart;
																						if (flag22)
																						{
																							value2 = "multipart;\tboundary=\"part_" + Guid.NewGuid().ToString().Replace("-", "_") + "\"";
																						}
																						else
																						{
																							bool flag23 = value == MediaType_enum.Message_rfc822;
																							if (flag23)
																							{
																								value2 = "message/rfc822";
																							}
																							else
																							{
																								bool flag24 = value == MediaType_enum.Message;
																								if (!flag24)
																								{
																									throw new Exception("Invalid flags combination of MediaType_enum was specified !");
																								}
																								value2 = "message";
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
				bool flag25 = this.m_pHeader.Contains("Content-Type:");
				if (flag25)
				{
					this.m_pHeader.GetFirst("Content-Type:").Value = value2;
				}
				else
				{
					this.m_pHeader.Add("Content-Type:", value2);
				}
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06000E72 RID: 3698 RVA: 0x00059664 File Offset: 0x00058664
		// (set) Token: 0x06000E73 RID: 3699 RVA: 0x000596A8 File Offset: 0x000586A8
		public string ContentTypeString
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Type:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Content-Type:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.DataEncoded != null;
				if (flag)
				{
					throw new Exception("ContentType can't be changed while there is data specified, set data to null before !");
				}
				bool flag2 = this.m_pHeader.Contains("Content-Type:");
				if (flag2)
				{
					this.m_pHeader.GetFirst("Content-Type:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Content-Type:", value);
				}
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06000E74 RID: 3700 RVA: 0x00059714 File Offset: 0x00058714
		// (set) Token: 0x06000E75 RID: 3701 RVA: 0x0005975C File Offset: 0x0005875C
		public ContentTransferEncoding_enum ContentTransferEncoding
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Transfer-Encoding:");
				ContentTransferEncoding_enum result;
				if (flag)
				{
					result = MimeUtils.ParseContentTransferEncoding(this.m_pHeader.GetFirst("Content-Transfer-Encoding:").Value);
				}
				else
				{
					result = ContentTransferEncoding_enum.NotSpecified;
				}
				return result;
			}
			set
			{
				bool flag = value == ContentTransferEncoding_enum.Unknown;
				if (flag)
				{
					throw new Exception("ContentTransferEncoding_enum.Unknown isn't allowed to set !");
				}
				bool flag2 = value == ContentTransferEncoding_enum.NotSpecified;
				if (flag2)
				{
					throw new Exception("ContentTransferEncoding_enum.NotSpecified isn't allowed to set !");
				}
				string value2 = MimeUtils.ContentTransferEncodingToString(value);
				bool flag3 = this.DataEncoded != null;
				if (flag3)
				{
					ContentTransferEncoding_enum contentTransferEncoding = this.ContentTransferEncoding;
					bool flag4 = contentTransferEncoding == ContentTransferEncoding_enum.Unknown || contentTransferEncoding == ContentTransferEncoding_enum.NotSpecified;
					if (flag4)
					{
						throw new Exception("Data can't be converted because old encoding '" + MimeUtils.ContentTransferEncodingToString(contentTransferEncoding) + "' is unknown !");
					}
					this.DataEncoded = this.EncodeData(this.Data, value);
				}
				bool flag5 = this.m_pHeader.Contains("Content-Transfer-Encoding:");
				if (flag5)
				{
					this.m_pHeader.GetFirst("Content-Transfer-Encoding:").Value = value2;
				}
				else
				{
					this.m_pHeader.Add("Content-Transfer-Encoding:", value2);
				}
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06000E76 RID: 3702 RVA: 0x00059840 File Offset: 0x00058840
		// (set) Token: 0x06000E77 RID: 3703 RVA: 0x00059888 File Offset: 0x00058888
		public ContentDisposition_enum ContentDisposition
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Disposition:");
				ContentDisposition_enum result;
				if (flag)
				{
					result = MimeUtils.ParseContentDisposition(this.m_pHeader.GetFirst("Content-Disposition:").Value);
				}
				else
				{
					result = ContentDisposition_enum.NotSpecified;
				}
				return result;
			}
			set
			{
				bool flag = value == ContentDisposition_enum.Unknown;
				if (flag)
				{
					throw new Exception("ContentDisposition_enum.Unknown isn't allowed to set !");
				}
				bool flag2 = value == ContentDisposition_enum.NotSpecified;
				if (flag2)
				{
					HeaderField first = this.m_pHeader.GetFirst("Content-Disposition:");
					bool flag3 = first != null;
					if (flag3)
					{
						this.m_pHeader.Remove(first);
					}
				}
				else
				{
					string value2 = MimeUtils.ContentDispositionToString(value);
					bool flag4 = this.m_pHeader.Contains("Content-Disposition:");
					if (flag4)
					{
						this.m_pHeader.GetFirst("Content-Disposition:").Value = value2;
					}
					else
					{
						this.m_pHeader.Add("Content-Disposition:", value2);
					}
				}
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x00059934 File Offset: 0x00058934
		// (set) Token: 0x06000E79 RID: 3705 RVA: 0x00059978 File Offset: 0x00058978
		public string ContentDescription
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Description:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Content-Description:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Content-Description:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Content-Description:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Content-Description:", value);
				}
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x000599CC File Offset: 0x000589CC
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x00059A10 File Offset: 0x00058A10
		public string ContentID
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-ID:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Content-ID:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Content-ID:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Content-ID:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Content-ID:", value);
				}
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x00059A64 File Offset: 0x00058A64
		// (set) Token: 0x06000E7D RID: 3709 RVA: 0x00059AD0 File Offset: 0x00058AD0
		public string ContentType_Name
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Type:");
				string result;
				if (flag)
				{
					ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
					bool flag2 = parametizedHeaderField.Parameters.Contains("name");
					if (flag2)
					{
						result = parametizedHeaderField.Parameters["name"];
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = !this.m_pHeader.Contains("Content-Type:");
				if (flag)
				{
					throw new Exception("Please specify Content-Type first !");
				}
				bool flag2 = (this.ContentType & MediaType_enum.Application) == (MediaType_enum)0;
				if (flag2)
				{
					throw new Exception("Parameter name is available only for ContentType application/xxx !");
				}
				ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
				bool flag3 = parametizedHeaderField.Parameters.Contains("name");
				if (flag3)
				{
					parametizedHeaderField.Parameters["name"] = value;
				}
				else
				{
					parametizedHeaderField.Parameters.Add("name", value);
				}
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x00059B74 File Offset: 0x00058B74
		// (set) Token: 0x06000E7F RID: 3711 RVA: 0x00059BE0 File Offset: 0x00058BE0
		public string ContentType_CharSet
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Type:");
				string result;
				if (flag)
				{
					ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
					bool flag2 = parametizedHeaderField.Parameters.Contains("charset");
					if (flag2)
					{
						result = parametizedHeaderField.Parameters["charset"];
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = !this.m_pHeader.Contains("Content-Type:");
				if (flag)
				{
					throw new Exception("Please specify Content-Type first !");
				}
				bool flag2 = (this.ContentType & MediaType_enum.Text) == (MediaType_enum)0;
				if (flag2)
				{
					throw new Exception("Parameter boundary is available only for ContentType text/xxx !");
				}
				bool flag3 = this.DataEncoded != null;
				if (flag3)
				{
					string text = this.ContentType_CharSet;
					bool flag4 = text == null;
					if (flag4)
					{
						text = "ascii";
					}
					try
					{
						Encoding.GetEncoding(text);
					}
					catch
					{
						throw new Exception("Data can't be converted because current charset '" + text + "' isn't supported !");
					}
					try
					{
						Encoding encoding = Encoding.GetEncoding(value);
						this.Data = encoding.GetBytes(this.DataText);
					}
					catch
					{
						throw new Exception("Data can't be converted because new charset '" + value + "' isn't supported !");
					}
				}
				ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
				bool flag5 = parametizedHeaderField.Parameters.Contains("charset");
				if (flag5)
				{
					parametizedHeaderField.Parameters["charset"] = value;
				}
				else
				{
					parametizedHeaderField.Parameters.Add("charset", value);
				}
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x00059D28 File Offset: 0x00058D28
		// (set) Token: 0x06000E81 RID: 3713 RVA: 0x00059D94 File Offset: 0x00058D94
		public string ContentType_Boundary
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Type:");
				string result;
				if (flag)
				{
					ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
					bool flag2 = parametizedHeaderField.Parameters.Contains("boundary");
					if (flag2)
					{
						result = parametizedHeaderField.Parameters["boundary"];
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = !this.m_pHeader.Contains("Content-Type:");
				if (flag)
				{
					throw new Exception("Please specify Content-Type first !");
				}
				bool flag2 = (this.ContentType & MediaType_enum.Multipart) == (MediaType_enum)0;
				if (flag2)
				{
					throw new Exception("Parameter boundary is available only for ContentType multipart/xxx !");
				}
				ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Type:"));
				bool flag3 = parametizedHeaderField.Parameters.Contains("boundary");
				if (flag3)
				{
					parametizedHeaderField.Parameters["boundary"] = value;
				}
				else
				{
					parametizedHeaderField.Parameters.Add("boundary", value);
				}
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06000E82 RID: 3714 RVA: 0x00059E38 File Offset: 0x00058E38
		// (set) Token: 0x06000E83 RID: 3715 RVA: 0x00059EAC File Offset: 0x00058EAC
		public string ContentDisposition_FileName
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Content-Disposition:");
				string result;
				if (flag)
				{
					ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Disposition:"));
					bool flag2 = parametizedHeaderField.Parameters.Contains("filename");
					if (flag2)
					{
						result = MimeUtils.DecodeWords(parametizedHeaderField.Parameters["filename"]);
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = !this.m_pHeader.Contains("Content-Disposition:");
				if (flag)
				{
					throw new Exception("Please specify Content-Disposition first !");
				}
				ParametizedHeaderField parametizedHeaderField = new ParametizedHeaderField(this.m_pHeader.GetFirst("Content-Disposition:"));
				bool flag2 = parametizedHeaderField.Parameters.Contains("filename");
				if (flag2)
				{
					parametizedHeaderField.Parameters["filename"] = MimeUtils.EncodeWord(value);
				}
				else
				{
					parametizedHeaderField.Parameters.Add("filename", MimeUtils.EncodeWord(value));
				}
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06000E84 RID: 3716 RVA: 0x00059F3C File Offset: 0x00058F3C
		// (set) Token: 0x06000E85 RID: 3717 RVA: 0x00059FA4 File Offset: 0x00058FA4
		public DateTime Date
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Date:");
				if (flag)
				{
					try
					{
						return MIME_Utils.ParseRfc2822DateTime(this.m_pHeader.GetFirst("Date:").Value);
					}
					catch
					{
						return DateTime.MinValue;
					}
				}
				return DateTime.MinValue;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Date:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Date:").Value = MIME_Utils.DateTimeToRfc2822(value);
				}
				else
				{
					this.m_pHeader.Add("Date:", MimeUtils.DateTimeToRfc2822(value));
				}
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0005A000 File Offset: 0x00059000
		// (set) Token: 0x06000E87 RID: 3719 RVA: 0x0005A044 File Offset: 0x00059044
		public string MessageID
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Message-ID:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Message-ID:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Message-ID:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Message-ID:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Message-ID:", value);
				}
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06000E88 RID: 3720 RVA: 0x0005A098 File Offset: 0x00059098
		// (set) Token: 0x06000E89 RID: 3721 RVA: 0x0005A138 File Offset: 0x00059138
		public AddressList To
		{
			get
			{
				bool flag = this.m_pHeader.Contains("To:");
				AddressList result;
				if (flag)
				{
					bool flag2 = this.m_pHeaderFieldCache.Contains("To:");
					if (flag2)
					{
						result = (AddressList)this.m_pHeaderFieldCache["To:"];
					}
					else
					{
						HeaderField first = this.m_pHeader.GetFirst("To:");
						AddressList addressList = new AddressList();
						addressList.Parse(first.EncodedValue);
						addressList.BoundedHeaderField = first;
						this.m_pHeaderFieldCache["To:"] = addressList;
						result = addressList;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.Remove(this.m_pHeader.GetFirst("To:"));
				}
				else
				{
					bool flag2 = this.m_pHeaderFieldCache["To:"] != null;
					if (flag2)
					{
						((AddressList)this.m_pHeaderFieldCache["To:"]).BoundedHeaderField = null;
					}
					HeaderField headerField = this.m_pHeader.GetFirst("To:");
					bool flag3 = headerField == null;
					if (flag3)
					{
						headerField = new HeaderField("To:", value.ToAddressListString());
						this.m_pHeader.Add(headerField);
					}
					else
					{
						headerField.Value = value.ToAddressListString();
					}
					value.BoundedHeaderField = headerField;
					this.m_pHeaderFieldCache["To:"] = value;
				}
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06000E8A RID: 3722 RVA: 0x0005A20C File Offset: 0x0005920C
		// (set) Token: 0x06000E8B RID: 3723 RVA: 0x0005A2AC File Offset: 0x000592AC
		public AddressList Cc
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Cc:");
				AddressList result;
				if (flag)
				{
					bool flag2 = this.m_pHeaderFieldCache.Contains("Cc:");
					if (flag2)
					{
						result = (AddressList)this.m_pHeaderFieldCache["Cc:"];
					}
					else
					{
						HeaderField first = this.m_pHeader.GetFirst("Cc:");
						AddressList addressList = new AddressList();
						addressList.Parse(first.EncodedValue);
						addressList.BoundedHeaderField = first;
						this.m_pHeaderFieldCache["Cc:"] = addressList;
						result = addressList;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.Remove(this.m_pHeader.GetFirst("Cc:"));
				}
				else
				{
					bool flag2 = this.m_pHeaderFieldCache["Cc:"] != null;
					if (flag2)
					{
						((AddressList)this.m_pHeaderFieldCache["Cc:"]).BoundedHeaderField = null;
					}
					HeaderField headerField = this.m_pHeader.GetFirst("Cc:");
					bool flag3 = headerField == null;
					if (flag3)
					{
						headerField = new HeaderField("Cc:", value.ToAddressListString());
						this.m_pHeader.Add(headerField);
					}
					else
					{
						headerField.Value = value.ToAddressListString();
					}
					value.BoundedHeaderField = headerField;
					this.m_pHeaderFieldCache["Cc:"] = value;
				}
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06000E8C RID: 3724 RVA: 0x0005A380 File Offset: 0x00059380
		// (set) Token: 0x06000E8D RID: 3725 RVA: 0x0005A420 File Offset: 0x00059420
		public AddressList Bcc
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Bcc:");
				AddressList result;
				if (flag)
				{
					bool flag2 = this.m_pHeaderFieldCache.Contains("Bcc:");
					if (flag2)
					{
						result = (AddressList)this.m_pHeaderFieldCache["Bcc:"];
					}
					else
					{
						HeaderField first = this.m_pHeader.GetFirst("Bcc:");
						AddressList addressList = new AddressList();
						addressList.Parse(first.EncodedValue);
						addressList.BoundedHeaderField = first;
						this.m_pHeaderFieldCache["Bcc:"] = addressList;
						result = addressList;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_pHeader.Remove(this.m_pHeader.GetFirst("Bcc:"));
				}
				else
				{
					bool flag2 = this.m_pHeaderFieldCache["Bcc:"] != null;
					if (flag2)
					{
						((AddressList)this.m_pHeaderFieldCache["Bcc:"]).BoundedHeaderField = null;
					}
					HeaderField headerField = this.m_pHeader.GetFirst("Bcc:");
					bool flag3 = headerField == null;
					if (flag3)
					{
						headerField = new HeaderField("Bcc:", value.ToAddressListString());
						this.m_pHeader.Add(headerField);
					}
					else
					{
						headerField.Value = value.ToAddressListString();
					}
					value.BoundedHeaderField = headerField;
					this.m_pHeaderFieldCache["Bcc:"] = value;
				}
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06000E8E RID: 3726 RVA: 0x0005A4F4 File Offset: 0x000594F4
		// (set) Token: 0x06000E8F RID: 3727 RVA: 0x0005A594 File Offset: 0x00059594
		public AddressList From
		{
			get
			{
				bool flag = this.m_pHeader.Contains("From:");
				AddressList result;
				if (flag)
				{
					bool flag2 = this.m_pHeaderFieldCache.Contains("From:");
					if (flag2)
					{
						result = (AddressList)this.m_pHeaderFieldCache["From:"];
					}
					else
					{
						HeaderField first = this.m_pHeader.GetFirst("From:");
						AddressList addressList = new AddressList();
						addressList.Parse(first.EncodedValue);
						addressList.BoundedHeaderField = first;
						this.m_pHeaderFieldCache["From:"] = addressList;
						result = addressList;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null && this.m_pHeader.Contains("From:");
				if (flag)
				{
					this.m_pHeader.Remove(this.m_pHeader.GetFirst("From:"));
				}
				else
				{
					bool flag2 = this.m_pHeaderFieldCache["From:"] != null;
					if (flag2)
					{
						((AddressList)this.m_pHeaderFieldCache["From:"]).BoundedHeaderField = null;
					}
					HeaderField headerField = this.m_pHeader.GetFirst("From:");
					bool flag3 = headerField == null;
					if (flag3)
					{
						headerField = new HeaderField("From:", value.ToAddressListString());
						this.m_pHeader.Add(headerField);
					}
					else
					{
						headerField.Value = value.ToAddressListString();
					}
					value.BoundedHeaderField = headerField;
					this.m_pHeaderFieldCache["From:"] = value;
				}
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06000E90 RID: 3728 RVA: 0x0005A678 File Offset: 0x00059678
		// (set) Token: 0x06000E91 RID: 3729 RVA: 0x0005A6C0 File Offset: 0x000596C0
		public MailboxAddress Sender
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Sender:");
				MailboxAddress result;
				if (flag)
				{
					result = MailboxAddress.Parse(this.m_pHeader.GetFirst("Sender:").EncodedValue);
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Sender:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Sender:").Value = value.ToMailboxAddressString();
				}
				else
				{
					this.m_pHeader.Add("Sender:", value.ToMailboxAddressString());
				}
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06000E92 RID: 3730 RVA: 0x0005A71C File Offset: 0x0005971C
		// (set) Token: 0x06000E93 RID: 3731 RVA: 0x0005A7BC File Offset: 0x000597BC
		public AddressList ReplyTo
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Reply-To:");
				AddressList result;
				if (flag)
				{
					bool flag2 = this.m_pHeaderFieldCache.Contains("Reply-To:");
					if (flag2)
					{
						result = (AddressList)this.m_pHeaderFieldCache["Reply-To:"];
					}
					else
					{
						HeaderField first = this.m_pHeader.GetFirst("Reply-To:");
						AddressList addressList = new AddressList();
						addressList.Parse(first.Value);
						addressList.BoundedHeaderField = first;
						this.m_pHeaderFieldCache["Reply-To:"] = addressList;
						result = addressList;
					}
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = value == null && this.m_pHeader.Contains("Reply-To:");
				if (flag)
				{
					this.m_pHeader.Remove(this.m_pHeader.GetFirst("Reply-To:"));
				}
				else
				{
					bool flag2 = this.m_pHeaderFieldCache["Reply-To:"] != null;
					if (flag2)
					{
						((AddressList)this.m_pHeaderFieldCache["Reply-To:"]).BoundedHeaderField = null;
					}
					HeaderField headerField = this.m_pHeader.GetFirst("Reply-To:");
					bool flag3 = headerField == null;
					if (flag3)
					{
						headerField = new HeaderField("Reply-To:", value.ToAddressListString());
						this.m_pHeader.Add(headerField);
					}
					else
					{
						headerField.Value = value.ToAddressListString();
					}
					value.BoundedHeaderField = headerField;
					this.m_pHeaderFieldCache["Reply-To:"] = value;
				}
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06000E94 RID: 3732 RVA: 0x0005A8A0 File Offset: 0x000598A0
		// (set) Token: 0x06000E95 RID: 3733 RVA: 0x0005A8E4 File Offset: 0x000598E4
		public string InReplyTo
		{
			get
			{
				bool flag = this.m_pHeader.Contains("In-Reply-To:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("In-Reply-To:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("In-Reply-To:");
				if (flag)
				{
					this.m_pHeader.GetFirst("In-Reply-To:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("In-Reply-To:", value);
				}
			}
		}

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0005A938 File Offset: 0x00059938
		// (set) Token: 0x06000E97 RID: 3735 RVA: 0x0005A97C File Offset: 0x0005997C
		public string DSN
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Disposition-Notification-To:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Disposition-Notification-To:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Disposition-Notification-To:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Disposition-Notification-To:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Disposition-Notification-To:", value);
				}
			}
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06000E98 RID: 3736 RVA: 0x0005A9D0 File Offset: 0x000599D0
		// (set) Token: 0x06000E99 RID: 3737 RVA: 0x0005AA14 File Offset: 0x00059A14
		public string Subject
		{
			get
			{
				bool flag = this.m_pHeader.Contains("Subject:");
				string result;
				if (flag)
				{
					result = this.m_pHeader.GetFirst("Subject:").Value;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.m_pHeader.Contains("Subject:");
				if (flag)
				{
					this.m_pHeader.GetFirst("Subject:").Value = value;
				}
				else
				{
					this.m_pHeader.Add("Subject:", value);
				}
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06000E9A RID: 3738 RVA: 0x0005AA68 File Offset: 0x00059A68
		// (set) Token: 0x06000E9B RID: 3739 RVA: 0x0005AAB8 File Offset: 0x00059AB8
		public byte[] Data
		{
			get
			{
				ContentTransferEncoding_enum contentTransferEncoding = this.ContentTransferEncoding;
				bool flag = contentTransferEncoding == ContentTransferEncoding_enum.Base64;
				byte[] result;
				if (flag)
				{
					result = Core.Base64Decode(this.DataEncoded);
				}
				else
				{
					bool flag2 = contentTransferEncoding == ContentTransferEncoding_enum.QuotedPrintable;
					if (flag2)
					{
						result = Core.QuotedPrintableDecode(this.DataEncoded);
					}
					else
					{
						result = this.DataEncoded;
					}
				}
				return result;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.DataEncoded = null;
				}
				else
				{
					ContentTransferEncoding_enum contentTransferEncoding = this.ContentTransferEncoding;
					this.DataEncoded = this.EncodeData(value, contentTransferEncoding);
				}
			}
		}

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0005AAF0 File Offset: 0x00059AF0
		// (set) Token: 0x06000E9D RID: 3741 RVA: 0x0005AB8C File Offset: 0x00059B8C
		public string DataText
		{
			get
			{
				bool flag = (this.ContentType & MediaType_enum.Text) == (MediaType_enum)0 && (this.ContentType & MediaType_enum.NotSpecified) == (MediaType_enum)0;
				if (flag)
				{
					throw new Exception("This property is available only if ContentType is Text/xxx... !");
				}
				string @string;
				try
				{
					string contentType_CharSet = this.ContentType_CharSet;
					bool flag2 = contentType_CharSet == null;
					if (flag2)
					{
						@string = Encoding.Default.GetString(this.Data);
					}
					else
					{
						@string = Encoding.GetEncoding(contentType_CharSet).GetString(this.Data);
					}
				}
				catch
				{
					@string = Encoding.Default.GetString(this.Data);
				}
				return @string;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.DataEncoded = null;
				}
				else
				{
					string contentType_CharSet = this.ContentType_CharSet;
					bool flag2 = contentType_CharSet == null;
					if (flag2)
					{
						throw new Exception("Please specify CharSet property first !");
					}
					Encoding encoding = null;
					try
					{
						encoding = Encoding.GetEncoding(contentType_CharSet);
					}
					catch
					{
						throw new Exception("Not supported charset '" + contentType_CharSet + "' ! If you need to use this charset, then set data through Data or DataEncoded property.");
					}
					this.Data = encoding.GetBytes(value);
				}
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06000E9E RID: 3742 RVA: 0x0005AC0C File Offset: 0x00059C0C
		// (set) Token: 0x06000E9F RID: 3743 RVA: 0x0005AC24 File Offset: 0x00059C24
		public byte[] DataEncoded
		{
			get
			{
				return this.m_EncodedData;
			}
			set
			{
				this.m_EncodedData = value;
			}
		}

		// Token: 0x04000610 RID: 1552
		private HeaderFieldCollection m_pHeader = null;

		// Token: 0x04000611 RID: 1553
		private MimeEntity m_pParentEntity = null;

		// Token: 0x04000612 RID: 1554
		private MimeEntityCollection m_pChildEntities = null;

		// Token: 0x04000613 RID: 1555
		private byte[] m_EncodedData = null;

		// Token: 0x04000614 RID: 1556
		private Hashtable m_pHeaderFieldCache = null;
	}
}
