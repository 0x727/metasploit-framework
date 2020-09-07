using System;
using System.Net;
using System.Text;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
	// Token: 0x02000184 RID: 388
	public class Mail_h_Received : MIME_h
	{
		// Token: 0x06000FA4 RID: 4004 RVA: 0x0006091C File Offset: 0x0005F91C
		public Mail_h_Received(string from, string by, DateTime time)
		{
			bool flag = from == null;
			if (flag)
			{
				throw new ArgumentNullException("from");
			}
			bool flag2 = from == string.Empty;
			if (flag2)
			{
				throw new ArgumentException("Argument 'from' value must be specified.", "from");
			}
			bool flag3 = by == null;
			if (flag3)
			{
				throw new ArgumentNullException("by");
			}
			bool flag4 = by == string.Empty;
			if (flag4)
			{
				throw new ArgumentException("Argument 'by' value must be specified.", "by");
			}
			this.m_From = from;
			this.m_By = by;
			this.m_Time = time;
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x000609FC File Offset: 0x0005F9FC
		public static Mail_h_Received Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			string[] array = value.Split(new char[]
			{
				':'
			}, 2);
			bool flag2 = array.Length != 2;
			if (flag2)
			{
				throw new ParseException("Invalid header field value '" + value + "'.");
			}
			Mail_h_Received mail_h_Received = new Mail_h_Received("a", "b", DateTime.MinValue);
			MIME_Reader mime_Reader = new MIME_Reader(array[1]);
			for (;;)
			{
				string text = mime_Reader.Word();
				bool flag3 = text == null && mime_Reader.Available == 0;
				if (flag3)
				{
					break;
				}
				bool flag4 = mime_Reader.StartsWith("(");
				if (flag4)
				{
					mime_Reader.ReadParenthesized();
				}
				else
				{
					bool flag5 = mime_Reader.StartsWith(";");
					if (flag5)
					{
						mime_Reader.Char(false);
						try
						{
							mail_h_Received.m_Time = MIME_Utils.ParseRfc2822DateTime(mime_Reader.QuotedReadToDelimiter(new char[]
							{
								';'
							}));
						}
						catch
						{
						}
					}
					else
					{
						bool flag6 = text == null;
						if (flag6)
						{
							mime_Reader.Char(true);
						}
						else
						{
							text = text.ToUpperInvariant();
							bool flag7 = text == "FROM";
							if (flag7)
							{
								mail_h_Received.m_From = mime_Reader.DotAtom();
								mime_Reader.ToFirstChar();
								bool flag8 = mime_Reader.StartsWith("(");
								if (flag8)
								{
									string[] array2 = mime_Reader.ReadParenthesized().Split(new char[]
									{
										' '
									});
									bool flag9 = array2.Length == 1;
									if (flag9)
									{
										bool flag10 = Net_Utils.IsIPAddress(array2[0]);
										if (flag10)
										{
											mail_h_Received.m_pFrom_TcpInfo = new Mail_t_TcpInfo(IPAddress.Parse(array2[0]), null);
										}
									}
									else
									{
										bool flag11 = array2.Length == 2;
										if (flag11)
										{
											bool flag12 = Net_Utils.IsIPAddress(array2[1]);
											if (flag12)
											{
												mail_h_Received.m_pFrom_TcpInfo = new Mail_t_TcpInfo(IPAddress.Parse(array2[1]), array2[0]);
											}
										}
									}
								}
							}
							else
							{
								bool flag13 = text == "BY";
								if (flag13)
								{
									mail_h_Received.m_By = mime_Reader.DotAtom();
									mime_Reader.ToFirstChar();
									bool flag14 = mime_Reader.StartsWith("(");
									if (flag14)
									{
										string[] array3 = mime_Reader.ReadParenthesized().Split(new char[]
										{
											' '
										});
										bool flag15 = array3.Length == 1;
										if (flag15)
										{
											bool flag16 = Net_Utils.IsIPAddress(array3[0]);
											if (flag16)
											{
												mail_h_Received.m_pBy_TcpInfo = new Mail_t_TcpInfo(IPAddress.Parse(array3[0]), null);
											}
										}
										else
										{
											bool flag17 = array3.Length == 2;
											if (flag17)
											{
												bool flag18 = Net_Utils.IsIPAddress(array3[1]);
												if (flag18)
												{
													mail_h_Received.m_pBy_TcpInfo = new Mail_t_TcpInfo(IPAddress.Parse(array3[1]), array3[0]);
												}
											}
										}
									}
								}
								else
								{
									bool flag19 = text == "VIA";
									if (flag19)
									{
										mail_h_Received.m_Via = mime_Reader.Word();
									}
									else
									{
										bool flag20 = text == "WITH";
										if (flag20)
										{
											mail_h_Received.m_With = mime_Reader.Word();
										}
										else
										{
											bool flag21 = text == "ID";
											if (flag21)
											{
												bool flag22 = mime_Reader.StartsWith("<");
												if (flag22)
												{
													mail_h_Received.m_ID = mime_Reader.ReadParenthesized();
												}
												else
												{
													mail_h_Received.m_ID = mime_Reader.Atom();
												}
											}
											else
											{
												bool flag23 = text == "FOR";
												if (flag23)
												{
													mime_Reader.ToFirstChar();
													bool flag24 = mime_Reader.StartsWith("<");
													if (flag24)
													{
														mail_h_Received.m_For = mime_Reader.ReadParenthesized();
													}
													else
													{
														string text2 = Mail_Utils.SMTP_Mailbox(mime_Reader);
														bool flag25 = text2 == null;
														if (flag25)
														{
															goto Block_27;
														}
														mail_h_Received.m_For = text2;
													}
												}
												else
												{
													mime_Reader.Word();
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
			mail_h_Received.m_ParseValue = value;
			return mail_h_Received;
			Block_27:
			throw new ParseException("Invalid Received: For parameter value '" + mime_Reader.ToEnd() + "'.");
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x00060DF8 File Offset: 0x0005FDF8
		public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
		{
			bool flag = reEncode || this.IsModified;
			string result;
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Received: ");
				stringBuilder.Append("from " + this.m_From);
				bool flag2 = this.m_pFrom_TcpInfo != null;
				if (flag2)
				{
					stringBuilder.Append(" (" + this.m_pFrom_TcpInfo.ToString() + ")");
				}
				stringBuilder.Append(" by " + this.m_By);
				bool flag3 = this.m_pBy_TcpInfo != null;
				if (flag3)
				{
					stringBuilder.Append(" (" + this.m_pBy_TcpInfo.ToString() + ")");
				}
				bool flag4 = !string.IsNullOrEmpty(this.m_Via);
				if (flag4)
				{
					stringBuilder.Append(" via " + this.m_Via);
				}
				bool flag5 = !string.IsNullOrEmpty(this.m_With);
				if (flag5)
				{
					stringBuilder.Append(" with " + this.m_With);
				}
				bool flag6 = !string.IsNullOrEmpty(this.m_ID);
				if (flag6)
				{
					stringBuilder.Append(" id " + this.m_ID);
				}
				bool flag7 = !string.IsNullOrEmpty(this.m_For);
				if (flag7)
				{
					stringBuilder.Append(" for " + this.m_For);
				}
				stringBuilder.Append("; " + MIME_Utils.DateTimeToRfc2822(this.m_Time));
				stringBuilder.Append("\r\n");
				result = stringBuilder.ToString();
			}
			else
			{
				result = this.m_ParseValue;
			}
			return result;
		}

		// Token: 0x17000535 RID: 1333
		// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x00060FB0 File Offset: 0x0005FFB0
		public override bool IsModified
		{
			get
			{
				return this.m_IsModified;
			}
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x00060FC8 File Offset: 0x0005FFC8
		public override string Name
		{
			get
			{
				return "Received";
			}
		}

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00060FE0 File Offset: 0x0005FFE0
		// (set) Token: 0x06000FAA RID: 4010 RVA: 0x00060FF8 File Offset: 0x0005FFF8
		public string From
		{
			get
			{
				return this.m_From;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("From");
				}
				bool flag2 = value == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Property 'From' value must be specified", "From");
				}
				this.m_From = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06000FAB RID: 4011 RVA: 0x00061048 File Offset: 0x00060048
		// (set) Token: 0x06000FAC RID: 4012 RVA: 0x00061060 File Offset: 0x00060060
		public Mail_t_TcpInfo From_TcpInfo
		{
			get
			{
				return this.m_pFrom_TcpInfo;
			}
			set
			{
				this.m_pFrom_TcpInfo = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06000FAD RID: 4013 RVA: 0x00061074 File Offset: 0x00060074
		// (set) Token: 0x06000FAE RID: 4014 RVA: 0x0006108C File Offset: 0x0006008C
		public string By
		{
			get
			{
				return this.m_By;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("By");
				}
				bool flag2 = value == string.Empty;
				if (flag2)
				{
					throw new ArgumentException("Property 'By' value must be specified", "By");
				}
				this.m_By = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06000FAF RID: 4015 RVA: 0x000610DC File Offset: 0x000600DC
		// (set) Token: 0x06000FB0 RID: 4016 RVA: 0x000610F4 File Offset: 0x000600F4
		public Mail_t_TcpInfo By_TcpInfo
		{
			get
			{
				return this.m_pBy_TcpInfo;
			}
			set
			{
				this.m_pBy_TcpInfo = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x00061108 File Offset: 0x00060108
		// (set) Token: 0x06000FB2 RID: 4018 RVA: 0x00061120 File Offset: 0x00060120
		public string Via
		{
			get
			{
				return this.m_Via;
			}
			set
			{
				this.m_Via = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x00061134 File Offset: 0x00060134
		// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0006114C File Offset: 0x0006014C
		public string With
		{
			get
			{
				return this.m_With;
			}
			set
			{
				this.m_With = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x00061160 File Offset: 0x00060160
		// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x00061178 File Offset: 0x00060178
		public string ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0006118C File Offset: 0x0006018C
		// (set) Token: 0x06000FB8 RID: 4024 RVA: 0x000611A4 File Offset: 0x000601A4
		public string For
		{
			get
			{
				return this.m_For;
			}
			set
			{
				this.m_For = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x000611B8 File Offset: 0x000601B8
		// (set) Token: 0x06000FBA RID: 4026 RVA: 0x000611D0 File Offset: 0x000601D0
		public DateTime Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
				this.m_IsModified = true;
			}
		}

		// Token: 0x04000675 RID: 1653
		private bool m_IsModified = false;

		// Token: 0x04000676 RID: 1654
		private string m_ParseValue = null;

		// Token: 0x04000677 RID: 1655
		private string m_From = "";

		// Token: 0x04000678 RID: 1656
		private Mail_t_TcpInfo m_pFrom_TcpInfo = null;

		// Token: 0x04000679 RID: 1657
		private string m_By = "";

		// Token: 0x0400067A RID: 1658
		private Mail_t_TcpInfo m_pBy_TcpInfo = null;

		// Token: 0x0400067B RID: 1659
		private string m_Via = null;

		// Token: 0x0400067C RID: 1660
		private string m_With = null;

		// Token: 0x0400067D RID: 1661
		private string m_ID = null;

		// Token: 0x0400067E RID: 1662
		private string m_For = null;

		// Token: 0x0400067F RID: 1663
		private DateTime m_Time;
	}
}
