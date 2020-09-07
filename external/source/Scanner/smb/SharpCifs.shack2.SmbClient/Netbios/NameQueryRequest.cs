using System;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D2 RID: 210
	internal class NameQueryRequest : NameServicePacket
	{
		// Token: 0x060006E6 RID: 1766 RVA: 0x000253B5 File Offset: 0x000235B5
		internal NameQueryRequest(Name name)
		{
			this.QuestionName = name;
			this.QuestionType = 32;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x000253D0 File Offset: 0x000235D0
		internal override int WriteBodyWireFormat(byte[] dst, int dstIndex)
		{
			return this.WriteQuestionSectionWireFormat(dst, dstIndex);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x000253EC File Offset: 0x000235EC
		internal override int ReadBodyWireFormat(byte[] src, int srcIndex)
		{
			return this.ReadQuestionSectionWireFormat(src, srcIndex);
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x00025408 File Offset: 0x00023608
		internal override int WriteRDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0002541C File Offset: 0x0002361C
		internal override int ReadRDataWireFormat(byte[] src, int srcIndex)
		{
			return 0;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x00025430 File Offset: 0x00023630
		public override string ToString()
		{
			return "NameQueryRequest[" + base.ToString() + "]";
		}
	}
}
