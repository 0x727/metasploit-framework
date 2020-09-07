using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.Net.IMAP.Server;
using LumiSoft.Net.IO;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001A0 RID: 416
	public class IMAP_r_u_Fetch : IMAP_r_u
	{
		// Token: 0x06001084 RID: 4228 RVA: 0x000662C0 File Offset: 0x000652C0
		public IMAP_r_u_Fetch(int msgSeqNo, IMAP_t_Fetch_r_i[] dataItems)
		{
			bool flag = msgSeqNo < 1;
			if (flag)
			{
				throw new ArgumentException("Argument 'msgSeqNo' value must be >= 1.", "msgSeqNo");
			}
			bool flag2 = dataItems == null;
			if (flag2)
			{
				throw new ArgumentNullException("dataItems");
			}
			this.m_MsgSeqNo = msgSeqNo;
			this.m_pDataItems = new List<IMAP_t_Fetch_r_i>();
			this.m_pDataItems.AddRange(dataItems);
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x00066330 File Offset: 0x00065330
		internal IMAP_r_u_Fetch(int msgSeqNo)
		{
			bool flag = msgSeqNo < 1;
			if (flag)
			{
				throw new ArgumentException("Argument 'msgSeqNo' value must be >= 1.", "msgSeqNo");
			}
			this.m_MsgSeqNo = msgSeqNo;
			this.m_pDataItems = new List<IMAP_t_Fetch_r_i>();
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x00066380 File Offset: 0x00065380
		internal void ParseAsync(IMAP_Client imap, string line, EventHandler<EventArgs<Exception>> callback)
		{
			bool flag = imap == null;
			if (flag)
			{
				throw new ArgumentNullException("imap");
			}
			bool flag2 = line == null;
			if (flag2)
			{
				throw new ArgumentNullException("line");
			}
			bool flag3 = callback == null;
			if (flag3)
			{
				throw new ArgumentNullException("callback");
			}
			StringReader stringReader = new StringReader(line);
			stringReader.ReadWord();
			this.m_MsgSeqNo = Convert.ToInt32(stringReader.ReadWord());
			stringReader.ReadWord();
			stringReader.ReadToFirstChar();
			bool flag4 = stringReader.StartsWith("(");
			if (flag4)
			{
				stringReader.ReadSpecifiedLength(1);
			}
			this.ParseDataItems(imap, stringReader, callback);
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0006641C File Offset: 0x0006541C
		protected override bool ToStreamAsync(IMAP_Session session, Stream stream, IMAP_Mailbox_Encoding mailboxEncoding, EventHandler<EventArgs<Exception>> completedAsyncCallback)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("* " + this.m_MsgSeqNo + " FETCH (");
			for (int i = 0; i < this.m_pDataItems.Count; i++)
			{
				IMAP_t_Fetch_r_i imap_t_Fetch_r_i = this.m_pDataItems[i];
				bool flag2 = i > 0;
				if (flag2)
				{
					stringBuilder.Append(" ");
				}
				bool flag3 = imap_t_Fetch_r_i is IMAP_t_Fetch_r_i_Flags;
				if (flag3)
				{
					stringBuilder.Append("FLAGS (" + ((IMAP_t_Fetch_r_i_Flags)imap_t_Fetch_r_i).Flags.ToString() + ")");
				}
				else
				{
					bool flag4 = imap_t_Fetch_r_i is IMAP_t_Fetch_r_i_Uid;
					if (!flag4)
					{
						throw new NotImplementedException("Fetch response data-item '" + imap_t_Fetch_r_i.ToString() + "' not implemented.");
					}
					stringBuilder.Append("UID " + ((IMAP_t_Fetch_r_i_Uid)imap_t_Fetch_r_i).UID.ToString());
				}
			}
			stringBuilder.Append(")\r\n");
			string text = stringBuilder.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			bool flag5 = session != null;
			if (flag5)
			{
				session.LogAddWrite((long)bytes.Length, text.TrimEnd(new char[0]));
			}
			IAsyncResult asyncResult = stream.BeginWrite(bytes, 0, bytes.Length, delegate(IAsyncResult r)
			{
				bool completedSynchronously2 = r.CompletedSynchronously;
				if (!completedSynchronously2)
				{
					try
					{
						stream.EndWrite(r);
						bool flag6 = completedAsyncCallback != null;
						if (flag6)
						{
							completedAsyncCallback(this, new EventArgs<Exception>(null));
						}
					}
					catch (Exception value)
					{
						bool flag7 = completedAsyncCallback != null;
						if (flag7)
						{
							completedAsyncCallback(this, new EventArgs<Exception>(value));
						}
					}
				}
			}, null);
			bool completedSynchronously = asyncResult.CompletedSynchronously;
			bool result;
			if (completedSynchronously)
			{
				stream.EndWrite(asyncResult);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000665F4 File Offset: 0x000655F4
		private void ParseDataItems(IMAP_Client imap, StringReader r, EventHandler<EventArgs<Exception>> callback)
		{
			bool flag = imap == null;
			if (flag)
			{
				throw new ArgumentNullException("imap");
			}
			bool flag2 = r == null;
			if (flag2)
			{
				throw new ArgumentNullException("r");
			}
			bool flag3 = callback == null;
			if (flag3)
			{
				throw new ArgumentNullException("callback");
			}
			for (;;)
			{
				r.ReadToFirstChar();
				bool flag4 = r.StartsWith("BODY[", false);
				if (flag4)
				{
					r.ReadWord();
					string section = r.ReadParenthesized();
					int offset = -1;
					bool flag5 = r.StartsWith("<");
					if (flag5)
					{
						offset = Convert.ToInt32(r.ReadParenthesized().Split(new char[]
						{
							' '
						})[0]);
					}
					IMAP_t_Fetch_r_i_Body imap_t_Fetch_r_i_Body = new IMAP_t_Fetch_r_i_Body(section, offset, new MemoryStreamEx(32000));
					this.m_pDataItems.Add(imap_t_Fetch_r_i_Body);
					IMAP_Client_e_FetchGetStoreStream imap_Client_e_FetchGetStoreStream = new IMAP_Client_e_FetchGetStoreStream(this, imap_t_Fetch_r_i_Body);
					imap.OnFetchGetStoreStream(imap_Client_e_FetchGetStoreStream);
					bool flag6 = imap_Client_e_FetchGetStoreStream.Stream != null;
					if (flag6)
					{
						imap_t_Fetch_r_i_Body.Stream.Dispose();
						imap_t_Fetch_r_i_Body.SetStream(imap_Client_e_FetchGetStoreStream.Stream);
					}
					bool flag7 = this.ReadData(imap, r, callback, imap_t_Fetch_r_i_Body.Stream);
					if (flag7)
					{
						break;
					}
				}
				else
				{
					bool flag8 = r.StartsWith("BODY ", false);
					if (flag8)
					{
						string source = null;
						for (;;)
						{
							StringReader stringReader = new StringReader(r.SourceString);
							stringReader.ReadWord();
							stringReader.ReadToFirstChar();
							try
							{
								source = stringReader.ReadParenthesized();
								r = stringReader;
								break;
							}
							catch
							{
								bool flag9 = this.ReadStringLiteral(imap, r, callback);
								if (flag9)
								{
									return;
								}
							}
						}
						this.m_pDataItems.Add(IMAP_t_Fetch_r_i_BodyStructure.Parse(new StringReader(source)));
					}
					else
					{
						bool flag10 = r.StartsWith("BODYSTRUCTURE", false);
						if (flag10)
						{
							string source2 = null;
							for (;;)
							{
								StringReader stringReader2 = new StringReader(r.SourceString);
								stringReader2.ReadWord();
								stringReader2.ReadToFirstChar();
								try
								{
									source2 = stringReader2.ReadParenthesized();
									r = stringReader2;
									break;
								}
								catch
								{
									bool flag11 = this.ReadStringLiteral(imap, r, callback);
									if (flag11)
									{
										return;
									}
								}
							}
							this.m_pDataItems.Add(IMAP_t_Fetch_r_i_BodyStructure.Parse(new StringReader(source2)));
						}
						else
						{
							bool flag12 = r.StartsWith("ENVELOPE", false);
							if (flag12)
							{
								string source3 = null;
								for (;;)
								{
									StringReader stringReader3 = new StringReader(r.SourceString);
									stringReader3.ReadWord();
									stringReader3.ReadToFirstChar();
									try
									{
										source3 = stringReader3.ReadParenthesized();
										r = stringReader3;
										break;
									}
									catch
									{
										bool flag13 = this.ReadStringLiteral(imap, r, callback);
										if (flag13)
										{
											return;
										}
									}
								}
								this.m_pDataItems.Add(IMAP_t_Fetch_r_i_Envelope.Parse(new StringReader(source3)));
							}
							else
							{
								bool flag14 = r.StartsWith("FLAGS", false);
								if (flag14)
								{
									r.ReadWord();
									this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_Flags(IMAP_t_MsgFlags.Parse(r.ReadParenthesized())));
								}
								else
								{
									bool flag15 = r.StartsWith("INTERNALDATE", false);
									if (flag15)
									{
										r.ReadWord();
										this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_InternalDate(IMAP_Utils.ParseDate(r.ReadWord())));
									}
									else
									{
										bool flag16 = r.StartsWith("RFC822 ", false);
										if (flag16)
										{
											r.ReadWord();
											r.ReadToFirstChar();
											IMAP_t_Fetch_r_i_Rfc822 imap_t_Fetch_r_i_Rfc = new IMAP_t_Fetch_r_i_Rfc822(new MemoryStreamEx(32000));
											this.m_pDataItems.Add(imap_t_Fetch_r_i_Rfc);
											IMAP_Client_e_FetchGetStoreStream imap_Client_e_FetchGetStoreStream2 = new IMAP_Client_e_FetchGetStoreStream(this, imap_t_Fetch_r_i_Rfc);
											imap.OnFetchGetStoreStream(imap_Client_e_FetchGetStoreStream2);
											bool flag17 = imap_Client_e_FetchGetStoreStream2.Stream != null;
											if (flag17)
											{
												imap_t_Fetch_r_i_Rfc.Stream.Dispose();
												imap_t_Fetch_r_i_Rfc.SetStream(imap_Client_e_FetchGetStoreStream2.Stream);
											}
											bool flag18 = this.ReadData(imap, r, callback, imap_t_Fetch_r_i_Rfc.Stream);
											if (flag18)
											{
												break;
											}
										}
										else
										{
											bool flag19 = r.StartsWith("RFC822.HEADER", false);
											if (flag19)
											{
												r.ReadWord();
												r.ReadToFirstChar();
												IMAP_t_Fetch_r_i_Rfc822Header imap_t_Fetch_r_i_Rfc822Header = new IMAP_t_Fetch_r_i_Rfc822Header(new MemoryStreamEx(32000));
												this.m_pDataItems.Add(imap_t_Fetch_r_i_Rfc822Header);
												IMAP_Client_e_FetchGetStoreStream imap_Client_e_FetchGetStoreStream3 = new IMAP_Client_e_FetchGetStoreStream(this, imap_t_Fetch_r_i_Rfc822Header);
												imap.OnFetchGetStoreStream(imap_Client_e_FetchGetStoreStream3);
												bool flag20 = imap_Client_e_FetchGetStoreStream3.Stream != null;
												if (flag20)
												{
													imap_t_Fetch_r_i_Rfc822Header.Stream.Dispose();
													imap_t_Fetch_r_i_Rfc822Header.SetStream(imap_Client_e_FetchGetStoreStream3.Stream);
												}
												bool flag21 = this.ReadData(imap, r, callback, imap_t_Fetch_r_i_Rfc822Header.Stream);
												if (flag21)
												{
													break;
												}
											}
											else
											{
												bool flag22 = r.StartsWith("RFC822.SIZE", false);
												if (flag22)
												{
													r.ReadWord();
													this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_Rfc822Size(Convert.ToInt32(r.ReadWord())));
												}
												else
												{
													bool flag23 = r.StartsWith("RFC822.TEXT", false);
													if (flag23)
													{
														r.ReadWord();
														r.ReadToFirstChar();
														IMAP_t_Fetch_r_i_Rfc822Text imap_t_Fetch_r_i_Rfc822Text = new IMAP_t_Fetch_r_i_Rfc822Text(new MemoryStreamEx(32000));
														this.m_pDataItems.Add(imap_t_Fetch_r_i_Rfc822Text);
														IMAP_Client_e_FetchGetStoreStream imap_Client_e_FetchGetStoreStream4 = new IMAP_Client_e_FetchGetStoreStream(this, imap_t_Fetch_r_i_Rfc822Text);
														imap.OnFetchGetStoreStream(imap_Client_e_FetchGetStoreStream4);
														bool flag24 = imap_Client_e_FetchGetStoreStream4.Stream != null;
														if (flag24)
														{
															imap_t_Fetch_r_i_Rfc822Text.Stream.Dispose();
															imap_t_Fetch_r_i_Rfc822Text.SetStream(imap_Client_e_FetchGetStoreStream4.Stream);
														}
														bool flag25 = this.ReadData(imap, r, callback, imap_t_Fetch_r_i_Rfc822Text.Stream);
														if (flag25)
														{
															break;
														}
													}
													else
													{
														bool flag26 = r.StartsWith("UID", false);
														if (flag26)
														{
															r.ReadWord();
															this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_Uid(Convert.ToInt64(r.ReadWord())));
														}
														else
														{
															bool flag27 = r.StartsWith("X-GM-MSGID", false);
															if (flag27)
															{
																r.ReadWord();
																this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_X_GM_MSGID(Convert.ToUInt64(r.ReadWord())));
															}
															else
															{
																bool flag28 = r.StartsWith("X-GM-THRID", false);
																if (!flag28)
																{
																	goto IL_608;
																}
																r.ReadWord();
																this.m_pDataItems.Add(new IMAP_t_Fetch_r_i_X_GM_THRID(Convert.ToUInt64(r.ReadWord())));
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
			return;
			IL_608:
			bool flag29 = r.StartsWith(")", false);
			if (!flag29)
			{
				throw new ParseException("Not supported FETCH data-item '" + r.ReadToEnd() + "'.");
			}
			callback(this, new EventArgs<Exception>(null));
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00066C7C File Offset: 0x00065C7C
		private bool ReadStringLiteral(IMAP_Client imap, StringReader r, EventHandler<EventArgs<Exception>> callback)
		{
			bool flag = imap == null;
			if (flag)
			{
				throw new ArgumentNullException("imap");
			}
			bool flag2 = r == null;
			if (flag2)
			{
				throw new ArgumentNullException("r");
			}
			bool flag3 = callback == null;
			if (flag3)
			{
				throw new ArgumentNullException("callback");
			}
			bool flag4 = r.SourceString.EndsWith("}") && r.SourceString.IndexOf("{") > -1;
			if (flag4)
			{
				MemoryStream stream = new MemoryStream();
				string value = r.SourceString.Substring(r.SourceString.LastIndexOf("{") + 1, r.SourceString.Length - r.SourceString.LastIndexOf("{") - 2);
				r.RemoveFromEnd(r.SourceString.Length - r.SourceString.LastIndexOf('{'));
				IMAP_Client.ReadStringLiteralAsyncOP op = new IMAP_Client.ReadStringLiteralAsyncOP(stream, Convert.ToInt32(value));
				op.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadStringLiteralAsyncOP> e)
				{
					try
					{
						bool flag7 = op.Error != null;
						if (flag7)
						{
							callback(this, new EventArgs<Exception>(op.Error));
						}
						else
						{
							r.AppendString(TextUtils.QuoteString(Encoding.UTF8.GetString(stream.ToArray())));
							bool flag8 = !this.ReadNextFetchLine(imap, r, callback);
							if (flag8)
							{
								this.ParseDataItems(imap, r, callback);
							}
						}
					}
					catch (Exception value2)
					{
						callback(this, new EventArgs<Exception>(value2));
					}
					finally
					{
						op.Dispose();
					}
				};
				bool flag5 = !imap.ReadStringLiteralAsync(op);
				if (flag5)
				{
					try
					{
						bool flag6 = op.Error != null;
						if (flag6)
						{
							callback(this, new EventArgs<Exception>(op.Error));
							return true;
						}
						r.AppendString(TextUtils.QuoteString(Encoding.UTF8.GetString(stream.ToArray())));
						return this.ReadNextFetchLine(imap, r, callback);
					}
					finally
					{
						op.Dispose();
					}
				}
				return true;
			}
			throw new ParseException("No string-literal available '" + r.SourceString + "'.");
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x00066F3C File Offset: 0x00065F3C
		private bool ReadData(IMAP_Client imap, StringReader r, EventHandler<EventArgs<Exception>> callback, Stream stream)
		{
			bool flag = imap == null;
			if (flag)
			{
				throw new ArgumentNullException("imap");
			}
			bool flag2 = r == null;
			if (flag2)
			{
				throw new ArgumentNullException("r");
			}
			bool flag3 = callback == null;
			if (flag3)
			{
				throw new ArgumentNullException("callback");
			}
			bool flag4 = stream == null;
			if (flag4)
			{
				throw new ArgumentNullException("stream");
			}
			r.ReadToFirstChar();
			bool flag5 = r.StartsWith("NIL", false);
			bool result;
			if (flag5)
			{
				r.ReadWord();
				result = false;
			}
			else
			{
				bool flag6 = r.StartsWith("{", false);
				if (flag6)
				{
					IMAP_Client.ReadStringLiteralAsyncOP op = new IMAP_Client.ReadStringLiteralAsyncOP(stream, Convert.ToInt32(r.ReadParenthesized()));
					op.CompletedAsync += delegate(object sender, EventArgs<IMAP_Client.ReadStringLiteralAsyncOP> e)
					{
						try
						{
							bool flag10 = op.Error != null;
							if (flag10)
							{
								callback(this, new EventArgs<Exception>(op.Error));
							}
							else
							{
								bool flag11 = !this.ReadNextFetchLine(imap, r, callback);
								if (flag11)
								{
									this.ParseDataItems(imap, r, callback);
								}
							}
						}
						catch (Exception value)
						{
							callback(this, new EventArgs<Exception>(value));
						}
						finally
						{
							op.Dispose();
						}
					};
					bool flag7 = !imap.ReadStringLiteralAsync(op);
					if (flag7)
					{
						try
						{
							bool flag8 = op.Error != null;
							if (flag8)
							{
								callback(this, new EventArgs<Exception>(op.Error));
								return true;
							}
							bool flag9 = !this.ReadNextFetchLine(imap, r, callback);
							if (flag9)
							{
								return false;
							}
							return true;
						}
						finally
						{
							op.Dispose();
						}
					}
					result = true;
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(r.ReadWord());
					stream.Write(bytes, 0, bytes.Length);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0006716C File Offset: 0x0006616C
		private bool ReadNextFetchLine(IMAP_Client imap, StringReader r, EventHandler<EventArgs<Exception>> callback)
		{
			bool flag = imap == null;
			if (flag)
			{
				throw new ArgumentNullException("imap");
			}
			bool flag2 = r == null;
			if (flag2)
			{
				throw new ArgumentNullException("r");
			}
			bool flag3 = callback == null;
			if (flag3)
			{
				throw new ArgumentNullException("callback");
			}
			SmartStream.ReadLineAsyncOP readLineOP = new SmartStream.ReadLineAsyncOP(new byte[64000], SizeExceededAction.JunkAndThrowException);
			readLineOP.CompletedAsync += delegate(object sender, EventArgs<SmartStream.ReadLineAsyncOP> e)
			{
				try
				{
					bool flag6 = readLineOP.Error != null;
					if (flag6)
					{
						callback(this, new EventArgs<Exception>(readLineOP.Error));
					}
					else
					{
						imap.LogAddRead((long)readLineOP.BytesInBuffer, readLineOP.LineUtf8);
						r.AppendString(readLineOP.LineUtf8);
						this.ParseDataItems(imap, r, callback);
					}
				}
				catch (Exception value)
				{
					callback(this, new EventArgs<Exception>(value));
				}
				finally
				{
					readLineOP.Dispose();
				}
			};
			bool flag4 = imap.TcpStream.ReadLine(readLineOP, true);
			if (flag4)
			{
				try
				{
					bool flag5 = readLineOP.Error != null;
					if (flag5)
					{
						callback(this, new EventArgs<Exception>(readLineOP.Error));
						return true;
					}
					imap.LogAddRead((long)readLineOP.BytesInBuffer, readLineOP.LineUtf8);
					r.AppendString(readLineOP.LineUtf8);
					return false;
				}
				finally
				{
					readLineOP.Dispose();
				}
			}
			return true;
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x000672D4 File Offset: 0x000662D4
		private IMAP_t_Fetch_r_i FilterDataItem(Type dataItem)
		{
			bool flag = dataItem == null;
			if (flag)
			{
				throw new ArgumentNullException("dataItem");
			}
			foreach (IMAP_t_Fetch_r_i imap_t_Fetch_r_i in this.m_pDataItems)
			{
				bool flag2 = imap_t_Fetch_r_i.GetType() == dataItem;
				if (flag2)
				{
					return imap_t_Fetch_r_i;
				}
			}
			return null;
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x0600108D RID: 4237 RVA: 0x0006735C File Offset: 0x0006635C
		public int SeqNo
		{
			get
			{
				return this.m_MsgSeqNo;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x00067374 File Offset: 0x00066374
		public IMAP_t_Fetch_r_i[] DataItems
		{
			get
			{
				return this.m_pDataItems.ToArray();
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x00067394 File Offset: 0x00066394
		public IMAP_t_Fetch_r_i_Body[] Body
		{
			get
			{
				List<IMAP_t_Fetch_r_i_Body> list = new List<IMAP_t_Fetch_r_i_Body>();
				foreach (IMAP_t_Fetch_r_i imap_t_Fetch_r_i in this.m_pDataItems)
				{
					bool flag = imap_t_Fetch_r_i is IMAP_t_Fetch_r_i_Body;
					if (flag)
					{
						list.Add((IMAP_t_Fetch_r_i_Body)imap_t_Fetch_r_i);
					}
				}
				return list.ToArray();
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x00067414 File Offset: 0x00066414
		public IMAP_t_Fetch_r_i_BodyStructure BodyStructure
		{
			get
			{
				return (IMAP_t_Fetch_r_i_BodyStructure)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_BodyStructure));
			}
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x06001091 RID: 4241 RVA: 0x0006743C File Offset: 0x0006643C
		public IMAP_t_Fetch_r_i_Envelope Envelope
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Envelope)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Envelope));
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x00067464 File Offset: 0x00066464
		public IMAP_t_Fetch_r_i_Flags Flags
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Flags)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Flags));
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x06001093 RID: 4243 RVA: 0x0006748C File Offset: 0x0006648C
		public IMAP_t_Fetch_r_i_InternalDate InternalDate
		{
			get
			{
				return (IMAP_t_Fetch_r_i_InternalDate)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_InternalDate));
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x000674B4 File Offset: 0x000664B4
		public IMAP_t_Fetch_r_i_Rfc822 Rfc822
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Rfc822)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Rfc822));
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x06001095 RID: 4245 RVA: 0x000674DC File Offset: 0x000664DC
		public IMAP_t_Fetch_r_i_Rfc822Header Rfc822Header
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Rfc822Header)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Rfc822Header));
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x00067504 File Offset: 0x00066504
		public IMAP_t_Fetch_r_i_Rfc822Size Rfc822Size
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Rfc822Size)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Rfc822Size));
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x06001097 RID: 4247 RVA: 0x0006752C File Offset: 0x0006652C
		public IMAP_t_Fetch_r_i_Rfc822Text Rfc822Text
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Rfc822Text)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Rfc822Text));
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06001098 RID: 4248 RVA: 0x00067554 File Offset: 0x00066554
		public IMAP_t_Fetch_r_i_Uid UID
		{
			get
			{
				return (IMAP_t_Fetch_r_i_Uid)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_Uid));
			}
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x06001099 RID: 4249 RVA: 0x0006757C File Offset: 0x0006657C
		public IMAP_t_Fetch_r_i_X_GM_MSGID X_GM_MSGID
		{
			get
			{
				return (IMAP_t_Fetch_r_i_X_GM_MSGID)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_X_GM_MSGID));
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x0600109A RID: 4250 RVA: 0x000675A4 File Offset: 0x000665A4
		public IMAP_t_Fetch_r_i_X_GM_THRID X_GM_THRID
		{
			get
			{
				return (IMAP_t_Fetch_r_i_X_GM_THRID)this.FilterDataItem(typeof(IMAP_t_Fetch_r_i_X_GM_THRID));
			}
		}

		// Token: 0x040006A7 RID: 1703
		private int m_MsgSeqNo = 0;

		// Token: 0x040006A8 RID: 1704
		private List<IMAP_t_Fetch_r_i> m_pDataItems = null;
	}
}
