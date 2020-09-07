using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F4 RID: 244
	public class MsrpcEnumerateAliasesInDomain : Samr.SamrEnumerateAliasesInDomain
	{
		// Token: 0x060007D7 RID: 2007 RVA: 0x0002AA08 File Offset: 0x00028C08
		public MsrpcEnumerateAliasesInDomain(SamrDomainHandle domainHandle, int acctFlags, Samr.SamrSamArray sam) : base(domainHandle, 0, acctFlags, null, 0)
		{
			this.Sam = sam;
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
