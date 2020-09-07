using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F6 RID: 246
	public class MsrpcLookupSids : Lsarpc.LsarLookupSids
	{
		// Token: 0x060007D9 RID: 2009 RVA: 0x0002AA61 File Offset: 0x00028C61
		public MsrpcLookupSids(LsaPolicyHandle policyHandle, Sid[] sids) : base(policyHandle, new LsarSidArrayX(sids), new Lsarpc.LsarRefDomainList(), new Lsarpc.LsarTransNameArray(), 1, sids.Length)
		{
			this.sids = sids;
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}

		// Token: 0x0400050B RID: 1291
		internal Sid[] sids;
	}
}
