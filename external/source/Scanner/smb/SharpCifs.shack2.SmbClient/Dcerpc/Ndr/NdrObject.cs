using System;

namespace SharpCifs.Dcerpc.Ndr
{
	// Token: 0x020000ED RID: 237
	public abstract class NdrObject
	{
		// Token: 0x060007C7 RID: 1991
		public abstract void Encode(NdrBuffer dst);

		// Token: 0x060007C8 RID: 1992
		public abstract void Decode(NdrBuffer src);
	}
}
