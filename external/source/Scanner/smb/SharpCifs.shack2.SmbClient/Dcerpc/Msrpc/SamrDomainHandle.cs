using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x02000102 RID: 258
	public class SamrDomainHandle : Rpc.PolicyHandle
	{
		// Token: 0x060007EA RID: 2026 RVA: 0x0002AD4C File Offset: 0x00028F4C
		public SamrDomainHandle(DcerpcHandle handle, SamrPolicyHandle policyHandle, int access, Rpc.SidT sid)
		{
			MsrpcSamrOpenDomain msrpcSamrOpenDomain = new MsrpcSamrOpenDomain(policyHandle, access, sid, this);
			handle.Sendrecv(msrpcSamrOpenDomain);
			bool flag = msrpcSamrOpenDomain.Retval != 0;
			if (flag)
			{
				throw new SmbException(msrpcSamrOpenDomain.Retval, false);
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00008663 File Offset: 0x00006863
		public virtual void Close()
		{
		}
	}
}
