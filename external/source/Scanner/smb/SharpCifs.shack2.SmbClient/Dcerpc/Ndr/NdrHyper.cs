using System;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000EB RID: 235
	public class NdrHyper : NdrObject
	{
		// Token: 0x060007C1 RID: 1985 RVA: 0x0002A7A9 File Offset: 0x000289A9
		public NdrHyper(long value)
		{
			this.Value = value;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0002A7BA File Offset: 0x000289BA
		public override void Encode(NdrBuffer dst)
		{
			dst.Enc_ndr_hyper(this.Value);
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0002A7CA File Offset: 0x000289CA
		public override void Decode(NdrBuffer src)
		{
			this.Value = src.Dec_ndr_hyper();
		}

		// Token: 0x040004F8 RID: 1272
		public long Value;
	}
}
