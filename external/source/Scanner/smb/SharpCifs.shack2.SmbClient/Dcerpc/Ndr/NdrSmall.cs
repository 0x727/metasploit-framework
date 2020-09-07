using System;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000EF RID: 239
	public class NdrSmall : NdrObject
	{
		// Token: 0x060007CD RID: 1997 RVA: 0x0002A83F File Offset: 0x00028A3F
		public NdrSmall(int value)
		{
			this.Value = (value & 255);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0002A856 File Offset: 0x00028A56
		public override void Encode(NdrBuffer dst)
		{
			dst.Enc_ndr_small(this.Value);
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0002A866 File Offset: 0x00028A66
		public override void Decode(NdrBuffer src)
		{
			this.Value = src.Dec_ndr_small();
		}

		// Token: 0x040004FB RID: 1275
		public int Value;
	}
}
