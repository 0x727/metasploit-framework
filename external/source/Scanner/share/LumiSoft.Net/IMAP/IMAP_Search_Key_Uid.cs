using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001F0 RID: 496
	public class IMAP_Search_Key_Uid : IMAP_Search_Key
	{
		// Token: 0x060011D4 RID: 4564 RVA: 0x0006C60C File Offset: 0x0006B60C
		public IMAP_Search_Key_Uid(IMAP_t_SeqSet seqSet)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			this.m_pSeqSet = seqSet;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0006C644 File Offset: 0x0006B644
		internal static IMAP_Search_Key_Uid Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			string a = r.ReadWord();
			bool flag2 = !string.Equals(a, "UID", StringComparison.InvariantCultureIgnoreCase);
			if (flag2)
			{
				throw new ParseException("Parse error: Not a SEARCH 'UID' key.");
			}
			r.ReadToFirstChar();
			string text = r.QuotedReadToDelimiter(' ');
			bool flag3 = text == null;
			if (flag3)
			{
				throw new ParseException("Parse error: Invalid 'UID' value.");
			}
			IMAP_Search_Key_Uid result;
			try
			{
				result = new IMAP_Search_Key_Uid(IMAP_t_SeqSet.Parse(text));
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'UID' value.");
			}
			return result;
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0006C6E4 File Offset: 0x0006B6E4
		public override string ToString()
		{
			return "UID " + this.m_pSeqSet.ToString();
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0006C70C File Offset: 0x0006B70C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x060011D8 RID: 4568 RVA: 0x0006C744 File Offset: 0x0006B744
		public IMAP_t_SeqSet Value
		{
			get
			{
				return this.m_pSeqSet;
			}
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x0006C75C File Offset: 0x0006B75C
		[Obsolete("Use constructor 'IMAP_Search_Key_Uid(IMAP_t_SeqSet seqSet)' instead.")]
		public IMAP_Search_Key_Uid(IMAP_SequenceSet seqSet)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			this.m_pSeqSet = IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString());
		}

		// Token: 0x040006FB RID: 1787
		private IMAP_t_SeqSet m_pSeqSet = null;
	}
}
