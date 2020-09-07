using System;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D8 RID: 216
	internal class NodeStatusRequest : NameServicePacket
	{
		// Token: 0x0600073E RID: 1854 RVA: 0x00028055 File Offset: 0x00026255
		internal NodeStatusRequest(Name name)
		{
			this.QuestionName = name;
			this.QuestionType = 33;
			this.IsRecurDesired = false;
			this.IsBroadcast = false;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0002807C File Offset: 0x0002627C
		internal override int WriteBodyWireFormat(byte[] dst, int dstIndex)
		{
			int hexCode = this.QuestionName.HexCode;
			this.QuestionName.HexCode = 0;
			int result = this.WriteQuestionSectionWireFormat(dst, dstIndex);
			this.QuestionName.HexCode = hexCode;
			return result;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x000280BC File Offset: 0x000262BC
		internal override int ReadBodyWireFormat(byte[] src, int srcIndex)
		{
			return 0;
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x000280D0 File Offset: 0x000262D0
		internal override int WriteRDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x000280E4 File Offset: 0x000262E4
		internal override int ReadRDataWireFormat(byte[] src, int srcIndex)
		{
			return 0;
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x000280F8 File Offset: 0x000262F8
		public override string ToString()
		{
			return "NodeStatusRequest[" + base.ToString() + "]";
		}
	}
}
