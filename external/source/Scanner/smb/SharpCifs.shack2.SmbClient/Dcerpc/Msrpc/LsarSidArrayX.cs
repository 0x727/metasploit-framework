using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F2 RID: 242
	internal class LsarSidArrayX : Lsarpc.LsarSidArray
	{
		// Token: 0x060007D4 RID: 2004 RVA: 0x0002A8E4 File Offset: 0x00028AE4
		internal LsarSidArrayX(Sid[] sids)
		{
			this.NumSids = sids.Length;
			this.Sids = new Lsarpc.LsarSidPtr[sids.Length];
			for (int i = 0; i < sids.Length; i++)
			{
				this.Sids[i] = new Lsarpc.LsarSidPtr();
				this.Sids[i].Sid = sids[i];
			}
		}
	}
}
