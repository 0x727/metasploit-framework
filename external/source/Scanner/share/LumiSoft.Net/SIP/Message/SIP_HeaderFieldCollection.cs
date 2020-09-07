using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
	// Token: 0x0200007B RID: 123
	public class SIP_HeaderFieldCollection : IEnumerable
	{
		// Token: 0x06000472 RID: 1138 RVA: 0x00015B04 File Offset: 0x00014B04
		public SIP_HeaderFieldCollection()
		{
			this.m_pHeaderFields = new List<SIP_HeaderField>();
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00015B20 File Offset: 0x00014B20
		public void Add(string fieldName, string value)
		{
			this.Add(this.GetheaderField(fieldName, value));
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00015B32 File Offset: 0x00014B32
		public void Add(SIP_HeaderField headerField)
		{
			this.m_pHeaderFields.Add(headerField);
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x00015B42 File Offset: 0x00014B42
		public void Insert(int index, string fieldName, string value)
		{
			this.m_pHeaderFields.Insert(index, this.GetheaderField(fieldName, value));
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00015B5C File Offset: 0x00014B5C
		public void Set(string fieldName, string value)
		{
			SIP_HeaderField first = this.GetFirst(fieldName);
			bool flag = first != null;
			if (flag)
			{
				first.Value = value;
			}
			else
			{
				this.Add(this.GetheaderField(fieldName, value));
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00015B97 File Offset: 0x00014B97
		public void Remove(int index)
		{
			this.m_pHeaderFields.RemoveAt(index);
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00015BA7 File Offset: 0x00014BA7
		public void Remove(SIP_HeaderField field)
		{
			this.m_pHeaderFields.Remove(field);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00015BB8 File Offset: 0x00014BB8
		public void RemoveFirst(string name)
		{
			foreach (SIP_HeaderField sip_HeaderField in this.m_pHeaderFields)
			{
				bool flag = sip_HeaderField.Name.ToLower() == name.ToLower();
				if (flag)
				{
					this.m_pHeaderFields.Remove(sip_HeaderField);
					break;
				}
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00015C34 File Offset: 0x00014C34
		public void RemoveAll(string fieldName)
		{
			for (int i = 0; i < this.m_pHeaderFields.Count; i++)
			{
				SIP_HeaderField sip_HeaderField = this.m_pHeaderFields[i];
				bool flag = sip_HeaderField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					this.m_pHeaderFields.Remove(sip_HeaderField);
					i--;
				}
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00015C98 File Offset: 0x00014C98
		public void Clear()
		{
			this.m_pHeaderFields.Clear();
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00015CA8 File Offset: 0x00014CA8
		public bool Contains(string fieldName)
		{
			foreach (SIP_HeaderField sip_HeaderField in this.m_pHeaderFields)
			{
				bool flag = sip_HeaderField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00015D20 File Offset: 0x00014D20
		public bool Contains(SIP_HeaderField headerField)
		{
			return this.m_pHeaderFields.Contains(headerField);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00015D40 File Offset: 0x00014D40
		public SIP_HeaderField GetFirst(string fieldName)
		{
			foreach (SIP_HeaderField sip_HeaderField in this.m_pHeaderFields)
			{
				bool flag = sip_HeaderField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					return sip_HeaderField;
				}
			}
			return null;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00015DB8 File Offset: 0x00014DB8
		public SIP_HeaderField[] Get(string fieldName)
		{
			List<SIP_HeaderField> list = new List<SIP_HeaderField>();
			foreach (SIP_HeaderField sip_HeaderField in this.m_pHeaderFields)
			{
				bool flag = sip_HeaderField.Name.ToLower() == fieldName.ToLower();
				if (flag)
				{
					list.Add(sip_HeaderField);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00015E40 File Offset: 0x00014E40
		public void Parse(string headerString)
		{
			this.Parse(new MemoryStream(Encoding.Default.GetBytes(headerString)));
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00015E5C File Offset: 0x00014E5C
		public void Parse(Stream stream)
		{
			this.m_pHeaderFields.Clear();
			StreamLineReader streamLineReader = new StreamLineReader(stream);
			streamLineReader.CRLF_LinesOnly = false;
			string text = streamLineReader.ReadLineString();
			while (text != null)
			{
				bool flag = text == "";
				if (flag)
				{
					break;
				}
				string text2 = text;
				text = streamLineReader.ReadLineString();
				while (text != null && (text.StartsWith("\t") || text.StartsWith(" ")))
				{
					text2 += text;
					text = streamLineReader.ReadLineString();
				}
				string[] array = text2.Split(new char[]
				{
					':'
				}, 2);
				bool flag2 = array.Length == 2;
				if (flag2)
				{
					this.Add(array[0] + ":", array[1].Trim());
				}
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00015F38 File Offset: 0x00014F38
		public string ToHeaderString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in this)
			{
				SIP_HeaderField sip_HeaderField = (SIP_HeaderField)obj;
				stringBuilder.Append(sip_HeaderField.Name + " " + sip_HeaderField.Value + "\r\n");
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00015FCC File Offset: 0x00014FCC
		private SIP_HeaderField GetheaderField(string name, string value)
		{
			name = name.Replace(":", "").Trim();
			bool flag = string.Equals(name, "i", StringComparison.InvariantCultureIgnoreCase);
			if (flag)
			{
				name = "Call-ID";
			}
			else
			{
				bool flag2 = string.Equals(name, "m", StringComparison.InvariantCultureIgnoreCase);
				if (flag2)
				{
					name = "Contact";
				}
				else
				{
					bool flag3 = string.Equals(name, "e", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						name = "Content-Encoding";
					}
					else
					{
						bool flag4 = string.Equals(name, "l", StringComparison.InvariantCultureIgnoreCase);
						if (flag4)
						{
							name = "Content-Length";
						}
						else
						{
							bool flag5 = string.Equals(name, "c", StringComparison.InvariantCultureIgnoreCase);
							if (flag5)
							{
								name = "Content-Type";
							}
							else
							{
								bool flag6 = string.Equals(name, "f", StringComparison.InvariantCultureIgnoreCase);
								if (flag6)
								{
									name = "From";
								}
								else
								{
									bool flag7 = string.Equals(name, "s", StringComparison.InvariantCultureIgnoreCase);
									if (flag7)
									{
										name = "Subject";
									}
									else
									{
										bool flag8 = string.Equals(name, "k", StringComparison.InvariantCultureIgnoreCase);
										if (flag8)
										{
											name = "Supported";
										}
										else
										{
											bool flag9 = string.Equals(name, "t", StringComparison.InvariantCultureIgnoreCase);
											if (flag9)
											{
												name = "To";
											}
											else
											{
												bool flag10 = string.Equals(name, "v", StringComparison.InvariantCultureIgnoreCase);
												if (flag10)
												{
													name = "Via";
												}
												else
												{
													bool flag11 = string.Equals(name, "u", StringComparison.InvariantCultureIgnoreCase);
													if (flag11)
													{
														name = "AllowEevents";
													}
													else
													{
														bool flag12 = string.Equals(name, "r", StringComparison.InvariantCultureIgnoreCase);
														if (flag12)
														{
															name = "Refer-To";
														}
														else
														{
															bool flag13 = string.Equals(name, "d", StringComparison.InvariantCultureIgnoreCase);
															if (flag13)
															{
																name = "Request-Disposition";
															}
															else
															{
																bool flag14 = string.Equals(name, "x", StringComparison.InvariantCultureIgnoreCase);
																if (flag14)
																{
																	name = "Session-Expires";
																}
																else
																{
																	bool flag15 = string.Equals(name, "o", StringComparison.InvariantCultureIgnoreCase);
																	if (flag15)
																	{
																		name = "Event";
																	}
																	else
																	{
																		bool flag16 = string.Equals(name, "b", StringComparison.InvariantCultureIgnoreCase);
																		if (flag16)
																		{
																			name = "Referred-By";
																		}
																		else
																		{
																			bool flag17 = string.Equals(name, "a", StringComparison.InvariantCultureIgnoreCase);
																			if (flag17)
																			{
																				name = "Accept-Contact";
																			}
																			else
																			{
																				bool flag18 = string.Equals(name, "y", StringComparison.InvariantCultureIgnoreCase);
																				if (flag18)
																				{
																					name = "Identity";
																				}
																				else
																				{
																					bool flag19 = string.Equals(name, "n", StringComparison.InvariantCultureIgnoreCase);
																					if (flag19)
																					{
																						name = "Identity-Info";
																					}
																					else
																					{
																						bool flag20 = string.Equals(name, "j", StringComparison.InvariantCultureIgnoreCase);
																						if (flag20)
																						{
																							name = "Reject-Contact";
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
			bool flag21 = string.Equals(name, "accept", StringComparison.InvariantCultureIgnoreCase);
			SIP_HeaderField result;
			if (flag21)
			{
				result = new SIP_MultiValueHF<SIP_t_AcceptRange>("Accept:", value);
			}
			else
			{
				bool flag22 = string.Equals(name, "accept-contact", StringComparison.InvariantCultureIgnoreCase);
				if (flag22)
				{
					result = new SIP_MultiValueHF<SIP_t_ACValue>("Accept-Contact:", value);
				}
				else
				{
					bool flag23 = string.Equals(name, "accept-encoding", StringComparison.InvariantCultureIgnoreCase);
					if (flag23)
					{
						result = new SIP_MultiValueHF<SIP_t_Encoding>("Accept-Encoding:", value);
					}
					else
					{
						bool flag24 = string.Equals(name, "accept-language", StringComparison.InvariantCultureIgnoreCase);
						if (flag24)
						{
							result = new SIP_MultiValueHF<SIP_t_Language>("Accept-Language:", value);
						}
						else
						{
							bool flag25 = string.Equals(name, "accept-resource-priority", StringComparison.InvariantCultureIgnoreCase);
							if (flag25)
							{
								result = new SIP_MultiValueHF<SIP_t_RValue>("Accept-Resource-Priority:", value);
							}
							else
							{
								bool flag26 = string.Equals(name, "alert-info", StringComparison.InvariantCultureIgnoreCase);
								if (flag26)
								{
									result = new SIP_MultiValueHF<SIP_t_AlertParam>("Alert-Info:", value);
								}
								else
								{
									bool flag27 = string.Equals(name, "allow", StringComparison.InvariantCultureIgnoreCase);
									if (flag27)
									{
										result = new SIP_MultiValueHF<SIP_t_Method>("Allow:", value);
									}
									else
									{
										bool flag28 = string.Equals(name, "allow-events", StringComparison.InvariantCultureIgnoreCase);
										if (flag28)
										{
											result = new SIP_MultiValueHF<SIP_t_EventType>("Allow-Events:", value);
										}
										else
										{
											bool flag29 = string.Equals(name, "authentication-info", StringComparison.InvariantCultureIgnoreCase);
											if (flag29)
											{
												result = new SIP_SingleValueHF<SIP_t_AuthenticationInfo>("Authentication-Info:", new SIP_t_AuthenticationInfo(value));
											}
											else
											{
												bool flag30 = string.Equals(name, "authorization", StringComparison.InvariantCultureIgnoreCase);
												if (flag30)
												{
													result = new SIP_SingleValueHF<SIP_t_Credentials>("Authorization:", new SIP_t_Credentials(value));
												}
												else
												{
													bool flag31 = string.Equals(name, "contact", StringComparison.InvariantCultureIgnoreCase);
													if (flag31)
													{
														result = new SIP_MultiValueHF<SIP_t_ContactParam>("Contact:", value);
													}
													else
													{
														bool flag32 = string.Equals(name, "Content-Disposition", StringComparison.InvariantCultureIgnoreCase);
														if (flag32)
														{
															result = new SIP_SingleValueHF<SIP_t_ContentDisposition>("Content-Disposition:", new SIP_t_ContentDisposition(value));
														}
														else
														{
															bool flag33 = string.Equals(name, "cseq", StringComparison.InvariantCultureIgnoreCase);
															if (flag33)
															{
																result = new SIP_SingleValueHF<SIP_t_CSeq>("CSeq:", new SIP_t_CSeq(value));
															}
															else
															{
																bool flag34 = string.Equals(name, "content-encoding", StringComparison.InvariantCultureIgnoreCase);
																if (flag34)
																{
																	result = new SIP_MultiValueHF<SIP_t_ContentCoding>("Content-Encoding:", value);
																}
																else
																{
																	bool flag35 = string.Equals(name, "content-language", StringComparison.InvariantCultureIgnoreCase);
																	if (flag35)
																	{
																		result = new SIP_MultiValueHF<SIP_t_LanguageTag>("Content-Language:", value);
																	}
																	else
																	{
																		bool flag36 = string.Equals(name, "error-info", StringComparison.InvariantCultureIgnoreCase);
																		if (flag36)
																		{
																			result = new SIP_MultiValueHF<SIP_t_ErrorUri>("Error-Info:", value);
																		}
																		else
																		{
																			bool flag37 = string.Equals(name, "event", StringComparison.InvariantCultureIgnoreCase);
																			if (flag37)
																			{
																				result = new SIP_SingleValueHF<SIP_t_Event>("Event:", new SIP_t_Event(value));
																			}
																			else
																			{
																				bool flag38 = string.Equals(name, "from", StringComparison.InvariantCultureIgnoreCase);
																				if (flag38)
																				{
																					result = new SIP_SingleValueHF<SIP_t_From>("From:", new SIP_t_From(value));
																				}
																				else
																				{
																					bool flag39 = string.Equals(name, "history-info", StringComparison.InvariantCultureIgnoreCase);
																					if (flag39)
																					{
																						result = new SIP_MultiValueHF<SIP_t_HiEntry>("History-Info:", value);
																					}
																					else
																					{
																						bool flag40 = string.Equals(name, "identity-info", StringComparison.InvariantCultureIgnoreCase);
																						if (flag40)
																						{
																							result = new SIP_SingleValueHF<SIP_t_IdentityInfo>("Identity-Info:", new SIP_t_IdentityInfo(value));
																						}
																						else
																						{
																							bool flag41 = string.Equals(name, "in-replay-to", StringComparison.InvariantCultureIgnoreCase);
																							if (flag41)
																							{
																								result = new SIP_MultiValueHF<SIP_t_CallID>("In-Reply-To:", value);
																							}
																							else
																							{
																								bool flag42 = string.Equals(name, "join", StringComparison.InvariantCultureIgnoreCase);
																								if (flag42)
																								{
																									result = new SIP_SingleValueHF<SIP_t_Join>("Join:", new SIP_t_Join(value));
																								}
																								else
																								{
																									bool flag43 = string.Equals(name, "min-se", StringComparison.InvariantCultureIgnoreCase);
																									if (flag43)
																									{
																										result = new SIP_SingleValueHF<SIP_t_MinSE>("Min-SE:", new SIP_t_MinSE(value));
																									}
																									else
																									{
																										bool flag44 = string.Equals(name, "path", StringComparison.InvariantCultureIgnoreCase);
																										if (flag44)
																										{
																											result = new SIP_MultiValueHF<SIP_t_AddressParam>("Path:", value);
																										}
																										else
																										{
																											bool flag45 = string.Equals(name, "proxy-authenticate", StringComparison.InvariantCultureIgnoreCase);
																											if (flag45)
																											{
																												result = new SIP_SingleValueHF<SIP_t_Challenge>("Proxy-Authenticate:", new SIP_t_Challenge(value));
																											}
																											else
																											{
																												bool flag46 = string.Equals(name, "proxy-authorization", StringComparison.InvariantCultureIgnoreCase);
																												if (flag46)
																												{
																													result = new SIP_SingleValueHF<SIP_t_Credentials>("Proxy-Authorization:", new SIP_t_Credentials(value));
																												}
																												else
																												{
																													bool flag47 = string.Equals(name, "proxy-require", StringComparison.InvariantCultureIgnoreCase);
																													if (flag47)
																													{
																														result = new SIP_MultiValueHF<SIP_t_OptionTag>("Proxy-Require:", value);
																													}
																													else
																													{
																														bool flag48 = string.Equals(name, "rack", StringComparison.InvariantCultureIgnoreCase);
																														if (flag48)
																														{
																															result = new SIP_SingleValueHF<SIP_t_RAck>("RAck:", new SIP_t_RAck(value));
																														}
																														else
																														{
																															bool flag49 = string.Equals(name, "reason", StringComparison.InvariantCultureIgnoreCase);
																															if (flag49)
																															{
																																result = new SIP_MultiValueHF<SIP_t_ReasonValue>("Reason:", value);
																															}
																															else
																															{
																																bool flag50 = string.Equals(name, "record-route", StringComparison.InvariantCultureIgnoreCase);
																																if (flag50)
																																{
																																	result = new SIP_MultiValueHF<SIP_t_AddressParam>("Record-Route:", value);
																																}
																																else
																																{
																																	bool flag51 = string.Equals(name, "refer-sub", StringComparison.InvariantCultureIgnoreCase);
																																	if (flag51)
																																	{
																																		result = new SIP_SingleValueHF<SIP_t_ReferSub>("Refer-Sub:", new SIP_t_ReferSub(value));
																																	}
																																	else
																																	{
																																		bool flag52 = string.Equals(name, "refer-to", StringComparison.InvariantCultureIgnoreCase);
																																		if (flag52)
																																		{
																																			result = new SIP_SingleValueHF<SIP_t_AddressParam>("Refer-To:", new SIP_t_AddressParam(value));
																																		}
																																		else
																																		{
																																			bool flag53 = string.Equals(name, "referred-by", StringComparison.InvariantCultureIgnoreCase);
																																			if (flag53)
																																			{
																																				result = new SIP_SingleValueHF<SIP_t_ReferredBy>("Referred-By:", new SIP_t_ReferredBy(value));
																																			}
																																			else
																																			{
																																				bool flag54 = string.Equals(name, "reject-contact", StringComparison.InvariantCultureIgnoreCase);
																																				if (flag54)
																																				{
																																					result = new SIP_MultiValueHF<SIP_t_RCValue>("Reject-Contact:", value);
																																				}
																																				else
																																				{
																																					bool flag55 = string.Equals(name, "replaces", StringComparison.InvariantCultureIgnoreCase);
																																					if (flag55)
																																					{
																																						result = new SIP_SingleValueHF<SIP_t_SessionExpires>("Replaces:", new SIP_t_SessionExpires(value));
																																					}
																																					else
																																					{
																																						bool flag56 = string.Equals(name, "reply-to", StringComparison.InvariantCultureIgnoreCase);
																																						if (flag56)
																																						{
																																							result = new SIP_MultiValueHF<SIP_t_AddressParam>("Reply-To:", value);
																																						}
																																						else
																																						{
																																							bool flag57 = string.Equals(name, "request-disposition", StringComparison.InvariantCultureIgnoreCase);
																																							if (flag57)
																																							{
																																								result = new SIP_MultiValueHF<SIP_t_Directive>("Request-Disposition:", value);
																																							}
																																							else
																																							{
																																								bool flag58 = string.Equals(name, "require", StringComparison.InvariantCultureIgnoreCase);
																																								if (flag58)
																																								{
																																									result = new SIP_MultiValueHF<SIP_t_OptionTag>("Require:", value);
																																								}
																																								else
																																								{
																																									bool flag59 = string.Equals(name, "resource-priority", StringComparison.InvariantCultureIgnoreCase);
																																									if (flag59)
																																									{
																																										result = new SIP_MultiValueHF<SIP_t_RValue>("Resource-Priority:", value);
																																									}
																																									else
																																									{
																																										bool flag60 = string.Equals(name, "retry-after", StringComparison.InvariantCultureIgnoreCase);
																																										if (flag60)
																																										{
																																											result = new SIP_SingleValueHF<SIP_t_RetryAfter>("Retry-After:", new SIP_t_RetryAfter(value));
																																										}
																																										else
																																										{
																																											bool flag61 = string.Equals(name, "route", StringComparison.InvariantCultureIgnoreCase);
																																											if (flag61)
																																											{
																																												result = new SIP_MultiValueHF<SIP_t_AddressParam>("Route:", value);
																																											}
																																											else
																																											{
																																												bool flag62 = string.Equals(name, "security-client", StringComparison.InvariantCultureIgnoreCase);
																																												if (flag62)
																																												{
																																													result = new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Client:", value);
																																												}
																																												else
																																												{
																																													bool flag63 = string.Equals(name, "security-server", StringComparison.InvariantCultureIgnoreCase);
																																													if (flag63)
																																													{
																																														result = new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Server:", value);
																																													}
																																													else
																																													{
																																														bool flag64 = string.Equals(name, "security-verify", StringComparison.InvariantCultureIgnoreCase);
																																														if (flag64)
																																														{
																																															result = new SIP_MultiValueHF<SIP_t_SecMechanism>("Security-Verify:", value);
																																														}
																																														else
																																														{
																																															bool flag65 = string.Equals(name, "service-route", StringComparison.InvariantCultureIgnoreCase);
																																															if (flag65)
																																															{
																																																result = new SIP_MultiValueHF<SIP_t_AddressParam>("Service-Route:", value);
																																															}
																																															else
																																															{
																																																bool flag66 = string.Equals(name, "session-expires", StringComparison.InvariantCultureIgnoreCase);
																																																if (flag66)
																																																{
																																																	result = new SIP_SingleValueHF<SIP_t_SessionExpires>("Session-Expires:", new SIP_t_SessionExpires(value));
																																																}
																																																else
																																																{
																																																	bool flag67 = string.Equals(name, "subscription-state", StringComparison.InvariantCultureIgnoreCase);
																																																	if (flag67)
																																																	{
																																																		result = new SIP_SingleValueHF<SIP_t_SubscriptionState>("Subscription-State:", new SIP_t_SubscriptionState(value));
																																																	}
																																																	else
																																																	{
																																																		bool flag68 = string.Equals(name, "supported", StringComparison.InvariantCultureIgnoreCase);
																																																		if (flag68)
																																																		{
																																																			result = new SIP_MultiValueHF<SIP_t_OptionTag>("Supported:", value);
																																																		}
																																																		else
																																																		{
																																																			bool flag69 = string.Equals(name, "target-dialog", StringComparison.InvariantCultureIgnoreCase);
																																																			if (flag69)
																																																			{
																																																				result = new SIP_SingleValueHF<SIP_t_TargetDialog>("Target-Dialog:", new SIP_t_TargetDialog(value));
																																																			}
																																																			else
																																																			{
																																																				bool flag70 = string.Equals(name, "timestamp", StringComparison.InvariantCultureIgnoreCase);
																																																				if (flag70)
																																																				{
																																																					result = new SIP_SingleValueHF<SIP_t_Timestamp>("Timestamp:", new SIP_t_Timestamp(value));
																																																				}
																																																				else
																																																				{
																																																					bool flag71 = string.Equals(name, "to", StringComparison.InvariantCultureIgnoreCase);
																																																					if (flag71)
																																																					{
																																																						result = new SIP_SingleValueHF<SIP_t_To>("To:", new SIP_t_To(value));
																																																					}
																																																					else
																																																					{
																																																						bool flag72 = string.Equals(name, "unsupported", StringComparison.InvariantCultureIgnoreCase);
																																																						if (flag72)
																																																						{
																																																							result = new SIP_MultiValueHF<SIP_t_OptionTag>("Unsupported:", value);
																																																						}
																																																						else
																																																						{
																																																							bool flag73 = string.Equals(name, "via", StringComparison.InvariantCultureIgnoreCase);
																																																							if (flag73)
																																																							{
																																																								result = new SIP_MultiValueHF<SIP_t_ViaParm>("Via:", value);
																																																							}
																																																							else
																																																							{
																																																								bool flag74 = string.Equals(name, "warning", StringComparison.InvariantCultureIgnoreCase);
																																																								if (flag74)
																																																								{
																																																									result = new SIP_MultiValueHF<SIP_t_WarningValue>("Warning:", value);
																																																								}
																																																								else
																																																								{
																																																									bool flag75 = string.Equals(name, "www-authenticate", StringComparison.InvariantCultureIgnoreCase);
																																																									if (flag75)
																																																									{
																																																										result = new SIP_SingleValueHF<SIP_t_Challenge>("WWW-Authenticate:", new SIP_t_Challenge(value));
																																																									}
																																																									else
																																																									{
																																																										result = new SIP_HeaderField(name + ":", value);
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
			return result;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00016AD0 File Offset: 0x00015AD0
		public IEnumerator GetEnumerator()
		{
			return this.m_pHeaderFields.GetEnumerator();
		}

		// Token: 0x17000173 RID: 371
		public SIP_HeaderField this[int index]
		{
			get
			{
				return this.m_pHeaderFields[index];
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x00016B14 File Offset: 0x00015B14
		public int Count
		{
			get
			{
				return this.m_pHeaderFields.Count;
			}
		}

		// Token: 0x0400015B RID: 347
		private List<SIP_HeaderField> m_pHeaderFields = null;
	}
}
