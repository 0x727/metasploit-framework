using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000FA RID: 250
	public class MsrpcSamrConnect4 : Samr.SamrConnect4
	{
		// Token: 0x060007DD RID: 2013 RVA: 0x0002AB5B File Offset: 0x00028D5B
		public MsrpcSamrConnect4(string server, int access, SamrPolicyHandle policyHandle) : base(server, 2, access, policyHandle)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
