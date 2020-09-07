using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FC RID: 252
	public class MsrpcSamrOpenDomain : Samr.SamrOpenDomain
	{
		// Token: 0x060007DF RID: 2015 RVA: 0x0002ABA8 File Offset: 0x00028DA8
		public MsrpcSamrOpenDomain(SamrPolicyHandle handle, int access, Rpc.SidT sid, SamrDomainHandle domainHandle) : base(handle, access, sid, domainHandle)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
