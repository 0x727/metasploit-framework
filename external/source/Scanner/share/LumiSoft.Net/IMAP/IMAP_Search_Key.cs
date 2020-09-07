using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D2 RID: 466
	public abstract class IMAP_Search_Key
	{
		// Token: 0x06001145 RID: 4421 RVA: 0x0006A16C File Offset: 0x0006916C
		internal static IMAP_Search_Key ParseKey(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadToFirstChar();
			bool flag2 = r.StartsWith("(", false);
			IMAP_Search_Key result;
			if (flag2)
			{
				result = IMAP_Search_Key_Group.Parse(new StringReader(r.ReadParenthesized()));
			}
			else
			{
				bool flag3 = r.StartsWith("ALL", false);
				if (flag3)
				{
					result = IMAP_Search_Key_All.Parse(r);
				}
				else
				{
					bool flag4 = r.StartsWith("ANSWERED", false);
					if (flag4)
					{
						result = IMAP_Search_Key_Answered.Parse(r);
					}
					else
					{
						bool flag5 = r.StartsWith("BCC", false);
						if (flag5)
						{
							result = IMAP_Search_Key_Bcc.Parse(r);
						}
						else
						{
							bool flag6 = r.StartsWith("BEFORE", false);
							if (flag6)
							{
								result = IMAP_Search_Key_Before.Parse(r);
							}
							else
							{
								bool flag7 = r.StartsWith("BODY", false);
								if (flag7)
								{
									result = IMAP_Search_Key_Body.Parse(r);
								}
								else
								{
									bool flag8 = r.StartsWith("CC", false);
									if (flag8)
									{
										result = IMAP_Search_Key_Cc.Parse(r);
									}
									else
									{
										bool flag9 = r.StartsWith("DELETED", false);
										if (flag9)
										{
											result = IMAP_Search_Key_Deleted.Parse(r);
										}
										else
										{
											bool flag10 = r.StartsWith("DRAFT", false);
											if (flag10)
											{
												result = IMAP_Search_Key_Draft.Parse(r);
											}
											else
											{
												bool flag11 = r.StartsWith("FLAGGED", false);
												if (flag11)
												{
													result = IMAP_Search_Key_Flagged.Parse(r);
												}
												else
												{
													bool flag12 = r.StartsWith("FROM", false);
													if (flag12)
													{
														result = IMAP_Search_Key_From.Parse(r);
													}
													else
													{
														bool flag13 = r.StartsWith("HEADER", false);
														if (flag13)
														{
															result = IMAP_Search_Key_Header.Parse(r);
														}
														else
														{
															bool flag14 = r.StartsWith("KEYWORD", false);
															if (flag14)
															{
																result = IMAP_Search_Key_Keyword.Parse(r);
															}
															else
															{
																bool flag15 = r.StartsWith("LARGER", false);
																if (flag15)
																{
																	result = IMAP_Search_Key_Larger.Parse(r);
																}
																else
																{
																	bool flag16 = r.StartsWith("NEW", false);
																	if (flag16)
																	{
																		result = IMAP_Search_Key_New.Parse(r);
																	}
																	else
																	{
																		bool flag17 = r.StartsWith("NOT", false);
																		if (flag17)
																		{
																			result = IMAP_Search_Key_Not.Parse(r);
																		}
																		else
																		{
																			bool flag18 = r.StartsWith("OLD", false);
																			if (flag18)
																			{
																				result = IMAP_Search_Key_Old.Parse(r);
																			}
																			else
																			{
																				bool flag19 = r.StartsWith("ON", false);
																				if (flag19)
																				{
																					result = IMAP_Search_Key_On.Parse(r);
																				}
																				else
																				{
																					bool flag20 = r.StartsWith("OR", false);
																					if (flag20)
																					{
																						result = IMAP_Search_Key_Or.Parse(r);
																					}
																					else
																					{
																						bool flag21 = r.StartsWith("RECENT", false);
																						if (flag21)
																						{
																							result = IMAP_Search_Key_Recent.Parse(r);
																						}
																						else
																						{
																							bool flag22 = r.StartsWith("SEEN", false);
																							if (flag22)
																							{
																								result = IMAP_Search_Key_Seen.Parse(r);
																							}
																							else
																							{
																								bool flag23 = r.StartsWith("SENTBEFORE", false);
																								if (flag23)
																								{
																									result = IMAP_Search_Key_SentBefore.Parse(r);
																								}
																								else
																								{
																									bool flag24 = r.StartsWith("SENTON", false);
																									if (flag24)
																									{
																										result = IMAP_Search_Key_SentOn.Parse(r);
																									}
																									else
																									{
																										bool flag25 = r.StartsWith("SENTSINCE", false);
																										if (flag25)
																										{
																											result = IMAP_Search_Key_SentSince.Parse(r);
																										}
																										else
																										{
																											bool flag26 = r.StartsWith("SEQSET", false);
																											if (flag26)
																											{
																												result = IMAP_Search_Key_SeqSet.Parse(r);
																											}
																											else
																											{
																												bool flag27 = r.StartsWith("SINCE", false);
																												if (flag27)
																												{
																													result = IMAP_Search_Key_Since.Parse(r);
																												}
																												else
																												{
																													bool flag28 = r.StartsWith("SMALLER", false);
																													if (flag28)
																													{
																														result = IMAP_Search_Key_Smaller.Parse(r);
																													}
																													else
																													{
																														bool flag29 = r.StartsWith("SUBJECT", false);
																														if (flag29)
																														{
																															result = IMAP_Search_Key_Subject.Parse(r);
																														}
																														else
																														{
																															bool flag30 = r.StartsWith("TEXT", false);
																															if (flag30)
																															{
																																result = IMAP_Search_Key_Text.Parse(r);
																															}
																															else
																															{
																																bool flag31 = r.StartsWith("TO", false);
																																if (flag31)
																																{
																																	result = IMAP_Search_Key_To.Parse(r);
																																}
																																else
																																{
																																	bool flag32 = r.StartsWith("UID", false);
																																	if (flag32)
																																	{
																																		result = IMAP_Search_Key_Uid.Parse(r);
																																	}
																																	else
																																	{
																																		bool flag33 = r.StartsWith("UNANSWERED", false);
																																		if (flag33)
																																		{
																																			result = IMAP_Search_Key_Unanswered.Parse(r);
																																		}
																																		else
																																		{
																																			bool flag34 = r.StartsWith("UNDELETED", false);
																																			if (flag34)
																																			{
																																				result = IMAP_Search_Key_Undeleted.Parse(r);
																																			}
																																			else
																																			{
																																				bool flag35 = r.StartsWith("UNDRAFT", false);
																																				if (flag35)
																																				{
																																					result = IMAP_Search_Key_Undraft.Parse(r);
																																				}
																																				else
																																				{
																																					bool flag36 = r.StartsWith("UNFLAGGED", false);
																																					if (flag36)
																																					{
																																						result = IMAP_Search_Key_Unflagged.Parse(r);
																																					}
																																					else
																																					{
																																						bool flag37 = r.StartsWith("UNKEYWORD", false);
																																						if (flag37)
																																						{
																																							result = IMAP_Search_Key_Unkeyword.Parse(r);
																																						}
																																						else
																																						{
																																							bool flag38 = r.StartsWith("UNSEEN", false);
																																							if (flag38)
																																							{
																																								result = IMAP_Search_Key_Unseen.Parse(r);
																																							}
																																							else
																																							{
																																								try
																																								{
																																									result = IMAP_Search_Key_SeqSet.Parse(r);
																																								}
																																								catch
																																								{
																																									throw new ParseException("Unknown search key '" + r.ReadToEnd() + "'.");
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

		// Token: 0x06001146 RID: 4422
		internal abstract void ToCmdParts(List<IMAP_Client_CmdPart> list);
	}
}
