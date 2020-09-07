using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001C2 RID: 450
	public class IMAP_t_SeqSet
	{
		// Token: 0x06001109 RID: 4361 RVA: 0x0006902B File Offset: 0x0006802B
		private IMAP_t_SeqSet()
		{
			this.m_pSequenceParts = new List<Range_long>();
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00069054 File Offset: 0x00068054
		public static IMAP_t_SeqSet Parse(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				throw new ArgumentNullException("value");
			}
			long maxValue = long.MaxValue;
			IMAP_t_SeqSet imap_t_SeqSet = new IMAP_t_SeqSet();
			string[] array = value.Trim().Split(new char[]
			{
				','
			});
			foreach (string text in array)
			{
				bool flag2 = text.IndexOf(":") > -1;
				if (flag2)
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					bool flag3 = array3.Length == 2;
					if (!flag3)
					{
						throw new Exception("Invalid <seq-range> '" + text + "' value !");
					}
					long num = imap_t_SeqSet.Parse_Seq_Number(array3[0], maxValue);
					long num2 = imap_t_SeqSet.Parse_Seq_Number(array3[1], maxValue);
					bool flag4 = num <= num2;
					if (flag4)
					{
						imap_t_SeqSet.m_pSequenceParts.Add(new Range_long(num, num2));
					}
					else
					{
						imap_t_SeqSet.m_pSequenceParts.Add(new Range_long(num2, num));
					}
				}
				else
				{
					imap_t_SeqSet.m_pSequenceParts.Add(new Range_long(imap_t_SeqSet.Parse_Seq_Number(text, maxValue)));
				}
			}
			imap_t_SeqSet.m_SequenceString = value;
			return imap_t_SeqSet;
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x000691A0 File Offset: 0x000681A0
		public bool Contains(long seqNumber)
		{
			foreach (Range_long range_long in this.m_pSequenceParts)
			{
				bool flag = range_long.Contains(seqNumber);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00069208 File Offset: 0x00068208
		public override string ToString()
		{
			return this.m_SequenceString;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00069220 File Offset: 0x00068220
		private long Parse_Seq_Number(string seqNumberValue, long seqMaxValue)
		{
			seqNumberValue = seqNumberValue.Trim();
			bool flag = seqNumberValue == "*";
			long result;
			if (flag)
			{
				result = seqMaxValue;
			}
			else
			{
				try
				{
					result = Convert.ToInt64(seqNumberValue);
				}
				catch
				{
					throw new Exception("Invalid <seq-number> '" + seqNumberValue + "' value !");
				}
			}
			return result;
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x0600110E RID: 4366 RVA: 0x00069280 File Offset: 0x00068280
		public Range_long[] Items
		{
			get
			{
				return this.m_pSequenceParts.ToArray();
			}
		}

		// Token: 0x040006D0 RID: 1744
		private List<Range_long> m_pSequenceParts = null;

		// Token: 0x040006D1 RID: 1745
		private string m_SequenceString = "";
	}
}
