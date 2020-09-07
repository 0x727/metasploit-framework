using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001D6 RID: 470
	public class IMAP_Search_Key_Before : IMAP_Search_Key
	{
		// Token: 0x06001156 RID: 4438 RVA: 0x0006A9E4 File Offset: 0x000699E4
		public IMAP_Search_Key_Before(DateTime date)
		{
			this.m_Date = date;
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0006A9F8 File Offset: 0x000699F8
		internal static IMAP_Search_Key_Before Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "BEFORE", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'BEFORE' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'BEFORE' value.");
			}
			DateTime date;
			try
			{
				date = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'BEFORE' value.");
			}
			return new IMAP_Search_Key_Before(date);
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0006AA94 File Offset: 0x00069A94
		public override string ToString()
		{
			return "BEFORE " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0006AAC8 File Offset: 0x00069AC8
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x0600115A RID: 4442 RVA: 0x0006AB00 File Offset: 0x00069B00
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006E6 RID: 1766
		private DateTime m_Date;
	}
}
