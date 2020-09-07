using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E9 RID: 489
	public class IMAP_Search_Key_SentSince : IMAP_Search_Key
	{
		// Token: 0x060011B0 RID: 4528 RVA: 0x0006BD7C File Offset: 0x0006AD7C
		public IMAP_Search_Key_SentSince(DateTime value)
		{
			this.m_Date = value;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0006BD90 File Offset: 0x0006AD90
		internal static IMAP_Search_Key_SentSince Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SENTSINCE", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SENTSINCE' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SENTSINCE' value.");
			}
			DateTime value;
			try
			{
				value = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'SENTSINCE' value.");
			}
			return new IMAP_Search_Key_SentSince(value);
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0006BE2C File Offset: 0x0006AE2C
		public override string ToString()
		{
			return "SENTSINCE " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x0006BE60 File Offset: 0x0006AE60
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x060011B4 RID: 4532 RVA: 0x0006BE98 File Offset: 0x0006AE98
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006F4 RID: 1780
		private DateTime m_Date;
	}
}
