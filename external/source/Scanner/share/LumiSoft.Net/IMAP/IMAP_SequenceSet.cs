using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x0200020B RID: 523
	[Obsolete("Use class 'IMAP_t_SeqSet' instead.")]
	public class IMAP_SequenceSet
	{
		// Token: 0x06001262 RID: 4706 RVA: 0x0006F07C File Offset: 0x0006E07C
		public IMAP_SequenceSet()
		{
			this.m_pSequenceParts = new List<Range_long>();
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x0006F0A3 File Offset: 0x0006E0A3
		public void Parse(string sequenceSetString)
		{
			this.Parse(sequenceSetString, long.MaxValue);
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x0006F0B8 File Offset: 0x0006E0B8
		public void Parse(string sequenceSetString, long seqMaxValue)
		{
			string[] array = sequenceSetString.Trim().Split(new char[]
			{
				','
			});
			foreach (string text in array)
			{
				bool flag = text.IndexOf(":") > -1;
				if (flag)
				{
					string[] array3 = text.Split(new char[]
					{
						':'
					});
					bool flag2 = array3.Length == 2;
					if (!flag2)
					{
						throw new Exception("Invalid <seq-range> '" + text + "' value !");
					}
					long num = this.Parse_Seq_Number(array3[0], seqMaxValue);
					long num2 = this.Parse_Seq_Number(array3[1], seqMaxValue);
					bool flag3 = num <= num2;
					if (flag3)
					{
						this.m_pSequenceParts.Add(new Range_long(num, num2));
					}
					else
					{
						this.m_pSequenceParts.Add(new Range_long(num2, num));
					}
				}
				else
				{
					this.m_pSequenceParts.Add(new Range_long(this.Parse_Seq_Number(text, seqMaxValue)));
				}
			}
			this.m_SequenceString = sequenceSetString;
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0006F1CC File Offset: 0x0006E1CC
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

		// Token: 0x06001266 RID: 4710 RVA: 0x0006F234 File Offset: 0x0006E234
		public string ToSequenceSetString()
		{
			return this.m_SequenceString;
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x0006F24C File Offset: 0x0006E24C
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

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06001268 RID: 4712 RVA: 0x0006F2AC File Offset: 0x0006E2AC
		public Range_long[] Items
		{
			get
			{
				return this.m_pSequenceParts.ToArray();
			}
		}

		// Token: 0x0400073C RID: 1852
		private List<Range_long> m_pSequenceParts = null;

		// Token: 0x0400073D RID: 1853
		private string m_SequenceString = "";
	}
}
