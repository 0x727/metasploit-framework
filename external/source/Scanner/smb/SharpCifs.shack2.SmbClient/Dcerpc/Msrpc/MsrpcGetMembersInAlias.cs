using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F5 RID: 245
	public class MsrpcGetMembersInAlias : Samr.SamrGetMembersInAlias
	{
		// Token: 0x060007D8 RID: 2008 RVA: 0x0002AA36 File Offset: 0x00028C36
		public MsrpcGetMembersInAlias(SamrAliasHandle aliasHandle, Lsarpc.LsarSidArray sids) : base(aliasHandle, sids)
		{
			this.Sids = sids;
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
