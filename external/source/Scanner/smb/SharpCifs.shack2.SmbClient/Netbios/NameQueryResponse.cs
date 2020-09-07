using System;

namespace SharpCifs.Netbios
{
	// Token: 0x020000D3 RID: 211
	internal class NameQueryResponse : NameServicePacket
	{
		// Token: 0x060006EC RID: 1772 RVA: 0x00025457 File Offset: 0x00023657
		public NameQueryResponse()
		{
			this.RecordName = new Name();
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002546C File Offset: 0x0002366C
		internal override int WriteBodyWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x00025480 File Offset: 0x00023680
		internal override int ReadBodyWireFormat(byte[] src, int srcIndex)
		{
			return this.ReadResourceRecordWireFormat(src, srcIndex);
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0002549C File Offset: 0x0002369C
		internal override int WriteRDataWireFormat(byte[] dst, int dstIndex)
		{
			return 0;
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x000254B0 File Offset: 0x000236B0
		internal override int ReadRDataWireFormat(byte[] src, int srcIndex)
		{
			bool flag = this.ResultCode != 0 || this.OpCode != 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool groupName = (src[srcIndex] & 128) == 128;
				int nodeType = (src[srcIndex] & 96) >> 5;
				srcIndex += 2;
				int num = NameServicePacket.ReadInt4(src, srcIndex);
				bool flag2 = num != 0;
				if (flag2)
				{
					this.AddrEntry[this.AddrIndex] = new NbtAddress(this.RecordName, num, groupName, nodeType);
				}
				else
				{
					this.AddrEntry[this.AddrIndex] = null;
				}
				result = 6;
			}
			return result;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x00025548 File Offset: 0x00023748
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"NameQueryResponse[",
				base.ToString(),
				",addrEntry=",
				this.AddrEntry,
				"]"
			});
		}
	}
}
