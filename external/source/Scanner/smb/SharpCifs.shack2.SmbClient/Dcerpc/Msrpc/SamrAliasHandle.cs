using System;
using SharpCifs.Smb;

namespace SharpCifs.Dcerpc.Msrpc
{
	// Token: 0x02000101 RID: 257
	public class SamrAliasHandle : Rpc.PolicyHandle
	{
		// Token: 0x060007E8 RID: 2024 RVA: 0x0002AD08 File Offset: 0x00028F08
		public SamrAliasHandle(DcerpcHandle handle, SamrDomainHandle domainHandle, int access, int rid)
		{
			MsrpcSamrOpenAlias msrpcSamrOpenAlias = new MsrpcSamrOpenAlias(domainHandle, access, rid, this);
			handle.Sendrecv(msrpcSamrOpenAlias);
			bool flag = msrpcSamrOpenAlias.Retval != 0;
			if (flag)
			{
				throw new SmbException(msrpcSamrOpenAlias.Retval, false);
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00008663 File Offset: 0x00006863
		public virtual void Close()
		{
		}
	}
}
