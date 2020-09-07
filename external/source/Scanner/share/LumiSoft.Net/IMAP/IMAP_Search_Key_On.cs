using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E3 RID: 483
	public class IMAP_Search_Key_On : IMAP_Search_Key
	{
		// Token: 0x06001193 RID: 4499 RVA: 0x0006B719 File Offset: 0x0006A719
		public IMAP_Search_Key_On(DateTime value)
		{
			this.m_Date = value;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x0006B72C File Offset: 0x0006A72C
		internal static IMAP_Search_Key_On Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "ON", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'ON' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'ON' value.");
			}
			DateTime value;
			try
			{
				value = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'ON' value.");
			}
			return new IMAP_Search_Key_On(value);
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x0006B7C8 File Offset: 0x0006A7C8
		public override string ToString()
		{
			return "ON " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x0006B7FC File Offset: 0x0006A7FC
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06001197 RID: 4503 RVA: 0x0006B834 File Offset: 0x0006A834
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006EF RID: 1775
		private DateTime m_Date;
	}
}
