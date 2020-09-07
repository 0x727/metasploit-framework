using System;

namespace LumiSoft.Net.IMAP
{
	// Token: 0x020001FE RID: 510
	public class IMAP_r_u_Expunge : IMAP_r_u
	{
		// Token: 0x06001214 RID: 4628 RVA: 0x0006D448 File Offset: 0x0006C448
		public IMAP_r_u_Expunge(int seqNo)
		{
			bool flag = seqNo < 1;
			if (flag)
			{
				throw new ArgumentException("Arguments 'seqNo' value must be >= 1.", "seqNo");
			}
			this.m_SeqNo = seqNo;
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0006D484 File Offset: 0x0006C484
		public static IMAP_r_u_Expunge Parse(string response)
		{
			bool flag = response == null;
			if (flag)
			{
				throw new ArgumentNullException("response");
			}
			return new IMAP_r_u_Expunge(Convert.ToInt32(response.Split(new char[]
			{
				' '
			})[1]));
		}

		// Token: 0x06001216 RID: 4630 RVA: 0x0006D4C8 File Offset: 0x0006C4C8
		public override string ToString()
		{
			return "* " + this.m_SeqNo.ToString() + " EXPUNGE\r\n";
		}

		// Token: 0x17000600 RID: 1536
		// (get) Token: 0x06001217 RID: 4631 RVA: 0x0006D4F4 File Offset: 0x0006C4F4
		public int SeqNo
		{
			get
			{
				return this.m_SeqNo;
			}
		}

		// Token: 0x04000708 RID: 1800
		private int m_SeqNo = 1;
	}
}
