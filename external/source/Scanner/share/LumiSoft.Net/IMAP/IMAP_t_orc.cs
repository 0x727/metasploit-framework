using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200018C RID: 396
	public abstract class IMAP_t_orc
	{
		// Token: 0x0600103E RID: 4158 RVA: 0x00065248 File Offset: 0x00064248
		public static IMAP_t_orc Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadToFirstChar();
			bool flag2 = r.StartsWith("[ALERT", false);
			IMAP_t_orc result;
			if (flag2)
			{
				result = IMAP_t_orc_Alert.Parse(r);
			}
			else
			{
				bool flag3 = r.StartsWith("[BADCHARSET", false);
				if (flag3)
				{
					result = IMAP_t_orc_BadCharset.Parse(r);
				}
				else
				{
					bool flag4 = r.StartsWith("[CAPABILITY", false);
					if (flag4)
					{
						result = IMAP_t_orc_Capability.Parse(r);
					}
					else
					{
						bool flag5 = r.StartsWith("[PARSE", false);
						if (flag5)
						{
							result = IMAP_t_orc_Parse.Parse(r);
						}
						else
						{
							bool flag6 = r.StartsWith("[PERMANENTFLAGS", false);
							if (flag6)
							{
								result = IMAP_t_orc_PermanentFlags.Parse(r);
							}
							else
							{
								bool flag7 = r.StartsWith("[READ-ONLY", false);
								if (flag7)
								{
									result = IMAP_t_orc_ReadOnly.Parse(r);
								}
								else
								{
									bool flag8 = r.StartsWith("[READ-WRITE", false);
									if (flag8)
									{
										result = IMAP_t_orc_ReadWrite.Parse(r);
									}
									else
									{
										bool flag9 = r.StartsWith("[TRYCREATE", false);
										if (flag9)
										{
											result = IMAP_t_orc_TryCreate.Parse(r);
										}
										else
										{
											bool flag10 = r.StartsWith("[UIDNEXT", false);
											if (flag10)
											{
												result = IMAP_t_orc_UidNext.Parse(r);
											}
											else
											{
												bool flag11 = r.StartsWith("[UIDVALIDITY", false);
												if (flag11)
												{
													result = IMAP_t_orc_UidValidity.Parse(r);
												}
												else
												{
													bool flag12 = r.StartsWith("[UNSEEN", false);
													if (flag12)
													{
														result = IMAP_t_orc_Unseen.Parse(r);
													}
													else
													{
														bool flag13 = r.StartsWith("[APPENDUID", false);
														if (flag13)
														{
															result = IMAP_t_orc_AppendUid.Parse(r);
														}
														else
														{
															bool flag14 = r.StartsWith("[COPYUID", false);
															if (flag14)
															{
																result = IMAP_t_orc_CopyUid.Parse(r);
															}
															else
															{
																result = IMAP_t_orc_Unknown.Parse(r);
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
	}
}
