using System;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x020000F9 RID: 249
	public class MsrpcSamrConnect2 : Samr.SamrConnect2
	{
		// Token: 0x060007DC RID: 2012 RVA: 0x0002AB36 File Offset: 0x00028D36
		public MsrpcSamrConnect2(string server, int access, SamrPolicyHandle policyHandle) : base(server, access, policyHandle)
		{
			this.Ptype = 0;
			this.Flags = (DcerpcConstants.DcerpcFirstFrag | DcerpcConstants.DcerpcLastFrag);
		}
	}
}
