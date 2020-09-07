using System;
using SharpCifs.Dcerpc.Ndr;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F8 RID: 248
	public class MsrpcQueryInformationPolicy : Lsarpc.LsarQueryInformationPolicy
	{
		// Token: 0x060007DB RID: 2011 RVA: 0x0002AB11 File Offset: 0x00028D11
		public MsrpcQueryInformationPolicy(LsaPolicyHandle policyHandle, short level, NdrObject info) : base(policyHandle, level, info)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
