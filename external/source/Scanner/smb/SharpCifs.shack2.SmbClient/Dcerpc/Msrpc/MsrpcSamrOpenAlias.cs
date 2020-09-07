using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FB RID: 251
	public class MsrpcSamrOpenAlias : Samr.SamrOpenAlias
	{
		// Token: 0x060007DE RID: 2014 RVA: 0x0002AB81 File Offset: 0x00028D81
		public MsrpcSamrOpenAlias(SamrDomainHandle handle, int access, int rid, SamrAliasHandle aliasHandle) : base(handle, access, rid, aliasHandle)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
