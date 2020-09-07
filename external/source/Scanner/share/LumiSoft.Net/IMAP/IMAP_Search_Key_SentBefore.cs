using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E7 RID: 487
	public class IMAP_Search_Key_SentBefore : IMAP_Search_Key
	{
		// Token: 0x060011A6 RID: 4518 RVA: 0x0006BB15 File Offset: 0x0006AB15
		public IMAP_Search_Key_SentBefore(DateTime value)
		{
			this.m_Date = value;
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0006BB28 File Offset: 0x0006AB28
		internal static IMAP_Search_Key_SentBefore Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SENTBEFORE", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SENTBEFORE' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SENTBEFORE' value.");
			}
			DateTime value;
			try
			{
				value = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'SENTBEFORE' value.");
			}
			return new IMAP_Search_Key_SentBefore(value);
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0006BBC4 File Offset: 0x0006ABC4
		public override string ToString()
		{
			return "SENTBEFORE " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x0006BBF8 File Offset: 0x0006ABF8
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x060011AA RID: 4522 RVA: 0x0006BC30 File Offset: 0x0006AC30
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006F2 RID: 1778
		private DateTime m_Date;
	}
}
