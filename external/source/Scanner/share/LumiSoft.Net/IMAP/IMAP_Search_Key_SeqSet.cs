using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001EA RID: 490
	public class IMAP_Search_Key_SeqSet : IMAP_Search_Key
	{
		// Token: 0x060011B5 RID: 4533 RVA: 0x0006BEB0 File Offset: 0x0006AEB0
		public IMAP_Search_Key_SeqSet(IMAP_t_SeqSet seqSet)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			this.m_pSeqSet = seqSet;
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0006BEE8 File Offset: 0x0006AEE8
		internal static IMAP_Search_Key_SeqSet Parse(StringReader r)
		{
			bool flag = r == null;
			if (flag)
			{
				throw new ArgumentNullException("r");
			}
			r.ReadToFirstChar();
			string text = r.QuotedReadToDelimiter(' ');
			bool flag2 = text == null;
			if (flag2)
			{
				throw new ParseException("Parse error: Invalid 'sequence-set' value.");
			}
			IMAP_Search_Key_SeqSet result;
			try
			{
				result = new IMAP_Search_Key_SeqSet(IMAP_t_SeqSet.Parse(text));
			}
			catch
			{
				throw new ParseException("Parse error: Invalid 'sequence-set' value.");
			}
			return result;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x0006BF5C File Offset: 0x0006AF5C
		public override string ToString()
		{
			return this.m_pSeqSet.ToString();
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x0006BF7C File Offset: 0x0006AF7C
		internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
		{
			bool flag = list == null;
			if (flag)
			{
				throw new ArgumentNullException("list");
			}
			list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant, this.ToString()));
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x060011B9 RID: 4537 RVA: 0x0006BFB4 File Offset: 0x0006AFB4
		public IMAP_t_SeqSet Value
		{
			get
			{
				return this.m_pSeqSet;
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0006BFCC File Offset: 0x0006AFCC
		[Obsolete("Use constructor 'IMAP_Search_Key_SeqSet(IMAP_t_SeqSet seqSet)' instead.")]
		public IMAP_Search_Key_SeqSet(IMAP_SequenceSet seqSet)
		{
			bool flag = seqSet == null;
			if (flag)
			{
				throw new ArgumentNullException("seqSet");
			}
			this.m_pSeqSet = IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString());
		}

		// Token: 0x040006F5 RID: 1781
		private IMAP_t_SeqSet m_pSeqSet = null;
	}
}
