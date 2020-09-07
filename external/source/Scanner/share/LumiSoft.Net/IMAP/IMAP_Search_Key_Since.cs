using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001EB RID: 491
	public class IMAP_Search_Key_Since : IMAP_Search_Key
	{
		// Token: 0x060011BB RID: 4539 RVA: 0x0006C00D File Offset: 0x0006B00D
		public IMAP_Search_Key_Since(DateTime value)
		{
			this.m_Date = value;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x0006C020 File Offset: 0x0006B020
		internal static IMAP_Search_Key_Since Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SINCE", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SINCE' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SINCE' value.");
			}
			DateTime value;
			try
			{
				value = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'SINCE' value.");
			}
			return new IMAP_Search_Key_Since(value);
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x0006C0BC File Offset: 0x0006B0BC
		public override string ToString()
		{
			return "SINCE " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0006C0F0 File Offset: 0x0006B0F0
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x060011BF RID: 4543 RVA: 0x0006C128 File Offset: 0x0006B128
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006F6 RID: 1782
		private DateTime m_Date;
	}
}
