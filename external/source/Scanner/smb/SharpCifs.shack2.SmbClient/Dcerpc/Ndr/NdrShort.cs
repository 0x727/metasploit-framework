using System;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000EE RID: 238
	public class NdrShort : NdrObject
	{
		// Token: 0x060007CA RID: 1994 RVA: 0x0002A809 File Offset: 0x00028A09
		public NdrShort(int value)
		{
			this.Value = (value & 255);
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0002A820 File Offset: 0x00028A20
		public override void Encode(NdrBuffer dst)
		{
			dst.Enc_ndr_short(this.Value);
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0002A830 File Offset: 0x00028A30
		public override void Decode(NdrBuffer src)
		{
			this.Value = src.Dec_ndr_short();
		}

		// Token: 0x040004FA RID: 1274
		public int Value;
	}
}
