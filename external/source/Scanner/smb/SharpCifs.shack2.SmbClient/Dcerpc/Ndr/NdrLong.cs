using System;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000EC RID: 236
	public class NdrLong : NdrObject
	{
		// Token: 0x060007C4 RID: 1988 RVA: 0x0002A7D9 File Offset: 0x000289D9
		public NdrLong(int value)
		{
			this.Value = value;
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0002A7EA File Offset: 0x000289EA
		public override void Encode(NdrBuffer dst)
		{
			dst.Enc_ndr_long(this.Value);
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0002A7FA File Offset: 0x000289FA
		public override void Decode(NdrBuffer src)
		{
			this.Value = src.Dec_ndr_long();
		}

		// Token: 0x040004F9 RID: 1273
		public int Value;
	}
}
