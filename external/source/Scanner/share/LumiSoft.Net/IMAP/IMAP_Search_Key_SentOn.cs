using System;
using System.Collections.Generic;
using System.Globalization;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001E8 RID: 488
	public class IMAP_Search_Key_SentOn : IMAP_Search_Key
	{
		// Token: 0x060011AB RID: 4523 RVA: 0x0006BC48 File Offset: 0x0006AC48
		public IMAP_Search_Key_SentOn(DateTime value)
		{
			this.m_Date = value;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x0006BC5C File Offset: 0x0006AC5C
		internal static IMAP_Search_Key_SentOn Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "SENTON", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'SENTON' key.");
			}
			string text = r.ReadWord();
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'SENTON' value.");
			}
			DateTime value;
			try
			{
				value = IMAP_Utils.ParseDate(text);
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'SENTON' value.");
			}
			return new IMAP_Search_Key_SentOn(value);
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0006BCF8 File Offset: 0x0006ACF8
		public override string ToString()
		{
			return "SENTON " + this.m_Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0006BD2C File Offset: 0x0006AD2C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x060011AF RID: 4527 RVA: 0x0006BD64 File Offset: 0x0006AD64
		public DateTime Date
		{
			get
			{
				return this.m_Date;
			}
		}

		// Token: 0x040006F3 RID: 1779
		private DateTime m_Date;
	}
}
